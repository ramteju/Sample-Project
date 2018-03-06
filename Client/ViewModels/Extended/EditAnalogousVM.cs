using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels
{
    public class EditAnalogousVM : ViewModelBase
    {
        public EditAnalogousVM()
        {
            AnalogousReactionPreview = new ReactionViewVM();
        }
        private ReactionViewVM reactionPreview;
        private bool creatingAnalogous;
        private string statusText;

        public ReactionViewVM AnalogousReactionPreview { get { return reactionPreview; } set { SetProperty(ref reactionPreview, value); } }
        public bool CreatingAnalogous { get { return creatingAnalogous; } set { SetProperty(ref creatingAnalogous, value); } }
        public string StatusText { get { return statusText; } set { SetProperty(ref statusText, value); } }
    }
}
