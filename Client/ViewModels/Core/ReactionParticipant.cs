using Client.Command;
using Client.Common;
using Client.ViewModel;
using Client.Views;
using DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Forms;

namespace Client.ViewModels
{
    public class ReactionParticipantVM : ViewModelBase
    {
        private Guid id;
        private string reg, name, formula;
        private int num;
        private int displayOrder;
        private string yield;
        private Guid tanChemicalId;
        private ParticipantType participantType;
        private ChemicalType chemicalType;
        private ReactionVM reactionVM;
        private StageVM stageVM;
        private bool keyProduct;
        private int keyProductSeqWithOutNum;
        private bool isYieldEditable;
        private bool isIgnorable;
        Regex yieldReg = new Regex(S.YIELD_REG_EXP);

        public Guid Id { get { return id; } set { SetProperty(ref id, value); } }
        public int Num { get { return num; } set { SetProperty(ref num, value); } }
        public int DisplayOrder { get { return displayOrder; } set { SetProperty(ref displayOrder, value); } }
        public String Reg { get { return reg; } set { SetProperty(ref reg, value); } }
        public String Formula { get { return formula; } set { SetProperty(ref formula, value); } }
        public String Name { get { return name; } set { SetProperty(ref name, value); } }
        public ParticipantType ParticipantType
        {
            get { return participantType; }
            set
            {
                SetProperty(ref participantType, value);
                IsYieldEditable = value == ParticipantType.Product ? false : true;
            }
        }
        public bool IsYieldEditable { get { return isYieldEditable; } set { SetProperty(ref isYieldEditable, value); } }
        public string Yield
        {
            get { return yield; }
            set
            {
                if (string.IsNullOrEmpty(value) || yieldReg.IsMatch(value))
                {
                    if (!string.IsNullOrEmpty(value) && !value.Contains(",") && int.Parse(value) > 100)
                        AppInfoBox.ShowInfoMessage("Yield Value exceeds 100.");
                    else if (!string.IsNullOrEmpty(value) && value.StartsWith("0"))
                        AppInfoBox.ShowInfoMessage("Yield Value started with 0.");
                    else
                        SetProperty(ref yield, value);
                }
                else
                    AppInfoBox.ShowInfoMessage("Invalid Yield Value.");
            }
        }
        public int ReactionYield
        {
            get
            {
                int yield = 0;
                if (!string.IsNullOrEmpty(Yield))
                {
                    var indexes = Yield.Split(',');
                    if (indexes.Length > 0)
                        int.TryParse(indexes[0], out yield);
                }
                return yield;
            }
        }
        public int ProductYield
        {
            get
            {
                int yield = 0;
                if (!string.IsNullOrEmpty(Yield) && Yield.Contains(","))
                {
                    var indexes = Yield.Split(',');
                    if (indexes.Length > 1)
                        int.TryParse(indexes[1], out yield);
                    //else
                    //    int.TryParse(indexes[0], out yield);
                }
                return yield;
            }
        }



        public Guid TanChemicalId { get { return tanChemicalId; } set { SetProperty(ref tanChemicalId, value); } }
        public ReactionVM ReactionVM { get { return reactionVM; } set { SetProperty(ref reactionVM, value); } }
        public StageVM StageVM { get { return stageVM; } set { SetProperty(ref stageVM, value); } }
        public ChemicalType ChemicalType { get { return chemicalType; } set { SetProperty(ref chemicalType, value); } }
        public bool KeyProduct { get { return keyProduct; } set { SetProperty(ref keyProduct, value); } }
        public int KeyProductSeqWithOutNum { get { return keyProductSeqWithOutNum; } set { SetProperty(ref keyProductSeqWithOutNum, value); } }
        public bool IsIgnorable { get { return isIgnorable; } set { SetProperty(ref isIgnorable, value); } }

        public ReactionParticipantVM()
        {
            IsYieldEditable = true;
        }

        public void DisplayDuplicateString(string RegNumber)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Chemical with Reg Number ");
            sb.Append(RegNumber);
            sb.AppendLine(" Already added.");
            sb.AppendLine($"RXN : {this.ReactionVM.DisplayOrder} [{this.ReactionVM.KeyProductSeq}]");
            sb.AppendLine("Stage : " + this.StageVM?.Name);
            sb.AppendLine("Category : " + this.ParticipantType);
            sb.AppendLine("NUM : " + this.Num);
            sb.AppendLine("Series : " + this.ChemicalType);
            AppInfoBox.ShowInfoMessage(sb.ToString());
        }
    }
}
