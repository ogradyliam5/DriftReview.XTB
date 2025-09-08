using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Xrm.Sdk;
using DriftReview.XTB.Models;

namespace DriftReview.XTB.Services
{
    internal class WebApiDataSource
    {
        private readonly IOrganizationService _svc;

        public WebApiDataSource(IOrganizationService svc) => _svc = svc;

        public List<DriftCandidate> GetCandidates(
            DateTime startUtc, DateTime endUtc,
            int pageSize, int maxRows,
            HashSet<string> excludedNamesCI,
            HashSet<Guid> excludedUserIds)
        {
            // Build Fetch for solutioncomponent + solution names
            string start = startUtc.ToString("s", CultureInfo.InvariantCulture) + "Z";
            string end = endUtc.ToString("s", CultureInfo.InvariantCulture) + "Z";

            string fetch = $@"
<fetch version='1.0' mapping='logical' no-lock='true' count='PLACEHOLDER' page='PLACEHOLDER'>
  <entity name='solutioncomponent'>
    <attribute name='modifiedon' />
    <attribute name='modifiedby' />
    <attribute name='objectid' />
    <attribute name='componenttype' />
    <attribute name='solutionid' />
    <order attribute='modifiedon' descending='true' />
    <filter type='and'>
      <condition attribute='modifiedon' operator='on-or-after' value='{start}' />
      <condition attribute='modifiedon' operator='on-or-before' value='{end}' />
    </filter>
    <link-entity name='solution' from='solutionid' to='solutionid' alias='sol' link-type='inner'>
      <attribute name='uniquename' />
      <attribute name='friendlyname' />
    </link-entity>
  </entity>
</fetch>";

            var rows = new List<DriftCandidate>();
            foreach (var e in FetchHelpers.RetrieveMultipleAll(_svc, fetch, Math.Max(1, pageSize)))
            {
                var modifiedOn = e.GetAttributeValue<DateTime?>("modifiedon")?.ToUniversalTime() ?? DateTime.MinValue;
                var modifiedBy = e.GetAttributeValue<EntityReference>("modifiedby");
                var objectId = e.GetAttributeValue<Guid?>("objectid") ?? Guid.Empty;
                var compType = e.GetAttributeValue<OptionSetValue>("componenttype")?.Value ?? 0;

                if (modifiedBy != null)
                {
                    // Exclusion by userId
                    if (excludedUserIds != null && excludedUserIds.Contains(modifiedBy.Id))
                        continue;
                    // Exclusion by display name (best-effort; sometimes Name not populated)
                    var name = (modifiedBy.Name ?? string.Empty).Trim();
                    if (!string.IsNullOrEmpty(name) && excludedNamesCI != null && excludedNamesCI.Contains(name.ToLowerInvariant()))
                        continue;
                }

                var r = new DriftCandidate
                {
                    ModifiedOnUtc = modifiedOn,
                    ModifiedById = modifiedBy?.Id ?? Guid.Empty,
                    ModifiedByName = modifiedBy?.Name ?? "",
                    ModifiedByType = "user",
                    ComponentType = compType,
                    ComponentTypeName = ComponentTypes.Name(compType),
                    ObjectId = objectId,
                    SolutionUniqueName = e.GetAttributeValue<AliasedValue>("sol.uniquename")?.Value as string ?? "",
                    SolutionFriendlyName = e.GetAttributeValue<AliasedValue>("sol.friendlyname")?.Value as string ?? ""
                };

                rows.Add(r);
                if (rows.Count >= maxRows) break;
            }
            return rows;
        }

        public List<DriftResolved> ResolveNames(IEnumerable<DriftCandidate> source, int batchSize = 200)
        {
            var list = source.ToList();
            var result = list.ConvertAll(c => new DriftResolved
            {
                ModifiedOnUtc = c.ModifiedOnUtc,
                ModifiedById = c.ModifiedById,
                ModifiedByName = c.ModifiedByName,
                ModifiedByType = c.ModifiedByType,
                ComponentType = c.ComponentType,
                ComponentTypeName = c.ComponentTypeName,
                ObjectId = c.ObjectId,
                SolutionFriendlyName = c.SolutionFriendlyName,
                SolutionUniqueName = c.SolutionUniqueName,
                ComponentName = "",
                ResolvedBy = "pending"
            });

            // group by component type
            var groups = result.GroupBy(r => r.ComponentType);
            foreach (var g in groups)
            {
                switch (g.Key)
                {
                    case 61: ResolveNamesSimple("webresource", "webresourceid", "name", g, batchSize); break;
                    case 60: ResolveNamesSimple("systemform", "formid", "name", g, batchSize); break;
                    case 80:
                        ResolveNamesMulti("appmodule", "appmoduleid", new[] { "uniquename", "name" }, g, batchSize,
                                               (e) => (e.GetAttributeValue<string>("name") ?? e.GetAttributeValue<string>("uniquename") ?? "")); break;
                    case 26: ResolveNamesSimple("savedquery", "savedqueryid", "name", g, batchSize); break;
                    case 59: ResolveNamesSimple("userquery", "userqueryid", "name", g, batchSize); break;
                    case 20: ResolveNamesSimple("role", "roleid", "name", g, batchSize); break;
                    case 29: ResolveNamesSimple("workflow", "workflowid", "name", g, batchSize); break;
                    case 31: ResolveNamesSimple("report", "reportid", "name", g, batchSize); break;
                    case 62:
                        foreach (var r in g) { r.ComponentName = "SiteMap"; r.ResolvedBy = "literal.sitemap"; }
                        break;

                    // TODO (later): metadata-based (1,2,3), env vars (380/381), conn refs (371), PCF (66), plugins (90-93)
                    default:
                        foreach (var r in g) { r.ComponentName = r.ObjectId.ToString(); r.ResolvedBy = "fallback.guid"; }
                        break;
                }
            }

            // hydrate ModifiedByName if empty (best-effort)
            TryHydrateModifierNames(result, batchSize);

            return result;
        }

        private void ResolveNamesSimple(string entity, string idAttr, string nameAttr,
            IEnumerable<DriftResolved> rows, int batchSize)
        {
            var map = new Dictionary<Guid, string>();
            foreach (var chunk in FetchHelpers.Chunk(rows.Select(r => r.ObjectId).Distinct(), batchSize))
            {
                var values = string.Join("", chunk.Select(g => $"<value uitype='{entity}'>{g:D}</value>"));
                var fetch = $@"
<fetch mapping='logical' no-lock='true' count='PLACEHOLDER' page='PLACEHOLDER'>
  <entity name='{entity}'>
    <attribute name='{idAttr}' />
    <attribute name='{nameAttr}' />
    <filter>
      <condition attribute='{idAttr}' operator='in'>{values}</condition>
    </filter>
  </entity>
</fetch>";
                foreach (var e in FetchHelpers.RetrieveMultipleAll(_svc, fetch, batchSize))
                {
                    var id = e.GetAttributeValue<Guid?>(idAttr) ?? Guid.Empty;
                    var name = e.GetAttributeValue<string>(nameAttr) ?? "";
                    if (id != Guid.Empty && !map.ContainsKey(id)) map[id] = name;
                }
            }
            foreach (var r in rows)
            {
                if (map.TryGetValue(r.ObjectId, out var n) && !string.IsNullOrEmpty(n))
                {
                    r.ComponentName = n; r.ResolvedBy = $"{entity}.{nameAttr}";
                }
                else
                {
                    r.ComponentName = r.ObjectId.ToString(); r.ResolvedBy = "fallback.guid";
                }
            }
        }

        private void ResolveNamesMulti(string entity, string idAttr, string[] nameAttrs,
            IEnumerable<DriftResolved> rows, int batchSize, Func<Entity, string> projector)
        {
            var map = new Dictionary<Guid, string>();
            foreach (var chunk in FetchHelpers.Chunk(rows.Select(r => r.ObjectId).Distinct(), batchSize))
            {
                var values = string.Join("", chunk.Select(g => $"<value uitype='{entity}'>{g:D}</value>"));
                var cols = string.Join("", nameAttrs.Select(a => $"<attribute name='{a}' />"));
                var fetch = $@"
<fetch mapping='logical' no-lock='true' count='PLACEHOLDER' page='PLACEHOLDER'>
  <entity name='{entity}'>
    <attribute name='{idAttr}' />{cols}
    <filter>
      <condition attribute='{idAttr}' operator='in'>{values}</condition>
    </filter>
  </entity>
</fetch>";
                foreach (var e in FetchHelpers.RetrieveMultipleAll(_svc, fetch, batchSize))
                {
                    var id = e.GetAttributeValue<Guid?>(idAttr) ?? Guid.Empty;
                    var display = projector(e) ?? "";
                    if (id != Guid.Empty && !map.ContainsKey(id)) map[id] = display;
                }
            }
            foreach (var r in rows)
            {
                if (map.TryGetValue(r.ObjectId, out var n) && !string.IsNullOrEmpty(n))
                {
                    r.ComponentName = n; r.ResolvedBy = $"{entity}.multi";
                }
                else
                {
                    r.ComponentName = r.ObjectId.ToString(); r.ResolvedBy = "fallback.guid";
                }
            }
        }

        private void TryHydrateModifierNames(List<DriftResolved> rows, int batchSize)
        {
            var missing = rows.Where(r => r.ModifiedById != Guid.Empty && string.IsNullOrEmpty(r.ModifiedByName))
                              .Select(r => r.ModifiedById).Distinct().ToList();
            if (missing.Count == 0) return;

            var names = new Dictionary<Guid, string>();
            foreach (var chunk in FetchHelpers.Chunk(missing, batchSize))
            {
                var values = string.Join("", chunk.Select(g => $"<value uitype='systemuser'>{g:D}</value>"));
                var fetch = $@"
<fetch mapping='logical' no-lock='true' count='PLACEHOLDER' page='PLACEHOLDER'>
  <entity name='systemuser'>
    <attribute name='systemuserid' />
    <attribute name='fullname' />
    <filter>
      <condition attribute='systemuserid' operator='in'>{values}</condition>
    </filter>
  </entity>
</fetch>";
                foreach (var e in FetchHelpers.RetrieveMultipleAll(_svc, fetch, batchSize))
                {
                    var id = e.GetAttributeValue<Guid?>("systemuserid") ?? Guid.Empty;
                    var nm = e.GetAttributeValue<string>("fullname") ?? "";
                    if (id != Guid.Empty && !names.ContainsKey(id)) names[id] = nm;
                }
            }

            foreach (var r in rows)
                if (string.IsNullOrEmpty(r.ModifiedByName) && r.ModifiedById != Guid.Empty && names.TryGetValue(r.ModifiedById, out var nm))
                    r.ModifiedByName = nm;
        }
    }
}
