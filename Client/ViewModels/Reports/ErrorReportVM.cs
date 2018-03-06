using Client.ViewModel;
using Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels.Reports
{
    public class ErrorReportVM : ViewModelBase
    {
        public ErrorReportVM()
        {
            ErrorReportData = new ObservableCollection<Reports.ErrorReportData>();
            User1 = "Curator";
            User2 = "Reviewer";
        }
        private ObservableCollection<ErrorReportData> errorReportData;
        private string user1,user2,tanNumber;

        public ObservableCollection<ErrorReportData> ErrorReportData { get { return errorReportData; } set { SetProperty(ref errorReportData, value); } }
        public string User1 { get { return user1; } set { SetProperty(ref user1, value); } }
        public string User2 { get { return user2; } set { SetProperty(ref user2, value); } }
        public string TanNumber { get { return tanNumber; } set { SetProperty(ref tanNumber, value); } }
        public ErrorReport ErrorReport { get; set; }
    }

    public class ErrorReportData : ViewModelBase
    {
        private string dataType;
        private int commonCount;
        private int updatedCount;
        private int deletedCount;
        private int addedCount;
        private double percentage;

        public string DataType { get { return dataType; } set { SetProperty(ref dataType, value); } }
        public int AddedCount { get { return addedCount; } set { SetProperty(ref addedCount, value); } }
        public int DeletedCount { get { return deletedCount; } set { SetProperty(ref deletedCount, value); } }
        public int UpdatedCount { get { return updatedCount; } set { SetProperty(ref updatedCount, value); } }
        public int CommonCount { get { return commonCount; } set { SetProperty(ref commonCount, value); } }
        public double Percentage { get { return percentage; } set { SetProperty(ref percentage, value); } }
    }
}
