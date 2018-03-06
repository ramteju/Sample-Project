using Client.Command;
using Client.Common;
using Client.Logging;
using iTextSharp.text;
using iTextSharp.text.pdf;
using ProductTracking.Models.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels.Shipment
{
    public class ShipmentUploadVM : ViewModelBase
    {
        private string path, statusText, totalTime, backUpTime;
        private bool workInProgress;
        private int progressPercentage;
        private ObservableCollection<Tasks> tasksState;
        public string InputPath { get { return path; } set { SetProperty(ref path, value); } }
        public string StatusText { get { return statusText; } set { SetProperty(ref statusText, value); } }
        public string TotalTime { get { return totalTime; } set { SetProperty(ref totalTime, value); } }
        public string BackUpTime { get { return backUpTime; } set { SetProperty(ref backUpTime, value); } }
        public bool WorkInProgress { get { return workInProgress; } set { SetProperty(ref workInProgress, value); } }
        public int ProgressPercentage { get { return progressPercentage; } set { SetProperty(ref progressPercentage, value); } }
        public ObservableCollection<Tasks> TasksState { get { return tasksState; } set { SetProperty(ref tasksState, value); } }

        public List<TanKeywords> keyWordsList = new List<TanKeywords>();

        public ShipmentUploadVM()
        {
            TasksState = new ObservableCollection<Shipment.Tasks>();
            UploadShipment = new DelegateCommand(this.DoUploadShipment);
            LoadKeywords().ContinueWith(t => { });
        }

        public async Task LoadKeywords()
        {
            RestStatus status = await RestHub.TanKeyWords();
            if (status.UserObject != null)
            {
                keyWordsList = (List<TanKeywords>)status.UserObject;
            }
        }

        private void DoUploadShipment(object obj)
        {
            WorkInProgress = true;
            if (!string.IsNullOrEmpty(InputPath) && Directory.Exists(InputPath))
            {
                Task.Run(() =>
                {
                    var StartTime = DateTime.Now;
                    ProgressPercentage = 1;
                    StatusText = $"Taking BackUp Of Source Directory";
                    var AddTasksState = new Tasks();
                    string BackUpPath = C.SHAREDPATH;
                    Task.Run(() =>
                    {
                        StartTime = DateTime.Now;
                        AddTasksState.TaskName = "Taking BackUp Of Source Directory";
                        Parallel.ForEach(Directory.GetFiles(InputPath), new ParallelOptions { MaxDegreeOfParallelism = 4 }, node =>
                        {
                            try
                            {
                                string DetinationDirectory = Path.Combine(BackUpPath, $"ShipmentBackUp\\{GetBatchNameFromPath(InputPath)}");
                                if (!Directory.Exists(DetinationDirectory))
                                    Directory.CreateDirectory(DetinationDirectory);
                                File.Copy(node, Path.Combine(BackUpPath, $"ShipmentBackUp\\{GetBatchNameFromPath(InputPath)}", Path.GetFileName(node)), true);
                            }
                            catch (Exception ex)
                            {
                                Log.This(ex);
                            }
                        });
                        AddTasksState.TimeConsumed = (DateTime.Now - StartTime).TotalSeconds;
                        BackUpTime = (DateTime.Now - StartTime).TotalMinutes.ToString();
                        TasksState.Add(AddTasksState);
                        //ProgressPercentage = 25;
                    });
                    StatusText = $"Extracting Files to Shipment Directory";
                    StartTime = DateTime.Now;
                    AddTasksState = new Tasks();
                    AddTasksState.TaskName = $"Extracting Files to Shipment Directory";
                    var imageZipPaths = Directory.GetFiles(InputPath, "*.images.*");

                    Task.Run(() =>
                    {
                        Parallel.ForEach(imageZipPaths, new ParallelOptions { MaxDegreeOfParallelism = 4 }, imageZipPath =>
                        {
                            try
                            {
                                string strImagePath = System.IO.Path.GetFileNameWithoutExtension(imageZipPath);
                                string strImagesExtractionPath = Path.Combine(imageZipPath, InputPath + "\\" + strImagePath);
                                string DetinationDirectory = Path.Combine(BackUpPath, $"ShipmentBackUp\\{GetBatchNameFromPath(InputPath)}");
                                ZipFile.ExtractToDirectory(imageZipPath, InputPath + "\\" + strImagePath);
                            }
                            catch (Exception ex)
                            {
                                Log.This(ex);
                            }
                        });
                        AddTasksState.TimeConsumed = (DateTime.Now - StartTime).TotalSeconds;
                        TasksState.Add(AddTasksState);
                        ProgressPercentage = 50;
                        StatusText = $"Creating Tiff files from Input source";
                        StartTime = DateTime.Now;
                        AddTasksState = new Tasks();
                        AddTasksState.TaskName = $"Creating Tiff files from Input source";
                        ConcurrentBag<Dictionary<string, string>> TanWiseTotalDocument = new ConcurrentBag<Dictionary<string, string>>();
                        ConcurrentBag<Dictionary<string, string>> TanWiseBestFiles = new ConcurrentBag<Dictionary<string, string>>();
                        ConcurrentBag<Dictionary<string, string>> TanWiseKeyWords = new ConcurrentBag<Dictionary<string, string>>();
                        Parallel.ForEach(imageZipPaths, new ParallelOptions { MaxDegreeOfParallelism = 2 }, imageZipPath =>
                        {
                            using (ZipArchive zip = ZipFile.Open(imageZipPath, ZipArchiveMode.Read))
                            {
                                string strImagePath = System.IO.Path.GetFileNameWithoutExtension(imageZipPath);
                                CreateTiffImages(zip, BackUpPath, strImagePath, TanWiseTotalDocument, TanWiseBestFiles, TanWiseKeyWords);
                            }
                        });
                        AddTasksState.TimeConsumed = (DateTime.Now - StartTime).TotalSeconds;
                        TasksState.Add(AddTasksState);
                        ProgressPercentage = 75;
                        WorkInProgress = false;
                        TotalTime = (TasksState.Sum(t => t.TimeConsumed) / 60).ToString();
                    });

                    

                                     
                });
            }
        }

        public DelegateCommand UploadShipment { get; private set; }


        public void CreateTiffImages(ZipArchive zip, string destination, string strImagePath, ConcurrentBag<Dictionary<string, string>> TanWiseAllFiles, ConcurrentBag<Dictionary<string, string>> TanWiseBestFiles, ConcurrentBag<Dictionary<string, string>> TanWiseKeywords)
        {
            var TotalTanNumbers = zip.Entries.Select(ze => ze.FullName.Split('.')[0]).Distinct();
            var tifEntries = zip.Entries.Where(ze => ze.FullName.ToLower().EndsWith(".tif"));
            var tanNumberWiseTifs = tifEntries.GroupBy(te => te.FullName.Substring(0, 9)).ToDictionary(d => d.Key, d => d.ToList());
            int count = TotalTanNumbers.Count();
            Dictionary<string, string> tanWiseAllFiles = new Dictionary<string, string>();
            Dictionary<string, string> tanWiseBestFiles = new Dictionary<string, string>();
            Dictionary<string, string> tanWisekeyword = new Dictionary<string, string>();
            foreach (var TanNumber in TotalTanNumbers)
            {
                try
                {
                    StatusText = $"Creating Tif Images For for Tan: {TanNumber}";
                    var TanwiseFiles = zip.Entries.Where(t => t.FullName.Contains(TanNumber) && !t.FullName.ToLower().EndsWith(".tif")).ToList();
                    var TanFolder = Path.Combine(destination, "ShipmentTans", strImagePath, TanNumber);
                    string strTiffPdfFileName = string.Empty;
                    if (!@Directory.Exists(TanFolder))
                        Directory.CreateDirectory(TanFolder);
                    foreach (var CopyFiles in TanwiseFiles)
                    {
                        try
                        {
                            var fileName = System.IO.Path.GetFileName(CopyFiles.FullName);
                            var destFile = System.IO.Path.Combine(TanFolder, fileName);
                            System.IO.File.Copy(InputPath + "\\" + strImagePath + "\\" + CopyFiles, destFile, true);
                        }
                        catch (Exception ex)
                        {
                            //Log.This(ex);
                        }
                    }


                    #region Tiff To Pdf
                    var TanWiseTif = tanNumberWiseTifs.Where(t => t.Key == TanNumber);
                    foreach (var tanNumberWiseEntry in TanWiseTif)
                    {
                        string tanNumber = tanNumberWiseEntry.Key;
                        strTiffPdfFileName = tanNumber + "_tiff.pdf";
                        string pdfPath = Path.Combine(TanFolder, strTiffPdfFileName);
                        using (var pdfStream = new FileStream(pdfPath, FileMode.OpenOrCreate, FileAccess.Write))
                        using (var doc = new Document())
                        {
                            PdfWriter.GetInstance(doc, pdfStream);
                            doc.Open();
                            doc.SetMargins(20f, 20f, 20f, 20f);
                            int i = 0;
                            foreach (var tifEntry in tanNumberWiseEntry.Value)
                            {
                                using (var tifStream = tifEntry.Open())
                                {
                                    i++;
                                    try
                                    {
                                        var docImage = Image.GetInstance(tifStream);
                                        docImage.ScaleToFit(doc.PageSize.Width, doc.PageSize.Height);
                                        doc.Add(docImage);

                                        if (i + 1 < tanNumberWiseEntry.Value.Count())
                                            doc.NewPage();
                                    }
                                    catch (Exception ex)
                                    {
                                        //Log.This(ex);
                                    }
                                }
                            }
                            doc.Close();
                        }
                    }
                    #endregion

                    #region Best File Selection
                    StatusText = $"Selecting best file for tan: {TanNumber}";
                    var TanWiseBestFileEntry = zip.Entries.Where(s => s.FullName.Contains(TanNumber) && s.FullName.Contains("markup")).OrderByDescending(x => x.Length).FirstOrDefault();
                    var TanWiseBestFile = string.Empty;

                    if (TanWiseBestFileEntry != null)
                    {
                        TanWiseBestFile = TanWiseBestFileEntry.FullName;
                    }
                    else if (TanWiseBestFile == string.Empty && strTiffPdfFileName != string.Empty)
                    {
                        TanWiseBestFile = strTiffPdfFileName;
                    }
                    else
                    {
                        TanWiseBestFile = zip.Entries.Where(s => s.FullName.Contains(TanNumber) && s.FullName.Contains("article") || s.FullName.Contains("patent")).OrderByDescending(x => x.Length).FirstOrDefault().FullName;
                    }

                    var TanWiseTotalDocumentsPath = zip.Entries.Where(s => s.FullName.Contains(TanNumber) && !s.FullName.Contains(".tif") && s.FullName != TanWiseBestFile && s.FullName.Contains(".pdf")).ToList();
                    List<string> TanwiseLocalPath = new List<string>();
                    foreach (var TanwiseEachDocument in TanWiseTotalDocumentsPath)
                        TanwiseLocalPath.Add(@"ShipmentTans" + "\\" + strImagePath + "\\" + TanNumber + "\\" + TanwiseEachDocument);
                    string strTotalDocumentsPath = String.Join(",", TanwiseLocalPath);
                    tanWiseAllFiles[TanNumber] = strTotalDocumentsPath;
                    string TempKeywordDestnationPDF = @"ShipmentTans" + "\\" + strImagePath + "\\" + TanNumber + "\\" + TanWiseBestFile;
                    tanWiseBestFiles[TanNumber] = TempKeywordDestnationPDF;

                    string strPDFfileData = string.Empty;
                    strPDFfileData = S.extractText(Path.Combine(destination, TempKeywordDestnationPDF));
                    if (strPDFfileData != "")
                    {
                        List<string> foundWords = new List<string>();
                        foreach (TanKeywords word in keyWordsList)
                        {
                            try
                            {
                                if (!String.IsNullOrEmpty(word.keyword))
                                {
                                    string upperWord = word.keyword.Trim().ToUpper();
                                    if (strPDFfileData.IndexOf(upperWord) > -1)
                                        foundWords.Add(word.keyword);
                                }
                            }
                            catch (Exception ex)
                            {
                                //Log.This(ex);
                            }

                        }
                        string combinedKeywords = String.Join(",", foundWords.ToArray());
                        tanWisekeyword[TanNumber] = combinedKeywords;
                    }
                    #endregion

                }
                catch (Exception ex)
                {
                    //Log.This(ex);
                }
            }
            TanWiseAllFiles.Add(tanWiseAllFiles);
            TanWiseBestFiles.Add(tanWiseBestFiles);
            TanWiseKeywords.Add(tanWisekeyword);
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
    }

    public class Tasks : ViewModelBase
    {
        private string taskName;
        private double timeConsumed;
        public string TaskName { get { return taskName; } set { SetProperty(ref taskName, value); } }
        public double TimeConsumed { get { return timeConsumed; } set { SetProperty(ref timeConsumed, value); } }
    }
}
