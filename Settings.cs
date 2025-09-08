using System;

[Serializable]
public class DriftSettings
{
    public string ServerVersionStamp { get; set; }   // for RetrieveMetadataChanges
    public int PageSize { get; set; } = 500;
    public int MaxRows { get; set; } = 100000;
    public string ExcludedNamesCsv { get; set; }
    public string ExcludedUserIdsCsv { get; set; }
    public string ExcludedAppIdsCsv { get; set; }
}







//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace DriftReview.XTB
//{
//    /// <summary>
//    /// This class can help you to store settings for your plugin
//    /// </summary>
//    /// <remarks>
//    /// This class must be XML serializable
//    /// </remarks>
//    public class Settings
//    {
//        public string LastUsedOrganizationWebappUrl { get; set; }
//    }
//}