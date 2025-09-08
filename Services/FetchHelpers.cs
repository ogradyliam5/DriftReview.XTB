using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;

namespace DriftReview.XTB.Services
{
    internal static class FetchHelpers
    {
        public static IEnumerable<Entity> RetrieveMultipleAll(IOrganizationService svc, string fetchXml, int pageSize)
        {
            int page = 1; string cookie = null;
            while (true)
            {
                var fx = InjectPaging(fetchXml, page, pageSize, cookie);
                var request = new RetrieveMultipleRequest
                {
                    Query = new FetchExpression(fx)
                };
                var resp = (RetrieveMultipleResponse)svc.Execute(request);
                foreach (var e in resp.EntityCollection.Entities) yield return e;

                if (!resp.EntityCollection.MoreRecords) break;
                cookie = resp.EntityCollection.PagingCookie;
                page++;
            }
        }

        private static string InjectPaging(string baseFetch, int page, int count, string cookie)
        {
            // If caller already provided page/count, we overwrite them; add cookie if present.
            var withPage = baseFetch
                .Replace("count='PLACEHOLDER'", $"count='{count}'")
                .Replace("page='PLACEHOLDER'", $"page='{page}'");

            if (!string.IsNullOrEmpty(cookie))
            {
                // Add paging-cookie on <fetch> tag
                var insertAt = withPage.IndexOf(">");
                return withPage.Insert(insertAt, $" paging-cookie='{System.Security.SecurityElement.Escape(cookie)}'");
            }
            return withPage;
        }

        public static IEnumerable<IEnumerable<T>> Chunk<T>(IEnumerable<T> src, int size)
        {
            var list = new List<T>(size);
            foreach (var item in src)
            {
                list.Add(item);
                if (list.Count == size) { yield return list.ToArray(); list.Clear(); }
            }
            if (list.Count > 0) yield return list.ToArray();
        }
    }
}
