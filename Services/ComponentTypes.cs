using System.Collections.Generic;

namespace DriftReview.XTB.Services
{
    internal static class ComponentTypes
    {
        // Minimal map (extend as needed)
        private static readonly Dictionary<int, string> _map = new Dictionary<int, string>
        {
            { 1, "Entity" }, { 2, "Attribute" }, { 3, "Relationship" },
            { 20, "Security Role" },
            { 26, "Saved Query" },
            { 29, "Workflow" },
            { 31, "Report" },
            { 59, "User Query" },
            { 60, "System Form" },
            { 61, "Web Resource" },
            { 62, "SiteMap" },
            { 66, "Custom Control (PCF)" },
            { 70, "Field Security Profile" },
            { 80, "App Module" },
            { 90, "Plugin Type" }, { 91, "Plugin Assembly" }, { 92, "SDK Step" }, { 93, "Step Image" },
            { 300, "Canvas App" },
            { 371, "Connection Reference" },
            { 380, "Env Var Def" }, { 381, "Env Var Value" },
            { 11998, "Solution Component Definition" }
        };

        public static string Name(int type) => _map.TryGetValue(type, out var n) ? n : $"Type {type}";
    }
}
