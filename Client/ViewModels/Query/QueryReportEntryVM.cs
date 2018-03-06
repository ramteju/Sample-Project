namespace Client.ViewModels.Query
{
    public class QueryReportEntryVM : ViewModelBase
    {
        private string user;
        private int created, responded;

        public string User { get { return user; } set { SetProperty(ref user, value); } }
        public int Created { get { return created; } set { SetProperty(ref created, value); } }
        public int Responded { get { return responded; } set { SetProperty(ref responded, value); } }
    }
}