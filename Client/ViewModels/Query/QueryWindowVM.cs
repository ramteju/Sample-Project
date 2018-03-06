using Client.Command;
using System.Collections.ObjectModel;
using System;
using System.Windows;
using Entities.DTO;
using System.Collections.Generic;
using Excelra.Utils.Library;
using Entities;
using System.Linq;
using Client.Views;
using Client.Common;

namespace Client.ViewModels.Query
{
    public class QueryWindowVM : ViewModelBase
    {
        private ObservableCollection<QueryVM> queries;
        private QueryVM selectedQuery, formQuery;
        private bool loading;
        private string queryWorkflow;
        private bool allowSubmit, allowReject;
        private string response;
        private ObservableCollection<QueryResponseVM> responses;

        public bool Loading { get { return loading; } set { SetProperty(ref loading, value); } }
        public bool AllowSubmit { get { return allowSubmit; } set { SetProperty(ref allowSubmit, value); } }
        public bool AllowReject { get { return allowReject; } set { SetProperty(ref allowReject, value); } }
        public string Response { get { return response; } set { SetProperty(ref response, value); } }
        public string QueryWorkflow { get { return queryWorkflow; } set { SetProperty(ref queryWorkflow, value); } }
        public QueryVM SelectedQuery
        {
            get { return selectedQuery; }
            set
            {
                SetProperty(ref selectedQuery, value);
                if (value != null)
                {
                    FormQuery = new QueryVM
                    {
                        Comment = value.Comment,
                        Id = value.Id,
                        Page = value.Page,
                        QueryType = value.QueryType,
                        TanNumber = value.TanNumber,
                        TanId = value.TanId,
                        Title = value.Title
                    };
                }
                LoadWorkflow.Execute(this);
                LoadResponses.Execute(this);
            }
        }
        public ObservableCollection<QueryResponseVM> Responses { get { return responses; } set { SetProperty(ref responses, value); } }
        public QueryVM FormQuery { get { return formQuery; } set { SetProperty(ref formQuery, value); } }
        public ObservableCollection<QueryVM> Queries { get { return queries; } set { SetProperty(ref queries, value); } }
        public DelegateCommand RefreshQueries { get; private set; }
        public DelegateCommand SaveQuery { get; private set; }
        public DelegateCommand ClearQuery { get; private set; }
        public DelegateCommand LoadResponses { get; private set; }
        public DelegateCommand LoadWorkflow { get; private set; }
        public DelegateCommand Submit { get; private set; }
        public DelegateCommand Revert { get; private set; }
        public DelegateCommand GetCurrentTan { get; private set; }

        public QueryWindowVM()
        {
            RefreshQueries = new DelegateCommand(DoRefreshQueriesAsync);
            SaveQuery = new DelegateCommand(DoSaveQueryAsync);
            ClearQuery = new DelegateCommand(DoClearQuery);
            LoadResponses = new DelegateCommand(DoLoadResponsesAsync);
            LoadWorkflow = new DelegateCommand(DoLoadWorkflow);
            Submit = new DelegateCommand(DoSubmitAsync);
            Revert = new DelegateCommand(DoRevertAsync);
            GetCurrentTan = new DelegateCommand(DOGetCUrrentTan);
            FormQuery = new QueryVM();
            Responses = new ObservableCollection<QueryResponseVM>();
            ClearState();
        }

        private void DOGetCUrrentTan(object obj)
        {
            var mainVM = (App.Current.MainWindow as MainWindow).DataContext as MainVM;
            if (mainVM != null && mainVM.TanVM != null)
            {
                FormQuery.TanId = mainVM.TanVM.Id;
                FormQuery.TanNumber = mainVM.TanVM.TanNumber;
            }
        }

        private async void DoRevertAsync(object obj)
        {
            if (FormQuery != null && FormQuery.Id > 0)
            {
                if(String.IsNullOrEmpty(Response))
                {
                    MessageBox.Show("Please Add Your Message . .");
                    return;
                }
                try
                {
                    var result = await RestHub.Revert(FormQuery.Id,Response);
                    if (result.UserObject != null && (bool)result.UserObject)
                    {
                        ClearQuery.Execute(this);
                        MessageBox.Show(result.StatusMessage);
                        RefreshQueries.Execute(this);
                    }
                    else
                        MessageBox.Show("Can't Revert Query . .");
                }
                catch (Exception ex)
                {
                    AppErrorBox.ShowErrorMessage("Error while Reverting Query . .", ex.ToString());
                }
            }
            else
                MessageBox.Show("Please save query or select already created query to proceed . .");
        }

        private async void DoSubmitAsync(object obj)
        {
            if (FormQuery != null && FormQuery.Id > 0)
            {
                if (String.IsNullOrEmpty(Response))
                {
                    MessageBox.Show("Please Add Your Message . .");
                    return;
                }
                try
                {
                    var result = await RestHub.Submit(FormQuery.Id,Response);
                    if (result.UserObject != null && (bool)result.UserObject)
                    {
                        ClearQuery.Execute(this);
                        MessageBox.Show(result.StatusMessage);
                        RefreshQueries.Execute(this);
                    }
                    else
                        MessageBox.Show("Can't Submitting Query . .");
                }
                catch (Exception ex)
                {
                    AppErrorBox.ShowErrorMessage("Error while Submitting Query . .", ex.ToString());
                }
            }
            else
                MessageBox.Show("Please save query or select already created query to proceed . .");
        }

        public void ClearState()
        {
            Responses = new ObservableCollection<QueryResponseVM>();
            Queries = new ObservableCollection<QueryVM>();
            FormQuery = new QueryVM();
            AllowReject = AllowSubmit = false;
            QueryWorkflow = String.Empty;
        }
        private void DoClearQuery(object obj)
        {
            FormQuery = new QueryVM();
            Responses = new ObservableCollection<QueryResponseVM>();
            Response = String.Empty;
            AllowReject = AllowSubmit = false;
            QueryWorkflow = String.Empty;
        }

        private async void DoLoadWorkflow(object obj)
        {
            Responses = new ObservableCollection<QueryResponseVM>();
            if (FormQuery != null && FormQuery.Id > 0)
            {
                try
                {
                    var result = await RestHub.QueryWorkflow(FormQuery.Id);
                    if (result.UserObject != null)
                    {
                        QueryWorkflowDTO dto = result.UserObject as QueryWorkflowDTO;
                        if (dto != null)
                        {
                            AllowSubmit = dto.AllowSubmit;
                            AllowReject = dto.AllowReject;
                            string before = String.IsNullOrEmpty(dto.PreviousUser) ? String.Empty : dto.PreviousUser + " <img src='http://www.iconsdb.com/icons/download/icon-sets/web-2-blue/arrow-11-24.jpg'/>";
                            string after = String.IsNullOrEmpty(dto.NextUser) ? String.Empty : "<img src='http://www.iconsdb.com/icons/download/orange/arrow-11-24.jpg'/>" + dto.NextUser;
                            QueryWorkflow = $"{before} <b>{dto.CurrentUser}</b> {after}";
                        }
                    }
                    else
                        MessageBox.Show("Can't Load Responses . .");
                }
                catch (Exception ex)
                {
                    AppErrorBox.ShowErrorMessage("Error while Loading Responses . .", ex.ToString());
                }
            }
        }

        private async void DoLoadResponsesAsync(object obj)
        {
            Responses = new ObservableCollection<QueryResponseVM>();
            if (FormQuery != null && FormQuery.Id > 0)
            {
                try
                {
                    var result = await RestHub.Responses(FormQuery.Id);
                    if (result.UserObject != null)
                    {
                        foreach (var response in result.UserObject as List<QueryResponseDTO>)
                        {
                            Responses.Add(new QueryResponseVM
                            {
                                Id = response.Id,
                                Response = response.Response,
                                Timestamp = response.TimeStamp,
                                User = response.User
                            });
                        }
                    }
                    else
                        MessageBox.Show("Can't Load Responses . .");
                }
                catch (Exception ex)
                {
                    AppErrorBox.ShowErrorMessage("Error while Loading Responses . .", ex.ToString());
                }
            }
        }
        
        private async void DoSaveQueryAsync(object obj)
        {
            Loading = true;
            if (FormQuery != null && FormQuery.TanId > 0 && !String.IsNullOrEmpty(FormQuery.TanNumber) && !String.IsNullOrEmpty(FormQuery.Title) && !String.IsNullOrEmpty(FormQuery.Comment) && U.RoleId != 4)
            {
                try
                {
                    QueryDTO dto = new QueryDTO()
                    {
                        Comment = FormQuery.Comment,
                        Id = FormQuery.Id,
                        Page = FormQuery.Page,
                        QueryType = FormQuery.QueryType,
                        TanId = FormQuery.TanId,
                        Title = FormQuery.Title,
                    };
                    var result = await RestHub.SaveQuery(dto);
                    if (result.UserObject != null && (bool)result.UserObject)
                    {
                        MessageBox.Show(result.StatusMessage);
                        RefreshQueries.Execute(this);
                        ClearQuery.Execute(this);
                    }
                    else
                        MessageBox.Show("Can't Save Query . .");
                }
                catch (Exception ex)
                {
                    AppErrorBox.ShowErrorMessage("Error while saving Query . .", ex.ToString());
                }
            }
            else
                AppInfoBox.ShowInfoMessage("Query can't be created without all required information . .");
            Loading = false;
        }

        private async void DoRefreshQueriesAsync(object obj)
        {
            Loading = true;
            try
            {
                ClearQuery.Execute(this);
                Queries = new ObservableCollection<QueryVM>();
                var result = await RestHub.MyQueries();
                if (result.UserObject != null)
                {
                    MyQueriesDTO myQueriesDto = (MyQueriesDTO)result.UserObject;
                    List<QueryDTO> dtos = myQueriesDto.Queries;
                    foreach (var dto in dtos)
                    {
                        Queries.Add(new QueryVM
                        {
                            Id = dto.Id,
                            TanId = dto.TanId,
                            DocumentPath = dto.DocumentPath,
                            TanNumber = dto.TanNumber,
                            Comment = dto.Comment,
                            Page = dto.Page,
                            QueryType = dto.QueryType,
                            Title = dto.Title
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                AppErrorBox.ShowErrorMessage("Error while loading Queries", ex.ToString());
            }
            Loading = false;

        }
    }
}
