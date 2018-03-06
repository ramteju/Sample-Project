using Client.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Views;
namespace Client.ViewModels
{
      public class UserManualVM : ViewModelBase
    {
        public string Value { get; set; }
        public string Name { get; set; }


        private UserManualVM selectedUserManual;
        //private void OpenSelectedPdf(object s)
        //{
        //    string Pdfpath = s.ToString();
        //    bool? result = new Manuals().UserManualOpenDocument(Pdfpath);

        //}

        public UserManualVM SelectedUserManual
        {
            get { return selectedUserManual; }
            set
            {
                SetProperty(ref selectedUserManual, value);
                if (value != null)
                {
                    OpenSelectedPdf(value.Value);
                }
            }
        }
        private void OpenSelectedPdf(object s)
        {
            string Pdfpath = s.ToString();
           // bool? result = new UserManual().UserManualOpenDocument(Pdfpath);
        }
    }
}
