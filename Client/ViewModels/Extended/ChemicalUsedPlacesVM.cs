using ProductTracking.Models.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels.Extended
{
    public class ChemicalUsedPlacesVM : ViewModelBase
    {
        public ChemicalUsedPlacesVM()
        {

        }
        private ReactionParticipantVM selectedparticipant;
        private ObservableCollection<ReactionParticipantVM> selectedParticipantsList;
        private NUMVM tanChemical;
        private int previewTabIndex;

        public ReactionParticipantVM Selectedparticipant { get { return selectedparticipant; } set { SetProperty(ref selectedparticipant, value); } }
        public ObservableCollection<ReactionParticipantVM> SelectedParticipantsList { get { return selectedParticipantsList; } set { SetProperty(ref selectedParticipantsList, value); } }
        public NUMVM TanChemical { get { return tanChemical; } set { SetProperty(ref tanChemical, value); } }
        public int PreviewTabIndex { get { return previewTabIndex; } set { SetProperty(ref previewTabIndex, value); } }
    }
}
