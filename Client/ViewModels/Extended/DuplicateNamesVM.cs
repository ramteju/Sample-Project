using Client.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client.ViewModels
{
    public class DuplicateNamesVM : ViewModelBase
    {
        public DuplicateNamesVM()
        {
            //HideWindow = Visibility.Visible;
            AddName = new Command.DelegateCommand(this.DoSelectName);
            DuplicateNamesView = new ObservableCollection<Names>();
        }

        private void DoSelectName(object obj)
        {
            DialogStatus = true;
            HideWindow = Visibility.Hidden;
        }

        private ObservableCollection<Names> duplicateNamesView;
        private Names selectedName;
        private Visibility hideWindow;

        public ObservableCollection<Names> DuplicateNamesView
        {
            get { return duplicateNamesView; }
            set
            {
                SetProperty(ref duplicateNamesView, value);
            }
        }
        public Names SelectedName { get { return selectedName; }set { SetProperty(ref selectedName, value); } }
        public Visibility HideWindow { get { return hideWindow; } set { SetProperty(ref hideWindow, value); } }
        public bool DialogStatus;
        public DelegateCommand AddName { get; private set; }
    }
    public class Names
    {
        public string Name { get; set; }
    }
}
