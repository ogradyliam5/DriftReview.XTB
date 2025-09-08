using System;

namespace DriftReview.XTB.Models
{
    public class DriftCandidate
    {
        public DateTime ModifiedOnUtc;
        public Guid ModifiedById;
        public string ModifiedByName;       // may be empty; we’ll hydrate if needed
        public string ModifiedByType;       // "user" | "app" (best-effort)
        public int ComponentType;           // e.g., 61
        public string ComponentTypeName;    // e.g., "Web Resource"
        public Guid ObjectId;               // component's primary key
        public string SolutionUniqueName;
        public string SolutionFriendlyName;
    }

    public class DriftResolved : DriftCandidate
    {
        public string ComponentName;        // friendly name after resolution
        public string ResolvedBy;           // e.g., "webresource.name" | "fallback.guid"
    }
}
