using DTO;
using Newtonsoft.Json;
using ProductTracking.Models.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Web;

namespace ProductTracking.Models.Core
{
    public class TanChemical
    {
        public TanChemical()
        {
            Substancepaths = new List<SubstanceImagePaths>();
        }

        public Guid Id { get; set; }
        [JsonIgnore]
        public Tan Tan { get; set; }

        [Required]
        [ForeignKey("Tan")]
        [Index("IX_TAN_NUM_REG", IsUnique = true, Order = 1)]
        public int TanId { get; set; }
        public ChemicalType ChemicalType { get; set; }

        [Index("IX_TAN_NUM_REG", IsUnique = true, Order = 2)]
        public int NUM { get; set; }

        [Index("IX_TAN_NUM_REG", IsUnique = true, Order = 3)]
        [MaxLength(50)]
        [Required(AllowEmptyStrings = true)]
        public string RegNumber { get; set; }

        public string Formula { get; set; }

        public string ABSSterio { get; set; }
        public string PeptideSequence { get; set; }
        public string NuclicAcidSequence { get; set; }
        public string OtherName { get; set; }

        public String ImagePath { get; set; }
        public ICollection<SubstanceImagePaths> Substancepaths { get; set; }


        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }

        public string CompoundNo { get; set; }
        public string GenericName { get; set; }
        [Column(TypeName = "text")]
        public string MolString { get; set; }
        public ICollection<TanChemicalMetaData> MetaData { get; set; }
        public string InchiKey { get; set; }

        public String FirstImagePath
        {
            get
            {
                return Substancepaths != null && Substancepaths.Count > 0 ? Substancepaths.First().ImagePath : String.Empty;
            }
        }
        public string CSH { get; set; }
    }


    public class TanChemicalMetaData
    {
        public Guid Id { get; set; }
        public string PageNo { get; set; }
        public int LineNo { get; set; }
        public int ParaNo { get; set; }
        public int ColumnNo { get; set; }
        public int TableNo { get; set; }
        public int Figure { get; set; }
        public string Scheme { get; set; }
        public int Sheet { get; set; }
        public string FootNote { get; set; }
        public string OtherNotes { get; set; }
        public TanChemical TanChemical { get; set; }

        [Required]
        [ForeignKey("TanChemical")]
        public Guid TanChemicalId { get; set; }

    }

    public class Reaction
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        [JsonIgnore]
        public virtual Tan Tan { get; set; }
        [ForeignKey("Tan")]
        [Required]
        public int TanId { get; set; }

        [JsonIgnore]
        public Reaction AnalogousFrom { get; set; }
        [ForeignKey("AnalogousFrom")]
        public Guid? AnalogousFromId { get; set; }
        public DateTime? CuratorCreatedDate { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public DateTime? CuratorCompletedDate { get; set; }
        public DateTime? ReviewerCreatedDate { get; set; }
        public DateTime ReviewLastUpdatedDate { get; set; }
        public DateTime? ReviewerCompletedDate { get; set; }
        public DateTime QCLastUpdatedDate { get; set; }
        public DateTime? QCCompletedDate { get; set; }
        public virtual ICollection<Stage> Stages { get; set; }
        public int DisplayOrder { get; set; }
        public int Yield { get; set; }
        public bool IsCurationCompleted { get; set; }
        public bool IsReviewCompleted { get; set; }

        [JsonIgnore]
        public int KeyProductSequence
        {
            get
            {
                return Tan.Participants.Where(p => p.Reaction!=null && p.Reaction.Id == Id && p.KeyProduct).Select(p=>p.KeyProductSeq).FirstOrDefault();
            }
        }
        [JsonIgnore]
        public int KeyProductNum
        {
            get
            {
                return Tan.Participants.Where(p => p.Reaction != null && p.Reaction.Id == Id && p.KeyProduct).Select(p => p.Participant.NUM).FirstOrDefault();
            }
        }

        [JsonIgnore]
        public string RSD
        {
            get
            {
                var participants = Tan.Participants.Where(rp => rp.ReactionId == this.Id);
                StringBuilder rsd = new StringBuilder();
                var products = (from product in participants where product.ParticipantType == ParticipantType.Product select (from p in Tan.TanChemicals where p.Id == product.ParticipantId select p.NUM).FirstOrDefault() + ((!string.IsNullOrEmpty(product.DisplayYield) && product.DisplayYield != "0") ? "(" + product.DisplayYield + ")" : string.Empty)).ToList();
                rsd.Append("P=" + string.Join(",", products));
                foreach (var stage in this.Stages)
                {
                    var reactants = (from product in participants where product.ParticipantType == ParticipantType.Reactant && product.StageId == stage.Id select (from p in Tan.TanChemicals where p.Id == product.ParticipantId select p.NUM).FirstOrDefault()).ToList();
                    var solvents = (from product in participants where product.ParticipantType == ParticipantType.Solvent && product.StageId == stage.Id select (from p in Tan.TanChemicals where p.Id == product.ParticipantId select p.NUM).FirstOrDefault()).ToList();
                    var agents = (from product in participants where product.ParticipantType == ParticipantType.Agent && product.StageId == stage.Id select (from p in Tan.TanChemicals where p.Id == product.ParticipantId select p.NUM).FirstOrDefault()).ToList();
                    var catalyst = (from product in participants where product.ParticipantType == ParticipantType.Catalyst && product.StageId == stage.Id select (from p in Tan.TanChemicals where p.Id == product.ParticipantId select p.NUM).FirstOrDefault()).ToList();
                    string ReactantString = string.Join(",", reactants);
                    string solventString = string.Join(",", solvents);
                    string AgentString = string.Join(",", agents);
                    string CatalystString = string.Join(",", catalyst);
                    rsd.Append((!string.IsNullOrEmpty(ReactantString) ? "R=" + ReactantString : string.Empty) + (!string.IsNullOrEmpty(AgentString) ? "A=" + AgentString : string.Empty) +
                        (!string.IsNullOrEmpty(solventString) ? "S=" + solventString : string.Empty) + (!string.IsNullOrEmpty(CatalystString) ? "C=" + CatalystString : string.Empty) + ";");
                }
                return rsd.ToString().TrimEnd(';');
            }
        }
        [JsonIgnore]
        public int? RxnNum
        {
            get
            {
                return Tan.Participants.Where(tp => tp.ParticipantType == ParticipantType.Product).OrderBy(p => p.DisplayOrder).FirstOrDefault()?.Participant?.NUM;
            }
        }

        public Reaction()
        {
            Stages = new List<Stage>();
        }
    }


    public class Stage
    {
        public Guid Id { get; set; }

        [JsonIgnore]
        public virtual Reaction Reaction { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }

        [Required]
        [ForeignKey("Reaction")]
        public Guid ReactionId { get; set; }

        public virtual ICollection<StageCondition> StageConditions { get; set; }

        public int DisplayOrder { get; set; }

        public Stage()
        {
            StageConditions = new List<StageCondition>();
        }
    }

    public class ReactionRSD
    {
        public Guid Id { get; set; }

        [JsonIgnore]
        public virtual Tan Tan { get; set; }
        [Required]
        [ForeignKey("Tan")]
        public int TanId { get; set; }

        [JsonIgnore]
        public virtual Reaction Reaction { get; set; }
        [Required]
        [ForeignKey("Reaction")]
        public Guid ReactionId { get; set; }

        public virtual Stage Stage { get; set; }
        [ForeignKey("Stage")]
        public Guid? StageId { get; set; }

        [JsonIgnore]
        public virtual TanChemical Participant { get; set; }
        [Required]
        [ForeignKey("Participant")]
        public Guid ParticipantId { get; set; }
        public ParticipantType ParticipantType { get; set; }
        public string DisplayYield { get; set; }
        public float Yield { get; set; }
        public bool KeyProduct { get; set; }
        public int KeyProductSeq { get; set; }
        public int DisplayOrder { get; set; }
        public string Name { get; set; }
    }

    public class StageCondition
    {
        public Guid Id { get; set; }
        public string Temperature { get; set; }
        public string Pressure { get; set; }
        public string PH { get; set; }
        public string Time { get; set; }
        public string TEMP_TYPE { get; set; }
        public string TIME_TYPE { get; set; }
        public string PH_TYPE { get; set; }
        public string PRESSURE_TYPE { get; set; }
        [JsonIgnore]
        public virtual Stage Stage { get; set; }
        [ForeignKey("Stage")]
        [Required]
        public Guid StageId { get; set; }
        public int DisplayOrder { get; set; }
    }


    public class ReactionRSN
    {
        public Guid Id { get; set; }

        [JsonIgnore]
        public virtual Tan Tan { get; set; }
        [Required]
        [ForeignKey("Tan")]
        public int TanId { get; set; }

        [JsonIgnore]
        public virtual Reaction Reaction { get; set; }
        [Required]
        [ForeignKey("Reaction")]
        public Guid ReactionId { get; set; }

        [JsonIgnore]
        public virtual Stage Stage { get; set; }
        [ForeignKey("Stage")]
        public Guid? StageId { get; set; }

        public string CVT { get; set; }
        public string FreeText { get; set; }
        public int DisplayOrder { get; set; }

        public bool IsIgnorableInDelivery { get; set; }

        public List<Guid> ReactionParticipantId { get; set; }

        public bool IsDifferentFrom(ReactionRSN otherRsn)
        {
            if (otherRsn == null)
                return true;
            return this.DisplayOrder != otherRsn.DisplayOrder || this.CVT != otherRsn.CVT || this.FreeText != otherRsn.FreeText;
        }

        public string Level
        {
            get
            {
                if (Stage != null)
                    return "Stage";
                if (Reaction != null)
                    return "Reaction";
                return String.Empty;
            }
        }
    }


    public class CVT
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(2000)]
        [Index("IX_CVTS", IsUnique = true)]
        public string CVTS { get; set; }
        public String AssociatedFreeText { get; set; }
        public bool IsIgnorableInDelivery { get; set; }
        public ParticipantType ExistingType { get; set; }
        public ParticipantType NewType { get; set; }
    }


    public class TanKeywords
    {
        public int Id { get; set; }
        public string keyword { set; get; }
        public string Status { set; get; }
    }

    public class TanWiseKeywords
    {
        public int Id { get; set; }
        public string TanKeywords { get; set; }
        [JsonIgnore]
        public virtual Tan Tan { get; set; }
        [ForeignKey("Tan")]
        [Index("IX_TAN", IsUnique = true)]
        public int TanId { get; set; }
    }

    public class FreeText
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(2000)]
        [Index("IX_FT", IsUnique = true)]
        public string FreeTexts { get; set; }
    }

    public class CommentDictionary
    {
        public CommentDictionary()
        {
            CVT = new List<Core.CVT>();
            FreeText = new List<Core.FreeText>();
        }
        public List<CVT> CVT { get; set; }
        public List<FreeText> FreeText { get; set; }
    }

    public class SubstanceImagePaths
    {
        public int Id { get; set; }
        public string ImagePath { get; set; }
        [JsonIgnore]
        public virtual TanChemical TanChemical { get; set; }
        [Required]
        [ForeignKey("TanChemical")]
        public Guid TanChemicalId { get; set; }
    }
}