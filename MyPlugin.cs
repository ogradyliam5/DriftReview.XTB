using System.ComponentModel.Composition;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Interfaces;

namespace DriftReview.XTB
{
    internal static class IconBase64
    {
        // 1x1 transparent PNG (valid Base-64, no prefix)
        public const string Pixel = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/x8AAwMCAO0N3aQAAAAASUVORK5CYII=";
    }

    [Export(typeof(IXrmToolBoxPlugin))]
    [ExportMetadata("Name", "Solution & Metadata Drift Review")]
    [ExportMetadata("Description", "Surfaces recent customization activity across all solutions (managed & unmanaged).")]
    [ExportMetadata("SmallImageBase64", IconBase64.Pixel)]
    [ExportMetadata("BigImageBase64", IconBase64.Pixel)]
    [ExportMetadata("BackgroundColor", "White")]
    [ExportMetadata("PrimaryFontColor", "Black")]
    [ExportMetadata("SecondaryFontColor", "Gray")]
    public class DriftReviewPlugin : PluginBase
    {
        public override IXrmToolBoxPluginControl GetControl() => new DriftReviewControl();
    }
}
