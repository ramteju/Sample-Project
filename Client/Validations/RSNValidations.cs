using Client.Common;
using Client.ViewModels;
using Client.Views;
using Excelra.Utils.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Client.Validations
{
    ////Short name for RSNValidations
    public static class RV
    {
        public static string FreeTextWithOutStageInfo(string freetext)
        {
            if (freetext.Contains("("))
            {
                return freetext.Substring(0, freetext.IndexOf('('));
            }
            else
                return freetext;
        }

        public static bool ValidateRsnEditArea(string FreeText, ReactionVM ReactionVM, StageVM StageVM, RsnLevel RsnLevel, out string outMsg)
        {
            bool result = false;
            if (RsnLevel == RsnLevel.STAGE && !FreeText.EndsWith(")"))
            {
                outMsg = $"Freetext must ends with ) in Stage level";
                return result;
            }
            var splittedFreetexts = FreeText.Split(new string[] { RsnLevel == RsnLevel.REACTION ? ", " : "), " }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var splittedText in splittedFreetexts)
            {
                string updatedFreetext = splittedText;
                if (RsnLevel == RsnLevel.STAGE && !updatedFreetext.EndsWith(")"))
                    updatedFreetext = updatedFreetext + ")";
                if (RsnLevel == RsnLevel.STAGE && !updatedFreetext.Contains(" (stage"))
                {
                    outMsg = $"Freetext and Stage information must be seperated by 'Single Space' in {splittedText}";
                    return result;
                }

                List<int> stageNumbers = new List<int>();
                outMsg = "";
                if (GetStageNumbersFromFreeText(updatedFreetext, ref stageNumbers, ref outMsg))
                {
                    if (stageNumbers.Count > 1)
                    {
                        if (!stageNumbers.SequenceEqual(stageNumbers.OrderBy(n => n)))
                        {
                            outMsg = S.STAGE_ASCE_ERR_MSG;
                            return result;
                        }
                        var duplicates = stageNumbers.GroupBy(s => s).SelectMany(grp => grp.Skip(1));
                        if (duplicates.Any())
                        {
                            outMsg = S.STAGE_DUPLICATE_ERR_MSG;
                            return result;
                        }
                        if (stageNumbers[0] > stageNumbers[stageNumbers.Count - 1] || stageNumbers.Max() > ReactionVM.Stages.Max(s => s.DisplayOrder) || stageNumbers.Min() <= 0
                            || stageNumbers.Min() > ReactionVM.Stages.Max(s => s.DisplayOrder) || stageNumbers.Min() != StageVM.DisplayOrder)
                        {
                            outMsg = $"{S.STAGE_RANGE_ERR_MSG}{updatedFreetext}";
                            return result;
                        }
                    }
                }
                else
                    return false;

                #region Commented
                //if (Regex.IsMatch(updatedFreetext, S.STAGEINFO_RANGE))
                //{
                //    var itemlist = regex.Match(updatedFreetext).Value.Replace("(stages ", "").Replace(")", "").Split('-').Select(c => Convert.ToInt32(c)).ToList();
                //    if (itemlist[0] > itemlist[1] || itemlist[0] == itemlist[1] || itemlist[1] > ReactionVM.Stages.Max(s => s.DisplayOrder) || itemlist[0] < 0 ||
                //        itemlist[0] > ReactionVM.Stages.Max(s => s.DisplayOrder) || itemlist[0] != StageVM.DisplayOrder)
                //    {
                //        outMsg = $"{S.STAGE_RANGE_ERR_MSG}{updatedFreetext}";
                //        return result;
                //    }
                //}
                //else if (Regex.IsMatch(updatedFreetext, S.STAGEINFO_AND))
                //{
                //    var itemlist = regex.Match(updatedFreetext).Value.Replace("(stages ", "").Replace(" and ", "-").Replace(")", "").Split('-').Select(c => Convert.ToInt32(c)).ToList();
                //    if (itemlist[0] > itemlist[1] || itemlist[0] == itemlist[1] || itemlist[1] > ReactionVM.Stages.Max(s => s.DisplayOrder) || itemlist[0] < 0 ||
                //        itemlist[0] > ReactionVM.Stages.Max(s => s.DisplayOrder) || itemlist[0] != StageVM.DisplayOrder)
                //    {
                //        outMsg = $"{S.STAGE_RANGE_ERR_MSG}{updatedFreetext}";
                //        return result;
                //    }
                //}

                //else if (Regex.IsMatch(updatedFreetext, S.STAGEINFO_MULTIPLE_COMMA))
                //{
                //    var itemlist = regex.Match(updatedFreetext).Value.Replace("(stages ", "").Replace(",", "-").Replace(")", "").Split('-').Select(c => Convert.ToInt32(c)).ToList();
                //    if (itemlist[0] > itemlist[itemlist.Count - 1] || itemlist[0] == itemlist[1] || itemlist[itemlist.Count - 1] > ReactionVM.Stages.Max(s => s.DisplayOrder) ||
                //        itemlist[0] < 0 || itemlist[0] > ReactionVM.Stages.Max(s => s.DisplayOrder) || itemlist[0] != StageVM.DisplayOrder)
                //    {
                //        outMsg = $"{S.STAGE_RANGE_ERR_MSG}{updatedFreetext}";
                //        return result;
                //    }
                //    if (!itemlist.SequenceEqual(itemlist.OrderBy(n => n)))
                //    {
                //        outMsg = S.STAGE_ASCE_ERR_MSG;
                //        return result;
                //    }
                //    var duplicates = itemlist.GroupBy(s => s).SelectMany(grp => grp.Skip(1));
                //    if (duplicates.Count() > 0)
                //    {
                //        outMsg = S.STAGE_DUPLICATE_ERR_MSG;
                //        return result;
                //    }
                //}
                //else if (Regex.IsMatch(updatedFreetext, S.STAGEINFO_MULTIPLE_COMMA_AND))
                //{
                //    var itemlist = regex.Match(updatedFreetext).Value.Replace("(stages ", "").Replace(",", "-").Replace(" and ", "-").Replace(")", "").Split('-').Select(c => Convert.ToInt32(c)).ToList();
                //    if (itemlist[0] > itemlist[itemlist.Count - 1] || itemlist.Distinct().Count() != itemlist.Count() || itemlist.Max() > ReactionVM.Stages.Max(s => s.DisplayOrder) ||
                //        itemlist[0] < 0 || itemlist[0] > ReactionVM.Stages.Max(s => s.DisplayOrder) || itemlist[0] != StageVM.DisplayOrder)
                //    {
                //        outMsg = $"{S.STAGE_RANGE_ERR_MSG}{updatedFreetext}";
                //        return result;
                //    }
                //    if (!itemlist.SequenceEqual(itemlist.OrderBy(n => n)))
                //    {
                //        outMsg = S.STAGE_ASCE_ERR_MSG;
                //        return result;
                //    }
                //    var duplicates = itemlist.GroupBy(s => s).SelectMany(grp => grp.Skip(1));
                //    if (duplicates.Count() > 0)
                //    {
                //        outMsg = S.STAGE_DUPLICATE_ERR_MSG;
                //        return result;
                //    }
                //}
                //else if (Regex.IsMatch(updatedFreetext, S.STAGEINFO_RANGE_AND_RANGE))
                //{
                //    var itemlist = regex.Match(updatedFreetext).Value.Replace("(stages ", string.Empty).Replace(" and ", "-").Replace(")", string.Empty).Split('-').Select(c => Convert.ToInt32(c)).ToList();
                //    if (itemlist[0] > itemlist[1] || itemlist.Distinct().Count() != itemlist.Count() || itemlist.Max() > ReactionVM.Stages.Max(s => s.DisplayOrder) || itemlist[0] < 0 ||
                //        itemlist[0] > ReactionVM.Stages.Max(s => s.DisplayOrder) || itemlist[0] != StageVM.DisplayOrder)
                //    {
                //        outMsg = $"{S.STAGE_RANGE_ERR_MSG}{updatedFreetext}";
                //        return result;
                //    }
                //    if (!itemlist.SequenceEqual(itemlist.OrderBy(n => n)))
                //    {
                //        outMsg = S.STAGE_ASCE_ERR_MSG;
                //        return result;
                //    }
                //    var duplicates = itemlist.GroupBy(s => s).SelectMany(grp => grp.Skip(1));
                //    if (duplicates.Count() > 0)
                //    {
                //        outMsg = S.STAGE_DUPLICATE_ERR_MSG;
                //        return result;
                //    }
                //}

                //else if (Regex.IsMatch(updatedFreetext, S.STAGEINFO_RANGE_COMMA_AND))
                //{
                //    List<int> StageNumbers = new List<int>();
                //    var itemlist = regex.Match(updatedFreetext).Value.Replace("(stages ", "").Replace(")", "").Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                //    var splittedListWithRange = itemlist[0].Split('-').Select(c => Convert.ToInt32(c)).ToList();
                //    if (splittedListWithRange.Count > 1)
                //        for (int r = splittedListWithRange[0]; r <= splittedListWithRange[1]; r++)
                //            StageNumbers.Add(r);
                //    var splittedList = itemlist[1].Replace(" and ", "-").Split('-').Select(c => Convert.ToInt32(c)).ToList();
                //    StageNumbers.AddRange(splittedList);


                //    if (StageNumbers[0] > StageNumbers[1] || itemlist.Distinct().Count() != itemlist.Count() || StageNumbers.Max() > ReactionVM.Stages.Max(s => s.DisplayOrder) || StageNumbers[0] < 0 || StageNumbers[0] > ReactionVM.Stages.Max(s => s.DisplayOrder) || StageNumbers[0] != StageVM.DisplayOrder)
                //    {
                //        outMsg = $"{S.STAGE_RANGE_ERR_MSG}{updatedFreetext}";
                //        return result;
                //    }
                //    if (!StageNumbers.SequenceEqual(StageNumbers.OrderBy(n => n)))
                //    {
                //        outMsg = S.STAGE_ASCE_ERR_MSG;
                //        return result;
                //    }
                //    var duplicates = StageNumbers.GroupBy(s => s).SelectMany(grp => grp.Skip(1));
                //    if (duplicates.Count() > 0)
                //    {
                //        outMsg = S.STAGE_DUPLICATE_ERR_MSG;
                //        return result;
                //    }
                //}
                //else if (Regex.IsMatch(updatedFreetext, S.STAGEINFO_SINGLE_STAGE))
                //{
                //    string s = regex.Match(updatedFreetext).Value.Replace("(stage ", "").Replace(")", "");
                //    if (!int.TryParse(s, out value) || value != StageVM.DisplayOrder)
                //    {
                //        outMsg = "Stage Level Information must be selected stage information";
                //        return result;
                //    }
                //} 
                #endregion
            }
            result = true;
            outMsg = string.Empty;
            return result;
        }

        public static string GetStageInfoWithOutFreeText(string freetext)
        {
            if (freetext.Contains("(stage"))
            {
                string stageInfo = freetext.Substring(freetext.IndexOf("(stage"));
                if (stageInfo.Contains(")"))
                    return stageInfo.Substring(0, stageInfo.IndexOf(")"));
                return stageInfo;
            }
            else
                return string.Empty;
        }


        public static bool GetStageNumbersFromFreeText(string data, ref List<int> stagenunbers, ref string msg)
        {
            string subdata = data.Substring(data.IndexOf("(stage"));
            if (data.Contains("(stages") && !data.Contains("(stages "))
            {
                msg = $"Stages and stage information must be seperated by Space in {data}";
                return false;
            }
            else if (data.Contains("(stage") && !data.Contains("(stages") && !data.Contains("(stage "))
            {
                msg = $"Stages and stage information must be seperated by Space in {data}";
                return false;
            }

            var splitteddata = subdata.Replace("(stages", "").Replace(")", "").Replace("(stage", "").Split(new string[] { "and", "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
            foreach (var item in splitteddata)
            {
                if (IsNumber(item))
                    stagenunbers.Add(int.Parse(item));
                else if (item.Contains("-"))
                {
                    var itemlist = item.Split('-');
                    if (itemlist.Where(i => !IsNumber(i)).Any())
                    {
                        msg = $"Text contains invalid stage numbers in {data}";
                        return false;
                    }
                    else
                    {
                        var newList = itemlist.Select(c => Convert.ToInt32(c)).ToList();
                        if (newList.Count > 1)
                            for (int r = newList[0]; r <= newList[1]; r++)
                                stagenunbers.Add(r);
                    }
                }
                else
                {
                    msg = $"Invalid freetext {data}";
                    return false;
                }
            }
            if (stagenunbers.Count == 1 && data.Contains("(stages"))
            {
                msg = $"Freetext term: '{data}' contains word '(stages'. It not allowed for single stage information";
                return false;
            }
            else if (stagenunbers.Count > 1 && !data.Contains("(stages"))
            {
                msg = $"Freetext term: '{data}' contains word '(stage'. It must be '(stages' for multi stage information";
                return false;
            }
            return true;
        }

        public static bool IsNumber(string input)
        {
            int num;
            if (int.TryParse(input, out num))
                return true;
            return false;
        }

        public static bool ValidateRsnFreetext(string FreeText, ReactionVM ReactionVM, StageVM StageVM, RsnLevel RsnLevel, out string outMsg)
        {
            string freetextREstring = S.RegularExpressions.Where(re => re.RegulerExpressionFor == ProductTracking.Models.Core.RegulerExpressionFor.FreeText).Select(re => re.Expression).FirstOrDefault();
            Regex FreetextRE = new Regex(freetextREstring);
            var SPLList = new List<string> { "==", "%%", ",,", "((", "))", "{{", "}}", "++", "//", "\\", "::", ";;", "--", "..", "  ", "''", "<<", ">>", "**", "@@", "[[", "]]", ", ,", ").", ".,", " ." };
            if (!string.IsNullOrEmpty(FreeText) && SPLList.Where(spl => FreeText.Contains(spl)).FirstOrDefault() != null)
            {
                outMsg = $"Freetext contains invalid repetation of special Characters <SPL Char Start>{SPLList.Where(spl => FreeText.Contains(spl)).FirstOrDefault()}</SPL Char End>.";
                return false;
            }
            if (!string.IsNullOrEmpty(FreeText) && StageVM != null && RsnLevel == RsnLevel.STAGE)
            {
                string[] list = FreeText.Split(new string[] { "), " }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var freetext in list)
                {
                    string newText = !freetext.EndsWith(")") ? $"{freetext})" : freetext;
                    List<int> data = PressureValidations.GetStageDisplayOrdersFromFreetexts(new List<RsnVM> { new ViewModels.RsnVM { Reaction = ReactionVM, Stage = StageVM, FreeText = newText } }, ReactionVM, FreeTextWithOutStageInfo(newText));
                    if (!data.Contains(StageVM.DisplayOrder) && RsnLevel == RsnLevel.STAGE)
                    {
                        outMsg = $"Freetext must contain current stage Number in '{freetext}'";
                        return false;
                    }
                }
            }
            if (RsnLevel == RsnLevel.REACTION && !string.IsNullOrEmpty(FreeText) && FreeText.Contains(","))
            {
                var list = $" {FreeText}".Split(',');
                if (list.Where(l => !l.StartsWith(" ")).Any())
                {
                    outMsg = "FreeText must seperated by comma and space in Reaction level.";
                    return false;
                }
            }
            if (RsnLevel == RsnLevel.STAGE && !string.IsNullOrEmpty(FreeText) && FreeText.Contains("),"))
            {
                var list = $" {FreeText}".Split(new string[] { ")," }, StringSplitOptions.RemoveEmptyEntries);
                if (list.Where(l => !l.StartsWith(" ")).Any())
                {
                    outMsg = "FreeText must seperated by comma and space in stage level.";
                    return false;
                }
            }
            if (!string.IsNullOrEmpty(FreeText) && FreeText.Contains(", ") && FreeText.ToLower().Split(new string[] { RsnLevel == RsnLevel.REACTION ? ", " : "), " }, StringSplitOptions.RemoveEmptyEntries).GroupBy(s => FreeTextWithOutStageInfo(s)).SelectMany(grp => grp.Skip(1)).Count() > 0)
            {
                outMsg = "FreeText Contains duplicates.";
                return false;
            }

            if (!string.IsNullOrEmpty(FreeText) && FreeText.Contains(", ") && FreeText.ToLower().Split(new string[] { RsnLevel == RsnLevel.REACTION ? ", " : "), " }, StringSplitOptions.RemoveEmptyEntries).Where(s => s.EndsWith(".")).Count() > 0)
            {
                outMsg = "FreeText ending with .(Period)";
                return false;
            }

            if (RsnLevel == RsnLevel.STAGE && StageVM == null)
            {
                outMsg = "Please Select Stage to Add Stage RSN";
                return false;
            }

            if (RsnLevel == RsnLevel.REACTION && !String.IsNullOrEmpty(FreeText) && FreeText.Contains("(stage"))
            {
                outMsg = "Reaction Level stage information Not allowed";
                return false;
            }

            if (!string.IsNullOrEmpty(FreeText) && !FreetextRE.IsMatch(FreeText))
            {
                outMsg = "FreeText Contains special characters.";
                return false;
            }
            outMsg = string.Empty;
            return true;
        }

        public static bool ValidateRsnReactionLevel(ReactionVM ReactionVM, StageVM StageVM, RsnLevel RsnLevel, string CVT, string FreeText, List<RsnVM> Rsns, out string outMsg, RsnVM EditingRsn = null)
        {
            if (RsnLevel == RsnLevel.STAGE)
            {
                if (!string.IsNullOrEmpty(CVT) && string.IsNullOrEmpty(FreeText))
                {
                    outMsg = "Stage Level CVT Used, Then Freetext is mandatory..";
                    return false;
                }
                else if (!string.IsNullOrEmpty(FreeText))
                {
                    if (!ValidateRsnEditArea(FreeText, ReactionVM, StageVM, RsnLevel, out outMsg))
                        return false;
                }
                else
                {
                    outMsg = "Stage Level Information missed in Freetext Term... / Ends with some Special characters";
                    return false;
                }
            }
            var reactionRsns = Rsns.Where(rsn => rsn.Reaction != null && rsn.Reaction.Id == ReactionVM.Id);
            bool OnlyOneFreeTextInReactionLevel = (CVT == String.Empty && RsnLevel == RsnLevel.REACTION && Rsns.Any(r => r.Reaction != null && r.Reaction.Id == ReactionVM.Id && (EditingRsn != null ? r.Id != EditingRsn.Id : true) && r.Stage == null && r.CvtText == String.Empty)) ? false : true;
            if (OnlyOneFreeTextInReactionLevel)
            {
                bool OnlyOneFreeTextInStageLevel = (CVT == String.Empty && RsnLevel == RsnLevel.STAGE && Rsns.Any(r => r.Reaction != null && (EditingRsn != null ? r.Id != EditingRsn.Id : true) && r.Reaction.Id == ReactionVM.Id && r.Stage != null && r.Stage.Id == StageVM.Id && r.CvtText == String.Empty)) ? false : true;
                if (OnlyOneFreeTextInStageLevel)
                {
                    if (!String.IsNullOrEmpty(CVT) && reactionRsns.Where(r => (EditingRsn != null ? r.Id != EditingRsn.Id : true) && r.CvtText.SafeEqualsLower(CVT)).Any())
                    {
                        var SelectedRSNTerm = reactionRsns.Where(r => (EditingRsn != null ? r.Id != EditingRsn.Id : true) && r.CvtText == CVT).FirstOrDefault();
                        outMsg = "Selected CVT " + (!String.IsNullOrEmpty(CVT) ? CVT : FreeText) + " Already used in " + (SelectedRSNTerm?.Stage != null ? SelectedRSNTerm?.Stage.Name : SelectedRSNTerm.Reaction.DisplayName);
                        return false;
                    }
                    if (!string.IsNullOrEmpty(FreeText))
                    {
                        var splittedFreetexts = FreeText.Split(new String[] { RsnLevel == RsnLevel.REACTION ? ", " : "), " }, StringSplitOptions.RemoveEmptyEntries).Select(c => FreeTextWithOutStageInfo(c));
                        foreach (var item in splittedFreetexts)
                        {
                            if (S.CommentDictionary.CVT.Where(cvt => item.Trim().SafeEqualsLower(cvt.CVTS.Trim())).Any())
                            {
                                outMsg = $"Selected FreeText contains CVT term \"{item}\"";
                                return false;
                            }
                            var result = reactionRsns.Where(r => (EditingRsn != null ? r.Id != EditingRsn.Id : true) && ((!String.IsNullOrEmpty(r.FreeText) && r.FreeText.Split(new String[] { r.IsRXN ? ", " : "), " }, StringSplitOptions.RemoveEmptyEntries).Where(eachText => FreeTextWithOutStageInfo(eachText).SafeEqualsLower(item)).Any()) || ((!string.IsNullOrEmpty(r.CvtText)) && item.SafeEqualsLower(r.CvtText.Trim()))));
                            if (!string.IsNullOrEmpty(item) && result != null && result.Any())
                            {
                                var SelectedRSNTerm = result.FirstOrDefault();
                                outMsg = "Selected FreeText " + item + " Already used in " + (SelectedRSNTerm?.Stage != null ? SelectedRSNTerm?.Stage.Name : SelectedRSNTerm.Reaction.DisplayName);
                                return false;
                            }
                        }
                    }
                    outMsg = string.Empty;
                    return true;
                }
                else
                {
                    outMsg = "Only One Stage Level Free Text Is Allowed With out CVT . .";
                    return false;
                }
            }
            else
            {
                outMsg = "Only One Reaction Level Free Text Is Allowed With out CVT . .";
                return false;
            }
        }
    }
}
