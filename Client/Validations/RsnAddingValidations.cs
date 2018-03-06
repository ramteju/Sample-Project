using Client.Common;
using Client.Logging;
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
    public static class RsnAddingValidations
    {
        public static bool ValidateAndAddRsn(string FreeText, string CVT, List<RsnVM> Rsns, List<CvtVM> CVTData, ReactionVM ReactionVM, StageVM StageVM, RsnLevel RsnLevel, Regex regex, RSNWindowVM RSNWindowVM, RsnVM EditingRsn = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(FreeText.Trim()) || !string.IsNullOrEmpty(CVT))
                {
                    string freetextREstring = S.RegularExpressions.Where(re => re.RegulerExpressionFor == ProductTracking.Models.Core.RegulerExpressionFor.FreeText).Select(re => re.Expression).FirstOrDefault();
                    Regex FreetextRE = new Regex(freetextREstring);
                    var SPLList = new List<string> { "==", "%%", ",,", "((", "))", "{{", "}}", "++", "//", "\\", "::", ";;", "--", "..", "  ", "''", "<<", ">>", "**", "@@", "[[", "]]", ", ,", ").", ".,", " ." };
                    if (!string.IsNullOrEmpty(FreeText) && SPLList.Where(spl => FreeText.Contains(spl)).FirstOrDefault() != null)
                    {
                        AppInfoBox.ShowInfoMessage($"Freetext contains invalid repetation of special Characters <SPL Char Start>{SPLList.Where(spl => FreeText.Contains(spl)).FirstOrDefault()}</SPL Char End>.");
                        //MessageBox.Show($"Freetext contains invalid repetation of special Characters {SPLList.Where(spl => FreeText.Contains(spl)).FirstOrDefault()}.", "Reactions", MessageBoxButton.OK, MessageBoxImage.Information);
                        return false;
                    }
                    if (!string.IsNullOrEmpty(FreeText) && FreeText.Contains("), ") && StageVM != null && RsnLevel == RsnLevel.STAGE)
                    {
                        string[] list = FreeText.Split(new string[] { "), " }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var freetext in list)
                        {
                            string newText = !freetext.EndsWith(")") ? $"{freetext})" : freetext;
                            if (regex.IsMatch(newText))
                            {
                                List<int> data = PressureValidations.GetStageDisplayOrdersFromFreetexts(new List<RsnVM> { new ViewModels.RsnVM { Reaction = ReactionVM, Stage = StageVM, FreeText = newText } }, ReactionVM, FreeTextWithOutStageInfo(newText));
                                if (!data.Contains(StageVM.DisplayOrder) && RsnLevel == RsnLevel.STAGE)
                                {
                                    AppInfoBox.ShowInfoMessage("Comma and space allowed only after stage information");
                                    return false;
                                }
                            }
                            else
                            {
                                AppInfoBox.ShowInfoMessage($"Invalid freetext '{newText}'");
                                return false;
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(FreeText) && FreeText.Contains(","))
                    {
                        string[] list = FreeText.Split(',');
                        for (int i = 1; i < list.Length; i++)
                        {
                            if (!Regex.IsMatch(list[i], @"^\s|\d"))
                            {
                                AppInfoBox.ShowInfoMessage("invalid freetext. It contains extra characters");
                                return false;
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(FreeText) && FreeText.Contains(", ") && FreeText.ToLower().Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).GroupBy(s => FreeTextWithOutStageInfo(s)).SelectMany(grp => grp.Skip(1)).Count() > 0)
                    {
                        AppInfoBox.ShowInfoMessage("FreeText Contains duplicates.");
                        return false;
                    }

                    if (!string.IsNullOrEmpty(FreeText) && FreeText.Contains(", ") && FreeText.ToLower().Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).Where(s => s.EndsWith(".")).Count() > 0)
                    {
                        AppInfoBox.ShowInfoMessage("FreeText Contains .(Period)");
                        return false;
                    }

                    if (RsnLevel == RsnLevel.STAGE && StageVM == null)
                    {
                        AppInfoBox.ShowInfoMessage("Please Select Stage to Add Stage RSN");
                        return false;
                    }

                    if (RsnLevel == RsnLevel.REACTION && !String.IsNullOrEmpty(FreeText) && FreeText.Contains("(stage"))
                    {
                        AppInfoBox.ShowInfoMessage("Reaction Level stage information Not allowed");
                        return false;
                    }

                    if (!string.IsNullOrEmpty(FreeText) && !FreetextRE.IsMatch(FreeText))
                    {
                        AppInfoBox.ShowInfoMessage("FreeText Contains special characters.");
                        return false;
                    }
                    if (EditingRsn != null)
                    {
                        bool OnlyOneFreeTextInReactionLevel = (CVT == String.Empty && RsnLevel == RsnLevel.REACTION && Rsns.Any(r => r.Reaction != null && r.Reaction.Id == ReactionVM.Id && (EditingRsn != null ? r.Id != EditingRsn.Id : true) && r.Stage == null && r.CvtText == String.Empty)) ? false : true;
                        if (OnlyOneFreeTextInReactionLevel)
                        {
                            bool OnlyOneFreeTextInStageLevel = (CVT == String.Empty && RsnLevel == RsnLevel.STAGE && Rsns.Any(r => r.Reaction != null && (EditingRsn != null ? r.Id != EditingRsn.Id : true) && r.Reaction.Id == ReactionVM.Id && r.Stage != null && r.Stage.Id == StageVM.Id && r.CvtText == String.Empty)) ? false : true;
                            if (OnlyOneFreeTextInStageLevel)
                            {
                                if (!String.IsNullOrEmpty(CVT) && Rsns.Where(r => r.Reaction.Id == ReactionVM.Id && (EditingRsn != null ? r.Id != EditingRsn.Id : true) && r.CvtText != string.Empty && r.CvtText == CVT).Count() > 0)
                                {
                                    var SelectedRSNTerm = Rsns.Where(r => r.Reaction.Id == ReactionVM.Id && (EditingRsn != null ? r.Id != EditingRsn.Id : true) && r.CvtText == CVT).FirstOrDefault();
                                    AppInfoBox.ShowInfoMessage("Selected CVT " + (!String.IsNullOrEmpty(CVT) ? CVT : FreeText) + " Already used in " + (SelectedRSNTerm?.Stage != null ? SelectedRSNTerm?.Stage.Name : SelectedRSNTerm.Reaction.Name));
                                    return false;
                                }
                                if (!string.IsNullOrEmpty(FreeText))
                                {
                                    var splittedFreetexts = FreeText.Split(new String[] { ", " }, StringSplitOptions.RemoveEmptyEntries).Select(c => FreeTextWithOutStageInfo(c));
                                    foreach (var item in splittedFreetexts)
                                    {
                                        if (CVTData.Where(cvt => item.Trim().SafeEqualsLower(cvt.Text.Trim()) && !cvt.Text.Trim().SafeEqualsLower(CVT.Trim())).Count() > 0)
                                        {
                                            AppInfoBox.ShowInfoMessage($"Selected FreeText contains CVT \"{item}\"");
                                            return false;
                                        }
                                        if (!string.IsNullOrEmpty(item) && Rsns.Where(r => r.Reaction.Id == ReactionVM.Id && (EditingRsn != null ? r.Id != EditingRsn.Id : true) && ((!String.IsNullOrEmpty(r.FreeText) && r.FreeText.Split(new String[] { ", " }, StringSplitOptions.RemoveEmptyEntries).Where(c => FreeTextWithOutStageInfo(c).Trim().SafeEqualsLower(item.Trim())).Count() > 0) || (!string.IsNullOrEmpty(r.CvtText) && item.Trim().SafeContainsLower(r.CvtText.Trim())))).Count() > 0)
                                        {
                                            var SelectedRSNTerm = Rsns.Where(r => r.Reaction.Id == ReactionVM.Id && (EditingRsn != null ? r.Id != EditingRsn.Id : true) && ((!String.IsNullOrEmpty(r.FreeText) && r.FreeText.Split(new String[] { ", " }, StringSplitOptions.RemoveEmptyEntries).Where(c => FreeTextWithOutStageInfo(c).Trim().SafeEqualsLower(item.Trim())).Count() > 0) || (!string.IsNullOrEmpty(r.CvtText) && item.Trim().SafeContainsLower(r.CvtText.Trim())))).FirstOrDefault();
                                            AppInfoBox.ShowInfoMessage("Selected FreeText " + FreeText + " Already used in " + (SelectedRSNTerm?.Stage != null ? SelectedRSNTerm?.Stage.Name : SelectedRSNTerm.Reaction.Name));
                                            return false;
                                        }
                                    }
                                }

                                if (RsnLevel == RsnLevel.STAGE)
                                {
                                    if (!string.IsNullOrEmpty(CVT) && string.IsNullOrEmpty(FreeText))
                                    {
                                        AppInfoBox.ShowInfoMessage("Stage Level CVT Used, Then Freetext is mandatory..");
                                        return false;
                                    }
                                    else if (!string.IsNullOrEmpty(FreeText) && regex.IsMatch(FreeText))
                                    {
                                        string outMsg = string.Empty;
                                        if (!ValidateRsnEditArea(FreeText, regex, ReactionVM, StageVM, out outMsg))
                                        {
                                            AppInfoBox.ShowInfoMessage(outMsg);
                                            return false;
                                        }
                                        return true;
                                    }
                                    else
                                    {
                                        AppInfoBox.ShowInfoMessage("Stage Level Information missed in Freetext Term... / Ends with some Special characters");
                                        return false;
                                    }
                                }
                                return true;
                            }
                            else
                            {
                                AppInfoBox.ShowInfoMessage("Only One Stage Level Free Text Is Allowed With out CVT . .");
                                return false;
                            }
                        }
                        else
                        {
                            AppInfoBox.ShowInfoMessage("Only One Reaction Level Free Text Is Allowed With out CVT . .");
                            return false;
                        }
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(CVT) || !String.IsNullOrEmpty(FreeText))
                        {
                            bool OnlyOneFreeTextInReactionLevel = (CVT == String.Empty && RsnLevel == RsnLevel.REACTION && Rsns.Any(r => r.Reaction != null && r.Reaction.Id == ReactionVM.Id && r.Stage == null && string.IsNullOrEmpty(r.CvtText))) ? false : true;
                            if (OnlyOneFreeTextInReactionLevel)
                            {
                                bool OnlyOneFreeTextInStageLevel = (CVT == String.Empty && RsnLevel == RsnLevel.STAGE && Rsns.Any(r => r.Reaction != null && r.Reaction.Id == ReactionVM.Id && r.Stage != null && r.Stage.Id == StageVM.Id && string.IsNullOrEmpty(r.CvtText))) ? false : true;
                                if (OnlyOneFreeTextInStageLevel)
                                {
                                    if (!String.IsNullOrEmpty(CVT) && Rsns.Where(r => r.Reaction.Id == ReactionVM.Id && r.CvtText.SafeEqualsLower(CVT)).Count() > 0)
                                    {
                                        var SelectedRSNTerm = Rsns.Where(r => r.Reaction.Id == ReactionVM.Id && r.CvtText.SafeEqualsLower(CVT)).FirstOrDefault();
                                        AppInfoBox.ShowInfoMessage("Selected CVT " + (!String.IsNullOrEmpty(CVT) ? CVT : FreeText) + " Already used in " + (SelectedRSNTerm?.Stage != null ? SelectedRSNTerm?.Stage.Name : SelectedRSNTerm.Reaction.Name));
                                        return false;
                                    }

                                    if (!string.IsNullOrEmpty(FreeText))
                                    {
                                        var splittedFreetexts = FreeText.Split(new String[] { ", " }, StringSplitOptions.RemoveEmptyEntries).Select(c => FreeTextWithOutStageInfo(c));
                                        foreach (var item in splittedFreetexts)
                                        {
                                            if (CVTData.Where(cvt => item.Trim().SafeEqualsLower(cvt.Text.Trim()) && !cvt.Text.Trim().SafeEqualsLower(CVT.Trim())).Count() > 0)
                                            {
                                                AppInfoBox.ShowInfoMessage($"Selected FreeText contains CVT \"{item}\"");
                                                return false;
                                            }
                                            if (!string.IsNullOrEmpty(item) && Rsns.Where(r => r.Reaction.Id == ReactionVM.Id && ((!String.IsNullOrEmpty(r.FreeText) && r.FreeText.Split(new String[] { ", " }, StringSplitOptions.RemoveEmptyEntries).Where(c => FreeTextWithOutStageInfo(c).Trim().SafeEqualsLower(item.Trim())).Count() > 0) || (!string.IsNullOrEmpty(r.CvtText) && item.Trim().SafeContainsLower(r.CvtText.Trim())))).Count() > 0)
                                            {
                                                var SelectedRSNTerm = Rsns.Where(r => r.Reaction.Id == ReactionVM.Id && ((!String.IsNullOrEmpty(r.FreeText) && r.FreeText.Split(new String[] { ", " }, StringSplitOptions.RemoveEmptyEntries).Where(c => FreeTextWithOutStageInfo(c).Trim().SafeEqualsLower(item.Trim())).Count() > 0) || (!string.IsNullOrEmpty(r.CvtText) && item.Trim().SafeContainsLower(r.CvtText.Trim())))).FirstOrDefault();
                                                AppInfoBox.ShowInfoMessage($"Selected FreeText '{FreeText}' Already used in {(SelectedRSNTerm?.Stage != null ? SelectedRSNTerm?.Stage.Name : SelectedRSNTerm.Reaction.Name)}");
                                                return false;
                                            }
                                        }
                                    }
                                    if (RsnLevel == RsnLevel.STAGE)
                                    {
                                        if (!string.IsNullOrEmpty(CVT) && string.IsNullOrEmpty(FreeText))
                                        {
                                            AppInfoBox.ShowInfoMessage("Stage Level CVT Used, Then Freetext is mandatory..");
                                            return false;
                                        }
                                        else if (!string.IsNullOrEmpty(FreeText) && regex.IsMatch(FreeText))
                                        {
                                            string OutMsg = string.Empty;
                                            if (!ValidateRsnEditArea(FreeText, regex, ReactionVM, StageVM, out OutMsg))
                                            {
                                                AppErrorBox.ShowErrorMessage(OutMsg, String.Empty);
                                                return false;
                                            }
                                            return true;
                                        }
                                        else
                                        {
                                            AppInfoBox.ShowInfoMessage("Stage Level Information missed in Freetext Term...");
                                            return false;
                                        }
                                    }
                                    return true;
                                }
                                else
                                {
                                    AppInfoBox.ShowInfoMessage("Only One Stage Level Free Text Is Allowed With out CVT . .");
                                    return false;
                                }
                            }
                            else
                            { 
                                AppInfoBox.ShowInfoMessage("Only One Reaction Level Free Text Is Allowed With out CVT . .");
                                return false;
                            }
                        }
                        return false;
                    }
                    if (Rsns.Where(rsn => rsn.CvtText.SafeEqualsLower(S.ENZYMIC_CVT)).Count() > 0 && Rsns.Where(rsn => rsn.CvtText.SafeEqualsLower(S.BIOTRANSFORMATION_CVT)).Count() == 0)
                    {
                        var enzymicRSN = Rsns.Where(rsn => rsn.CvtText.SafeEqualsLower(S.ENZYMIC_CVT)).FirstOrDefault();
                        string freeTextToAdd = !string.IsNullOrEmpty(enzymicRSN.FreeText) ? StageInfoWithOutFreeText(enzymicRSN.FreeText, regex) : string.Empty;
                        Rsns.Add(new RsnVM
                        {
                            CvtText = S.BIOTRANSFORMATION_CVT,
                            FreeText = $"{S.BIOTRANSFORMATION_FREETEXT} {freeTextToAdd}",
                            IsRXN = true,
                            Stage = enzymicRSN.Stage != null ? enzymicRSN.Stage : null,
                            Reaction = ReactionVM,
                            RSNWindowVM = RSNWindowVM,
                            Id = Guid.NewGuid()
                        });
                    }
                    //ClearEditForm.Execute(this);
                }
                else
                    AppInfoBox.ShowInfoMessage("Either CVT or FreeText mandatory to save RSN");
                return true;

            }
            catch (Exception ex)
            {
                Log.This(ex);
                AppErrorBox.ShowErrorMessage(ex.Message, ex.ToString());
                return false;
            }
        }

        public static string FreeTextWithOutStageInfo(string freetext)
        {
            if (freetext.Contains("("))
            {
                return freetext.Substring(0, freetext.IndexOf('('));
            }
            else
                return freetext;
        }
        public static string StageInfoWithOutFreeText(string freetext, Regex regex)
        {
            if (regex.IsMatch(freetext))
                return regex.Match(freetext).Value;
            else
                return string.Empty;
        }

        public static bool ValidateRsnEditArea(string FreeText, Regex regex, ReactionVM ReactionVM, StageVM StageVM, out string outMsg)
        {
            bool result = false;
            int value;
            var splittedFreetexts = FreeText.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var splittedText in splittedFreetexts)
            {
                if (Regex.IsMatch(splittedText, S.STAGEINFO_RANGE))
                {
                    var itemlist = regex.Match(splittedText).Value.Replace("(stages ", "").Replace(")", "").Split('-').Select(c => Convert.ToInt32(c)).ToList();
                    if (itemlist[0] > itemlist[1] || itemlist[0] == itemlist[1] || itemlist[1] > ReactionVM.Stages.Max(s => s.DisplayOrder) || itemlist[0] < 0 || itemlist[0] > ReactionVM.Stages.Max(s => s.DisplayOrder) || itemlist[0] != StageVM.DisplayOrder)
                    {
                        outMsg = "Invalid Stage Range. Please Check Stage range in splittedText";
                        return result;
                    }
                }
                else if (Regex.IsMatch(splittedText, S.STAGEINFO_AND))
                {
                    var itemlist = regex.Match(splittedText).Value.Replace("(stages ", "").Replace(" and ", "-").Replace(")", "").Split('-').Select(c => Convert.ToInt32(c)).ToList();
                    if (itemlist[0] > itemlist[1] || itemlist[0] == itemlist[1] || itemlist[1] > ReactionVM.Stages.Max(s => s.DisplayOrder) || itemlist[0] < 0 || itemlist[0] > ReactionVM.Stages.Max(s => s.DisplayOrder) || itemlist[0] != StageVM.DisplayOrder)
                    {
                        outMsg = "Invalid Stage Range. Please Check Stage range in splittedText";
                        return result;
                    }
                }

                else if (Regex.IsMatch(splittedText, S.STAGEINFO_MULTIPLE_COMMA))
                {
                    var itemlist = regex.Match(splittedText).Value.Replace("(stages ", "").Replace(",", "-").Replace(")", "").Split('-').Select(c => Convert.ToInt32(c)).ToList();
                    if (itemlist[0] > itemlist[itemlist.Count - 1] || itemlist[0] == itemlist[1] || itemlist[itemlist.Count - 1] > ReactionVM.Stages.Max(s => s.DisplayOrder) || itemlist[0] < 0 || itemlist[0] > ReactionVM.Stages.Max(s => s.DisplayOrder) || itemlist[0] != StageVM.DisplayOrder)
                    {
                        outMsg = "Invalid Stage Range. Please Check Stage range in splittedText";
                        return result;
                    }
                    if (!itemlist.SequenceEqual(itemlist.OrderBy(n => n)))
                    {
                        outMsg = "Stage Information values are must in ascending Order";
                        return result;
                    }
                    var duplicates = itemlist.GroupBy(s => s).SelectMany(grp => grp.Skip(1));
                    if (duplicates.Count() > 0)
                    {
                        outMsg = "Stage Information values contains Duplicates";
                        return result;
                    }
                }
                else if (Regex.IsMatch(splittedText, S.STAGEINFO_MULTIPLE_COMMA_AND))
                {
                    var itemlist = regex.Match(splittedText).Value.Replace("(stages ", "").Replace(",", "-").Replace(" and ", "-").Replace(")", "").Split('-').Select(c => Convert.ToInt32(c)).ToList();
                    if (itemlist[0] > itemlist[itemlist.Count - 1] || itemlist.Distinct().Count() != itemlist.Count() || itemlist.Max() > ReactionVM.Stages.Max(s => s.DisplayOrder) || itemlist[0] < 0 || itemlist[0] > ReactionVM.Stages.Max(s => s.DisplayOrder) || itemlist[0] != StageVM.DisplayOrder)
                    {
                        outMsg = "Invalid Stage Range. Please Check Stage range in splittedText";
                        return result;
                    }
                    if (!itemlist.SequenceEqual(itemlist.OrderBy(n => n)))
                    {
                        outMsg = "Stage Information values are must in ascending Order";
                        return result;
                    }
                    var duplicates = itemlist.GroupBy(s => s).SelectMany(grp => grp.Skip(1));
                    if (duplicates.Count() > 0)
                    {
                        outMsg = "Stage Information values contains Duplicates";
                        return result;
                    }
                }
                //else if (Regex.IsMatch(splittedText, S.STAGEINFO_RANGE_AND_RANGE))
                //{
                //    var itemlist = regex.Match(splittedText).Value.Replace("(stages ", string.Empty).Replace(" and ", "-").Replace(")", string.Empty).Split('-').Select(c => Convert.ToInt32(c)).ToList();
                //    if (itemlist[0] > itemlist[1] || itemlist.Distinct().Count() != itemlist.Count() || itemlist.Max() > ReactionVM.Stages.Max(s => s.DisplayOrder) || itemlist[0] < 0 || itemlist[0] > ReactionVM.Stages.Max(s => s.DisplayOrder) || itemlist[0] != StageVM.DisplayOrder)
                //    {
                //        outMsg = "Invalid Stage Range. Please Check Stage range in splittedText";
                //        return result;
                //    }
                //    if (!itemlist.SequenceEqual(itemlist.OrderBy(n => n)))
                //    {
                //        outMsg = "Stage Information values are must in ascending Order";
                //        return result;
                //    }
                //    var duplicates = itemlist.GroupBy(s => s).SelectMany(grp => grp.Skip(1));
                //    if (duplicates.Count() > 0)
                //    {
                //        outMsg = "Stage Information values contains Duplicates";
                //        return result;
                //    }
                //}

                else if (Regex.IsMatch(splittedText, S.STAGEINFO_SINGLE_STAGE))
                {
                    string s = regex.Match(splittedText).Value.Replace("(stage ", "").Replace(")", "");
                    if (int.TryParse(s, out value) && value == StageVM.DisplayOrder)
                    {
                    }
                    else
                    {
                        outMsg = "Stage Level Information must be selected stage information";
                        return result;
                    }
                }
            }
            result = true;
            outMsg = string.Empty;
            return result;
        }

    }
}
