#region NameSpaces
using DTO;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using ProductTracking.Hubs;
using ProductTracking.Models;
using ProductTracking.Models.Core;
using ProductTracking.Models.Core.ViewModels;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.IO.Compression;
using iTextSharp.text;
using iTextSharp.text.pdf;
using LinqToExcel;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Globalization;
using ProductTracking.Store;
using ProductTracking.Logging;
using System.Security.Claims;
using Microsoft.Practices.Unity;
using ProductTracking.Services.Core;
using Entities;
using ProductTracking.Filter;
using System.Threading.Tasks;
using ProductTracking.Util;
using Excelra.Utils.Library;

#endregion
namespace ProductTracking.Controllers.Core
{
    [AuthorizeRoles(Role.ToolManager, Role.ProjectManger)]
    public class ShipmentController : Controller
    {
        // private ApplicationDbContext db = new ApplicationDbContext();
        string regNumber;
        ShipmentTrace Logger = new ShipmentTrace();
        public bool bIsPBIBElement;
        [Dependency("ClaimService")]
        public ClaimService claimServices { get; set; }

        public ActionResult Index()
        {
            using (var db = new ApplicationDbContext())
            {
                return Content(db.ActivityTracing.ToList().Count + "");
            }
        }

        #region shipments
        public ActionResult Shipments(int id)
        {
            using (var db = new ApplicationDbContext())
            {
                var list = new ArrayList();
                var shipments = db.Batches.ToList();
                foreach (var shipment in shipments)
                {
                    int tanCount = (from t in db.Tans where t.Batch.Id == shipment.Id select t).Count();
                    list.Add(new
                    {
                        id = shipment.Id,
                        Name = shipment.Name,
                        ShipmentPath = shipment.DocumentsPath,
                        ShipmentDate = shipment.DateCreated.HasValue ? shipment.DateCreated.Value.ToString("dd-MM-yyyy") : null,
                        tanCount = tanCount
                    });
                }
                return Json(list, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ShipmentsCrud(ShipmentVM viewModel)
        {
            using (var db = new ApplicationDbContext())
            {
                if (viewModel.oper == "add")
                {
                    if (ModelState.IsValid)
                    {
                        Batch batch = new Batch
                        {
                            Name = viewModel.Name,
                            DateCreated = viewModel.ShipmentDate,
                            DocumentsPath = viewModel.ShipmentPath
                        };
                        db.Entry(batch).State = EntityState.Added;
                        db.SaveChanges();
                        return Content("Created");
                    }
                }

                if (viewModel.oper == "edit")
                {
                    if (ModelState.IsValid)
                    {
                        Batch batch = db.Batches.Find(Int32.Parse(HttpContext.Request.Params.Get("id")));
                        batch.Name = viewModel.Name;
                        batch.DateCreated = viewModel.ShipmentDate;
                        batch.DocumentsPath = viewModel.ShipmentPath;
                        db.Entry(batch).State = EntityState.Modified;
                        db.SaveChanges();
                        return Content("Updated");
                    }
                }

                if (viewModel.oper == "del")
                {
                    string id = HttpContext.Request.Params.Get("id");
                    if (id != null)
                    {
                        Batch batch = db.Batches.Find(Int32.Parse(id));
                        db.Batches.Remove(batch);
                        db.SaveChanges();
                        return Content("Deleted");
                    }
                }
                return Content("Invalid request");
            }
        }
        #endregion

        public ActionResult Upload()
        {
            return View();
        }

        private string GetBatchNameFromPath(string Path)
        {
            if (!string.IsNullOrEmpty(Path))
            {
                string root = Path.TrimEnd('\\').Split('\\').LastOrDefault();
                return root;
            }
            return "";
        }

        public ActionResult UploadShipment(string txtXmlPath)
        {
            string BatchNo = System.IO.Path.GetFileName(txtXmlPath);
            Logger.WriteTrace(BatchNo, "------------Upload  start");
            var startTime = DateTime.Now;
            List<Tan> tansList = new List<Tan>();
            int FreeNumber = 500000;
            bool Missing = false;
            List<TanKeywords> keyWordsList = new List<TanKeywords>();
            using (var db = new ApplicationDbContext())
            {
                keyWordsList = db.TanKeywords.ToList();
            }
            Dictionary<string, string> lstTanWiseKeywords = new Dictionary<string, string>();
            Dictionary<string, string> lstTanWiseBestFile = new Dictionary<string, string>();
            Dictionary<string, string> lstTanWiseTotalDocuments = new Dictionary<string, string>();
            //-----------------------------------------
            var context = GlobalHost.ConnectionManager.GetHubContext<LiveHub>();
            string source = txtXmlPath;
            string destination = ConfigurationManager.AppSettings.Get("SharedDirectory");
            var CheckingBatch = new Batch();
            Logger.WriteTrace(BatchNo, "------------Create Backup for Source File . .");
            context.Clients.All.progress("Create Backup for Source File . .");
            #region BackUp Section
            string strSourcePath = System.IO.Path.GetFileName(source);
            string BackUpPath = C.ShipmentSharedPath;
            Task.Run(() =>
            {
                Parallel.ForEach(Directory.GetFiles(txtXmlPath), new ParallelOptions { MaxDegreeOfParallelism = 4 }, node =>
                {
                    try
                    {
                        string DetinationDirectory = Path.Combine(BackUpPath, $"ShipmentBackUp\\{GetBatchNameFromPath(txtXmlPath)}");
                        if (!Directory.Exists(DetinationDirectory))
                            Directory.CreateDirectory(DetinationDirectory);
                        System.IO.File.Copy(node, Path.Combine(BackUpPath, $"ShipmentBackUp\\{GetBatchNameFromPath(txtXmlPath)}", Path.GetFileName(node)), true);
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteTrace(BatchNo, ex.ToString());
                    }
                });
                Logger.WriteTrace(BatchNo, "Backup Completed");
            });
            #endregion
            Logger.WriteTrace(BatchNo, "------------Extracting for IMAGES file from Source Path . .");
            context.Clients.All.progress("Extracting for IMAGES file from Source Path . .");
            #region ZipExtraction Section
            var imageZipPaths = Directory.GetFiles(source, "*.images.*");
            int FilesCount = imageZipPaths.Count();
            Parallel.ForEach(imageZipPaths, new ParallelOptions { MaxDegreeOfParallelism = FilesCount }, imageZipPath =>
            {
                try
                {
                    string strImagePath = System.IO.Path.GetFileNameWithoutExtension(imageZipPath);
                    string strImagesExtractionPath = Path.Combine(imageZipPath, txtXmlPath + "\\" + strImagePath);
                    string DetinationDirectory = Path.Combine(BackUpPath, $"ShipmentBackUp\\{GetBatchNameFromPath(txtXmlPath)}");
                    ZipFile.ExtractToDirectory(imageZipPath, txtXmlPath + "\\" + strImagePath);
                }
                catch (Exception ex)
                {
                    Logger.WriteTrace(BatchNo, ex.ToString());
                }
            });
            Logger.WriteTrace(BatchNo, "Extraction Completed Success");
            #endregion
            #region TiffToPdf And Best Pdf Selection
            ConcurrentBag<Dictionary<string, string>> TanWiseTotalDocument = new ConcurrentBag<Dictionary<string, string>>();
            ConcurrentBag<Dictionary<string, string>> TanWiseBestFiles = new ConcurrentBag<Dictionary<string, string>>();
            ConcurrentBag<Dictionary<string, string>> TanWiseKeyWords = new ConcurrentBag<Dictionary<string, string>>();
            Logger.WriteTrace(BatchNo, "Creating Pdf Files from tif Images and Selecting Best file");
            context.Clients.All.progress("Creating Pdf Files from tif Images and Selecting Best file");
            Parallel.ForEach(imageZipPaths, new ParallelOptions { MaxDegreeOfParallelism = FilesCount }, imageZipPath =>
            {
                using (ZipArchive zip = ZipFile.Open(imageZipPath, ZipArchiveMode.Read))
                {
                    string strImagePath = System.IO.Path.GetFileNameWithoutExtension(imageZipPath);
                    ImageToOdf.CreateTiffImages(txtXmlPath, zip, BackUpPath, strImagePath, TanWiseTotalDocument, TanWiseBestFiles, TanWiseKeyWords, keyWordsList);
                }
            });
            context.Clients.All.progress("Collecting all data from multiple processes");
            Logger.WriteTrace(BatchNo, "Collecting all data from multiple processes");

            foreach (var bag in TanWiseTotalDocument)
            {
                lstTanWiseTotalDocuments.AddRange(bag);
            }
            foreach (var bag in TanWiseKeyWords)
            {
                lstTanWiseKeywords.AddRange(bag);
            }
            foreach (var bag in TanWiseBestFiles)
            {
                lstTanWiseBestFile.AddRange(bag);
            }
            #endregion
            #region Commented
            //foreach (var imageZipPath in imageZipPaths)
            //{
            //    try
            //    {
            //        using (ZipArchive zip = ZipFile.Open(imageZipPath, ZipArchiveMode.Read))
            //        {
            //            string strImagePath = System.IO.Path.GetFileNameWithoutExtension(imageZipPath);
            //            string strImagesExtractionPath = Path.Combine(imageZipPath, source + "\\" + strImagePath);

            //            if (Directory.Exists(strImagesExtractionPath))
            //                Directory.Delete(strImagesExtractionPath, true);

            //            ZipFile.ExtractToDirectory(imageZipPath, source + "\\" + strImagePath);
            //            var TotalTanNumbers = zip.Entries.Select(ze => ze.FullName.Split('.')[0]).Distinct();
            //            var tifEntries = zip.Entries.Where(ze => ze.FullName.ToLower().EndsWith(".tif"));
            //            var tanNumberWiseTifs = tifEntries.GroupBy(te => te.FullName.Substring(0, 9)).ToDictionary(d => d.Key, d => d.ToList());
            //            int count = TotalTanNumbers.Count();
            //            Logger.WriteTrace(BatchNo, "------------Creating Folder for Each Tan  . .");
            //            context.Clients.All.progress("Creating Folder for Each Tan  . .");
            //            int tanCount = 0;
            //            foreach (var TanNumber in TotalTanNumbers)
            //            {
            //                try
            //                {
            //                    var TanwiseFiles = zip.Entries.Where(t => t.FullName.Contains(TanNumber) && !t.FullName.ToLower().EndsWith(".tif")).ToList();
            //                    var TanFolder = Path.Combine(destination, "ShipmentTans", strImagePath, TanNumber);
            //                    string strTiffPdfFileName = string.Empty;
            //                    context.Clients.All.progress($"Create Folder for {++tanCount} Tan {TanNumber} of {count} tans");
            //                    if (!@Directory.Exists(TanFolder))
            //                        Directory.CreateDirectory(TanFolder);
            //                    foreach (var CopyFiles in TanwiseFiles)
            //                    {
            //                        try
            //                        {
            //                            var fileName = System.IO.Path.GetFileName(CopyFiles.FullName);
            //                            var destFile = System.IO.Path.Combine(TanFolder, fileName);
            //                            System.IO.File.Copy(source + "\\" + strImagePath + "\\" + CopyFiles, destFile, true);
            //                            var TanWiseTif = tanNumberWiseTifs.Where(t => t.Key == TanNumber);
            //                            foreach (var tanNumberWiseEntry in TanWiseTif)
            //                            {
            //                                string tanNumber = tanNumberWiseEntry.Key;
            //                                strTiffPdfFileName = tanNumber + "_tiff.pdf";
            //                                string pdfPath = Path.Combine(TanFolder, strTiffPdfFileName);
            //                                using (var pdfStream = new FileStream(pdfPath, FileMode.OpenOrCreate, FileAccess.Write))
            //                                using (var doc = new Document())
            //                                {
            //                                    PdfWriter.GetInstance(doc, pdfStream);
            //                                    doc.Open();
            //                                    doc.SetMargins(20f, 20f, 20f, 20f);
            //                                    int i = 0;
            //                                    foreach (var tifEntry in tanNumberWiseEntry.Value)
            //                                    {
            //                                        using (var tifStream = tifEntry.Open())
            //                                        {
            //                                            i++;
            //                                            var docImage = Image.GetInstance(tifStream);
            //                                            docImage.ScaleToFit(doc.PageSize.Width, doc.PageSize.Height);
            //                                            doc.Add(docImage);

            //                                            if (i + 1 < tanNumberWiseEntry.Value.Count())
            //                                                doc.NewPage();
            //                                        }
            //                                    }
            //                                    doc.Close();
            //                                }
            //                            }
            //                        }
            //                        catch (Exception ex)
            //                        {
            //                            Logger.WriteTrace(BatchNo, TanNumber + "-----------" + ex.Message);
            //                            ShipmentException(TanNumber + "-----------" + ex.Message, strSourcePath, Entities.ShipmentStatus.GenerateTifPdf.ToString());
            //                        }
            //                    }
            //                    var TanWiseBestFileEntry = zip.Entries.Where(s => s.FullName.Contains(TanNumber) && s.FullName.Contains("markup")).OrderByDescending(x => x.Length).FirstOrDefault();
            //                    var TanWiseBestFile = string.Empty;

            //                    if (TanWiseBestFileEntry != null)
            //                    {
            //                        TanWiseBestFile = TanWiseBestFileEntry.FullName;
            //                    }
            //                    else if (TanWiseBestFile == string.Empty && strTiffPdfFileName != string.Empty)
            //                    {
            //                        TanWiseBestFile = strTiffPdfFileName;
            //                    }
            //                    else
            //                    {
            //                        TanWiseBestFile = zip.Entries.Where(s => s.FullName.Contains(TanNumber) && s.FullName.Contains("article") || s.FullName.Contains("patent")).OrderByDescending(x => x.Length).FirstOrDefault().FullName;
            //                    }

            //                    var TanWiseTotalDocumentsPath = zip.Entries.Where(s => s.FullName.Contains(TanNumber) && !s.FullName.Contains(".tif") && s.FullName != TanWiseBestFile && s.FullName.Contains(".pdf")).ToList();
            //                    List<string> TanwiseLocalPath = new List<string>();
            //                    foreach (var TanwiseEachDocument in TanWiseTotalDocumentsPath)
            //                    {
            //                        TanwiseLocalPath.Add(@"ShipmentTans" + "\\" + strImagePath + "\\" + TanNumber + "\\" + TanwiseEachDocument);
            //                    }
            //                    //OCR
            //                    if (TanWiseBestFile != null)
            //                    {
            //                        var destFile = System.IO.Path.Combine(@"ShipmentTans" + "\\" + strImagePath + "\\" + TanNumber, TanWiseBestFile);
            //                        //  System.IO.File.Copy(destFile, strOCRPath + "\\" + TanWiseBestFile, true);
            //                    }
            //                    //End OCR
            //                    string strTotalDocumentsPath = String.Join(",", TanwiseLocalPath);
            //                    lstTanWiseTotalDocuments.Add(TanNumber, strTotalDocumentsPath);
            //                    string TempKeywordDestnationPDF = @"ShipmentTans" + "\\" + strImagePath + "\\" + TanNumber + "\\" + TanWiseBestFile;
            //                    lstTanWiseBestFile.Add(TanNumber, TempKeywordDestnationPDF);
            //                    string strPDFfileData = string.Empty;
            //                    strPDFfileData = StaticStore.extractText(TempKeywordDestnationPDF);
            //                    if (strPDFfileData != "")
            //                    {
            //                        List<string> foundWords = new List<string>();
            //                        foreach (TanKeywords word in keyWordsList)
            //                        {
            //                            try
            //                            {
            //                                if (!String.IsNullOrEmpty(word.keyword))
            //                                {
            //                                    string upperWord = word.keyword.Trim().ToUpper();
            //                                    if (strPDFfileData.IndexOf(upperWord) > -1)
            //                                        foundWords.Add(word.keyword);
            //                                }
            //                            }
            //                            catch (Exception ex)
            //                            {
            //                                Logger.WriteTrace(BatchNo, "TanKeywords Log" + TanNumber + "-----------" + ex.Message);
            //                                ShipmentException(TanNumber + "-----------" + ex.Message, strSourcePath, Entities.ShipmentStatus.TanWiseKeyWordsList.ToString());
            //                            }

            //                        }
            //                        string concat = String.Join(",", foundWords.ToArray());
            //                        lstTanWiseKeywords.Add(TanNumber, concat);
            //                    }
            //                }
            //                catch (Exception ex)
            //                {
            //                    Log.Error(ex);
            //                    Logger.WriteTrace(BatchNo, "Main" + TanNumber + "-----------" + ex.Message);
            //                    ShipmentException(TanNumber + "-----------" + ex.Message, strSourcePath, Entities.ShipmentStatus.CreatingTanWiseFolder.ToString());
            //                }

            //            }

            //        }
            //    }
            //    catch (Exception ex)
            //    {

            //        throw;
            //    }

            //} 
            #endregion
            context.Clients.All.progress("Processing CGM File");
            var sgmlZipPaths = Directory.GetFiles(source, "*.sgml.*");
            List<TanIssues> tanIssues = new List<TanIssues>();
            foreach (var sgmlEachPath in sgmlZipPaths)
            {
                string strsgmlFilename = System.IO.Path.GetFileNameWithoutExtension(sgmlEachPath);

                var destinationSgmlpath = System.IO.Path.Combine(source, strsgmlFilename);
                if (!@Directory.Exists(destinationSgmlpath))
                    ZipFile.ExtractToDirectory(sgmlEachPath, destinationSgmlpath);

                var sgmlFilePaths = Directory.GetFiles(destinationSgmlpath);
                string strRenamedsgml = string.Empty;
                foreach (var file in sgmlFilePaths)
                {
                    try
                    {
                        Logger.WriteTrace(BatchNo, "------------Rename Sgml File  . .");
                        string strBatchFileName = System.IO.Path.GetFileName(file);
                        strRenamedsgml = Path.Combine(destinationSgmlpath, string.Concat("rxnfile.", strSourcePath, Path.GetExtension(file)));//rxnfile.batchno.cgm
                        System.IO.File.Move(file, strRenamedsgml);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);
                        Logger.WriteTrace(BatchNo, sgmlEachPath + "-----------" + ex.Message);
                        ShipmentException(sgmlEachPath + "-----------" + ex.Message, strSourcePath, Entities.ShipmentStatus.RenameSgmlFile.ToString());
                    }
                }

                txtXmlPath = strRenamedsgml;
                strSourcePath = (txtXmlPath.Substring(txtXmlPath.LastIndexOf('\\') + 1).Split('.')[2]);
                /// End Shippment

                if (!string.IsNullOrEmpty(txtXmlPath))
                {
                    try
                    {
                        ConcurrentDictionary<String, Substance> regNumberWiseSubstanceDict = new ConcurrentDictionary<string, Substance>();
                        var sgmlPath = txtXmlPath;
                        Batch batch = new Batch();
                        batch.DateCreated = System.DateTime.Now;
                        batch.DocumentsPath = "";
                        batch.GifImagesPath = "";
                        batch.Name = Convert.ToInt32(txtXmlPath.Substring(txtXmlPath.LastIndexOf('\\') + 1).Split('.')[2]);
                        Logger.WriteTrace(BatchNo, "------------Loading XML document . .");
                        context.Clients.All.progress("Loading XML document . .");
                        var xmlDoc = StaticStore.FromHtml(System.IO.File.OpenText(sgmlPath));
                        if (xmlDoc != null)
                        {
                            var casdvwNode = xmlDoc.ChildNodes.OfType<XmlElement>().Where(e => e.Name == StaticStore.DOC_ROOT).FirstOrDefault();
                            if (casdvwNode != null)
                            {
                                #region Articles
                                int articleCount = 1;
                                var articles = StaticStore.GetChilds(casdvwNode, StaticStore.ARTICLE_NODE);
                                var totalArticles = articles.Count();
                                foreach (var articleNode in articles)
                                {
                                    try
                                    {
                                        Logger.WriteTrace(BatchNo, "------------Processing Articles . . " + (articleCount++) + " / " + totalArticles);
                                        context.Clients.All.progress("Processing Articles . . " + (articleCount++) + " / " + totalArticles);
                                        Tan tan = new Models.Core.Tan();
                                        tan.Batch = batch;
                                        tan.tanNumber = StaticStore.ChildNodeText(articleNode, StaticStore.TAN_NODE, bIsPBIBElement);
                                        tan.CAN = StaticStore.ChildNodeText(articleNode, StaticStore.CAN_NODE, bIsPBIBElement);
                                        tan.DateCreated = System.DateTime.Now;
                                        tan.TanState = TanState.Not_Assigned;
                                        tan.DocumentPath = lstTanWiseBestFile.Where(str => str.Key.Contains(tan.tanNumber)).Select(t => t.Value).SingleOrDefault();
                                        tan.OCRStatus = Entities.Status.Live.ToString();
                                        tan.TotalDocumentsPath = lstTanWiseTotalDocuments.Where(t => t.Key.Contains(tan.tanNumber)).Select(s => s.Value).SingleOrDefault();
                                        var typeText = StaticStore.ChildNodeText(articleNode, StaticStore.TYPE_NODE, bIsPBIBElement);
                                        if (!String.IsNullOrEmpty(typeText))
                                        {
                                            if (typeText.IndexOf(StaticStore.JOURNAL_STRING, StringComparison.InvariantCultureIgnoreCase) > 0)
                                                tan.TanType = ArticleType.Journal.ToString();

                                            if (typeText.IndexOf(StaticStore.PATENT_STRING, StringComparison.InvariantCultureIgnoreCase) > 0)
                                                tan.TanType = ArticleType.Patent.ToString();

                                            if (typeText.IndexOf(StaticStore.CONFERENCE_STRING, StringComparison.InvariantCultureIgnoreCase) > 0)
                                                tan.TanType = ArticleType.Conference.ToString();

                                            if (typeText.IndexOf(StaticStore.PRIPRINT_STRING, StringComparison.InvariantCultureIgnoreCase) > 0)
                                                tan.TanType = ArticleType.PrePrint.ToString();
                                        }

                                        var jbibNode = StaticStore.GetChilds(articleNode, StaticStore.JBIB_NODE).FirstOrDefault();
                                        var pbibNode = StaticStore.GetChilds(articleNode, StaticStore.PBIB_NODE).FirstOrDefault();

                                        if (jbibNode != null)
                                        {
                                            tan.JournalName = StaticStore.ChildNodeText(jbibNode, StaticStore.JT_NODE, bIsPBIBElement);
                                            tan.Issue = StaticStore.ChildNodeText(jbibNode, "journal", bIsPBIBElement);
                                        }
                                        else if (pbibNode != null)
                                        {
                                            bIsPBIBElement = true;
                                            tan.JournalName = StaticStore.ChildNodeText(pbibNode, StaticStore.PBIB_JNAME_NODE, bIsPBIBElement);
                                            tan.Issue = StaticStore.ChildNodeText(pbibNode, "patent", bIsPBIBElement);
                                        }
                                        tan.JournalYear = StaticStore.ChildNodeText(articleNode, StaticStore.JournalYear_Node, bIsPBIBElement);
                                        foreach (var csieNode in StaticStore.GetChilds(articleNode, StaticStore.CSIE_NODE))
                                        {
                                            string strRegNumber = StaticStore.ChildNodeText(csieNode, StaticStore.RN_NODE, bIsPBIBElement);
                                            string strCSH_Node = StaticStore.ChildNodeText(csieNode, StaticStore.CSH_NODE, bIsPBIBElement);
                                            string strCSM_Node = StaticStore.ChildNodeText(csieNode, StaticStore.CSM_NODE, bIsPBIBElement);
                                            TanChemical tanchemical = new TanChemical();
                                            tanchemical.Id = Guid.NewGuid();
                                            if (string.IsNullOrEmpty(strRegNumber))
                                            {
                                                TanIssues issue = new TanIssues { Id = Guid.NewGuid(), IssueDescription = $"RegNumber Missed", Tan = tan, TanIssueType = TanIssueType.REG_NUM_MISSED, CreatedDate = DateTime.Now, Status = Entities.Status.New.ToString(), strCSH = strCSH_Node, strCSM = strCSM_Node, TanChemical = tanchemical };
                                                tanIssues.Add(issue);
                                            }
                                            string strNum = StaticStore.ChildNodeText(csieNode, StaticStore.NUM_NODE, bIsPBIBElement);
                                            int intNum = !string.IsNullOrEmpty(strNum) ? Int32.Parse(strNum) : 0;
                                            if (intNum == 0)
                                            {
                                                Missing = true;
                                                intNum = FreeNumber++;
                                                TanIssues issue = new TanIssues { Id = Guid.NewGuid(), IssueDescription = $"Num Not Found for {strRegNumber.Replace("-", "")}", Tan = tan, TanIssueType = TanIssueType.NUMINFORMATIONMISSED, CreatedDate = DateTime.Now, Status = Entities.Status.New.ToString(), strCSH = strCSH_Node, strCSM = strCSM_Node, RegNumber = strRegNumber.Replace("-", ""), TanChemical = tanchemical };
                                                tanIssues.Add(issue);
                                            }
                                            tanchemical.NUM = intNum;
                                            tanchemical.RegNumber = strRegNumber.Replace("-", "");
                                            tanchemical.ChemicalType = ChemicalType.NUM;
                                            tanchemical.CSH = strCSH_Node;
                                            tan.TanChemicals.Add(tanchemical);
                                            // tan.TanChemicals.Add(new TanChemical { Id = Guid.NewGuid(), NUM = intNum, RegNumber = strRegNumber.Replace("-", ""), ChemicalType = ChemicalType.NUM });
                                            //var list = new TanChemical { Id = Guid.NewGuid(), NUM = intNum, RegNumber = strRegNumber.Replace("-", ""), ChemicalType = ChemicalType.NUM };
                                        }
                                        tan.NumsCount = tan.TanChemicals.Count();
                                        if (Missing)
                                        {
                                            tan.TanStatus = TanStatus.OnHold;
                                            Missing = false;
                                        }
                                        else
                                            tan.TanStatus = TanStatus.Live;

                                        tansList.Add(tan);

                                    }
                                    catch (Exception ex)
                                    {
                                        Log.Error(ex);
                                        ShipmentException(articleNode + "-----------" + ex.Message, strSourcePath, Entities.ShipmentStatus.TanWiseArticle.ToString());
                                    }
                                }
                                #endregion
                                string strGifImageDir = ConfigurationManager.AppSettings["SharedDirectory"].ToString();
                                string strTempDirPath = Path.Combine(strGifImageDir, "NumImages", batch.Name.ToString(), "Img");
                                if (!Directory.Exists(strTempDirPath))
                                    Directory.CreateDirectory(strTempDirPath);
                                #region Substance
                                int substanceIndex = 0;
                                var substanceChildList = StaticStore.GetChilds(casdvwNode, StaticStore.SUBSTANCE_NODE);
                                var totalSubstances = substanceChildList.Count();
                                var PeptideSequence = string.Empty;
                                var NuclicAcidSequence = string.Empty;
                                string OtherName = string.Empty;
                                foreach (var substanceNode in substanceChildList)
                                {
                                    substanceIndex++;
                                    try
                                    {
                                        List<SubstanceImagePaths> listImagePaths = new List<SubstanceImagePaths>();
                                        //XmlNodeList substanceChilds;
                                        XmlNodeList substanceChilds = substanceNode.ChildNodes;
                                        int substanceChildsCount = substanceChilds.Count;
                                        List<String> hexCodes = new List<String>();
                                        if (substanceIndex % 10 == 0)
                                        {
                                            Logger.WriteTrace(BatchNo, "------------Processing Substance . ." + (substanceIndex) + " / " + totalSubstances);
                                            context.Clients.All.progress("Processing Substance . ." + (substanceIndex) + " / " + totalSubstances);
                                        }
                                        Logger.WriteTrace(BatchNo, $"------------ChildNodeText Substances {substanceChildsCount} for {substanceIndex}");
                                        for (int substacneNodeIndex = 0; substacneNodeIndex < substanceChildsCount; substacneNodeIndex++)
                                        {
                                            regNumber = StaticStore.ChildNodeText(substanceNode, StaticStore.RN_NODE, bIsPBIBElement).Replace("-", "");
                                            PeptideSequence = StaticStore.ChildNodeText(substanceNode, StaticStore.PSEQ_NODE, bIsPBIBElement);
                                            NuclicAcidSequence = StaticStore.ChildNodeText(substanceNode, StaticStore.NSEQ_NODE, bIsPBIBElement);
                                            OtherName = StaticStore.ChildNodeText(substanceNode, StaticStore.Other_NODE, bIsPBIBElement);
                                            if (substanceChilds[substacneNodeIndex].Name == StaticStore.SIM_NODE)
                                                hexCodes.Add(substanceChilds[substacneNodeIndex].InnerText);
                                            else if (substanceChilds[substacneNodeIndex].Name == StaticStore.COMP_NODE)
                                            {
                                                XmlNodeList compNodeChilds = substanceChilds[substacneNodeIndex].ChildNodes;
                                                if (compNodeChilds != null && compNodeChilds.Count > 0)
                                                {
                                                    for (int csimNodeIndex = 0; csimNodeIndex < compNodeChilds.Count; csimNodeIndex++)
                                                    {
                                                        var compChild = compNodeChilds[csimNodeIndex];
                                                        if (compChild.Name == StaticStore.CSIM_NODE)
                                                            hexCodes.Add(compChild.InnerText);
                                                    }
                                                }
                                            }
                                            int registerNumberCount = 1;
                                            foreach (var hex in hexCodes)
                                            {
                                                if (!String.IsNullOrEmpty(hex))
                                                {
                                                    try
                                                    {
                                                        var cgmFilePath = Path.GetTempPath() + Guid.NewGuid().ToString() + ".cgm";
                                                        var gifFilePath = registerNumberCount > 1 ?
                                                        Path.Combine("NumImages", batch.Name.ToString(), "Img", regNumber + "_" + registerNumberCount + ".gif") :
                                                        Path.Combine("NumImages", batch.Name.ToString(), "Img", regNumber + ".gif");
                                                        listImagePaths.Add(new Models.Core.SubstanceImagePaths { ImagePath = gifFilePath });
                                                        registerNumberCount++;
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        Log.Error(ex);
                                                        Logger.WriteTrace(BatchNo, "------------Some error Occured" + ex.Message + hex);
                                                        ShipmentException(hexCodes + "-----------" + ex.Message + hex, strSourcePath, Entities.ShipmentStatus.Hexcode.ToString());
                                                    }
                                                }
                                            }
                                        }
                                        string str = String.Empty;
                                        Substance substance = new Substance()
                                        {
                                            RegisterNumber = regNumber,
                                            Formula = StaticStore.ChildNodeText(substanceNode, StaticStore.MF_NODE, bIsPBIBElement),
                                            IUPAC = StaticStore.ChildNodeText(substanceNode, StaticStore.IN_NODE, bIsPBIBElement),
                                            StereoMessage = StaticStore.ChildNodeText(substanceNode, StaticStore.STE_NODE, bIsPBIBElement),
                                            PeptideSequence = PeptideSequence,
                                            NuclicAcid = NuclicAcidSequence,
                                            OtherName = OtherName,
                                            ListImagepaths = listImagePaths
                                        };
                                        regNumberWiseSubstanceDict[regNumber] = substance;
                                    }
                                    catch (Exception ex)
                                    {
                                        ShipmentException(substanceNode + "-----------" + ex.StackTrace, strSourcePath, Entities.ShipmentStatus.Substance.ToString());
                                        ShipmentException(substanceNode + "-----------" + ex.Message, strSourcePath, Entities.ShipmentStatus.Substance + "---Message".ToString());
                                    }
                                }
                                #endregion
                            }
                        }

                        #region Update Tans Details
                        Logger.WriteTrace(BatchNo, "------------Updating TAN Details");
                        foreach (var tan in tansList)
                        {
                            try
                            {
                                var existingList = tan.TanChemicals.ToList();
                                foreach (var tanChemical in existingList)
                                {
                                    if (regNumberWiseSubstanceDict.ContainsKey(tanChemical.RegNumber))
                                    {
                                        var substance = regNumberWiseSubstanceDict[tanChemical.RegNumber];
                                        tanChemical.Formula = substance.Formula;
                                        tanChemical.Name = substance.IUPAC;
                                        tanChemical.ABSSterio = substance.StereoMessage;
                                        tanChemical.PeptideSequence = substance.PeptideSequence;
                                        tanChemical.NuclicAcidSequence = substance.NuclicAcid;
                                        tanChemical.OtherName = substance.OtherName;
                                        var imagePaths = new List<SubstanceImagePaths>();

                                        foreach (var item in substance.ListImagepaths)
                                        {
                                            var path = new SubstanceImagePaths { TanChemical = tanChemical, ImagePath = item.ImagePath };
                                            imagePaths.Add(path);
                                        }
                                        tanChemical.Substancepaths = imagePaths;
                                    }
                                    else
                                        tanChemical.Name = "Register Not Found";
                                }
                            }
                            catch (Exception ex)
                            {
                                Log.Error(ex);
                                ShipmentException(tan.tanNumber + "-----------" + ex.Message, strSourcePath, Entities.ShipmentStatus.UpdatingTan.ToString());
                            }
                        }
                        #endregion
                        using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(30)))
                        {
                            using (var dbx = new ApplicationDbContext())
                            {
                                Logger.WriteTrace(BatchNo, "------------Saving Batch . . ");
                                context.Clients.All.progress("Saving Batch . .");
                                dbx.Batches.Add(batch);
                                dbx.SaveChanges();

                                context.Clients.All.progress("Checking Duplicate Tans . .");
                                #region Checking Duplicating Tans While inserting
                                try
                                {
                                    var tempTansTempList = new List<Tan>();
                                    var DuplicateTanList = new List<Tan>();
                                    DuplicateTanList = tansList.ToList().Where(x => dbx.Tans.Where(t => t.tanNumber == x.tanNumber).Any()).ToList();
                                    DuplicateTanList.ForEach(t => t.IsDuplicate = "Y");
                                    var tempTanCheck = new List<Tan>();
                                    tempTanCheck = tansList.Except(DuplicateTanList).ToList();
                                    tansList = tempTanCheck.Union(DuplicateTanList).ToList();
                                }
                                catch (Exception ex)
                                {
                                    Log.Error(ex);
                                    ShipmentException(ex.Message, strSourcePath, Entities.ShipmentStatus.DuplicateTan.ToString());
                                }
                                #endregion
                                context.Clients.All.progress("Saving TANs . .");
                                Logger.WriteTrace(BatchNo, "------------Saving TANs . . ");
                                var tempTansList = new List<Tan>();
                                for (int i = 0; i < tansList.Count; i++)
                                {
                                    try
                                    {
                                        tempTansList.Add(tansList[i]);
                                        if (i % 50 == 0)
                                        {
                                            dbx.Tans.AddRange(tempTansList);
                                            tempTansList.Clear();
                                            dbx.SaveChanges();
                                            //    context.Clients.All.progress("Saving TANs . ." + (i + 1));
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Log.Error(ex);
                                        Logger.WriteTrace(BatchNo, tansList[i].tanNumber + "-----------" + ex.InnerException.ToString());
                                        ShipmentException(tansList[i].tanNumber + "-----------" + ex.InnerException.ToString(), strSourcePath, Entities.ShipmentStatus.SaveTan.ToString());
                                    }
                                }
                                try
                                {
                                    dbx.Tans.AddRange(tempTansList);
                                    dbx.SaveChanges();
                                }
                                catch (Exception ex)
                                {
                                    Log.Error(ex);
                                    Logger.WriteTrace(BatchNo, "-----------" + ex.InnerException.ToString());
                                    ShipmentException("-----------" + ex.InnerException.ToString(), strSourcePath, Entities.ShipmentStatus.SaveTan.ToString());
                                }
                                Logger.WriteTrace(BatchNo, "------------Saving TAN Data . .");
                                context.Clients.All.progress("Saving TAN Data . .");
                                var tempTanDataList = new List<Entities.TanData>();
                                for (int i = 0; i < tansList.Count; i++)
                                {
                                    try
                                    {
                                        var tan = tansList[i];
                                        tempTanDataList.Add(new Entities.TanData { Data = JsonConvert.SerializeObject(tan), Tan = tan });
                                        if (i % 50 == 0)
                                        {
                                            dbx.TanData.AddRange(tempTanDataList);
                                            tempTanDataList.Clear();
                                            dbx.SaveChanges();
                                            context.Clients.All.progress("Saving TAN Datas . ." + (i + 1));
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Log.Error(ex);
                                        Logger.WriteTrace(BatchNo, "------------Saving TAN Data . ." + tansList[i].tanNumber + "-----------" + ex.ToString());
                                        ShipmentException(tansList[i].tanNumber + "-----------" + ex.Message, strSourcePath, Entities.ShipmentStatus.SavingTanDatas.ToString());
                                    }
                                }
                                Logger.WriteTrace(BatchNo, "------------End Saving TAN Data . .");
                                Logger.WriteTrace(BatchNo, "------------Saving TAN Keywords . .");
                                dbx.TanData.AddRange(tempTanDataList);
                                var tempTanwiseKeywords = new List<TanWiseKeywords>();
                                for (int i = 0; i < tansList.Count; i++)
                                {
                                    var tan = tansList[i];
                                    for (int j = 0; j < lstTanWiseKeywords.Count; j++)
                                    {
                                        try
                                        {
                                            var Tankeyword = lstTanWiseKeywords.Where(k => k.Key == tan.tanNumber).Select(s => s.Value).SingleOrDefault();
                                            tempTanwiseKeywords.Add(new TanWiseKeywords { TanKeywords = Tankeyword, Tan = tan });
                                            if (j % 50 == 0)
                                            {
                                                dbx.TanWiseKeywords.AddRange(tempTanwiseKeywords);
                                                tempTanwiseKeywords.Clear();
                                                dbx.SaveChanges();
                                                context.Clients.All.progress("Saving Tan Keywords . ." + tan.tanNumber);
                                                System.IO.File.AppendAllText(destination + "\\TanwiseKeyWordSearch.txt", tan.tanNumber + "------------" + Tankeyword + Environment.NewLine);
                                                break;
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Log.Error(ex);
                                            Logger.WriteTrace(BatchNo, "------------Saving TAN Keywords . ." + tansList[i].tanNumber + "-----------" + ex.ToString());
                                            ShipmentException(tansList[i].tanNumber + "-----------" + ex.Message, strSourcePath, Entities.ShipmentStatus.SavingTanKeyWords.ToString());
                                        }
                                    }
                                }
                                Logger.WriteTrace(BatchNo, "------------TAN Keywords saved successfull. .");

                                Entities.ShippmentUploadStatus shippmentUploadstatus = new Entities.ShippmentUploadStatus();
                                shippmentUploadstatus.BatchNo = Convert.ToInt32(txtXmlPath.Substring(txtXmlPath.LastIndexOf('\\') + 1).Split('.')[2]).ToString();
                                shippmentUploadstatus.DateCreated = DateTime.Now;
                                shippmentUploadstatus.NumImagesPath = ConfigurationManager.AppSettings["SharedDirectory"].ToString() + @"\NumImages";
                                shippmentUploadstatus.Path = txtXmlPath;
                                shippmentUploadstatus.Status = Entities.ShippmentUploadEnumStatus.ProcessCompleted.ToString();
                                dbx.ShippmentUploadStatus.Add(shippmentUploadstatus);
                                dbx.SaveChanges();
                                /// OCR Strart
                                ////Entities.ShippmentOCR shippmentOcr = new Entities.ShippmentOCR();
                                ////shippmentOcr.OCRPath = strOCRPath;
                                ////shippmentOcr.Shipment = batch.Id.ToString();
                                ////shippmentOcr.Status = Entities.Status.Live.ToString();
                                ////shippmentOcr.Datecreated = DateTime.Now;
                                ////dbx.ShippmentOCR.Add(shippmentOcr);
                                ////dbx.SaveChanges();
                                dbx.TanWiseKeywords.AddRange(tempTanwiseKeywords);
                                dbx.TanIssues.AddRange(tanIssues);
                                dbx.SaveChanges();
                            }
                            scope.Complete();
                            List<string> TanNumbers = new List<string>();
                            TanNumbers = tansList.Select(t => t.tanNumber).ToList();
                            StaticStore.SendMail(TanNumbers, batch.Name, tanIssues, (DateTime.Now - startTime));
                            context.Clients.All.progress("Shipment upload completed successfully");
                            Logger.WriteTrace(BatchNo, "Shipment Import Process Completed.");
                            return Content("Shipment upload completed.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteTrace(BatchNo, txtXmlPath + "-----------" + ex.Message);
                        ShipmentException(txtXmlPath + "-----------" + ex.Message, strSourcePath, Entities.ShipmentStatus.CGMFile.ToString());
                        return Content(ex.Message);
                    }
                    finally
                    {
                        context.Clients.All.progress("Shipment upload completed.");
                    }

                }
            }
            //}
            //else
            //{
            //    Logger.WriteTrace(txtXmlPath + "-----------already Exists");
            //    return Content("Shipment Already Exist.");
            //}
            return View();
        }

        public void ShipmentException(string Message, string BatchNo, string StackTrace)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(10)))
            {
                using (var dbx = new ApplicationDbContext())
                {
                    Entities.ShipmentException SE = new Entities.ShipmentException();
                    SE.BatchNo = BatchNo;
                    SE.Message = Message;
                    SE.StackTrace = StackTrace;
                    SE.Status = Entities.Status.New.ToString();
                    SE.DateCreated = DateTime.Now;
                    dbx.ShipmentExceptions.Add(SE);
                    dbx.SaveChanges();
                    scope.Complete();
                }
            }
        }

        public ActionResult ShipmentmissingNodes()
        {
            return View();
        }

        public ActionResult ShipmentmissingNodesList()
        {
            using (var db = new ApplicationDbContext())
            {

                try
                {
                    Logger.WriteTrace("Other", "Missing Shipment Node View Start");
                    ViewBag.tanIssues = db.TanIssues.Include(c => c.Tan.Batch).Include(b => b.Tan.TanChemicals).Where(t => t.Status != Entities.Status.close.ToString()).ToList();
                    return PartialView("~/Views/Shipment/_ShipmentmissingNodes.cshtml");
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                    throw;
                }
            }
        }

        public ActionResult ShipmentMissingSingleNode(Guid Id)
        {
            using (var db = new ApplicationDbContext())
            {

                var list = new ArrayList();
                try
                {
                    Logger.WriteTrace("Other", "Get Single Tan for Edit View");
                    TanIssues tanIssues = db.TanIssues.Where(t => t.Id == Id).FirstOrDefault();
                    list.Add(new { IssueDescription = tanIssues.IssueDescription, Id = tanIssues.Id, tannumber = tanIssues.Tan.tanNumber, CSM = tanIssues.strCSM, CSH = tanIssues.strCSH, RegNumber = tanIssues.RegNumber, Num = tanIssues.Tan.TanChemicals.Where(x => x.RegNumber == tanIssues.RegNumber && x.Id == tanIssues.TanChemical.Id).Select(t => t.NUM) });
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                    throw;
                }
                return Json(list, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult UpdateShipmentMissingNode()
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(30)))
            {
                using (var db = new ApplicationDbContext())
                {
                    try
                    {
                        Logger.WriteTrace("Other", "Update Shipment Node Start");
                        string tannumber = Request.Form[1].ToString();
                        string Id = Request.Form[0];
                        string RegNumber = Request.Form[3];
                        var id = claimServices.UserId((ClaimsIdentity)User.Identity);
                        TanIssues tanIssue = db.TanIssues.Where(t => t.Id.ToString() == Id).FirstOrDefault();
                        var tan = db.Tans.Where(t => t.tanNumber == tannumber).FirstOrDefault();
                        if (tan != null)
                        {
                            var tandata = db.TanData.Where(t => t.TanId == tan.Id).FirstOrDefault();
                            Tan serialisedtan = JsonConvert.DeserializeObject<Tan>(tandata.Data);
                            TanChemical tanchemical = serialisedtan.TanChemicals.Where(tc => tc.Id == tanIssue.TanChemical.Id).FirstOrDefault();
                            tanchemical.NUM = Convert.ToInt32(Request.Form[2]);
                            tandata.Data = JsonConvert.SerializeObject(serialisedtan);
                        }

                        tanIssue.Status = Entities.Status.Live.ToString();
                        tanIssue.UpdatedDate = DateTime.Now;
                        tanIssue.LastUpdatedUser = id;
                        TanChemical Rdtanchemical = db.TanChemicals.Where(t => t.Tan.tanNumber == tannumber && t.RegNumber == RegNumber && t.Id == tanIssue.TanChemical.Id).FirstOrDefault();
                        Rdtanchemical.NUM = Convert.ToInt32(Request.Form[2]);
                        db.SaveChanges();
                        ViewBag.tanIssues = db.TanIssues.Include(c => c.Tan.Batch).Include(b => b.Tan.TanChemicals).Where(t => t.Status != Entities.Status.close.ToString()).ToList();
                        scope.Complete();
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);
                        throw;
                    }
                }
            }
            return PartialView("~/Views/Shipment/_ShipmentmissingNodes.cshtml");
        }

        public ActionResult ShipmentMissingTanLive(string Id)
        {

            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(30)))
            {
                using (var dbx = new ApplicationDbContext())
                {
                    try
                    {
                        List<TanIssues> TanIssueList = new List<TanIssues>();
                        var id = claimServices.UserId((ClaimsIdentity)User.Identity);
                        Logger.WriteTrace("Other", $"TanIssue {Id}...");
                        TanIssues tanIssues = dbx.TanIssues.Where(t => t.Id.ToString() == Id).FirstOrDefault();

                        if (dbx.TanIssues.Where(t => t.Tan.Id == tanIssues.Tan.Id && t.Status == Entities.Status.New.ToString()).Count() == 0)
                        {

                            Tan tan = dbx.Tans.Include("ApplicationUser").Include("Batch").Include("Workflow").Where(t => t.tanNumber == tanIssues.Tan.tanNumber).FirstOrDefault();
                            tan.TanStatus = TanStatus.Live;

                            dbx.SaveChanges();
                            Logger.WriteTrace("Other", "Update Shipment Node Start");
                            TanIssueList = dbx.TanIssues.Where(t => t.Tan.Id == tanIssues.Tan.Id).ToList();
                            TanIssueList.ForEach(a => { a.Status = Entities.Status.close.ToString(); a.LastUpdatedUser = id; });
                            dbx.SaveChanges();
                            ViewBag.tanIssues = dbx.TanIssues.Where(t => t.Status != Entities.Status.close.ToString()).ToList();

                            scope.Complete();
                            return Content($"This Tan is Live");
                        }
                        else
                        {
                            return Content($"{tanIssues.Tan.tanNumber} Some records avaliable");
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);
                        throw;
                    }
                }
            }
        }

        public ActionResult Deleteshipment(Guid id)
        {
            try
            {
                if (id != null)
                {
                    using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(30)))
                    {
                        using (var dbx = new ApplicationDbContext())
                        {
                            TanIssues tankeyword = dbx.TanIssues.Where(x => x.Id == id).FirstOrDefault();
                            dbx.TanIssues.Remove(tankeyword);
                            dbx.SaveChanges();
                            scope.Complete();
                        }
                    }
                }

                return RedirectPermanent("~/Shipment/ShipmentmissingNodes");
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }



        }



        public ActionResult ShipmentSummary()
        {
            using (var db = new ApplicationDbContext())
            {
                ViewBag.batches = db.Batches.OrderByDescending(b => b.DateCreated).ToList();
                ViewBag.batchWiseTans = (from t in db.Tans group t by t.Batch.Id into batchWise select new { shipmentId = batchWise.Key, tans = batchWise.Count() })
                    .ToDictionary(d => d.shipmentId, d => d.tans);
                return PartialView("~/Views/Shipment/_shipmentSummary.cshtml");
            }
        }

        public ActionResult SelectedShipments(int ToBatchId, int FromBatchId)
        {
            using (var db = new ApplicationDbContext())
            {
                ViewBag.batches = db.Batches.Where(b => b.Name >= FromBatchId && b.Name <= ToBatchId).ToList();
                ViewBag.batchWiseTans = (from t in db.Tans where t.Batch.Name >= FromBatchId && t.Batch.Name <= ToBatchId group t by t.Batch.Id into batchWise select new { shipmentId = batchWise.Key, tans = batchWise.Count() })
                    .ToDictionary(d => d.shipmentId, d => d.tans);
                ViewBag.batchUpdatedTans = null;
                return PartialView("~/Views/Shipment/_TargetedDateShipmentsList.cshtml");
            }
        }

        public ActionResult TargetedDate()
        {
            using (var db = new ApplicationDbContext())
            {

                try
                {
                    Logger.WriteTrace("Other", "Shippments TargetDate Loading Start");
                    ViewBag.batches = db.Batches.OrderByDescending(b => b.DateCreated).ToList();
                    return View();
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                    throw;
                }
            }
        }

        public ActionResult ShipmentExceptionDetails()
        {
            using (var db = new ApplicationDbContext())
            {

                try
                {
                    Logger.WriteTrace("Other", "Shippments ShipmentException Loading Start");
                    ViewBag.batches = db.Batches.OrderByDescending(b => b.DateCreated).ToList();
                    return View();
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                    throw;
                }
            }
        }

        public ActionResult ShippmentTansList(int ToBatchId, int FromBatchId)
        {
            using (var db = new ApplicationDbContext())
            {

                try
                {
                    Logger.WriteTrace("Other", "Shipment Tans List Start");
                    ViewBag.batchWiseTans = db.Tans.Where(x => x.Batch.Name >= FromBatchId && x.Batch.Name <= ToBatchId).ToList();
                    return PartialView("~/Views/Shipment/_ShippmentTansList.cshtml");
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                    throw;
                }
            }
        }

        public ActionResult ShipmentExceptionlist(string BatchNo)
        {
            using (var db = new ApplicationDbContext())
            {

                try
                {
                    Logger.WriteTrace("Other", "DuplicateTansList Start");
                    ViewBag.ShippmentExceptionList = db.ShipmentExceptions.Where(x => x.BatchNo == BatchNo).ToList();
                    return PartialView("~/Views/Shipment/_ShipmentExceptionList.cshtml");
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                    throw;
                }
            }
        }

        public ActionResult UpdateTargetDate()
        {
            try
            {
                Logger.WriteTrace("Other", "UpdateTargetDate Start");

                int BatchId = Convert.ToInt32(Request.Form[0]);
                string SpreadSheetName = string.Empty;

                HttpPostedFileBase file = null;
                List<Entities.ShippmentBatchExcel> shipmentbatch = new List<Entities.ShippmentBatchExcel>();
                Dictionary<string, Entities.ShippmentBatchExcel> TanWiseUpdateProperties = new Dictionary<string, Entities.ShippmentBatchExcel>();
                if (Request.Files.Count > 0)
                {
                    file = Request.Files[0];
                    SpreadSheetName = System.IO.Path.GetFileName(file.FileName);
                    file.SaveAs(Server.MapPath("~/ShippmentExcel/" + System.IO.Path.GetFileName(file.FileName)));

                    string pathToExcelFile = Server.MapPath("~/ShippmentExcel/" + Path.GetFileName(file.FileName));


                    using (var stream = System.IO.File.Open(pathToExcelFile, FileMode.Open, FileAccess.Read))
                    {
                        using (var reader = ExcelDataReader.ExcelReaderFactory.CreateReader(stream))
                        {
                            bool isHeadersRead = false;
                            while (reader.Read())
                            {
                                if (isHeadersRead)
                                {
                                    try
                                    {
                                        ShippmentBatchExcel objmb = new ShippmentBatchExcel();
                                        objmb.TAN = (reader.FieldCount > 0 && reader.GetValue(0) != null) ? reader.GetValue(0).ToString() : string.Empty;
                                        objmb.VOL = (reader.FieldCount > 1 && reader.GetValue(1) != null) ? reader.GetValue(1).ToString() : string.Empty;
                                        objmb.IS = (reader.FieldCount > 2 && reader.GetValue(2) != null) ? reader.GetValue(2).ToString() : string.Empty;
                                        objmb.DocClass = (reader.FieldCount > 3 && reader.GetValue(3) != null) ? reader.GetValue(3).ToString() : string.Empty;
                                        objmb.ProcessingNotes = (reader.FieldCount > 4 && reader.GetValue(4) != null) ? reader.GetValue(4).ToString() : string.Empty;
                                        objmb.Completebydate = (reader.FieldCount > 5 && reader.GetValue(5) != null) ? reader.GetValue(5).ToString() : string.Empty;
                                        TanWiseUpdateProperties[objmb.TAN] = objmb;
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                }
                                isHeadersRead = true;
                            }
                            isHeadersRead = false;
                        }
                    }


                    #region Commented
                    //var excelFile = new ExcelQueryFactory(pathToExcelFile);
                    //// var GetSheetNames=excelFile.GetWorksheetNames();
                    //var results = StaticStore.GetAllWorksheets(pathToExcelFile);
                    //Sheet sheet = results.FirstOrDefault() as Sheet;
                    //Logger.WriteTrace("Other", $"upload to this folder {pathToExcelFile}");
                    //var artistAlbums = from a in excelFile.Worksheet(sheet.Name) select a;
                    //string[] excelcolumn = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };//Created array for excel columns
                    //string firstvalue = string.Empty;
                    //int row = 2;
                    //firstvalue = (ExcelReadingCellValue.GetCellValue(pathToExcelFile, sheet.Name, excelcolumn[0] + row));

                    //while (firstvalue != string.Empty)
                    //{
                    //    ShippmentBatchExcel objmb = new ShippmentBatchExcel();
                    //    objmb.TAN = ExcelReadingCellValue.GetCellValue(pathToExcelFile, sheet.Name, excelcolumn[0] + row) == null ? "" : (ExcelReadingCellValue.GetCellValue(pathToExcelFile, sheet.Name, excelcolumn[0] + row)).ToString();
                    //    firstvalue = objmb.TAN;
                    //    if (firstvalue == string.Empty)
                    //    {
                    //        break;
                    //    }

                    //    // TAN VOL IS Doc Class Processing Notes Complete by date

                    //    objmb.VOL = ExcelReadingCellValue.GetCellValue(pathToExcelFile, sheet.Name, excelcolumn[1] + row);
                    //    objmb.IS = ExcelReadingCellValue.GetCellValue(pathToExcelFile, sheet.Name, excelcolumn[2] + row);
                    //    objmb.DocClass = ExcelReadingCellValue.GetCellValue(pathToExcelFile, sheet.Name, excelcolumn[3] + row);
                    //    objmb.ProcessingNotes = ExcelReadingCellValue.GetCellValue(pathToExcelFile, sheet.Name, excelcolumn[4] + row);
                    //    objmb.Completebydate = ExcelReadingCellValue.GetCellValue(pathToExcelFile, sheet.Name, excelcolumn[5] + row);
                    //    shipmentbatch.Add(objmb);
                    //    row++;
                    //}
                    //Logger.WriteTrace("Other", $"End Read Excel Data and save to object"); 
                    #endregion
                }
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(30)))
                {
                    Logger.WriteTrace("Other", $"Scope Start");
                    using (var dbx = new ApplicationDbContext())
                    {
                        Logger.WriteTrace("Other", $"DBx Start");
                        List<Tan> tans = new List<Tan>();
                        List<Batch> Batches = new List<Batch>();

                        int FromBatchId = Convert.ToInt32(Request.Form[1]);
                        int ToBatchId = Convert.ToInt32(Request.Form[2]);
                        Logger.WriteTrace("Other", $"Get Batchs Start");
                        try
                        {
                            Batches = dbx.Batches.Where(x => x.Name >= FromBatchId && x.Name <= ToBatchId).ToList();
                        }
                        catch (Exception ex)
                        {
                            Logger.WriteTrace("Other", $"{ex.InnerException}-----{ex.StackTrace}");
                        }

                        tans = dbx.Tans.Include(t => t.Batch).Where(t => TanWiseUpdateProperties.Keys.Contains(t.tanNumber)).ToList();

                        var CheckingUpdatedTans = new List<KeyValuePair<int, string>>();
                        List<string> TanNumbersList = new List<string>();
                        int ENP_FF = 0;
                        int ENJ_FF = 0;
                        KeyValuePair<int, string> UpdatedList;
                        foreach (var tan in tans)
                        {
                            var updateshipmentbatch = TanWiseUpdateProperties.ContainsKey(tan.tanNumber) ? TanWiseUpdateProperties[tan.tanNumber] : null;
                            if (updateshipmentbatch != null)
                            {
                                DateTime outtime;
                                if (!string.IsNullOrEmpty(updateshipmentbatch.Completebydate) && DateTime.TryParse(updateshipmentbatch.Completebydate, CultureInfo.GetCultureInfo("en-gb"), DateTimeStyles.None, out outtime))
                                    tan.TargetedDate = DateTime.Parse(updateshipmentbatch.Completebydate, CultureInfo.GetCultureInfo("en-gb"));
                                tan.DocClass = updateshipmentbatch.DocClass;
                                tan.TanType = updateshipmentbatch.DocClass;
                                tan.SpreadSheetName = SpreadSheetName;
                                tan.ShipmentreceivedDate = DateTime.Parse(Request.Form[3], CultureInfo.GetCultureInfo("en-gb"));
                                UpdatedList = new KeyValuePair<int, string>(tan.BatchId, updateshipmentbatch.TAN); ;
                                CheckingUpdatedTans.Add(UpdatedList);
                                TanNumbersList.Add(tan.tanNumber);
                                if (updateshipmentbatch.DocClass == "ENP_FF_PW")
                                    ENP_FF++;
                                else
                                    ENJ_FF++;
                                tan.ProcessingNode = updateshipmentbatch.ProcessingNotes;
                                tan.ShipmentreceivedDate = DateTime.Parse(Request.Form[3], CultureInfo.GetCultureInfo("en-gb"));
                            }

                            #region Commented
                            //foreach (var _shipmentbatch in shipmentbatch)
                            //{
                            //    if (tan.tanNumber == _shipmentbatch.TAN)
                            //    {


                            //        // Logger.WriteTrace($"Date Time {tan.tanNumber}----{Convert.ToDateTime(_shipmentbatch.Completebydate)}");

                            //        UpdatedList = new KeyValuePair<int, string>(tan.BatchId, _shipmentbatch.TAN);
                            //        CheckingUpdatedTans.Add(UpdatedList);
                            //        TanNumbersList.Add(tan.tanNumber);



                            //        //tan.TargetedDate = Convert.ToDateTime(_shipmentbatch.Completebydate);

                            //        //   Logger.WriteTrace($"First Start{_shipmentbatch.Completebydate}");
                            //        tan.TargetedDate = DateTime.Parse(_shipmentbatch.Completebydate, CultureInfo.GetCultureInfo("en-gb"));
                            //        //  Logger.WriteTrace($"First convert Date {tan.TargetedDate}");

                            //        //   Logger.WriteTrace($"First End");
                            //        //   tan.TargetedDate = DateTime.ParseExact(_shipmentbatch.Completebydate,"MM/dd/yyyy",CultureInfo.InvariantCulture);
                            //        tan.DocClass = _shipmentbatch.DocClass;
                            //        if (_shipmentbatch.DocClass == "ENP_FF")
                            //        {
                            //            ENP_FF++;
                            //        }
                            //        else
                            //        {
                            //            ENJ_FF++;
                            //        }
                            //        tan.ProcessingNode = _shipmentbatch.ProcessingNotes;
                            //        //      Logger.WriteTrace($"Second Start");
                            //        tan.ShipmentreceivedDate = DateTime.Parse(Request.Form[3], CultureInfo.GetCultureInfo("en-gb"));
                            //        //    Logger.WriteTrace($"Second End");
                            //        //  tan.ShipmentreceivedDate = Convert.ToDateTime(Request.Form[3]);
                            //        // tan.StinId=_shipmentbatch.     DateTime.Parse(Request.Form[3], CultureInfo.CurrentUICulture.DateTimeFormat);
                            //        break;
                            //    }
                            //} 
                            #endregion
                        }
                        List<Entities.ShippmentBatchExcel> RemainingTansList = new List<Entities.ShippmentBatchExcel>();
                        RemainingTansList = TanWiseUpdateProperties.Where(x => !TanNumbersList.Contains(x.Key.ToString())).Select(t => t.Value).ToList();

                        string strremaining = String.Join(",", RemainingTansList.Select(x => x.TAN));

                        Entities.ShippmentUploadedExcel shippmentUploadedExcel = new Entities.ShippmentUploadedExcel();
                        Logger.WriteTrace("Other", $"Start Save data to ShippmentUploadedExcels....");
                        shippmentUploadedExcel.BatchNumber = Request.Form[1] + "-" + Request.Form[2];
                        shippmentUploadedExcel.excelPath = Path.GetFileName(file.FileName);
                        shippmentUploadedExcel.RemainingTans = strremaining;
                        shippmentUploadedExcel.UploadedDate = DateTime.Now;
                        shippmentUploadedExcel.SpreadSheetName = SpreadSheetName;
                        shippmentUploadedExcel.Batches = Batches;
                        if (SpreadSheetName.SafeContainsLower("-"))
                        {
                            try
                            {
                                string date = SpreadSheetName.Split('-')[0].Replace("Excelra ", "");
                                int year = Convert.ToInt32(date.SafeSubstring(4, 0));
                                int month = Convert.ToInt32(date.SafeSubstring(2, 4));
                                int day = Convert.ToInt32(date.SafeSubstring(2, 6));
                                shippmentUploadedExcel.RecievedDate = new DateTime(year, month, day);
                            }
                            catch (Exception ex)
                            {
                                Log.Error(ex);
                            }
                        }

                        dbx.ShippmentUploadedExcels.Add(shippmentUploadedExcel);
                        //  dbx.SaveChanges();
                        Logger.WriteTrace("Other", $"End Save data to ShippmentUploadedExcels....");
                        ViewBag.RemainingTans = RemainingTansList;

                        List<KeyValuePair<int, string>> UpdatedTans = new List<KeyValuePair<int, string>>();

                        foreach (var batch in Batches)
                        {
                            var abc = CheckingUpdatedTans.Where(a => a.Key == batch.Id).GroupBy(x => x.Key).Select(x => x.Count());
                            UpdatedTans.Add(new KeyValuePair<int, string>(batch.Id, !string.IsNullOrEmpty(abc.Select(t => t.ToString()).FirstOrDefault()) ? abc.Select(t => t.ToString()).FirstOrDefault() : "0"));
                        }

                        ViewBag.batchUpdatedTans = UpdatedTans.ToDictionary(d => d.Key, d => d.Value);

                        ViewBag.Docclass = $"ENP_FF({ENP_FF}),ENJ_FF({ENJ_FF})";

                        ViewBag.batches = dbx.Batches.Where(b => b.Name >= FromBatchId && b.Name <= ToBatchId).ToList();
                        ViewBag.batchWiseTans = (from t in dbx.Tans where t.Batch.Name >= FromBatchId && t.Batch.Name <= ToBatchId group t by t.Batch.Id into batchWise select new { shipmentId = batchWise.Key, tans = batchWise.Count() })
                            .ToDictionary(d => d.shipmentId, d => d.tans);
                        dbx.SaveChanges();
                        scope.Complete();
                        Logger.WriteTrace("Other", $"Completed Target date for selected Shipments....");
                    }
                    return PartialView("~/Views/Shipment/_TargetedDateShipmentsList.cshtml");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        public ActionResult TargetDateUpdatedList(int? Id)
        {
            using (var db = new ApplicationDbContext())
            {
                ViewBag.batchWiseTans = db.Tans.Where(x => x.Batch.Name == Id && x.TargetedDate != null).ToList();
                return View();
            }
        }
        public ActionResult RemainingList(int? Id)
        {
            using (var db = new ApplicationDbContext())
            {
                ViewBag.batchWiseTans = db.Tans.Where(x => x.Batch.Name == Id && x.TargetedDate == null).ToList();
                return View();
            }
        }

    }

}

