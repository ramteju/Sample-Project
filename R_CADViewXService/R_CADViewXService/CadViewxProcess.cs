using CADViewLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Configuration;
using Entities;
using System.Transactions;
using ProductTracking.Models;
using R_CADViewXService.RestConnection;

namespace R_CADViewXService
{
    public static class CadViewxProcess
    {

        public static bool bIsPBIBElement;
        static string regNumber;


        public static async Task RunCadViewx()
        {

            NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
            Logger.Info("------------CadViewx Service Started . .");

            try
            {
                ShippmentUploadStatus shippmentUploadstatus = null;
                RestStatus status = await RestHub.GetShipmentUploadStatus();
                if (status.UserObject != null)
                {
                    shippmentUploadstatus = (ShippmentUploadStatus)status.UserObject;
                }
                if (shippmentUploadstatus != null)
                {
                    shippmentUploadstatus.Status = ShippmentUploadEnumStatus.CadViewProgress.ToString();
                    await RestHub.UpdateShipmentUploadStatus(shippmentUploadstatus);
                    Logger.Info(shippmentUploadstatus.Path.ToString());

                    string DOC_ROOT = "casdvw",
                           ARTICLE_NODE = "article",
                           RN_NODE = "rn",
                           SUBSTANCE_NODE = "substanc",
                           SIM_NODE = "sim",
                           COMP_NODE = "comp",
                           CSIM_NODE = "csim";
                    XmlDocument xmlDoc = null;
                    try
                    {
                        xmlDoc = FromHtml(System.IO.File.OpenText(shippmentUploadstatus.Path));
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex.ToString());
                    }
                    if (xmlDoc != null)
                    {
                        var casdvwNode = xmlDoc.ChildNodes.OfType<XmlElement>().Where(i => i.Name == DOC_ROOT).FirstOrDefault();
                        if (casdvwNode != null)
                        {
                            var articles = GetChilds(casdvwNode, ARTICLE_NODE);
                            var totalArticles = articles.Count();
                            string strTempDirPath = Path.Combine(shippmentUploadstatus.NumImagesPath, shippmentUploadstatus.BatchNo.ToString(), "Img");
                            Logger.Info($"Started Image processing from the path : {strTempDirPath}");
                            if (!Directory.Exists(strTempDirPath))
                                Directory.CreateDirectory(strTempDirPath);

                            int substanceIndex = 0;
                            var substanceChildList = GetChilds(casdvwNode, SUBSTANCE_NODE);
                            var totalSubstances = substanceChildList.Count();
                            foreach (var substanceNode in substanceChildList)
                            {
                                try
                                {
                                    CADViewX cadViewX = new CADViewX();
                                    substanceIndex++;
                                    // Interlocked.Increment(ref substanceIndex);
                                    Logger.Info("------------Start Processing Substance . ." + (substanceIndex));
                                    if (substanceIndex % 10 == 0)
                                    {
                                        // ShipmentException((substanceIndex) + " / " + totalSubstances + "-----------" , strSourcePath, "Testing");
                                        Logger.Info("------------Processing Substance . ." + (substanceIndex) + " / " + totalSubstances);
                                    }


                                    XmlNodeList substanceChilds = substanceNode.ChildNodes;
                                    List<String> hexCodes = new List<String>();

                                    for (int substacneNodeIndex = 0; substacneNodeIndex < substanceChilds.Count; substacneNodeIndex++)
                                    {
                                        regNumber = ChildNodeText(substanceNode, RN_NODE).Replace("-", "");

                                        if (substanceChilds[substacneNodeIndex].Name == SIM_NODE)
                                            hexCodes.Add(substanceChilds[substacneNodeIndex].InnerText);

                                        else if (substanceChilds[substacneNodeIndex].Name == COMP_NODE)
                                        {
                                            XmlNodeList compNodeChilds = substanceChilds[substacneNodeIndex].ChildNodes;
                                            if (compNodeChilds != null && compNodeChilds.Count > 0)
                                            {
                                                for (int csimNodeIndex = 0; csimNodeIndex < compNodeChilds.Count; csimNodeIndex++)
                                                {
                                                    var compChild = compNodeChilds[csimNodeIndex];
                                                    if (compChild.Name == CSIM_NODE)
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
                                                    var byteArray = ConvertHexToBytes(hex);

                                                    var cgmFilePath = Path.GetTempPath() + Guid.NewGuid().ToString() + ".cgm";

                                                    var gifFilePath = registerNumberCount > 1 ?
                                                    Path.Combine(strTempDirPath, regNumber + "_" + registerNumberCount + ".gif") :
                                                    Path.Combine(strTempDirPath, regNumber + ".gif");

                                                    try
                                                    {
                                                        Logger.Info(cgmFilePath + "------------Start while gif generation . ." + (substanceIndex + regNumber));
                                                        System.IO.File.WriteAllBytes(cgmFilePath, byteArray);
                                                        Logger.Info(cgmFilePath + "------------End while gif generation . ." + (substanceIndex + regNumber));
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        Logger.Info("------------Error while gif generation . ." + (substanceIndex + regNumber) + " : " + ex.Message);
                                                        // context.Clients.All.progress("Error while gif generation . ." + (substanceIndex) + " : " + ex.Message);
                                                    }
                                                    cadViewX.LoadFile(cgmFilePath);
                                                    if (!System.IO.File.Exists(gifFilePath))
                                                        cadViewX.SaveToFile(gifFilePath);
                                                    cadViewX.CloseFile();
                                                    registerNumberCount++;
                                                    if (File.Exists(cgmFilePath))
                                                        File.Delete(cgmFilePath);
                                                }
                                                catch (Exception ex)
                                                {
                                                    Logger.Info("------------Some error Occured" + ex.Message + hex);
                                                    // Debug.WriteLine("Some error Occured" + ex.Message + hex);
                                                }
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {

                                }
                            }


                        }
                    }
                    shippmentUploadstatus.Status = ShippmentUploadEnumStatus.CadViewCompleted.ToString();
                    RestStatus UpdateStatus = await RestHub.UpdateShipmentUploadStatus(shippmentUploadstatus);

                }
                else
                {
                    Logger.Info($"Under CADViewx Processing : ");
                }

            }
            catch (Exception ex)
            {

            }

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
        public static String ChildNodeText(XmlNode node, String childNodeName, int childNodePosition = 0)
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

                return String.Empty;
            }

        }
        static XmlDocument FromHtml(TextReader reader)
        {

            Sgml.SgmlReader sgmlReader = new Sgml.SgmlReader();
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
        static IEnumerable<XmlNode> GetChilds(XmlNode root, String childNodeName)
        {
            return root.ChildNodes.OfType<XmlElement>().Where(e => e.Name == childNodeName);
        }
    }
}
