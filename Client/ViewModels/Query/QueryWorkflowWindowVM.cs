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
using Client.ViewModel;
namespace Client.ViewModels.Query
{
    public class QueryWorkflowWindowVM : ViewModelBase
    {
        private ObservableCollection<QueryWorkflowVM> workflows;
        private ObservableCollection<string> usernames;
        private bool loading;
        public bool Loading { get { return loading; } set { SetProperty(ref loading, value); } }
        public ObservableCollection<QueryWorkflowVM> Workflows { get { return workflows; } set { SetProperty(ref workflows, value); } }
        public ObservableCollection<string> Usernames { get { return usernames; } set { SetProperty(ref usernames, value); } }
        public DelegateCommand Refresh { get; private set; }
        public DelegateCommand LoadUsers { get; private set; }
        public DelegateCommand SaveUsers { get; private set; }

        public QueryWorkflowWindowVM()
        {
            Refresh = new DelegateCommand(DoRefresh);
            LoadUsers = new DelegateCommand(DoLoadUsers);
            SaveUsers = new DelegateCommand(DoSaveUsers);
        }

        private async void DoSaveUsers(object obj)
        {
            Loading = true;
            if (Workflows != null)
            {
                if (Workflows.Where(w => w.IsValid == YesNo.No.ToString()).Any())
                {
                    Loading = false;
                    MessageBox.Show("You have some query workflows which are not valid. Please verify . .");
                    return;
                }
                List<QueryWorkflowUserDTO> dtos = new List<QueryWorkflowUserDTO>();
                foreach (var workflow in Workflows)
                {
                    QueryWorkflowUserDTO dto = new QueryWorkflowUserDTO();
                    dto.L1User = workflow.L1user;
                    dto.L2User = workflow.L2user;
                    dto.L3User = workflow.L3user;
                    dto.L4User = workflow.L4user;
                    dto.L5User = workflow.L5user;
                    dtos.Add(dto);
                }
                var result = await RestHub.SaveQueryWorkflows(dtos);
                if (result.StatusMessage != null)
                {
                    MessageBox.Show(result.StatusMessage);
                    Refresh.Execute(this);
                }
                else
                    MessageBox.Show("Can't Save Workflows . .");
            }
            Loading = false;
        }

        private async void DoRefresh(object obj)
        {
            Loading = true;
            Workflows = new ObservableCollection<QueryWorkflowVM>();
            Workflows.CollectionChanged += Workflows_CollectionChanged;
            try
            {
                var result = await RestHub.QueryWorkflows();
                if (result.UserObject != null)
                {
                    List<QueryWorkflowUserDTO> dto = result.UserObject as List<QueryWorkflowUserDTO>;
                    if (dto != null)
                    {
                        foreach (var username in Usernames)
                        {
                            var workflow = dto.Where(w => w.L1User == username).FirstOrDefault();
                            Workflows.Add(new QueryWorkflowVM
                            {
                                L1user = username,
                                L2user = workflow?.L2User,
                                L3user = workflow?.L3User,
                                L4user = workflow?.L4User,
                                L5user = workflow?.L5User,
                            });
                        }
                        Workflows.UpdateDisplayOrder();
                    }
                }
                else
                    MessageBox.Show("Can't Load Users . .");
            }
            catch (Exception ex)
            {
                AppErrorBox.ShowErrorMessage("Error While Loading Users . .", ex.ToString());
            }
            Loading = false;
        }

        private void Workflows_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Workflows.UpdateDisplayOrder();
        }

        private async void DoLoadUsers(object obj)
        {
            Loading = true;
            try
            {
                var result = await RestHub.Users();
                if (result.UserObject != null)
                {
                    List<string> dto = result.UserObject as List<string>;
                    if (dto != null)
                        Usernames = new ObservableCollection<string>(dto);
                }
                else
                    MessageBox.Show("Can't Load Users . .");
            }
            catch (Exception ex)
            {
                AppErrorBox.ShowErrorMessage("Error While Loading Users . .", ex.ToString());
            }
            Loading = false;
        }

        public void ClearState()
        {
            Workflows = new ObservableCollection<QueryWorkflowVM>();
            Usernames = new ObservableCollection<string>();
            LoadUsers.Execute(this);
        }
    }
}
