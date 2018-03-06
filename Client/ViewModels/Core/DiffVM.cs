using Client.Command;
using Client.Common;
using Client.Logging;
using Client.ViewModels.Core;
using Client.Views;
using Entities;
using Entities.DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client.ViewModels
{
    public class DiffVM : ViewModelBase
    {
        private string tanNumber;
        private bool loading;
        private TanHistoryVM fromVersion, toVersion;
        private ObservableCollection<TanHistoryVM> fromHistory, toHistory;
        private Visibility dataLoaded;
        public string TanNumber { get { return tanNumber; } set { SetProperty(ref tanNumber, value); } }
        public bool Loading { get { return loading; } set { SetProperty(ref loading, value); } }
        public TanHistoryVM FromVersion { get { return fromVersion; } set { SetProperty(ref fromVersion, value); } }
        public TanHistoryVM ToVersion { get { return toVersion; } set { SetProperty(ref toVersion, value); } }
        public Visibility DataLoaded { get { return dataLoaded; } set { SetProperty(ref dataLoaded, value); } }
        public ObservableCollection<TanHistoryVM> FromHistory { get { return fromHistory; } set { SetProperty(ref fromHistory, value); } }
        public ObservableCollection<TanHistoryVM> ToHistory { get { return toHistory; } set { SetProperty(ref toHistory, value); } }
        public DelegateCommand GetVersions { get; private set; }
        public DelegateCommand Diff { get; private set; }

        public DiffVM()
        {
            GetVersions = new DelegateCommand(DoGetVersions);
            Diff = new DelegateCommand(DoDiff);
            DataLoaded = Visibility.Hidden;
        }

        private void DoDiff(object obj)
        {
            
            try
            {
                 ;
                if (FromVersion != null && ToVersion != null)
                {
                    DataLoaded = Visibility.Visible;
                     ;
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
              
            }
        }

        async private void DoGetVersions(object obj)
        {
            
            try
            {
                FromHistory = new ObservableCollection<TanHistoryVM>();
                ToHistory = new ObservableCollection<TanHistoryVM>();
                if (!String.IsNullOrEmpty(TanNumber))
                {
                    Loading = true;
                    var result = await RestHub.Versions(TanNumber);
                    if (result.HttpCode == System.Net.HttpStatusCode.OK)
                    {
                        List<TanHistoryDTO> dtos = (List<TanHistoryDTO>)result.UserObject;
                        foreach (var dto in dtos)
                        {
                            FromHistory.Add(new Core.TanHistoryVM
                            {
                                Id = dto.Id,
                                Text = dto.Text
                            });
                            ToHistory.Add(new Core.TanHistoryVM
                            {
                                Id = dto.Id,
                                Text = dto.Text
                            });
                        }
                    }
                    else
                        AppInfoBox.ShowInfoMessage("Error While Versions . ." + Environment.NewLine + result.HttpResponse);
                    Loading = false;
                }
                else
                   AppInfoBox.ShowInfoMessage("Please Enter TAN Number . .");
            }
            catch (Exception ex)
            {
                Log.This(ex);
              
            }
        }

        internal void ClearState()
        {
            FromHistory = null;
            ToHistory = null;
            FromVersion = null;
            ToVersion = null;
            TanNumber = String.Empty;
            DataLoaded = Visibility.Hidden;
        }
    }
}
