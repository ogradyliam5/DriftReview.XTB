using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace DriftReview.XTB
{
    [Serializable]
    public class DriftSettings
    {
        // Filters
        public DateTime StartUtc { get; set; } = DateTime.UtcNow.AddDays(-7);
        public DateTime EndUtc { get; set; } = DateTime.UtcNow;
        public string ExcludedNamesCsv { get; set; } = "";
        public string ExcludedUserIdsCsv { get; set; } = "";
        public string ExcludedAppIdsCsv { get; set; } = "";
        public int PageSize { get; set; } = 500;
        public int MaxRows { get; set; } = 100000;

        // Metadata delta tracking
        public string ServerVersionStamp { get; set; } = null;

        // UI
        public int SplitterDistance { get; set; } = 220;

        // Parsed views (convenience)
        public HashSet<string> ExcludedNamesSetCI =>
            ParseNamesToLowerHashSet(ExcludedNamesCsv);

        public HashSet<Guid> ExcludedUserIdsSet =>
            ParseGuidsHashSet(ExcludedUserIdsCsv);

        private static HashSet<string> ParseNamesToLowerHashSet(string csv)
        {
            if (string.IsNullOrWhiteSpace(csv)) return new HashSet<string>();
            var seps = new[] { ',', ';', '|', '\n', '\r', '\t' };
            return new HashSet<string>(
                csv.Split(seps, StringSplitOptions.RemoveEmptyEntries)
                   .Select(s => s.Trim())
                   .Where(s => s.Length > 0)
                   .Select(s => s.ToLowerInvariant())
            );
        }

        private static HashSet<Guid> ParseGuidsHashSet(string csv)
        {
            var set = new HashSet<Guid>();
            if (string.IsNullOrWhiteSpace(csv)) return set;

            var seps = new[] { ',', ';', '|', '\n', '\r', '\t' };
            foreach (var token in csv.Split(seps, StringSplitOptions.RemoveEmptyEntries))
            {
                if (Guid.TryParse(token.Trim(), out var id))
                    set.Add(id);
            }
            return set;
        }
    }
}


