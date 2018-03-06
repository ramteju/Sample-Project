using DTO;
using Entities;
using Entities.DTO;
using Newtonsoft.Json;
using ProductTracking.Models;
using ProductTracking.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using Excelra.Utils.Library;
using System.Data.Entity;
using ProductTracking.Logging;

namespace ProductTracking.Services.Core
{
    public class ShipmentService
    {
        private static HashSet<int> CategorySet(int tanCategory)
        {
            HashSet<int> categorySet = new HashSet<int>();
            if (tanCategory == (int)TanCategory.All)
            {
                foreach (var value in Enum.GetValues(typeof(TanCategory)))
                    categorySet.Add((int)value);
            }
            else if (tanCategory == (int)TanCategory.ReadyToDeliver)
            {
                categorySet.Add((int)TanCategory.Journals);
                categorySet.Add((int)TanCategory.Patents);
            }
            else
                categorySet.Add((int)tanCategory);
            return categorySet;
        }

        public ServiceResult LoadShipment()
        {
            try
            {
                ServiceResult result = new ServiceResult();
                result.IsSuccess = true;
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }
        public List<BatchDTO> Batches()
        {
            try
            {
                var shipmentList = new List<BatchDTO>();
                using (var db = new ApplicationDbContext())
                {
                    foreach (var batch in db.Batches)
                        shipmentList.Add(new BatchDTO
                        {
                            Id = batch.Id,
                            Name = batch.Name,
                            DateCreated = batch.DateCreated.Value,
                            DocumentsPath = batch.DocumentsPath
                        });
                }
                return shipmentList;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        public List<TanKeywords> TanKeyWords()
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                return db.TanKeywords.ToList();
            }
        }
        public List<BatchTanDto> TansBetweenBatches(int fromBatchNumber, int toBatchNumber, int tanCategory)
        {
            List<BatchTanDto> tans = new List<BatchTanDto>();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                HashSet<int> categorySet = CategorySet(tanCategory);
                List<int> tanIds = new List<int>();
                var DeliveryBatchTans = db.DeliveryBatches.Select(d => d.Tans.Select(t => t.Id)).ToList();
                foreach (var ids in DeliveryBatchTans)
                    tanIds.AddRange(ids);
                var tanDatas = db.TanData
                    .Include(td => td.Tan.Batch)
                    .Include(td => td.Tan.Curator)
                    .Include(td => td.Tan.Reviewer)
                    .Include(td => td.Tan.QC)
                    .Include(td => td.Tan.CurrentUserRole)
                    .Where(t => !tanIds.Contains(t.Tan.Id) && t.Tan.Batch.Name >= fromBatchNumber && t.Tan.Batch.Name <= toBatchNumber && categorySet.Contains((int)t.Tan.TanCategory));

                foreach (var tanData in tanDatas.ToList())
                {
                    var tan = db.Tans.Include("CurrentUserRole").Include("CurrentUser").Include("Curator").Include("Reviewer").Include("QC").Where(t => t.Id == tanData.Tan.Id).FirstOrDefault();
                    var tanDto = new BatchTanDto();
                    tanDto.Id = tanData.Tan.Id;
                    tanDto.TanNumber = tanData.Tan.tanNumber;
                    tanDto.BatchNumber = tanData.Tan.Batch.Name;
                    tanDto.TanType = tanData.Tan.TanType;
                    tanDto.TanState = tan.TanState;
                    tanDto.TanCategory = tanData.Tan.TanCategory;
                    tanDto.Curator = tan.Curator?.UserName;
                    tanDto.Reviewer = tan.Reviewer?.UserName;
                    tanDto.QC = tan.QC?.UserName;
                    if (tan.CurrentUserRole != null)
                        tanDto.CurrentRole = tan.CurrentUserRole.Role;
                    var serialisedtan = JsonConvert.DeserializeObject<Tan>(tanData.Data);
                    //XmlUtils.LoadMasterData(tanData);
                    tanDto.Nums = serialisedtan.TanChemicals.Count;
                    tanDto.Rxns = serialisedtan.Reactions.Count;
                    tanDto.Stages = serialisedtan.Reactions.Select(r => r.Stages.Count).Sum();
                    tanDto.NearToTargetDate = (tan.TargetedDate.HasValue && (tan.TargetedDate.Value - DateTime.Now).TotalDays <= 5) ? true : false;
                    tanDto.ProcessingNote = tan.ProcessingNode;
                    tanDto.TargetDate = tanData.Tan.TargetedDate.HasValue ? tanData.Tan.TargetedDate.Value.ToString("dd/MM/YYYY") : string.Empty;
                    tanDto.IsDoubtRaised = db.Queries.Select(q => q.TanId).Contains(tanData.Tan.Id);
                    tans.Add(tanDto);
                }
            }
            return tans;
        }

        internal string UpdateDeliveryStatsu(int batchId)
        {
            using (TransactionScope sc = new TransactionScope())
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var deliveryBatch = db.DeliveryBatches.Where(d => d.Id == batchId).FirstOrDefault();
                if(deliveryBatch!=null)
                {
                    deliveryBatch.Delivered = true;
                    deliveryBatch.DeliveredDate = DateTime.Now;
                }
                db.SaveChanges();
                sc.Complete();
                return "Delivery status updated successfully";
            }
        }

        public List<BatchTanDto> TansFromBatches(List<int> batchNos, int tanCategory)
        {
            List<BatchTanDto> tans = new List<BatchTanDto>();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                HashSet<int> categorySet = CategorySet(tanCategory);
                List<int> tanIds = new List<int>();
                var DeliveryBatchTans = db.DeliveryBatches.Select(d => d.Tans.Select(t => t.Id)).ToList();
                foreach (var ids in DeliveryBatchTans)
                    tanIds.AddRange(ids);

                var tanDatas = db.TanData
                    .Include(td => td.Tan.Batch)
                    .Include(td => td.Tan.Curator)
                    .Include(td => td.Tan.Reviewer)
                    .Include(td => td.Tan.QC)
                    .Include(td => td.Tan.CurrentUserRole)
                    .Where(t => !tanIds.Contains(t.Tan.Id) && batchNos.Contains(t.Tan.Batch.Name) && categorySet.Contains((int)t.Tan.TanCategory));

                foreach (var tanData in tanDatas.ToList())
                {
                    var tan = db.Tans.Include("CurrentUserRole").Include("CurrentUser").Include("Curator").Include("Reviewer").Include("QC").Where(t => t.Id == tanData.Tan.Id).FirstOrDefault();
                    var tanDto = new BatchTanDto();
                    tanDto.Id = tanData.Tan.Id;
                    tanDto.TanNumber = tanData.Tan.tanNumber;
                    tanDto.BatchNumber = tanData.Tan.Batch.Name;
                    tanDto.TanType = tanData.Tan.TanType;
                    tanDto.TanState = tan.TanState;
                    tanDto.TanCategory = tanData.Tan.TanCategory;
                    tanDto.Curator = tan.Curator?.UserName;
                    tanDto.Reviewer = tan.Reviewer?.UserName;
                    tanDto.QC = tan.QC?.UserName;
                    if (tan.CurrentUserRole != null)
                        tanDto.CurrentRole = tan.CurrentUserRole.Role;
                    var serialisedtan = JsonConvert.DeserializeObject<Tan>(tanData.Data);
                    //XmlUtils.LoadMasterData(tanData);
                    tanDto.Nums = serialisedtan.TanChemicals.Count;
                    tanDto.Rxns = serialisedtan.Reactions.Count;
                    tanDto.Version = serialisedtan.Version.HasValue ? serialisedtan.Version.Value : 0;
                    tanDto.Stages = serialisedtan.Reactions.Select(r => r.Stages.Count).Sum();
                    tanDto.NearToTargetDate = (tan.TargetedDate.HasValue && (tan.TargetedDate.Value - DateTime.Now).TotalDays <= 5) ? true : false;
                    tanDto.TargetDate = tanData.Tan.TargetedDate.HasValue ? tanData.Tan.TargetedDate.Value.ToString("dd/MM/yyyy") : string.Empty;
                    tanDto.ProcessingNote = tan.ProcessingNode;
                    tanDto.IsDoubtRaised = db.Queries.Select(q => q.TanId).Contains(tanData.Tan.Id);
                    tans.Add(tanDto);
                }
            }
            return tans;
        }

        public string MoveTansToCategory(MoveTansDTO moveTansDto)
        {
            using (TransactionScope scope = new TransactionScope())
            using (var db = new ApplicationDbContext())
            {
                var tansToMove = db.Tans.Include("Batch").Where(t => moveTansDto.TanIds.Contains(t.Id)).ToList();
                foreach (var tan in tansToMove)
                    tan.TanCategory = (TanCategory)moveTansDto.TargetCategory;
                db.SaveChanges();
                scope.Complete();
                return $"{tansToMove.Count} Tans Moved Successfully";
            }
        }
        public string MoveTansToDelivery(MoveTansDTO moveTansDto)
        {
            using (TransactionScope scope = new TransactionScope())
            using (var db = new ApplicationDbContext())
            {
                var tans = db.Tans.Include("Batch").Where(t => moveTansDto.TanIds.Contains(t.Id)).ToList();
                var DeliveryBatch = db.DeliveryBatches.Where(t => t.Id == moveTansDto.TargetCategory).FirstOrDefault();
                if (DeliveryBatch != null)
                    DeliveryBatch.Tans = tans;
                //var tansToMove = db.Tans.Include("Batch").Where(t => moveTansDto.TanIds.Contains(t.Id)).ToList();
                //foreach (var tan in tansToMove)
                //    tan.DeliveryBatchId = moveTansDto.TargetCategory;
                db.SaveChanges();
                scope.Complete();
                return $"{tans.Count} Tans Moved Successfully";
            }
        }
        public List<S8000NameLocationDTO> S8000NameLocations(int batchId, int tanCategory)
        {
            try
            {
                var result = new List<S8000NameLocationDTO>();
                using (var db = new ApplicationDbContext())
                {
                    HashSet<int> categorySet = CategorySet(tanCategory);
                    var DeliveryTans = db.DeliveryBatches.Where(d => d.Id == batchId).Select(d => d.Tans).ToList();
                    List<Tan> tans = new List<Tan>();
                    foreach (var tan in DeliveryTans)
                        tans.AddRange(tan);
                    var TanIDs = tans.Select(t => t.Id).ToList();
                    var tanDatas = db.TanData.Where(t => TanIDs.Contains(t.Tan.Id) && categorySet.Contains((int)t.Tan.TanCategory)).ToList();
                    foreach (var tanData in tanDatas)
                    {
                        string category = tanData.Tan.TanCategory.DescriptionAttribute();
                        XmlUtils.LoadMasterData(tanData);
                        var s8000TanChemicals = tanData.Tan.TanChemicals.Where(p => p.ChemicalType == ChemicalType.S8000);
                        foreach (var s8000TanChemical in s8000TanChemicals)
                        {
                            if (s8000TanChemical.MetaData != null)
                            {
                                foreach (var metaData in s8000TanChemical.MetaData)
                                {
                                    S8000NameLocationDTO dto = new S8000NameLocationDTO()
                                    {
                                        TanNumber = tanData.Tan.tanNumber,
                                        TanSeries = s8000TanChemical.NUM,
                                        TanCategory = category,
                                        SubstanceName = s8000TanChemical.Name,
                                        SubstanceLocation = metaData.PageNo
                                    };
                                    result.Add(dto);
                                }
                            }
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }
        public List<S8580CommentsDTO> S8580Comments(int batchId, int tanCategory)
        {
            try
            {
                var result = new List<S8580CommentsDTO>();
                using (var db = new ApplicationDbContext())
                {
                    var categorySet = CategorySet(tanCategory);
                    var DeliveryTans = db.DeliveryBatches.Where(d => d.Id == batchId).Select(d => d.Tans).ToList();
                    List<Tan> tans = new List<Tan>();
                    foreach (var tan in DeliveryTans)
                        tans.AddRange(tan);
                    var TanIDs = tans.Select(t => t.Id).ToList();
                    var tanDatas = db.TanData.Where(t => TanIDs.Contains(t.Tan.Id) && categorySet.Contains((int)t.Tan.TanCategory)).ToList();
                    foreach (var tanData in tanDatas)
                    {
                        string category = tanData.Tan.TanCategory.DescriptionAttribute();
                        XmlUtils.LoadMasterData(tanData);
                        foreach (var comment in tanData.Tan.TanComments)
                        {
                            S8580CommentsDTO dto = new S8580CommentsDTO()
                            {
                                TanNumber = tanData.Tan.tanNumber,
                                TanCategory = category,
                                CommentType = comment.CommentType.DescriptionAttribute(),
                                Comment = comment.Comment,
                                UserComment = String.Empty
                            };
                            result.Add(dto);
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }
        public List<ExtractRSNDto> ExtractRSN(int batchId, int tanCategory)
        {
            try
            {
                var result = new List<ExtractRSNDto>();
                using (var db = new ApplicationDbContext())
                {
                    var categorySet = CategorySet(tanCategory);
                    var DeliveryTans = db.DeliveryBatches.Where(d => d.Id == batchId).Select(d=>d.Tans).ToList();
                    List<Tan> tans = new List<Tan>();
                    foreach (var tan in DeliveryTans)
                        tans.AddRange(tan);
                    var TanIDs = tans.Select(t => t.Id).ToList();
                    var tanDatas = db.TanData.Where(t => TanIDs.Contains(t.Tan.Id) && categorySet.Contains((int)t.Tan.TanCategory)).ToList();
                    foreach (var tanData in tanDatas)
                    {
                        string category = tanData.Tan.TanCategory.DescriptionAttribute();
                        XmlUtils.LoadMasterData(tanData);
                        foreach (var rsn in tanData.Tan.RSNs)
                        {
                            ExtractRSNDto dto = new ExtractRSNDto()
                            {
                                TanNumber = rsn.Tan.tanNumber,
                                RXNSno = rsn.Reaction.DisplayOrder,
                                ProductNumber = rsn.Reaction.KeyProductNum,
                                RxnSeq = rsn.Reaction.KeyProductSequence,
                                Stage = rsn.Stage?.DisplayOrder,
                                CVT = rsn.CVT,
                                FreeText = rsn.FreeText,
                                RSNType = rsn.Level,
                                Id = rsn.Id,
                            };
                            result.Add(dto);
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }
        public bool UpdateBulkFreeText(FreeTextBulkDto bulkDto, string userName, string IP)
        {
            using (TransactionScope scope = new TransactionScope())
            using (var db = new ApplicationDbContext())
            {
                var categorySet = CategorySet(bulkDto.CategoryId);
                var DeliveryTans = db.DeliveryBatches.Where(d => d.Id == bulkDto.BatchId).Select(d => d.Tans).ToList();
                List<Tan> tans = new List<Tan>();
                foreach (var tan in DeliveryTans)
                    tans.AddRange(tan);
                var TanIDs = tans.Select(t => t.Id).ToList();
                var tanDatas = db.TanData.Where(t => TanIDs.Contains(t.Tan.Id) && categorySet.Contains((int)t.Tan.TanCategory)).ToList();
                Dictionary<Guid, string> rsnIdWiseFreeText = bulkDto.Dtos.ToDictionary(d => d.Id, d => d.FreeText);
                int count = 0;
                foreach (var tanData in tanDatas)
                {
                    bool needToUpdate = false;
                    var deserializedTan = JsonConvert.DeserializeObject<Tan>(tanData.Data);
                    foreach (var rsn in deserializedTan.RSNs)
                    {
                        if (rsnIdWiseFreeText.ContainsKey(rsn.Id))
                        {
                            rsn.FreeText = rsnIdWiseFreeText[rsn.Id];
                            needToUpdate = true;
                            count++;
                        }
                    }
                    if (needToUpdate)
                    {
                        tanData.Data = JsonConvert.SerializeObject(deserializedTan);
                        tanData.Date = DateTime.Now;
                        tanData.User = userName;
                        tanData.Ip = IP;
                    }
                }
                db.SaveChanges();
                scope.Complete();
            }
            return true;
        }

        public bool UpdateReviewAllowTag(List<int> batchIds)
        {
            //using (TransactionScope scope = new TransactionScope())
            //using (var db = new ApplicationDbContext())
            //{
            //    foreach (var batchnum in batchIds)
            //    {
            //        var batch = db.Batches.Where(b => b.Name == batchnum).FirstOrDefault();
            //        if(batch!=null)
            //            batch.AllowForReview = true;
            //    }
            //    db.SaveChanges();
            //    scope.Complete();
            //}
            return true;
        }
        public ShippmentUploadStatus GetShipmentUploadStatus()
        {
            ShippmentUploadStatus ShippmentUploadStatus = null;
            using (TransactionScope scope = new TransactionScope())
            using (var db = new ApplicationDbContext())
            {
                ShippmentUploadStatus = db.ShippmentUploadStatus.Where(sp => sp.Status == ShippmentUploadEnumStatus.ProcessCompleted.ToString()).FirstOrDefault();
            }
            return ShippmentUploadStatus;
        }
        public bool UpdateShipmentUploadStatus(ShippmentUploadStatus ShippmentUploadStatus)
        {
            using (TransactionScope scope = new TransactionScope())
            using (var db = new ApplicationDbContext())
            {

                var DBShippmentUploadStatus = db.ShippmentUploadStatus.Where(sp => sp.Id == ShippmentUploadStatus.Id).FirstOrDefault();
                if (DBShippmentUploadStatus != null)
                    DBShippmentUploadStatus.Status = ShippmentUploadStatus.Status;
                db.SaveChanges();
                scope.Complete();
            }
            return true;
        }
    }
}