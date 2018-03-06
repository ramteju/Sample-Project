using Client.Common;
using Client.Logging;
using Client.RestConnection;
using Client.Views;
using Entities;
using Entities.DTO;
using Newtonsoft.Json;
using ProductTracking.Models.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels.Reports
{
    public static class ErrorPercentageReportVM
    {

        private static async Task GetErrorPercentage(ErrorPercentageDto err)
        {

            try
            {
                //int ReactionsAdd = 0, ReactionsDel = 0, ParticipantsAdd = 0, ParticipantsDel = 0, RsnAdd = 0, RsnDel = 0, CommentsAdd = 0, CommentsDel = 0;
                ErrorPercentageDto returnData = new ErrorPercentageDto();
                List<Tan> firstTans = new List<Tan>();
                List<Tan> secondTans = new List<Tan>();
                RestStatus status = await ReportHub.GetErrorPercentage(err);
                if (status.HttpCode == System.Net.HttpStatusCode.OK)
                {
                    returnData = (ErrorPercentageDto)status.UserObject;
                    foreach (var item in returnData.FirstRoleTanData)
                    {
                        firstTans.Add(JsonConvert.DeserializeObject<Tan>(item));
                    }
                    foreach (var item in returnData.SecondRoleTanData)
                    {
                        secondTans.Add(JsonConvert.DeserializeObject<Tan>(item));
                    }
                    Calculate(firstTans, secondTans);
                }
                else
                {
                    AppErrorBox.ShowErrorMessage("Unable to load user permission", status.StatusMessage);
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        public static List<ErrorReport> Calculate(List<Tan> firstTans, List<Tan> secondTans)
        {
            List<Reaction> firstReact = new List<Reaction>();
            List<Reaction> secondReact = new List<Reaction>();
            List<ErrorReport> calerror = new List<ErrorReport>();
            var Reactdict = firstTans.GroupBy(x => x.Id).ToDictionary(x => x.Key, x => x.FirstOrDefault().Reactions.Select(R => R.Id).ToList());
            firstReact = firstTans.SelectMany(x => x.Reactions).ToList();
            secondReact = secondTans.SelectMany(x => x.Reactions).ToList();
            if (firstReact.Count == 0)
            {
                calerror.Add(new ErrorReport
                {
                    TanId = Reactdict.Keys.FirstOrDefault(),
                    AddedReactions = secondReact.Select(x => x.Id).Except(firstReact.Select(x => x.Id)).Count()
                });
            }
            foreach (var item in Reactdict)
            {
                List<Comments> firstComments = new List<ProductTracking.Models.Core.Comments>();
                List<Comments> seondComments = new List<ProductTracking.Models.Core.Comments>();
                List<Guid> firstReactIds = item.Value;
                List<Guid> secondReactIds = secondReact.Where(x => x.TanId == item.Key).Select(x => x.Id).ToList();
                firstComments = firstTans.Where(x => x.Id == item.Key).FirstOrDefault().TanComments.ToList();
                seondComments = secondTans.Where(x => x.Id == item.Key).FirstOrDefault().TanComments.ToList();
                int commoncomments = firstComments.Select(x => x.TotalComment).Intersect(seondComments.Select(x => x.TotalComment)).Count();
                int commonreactions = firstReactIds.Intersect(secondReactIds).Count();
                int addedreactions = secondReactIds.Except(firstReactIds).Count();
                int deletedreactions = firstReactIds.Except(secondReactIds).Count();

                if (commonreactions > 0)
                {
                    foreach (var reactionID in item.Value)
                    {
                        List<Stage> firstStage = new List<Stage>();
                        List<StageCondition> firststgCond = new List<StageCondition>();
                        List<ReactionRSD> firstparts = new List<ProductTracking.Models.Core.ReactionRSD>();
                        List<ReactionRSN> firstRSN = new List<ProductTracking.Models.Core.ReactionRSN>();
                        List<Stage> secondStage = new List<Stage>();
                        List<StageCondition> secondstgCond = new List<StageCondition>();
                        List<ReactionRSD> secondparts = new List<ProductTracking.Models.Core.ReactionRSD>();
                        List<ReactionRSN> secondRSN = new List<ProductTracking.Models.Core.ReactionRSN>();

                        #region stage, condition, participants, rsns information.

                        if (firstTans.Where(x => x.Id == item.Key).FirstOrDefault().Reactions.Count() > 0
                            && firstTans.Where(x => x.Id == item.Key).FirstOrDefault().Reactions.Where(x => x.Id == reactionID).FirstOrDefault().Stages.Count > 0)
                        {
                            firstStage.AddRange(firstTans.Where(x => x.Id == item.Key).FirstOrDefault().Reactions.Where(x => x.Id == reactionID).FirstOrDefault().Stages);
                        }
                        if (secondTans.Where(x => x.Id == item.Key).FirstOrDefault().Reactions.Where(x => x.Id == reactionID).Count() > 0
                            && secondTans.Where(x => x.Id == item.Key).FirstOrDefault().Reactions.Where(x => x.Id == reactionID).FirstOrDefault().Stages.Count > 0)
                        {
                            secondStage.AddRange(secondTans.Where(x => x.Id == item.Key).FirstOrDefault().Reactions.Where(x => x.Id == reactionID).FirstOrDefault().Stages);
                        }
                        if (firstTans.Where(x => x.Id == item.Key).FirstOrDefault().Reactions.Where(x => x.Id == reactionID).Count() > 0
                            && firstTans.Where(x => x.Id == item.Key).FirstOrDefault().Reactions.Where(x => x.Id == reactionID).FirstOrDefault().Stages.SelectMany(x => x.StageConditions).Count() > 0)
                        {
                            firststgCond.AddRange(firstTans.Where(x => x.Id == item.Key).FirstOrDefault().Reactions.Where(x => x.Id == reactionID).FirstOrDefault().Stages.SelectMany(x => x.StageConditions));
                        }
                        if (secondTans.Where(x => x.Id == item.Key).FirstOrDefault().Reactions.Where(x => x.Id == reactionID).Count() > 0
                            && secondTans.Where(x => x.Id == item.Key).FirstOrDefault().Reactions.Where(x => x.Id == reactionID).FirstOrDefault().Stages.SelectMany(x => x.StageConditions).Count() > 0)
                        {
                            secondstgCond.AddRange(secondTans.Where(x => x.Id == item.Key).FirstOrDefault().Reactions.Where(x => x.Id == reactionID).FirstOrDefault().Stages.SelectMany(x => x.StageConditions));
                        }
                        if (firstTans.Where(x => x.Id == item.Key).FirstOrDefault().Participants.Where(p => p.ReactionId == reactionID).Count() > 0)
                        {
                            firstparts.AddRange(firstTans.Where(x => x.Id == item.Key).FirstOrDefault().Participants.Where(p => p.ReactionId == reactionID));
                        }
                        if (secondTans.Where(x => x.Id == item.Key).FirstOrDefault().Participants.Where(p => p.ReactionId == reactionID).Count() > 0)
                        {
                            secondparts.AddRange(secondTans.Where(x => x.Id == item.Key).FirstOrDefault().Participants.Where(p => p.ReactionId == reactionID));
                        }
                        if (firstTans.Where(x => x.Id == item.Key).FirstOrDefault().RSNs.Where(p => p.ReactionId == reactionID).Count() > 0)
                        {
                            firstRSN.AddRange(firstTans.Where(x => x.Id == item.Key).FirstOrDefault().RSNs.Where(p => p.ReactionId == reactionID));
                        }
                        if (secondTans.Where(x => x.Id == item.Key).FirstOrDefault().RSNs.Where(p => p.ReactionId == reactionID).Count() > 0)
                        {
                            secondRSN.AddRange(secondTans.Where(x => x.Id == item.Key).FirstOrDefault().RSNs.Where(p => p.ReactionId == reactionID));
                        }

                        #endregion

                        #region calculating Participants at reactions level

                        #region calculating participants products at reactions level

                        int addpartprods = secondparts.Where(x => x.StageId == Guid.Empty && x.ParticipantType == DTO.ParticipantType.Product).Select(x => x.ParticipantId).Except(firstparts.Where(x => x.StageId == Guid.Empty && x.ParticipantType == DTO.ParticipantType.Product).Select(x => x.ParticipantId)).Count();
                        int delpartprods = firstparts.Where(x => x.StageId == Guid.Empty && x.ParticipantType == DTO.ParticipantType.Product).Select(x => x.ParticipantId).Except(secondparts.Where(x => x.StageId == Guid.Empty && x.ParticipantType == DTO.ParticipantType.Product).Select(x => x.ParticipantId)).Count();
                        var firstwostgpartprods = firstparts.Where(x => x.StageId == Guid.Empty && x.ParticipantType == DTO.ParticipantType.Product).ToDictionary(x => x.Id, x => x.ParticipantId);
                        var secondwostgpartprods = secondparts.Where(x => x.StageId == Guid.Empty && x.ParticipantType == DTO.ParticipantType.Product).ToDictionary(x => x.Id, x => x.ParticipantId);
                        int modpartprods = firstwostgpartprods.Where(x => secondwostgpartprods.ContainsKey(x.Key) && secondwostgpartprods[x.Key] != x.Value).Count();
                        int commonpartprods = firstparts.Where(x => x.StageId == Guid.Empty && x.ParticipantType == DTO.ParticipantType.Product).Select(x => x.ParticipantId).Intersect(secondparts.Where(x => x.StageId == Guid.Empty && x.ParticipantType == DTO.ParticipantType.Product).Select(x => x.ParticipantId)).Count() - modpartprods;

                        #endregion

                        int addpartreactants = 0, delpartreactants = 0, modpartreactants = 0, commonpartreactants = 0, addpartsolvents = 0, delpartsolvents = 0, modpartsolvents = 0, commonpartsolvents = 0, addpartagents = 0,
                            delpartagents = 0, modpartagents = 0, commonpartagents = 0, addpartcatalysts = 0, delpartcatalysts = 0, modpartcatalysts = 0, commonpartcatalysts = 0, fiststgprodwstg = 0;
                        int addedstages = secondStage.Select(x => x.Id).Except(firstStage.Select(x => x.Id)).Count();
                        var commonstages = firstStage.Select(x => x.Id).Intersect(secondStage.Select(x => x.Id));
                        int deletedstages = firstStage.Select(x => x.Id).Except(secondStage.Select(x => x.Id)).Count();

                        #region calculating participants products at stage level

                        var firststgpartprods = firstparts.Where(x => !string.IsNullOrEmpty(x.StageId.ToString()) && x.ParticipantType == DTO.ParticipantType.Product).GroupBy(p => p.StageId).ToDictionary(x => x.Key, x => x.Select(a => a.Id));
                        var secondstgpartprods = secondparts.Where(x => !string.IsNullOrEmpty(x.StageId.ToString()) && x.ParticipantType == DTO.ParticipantType.Product).GroupBy(x => x.StageId).ToDictionary(y => y.Key, y => y.Select(a => a.Id));

                        foreach (var key in commonstages)
                        {
                            if (secondstgpartprods.Keys.Where(x => x.Value == key).Count() > 0)
                            {
                                if (firststgpartprods.Where(x => x.Key == key).Count() > 0)
                                {
                                    addpartprods = addpartprods + secondstgpartprods[key].Except(firststgpartprods[key]).Count();
                                    delpartprods = delpartprods + firststgpartprods[key].Except(secondstgpartprods[key]).Count();
                                    commonpartprods = commonpartprods + firststgpartprods[key].Intersect(secondstgpartprods[key]).Count();
                                    fiststgprodwstg = fiststgprodwstg + firststgpartprods[key].Count();
                                }
                                else
                                {
                                    addpartprods = addpartprods + secondstgpartprods[key].Count();
                                }
                            }
                        }

                        var firststgchemicalidprods = firstparts.Where(x => !string.IsNullOrEmpty(x.StageId.ToString()) && x.ParticipantType == DTO.ParticipantType.Product).GroupBy(p => p.StageId).ToDictionary(x => x.Key, x => x.Select(a => new chem { ParticipantId = a.Id, ChemicalId = a.ParticipantId }));
                        var secondstgchemicalidprods = secondparts.Where(x => !string.IsNullOrEmpty(x.StageId.ToString()) && x.ParticipantType == DTO.ParticipantType.Product).GroupBy(p => p.StageId).ToDictionary(x => x.Key, x => x.Select(a => new chem { ParticipantId = a.Id, ChemicalId = a.ParticipantId }));
                        foreach (var key in commonstages)
                        {
                            if (firststgchemicalidprods.Where(x => x.Key == key).Count() > 0)
                            {
                                foreach (var values in firststgchemicalidprods[key])
                                {
                                    if (secondstgchemicalidprods.Keys.Where(x => x == key).Count() > 0)
                                    {
                                        if (secondstgchemicalidprods[key].Where(x => x.ParticipantId == values.ParticipantId && x.ChemicalId != values.ChemicalId).Count() > 0)
                                        {
                                            modpartprods = modpartprods + 1;
                                            if (commonpartprods > 0)
                                            {
                                                commonpartprods = commonpartprods - 1;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        #endregion

                        #region calculating participants Reactants at stage level

                        var firststgpartreactants = firstparts.Where(x => !string.IsNullOrEmpty(x.StageId.ToString()) && x.ParticipantType == DTO.ParticipantType.Reactant).GroupBy(p => p.StageId).ToDictionary(x => x.Key, x => x.Select(a => a.Id));
                        var secondstgpartreactants = secondparts.Where(x => !string.IsNullOrEmpty(x.StageId.ToString()) && x.ParticipantType == DTO.ParticipantType.Reactant).GroupBy(x => x.StageId).ToDictionary(y => y.Key, y => y.Select(a => a.Id));

                        foreach (var key in commonstages)
                        {
                            if (secondstgpartreactants.Keys.Where(x => x.Value == key).Count() > 0)
                            {
                                if (firststgpartreactants.Where(x => x.Key == key).Count() > 0)
                                {
                                    addpartreactants = addpartreactants + secondstgpartreactants[key].Except(firststgpartreactants[key]).Count();
                                    delpartreactants = delpartreactants + firststgpartreactants[key].Except(secondstgpartreactants[key]).Count();
                                    commonpartreactants = commonpartreactants + firststgpartreactants[key].Intersect(secondstgpartreactants[key]).Count();
                                }
                                else
                                {
                                    addpartreactants = addpartreactants + secondstgpartreactants[key].Count();
                                }
                            }
                        }

                        var firststgchemicalidreactants = firstparts.Where(x => !string.IsNullOrEmpty(x.StageId.ToString()) && x.ParticipantType == DTO.ParticipantType.Reactant).GroupBy(p => p.StageId).ToDictionary(x => x.Key, x => x.Select(a => new chem { ParticipantId = a.Id, ChemicalId = a.ParticipantId }));
                        var secondstgchemicalidreactants = secondparts.Where(x => !string.IsNullOrEmpty(x.StageId.ToString()) && x.ParticipantType == DTO.ParticipantType.Reactant).GroupBy(p => p.StageId).ToDictionary(x => x.Key, x => x.Select(a => new chem { ParticipantId = a.Id, ChemicalId = a.ParticipantId }));

                        foreach (var key in commonstages)
                        {
                            if (firststgchemicalidreactants.Where(x => x.Key == key).Count() > 0)
                            {
                                foreach (var values in firststgchemicalidreactants[key])
                                {
                                    if (secondstgchemicalidreactants.Keys.Where(x => x == key).Count() > 0)
                                    {
                                        if (secondstgchemicalidreactants[key].Where(x => x.ParticipantId == values.ParticipantId && x.ChemicalId != values.ChemicalId).Count() > 0)
                                        {
                                            modpartreactants = modpartreactants + 1;
                                            if (commonpartreactants > 0)
                                                commonpartreactants = commonpartreactants - 1;
                                        }
                                    }
                                }
                            }
                        }

                        #endregion

                        #region calculating participants solvents at stage level

                        var firststgpartsolvents = firstparts.Where(x => !string.IsNullOrEmpty(x.StageId.ToString()) && x.ParticipantType == DTO.ParticipantType.Solvent).GroupBy(p => p.StageId).ToDictionary(x => x.Key, x => x.Select(a => a.Id));
                        var secondstgpartsolvents = secondparts.Where(x => !string.IsNullOrEmpty(x.StageId.ToString()) && x.ParticipantType == DTO.ParticipantType.Solvent).GroupBy(x => x.StageId).ToDictionary(y => y.Key, y => y.Select(a => a.Id));

                        foreach (var key in commonstages)
                        {
                            if (secondstgpartsolvents.Keys.Where(x => x.Value == key).Count() > 0)
                            {
                                if (firststgpartsolvents.Where(x => x.Key == key).Count() > 0)
                                {
                                    addpartsolvents = addpartsolvents + secondstgpartsolvents[key].Except(firststgpartsolvents[key]).Count();
                                    delpartsolvents = delpartsolvents + firststgpartsolvents[key].Except(secondstgpartsolvents[key]).Count();
                                    commonpartsolvents = commonpartsolvents + firststgpartsolvents[key].Intersect(secondstgpartsolvents[key]).Count();
                                }
                                else
                                {
                                    addpartsolvents = addpartsolvents + secondstgpartsolvents[key].Count();
                                }
                            }
                        }

                        var firststgchemicalidsolvents = firstparts.Where(x => !string.IsNullOrEmpty(x.StageId.ToString()) && x.ParticipantType == DTO.ParticipantType.Solvent).GroupBy(p => p.StageId).ToDictionary(x => x.Key, x => x.Select(a => new chem { ParticipantId = a.Id, ChemicalId = a.ParticipantId }));
                        var secondstgchemicalidsolvents = secondparts.Where(x => !string.IsNullOrEmpty(x.StageId.ToString()) && x.ParticipantType == DTO.ParticipantType.Solvent).GroupBy(p => p.StageId).ToDictionary(x => x.Key, x => x.Select(a => new chem { ParticipantId = a.Id, ChemicalId = a.ParticipantId }));

                        foreach (var key in commonstages)
                        {
                            if (firststgchemicalidsolvents.Where(x => x.Key == key).Count() > 0)
                            {
                                foreach (var values in firststgchemicalidsolvents[key])
                                {
                                    if (secondstgchemicalidsolvents.Keys.Where(x => x == key).Count() > 0)
                                    {
                                        if (secondstgchemicalidsolvents[key].Where(x => x.ParticipantId == values.ParticipantId && x.ChemicalId != values.ChemicalId).Count() > 0)
                                        {
                                            modpartsolvents = modpartsolvents + 1;
                                            if (commonpartsolvents > 0)
                                                commonpartsolvents -= 1;
                                        }
                                    }
                                }
                            }
                        }

                        #endregion

                        #region calculating participants Agents at stage level

                        var firststgpartagents = firstparts.Where(x => !string.IsNullOrEmpty(x.StageId.ToString()) && x.ParticipantType == DTO.ParticipantType.Agent).GroupBy(p => p.StageId).ToDictionary(x => x.Key, x => x.Select(a => a.Id));
                        var secondstgpartagents = secondparts.Where(x => !string.IsNullOrEmpty(x.StageId.ToString()) && x.ParticipantType == DTO.ParticipantType.Agent).GroupBy(x => x.StageId).ToDictionary(y => y.Key, y => y.Select(a => a.Id));

                        foreach (var key in commonstages)
                        {
                            if (secondstgpartagents.Keys.Where(x => x.Value == key).Count() > 0)
                            {
                                if (firststgpartagents.Where(x => x.Key == key).Count() > 0)
                                {
                                    addpartagents = addpartagents + secondstgpartagents[key].Except(firststgpartagents[key]).Count();
                                    delpartagents = delpartagents + firststgpartagents[key].Except(secondstgpartagents[key]).Count();
                                    commonpartagents = commonpartagents + firststgpartagents[key].Intersect(secondstgpartagents[key]).Count();
                                }
                            }
                        }

                        var firststgchemicalidagents = firstparts.Where(x => !string.IsNullOrEmpty(x.StageId.ToString()) && x.ParticipantType == DTO.ParticipantType.Agent).GroupBy(p => p.StageId).ToDictionary(x => x.Key, x => x.Select(a => new chem { ParticipantId = a.Id, ChemicalId = a.ParticipantId }));
                        var secondstgchemicalidagents = secondparts.Where(x => !string.IsNullOrEmpty(x.StageId.ToString()) && x.ParticipantType == DTO.ParticipantType.Agent).GroupBy(p => p.StageId).ToDictionary(x => x.Key, x => x.Select(a => new chem { ParticipantId = a.Id, ChemicalId = a.ParticipantId }));

                        foreach (var key in commonstages)
                        {
                            if (firststgchemicalidagents.Where(x => x.Key == key).Count() > 0)
                            {
                                foreach (var values in firststgchemicalidagents[key])
                                {
                                    if (secondstgchemicalidagents.Keys.Where(x => x == key).Count() > 0)
                                    {
                                        if (secondstgchemicalidagents[key].Where(x => x.ParticipantId == values.ParticipantId && x.ChemicalId != values.ChemicalId).Count() > 0)
                                        {
                                            modpartagents = modpartagents + 1;
                                            if (commonpartagents > 0)
                                                commonpartagents -= 1;
                                        }
                                    }
                                }
                            }
                        }

                        #endregion

                        #region calculating participants Catalysts at stage level

                        var firststgpartcatalysts = firstparts.Where(x => !string.IsNullOrEmpty(x.StageId.ToString()) && x.ParticipantType == DTO.ParticipantType.Catalyst).GroupBy(p => p.StageId).ToDictionary(x => x.Key, x => x.Select(a => a.Id));
                        var secondstgpartcatalysts = secondparts.Where(x => !string.IsNullOrEmpty(x.StageId.ToString()) && x.ParticipantType == DTO.ParticipantType.Catalyst).GroupBy(x => x.StageId).ToDictionary(y => y.Key, y => y.Select(a => a.Id));

                        foreach (var key in commonstages)
                        {
                            if (secondstgpartcatalysts.Keys.Where(x => x.Value == key).Count() > 0)
                            {
                                if (firststgpartcatalysts.Where(x => x.Key == key).Count() > 0)
                                {
                                    addpartcatalysts = addpartcatalysts + secondstgpartcatalysts[key].Except(firststgpartcatalysts[key]).Count();
                                    delpartcatalysts = delpartcatalysts + firststgpartcatalysts[key].Except(secondstgpartcatalysts[key]).Count();
                                    commonpartcatalysts = commonpartcatalysts + firststgpartcatalysts[key].Intersect(secondstgpartcatalysts[key]).Count();
                                }
                            }
                        }

                        var firststgchemicalidcatalysts = firstparts.Where(x => !string.IsNullOrEmpty(x.StageId.ToString()) && x.ParticipantType == DTO.ParticipantType.Catalyst).GroupBy(p => p.StageId).ToDictionary(x => x.Key, x => x.Select(a => new chem { ParticipantId = a.Id, ChemicalId = a.ParticipantId }));
                        var secondstgchemicalidcatalysts = secondparts.Where(x => !string.IsNullOrEmpty(x.StageId.ToString()) && x.ParticipantType == DTO.ParticipantType.Catalyst).GroupBy(p => p.StageId).ToDictionary(x => x.Key, x => x.Select(a => new chem { ParticipantId = a.Id, ChemicalId = a.ParticipantId }));

                        foreach (var key in commonstages)
                        {
                            if (firststgchemicalidsolvents.Where(x => x.Key == key).Count() > 0)
                            {
                                foreach (var values in firststgchemicalidcatalysts[key])
                                {
                                    if (secondstgchemicalidcatalysts.Keys.Where(x => x == key).Count() > 0)
                                    {
                                        if (secondstgchemicalidcatalysts[key].Where(x => x.ParticipantId == values.ParticipantId && x.ChemicalId != values.ChemicalId).Count() > 0)
                                        {
                                            modpartcatalysts = modpartcatalysts + 1;
                                            if (commonpartcatalysts > 0)
                                                commonpartcatalysts -= 1;
                                        }
                                    }
                                }
                            }
                        }

                        #endregion

                        #endregion

                        #region calculating Rsn information created at Reaction Level

                        //int addrsns = secondRSN.Where(x => string.IsNullOrEmpty(x.StageId.ToString())).Select(x => x.Id).Except(firstRSN.Where(x => string.IsNullOrEmpty(x.StageId.ToString())).Select(x => x.Id)).Count();
                        //int delrsns = firstRSN.Where(x => string.IsNullOrEmpty(x.StageId.ToString())).Select(x => x.Id).Except(secondRSN.Where(x => string.IsNullOrEmpty(x.StageId.ToString())).Select(x => x.Id)).Count();
                        //var firstwostgrsn = firstRSN.Where(x => string.IsNullOrEmpty(x.StageId.ToString())).Select(x => new RsnText { RsnId = x.Id, FreeText = x.FreeText, CVT = x.CVT }).ToList();
                        //var secondwostgrsn = secondRSN.Where(x => string.IsNullOrEmpty(x.StageId.ToString())).Select(x => new RsnText { RsnId = x.Id, FreeText = x.FreeText, CVT = x.CVT }).ToList();
                        //int modrsn = firstwostgrsn.Where(x => secondwostgrsn.Any(y => y.RsnId == x.RsnId && (y.FreeText != x.FreeText || y.CVT != x.CVT))).Count();
                        ////int commonrsn = firstRSN.Where(x => string.IsNullOrEmpty(x.StageId.ToString())).Select(x => x.Id).Intersect(secondRSN.Where(x => string.IsNullOrEmpty(x.StageId.ToString())).Select(x => x.Id)).Count() - modrsn;

                        #region calculations Rsn information for CVTs

                        int addcvts = secondRSN.Where(x => x.StageId.ToString() == "00000000-0000-0000-0000-000000000000" && !string.IsNullOrEmpty(x.CVT)).Select(x => x.CVT).Except(firstRSN.Where(x => x.StageId.ToString() == "00000000-0000-0000-0000-000000000000").Select(x => x.CVT)).Count();
                        int delcvts = firstRSN.Where(x => x.StageId.ToString() == "00000000-0000-0000-0000-000000000000" && !string.IsNullOrEmpty(x.CVT)).Select(x => x.CVT).Except(secondRSN.Where(x => x.StageId.ToString() == "00000000-0000-0000-0000-000000000000").Select(x => x.CVT)).Count();
                        int commoncvts = firstRSN.Where(x => string.IsNullOrEmpty(x.StageId.ToString())).Select(x => x.CVT).Intersect(secondRSN.Where(x => string.IsNullOrEmpty(x.StageId.ToString())).Select(x => x.CVT)).Count();

                        #endregion

                        #region Rsn information for FreeTexts

                        int addfreetext = secondRSN.Where(x => x.StageId == Guid.Empty).Select(x => x.FreeText).Except(firstRSN.Where(x => x.StageId == Guid.Empty).Select(x => x.FreeText)).Count();
                        int delfreetext = firstRSN.Where(x => x.StageId == Guid.Empty).Select(x => x.FreeText).Except(secondRSN.Where(x => x.StageId == Guid.Empty).Select(x => x.FreeText)).Count();
                        int commonfreetext = firstRSN.Where(x => string.IsNullOrEmpty(x.StageId.ToString())).Select(x => x.FreeText).Intersect(secondRSN.Where(x => string.IsNullOrEmpty(x.StageId.ToString())).Select(x => x.FreeText)).Count();

                        #endregion

                        #endregion

                        #region calculating Rsn information created at stage level

                        //var firstwstgrsn = firstRSN.Where(x => !string.IsNullOrEmpty(x.StageId.ToString())).GroupBy(p => p.StageId).ToDictionary(x => x.Key, x => x.Select(a => new { a.Id, a.FreeText, a.CVT }));
                        //var secondwstgrsn = secondRSN.Where(x => !string.IsNullOrEmpty(x.StageId.ToString())).GroupBy(x => x.StageId).ToDictionary(y => y.Key, y => y.Select(a => new { a.Id, a.FreeText, a.CVT }));
                        //foreach (var key in commonstages)
                        //{
                        //    if (secondwstgrsn.Keys.Where(x => x.Value == key).Count() > 0)
                        //    {
                        //        if (firstwstgrsn.Where(x => x.Key == key).Count() > 0)
                        //        {
                        //            int delrsnwstg = firstwstgrsn[key].Select(x => x.Id).Except(secondwstgrsn[key].Select(x => x.Id)).Count();
                        //            addrsns = addrsns + secondwstgrsn[key].Select(x => x.Id).Except(firstwstgrsn[key].Select(x => x.Id)).Count();
                        //            delrsns = delrsns + delrsnwstg;
                        //            commonrsn = commonrsn + firstwstgrsn[key].Select(x => x.Id).Intersect(secondwstgrsn[key].Select(x => x.Id)).Count();

                        //            foreach (var value in firstwstgrsn[key])
                        //            {
                        //                modrsn = modrsn + secondwstgrsn[key].Where(x => x.Id == value.Id && (x.FreeText != value.FreeText || x.CVT != value.CVT)).Count();
                        //                if (commonrsn > 0)
                        //                    commonrsn = commonrsn - secondwstgrsn[key].Where(x => x.Id == value.Id && (x.FreeText != value.FreeText || x.CVT != value.CVT)).Count();
                        //            }
                        //        }
                        //        else
                        //        {
                        //            addrsns = addrsns + secondwstgrsn[key].Count();
                        //        }
                        //    }
                        //}

                        #region calculating rsn information for CVTs at stage level

                        var firstwstgcvt = firstRSN.Where(x => !string.IsNullOrEmpty(x.StageId.ToString())).GroupBy(p => p.StageId).ToDictionary(x => x.Key, x => x.Select(a => new { a.CVT }));
                        var secondwstgcvt = secondRSN.Where(x => !string.IsNullOrEmpty(x.StageId.ToString())).GroupBy(x => x.StageId).ToDictionary(y => y.Key, y => y.Select(a => new { a.CVT }));
                        foreach (var key in commonstages)
                        {
                            if (secondwstgcvt.Keys.Where(x => x.Value == key).Count() > 0)
                            {
                                if (firstwstgcvt.Where(x => x.Key == key).Count() > 0)
                                {
                                    int delrsnwstg = firstwstgcvt[key].Select(x => x.CVT).Except(secondwstgcvt[key].Select(x => x.CVT)).Count();
                                    addcvts = addcvts + secondwstgcvt[key].Select(x => x.CVT).Except(firstwstgcvt[key].Select(x => x.CVT)).Count();
                                    delcvts = delcvts + delrsnwstg;
                                    commoncvts = commoncvts + firstwstgcvt[key].Select(x => x.CVT).Intersect(secondwstgcvt[key].Select(x => x.CVT)).Count();
                                }
                                else
                                {
                                    addcvts = addcvts + secondwstgcvt[key].Count();
                                }
                            }
                        }

                        #endregion

                        #region calculating rsn information for FreeTexts at stage level

                        var firstwstgfreetext = firstRSN.Where(x => !string.IsNullOrEmpty(x.StageId.ToString())).GroupBy(p => p.StageId).ToDictionary(x => x.Key, x => x.Select(a => new { a.CVT }));
                        var secondwstgfreetext = secondRSN.Where(x => !string.IsNullOrEmpty(x.StageId.ToString())).GroupBy(x => x.StageId).ToDictionary(y => y.Key, y => y.Select(a => new { a.CVT }));
                        foreach (var key in commonstages)
                        {
                            if (secondwstgfreetext.Keys.Where(x => x.Value == key).Count() > 0)
                            {
                                if (firstwstgfreetext.Where(x => x.Key == key).Count() > 0)
                                {
                                    int delrsnwstg = firstwstgfreetext[key].Select(x => x.CVT).Except(secondwstgfreetext[key].Select(x => x.CVT)).Count();
                                    addcvts = addcvts + secondwstgfreetext[key].Select(x => x.CVT).Except(firstwstgfreetext[key].Select(x => x.CVT)).Count();
                                    delcvts = delcvts + delrsnwstg;
                                    commoncvts = commoncvts + firstwstgfreetext[key].Select(x => x.CVT).Intersect(secondwstgfreetext[key].Select(x => x.CVT)).Count();
                                }
                                else
                                {
                                    addcvts = addcvts + secondwstgfreetext[key].Count();
                                }
                            }
                        }

                        #endregion

                        #endregion

                        #region Calculating Conditions Information

                        #region calculating condition PH information created at stage level

                        int modcondph = 0, addcondph = 0, delcondph = 0, commoncondph = 0;

                        var firstwstgcondph = firststgCond.Where(x => !string.IsNullOrEmpty(x.StageId.ToString()) && (!string.IsNullOrEmpty(x.PH) || (!string.IsNullOrEmpty(x.PH_TYPE) && x.PH_TYPE != PHEnum.None.ToString()))).GroupBy(x => x.StageId).ToDictionary(x => x.Key, x => x.Select(a => new cond
                        {
                            CondID = a.Id,
                            PH = a.PH,
                            PH_TYPE = a.PH_TYPE,
                        }));
                        var secondwstgcondph = secondstgCond.Where(x => !string.IsNullOrEmpty(x.StageId.ToString()) && (!string.IsNullOrEmpty(x.PH) || (!string.IsNullOrEmpty(x.PH_TYPE) && x.PH_TYPE != PHEnum.None.ToString()))).GroupBy(x => x.StageId).ToDictionary(x => x.Key, x => x.Select(a => new cond
                        {
                            CondID = a.Id,
                            PH = a.PH,
                            PH_TYPE = a.PH_TYPE,
                        }));

                        foreach (var key in commonstages)
                        {
                            if (secondwstgcondph.Keys.Where(x => x == key).Count() > 0)
                            {
                                if (firstwstgcondph.Where(x => x.Key == key).Count() > 0)
                                {
                                    addcondph = addcondph + secondwstgcondph[key].Select(x => x.CondID).Except(firstwstgcondph[key].Select(x => x.CondID)).Count();
                                    delcondph = delcondph + firstwstgcondph[key].Select(x => x.CondID).Except(secondwstgcondph[key].Select(x => x.CondID)).Count();
                                    commoncondph = commoncondph + firstwstgcondph[key].Select(x => x.CondID).Intersect(secondwstgcondph[key].Select(x => x.CondID)).Count();
                                    if (firstwstgcondph.Where(x => x.Key == key).Count() > 0)
                                    {
                                        foreach (var value in firstwstgcondph[key])
                                        {
                                            modcondph = modcondph + secondwstgcondph[key].Where(x => x.CondID == value.CondID && (x.PH != value.PH || x.PH_TYPE != value.PH_TYPE)).Count();
                                            if (commoncondph > 0)
                                                commoncondph = commoncondph - secondwstgcondph[key].Where(x => x.CondID == value.CondID && (x.PH != value.PH || x.PH_TYPE != value.PH_TYPE)).Count();
                                        }
                                    }
                                }
                                else
                                {
                                    addcondph = addcondph + secondwstgcondph[key].Count();
                                }
                            }
                        }

                        #endregion

                        #region calculating condition Pressure information created at stage level

                        int modcondpressure = 0, addcondpressure = 0, delcondpressure = 0, commoncondpressure = 0;

                        var firstwstgcondpressure = firststgCond.Where(x => !string.IsNullOrEmpty(x.StageId.ToString()) && (!string.IsNullOrEmpty(x.Pressure) || (!string.IsNullOrEmpty(x.PRESSURE_TYPE) && x.PRESSURE_TYPE != PressureEnum.None.ToString()))).GroupBy(x => x.StageId).ToDictionary(x => x.Key, x => x.Select(a => new cond
                        {
                            CondID = a.Id,
                            Pressure = a.Pressure,
                            PRESSURE_TYPE = a.PRESSURE_TYPE,
                        }));
                        var secondwstgcondpressure = secondstgCond.Where(x => !string.IsNullOrEmpty(x.StageId.ToString()) && (!string.IsNullOrEmpty(x.Pressure) || (!string.IsNullOrEmpty(x.PRESSURE_TYPE) && x.PRESSURE_TYPE != PressureEnum.None.ToString()))).GroupBy(x => x.StageId).ToDictionary(x => x.Key, x => x.Select(a => new cond
                        {
                            CondID = a.Id,
                            Pressure = a.Pressure,
                            PRESSURE_TYPE = a.PRESSURE_TYPE,
                        }));

                        foreach (var key in commonstages)
                        {
                            if (secondwstgcondpressure.Keys.Where(x => x == key).Count() > 0)
                            {
                                if (firstwstgcondpressure.Where(x => x.Key == key).Count() > 0)
                                {
                                    addcondpressure = addcondpressure + secondwstgcondpressure[key].Select(x => x.CondID).Except(firstwstgcondpressure[key].Select(x => x.CondID)).Count();
                                    delcondpressure = delcondpressure + firstwstgcondpressure[key].Select(x => x.CondID).Except(secondwstgcondpressure[key].Select(x => x.CondID)).Count();
                                    commoncondpressure = commoncondpressure + firstwstgcondpressure[key].Select(x => x.CondID).Intersect(secondwstgcondpressure[key].Select(x => x.CondID)).Count();
                                    if (firstwstgcondpressure.Where(x => x.Key == key).Count() > 0)
                                    {
                                        foreach (var value in firstwstgcondpressure[key])
                                        {
                                            modcondpressure = modcondpressure + secondwstgcondpressure[key].Where(x => x.CondID == value.CondID && (x.Pressure != value.Pressure || x.PRESSURE_TYPE != value.PRESSURE_TYPE)).Count();
                                            if (commoncondpressure > 0)
                                                commoncondpressure = commoncondpressure - secondwstgcondpressure[key].Where(x => x.CondID == value.CondID && (x.Pressure != value.Pressure || x.PRESSURE_TYPE != value.PRESSURE_TYPE)).Count();
                                        }
                                    }
                                }
                                else
                                {
                                    addcondpressure = addcondpressure + secondwstgcondpressure[key].Count();
                                }
                            }
                        }

                        #endregion

                        #region calculating condition Temp information created at stage level

                        int modcondtemp = 0, addcondtemp = 0, delcondtemp = 0, commoncondtemp = 0;

                        var firstwstgcondtemp = firststgCond.Where(x => !string.IsNullOrEmpty(x.StageId.ToString()) && (!string.IsNullOrEmpty(x.Temperature) || (!string.IsNullOrEmpty(x.TEMP_TYPE) && x.TEMP_TYPE != TemperatureEnum.None.ToString()))).GroupBy(x => x.StageId).ToDictionary(x => x.Key, x => x.Select(a => new cond
                        {
                            CondID = a.Id,
                            Temperature = a.Temperature,
                            TEMP_TYPE = a.TEMP_TYPE,
                        }));
                        var secondwstgcondtemp = secondstgCond.Where(x => !string.IsNullOrEmpty(x.StageId.ToString()) && (!string.IsNullOrEmpty(x.Temperature) || (!string.IsNullOrEmpty(x.TEMP_TYPE) && x.TEMP_TYPE != TemperatureEnum.None.ToString()))).GroupBy(x => x.StageId).ToDictionary(x => x.Key, x => x.Select(a => new cond
                        {
                            CondID = a.Id,
                            Temperature = a.Temperature,
                            TEMP_TYPE = a.TEMP_TYPE,
                        }));

                        foreach (var key in commonstages)
                        {
                            if (secondwstgcondtemp.Keys.Where(x => x == key).Count() > 0)
                            {
                                if (firstwstgcondtemp.Where(x => x.Key == key).Count() > 0)
                                {
                                    addcondtemp = addcondtemp + secondwstgcondtemp[key].Select(x => x.CondID).Except(firstwstgcondtemp[key].Select(x => x.CondID)).Count();
                                    delcondtemp = delcondtemp + firstwstgcondtemp[key].Select(x => x.CondID).Except(secondwstgcondtemp[key].Select(x => x.CondID)).Count();
                                    commoncondtemp = commoncondtemp + firstwstgcondtemp[key].Select(x => x.CondID).Intersect(secondwstgcondtemp[key].Select(x => x.CondID)).Count();
                                    if (firstwstgcondtemp.Where(x => x.Key == key).Count() > 0)
                                    {
                                        foreach (var value in firstwstgcondtemp[key])
                                        {
                                            modcondtemp = modcondtemp + secondwstgcondtemp[key].Where(x => x.CondID == value.CondID && (x.Temperature != value.Temperature || x.TEMP_TYPE != value.TEMP_TYPE)).Count();
                                            if (commoncondtemp > 0)
                                                commoncondtemp = commoncondtemp - secondwstgcondtemp[key].Where(x => x.CondID == value.CondID && (x.Temperature != value.Temperature || x.TEMP_TYPE != value.TEMP_TYPE)).Count();
                                        }
                                    }
                                }
                                else
                                {
                                    addcondtemp = addcondtemp + secondwstgcondtemp[key].Count();
                                }
                            }
                        }

                        #endregion

                        #region calculating condition Time information created at stage level

                        int modcondtime = 0, addcondtime = 0, delcondtime = 0, commoncondtime = 0;

                        var firstwstgcondtime = firststgCond.Where(x => !string.IsNullOrEmpty(x.StageId.ToString()) && (!string.IsNullOrEmpty(x.Time) || (!string.IsNullOrEmpty(x.TIME_TYPE) && x.TIME_TYPE != TimeEnum.None.ToString()))).GroupBy(x => x.StageId).ToDictionary(x => x.Key, x => x.Select(a => new cond
                        {
                            CondID = a.Id,
                            Time = a.Time,
                            TIME_TYPE = a.TIME_TYPE
                        }));
                        var secondwstgcondtime = secondstgCond.Where(x => !string.IsNullOrEmpty(x.StageId.ToString()) && (!string.IsNullOrEmpty(x.Time) || (!string.IsNullOrEmpty(x.TIME_TYPE) && x.TIME_TYPE != TimeEnum.None.ToString()))).GroupBy(x => x.StageId).ToDictionary(x => x.Key, x => x.Select(a => new cond
                        {
                            CondID = a.Id,
                            PH = a.PH,
                            PH_TYPE = a.PH_TYPE,
                            Time = a.Time,
                            TIME_TYPE = a.TIME_TYPE
                        }));

                        foreach (var key in commonstages)
                        {
                            if (secondwstgcondtime.Keys.Where(x => x == key).Count() > 0)
                            {
                                if (firstwstgcondtime.Where(x => x.Key == key).Count() > 0)
                                {
                                    addcondtime = addcondtime + secondwstgcondtime[key].Select(x => x.CondID).Except(firstwstgcondtime[key].Select(x => x.CondID)).Count();
                                    delcondtime = delcondtime + firstwstgcondtime[key].Select(x => x.CondID).Except(secondwstgcondtime[key].Select(x => x.CondID)).Count();
                                    commoncondtime = commoncondtime + firstwstgcondtime[key].Select(x => x.CondID).Intersect(secondwstgcondtime[key].Select(x => x.CondID)).Count();
                                    if (firstwstgcondtime.Where(x => x.Key == key).Count() > 0)
                                    {
                                        foreach (var value in firstwstgcondtime[key])
                                        {
                                            modcondtime = modcondtime + secondwstgcondtime[key].Where(x => x.CondID == value.CondID && (x.Time != value.Time || x.TIME_TYPE != value.TIME_TYPE)).Count();
                                            if (commoncondtime > 0)
                                                commoncondtime = commoncondtime - secondwstgcondtime[key].Where(x => x.CondID == value.CondID && (x.Time != value.Time || x.TIME_TYPE != value.TIME_TYPE)).Count();
                                        }
                                    }
                                }
                                else
                                {
                                    addcondtime = addcondtime + secondwstgcondtime[key].Count();
                                }
                            }
                        }

                        #endregion

                        #endregion

                        calerror.Add(new ErrorReport
                        {
                            TanId = item.Key,
                            AddedReactions = addedreactions,
                            DeletedReactions = deletedreactions,
                            CommonReactions = commonreactions,
                            DeletedStages = deletedstages,
                            AddedStages = addedstages,
                            CommonStages = firstStage.Select(x => x.Id).Intersect(secondStage.Select(x => x.Id)).Count(),
                            AddedProducts = addpartprods,
                            DeletedProducts = delpartprods,
                            UpdatedProducts = modpartprods,
                            CommonProducts = commonpartprods,
                            AddedReactants = addpartreactants,
                            DeletedReactants = delpartreactants,
                            UpdatedReactants = modpartreactants,
                            CommonReactants = commonpartreactants,
                            AddedSolvents = addpartsolvents,
                            DeletedSolvents = delpartsolvents,
                            UpdatedSolvents = delpartsolvents,
                            CommonSolvents = commonpartsolvents,
                            AddedAgents = addpartagents,
                            DeletedAgents = delpartagents,
                            UpdatedAgents = modpartagents,
                            CommonAgents = commonpartagents,
                            AddedCatalysts = addpartcatalysts,
                            DeletedCatalysts = delpartcatalysts,
                            UpdatedCatalysts = modpartcatalysts,
                            CommonCatalysts = commonpartcatalysts,
                            AddedCVTS = addcvts,
                            DeletedCVTS = delcvts,
                            CommonCVTS = commoncvts,
                            AddedFreeText = addfreetext,
                            DeletedFreeText = delfreetext,
                            CommonFreeText = commonfreetext,
                            AddedTime = addcondtime,
                            DeletedTime = delcondtime,
                            UpdatedTime = modcondtime,
                            CommonTime = commoncondtime,
                            AddedTemperature = addcondtemp,
                            DeletedTemperature = delcondtemp,
                            UpdatedTemperature = modcondtemp,
                            CommonTemperature = commoncondtemp,
                            AddedPressure = addcondpressure,
                            DeletedPressure = delcondpressure,
                            UpdatedPressure = modcondpressure,
                            CommonPressure = commoncondpressure,
                            AddedpH = addcondph,
                            DeletedpH = delcondph,
                            UpdatedpH = modcondph,
                            CommonpH = commoncondph,
                            AddedComments = seondComments.Select(x => x.TotalComment).Except(firstComments.Select(x => x.TotalComment)).Count(),
                            DeletedComments = firstComments.Select(x => x.TotalComment).Except(seondComments.Select(x => x.TotalComment)).Count(),
                            UpdatedComments = 0,
                            CommonComments = commoncomments
                        });


                    }
                }
                else
                {
                    calerror.Add(new ErrorReport
                    {
                        TanId = item.Key,
                        AddedReactions = addedreactions,
                        DeletedReactions = deletedreactions,
                        CommonReactions = commonreactions,
                        AddedComments = seondComments.Select(x => x.TotalComment).Except(firstComments.Select(x => x.TotalComment)).Count(),
                        DeletedComments = firstComments.Select(x => x.TotalComment).Except(seondComments.Select(x => x.TotalComment)).Count(),
                        UpdatedComments = 0,
                        CommonComments = commoncomments
                    });
                }
            }

            var resultData = calerror.GroupBy(x => x.TanId).Select(x => new ErrorReport
            {
                TanId = x.First().TanId,
                AddedReactions = x.First().AddedReactions,
                DeletedReactions = x.First().DeletedReactions,
                CommonReactions = x.First().CommonReactions,
                AddedStages = x.Sum(a => a.AddedStages),
                DeletedStages = x.Sum(a => a.DeletedStages),
                CommonStages = x.Sum(a => a.CommonStages),
                AddedProducts = x.Sum(a => a.AddedProducts),
                DeletedProducts = x.Sum(a => a.DeletedProducts),
                UpdatedProducts = x.Sum(a => a.UpdatedProducts),
                CommonProducts = x.Sum(a => a.CommonProducts),
                AddedReactants = x.Sum(a => a.AddedReactants),
                DeletedReactants = x.Sum(a => a.DeletedReactants),
                UpdatedReactants = x.Sum(a => a.UpdatedReactants),
                CommonReactants = x.Sum(a => a.CommonReactants),
                AddedSolvents = x.Sum(a => a.AddedSolvents),
                DeletedSolvents = x.Sum(a => a.DeletedSolvents),
                UpdatedSolvents = x.Sum(a => a.UpdatedSolvents),
                CommonSolvents = x.Sum(a => a.CommonSolvents),
                AddedAgents = x.Sum(a => a.AddedAgents),
                DeletedAgents = x.Sum(a => a.DeletedAgents),
                UpdatedAgents = x.Sum(a => a.UpdatedAgents),
                CommonAgents = x.Sum(a => a.CommonAgents),
                AddedCatalysts = x.Sum(a => a.AddedCatalysts),
                DeletedCatalysts = x.Sum(a => a.DeletedCatalysts),
                UpdatedCatalysts = x.Sum(a => a.UpdatedCatalysts),
                CommonCatalysts = x.Sum(a => a.CommonCatalysts),
                AddedCVTS = x.Sum(a => a.AddedCVTS),
                DeletedCVTS = x.Sum(a => a.DeletedCVTS),
                CommonCVTS = x.Sum(a => a.CommonCVTS),
                AddedFreeText = x.Sum(a => a.AddedFreeText),
                DeletedFreeText = x.Sum(a => a.DeletedFreeText),
                CommonFreeText = x.Sum(a => a.CommonFreeText),
                AddedTime = x.Sum(a => a.AddedTime),
                DeletedTime = x.Sum(a => a.DeletedTime),
                UpdatedTime = x.Sum(a => a.UpdatedTime),
                CommonTime = x.Sum(a => a.CommonTime),
                AddedTemperature = x.Sum(a => a.AddedTemperature),
                DeletedTemperature = x.Sum(a => a.DeletedTemperature),
                UpdatedTemperature = x.Sum(a => a.UpdatedTemperature),
                CommonTemperature = x.Sum(a => a.CommonTemperature),
                AddedPressure = x.Sum(a => a.AddedPressure),
                DeletedPressure = x.Sum(a => a.DeletedPressure),
                UpdatedPressure = x.Sum(a => a.UpdatedPressure),
                CommonPressure = x.Sum(a => a.CommonPressure),
                AddedpH = x.Sum(a => a.AddedpH),
                DeletedpH = x.Sum(a => a.DeletedpH),
                UpdatedpH = x.Sum(a => a.UpdatedpH),
                CommonpH = x.Sum(a => a.CommonpH),
                AddedComments = x.Sum(a => a.AddedComments),
                DeletedComments = x.Sum(a => a.DeletedComments),
                CommonComments = x.Sum(a => a.CommonComments),
                UpdatedComments = x.Sum(a => a.UpdatedComments)
            }).ToList();
            return resultData;
        }
        
        struct RsnText
        {
            public Guid RsnId { get; set; }
            public string FreeText { get; set; }
            public string CVT { get; set; }
        }

        struct cond
        {
            public Guid CondID { get; set; }
            public string Temperature { get; set; }
            public string Pressure { get; set; }
            public string PH { get; set; }
            public string Time { get; set; }
            public string TEMP_TYPE { get; set; }
            public string TIME_TYPE { get; set; }
            public string PH_TYPE { get; set; }
            public string PRESSURE_TYPE { get; set; }
        }

        struct chem
        {
            public Guid ParticipantId { get; set; }
            public Guid ChemicalId { get; set; }
        }
    }
}
