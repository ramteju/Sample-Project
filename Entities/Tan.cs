using Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using Excelra.Utils.Library;
namespace ProductTracking.Models.Core
{
    public class Tan
    {
        public int Id { get; set; }

        [MaxLength(50)]
        [Required]
        public string tanNumber { get; set; }
        public TanCategory TanCategory { get; set; }
        public TanState? TanState { get; set; }

        [Required]
        public virtual Batch Batch { get; set; }
        [ForeignKey("Batch")]
        public int BatchId { get; set; }
        public DateTime DateCreated { get; set; }
        public virtual ICollection<Reaction> Reactions { get; set; }
        public virtual ICollection<TanChemical> TanChemicals { get; set; }
        public virtual ICollection<ReactionRSD> Participants { get; set; }
        public virtual ICollection<ReactionRSN> RSNs { get; set; }
        public virtual ICollection<Comments> TanComments { get; set; }
        [JsonIgnore]
        public virtual ICollection<DeliveryBatch> DeliveryBatches { get; set; }
        [JsonIgnore]
        public virtual ApplicationUser Curator { get; set; }
        [ForeignKey("Curator")]
        public string CuratorId { get; set; }

        [JsonIgnore]
        public virtual ApplicationUser Reviewer { get; set; }
        [ForeignKey("Reviewer")]
        public string ReviewerId { get; set; }

        [JsonIgnore]
        public virtual ApplicationUser QC { get; set; }
        [ForeignKey("QC")]
        public string QCId { get; set; }
        [JsonIgnore]
        public virtual ApplicationUser CurrentUser { get; set; }
        [ForeignKey("CurrentUser")]
        public string CurrentUserId { get; set; }
        [JsonIgnore]
        public virtual UserRole CurrentUserRole { get; set; }
        [ForeignKey("CurrentUserRole")]
        public int? CurrentUserRoleId { get; set; }
        public String DocumentPath { get; set; }
        public String LocalDocumentPath { get; set; }

        [JsonIgnore]
        public virtual ApplicationUser LastAccessedBy { get; set; }
        public DateTime? LastAccessedTime { get; set; }
        [JsonIgnore]
        public virtual ApplicationUser ApplicationUser { get; set; }
        [ForeignKey("ApplicationUser")]
        public string DocumentReviwedUser { get; set; }
        public DateTime? DocumentReadStartTime { get; set; }
        public DateTime? DocumentReadCompletedTime { get; set; }
        public bool QCRequired { get; set; }
        public TaskAllocatedType AllocatedType { get; set; }
        public bool IsQCCompleted { get; set; }
        public int NumsCount { get; set; }
        public int RxnCount { get; set; }
        public bool MarkedAsQuery { get; set; }
        public int DocumentCurrentPage { get; set; }
        public int? Version { get; set; }

        //[JsonIgnore]
        //public virtual DeliveryBatch DeliveryBatch { get; set; }
        //public int? DeliveryBatchId { get; set; }
        public string DeliveryRevertMessage { get; set; }
        public Tan()
        {
            TanCategory = TanCategory.Progress;
            Reactions = new List<Reaction>();
            TanChemicals = new List<TanChemical>();
            Participants = new List<ReactionRSD>();
            RSNs = new List<ReactionRSN>();
            TanComments = new List<Comments>();
            DeliveryBatches = new List<DeliveryBatch>();
        }
        public TanStatus TanStatus { get; set; }
        public string IsDuplicate { get; set; }
        public string CAN { get; set; }
        public string TanType { get; set; }
        public string JournalName { get; set; }
        public string Issue { get; set; }
        public string JournalYear { get; set; }
        public String TotalDocumentsPath { get; set; }
        public string OCRStatus { get; set; }
        public string DocClass { get; set; }
        public string density { get; set; }
        public string ProcessingNode { get; set; }
        public DateTime? TargetedDate { get; set; }
        public DateTime? ShipmentreceivedDate { get; set; }
        public string SpreadSheetName { get; set; }
        #region XML Comments
        private List<string> TypeWiseCommentTexts(CommentType type, bool includeColon)
        {
           return TanComments.Where(tc => tc.CommentType == type).Select(tc => $"{tc.CommentType.DescriptionAttribute()}{(includeColon ? ": " : " ")}{tc.TotalComment.TrimEnd('.').Trim()}").ToList();
            //if (texts.Any() && texts.Count() == 1)
            //    return texts.Select(c => $"{c}. ").FirstOrDefault();
            //return String.Join("", texts);
        }
        public string CommentsForXml
        {
            get
            {
                var allComments = new List<string>();
                if (TanComments != null)
                {
                    allComments.AddRange(TypeWiseCommentTexts(CommentType.CAS, true));
                    allComments.AddRange(TypeWiseCommentTexts(CommentType.INDEXING, true));
                    allComments.AddRange(TypeWiseCommentTexts(CommentType.AUTHOR, true));
                    allComments.AddRange(TypeWiseCommentTexts(CommentType.TEMPERATURE, true));
                    allComments.AddRange(TypeWiseCommentTexts(CommentType.OTHER, true));
                    allComments.AddRange(TypeWiseCommentTexts(CommentType.DEFAULT, true));
                }

                return string.Join(". ", allComments).Replace("Reactant","reactant").Replace("Product","product");
            }
        }
        #endregion
    }

    public class Comments
    {

        public Guid Id { get; set; }
        public string Comment { get; set; }
        public CommentType CommentType { get; set; }
        public int Length { get; set; }
        public int Num { get; set; }
        public string Page { get; set; }
        public string Para { get; set; }
        public string Line { get; set; }
        public string Column { get; set; }
        public string Table { get; set; }
        public string Figure { get; set; }
        public string Schemes { get; set; }
        public string Sheet { get; set; }
        public string FootNote { get; set; }
        public string TotalComment { get; set; }
    }

    public class RegulerExpression
    {
        public int Id { get; set; }
        public RegulerExpressionFor RegulerExpressionFor { get; set; }
        public string Expression { get; set; }
    }

    public class TanIssues
    {
        public Guid Id { get; set; }
        [JsonIgnore]
        public virtual Tan Tan { get; set; }
        public string IssueDescription { get; set; }
        public TanIssueType TanIssueType { get; set; }
        public string Status { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        [ForeignKey("ApplicationUser")]
        public string LastUpdatedUser { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string strCSH { get; set; }
        public string strCSM { get; set; }
        public string RegNumber { get; set; }
        public virtual  TanChemical TanChemical { get; set; }
    }

 


    public enum CommentType
    {
        [Description("Indexing error")]
        INDEXING = 0,
        [Description("Author error")]
        AUTHOR = 1,
        [Description("Other comment")]
        OTHER = 2,
        [Description("Other comment")]
        TEMPERATURE = 3,
        [Description("Other comment")]
        DEFAULT = 4,
        [Description("CAS consulted for")]
        CAS = 5
    }

    public enum RegulerExpressionFor
    {
        [Description("Characters To Allow in CVT")]
        CVT = 0,
        [Description("Characters To Allow in FreeText")]
        FreeText = 1
       
    }

    public enum TaskAllocatedType
    {
        [Description("Manual Allocation")]
        MANUALALLOCATION = 0,
        [Description("Manual Reallocation")]
        MANUALREALLOCATION = 1,
        [Description("Auto Allocation")]
        AUTOALLOCATION = 2
    }
    public enum TanIssueType
    {
        [Description("Num Information Missed")]
        NUMINFORMATIONMISSED = 0,
        [Description("Registration Number Missed")]
        REG_NUM_MISSED = 1
        
    }
    public enum TanStatus
    {
        [Description("Live")]
        Live = 0,
        [Description("OnHold")]
        OnHold = 1
    }

}