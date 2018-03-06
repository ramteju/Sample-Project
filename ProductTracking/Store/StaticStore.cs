using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using iTextSharp.text.pdf;
using ProductTracking.Logging;
using ProductTracking.Models;
using ProductTracking.Models.Core;
using ProductTracking.Util;
using Sgml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;

namespace ProductTracking.Store
{
    public static partial class StaticStore
    {
        public static List<TanState> InprogressState = new List<TanState> {TanState.Curation_Assigned,
                                                                           TanState.Curation_Assigned_Rejected,
                                                                           TanState.Curation_InProgress,
                                                                           TanState.Curation_Progress_Rejected,
                                                                           TanState.Curation_ReAssigned,
                                                                           TanState.QC_InProgress,
                                                                           TanState.QC_ReAssigned,
                                                                           TanState.QC_Rejected,
                                                                           TanState.Review_Assigned,
                                                                           TanState.Review_Assigned_Rejected,
                                                                           TanState.Review_InProgress,
                                                                           TanState.Review_Progress_Rejected,
                                                                           TanState.Review_ReAssigned,
                                                                           TanState.Review_Rejected };
        public static string
     DOC_ROOT = "casdvw",
     ARTICLE_NODE = "article",
     CSIE_NODE = "csie",
     TAN_NODE = "tan",
     CAN_NODE = "an",
     JBIB_NODE = "jbib",
     JT_NODE = "jt",
     PBIB_NODE = "pbib",
     PBIB_JNAME_NODE = "py",
     RN_NODE = "rn",
     NUM_NODE = "num",
     TYPE_NODE = "dt",
     Issue_node = "",
     JOURNAL_STRING = "journal",
     PATENT_STRING = "patent",
     CONFERENCE_STRING = "conference",
     PRIPRINT_STRING = "preprint",
     JournalYear_Node = "caspy",
     SUBSTANCE_NODE = "substanc",
     MF_NODE = "mf",
     IN_NODE = "in",
     SIM_NODE = "sim",
     COMP_NODE = "comp",
     CSIM_NODE = "csim",
      STE_NODE = "stemsg",
      CSH_NODE = "csh",
      CSM_NODE = "csm",
      PSEQ_NODE = "pseq",
      NSEQ_NODE = "nseq",
      Other_NODE = "syn";
        public static Sheets GetAllWorksheets(string fileName)
        {
            Sheets theSheets = null;

            using (SpreadsheetDocument document =
                SpreadsheetDocument.Open(fileName, false))
            {
                WorkbookPart wbPart = document.WorkbookPart;
                theSheets = wbPart.Workbook.Sheets;
            }
            return theSheets;
        }

        public static XmlDocument FromHtml(TextReader reader)
        {

            SgmlReader sgmlReader = new SgmlReader();
            sgmlReader.DocType = "XML";
            sgmlReader.WhitespaceHandling = WhitespaceHandling.None;
            sgmlReader.CaseFolding = Sgml.CaseFolding.ToLower;
            sgmlReader.InputStream = reader;

            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = true;
            doc.XmlResolver = null;
            doc.Load(sgmlReader);
            return doc;
        }

        public static IEnumerable<XmlNode> GetChilds(XmlNode root, String childNodeName)
        {
            return root.ChildNodes.OfType<XmlElement>().Where(e => e.Name == childNodeName);
        }

        public static String extractText(string pdfPath)
        {
            try
            {
                PdfReader pdfReader = new PdfReader(pdfPath);
                if (pdfReader.NumberOfPages <= 300)
                {
                    AppLocationTextExtractionStrategy strategy = new AppLocationTextExtractionStrategy();
                    for (int pageNumber = 1; pageNumber <= pdfReader.NumberOfPages; pageNumber++)
                    {

                        iTextSharp.text.pdf.parser.PdfTextExtractor.GetTextFromPage(pdfReader, pageNumber, strategy);
                    }
                    return strategy.textResult.ToString();
                }
                return String.Empty;
            }
            catch
            {
                return String.Empty;
            }
        }
        public static void SendMail(List<string> TanNumbers, int BatchNo, List<TanIssues> tanIssues, TimeSpan timeSpan)
        {
            string templateString = String.Empty;
            using (var sr = new StreamReader(System.Web.Hosting.HostingEnvironment.MapPath("~/Mailtemplates/") + "_mailTemplate.cshtml"))
                templateString = sr.ReadToEnd();
            StringBuilder s = new StringBuilder();
            string strBody = string.Join(", ", TanNumbers);
            if (tanIssues.Any())
                strBody = strBody + "<h4>Missing Tan Nums List : </h4><br/>" + string.Join(", ", tanIssues.Select(i => i.Tan.tanNumber).ToList());
            s.Append(strBody + "<br/>");
            s.Append($"<br/> Total Time Taken For Process: {timeSpan.TotalMinutes} Minutes.");
            string addedMsg = "<h4>New Tans : </h4><br/>" + s;

            string mailBody = string.Format(templateString, $"<b>{BatchNo.ToString()}</b>",
                addedMsg,
                DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss tt")
                );

            Mail.ReportMail("CAS-R New Shipment - Request To Excelra", mailBody);
        }
        private static byte[] ConvertHexToBytes(string input)
        {
            var result = new byte[(input.Length + 1) / 2];
            var offset = 0;
            if (input.Length % 2 == 1)
            {
                // If length of input is odd, the first character has an implicit 0 prepended.
                result[0] = (byte)Convert.ToUInt32(input[0] + "", 16);
                offset = 1;
            }
            for (int i = 0; i < input.Length / 2; i++)
            {
                result[i + offset] = (byte)Convert.ToUInt32(input.Substring(i * 2 + offset, 2), 16);
            }
            return result;
        }
        public static String ChildNodeText(XmlNode node, String childNodeName, bool bIsPBIBElement, int childNodePosition = 0)
        {
            try
            {
                if (childNodeName == "journal" && bIsPBIBElement == false)
                {
                    if (node.HasChildNodes)
                    {
                        var nodes = node.ChildNodes.OfType<XmlElement>().Where(e => e.Name == "jt");
                        if (nodes != null && nodes.Count() > 0)
                            return nodes.ElementAt(childNodePosition).NextSibling.InnerText;
                    }
                    return String.Empty;
                }
                else if (childNodeName == "patent" && bIsPBIBElement == true)
                {
                    if (node.HasChildNodes)
                    {
                        var nodes = node.ChildNodes.OfType<XmlElement>().Where(e => e.Name == "py");
                        if (nodes != null && nodes.Count() > 0)
                            return nodes.ElementAt(childNodePosition).PreviousSibling.InnerText;
                    }
                    return String.Empty;
                }
                else
                {
                    if (node.HasChildNodes)
                    {
                        var nodes = node.ChildNodes.OfType<XmlElement>().Where(e => e.Name == childNodeName);
                        if (nodes != null && nodes.Count() > 0)
                            return nodes.ElementAt(childNodePosition).InnerText;
                    }
                    return String.Empty;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return String.Empty;
            }

        }
    }



}