using iTextSharp.text;
using iTextSharp.text.pdf;
using ProductTracking.Models.Core;
using ProductTracking.Store;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web;

namespace ProductTracking.Util
{
    public static class ImageToOdf
    {
        public static void CreateTiffImages(string InputPath, ZipArchive zip, string destination, string strImagePath, ConcurrentBag<Dictionary<string, string>> TanWiseAllFiles,
                                            ConcurrentBag<Dictionary<string, string>> TanWiseBestFiles, ConcurrentBag<Dictionary<string, string>> TanWiseKeywords, List<TanKeywords> keyWordsList)
        {
            List<string> Emptytans = new List<string>();
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
                    var TanwiseFiles = zip.Entries.Where(t => t.FullName.Contains(TanNumber) && !t.FullName.ToLower().EndsWith(".tif")).ToList();
                    var TanFolder = Path.Combine(destination, "ShipmentTans", strImagePath, TanNumber);
                    string strTiffPdfFileName = string.Empty;
                    if (!Directory.Exists(TanFolder))
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
                        }
                    }


                    #region Tiff To Pdf
                    var TanWiseTif = tanNumberWiseTifs.Where(t => t.Key == TanNumber);
                    foreach (var tanNumberWiseEntry in TanWiseTif)
                    {
                        try
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
                        catch (Exception ex)
                        {
                        }
                    }
                    #endregion

                    #region Best File Selection
                    var TanWiseBestFileEntry = zip.Entries.Where(s => s.FullName.Contains(TanNumber) && s.FullName.Contains("markup")).OrderByDescending(x => x.Length).FirstOrDefault();
                    var TanWiseBestFile = string.Empty;

                    if (TanWiseBestFileEntry != null)
                        TanWiseBestFile = TanWiseBestFileEntry.FullName;
                    else if (TanWiseBestFile == string.Empty && strTiffPdfFileName != string.Empty)
                        TanWiseBestFile = strTiffPdfFileName;
                    else if (zip.Entries.Where(s => s.FullName.Contains(TanNumber) && (s.FullName.Contains("article") || s.FullName.Contains("patent"))).Any())
                        TanWiseBestFile = zip.Entries.Where(s => s.FullName.Contains(TanNumber) && s.FullName.Contains("article") || s.FullName.Contains("patent")).OrderByDescending(x => x.Length).FirstOrDefault().FullName;
                    else if (zip.Entries.Where(s => s.FullName.Contains(TanNumber) && s.FullName.Contains(".pdf")).Any())
                        TanWiseBestFile = zip.Entries.Where(s => s.FullName.Contains(TanNumber) && s.FullName.Contains(".pdf")).FirstOrDefault().FullName;
                    var TanWiseTotalDocumentsPath = zip.Entries.Where(s => s.FullName.Contains(TanNumber) && !s.FullName.Contains(".tif") && s.FullName != TanWiseBestFile && s.FullName.Contains(".pdf")).ToList();
                    List<string> TanwiseLocalPath = new List<string>();
                    foreach (var TanwiseEachDocument in TanWiseTotalDocumentsPath)
                        TanwiseLocalPath.Add(@"ShipmentTans" + "\\" + strImagePath + "\\" + TanNumber + "\\" + TanwiseEachDocument.FullName);
                    string strTotalDocumentsPath = String.Join(",", TanwiseLocalPath);
                    tanWiseAllFiles[TanNumber] = strTotalDocumentsPath;
                    string TempKeywordDestnationPDF = $"ShipmentTans\\{strImagePath}\\{TanNumber}\\{(!string.IsNullOrEmpty(TanWiseBestFile) ? TanWiseBestFile : TanwiseLocalPath.Any() ? TanwiseLocalPath.First() : string.Empty)}";
                    tanWiseBestFiles[TanNumber] = TempKeywordDestnationPDF;

                    string strPDFfileData = string.Empty;
                    strPDFfileData = StaticStore.extractText(Path.Combine(destination, TempKeywordDestnationPDF));
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
    }
}