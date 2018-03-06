using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels.Core
{
    public class PdfSearchVM : ViewModelBase
    {
        private string totalCount;
        private ObservableCollection<PdfSearchResultVM> results;
        public string TotalCount { get { return totalCount; } set { SetProperty(ref totalCount, value); } }
        public ObservableCollection<PdfSearchResultVM> Results { get { return results; } set { SetProperty(ref results, value); } }

        public void Clear()
        {
            Results = new ObservableCollection<PdfSearchResultVM>();
            TotalCount = String.Empty;
        }
    }
}
