using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels.Core
{
    public class TanDocumentsVM : ViewModelBase
    {
        private int id;
        private string path;
        private bool keyPath;
        private string fileName;
        private int tanId;
        private string localPath;

        public int Id { get { return id; } set { SetProperty(ref id, value); } }
        public string Path { get { return path; } set { SetProperty(ref path, value); } }
        public bool KeyPath { get { return keyPath; } set { SetProperty(ref keyPath, value); } }
        public String FileName { get { return fileName; } set { SetProperty(ref fileName, value); } }
        public int TanId { get { return tanId; } set { SetProperty(ref tanId, value); } }
        public String LocalPath { get { return localPath; } set { SetProperty(ref localPath, value); } }
    }
}
