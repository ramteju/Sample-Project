using FoxitPDFSDKProLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels.Core
{
    public class PdfSearchResultVM : ViewModelBase
    {
        private string resultText;
        private int pageNum;
        public FindResult FindResult;
        public string ResultText { get { return resultText; } set { SetProperty(ref resultText, value); } }
        public int PageNum { get { return pageNum; } set { SetProperty(ref pageNum, value); } }

        public override string ToString()
        {
            return $"{ResultText} - Page {PageNum}";
        }
    }
}
