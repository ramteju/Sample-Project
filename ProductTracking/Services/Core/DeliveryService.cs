using DTO;
using Entities;
using Entities.DTO;
using Microsoft.AspNet.SignalR;
using ProductTracking.Hubs;
using ProductTracking.Models;
using ProductTracking.Models.Core;
using ProductTracking.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Xml;
using System.Xml.Serialization;
using System.Data.Entity;
using Entities.DTO.Delivery;
using xsd;
using Excelra.Utils.Library;
using ProductTracking.Logging;

namespace ProductTracking.Services.Core
{
    public class DeliveryService
    {


        public GenerateXMLDTO GenerateXML(int id)
        {
            GenerateXMLDTO dto = new GenerateXMLDTO();

            if (!Directory.Exists(Store.C.DELIVERY_PATH))
                throw (new Exception($"Delivery Folder {Store.C.DELIVERY_PATH} Not Exists . ."));

            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, TimeSpan.FromMinutes(15)))
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                List<string> IgnorableCVTs = db.CVT.Where(c => c.IsIgnorableInDelivery).Select(c => c.CVTS).ToList();
                DeliveryBatch deliveryBatch = db.DeliveryBatches.Find(id);
                var outXmlPath = Path.Combine(Store.C.DELIVERY_PATH, deliveryBatch.BatchNumber + ".xml");

                var tanIds = deliveryBatch.Tans.Select(t=>t.Id).ToList();
                var tanDatas = db.TanData.Where(td => tanIds.Contains(td.TanId)).ToList();
                if (!tanDatas.Any())
                {
                    throw new Exception($"No Curated TANs Found In Delivery Batch {deliveryBatch.BatchNumber}.");
                }
                xsd.RXNFILE rxnFile = new xsd.RXNFILE();
                rxnFile.SOURCE = "GVKbio";
                rxnFile.FILENUM = deliveryBatch.BatchNumber;
                rxnFile.VERSION = 2;
                var documents = new List<xsd.DOCUMENT>();
                foreach (var tanData in tanDatas)
                {
                    XmlUtils.LoadMasterData(tanData);
                    xsd.RXNGRP rxnGroup = new xsd.RXNGRP();
                    List<xsd.RXN> rxns = new List<xsd.RXN>();
                    foreach (var reaction in tanData.Tan.Reactions)
                    {
                        #region RXNID
                        var keyProduct = tanData.Tan.Participants.Where(p => p.ReactionId == reaction.Id && p.KeyProduct == true).FirstOrDefault();
                        var rxnId = new RXNID();
                        rxnId.RXNNUM = keyProduct?.Participant?.NUM.ToString();
                        rxnId.RXNSEQ = keyProduct?.KeyProductSeq.ToString();
                        #endregion

                        #region RXNPROCESS
                        var rxnProcess = new xsd.RXNPROCESS();
                        var xmlStages = new List<xsd.STAGE>();

                        #region RSNs                        
                        rxnProcess.RSN = GetRSN(reaction, IgnorableCVTs);
                        #endregion

                        #region Stages
                        foreach (var stage in reaction.Stages)
                        {
                            var xmlStage = new xsd.STAGE();
                            #region Conditions (Sub stages)
                            var subStages = new List<xsd.SUBSTAGE>();
                            foreach (var condition in stage.StageConditions)
                            {
                                var subStage = new xsd.SUBSTAGE();
                                var conditions = new List<xsd.COND>();

                                if (!String.IsNullOrEmpty(condition.Temperature))
                                    conditions.Add(new xsd.COND { Value = condition.Temperature, TYPE = CondType.TP });
                                if (!String.IsNullOrEmpty(condition.Pressure))
                                    conditions.Add(new xsd.COND { Value = condition.Pressure, TYPE = CondType.PR });
                                if (!String.IsNullOrEmpty(condition.Time))
                                    conditions.Add(new xsd.COND { Value = condition.Time, TYPE = CondType.TM });
                                if (!String.IsNullOrEmpty(condition.PH))
                                    conditions.Add(new xsd.COND { Value = condition.PH, TYPE = CondType.PH });

                                subStage.COND = conditions.ToArray();
                                subStages.Add(subStage);
                            }
                            xmlStage.SUBSTAGE = subStages.ToArray();
                            #endregion
                            xmlStages.Add(xmlStage);
                        }
                        rxnProcess.STAGE = xmlStages.ToArray();
                        #endregion
                        #endregion

                        #region XREFGRP
                        var xRefGroup = new xsd.XREFGRP();
                        var nrns = new List<xsd.NRN>();
                        foreach (var tanParticipant in tanData.Tan.Participants.Where(p => p.Reaction.Id == reaction.Id && p.Participant.ChemicalType != ChemicalType.S8000))
                        {
                            xsd.NRN nrn;
                            if (!nrns.Where(n => n.NRNNUM == tanParticipant.Participant.NUM).Any())
                            {
                                nrn = new xsd.NRN()
                                {
                                    NRNNUM = tanParticipant.Participant.NUM,
                                    NRNREG = Int32.Parse(tanParticipant.Participant.RegNumber)
                                };
                                nrns.Add(nrn);
                            }
                        }
                        xRefGroup.NRN = nrns.ToArray();
                        #endregion

                        xsd.SUBDESC subDesc = null;
                        var s8000Participants = reaction.Tan.Participants.
                            Where(rp => rp.ReactionId == reaction.Id && rp.Participant.ChemicalType == ChemicalType.S8000);
                        var subDefinitions = new List<xsd.SUBDEFN>();
                        if (s8000Participants.Any())
                        {
                            foreach (var s8000Particiapnt in s8000Participants)
                            {
                                var subDefinition = new xsd.SUBDEFN();
                                if (!subDefinitions.Where(p => p.NRNNUM == s8000Particiapnt.Participant.NUM.ToString()).Any())
                                {
                                    subDefinition.NRNNUM = s8000Particiapnt.Participant.NUM.ToString();
                                    subDefinition.SUBNAME = s8000Particiapnt.Participant.Name;
                                    subDefinition.SUBLOC = String.Join(",", s8000Particiapnt.Participant.MetaData.Select(md => md.PageNo).ToList());
                                    subDefinitions.Add(subDefinition);
                                }
                            }
                        }
                        subDesc = new xsd.SUBDESC();
                        if (!subDefinitions.Any())
                            subDefinitions.Add(new SUBDEFN());
                        subDesc.SUBDEFN = subDefinitions.ToArray();

                        #region RXN
                        var rxn = new xsd.RXN();
                        rxn.NO = reaction.DisplayOrder;
                        rxn.RXNID = new xsd.RXNID { RXNNUM = reaction.KeyProductNum, RXNSEQ = reaction.KeyProductSequence };
                        rxn.RSD = reaction.RSD;
                        rxn.XREFGRP = xRefGroup;
                        rxn.RXNPROCESS = rxnProcess;
                        rxn.SUBDESC = subDesc;
                        rxns.Add(rxn);
                        #endregion
                    }
                    rxnGroup.RXN = rxns.ToArray();

                    var document = new xsd.DOCUMENT();
                    document.VIEW = "RXN";
                    document.CAN = tanData.Tan.CAN;
                    document.TAN = tanData.Tan.tanNumber;
                    document.ANALYST = 8005.ToString();
                    document.COMMENTS = tanData.Tan.CommentsForXml;
                    document.RXNGRP = rxnGroup;
                    documents.Add(document);
                }
                rxnFile.DOCUMENT = documents.ToArray();
                var serializer = new XmlSerializer(typeof(xsd.RXNFILE), String.Empty);
                var settings = new XmlWriterSettings
                {
                    Indent = false,
                    OmitXmlDeclaration = true
                };
                using (var writer = new StreamWriter(outXmlPath))
                using (var xmlWriter = XmlWriter.Create(writer, settings))
                {
                    XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                    serializer.Serialize(xmlWriter, rxnFile, ns);
                }

                return new GenerateXMLDTO
                {
                    IsSuccess = true,
                    OutXmlPath = outXmlPath
                };
            }
        }

        private xsd.RSN[] GetRSN(Reaction reaction, List<string> IgnorableCVTs)
        {
            List<xsd.RSN> rsns = new List<xsd.RSN>();
            List<string> freeTexts = new List<string>();
            if (reaction.Tan.RSNs != null)
            {
                #region FreeText
                List<string> reactionFreeTexts = reaction.Tan.RSNs
                            .Where(rsn => rsn.Reaction.Id == reaction.Id && rsn.Stage == null && !rsn.IsIgnorableInDelivery && String.IsNullOrEmpty(rsn.CVT))
                            .Select(rsn => rsn.FreeText)
                            .ToList();

                List<string> stageFreeTexts = reaction.Tan.RSNs
                    .Where(rsn => rsn.Reaction.Id == reaction.Id && rsn.Stage != null && !rsn.IsIgnorableInDelivery && String.IsNullOrEmpty(rsn.CVT))
                    .OrderBy(rsn => rsn.Stage.DisplayOrder)
                    .Select(rsn => rsn.FreeText)
                    .ToList();

                freeTexts.AddRange(reactionFreeTexts);
                freeTexts.AddRange(stageFreeTexts);

                string freeText = String.Join(", ", freeTexts);

                if (freeText.Length > 0)
                    rsns.Add(new xsd.RSN { TYPE = "FREE", Value = freeText });
                #endregion

                #region CVT
                List<ReactionRSN> reactionCVTs = reaction.Tan.RSNs
                            .Where(rsn => rsn.Reaction.Id == reaction.Id && rsn.Stage == null && !rsn.IsIgnorableInDelivery && !IgnorableCVTs.Contains(rsn.CVT) && !String.IsNullOrEmpty(rsn.CVT))
                            .ToList();
                foreach (var rsn in reactionCVTs)
                    rsns.Add(new xsd.RSN { TYPE = rsn.CVT, Value = rsn.FreeText });

                List<ReactionRSN> stageCVTs = reaction.Tan.RSNs
                            .Where(rsn => rsn.Reaction.Id == reaction.Id && rsn.Stage != null && !rsn.IsIgnorableInDelivery && !IgnorableCVTs.Contains(rsn.CVT) && !String.IsNullOrEmpty(rsn.CVT))
                            .OrderBy(rsn => rsn.Stage.DisplayOrder)
                            .ToList();
                foreach (var rsn in stageCVTs)
                    rsns.Add(new xsd.RSN { TYPE = rsn.CVT.Trim(), Value = rsn.FreeText.Trim() });
                #endregion
            }
            return rsns.ToArray();
        }
        public object GenerateZip(int id, string userName, long maxZipSize = 1450000000)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<LiveHub>();
            List<string> connectionIds = LiveHub.UserConnectionIds.ContainsKey(userName) ? LiveHub.UserConnectionIds[userName] : null;
            if (!Directory.Exists(Store.C.DELIVERY_PATH))
            {
                throw new Exception("Delivery Folder Is Not Valid");
            }
            try
            {
                if (maxZipSize > 1450000000)
                    maxZipSize = 1450000000;
                List<string> zipPaths = new List<string>();
                List<DeliveryTan> deliveryTans = new List<DeliveryTan>();
                StringBuilder sb = new StringBuilder();
                List<Tan> tans;
                int batchNumber;
                using (var db = new ApplicationDbContext())
                {
                    batchNumber = db.DeliveryBatches.Find(id).BatchNumber;
                    tans = db.DeliveryBatches.Find(id).Tans.Where(t=>t.RxnCount > 0).ToList();
                }
                if (connectionIds != null)
                    foreach (var connectionId in connectionIds)
                        context.Clients.Client(connectionId).deliveryStatus("Identifying TAN Sizes . .");
                int bucket = 1;
                long currentBucketSize = 0;
                foreach (var tan in tans)
                {
                    string DocumnetFullPath = Path.Combine(Store.C.ShipmentSharedPath, tan.DocumentPath);
                    if (File.Exists(DocumnetFullPath))
                    {
                        var fileSize = new FileInfo(DocumnetFullPath).Length;
                        deliveryTans.Add(new DeliveryTan
                        {
                            tanNumber = tan.tanNumber,
                            tanPath = DocumnetFullPath,
                            fileSize = fileSize,
                            bucket = bucket
                        });
                        currentBucketSize += fileSize;
                        if (currentBucketSize >= maxZipSize)
                        {
                            currentBucketSize = 0;
                            bucket++;
                        }
                    }
                }
                Dictionary<int, List<DeliveryTan>> tanBuckets = deliveryTans.GroupBy(d => d.bucket).ToDictionary(d => d.Key, d => d.ToList());
                HashSet<string> tanNumbers = new HashSet<string>();
                foreach (var tanBucket in tanBuckets)
                {
                    if (connectionIds != null)
                        foreach (var connectionId in connectionIds)
                            context.Clients.Client(connectionId).deliveryStatus($"Creating Directory For Zip {tanBucket.Key} . .");
                    string zipDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                    int count = 1;
                    foreach (var tan in tanBucket.Value)
                    {
                        if (!String.IsNullOrEmpty(tan.tanPath))
                        {
                            if (connectionIds != null)
                                foreach (var connectionId in connectionIds)
                                    context.Clients.Client(connectionId).deliveryStatus($"For ZIP {tanBucket.Key}, Moving TAN {tan.tanNumber} {count}/{tanBucket.Value.Count}");
                            var tanFolder = Path.Combine(zipDirectory, tan.tanNumber, "markup");
                            Directory.CreateDirectory(tanFolder);
                            var outPdfPath = Path.Combine(tanFolder, "Markup 1 - casreact.pdf");
                            PdfAnnotsReplacer.ApplyProperties(tan.tanPath, outPdfPath);
                            //File.Copy(tan.tanPath, outPdfPath);
                            count++;
                            tanNumbers.Add(tan.tanNumber);
                        }
                    }
                    string fileIndex = String.Empty;
                    if (tanBuckets.Count > 1)
                        fileIndex = $"_{tanBucket.Key.ToString()}";
                    var zipPath = Path.Combine(Store.C.DELIVERY_PATH, $"rxnmarkup.{batchNumber}{fileIndex}.zip");
                    if (File.Exists(zipPath))
                    {
                        if (connectionIds != null)
                            foreach (var connectionId in connectionIds)
                                context.Clients.Client(connectionId).deliveryStatus("Deleting Existing Zip File . .");
                        File.Delete(zipPath);
                    }
                    var size = new DirectoryInfo(zipDirectory).GetFiles("*.*", SearchOption.AllDirectories).Sum(file => file.Length);
                    if (connectionIds != null)
                        foreach (var connectionId in connectionIds)
                            context.Clients.Client(connectionId).deliveryStatus($"Creating Zip File For Total Size {size / (1000 * 1000)} MB");
                    ZipFile.CreateFromDirectory(zipDirectory, zipPath);
                    zipPaths.Add(zipPath);
                }
                return new ZipResultDTO { TanNumbers = tanNumbers, Path = Store.C.DELIVERY_PATH, Count = tanBuckets.Count };
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
            finally
            {
                if (connectionIds != null)
                    foreach (var connectionId in connectionIds)
                        context.Clients.Client(connectionId).deliveryStatus(SignalRCode.DONE);
            }
        }
        public string GenerateEmail(int id)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                List<Tan> tans;
                DeliveryBatch deliveryBatch;
                int totalReactions;
                int zeroRXNTANs;
                using (var db = new ApplicationDbContext())
                {
                    deliveryBatch = db.DeliveryBatches.Find(id);
                    tans = deliveryBatch.Tans.ToList();
                    totalReactions = tans.Select(t => t.RxnCount).Sum();
                    zeroRXNTANs = tans.Where(t => t.RxnCount == 0).Count();
                }
                sb.Append("<html>");
                sb.Append("<body style='font-family:Calibri;'>");
                sb.Append("<p>Comment:</p>");
                sb.Append($"<br>Batch number:{deliveryBatch.BatchNumber}");
                sb.Append($"<br>Number of TANs:{tans.Count}");
                sb.Append($"<br>Number of reactions:{totalReactions}");
                sb.Append($"<br>Number of TANs with no reactions:{zeroRXNTANs}</br>");
                sb.Append($"<br>Doc detail:</br>");
                sb.Append("<table style='font-family:Calibri;' cellpadding='2'>");
                sb.Append("<tbody>");
                foreach (var tan in tans)
                {
                    sb.Append("<tr>");
                    sb.Append("<td>");
                    sb.Append(tan.tanNumber);
                    sb.Append("</td>");
                    sb.Append("<td style='padding-left:5'>");
                    sb.Append(tan.RxnCount);
                    sb.Append("</td>");
                    sb.Append("</tr>");
                }
                sb.Append("</tbody>");
                sb.Append("</table>");
                sb.Append("<br/>");
                sb.Append("</body>");
                sb.Append("</html>");
                return sb.ToString();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }
        public List<DeliveryBatchDTO> Batches()
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                return db.DeliveryBatches.OrderByDescending(d => d.BatchNumber).Select(d => new DeliveryBatchDTO { BatchNumber = d.BatchNumber, Id = d.Id }).ToList();
            }
        }
        public List<DeliveryBatchDTO> BatchWiseTans()
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                return db.DeliveryBatches.Select(d => new DeliveryBatchDTO { Id = d.Id, BatchNumber = d.BatchNumber, TansCount = d.Tans != null ? d.Tans.Count : 0 }).ToList();
                //return db.Tans
                //    .Where(t => t.DeliveryBatches != null && t.DeliveryBatches.Any())
                //    .GroupBy(t => t.DeliveryBatch)
                //    .ToDictionary(d => d.Key, d => d.Count())
                //    .Select(d => new DeliveryBatchDTO { BatchNumber = d.Key.BatchNumber, Id = d.Key.Id, TansCount = d.Value })
                //    .ToList();
            }
        }
        public List<DeliveryTanDTO> TansOfDelivery(int id, bool isZeroRxns, bool isQueried)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var tans = db.Tans.Where(t => t.DeliveryBatches != null && t.DeliveryBatches.Select(d=>d.Id).Contains(t.Id));
                if (isZeroRxns)
                    tans = tans.Where(t => t.RxnCount == 0);
                if (isQueried)
                    tans = tans.Where(t => t.MarkedAsQuery);
                return tans.Select(t => new DeliveryTanDTO
                {
                    Id = t.Id,
                    IsQueried = t.MarkedAsQuery,
                    RXNCount = t.RxnCount,
                    TanNumber = t.tanNumber,
                    DeliveryRevertMessage = t.DeliveryRevertMessage
                }).ToList();
            }
        }
        public string GenerateNextBatchNumber(int Nextbatch)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                if (Nextbatch == 0)
                {
                    Nextbatch = db.DeliveryBatches.Any() ? db.DeliveryBatches.Select(d => d.BatchNumber).Max() + 1 : 1;
                    db.DeliveryBatches.Add(new DeliveryBatch { BatchNumber = Nextbatch, CreatedDate = DateTime.Now });
                }
                else
                {
                    db.DeliveryBatches.Add(new DeliveryBatch { BatchNumber = Nextbatch, CreatedDate = DateTime.Now });
                }
                db.SaveChanges();
                return $"New Delivery Batch {Nextbatch} Created Successfully";
            }
        }
        public string Revert(int id, Role role, string msg)
        {
            if (String.IsNullOrEmpty(msg))
                return "Delivery Status Message Is Required";

            using (TransactionScope scope = new TransactionScope())
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                Tan tan = db.Tans
                    .Include(t => t.Curator)
                    .Include(t => t.Reviewer)
                    .Include(t => t.QC)
                    .Include(t => t.Batch)
                    .Include(t => t.CurrentUser)
                    .Include(t => t.CurrentUserRole)
                    .Where(t => t.Id == id)
                    .FirstOrDefault();
                if (tan != null)
                {
                    string previousUserId = null;
                    if (role == Role.Curator)
                        previousUserId = tan.CuratorId;
                    else if (role == Role.Reviewer)
                        previousUserId = tan.ReviewerId;
                    else if (role == Role.QC)
                        previousUserId = tan.QCId;
                    if (!String.IsNullOrEmpty(previousUserId))
                    {
                        var userRole = db.UserRoles
                            .Include(ur => ur.ApplicationUser)
                            .Where(ur => ur.UserId == previousUserId && ur.Role == role)
                            .FirstOrDefault();
                        if (userRole != null)
                        {
                            tan.CurrentUserRoleId = userRole.Id;
                            tan.CurrentUserId = userRole.UserId;
                            tan.DeliveryRevertMessage = msg;
                            db.SaveChanges();
                            scope.Complete();
                            return $"TAN reverted successfully to {userRole.ApplicationUser.UserName} ({userRole.Role.DescriptionAttribute()})";
                        }
                        else
                            return "Role Not Found For Previous User";
                    }
                    else
                        return "Previous User Not Found";
                }
                else
                    return "Tan Not Found";
            }
        }
    }
}