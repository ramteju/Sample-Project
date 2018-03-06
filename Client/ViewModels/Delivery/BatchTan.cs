using ProductTracking.Models.Core;
using Entities;
using Excelra.Utils.Library;
using Client.ViewModel;

namespace Client.ViewModels.Delivery
{
    public class BatchTanVM : OrderableVM
    {
        private int batchNum, id;
        private TanState? tanState;
        private string tanNumber, tanType,targetedDate;
        private TanCategoryVM tanCategory;
        private bool nearToTargetDate;
        private string isDoubtRaised;
        private int displayOrder;
        private string processingNote;

        public int Id { get { return id; } set { SetProperty(ref id, value); } }
        public TanState? TanState { get { return tanState; } set { SetProperty(ref tanState, value); } }
        public int BatchNum { get { return batchNum; } set { SetProperty(ref batchNum, value); } }
        public string TanNumber { get { return tanNumber; } set { SetProperty(ref tanNumber, value); } }
        public TanCategoryVM TanCategory { get { return tanCategory; } set { SetProperty(ref tanCategory, value); } }
        public bool NearToTargetDate { get { return nearToTargetDate; } set { SetProperty(ref nearToTargetDate, value); } }
        public string IsDoubtRaised { get { return isDoubtRaised; } set { SetProperty(ref isDoubtRaised, value); } }
        public string TanType { get { return tanType; } set { SetProperty(ref tanType, value); } }
        public string TargetedDate { get { return targetedDate; } set { SetProperty(ref targetedDate, value); } }
        public string ProcessingNote { get { return processingNote; } set { SetProperty(ref processingNote, value); } }
        public int Nums { get; set; }
        public int Rxns { get; set; }
        public int Stages { get; set; }
        public string StatusString { get { return TanState.DescriptionAttribute(); } }
        public string Curator { get; set; }
        public string Reviewer { get; set; }
        public string QC { get; set; }
        public int Version { get; set; }
        public Role CurrentRole { get; set; }

        public override int DisplayOrder
        {
            get { return displayOrder; }
            set
            {
                SetProperty(ref displayOrder, value);
            }
        }
    }
}
