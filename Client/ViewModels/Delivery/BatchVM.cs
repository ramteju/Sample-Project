using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels.Delivery
{
    public class BatchVM : ViewModelBase
    {
        private int id, name;
        private DateTime? dateCreated;
        private string documentsPath;

        public int Id { get { return id; } set { SetProperty(ref id, value); } }
        public int Name { get { return name; } set { SetProperty(ref name, value); } }
        public DateTime? DateCreated  { get { return dateCreated; } set { SetProperty(ref dateCreated, value); } }
        public string DocumentsPath { get { return documentsPath; } set { SetProperty(ref documentsPath, value); } }
    }
}
