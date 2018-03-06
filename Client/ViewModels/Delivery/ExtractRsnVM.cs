using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels.Delivery
{
    public class ExtractRsnVM : ViewModelBase
    {
        private Guid id;
        private string tanNumber, cvt, freeText, level,comment;
        private int sno, rxnSeq, totalLength;
        private int? productNumber, stage;
        private int displayOrder;

        public string TanNumber { get { return tanNumber; } set { SetProperty(ref tanNumber, value); } }
        public int? Stage { get { return stage; } set { SetProperty(ref stage, value); } }
        public string FreeText { get { return freeText; } set { SetProperty(ref freeText, value); } }
        public Guid Id { get { return id; } set { SetProperty(ref id, value); } }
        public string CVT { get { return cvt; } set { SetProperty(ref cvt, value); } }
        public string Comment { get { return comment; } set { SetProperty(ref comment, value); } }
        public string Level { get { return level; } set { SetProperty(ref level, value); } }
        public int RXNSno { get { return sno; } set { SetProperty(ref sno, value); } }
        public int RxnSeq { get { return rxnSeq; } set { SetProperty(ref rxnSeq, value); } }
        public int? ProductNumber { get { return productNumber; } set { SetProperty(ref productNumber, value); } }
        public int TotalLength { get { return totalLength; } set { SetProperty(ref totalLength, value); } }
        public int DisplayOrder { get { return displayOrder; } set { SetProperty(ref displayOrder, value); } }

    }
}
