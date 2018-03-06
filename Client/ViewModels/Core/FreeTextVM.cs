using System;

namespace Client.ViewModels
{
    public class FreeTextVM : ViewModelBase
    {
        private int id;
        private String text;

        public int Id { get { return id; } set { SetProperty(ref id, value); } }
        public String Text { get { return text; } set { SetProperty(ref text, value); } }
    }
}
