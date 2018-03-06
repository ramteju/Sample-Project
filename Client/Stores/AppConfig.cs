using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Common
{
    //Short name for AppConfig
    public static class C
    {

        public static readonly string AppTitle = "Reactions-NG";

        #region Network Store
        public static readonly string NetworkImagesFolder = ConfigurationManager.AppSettings["NetworkImagesFolder"];
        public static readonly string NetworkFilePath8500 = ConfigurationManager.AppSettings["NetworkFilePath8500"];
        public static readonly string NetworkFilePath9000 = ConfigurationManager.AppSettings["NetworkFilePath9000"];
        public static readonly string NetworkFolderPathUserManuals = ConfigurationManager.AppSettings["UserManual"];
        public static readonly string SHAREDPATH = ConfigurationManager.AppSettings["SHARED_PATH"];
        #endregion

        public static readonly string AppType = ConfigurationManager.AppSettings["APP_TYPE"];

        public static readonly string BASE_URL = ConfigurationManager.AppSettings["rest_base_url"];

        #region Local Store
        public static readonly string LocalStoragePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Reactions");
        public static readonly string MolImagesFolder = Path.Combine(LocalStoragePath, "Images");
        public static readonly string FilePath8500 = Path.Combine(LocalStoragePath, "OrgRef8500.xml");
        public static readonly string FilePath9000 = Path.Combine(LocalStoragePath, "OrgRef9000.xml");
        public static readonly string UserManualsPath = Path.Combine(LocalStoragePath, "UserManuals");
        #endregion

        #region Foxit
        public static readonly string LICENCE_ID = "SDKAXYX9589";
        public static readonly string UNLOCK_CODE = "9406672DBE826D089AD8081CE94758E1D6F058EB";
        public static readonly string HAND_TOOL = "Hand Tool";
        public static readonly string TEXT_TOOL = "Select Text Tool";
        public static readonly string RXN_MRK_TEXT = "rxn";
        public static readonly string ANNOT_TYPE = "Typewriter";
        public static readonly string ANNOT_FONT_NAME = "Courier";
        public static readonly int ANNOT_FONT_SIZE = 12;
        public static readonly Color ANNOT_FONT_COLOR = Color.Red;
        #endregion
    }
}
