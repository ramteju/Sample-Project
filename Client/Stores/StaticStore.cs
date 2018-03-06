using Client.Logging;
using Client.Models;
using Client.Views;
using DTO;
using Entities;
using Entities.DTO;
using Entities.DTO.Static;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using ProductTracking.Models.Core;
using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace Client.Common
{
    public static partial class S
    {
        private static bool IsDataLoaded = false;

        private static readonly CommentDictionary commentDictionary = new CommentDictionary();
        private static readonly Dictionary<string, TanChemicalVM> AppChemicalDict = new Dictionary<string, TanChemicalVM>();
        private static readonly List<RegulerExpressionDTO> regularExpressions = new List<RegulerExpressionDTO>();
        private static readonly List<SolventBoilingPointDTO> solventBoilingPoints = new List<SolventBoilingPointDTO>();
        private static readonly List<TanChemicalVM> allSeriesNames = new List<TanChemicalVM>();
        private static readonly List<NamePriorities> NamePriorities = new List<NamePriorities>();

        public static event EventHandler<bool> DataLoadingComplete = delegate { };

        public static string VersionInfo
        {
            get
            {
                try
                {
                    string appType = C.AppType;
                    if (string.IsNullOrEmpty(appType))
                        appType = "Not Specified";
                    String appName = "Reactions - " + appType;

                    if (!appType.Equals("Production"))
                    {
                        if (ApplicationDeployment.IsNetworkDeployed)
                            return appName + " - " + ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString(4);
                        else
                            return appName + " - " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
                    }
                    return appName;
                }
                catch (Exception ex)
                {
                    Log.This(ex);
                    return String.Empty;
                }
            }
        }

        public static string Version
        {
            get
            {
                try
                {

                    if (ApplicationDeployment.IsNetworkDeployed)
                        return ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString(4);
                    else
                        return Assembly.GetExecutingAssembly().GetName().Version.ToString();
                }
                catch (Exception ex)
                {
                    Log.This(ex);
                    return String.Empty;
                }
            }
        }

        public static void CloseAllWindows()
        {
            foreach (var window in App.Current.Windows)
            {
                if (!(window is MainWindow || window is LoginWindow))
                {
                    var w = window as Window;
                    if (w.IsVisible && S.WindowsToClose.Contains(w.Name))
                    { w.Close(); Debug.WriteLine($"Window Name: {w.Name} has closed"); }
                }
            }
        }

        public static Dictionary<ParticipantType, List<CVT>> GetGroupedCVTs()
        {
            if (IsDataLoaded)
            {
                return commentDictionary.CVT.Where(cvt => cvt.IsIgnorableInDelivery).GroupBy(cvt => cvt.NewType).ToDictionary(cvt => cvt.Key, cvt => cvt.ToList());
            }
            return null;
        }

        public async static void LoadData()
        {
            IsDataLoaded = false;
            try
            {
                await GetStaticData();
                await Task.WhenAll(
                         Task.Run(() => LoadSeriesData()));
                IsDataLoaded = true;
            }
            catch (Exception ex)
            {
                Log.This(ex);
                IsDataLoaded = false;
            }
            if (IsDataLoaded)
                DataLoadingComplete.Invoke(String.Empty, true);
            else
                AppErrorBox.ShowErrorMessage("Required data is not available . .", String.Empty);
        }
        public static async Task GetStaticData()
        {
            try
            {
                regularExpressions.Clear();
                solventBoilingPoints.Clear();
                NamePriorities.Clear();

                RestStatus status = await RestHub.GetStaticData();
                if (status.UserObject != null)
                {
                    AppStaticDTO dto = (AppStaticDTO)status.UserObject;
                    regularExpressions.AddRange(dto.RegulerExpressions);
                    solventBoilingPoints.AddRange(dto.SolventBoilingPoints.OrderBy(sbp => sbp.Name).ToList());

                    commentDictionary.FreeText.Clear();
                    commentDictionary.CVT.Clear();

                    commentDictionary.FreeText = new List<FreeText>();
                    commentDictionary.FreeText = dto.CommentDictionary.Freetexts;

                    commentDictionary.CVT = new List<CVT>();
                    commentDictionary.CVT = dto.CommentDictionary.CVTs;
                    NamePriorities.AddRange(dto.NamePriorities);
                }
                else
                    AppErrorBox.ShowErrorMessage("Unable to fetch initial data . .", status.StatusMessage);
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        public static String extractText(string pdfPath)
        {
            try
            {
                using (PdfReader pdfReader = new PdfReader(pdfPath))
                {
                    if (pdfReader.NumberOfPages <= 300)
                    {
                        AppLocationTextExtractionStrategy strategy = new AppLocationTextExtractionStrategy();
                        for (int pageNumber = 1; pageNumber <= pdfReader.NumberOfPages; pageNumber++)
                        {

                            iTextSharp.text.pdf.parser.PdfTextExtractor.GetTextFromPage(pdfReader, pageNumber, strategy);
                        }
                        return strategy.textResult.ToString();
                    }
                }
                return String.Empty;
            }
            catch
            {
                return String.Empty;
            }
        }

        public static void LoadSeriesData()
        {
            try
            {
                allSeriesNames.Clear();
                Dictionary<string, string> DictNamePrioroty = NamePriorities.GroupBy(n => n.RegNumber).ToDictionary(n => n.Key, n => n.FirstOrDefault().Name);
                Dictionary<string, TanChemicalVM> registerNumWiseChemicalNameDict = new Dictionary<string, TanChemicalVM>();

                #region Series 8500                
                if (File.Exists(C.FilePath8500))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(C.NetworkFilePath8500);
                    XmlNode series8500Node = doc.DocumentElement.SelectSingleNode("/Series8500");
                    foreach (XmlNode tableNode in series8500Node.ChildNodes)
                    {
                        TanChemicalVM vm = new TanChemicalVM();
                        vm.InChiKey = tableNode.SelectSingleNode("INCHI_KEY")?.InnerText;
                        vm.RegNumber = tableNode.SelectSingleNode("REG_NO")?.InnerText;
                        vm.Name = tableNode.SelectSingleNode("ORGREF_NAME")?.InnerText;
                        vm.SearchName = vm.Name;
                        vm.ChemicalType = ChemicalType.S8500;
                        allSeriesNames.Add(vm);
                    }
                }
                else
                    AppErrorBox.ShowErrorMessage("8500 Series File is not found !", String.Empty);
                #endregion

                #region Series 9000
                if (File.Exists(C.FilePath9000))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(C.NetworkFilePath9000);
                    XmlNode series9000Node = doc.DocumentElement.SelectSingleNode("/Series9000");
                    foreach (XmlNode tableNode in series9000Node.ChildNodes)
                    {
                        TanChemicalVM vm = new TanChemicalVM();
                        var numText = tableNode.SelectSingleNode("NUM")?.InnerText;
                        int num;
                        Int32.TryParse(numText, out num);
                        vm.NUM = num;
                        vm.InChiKey = tableNode.SelectSingleNode("INCHI_KEY")?.InnerText;
                        vm.RegNumber = tableNode.SelectSingleNode("REG_NO")?.InnerText;
                        vm.Name = tableNode.SelectSingleNode("ORGREF_NAME")?.InnerText;
                        vm.SearchName = vm.Name;
                        vm.ChemicalType = ChemicalType.S9000;
                        allSeriesNames.Add(vm);
                    }
                }
                else
                    AppErrorBox.ShowErrorMessage("9000 Series File is not found !", String.Empty);
                #endregion

                //Make synonyms from chemical names having same register numbers.
                foreach (TanChemicalVM c in allSeriesNames)
                {
                    c.Name = DictNamePrioroty.ContainsKey(c.RegNumber) ? DictNamePrioroty[c.RegNumber] : c.Name;
                    if (registerNumWiseChemicalNameDict.ContainsKey(c.RegNumber))
                    {
                        TanChemicalVM existingChemicalName = registerNumWiseChemicalNameDict[c.RegNumber];
                        existingChemicalName.SearchName = String.Join("; ", existingChemicalName.SearchName, c.SearchName);
                    }
                    else
                        registerNumWiseChemicalNameDict[c.RegNumber] = c;
                }

                AppChemicalDict.Clear();
                foreach (var entry in registerNumWiseChemicalNameDict)
                    AppChemicalDict[entry.Key] = entry.Value;
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }

        }
        public static CommentDictionary CommentDictionary
        {
            get
            {
                if (IsDataLoaded)
                    return commentDictionary;
                return new CommentDictionary();
            }
        }
        public static Dictionary<string, TanChemicalVM> ChemicalDict
        {
            get
            {
                if (IsDataLoaded)
                    return AppChemicalDict;
                return new Dictionary<string, TanChemicalVM>();
            }
        }
        public static List<TanChemicalVM> AllSeriesNames
        {
            get
            {
                if (IsDataLoaded)
                    return allSeriesNames;
                return new List<TanChemicalVM>();
            }
        }
        public static List<RegulerExpressionDTO> RegularExpressions
        {
            get
            {
                if (IsDataLoaded)
                    return regularExpressions;
                return new List<RegulerExpressionDTO>();
            }
        }
        public static List<SolventBoilingPointDTO> SolventBoilingPoints
        {
            get
            {
                if (IsDataLoaded)
                    return solventBoilingPoints;
                return new List<SolventBoilingPointDTO>();
            }
        }
        public static String MolImagePath(String regNumber)
        {
            return Path.Combine(C.MolImagesFolder, regNumber + ".gif");
        }
        public static TanChemicalVM Find(String regNumber)
        {
            if (!IsDataLoaded || !AppChemicalDict.ContainsKey(regNumber))
                return null;
            return AppChemicalDict[regNumber];
        }

    }


    public class AppLocationTextExtractionStrategy : LocationTextExtractionStrategy
    {
        public int pageNumber { get; set; }
        public StringBuilder textResult { get; set; }

        public AppLocationTextExtractionStrategy()
        {
            textResult = new StringBuilder();
        }

        //Automatically called for each chunk of text in the PDF
        public override void RenderText(TextRenderInfo renderInfo)
        {
            try
            {
                base.RenderText(renderInfo);
                textResult.Append(renderInfo.GetText().ToUpper());
            }
            catch (Exception ex)
            {
                Log.This(ex);
                throw;
            }
        }
    }
}
