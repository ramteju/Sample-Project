using Client.Logging;
using Client.Models;
using Client.ViewModels;
using Client.ViewModels.Core;
using Client.ViewModels.Delivery;
using Client.ViewModels.Tasks;
using Client.Views;
using DTO;
using ProductTracking.Models.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Client.Util;
using Client.ViewModels.Reports;
using Entities;

namespace Client.Common
{
    public static class ViewModelToModel
    {
        public static Tan GetTanFromViewModel(MainVM mainViewModel)
        {
            var tan = new Tan();
            try
            {
                var ReactionParticipants = new List<ReactionRSD>();
                var reactions = new List<Reaction>();
                var Participants = new List<ReactionRSD>();
                var RSNs = new List<ProductTracking.Models.Core.ReactionRSN>();
                tan.TanChemicals = mainViewModel.TanVM.TanChemicals;
                List<string> TanTempDefaultComments = new List<string>();
                #region Reactions
                foreach (var reaction in mainViewModel.TanVM.Reactions)
                {
                    var reactionYield = mainViewModel.TanVM.ReactionParticipants.OfReactionOfType(reaction.Id, ParticipantType.Product).Select(rp => rp.ReactionYield).FirstOrDefault();
                    List<string> PORMComments = new List<string>();
                    var dbreaction = new Reaction
                    {
                        Id = reaction.Id,
                        DisplayOrder = reaction.DisplayOrder,
                        Name = reaction.Name,
                        TanId = reaction.TanVM.Id,
                        AnalogousFromId = reaction.AnalogousVMId,
                        IsCurationCompleted = reaction.IsCurationCompleted,
                        IsReviewCompleted = reaction.IsReviewCompleted,
                        Yield = reactionYield,
                        Tan = tan,
                        CuratorCreatedDate = reaction.CuratorCreatedDate,
                        LastUpdatedDate = reaction.LastupdatedDate,
                        CuratorCompletedDate = reaction.CuratorCompletedDate,
                        ReviewerCreatedDate = reaction.ReviewerCreatedDate,
                        ReviewLastUpdatedDate = reaction.ReviewLastupdatedDate,
                        ReviewerCompletedDate = reaction.ReviewerCompletedDate,
                        QCLastUpdatedDate = reaction.QCLastupdatedDate,
                        QCCompletedDate = reaction.QcCompletedDate
                    };
                    var stages = new List<Stage>();
                    foreach (var stage in reaction.Stages)
                    {
                        var conditions = new List<ProductTracking.Models.Core.StageCondition>();
                        if (stage.Conditions != null)
                        {
                            foreach (var condition in stage.Conditions)
                            {
                                var dbcondition = new ProductTracking.Models.Core.StageCondition
                                {
                                    Id = condition.Id,
                                    DisplayOrder = condition.DisplayOrder,
                                    PH = condition.PH,
                                    Pressure = condition.Pressure,
                                    StageId = stage.Id,
                                    Temperature = condition.Temperature,
                                    Time = condition.Time,
                                    TEMP_TYPE = condition.TEMP_TYPE,
                                    TIME_TYPE = condition.TIME_TYPE,
                                    PH_TYPE = condition.PH_TYPE,
                                    PRESSURE_TYPE = condition.PRESSURE_TYPE
                                };
                                if (condition.TEMP_TYPE == TemperatureEnum.pluseDevminus.ToString())
                                {
                                    PORMComments.Add(" needs TP=" + condition.Temperature);
                                }
                                if (dbcondition.Id == Guid.Empty) { }
                                else
                                {
                                    conditions.Add(dbcondition);
                                }
                            }
                        }
                        var dbstage = new Stage
                        {
                            Id = stage.Id,
                            DisplayOrder = stage.DisplayOrder,
                            Name = stage.Name,
                            ReactionId = reaction.Id,
                            StageConditions = conditions
                        };
                        stages.Add(dbstage);
                    }
                    dbreaction.Stages = stages;
                    reactions.Add(dbreaction);
                    if (PORMComments.Count > 0)
                        TanTempDefaultComments.Add("NUM-SEQ " + reaction.KeyProductSeq + string.Join(", ", PORMComments));
                }
                tan.Reactions = reactions;

                #region TanDefaultComments
                List<ViewModels.Core.Comments> tempComments = mainViewModel.TanVM.TanComments.TanComments.Where(tc => tc.CommentType == CommentType.TEMPERATURE).ToList();
                foreach (var item in tempComments)
                    mainViewModel.TanVM.TanComments.TanComments.Remove(item);
                if (TanTempDefaultComments.Count > 0)
                    mainViewModel.TanVM.TanComments.TanComments.Add(new ViewModels.Core.Comments
                    {
                        Comment = string.Join(". ", TanTempDefaultComments),
                        TotalComment = string.Join(". ", TanTempDefaultComments),
                        CommentType = CommentType.TEMPERATURE,
                        Id = Guid.NewGuid()
                    });
                List<ViewModels.Core.Comments> defaultComments = mainViewModel.TanVM.TanComments.TanComments.Where(tc => tc.CommentType == CommentType.DEFAULT).ToList();
                foreach (var item in defaultComments)
                    mainViewModel.TanVM.TanComments.TanComments.Remove(item);


                var _8500Series = mainViewModel.TanVM.ReactionParticipants.OfType(ParticipantType.Product, ParticipantType.Reactant).OfChemicalType(ChemicalType.S8500);
                var _8000Series = mainViewModel.TanVM.ReactionParticipants.OfType(ParticipantType.Product, ParticipantType.Reactant).OfChemicalType(ChemicalType.S8000);

                if (_8500Series.Where(p => p.ParticipantType == ParticipantType.Product).Count() > 0 && _8500Series.Where(p => p.ParticipantType == ParticipantType.Reactant).Count() > 0)
                {
                    mainViewModel.TanVM.TanComments.TanComments.Add(new ViewModels.Core.Comments { Comment = "8500 used as product and reactant", TotalComment = "8500 used as product and reactant", CommentType = CommentType.DEFAULT, Id = Guid.NewGuid() });
                }
                if (_8000Series.Where(p => p.ParticipantType == ParticipantType.Product).Count() > 0 && _8000Series.Where(p => p.ParticipantType == ParticipantType.Reactant).Count() > 0)
                {
                    mainViewModel.TanVM.TanComments.TanComments.Add(new ViewModels.Core.Comments { Comment = "8000 used as product and reactant", TotalComment = "8000 used as product and reactant", CommentType = CommentType.DEFAULT, Id = Guid.NewGuid() });
                }

                if (_8500Series.Where(p => p.ParticipantType == ParticipantType.Product).Count() > 0 && _8500Series.Where(p => p.ParticipantType == ParticipantType.Reactant).Count() == 0)
                {
                    mainViewModel.TanVM.TanComments.TanComments.Add(new ViewModels.Core.Comments { Comment = "8500 used as product", TotalComment = "8500 used as product", CommentType = CommentType.DEFAULT, Id = Guid.NewGuid() });
                }
                if (_8000Series.Where(p => p.ParticipantType == ParticipantType.Product).Count() > 0 && _8000Series.Where(p => p.ParticipantType == ParticipantType.Reactant).Count() == 0)
                {
                    mainViewModel.TanVM.TanComments.TanComments.Add(new ViewModels.Core.Comments { Comment = "8000 used as product", TotalComment = "8000 used as product", CommentType = CommentType.DEFAULT, Id = Guid.NewGuid() });
                }

                if (_8500Series.Where(p => p.ParticipantType == ParticipantType.Product).Count() == 0 && _8500Series.Where(p => p.ParticipantType == ParticipantType.Reactant).Count() > 0)
                {
                    mainViewModel.TanVM.TanComments.TanComments.Add(new ViewModels.Core.Comments { Comment = "8500 used as reactant", TotalComment = "8500 used as reactant", CommentType = CommentType.DEFAULT, Id = Guid.NewGuid() });
                }
                if (_8000Series.Where(p => p.ParticipantType == ParticipantType.Product).Count() == 0 && _8000Series.Where(p => p.ParticipantType == ParticipantType.Reactant).Count() > 0)
                {
                    mainViewModel.TanVM.TanComments.TanComments.Add(new ViewModels.Core.Comments { Comment = "8000 used as reactant", TotalComment = "8000 used as reactant", CommentType = CommentType.DEFAULT, Id = Guid.NewGuid() });
                }
                #endregion

                #endregion

                #region Participants
                if (mainViewModel.TanVM.ReactionParticipants != null)
                {
                    foreach (var participant in mainViewModel.TanVM.ReactionParticipants)
                    {
                        var dbParticipant = new ReactionRSD
                        {
                            Id = participant.Id,
                            DisplayYield = participant.Yield,
                            Yield = participant.ProductYield,
                            DisplayOrder = participant.DisplayOrder,
                            ParticipantId = participant.TanChemicalId,
                            ParticipantType = participant.ParticipantType,
                            ReactionId = participant.ReactionVM.Id,
                            StageId = participant.StageVM?.Id,
                            TanId = mainViewModel.TanVM.Id,
                            KeyProduct = participant.KeyProduct,
                            KeyProductSeq = participant.KeyProductSeqWithOutNum,
                            Name = participant.Name
                        };
                        Participants.Add(dbParticipant);
                    }
                }
                tan.Participants = Participants;
                #endregion

                #region RSN
                foreach (var comment in mainViewModel.TanVM.Rsns)
                {
                    var dbcomment = new ProductTracking.Models.Core.ReactionRSN
                    {
                        CVT = comment.CvtText,
                        FreeText = comment.FreeText,
                        ReactionId = comment.Reaction.Id,
                        StageId = comment.Stage != null ? comment.Stage?.Id : Guid.Empty,
                        TanId = mainViewModel.TanVM.Id,
                        Id = comment.Id,
                        DisplayOrder = comment.DisplayOrder,
                        IsIgnorableInDelivery = comment.IsIgnorableInDelivery,
                        ReactionParticipantId = comment.ReactionParticipantId
                    };
                    RSNs.Add(dbcomment);
                }
                tan.RSNs = RSNs;
                #endregion

                tan.Id = mainViewModel.TanVM.Id;
                tan.tanNumber = mainViewModel.TanVM.TanNumber;
                tan.DocumentPath = mainViewModel.TanVM.DocumentPath;
                tan.IsQCCompleted = mainViewModel.TanVM.IsQCCompleted;
                tan.CAN = mainViewModel.TanVM.CanNumber;
                #region TanComments
                var tancomments = new List<ProductTracking.Models.Core.Comments>();
                foreach (var com in mainViewModel.TanVM.TanComments.TanComments)
                {
                    var comment = new ProductTracking.Models.Core.Comments
                    {
                        Comment = com.Comment,
                        TotalComment = com.TotalComment,
                        CommentType = com.CommentType,
                        Id = com.Id,
                        Length = com.TotalComment != null ? com.TotalComment.Length : 0,
                        Column = com.Column,
                        Figure = com.Figure,
                        FootNote = com.FootNote,
                        Line = com.Line,
                        Num = com.Num,
                        Page = com.Page,
                        Para = com.Para,
                        Schemes = com.Schemes,
                        Sheet = com.Sheet,
                        Table = com.Table,
                    };
                    tancomments.Add(comment);
                }
                if (tan.Reactions.Count == 0)
                {
                    if (tancomments.Where(tc => tc.Comment.ToLower() == "No Reactions in the Article".ToLower()).Count() == 0)
                        tancomments.Add(new ProductTracking.Models.Core.Comments { Comment = "No reactions in the article", TotalComment = "No reactions in the article", CommentType = CommentType.DEFAULT, Id = Guid.NewGuid(), Length = "No reactions in the article".Length });
                }
                else
                {
                    var comment = tancomments.Where(tc => tc.Comment.ToLower() == "No reactions in the article".ToLower()).FirstOrDefault();
                    tancomments.Remove(comment);
                }
                tan.TanComments = tancomments;
                #endregion
                tan.Batch = new ProductTracking.Models.Core.Batch
                {
                    Id = mainViewModel.Batch.Id,
                    DateCreated = mainViewModel.Batch.DateCreated,
                    DocumentsPath = mainViewModel.Batch.DocumentsPath,
                    GifImagesPath = mainViewModel.Batch.GifImagesPath,
                    Name = mainViewModel.Batch.Name
                };
            }
            catch (Exception ex)
            {
                Log.This(ex);
                AppErrorBox.ShowErrorMessage("Can't Get TAN From View . .", ex.ToString());
            }
            return tan;
        }

        public static TanVM GetTanVMFromTan(Tan MasterTan, Tan serializedTan)
        {
            var tanComments = new ObservableCollection<ViewModels.Core.Comments>();
            foreach (var comment in serializedTan.TanComments)
            {
                var com = new ViewModels.Core.Comments
                {
                    Comment = comment.Comment,
                    TotalComment = comment.TotalComment,
                    CommentType = comment.CommentType,
                    Id = comment.Id,
                    Column = comment.Column,
                    Figure = comment.Figure,
                    FootNote = comment.FootNote,
                    Line = comment.Line,
                    Num = comment.Num,
                    Page = comment.Page,
                    Para = comment.Para,
                    Schemes = comment.Schemes,
                    Sheet = comment.Sheet,
                    Table = comment.Table
                };
                tanComments.Add(com);
            }
            var tanVM = new TanVM
            {
                Id = MasterTan.Id,
                TanNumber = MasterTan.tanNumber,
                BatchNumber = MasterTan.Batch.Name,
                DocumentPath = MasterTan.DocumentPath,
                TanComments = new TanCommentsVM(tanComments),
                IsQCCompleted = serializedTan.IsQCCompleted
            };
            return tanVM;
        }

        public static Tuple<TanChemicalVM, ObservableCollection<ImagesVM>> GetTanChemicalVMFromTanchemical(TanChemical num)
        {
            var chemical = new TanChemicalVM();
            var images = new ObservableCollection<ImagesVM>();
            Tuple<TanChemicalVM, ObservableCollection<ImagesVM>> returnObjet;
            try
            {
                chemical.ChemicalType = num.ChemicalType;
                chemical.MolString = num.MolString;
                chemical.RegNumber = num.RegNumber;
                chemical.ImagePath = num.ChemicalType == ChemicalType.NUM ? num.FirstImagePath : num.ImagePath;
                chemical.AllImagePaths = num.Substancepaths.Select(s => s.ImagePath).Distinct().ToList();
                if (chemical.AllImagePaths == null || chemical.AllImagePaths.Count() == 0)
                {
                    images.Add(new ImagesVM { ChemicalType = num.ChemicalType, ImagePath = null, MolString = num.MolString, RegNumber = num.RegNumber });
                }
                else
                {
                    foreach (var path in chemical.AllImagePaths)
                    {
                        images.Add(new ImagesVM { ChemicalType = num.ChemicalType, ImagePath = path, MolString = num.MolString, RegNumber = num.RegNumber });
                    }
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
            returnObjet = new Tuple<TanChemicalVM, ObservableCollection<ImagesVM>>(chemical, images);
            return returnObjet;
        }

        public static ErrorReportVM GetErrorReportVMFromModel(ErrorReport ErrorReport)
        {
            ErrorReportVM vm = new ViewModels.Reports.ErrorReportVM();
            vm.User1 = ErrorReport.Role1Name;
            vm.User2 = ErrorReport.Role2Name;
            vm.ErrorReport = ErrorReport;
            ErrorReportData data = new ErrorReportData();
            data.DataType = "Reactions";
            data.AddedCount = ErrorReport.AddedReactions;
            data.DeletedCount = ErrorReport.DeletedReactions;
            data.UpdatedCount = 0;
            data.CommonCount = ErrorReport.CommonReactions;
            data.Percentage = (double)((double)ErrorReport.CommonReactions / (double)(ErrorReport.AddedReactions + ErrorReport.DeletedReactions + ErrorReport.CommonReactions)) * 100;
            vm.ErrorReportData.Add(data);
            data = new ErrorReportData();
            data.DataType = "Stages";
            data.AddedCount = ErrorReport.AddedStages;
            data.DeletedCount = ErrorReport.DeletedStages;
            data.UpdatedCount = 0;
            data.CommonCount = ErrorReport.CommonStages;
            data.Percentage = (double)((double)ErrorReport.CommonStages / (double)(ErrorReport.AddedStages + ErrorReport.DeletedStages + ErrorReport.CommonStages)) * 100;
            vm.ErrorReportData.Add(data);
            data = new ErrorReportData();
            data.DataType = "Products";
            data.AddedCount = ErrorReport.AddedProducts;
            data.DeletedCount = ErrorReport.DeletedProducts;
            data.UpdatedCount = ErrorReport.UpdatedProducts;
            data.CommonCount = ErrorReport.CommonProducts;
            data.Percentage = (double) ((double)ErrorReport.CommonProducts / (double)(ErrorReport.AddedProducts + ErrorReport.DeletedProducts + ErrorReport.UpdatedProducts + ErrorReport.CommonProducts)) * 100;
            vm.ErrorReportData.Add(data);
            data = new ErrorReportData();
            data.DataType = "Reactants";
            data.AddedCount = ErrorReport.AddedReactants;
            data.DeletedCount = ErrorReport.DeletedReactants;
            data.UpdatedCount = ErrorReport.UpdatedReactants;
            data.CommonCount = ErrorReport.CommonReactants;
            data.Percentage = (double) ((double)ErrorReport.CommonReactants / (double)(ErrorReport.AddedReactants + ErrorReport.DeletedReactants + ErrorReport.UpdatedReactants + ErrorReport.CommonReactants)) * 100;
            vm.ErrorReportData.Add(data);
            data = new ErrorReportData();
            data.DataType = "Solvents";
            data.AddedCount = ErrorReport.AddedSolvents;
            data.DeletedCount = ErrorReport.DeletedSolvents;
            data.UpdatedCount = ErrorReport.UpdatedSolvents;
            data.CommonCount = ErrorReport.CommonSolvents;
            data.Percentage = (double) ((double)ErrorReport.CommonSolvents / (double)(ErrorReport.AddedSolvents + ErrorReport.DeletedSolvents + ErrorReport.UpdatedSolvents + ErrorReport.CommonSolvents)) * 100;
            vm.ErrorReportData.Add(data);
            data = new ErrorReportData();
            data.DataType = "Agents";
            data.AddedCount = ErrorReport.AddedAgents;
            data.DeletedCount = ErrorReport.DeletedAgents;
            data.UpdatedCount = ErrorReport.UpdatedAgents;
            data.CommonCount = ErrorReport.CommonAgents;
            data.Percentage = (double) ((double)ErrorReport.CommonAgents / (double)(ErrorReport.AddedAgents + ErrorReport.DeletedAgents + ErrorReport.UpdatedAgents + ErrorReport.CommonAgents)) * 100;
            vm.ErrorReportData.Add(data);
            data = new ErrorReportData();
            data.DataType = "Catalyst";
            data.AddedCount = ErrorReport.AddedCatalysts;
            data.DeletedCount = ErrorReport.DeletedCatalysts;
            data.UpdatedCount = ErrorReport.UpdatedCatalysts;
            data.CommonCount = ErrorReport.CommonCatalysts;
            data.Percentage = (double) ((double)ErrorReport.CommonCatalysts / (double)(ErrorReport.AddedCatalysts + ErrorReport.DeletedCatalysts + ErrorReport.UpdatedCatalysts + ErrorReport.CommonCatalysts)) * 100;
            vm.ErrorReportData.Add(data);
            data = new ErrorReportData();
            data.DataType = "Time";
            data.AddedCount = ErrorReport.AddedTime;
            data.DeletedCount = ErrorReport.DeletedTime;
            data.UpdatedCount = ErrorReport.UpdatedTime;
            data.CommonCount = ErrorReport.CommonTime;
            data.Percentage = (double) ((double)ErrorReport.CommonTime / (double)(ErrorReport.AddedTime + ErrorReport.DeletedTime + ErrorReport.UpdatedTime + ErrorReport.CommonTime)) * 100;
            vm.ErrorReportData.Add(data);
            data = new ErrorReportData();
            data.DataType = "Temperature";
            data.AddedCount = ErrorReport.AddedTemperature;
            data.DeletedCount = ErrorReport.DeletedTemperature;
            data.UpdatedCount = ErrorReport.UpdatedTemperature;
            data.CommonCount = ErrorReport.CommonTemperature;
            data.Percentage = (double) ((double)ErrorReport.CommonTemperature / (double)(ErrorReport.AddedTemperature + ErrorReport.DeletedTemperature + ErrorReport.UpdatedTemperature + ErrorReport.CommonTemperature)) * 100;
            vm.ErrorReportData.Add(data);
            data = new ErrorReportData();
            data.DataType = "Pressure";
            data.AddedCount = ErrorReport.AddedPressure;
            data.DeletedCount = ErrorReport.DeletedPressure;
            data.UpdatedCount = ErrorReport.UpdatedPressure;
            data.CommonCount = ErrorReport.CommonPressure;
            data.Percentage = (double) ((double)ErrorReport.CommonPressure / (double)(ErrorReport.AddedPressure + ErrorReport.DeletedPressure + ErrorReport.UpdatedPressure + ErrorReport.CommonPressure)) * 100;
            vm.ErrorReportData.Add(data);
            data = new ErrorReportData();
            data.DataType = "pH";
            data.AddedCount = ErrorReport.AddedpH;
            data.DeletedCount = ErrorReport.DeletedpH;
            data.UpdatedCount = ErrorReport.UpdatedpH;
            data.CommonCount = ErrorReport.CommonpH;
            data.Percentage = (double) ((double)ErrorReport.CommonpH / (double)(ErrorReport.AddedpH + ErrorReport.DeletedpH + ErrorReport.UpdatedpH + ErrorReport.CommonpH)) * 100;
            vm.ErrorReportData.Add(data);
            data = new ErrorReportData();
            data.DataType = "Comments";
            data.AddedCount = ErrorReport.AddedComments;
            data.DeletedCount = ErrorReport.DeletedComments;
            data.UpdatedCount = 0;
            data.CommonCount = ErrorReport.CommonComments;
            data.Percentage = (double) ((double)ErrorReport.CommonComments / (double)(ErrorReport.AddedComments + ErrorReport.DeletedComments + ErrorReport.UpdatedComments + ErrorReport.CommonComments)) * 100;
            vm.ErrorReportData.Add(data);
            //data = new ErrorReportData();
            //data.DataType = "Rsns";
            //data.AddedCount = ErrorReport.AddedRsns;
            //data.DeletedCount = ErrorReport.DeletedRsns;
            //data.UpdatedCount = ErrorReport.UpdatedRsns;
            //data.CommonCount = ErrorReport.CommonRsns;
            //data.Percentage = (double) ((double)ErrorReport.CommonRsns / (double)(ErrorReport.AddedRsns + ErrorReport.DeletedRsns + ErrorReport.UpdatedRsns + ErrorReport.CommonRsns)) * 100;
            //vm.ErrorReportData.Add(data);
            data = new ErrorReportData();
            data.DataType = "CVTs";
            data.AddedCount = ErrorReport.AddedCVTS;
            data.DeletedCount = ErrorReport.DeletedCVTS;
            data.CommonCount = ErrorReport.CommonCVTS;
            data.Percentage = (double)((double)ErrorReport.CommonCVTS / (double)(ErrorReport.AddedCVTS + ErrorReport.DeletedCVTS + ErrorReport.CommonCVTS)) * 100;
            vm.ErrorReportData.Add(data);
            data = new ErrorReportData();
            data.DataType = "FreeText";
            data.AddedCount = ErrorReport.AddedFreeText;
            data.DeletedCount = ErrorReport.DeletedFreeText;
            data.CommonCount = ErrorReport.CommonFreeText;
            data.Percentage = (double)((double)ErrorReport.CommonFreeText / (double)(ErrorReport.AddedFreeText + ErrorReport.DeletedFreeText + ErrorReport.CommonFreeText)) * 100;
            vm.ErrorReportData.Add(data);
            return vm;
        }

    }
}
