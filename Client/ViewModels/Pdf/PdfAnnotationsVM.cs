using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels.Pdf
{
    public class PdfAnnotationsVM : ViewModelBase
    {
        private string totalCount, fileName;
        private ObservableCollection<PdfAnnotationResultVM> results;
        private PdfAnnotationResultVM selectedAnnotation;
        public string TotalCount { get { return totalCount; } set { SetProperty(ref totalCount, value); } }
        public PdfAnnotationResultVM SelectedAnnotation { get { return selectedAnnotation; } set { SetProperty(ref selectedAnnotation, value); } }
        public string FileName { get { return fileName; } set { SetProperty(ref fileName, value); } }        
        public ObservableCollection<PdfAnnotationResultVM> Results { get { return results; } set { SetProperty(ref results, value); } }


        public void Clear()
        {
            Results = new ObservableCollection<PdfAnnotationResultVM>();
            TotalCount = String.Empty;
        }
    }
}
