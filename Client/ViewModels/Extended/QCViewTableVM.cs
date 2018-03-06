using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels.Extended
{
    public class QCTAbleViewVM : ViewModelBase
    {
        private ObservableCollection<CuratedTanVM> curatedTanData;
        private int totalReactions;
        private int pH;
        private int totalParticipants;
        private int totalStages;
        private int products;
        private int time;
        private int distinct8000Count;
        private int reactants;
        private int pressure;
        private int reactionsWith8000;
        private int catalyst;
        private int temp;
        private int solvent;
        private int agent;

        public ObservableCollection<CuratedTanVM> CuratedTanData { get { return curatedTanData; } set { SetProperty(ref curatedTanData, value); } }
        public int TotalReactions { get { return totalReactions; } set { SetProperty(ref totalReactions, value); } }
        public int TotalParticipants { get { return totalParticipants; } set { SetProperty(ref totalParticipants, value); } }
        public int TotalStages { get { return totalStages; } set { SetProperty(ref totalStages, value); } }
        public int Products { get { return products; } set { SetProperty(ref products, value); } }
        public int Time { get { return time; } set { SetProperty(ref time, value); } }
        public int Distinct8000Count { get { return distinct8000Count; } set { SetProperty(ref distinct8000Count, value); } }
        public int Reactants { get { return reactants; } set { SetProperty(ref reactants, value); } }
        public int Pressure { get { return pressure; } set { SetProperty(ref pressure, value); } }
        public int ReactionsWith8000 { get { return reactionsWith8000; } set { SetProperty(ref reactionsWith8000, value); } }
        public int Catalyst { get { return catalyst; } set { SetProperty(ref catalyst, value); } }
        public int Temp { get { return temp; } set { SetProperty(ref temp, value); } }
        public int Solvent { get { return solvent; } set { SetProperty(ref solvent, value); } }
        public int Agent { get { return agent; } set { SetProperty(ref agent, value); } }
        public int PH { get { return pH; } set { SetProperty(ref pH, value); } }



        public QCTAbleViewVM()
        {
            CuratedTanData = new ObservableCollection<CuratedTanVM>();
        }

        public void PrepareData(TanVM tanVM)
        {
            var GroupedParticipants = tanVM.ReactionParticipants.GroupBy(rp => rp.ReactionVM.Id).ToDictionary(k => k.Key, k => k.ToList());
            var GroupedRsns = tanVM.Rsns.GroupBy(rp => rp.Reaction.Id).ToDictionary(k => k.Key, k => k.ToList());
            this.CuratedTanData = new ObservableCollection<CuratedTanVM>();
            this.Temp = 0;
            this.Time = 0;
            this.Pressure = 0;
            this.PH = 0;
            foreach (ReactionVM reactionVM in tanVM.Reactions)
            {
                var FirstStage = reactionVM.Stages.OrderBy(s => s.DisplayOrder).FirstOrDefault();
                if (FirstStage != null)
                {
                    var reaction = new CuratedTanVM();
                    reaction.Id = reactionVM.DisplayOrder;
                    reaction.Sno = reactionVM.DisplayOrder.ToString();
                    reaction.RxnNumWithSequence = reactionVM.KeyProductSeq;
                    reaction.Products = GroupedParticipants.ContainsKey(reactionVM.Id) ? string.Join(",", GroupedParticipants[reactionVM.Id].Where(rp => rp.ParticipantType == DTO.ParticipantType.Product).Select(rp => $"{rp.Num} {(!String.IsNullOrEmpty(rp.Yield) ? "(" + rp.Yield + ")" : string.Empty )}")) : string.Empty;
                    reaction.Reactants = GroupedParticipants.ContainsKey(reactionVM.Id) ? string.Join(",", GroupedParticipants[reactionVM.Id].Where(rp => rp.ParticipantType == DTO.ParticipantType.Reactant && rp.StageVM!=null && rp.StageVM.Id == FirstStage.Id).Select(rp => $"{rp.Num}")) : string.Empty;
                    reaction.Agents = GroupedParticipants.ContainsKey(reactionVM.Id) ? string.Join(",", GroupedParticipants[reactionVM.Id].Where(rp => rp.ParticipantType == DTO.ParticipantType.Agent && rp.StageVM != null && rp.StageVM.Id == FirstStage.Id).Select(rp => $"{rp.Num}")) : string.Empty;
                    reaction.Catalysts = GroupedParticipants.ContainsKey(reactionVM.Id) ? string.Join(",", GroupedParticipants[reactionVM.Id].Where(rp => rp.ParticipantType == DTO.ParticipantType.Catalyst && rp.StageVM != null && rp.StageVM.Id == FirstStage.Id).Select(rp => $"{rp.Num}")) : string.Empty;
                    reaction.Solvents = GroupedParticipants.ContainsKey(reactionVM.Id) ? string.Join(",", GroupedParticipants[reactionVM.Id].Where(rp => rp.ParticipantType == DTO.ParticipantType.Solvent && rp.StageVM != null && rp.StageVM.Id == FirstStage.Id).Select(rp => $"{rp.Num}")) : string.Empty;
                    reaction.Time = string.Join(",", FirstStage.Conditions.Where(c=>!string.IsNullOrEmpty(c.Time)).Select(c => c.Time).ToList());
                    this.Time = !string.IsNullOrEmpty(reaction.Time) ? this.time + reaction.Time.Split(',').Count() : this.Time;
                    reaction.Temperature = string.Join(",", FirstStage.Conditions.Where(c => !string.IsNullOrEmpty(c.Temperature)).Select(c => c.Temperature).ToList());
                    this.Temp = !string.IsNullOrEmpty(reaction.Temperature) ? this.Temp + reaction.Temperature.Split(',').Count() : this.Temp;
                    reaction.Pressure = string.Join(",", FirstStage.Conditions.Where(c => !string.IsNullOrEmpty(c.Pressure)).Select(c => c.Pressure).ToList());
                    this.Pressure = !string.IsNullOrEmpty(reaction.Pressure) ? this.Pressure + reaction.Pressure.Split(',').Count() : this.Pressure;
                    reaction.PH = string.Join(",", FirstStage.Conditions.Where(c => !string.IsNullOrEmpty(c.PH)).Select(c => c.PH).ToList());
                    this.PH = !string.IsNullOrEmpty(reaction.PH) ? this.PH + reaction.PH.Split(',').Count() : this.PH;
                    reaction.Cvt = GroupedRsns.ContainsKey(reactionVM.Id) ? string.Join(",", GroupedRsns[reactionVM.Id].Where(rsn => (rsn.Stage == null || rsn.Stage.Id == FirstStage.Id) && !string.IsNullOrEmpty(rsn.CvtText)).Select(cvt => cvt.CvtText)) : string.Empty;
                    reaction.Freetext = GroupedRsns.ContainsKey(reactionVM.Id) ? string.Join(",", GroupedRsns[reactionVM.Id].Where(rsn => (rsn.Stage == null || rsn.Stage.Id == FirstStage.Id) && !string.IsNullOrEmpty(rsn.FreeText)).Select(cvt => cvt.FreeText)) : string.Empty;
                    this.CuratedTanData.Add(reaction);
                    foreach (StageVM stage in reactionVM.Stages)
                    {
                        if(stage.Id != FirstStage.Id)
                        {
                            var stagereaction = new CuratedTanVM();
                            stagereaction.Id = reactionVM.DisplayOrder;
                            stagereaction.Reactants = GroupedParticipants.ContainsKey(reactionVM.Id) ? string.Join(",", GroupedParticipants[reactionVM.Id].Where(rp => rp.ParticipantType == DTO.ParticipantType.Reactant && rp.StageVM != null && rp.StageVM.Id == stage.Id).Select(rp => $"{rp.Num}")) : string.Empty;
                            stagereaction.Agents = GroupedParticipants.ContainsKey(reactionVM.Id) ? string.Join(",", GroupedParticipants[reactionVM.Id].Where(rp => rp.ParticipantType == DTO.ParticipantType.Agent && rp.StageVM != null && rp.StageVM.Id == stage.Id).Select(rp => $"{rp.Num}")) : string.Empty;
                            stagereaction.Catalysts = GroupedParticipants.ContainsKey(reactionVM.Id) ? string.Join(",", GroupedParticipants[reactionVM.Id].Where(rp => rp.ParticipantType == DTO.ParticipantType.Catalyst && rp.StageVM != null && rp.StageVM.Id == stage.Id).Select(rp => $"{rp.Num}")) : string.Empty;
                            stagereaction.Solvents = GroupedParticipants.ContainsKey(reactionVM.Id) ? string.Join(",", GroupedParticipants[reactionVM.Id].Where(rp => rp.ParticipantType == DTO.ParticipantType.Solvent && rp.StageVM != null && rp.StageVM.Id == stage.Id).Select(rp => $"{rp.Num}")) : string.Empty;
                            stagereaction.Time = string.Join(",", stage.Conditions.Where(c => !string.IsNullOrEmpty(c.Time)).Select(c => c.Time).ToList());
                            stagereaction.Temperature = string.Join(",", stage.Conditions.Where(c => !string.IsNullOrEmpty(c.Temperature)).Select(c => c.Temperature).ToList());
                            stagereaction.Pressure = string.Join(",", stage.Conditions.Where(c => !string.IsNullOrEmpty(c.Pressure)).Select(c => c.Pressure).ToList());
                            stagereaction.PH = string.Join(",", stage.Conditions.Where(c => !string.IsNullOrEmpty(c.PH)).Select(c => c.PH).ToList());
                            this.Time = !string.IsNullOrEmpty(stagereaction.Time) ? this.time + stagereaction.Time.Split(',').Count() : this.Time;
                            this.Temp = !string.IsNullOrEmpty(stagereaction.Temperature) ? this.Temp + stagereaction.Temperature.Split(',').Count() : this.Temp;
                            this.Pressure = !string.IsNullOrEmpty(stagereaction.Pressure) ? this.Pressure + stagereaction.Pressure.Split(',').Count() : this.Pressure;
                            this.PH = !string.IsNullOrEmpty(stagereaction.PH) ? this.PH + stagereaction.PH.Split(',').Count() : this.PH;
                            stagereaction.Cvt = GroupedRsns.ContainsKey(reactionVM.Id) ? string.Join(",", GroupedRsns[reactionVM.Id].Where(rsn => rsn.Stage != null && rsn.Stage.Id == stage.Id && !string.IsNullOrEmpty(rsn.CvtText)).Select(cvt => cvt.CvtText)) : string.Empty;
                            stagereaction.Freetext = GroupedRsns.ContainsKey(reactionVM.Id) ? string.Join(",", GroupedRsns[reactionVM.Id].Where(rsn => rsn.Stage != null && rsn.Stage.Id == stage.Id && !string.IsNullOrEmpty(rsn.FreeText)).Select(cvt => cvt.FreeText)) : string.Empty;
                            this.CuratedTanData.Add(stagereaction);
                        }
                    }
                }
            }
            this.TotalReactions = tanVM.Reactions.Count();
            this.TotalParticipants = tanVM.ReactionParticipants.Count();
            this.TotalStages = tanVM.Reactions.Select(r => r.Stages.Count()).Sum();
            this.Products = tanVM.ReactionParticipants.Where(rp => rp.ParticipantType == DTO.ParticipantType.Product).Count();
            this.Distinct8000Count = tanVM.TanChemicals.Where(tc => tc.ChemicalType == DTO.ChemicalType.S8000).Count();
            this.Reactants = tanVM.ReactionParticipants.Where(rp => rp.ParticipantType == DTO.ParticipantType.Reactant).Count();
            this.ReactionsWith8000 = GroupedParticipants.Where(D => D.Value.Where(C => C.ChemicalType == DTO.ChemicalType.S8000).Any()).Count();
            this.Catalyst = tanVM.ReactionParticipants.Where(rp => rp.ParticipantType == DTO.ParticipantType.Catalyst).Count();
            this.Solvent = tanVM.ReactionParticipants.Where(rp => rp.ParticipantType == DTO.ParticipantType.Solvent).Count();
            this.Agent = tanVM.ReactionParticipants.Where(rp => rp.ParticipantType == DTO.ParticipantType.Agent).Count();
        }

    }
    public class CuratedTanVM : ViewModelBase
    {
        private string sno;
        private string rxnNumWithSequence;
        private string reactants, products;
        private string agents;
        private string catalysts;
        private string solvents, time, temparature, pH, cvt, pressure, freetext;
        private int id;

        public int Id { get { return id; } set { SetProperty(ref id, value); } }
        public string Sno { get { return sno; } set { SetProperty(ref sno, value); } }
        public string RxnNumWithSequence { get { return rxnNumWithSequence; } set { SetProperty(ref rxnNumWithSequence, value); } }
        public string Products { get { return products; } set { SetProperty(ref products, value); } }
        public string Reactants { get { return reactants; } set { SetProperty(ref reactants, value); } }
        public string Agents { get { return agents; } set { SetProperty(ref agents, value); } }
        public string Catalysts { get { return catalysts; } set { SetProperty(ref catalysts, value); } }
        public string Solvents { get { return solvents; } set { SetProperty(ref solvents, value); } }
        public string Time { get { return time; } set { SetProperty(ref time, value); } }
        public string Temperature { get { return temparature; } set { SetProperty(ref temparature, value); } }
        public string Pressure { get { return pressure; } set { SetProperty(ref pressure, value); } }
        public string PH { get { return pH; } set { SetProperty(ref pH, value); } }
        public string Cvt { get { return cvt; } set { SetProperty(ref cvt, value); } }
        public string Freetext { get { return freetext; } set { SetProperty(ref freetext, value); } }
    }
}
