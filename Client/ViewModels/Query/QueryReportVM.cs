using Client.Command;
using Client.Views;
using Entities.DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client.ViewModels.Query
{
    public class QueryReportVM : ViewModelBase
    {

        private DateTime from, to;
        private bool loading;
        private ObservableCollection<QueryReportEntryVM> userEntries;
        public bool Loading { get { return loading; } set { SetProperty(ref loading, value); } }
        public DateTime From { get { return from; } set { SetProperty(ref from, value); } }
        public DateTime To { get { return to; } set { SetProperty(ref to, value); } }
        public ObservableCollection<QueryReportEntryVM> UserEntries { get { return userEntries; } set { SetProperty(ref userEntries, value); } }
        public DelegateCommand Search { get; private set; }
        public QueryReportVM()
        {
            DateTime now = DateTime.Now;
            From = new DateTime(now.Year, now.Month, 1);
            To = From.AddMonths(1).AddDays(-1);
            Search = new DelegateCommand(DoSearch);
        }
        private async void DoSearch(object obj)
        {
            Loading = true;
            UserEntries = new ObservableCollection<QueryReportEntryVM>();
            if (From != null && To != null)
            {
                var result = await RestHub.QueryReport(new QueryReportRequestDTO { From = From, To = To });
                if (result.UserObject != null)
                {
                    var dtos = result.UserObject as List<QueryReportEntryDTO>;
                    if (dtos != null)
                    {
                        foreach (var dto in dtos)
                            UserEntries.Add(new QueryReportEntryVM { User = dto.User, Created = dto.Created, Responded = dto.Responded });
                    }
                }
                else
                    AppErrorBox.ShowErrorMessage("Error While Loading Report . .", result.StatusMessage);
            }
            else
                MessageBox.Show("Please select dates . .");
            Loading = false;
        }

        public void ClearState()
        {
            UserEntries = new ObservableCollection<QueryReportEntryVM>();
        }
    }
}
