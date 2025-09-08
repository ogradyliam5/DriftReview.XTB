using System;

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
    }
}
