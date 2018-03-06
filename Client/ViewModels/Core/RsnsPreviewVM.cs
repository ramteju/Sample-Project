namespace Client.ViewModels
{
    public class RsnsPreviewVM : ViewModelBase
    {
        private string reactionOrStageName;
        private string cvt;
        private string freeText;

        public string ReactionOrStageName { get { return reactionOrStageName; } set { SetProperty(ref reactionOrStageName, value); } }
        public string Cvt { get { return cvt; } set { SetProperty(ref cvt, value); } }
        public string FreeText { get { return freeText; } set { SetProperty(ref freeText, value); } }
    }
}
