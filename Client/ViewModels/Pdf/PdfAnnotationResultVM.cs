using Client.ViewModel;

namespace Client.ViewModels.Pdf
{
    public class PdfAnnotationResultVM : OrderableVM
    {
        private string text, author, type;
        private int pageNum, displayOrder;
        public string Text { get { return text; } set { SetProperty(ref text, value); } }
        public string Author { get { return author; } set { SetProperty(ref author, value); } }
        public string Type { get { return type; } set { SetProperty(ref type, value); } }
        public int PageNum { get { return pageNum; } set { SetProperty(ref pageNum, value); } }
        public override int DisplayOrder { get { return displayOrder; } set { SetProperty(ref displayOrder, value); } }

        public override string ToString()
        {
            return $"{Text} - Page {PageNum}";
        }
    }
}