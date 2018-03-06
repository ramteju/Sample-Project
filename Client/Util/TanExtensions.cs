using Client.ViewModels;
using DTO;
using Excelra.Utils.Library;
using ProductTracking.Models.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Client.Util
{
    public static class CollectionUtils
    {
        public static Collection<T> ToCollection<T>(this IEnumerable<T> data)
        {
            return new Collection<T>(data.ToList());
        }
    }

    public static class TanExtensions
    {

        #region Partcipants
      
        public static IEnumerable<ReactionParticipantVM> OfType(this IEnumerable<ReactionParticipantVM> participants, params ParticipantType[] types)
        {
            if (participants == null)
                return null;
            return  participants.Where(p => types.Contains(p.ParticipantType));
        }
        public static IEnumerable<ReactionParticipantVM> OfChemicalType(this IEnumerable<ReactionParticipantVM> participants, ChemicalType type)
        {
            if (participants == null)
                return null;
            return participants.Where(p => p.ChemicalType == type);
        }
        public static IEnumerable<ReactionParticipantVM> OfReg(this IEnumerable<ReactionParticipantVM> participants, string regNumber)
        {
            if (participants == null)
                return null;
            return participants.Where(p => p.Reg == regNumber);
        }

        public static IEnumerable<ReactionParticipantVM> OfReaction(this IEnumerable<ReactionParticipantVM> participants, Guid reacitionId)
        {
            if (participants == null)
                return null;
            return participants.Where(p => p.ReactionVM != null && p.ReactionVM.Id == reacitionId);
        }

        public static IEnumerable<ReactionParticipantVM> OfReactionOfType(this IEnumerable<ReactionParticipantVM> participants, Guid reacitionId, ParticipantType type)
        {
            if (participants == null)
                return null;
            return participants.Where(p => p.ReactionVM != null && p.ReactionVM.Id == reacitionId && p.ParticipantType == type);
        }
        public static IEnumerable<ReactionParticipantVM> OfStageOfType(this IEnumerable<ReactionParticipantVM> participants, Guid stageId, ParticipantType type)
        {
            if (participants == null)
                return null;
            return participants.Where(p => p.StageVM != null && p.StageVM.Id == stageId && p.ParticipantType == type);
        }
        public static IEnumerable<ReactionParticipantVM> OfStage(this IEnumerable<ReactionParticipantVM> participants, Guid stageId)
        {
            if (participants == null)
                return null;
            return participants.Where(p => p.StageVM != null && p.StageVM.Id == stageId);
        }
        public static IEnumerable<ReactionParticipantVM> OfName(this IEnumerable<ReactionParticipantVM> participants, string name)
        {
            if (participants == null)
                return null;
            return participants.Where(p => p.Name.SafeEqualsLower(name));
        }
        public static IEnumerable<ReactionParticipantVM> OfNum(this IEnumerable<ReactionParticipantVM> participants, int Num)
        {
            if (participants == null)
                return null;
            return participants.Where(p => p.Num == Num);
        }//r.Num == selectedChemical.NUM || (!string.IsNullOrEmpty(r.Reg) && !string.IsNullOrEmpty(selectedChemical.RegNumber) && r.Reg == selectedChemical.RegNumber)
        public static IEnumerable<ReactionParticipantVM> OfNumOrReg(this IEnumerable<ReactionParticipantVM> participants, int Num, string Reg)
        {
            if (participants == null)
                return null;
            return participants.Where(r => r.Num == Num || (!string.IsNullOrEmpty(r.Reg) && !string.IsNullOrEmpty(Reg) && r.Reg == Reg));
        }
        public static IEnumerable<ReactionParticipantVM> OfExceptTypes(this IEnumerable<ReactionParticipantVM> participants, params ParticipantType[] types)
        {
            if (participants == null)
                return null;
            return participants.Where(p => !types.Contains(p.ParticipantType));
        }
        public static IEnumerable<ReactionParticipantVM> OfReactionAndStage(this IEnumerable<ReactionParticipantVM> participants, Guid reactionId, Guid stageId)
        {
            if (participants == null)
                return null;
            return participants.Where(r => r.ReactionVM != null && r.ReactionVM.Id == reactionId && r.StageVM != null && r.StageVM.Id == stageId);
        }

        public static IEnumerable<ReactionParticipantVM> OfReactionAndExceptStage(this IEnumerable<ReactionParticipantVM> participants, Guid reactionId, Guid stageId)
        {
            if (participants == null)
                return null;
            return participants.Where(r => r.ReactionVM != null && r.ReactionVM.Id == reactionId && (r.StageVM == null || r.StageVM.Id != stageId));
        }

        public static List<ReactionParticipantVM> ParticipantOfNUMOrReg(this List<ReactionParticipantVM> participants, TanChemical tanChemical, ReactionVM reaction)
        {
            if (participants == null)
                return null;
            return participants.Where(p =>
            (p.Num == tanChemical.NUM || p.Reg == tanChemical.RegNumber) &&
            p.ReactionVM.Id == reaction.Id &&
            p.StageVM != null &&
            reaction.SelectedStage != null &&
            p.StageVM.Id == reaction.SelectedStage.Id
            ).ToList();
        }
        public static List<ReactionParticipantVM> ExceptTypes(this List<ReactionParticipantVM> participants, List<ParticipantType> types)
        {
            if (participants == null)
                return null;
            return participants.Where(p => !types.Contains(p.ParticipantType)).ToList();
        }
        public static List<ReactionParticipantVM> OnlyTypes(this List<ReactionParticipantVM> participants, List<ParticipantType> types)
        {
            if (participants == null)
                return null;
            return participants.Where(p => types.Contains(p.ParticipantType)).ToList();
        }
        #endregion

        #region RSNs
        //if reactionId is null, will pass RSNs of all reactions
        public static Collection<RsnVM> OfReaction(this Collection<RsnVM> rsns, Guid reactionId, bool includeStages = false)
        {
            return rsns.Where(r => (reactionId != null ? r.Reaction != null && r.Reaction.Id == reactionId : true)
                                   && (includeStages ? true : r.Stage == null)
                             ).ToCollection();
        }

        public static Collection<RsnVM> ExceptReaction(this Collection<RsnVM> rsns, Guid reactionId, bool includeStages = false)
        {
            return rsns.Where(r => (reactionId != null ? r.Reaction != null && r.Reaction.Id != reactionId : true)
                                   && (includeStages ? true : r.Stage == null)
                             ).ToCollection();
        }

        public static Collection<RsnVM> OfReactionOnlyStages(this Collection<RsnVM> rsns, Guid reactionId)
        {
            return rsns.Where(r => (reactionId != null ? r.Reaction != null && r.Reaction.Id == reactionId : true)
                                   && (r.Stage != null)
                             ).ToCollection();
        }

        public static Collection<RsnVM> OfReactionAndStage(this Collection<RsnVM> rsns, Guid reactionId, Guid stageId)
        {
            return rsns.Where(r => r.Reaction != null && r.Reaction.Id == reactionId && r.Stage != null && r.Stage.Id == stageId).ToCollection();
        }
        public static Collection<RsnVM> OfFreetextContains(this Collection<RsnVM> rsns, string freetext)
        {
            return rsns.Where(r => r.FreeText.SafeContainsLower(freetext)).ToCollection();
        }
        public static Collection<RsnVM> OfCVTEquals(this Collection<RsnVM> rsns, string cvttext)
        {
            return rsns.Where(r => r.CvtText.SafeEqualsLower(cvttext)).ToCollection();
        }
        #endregion
    }
}
