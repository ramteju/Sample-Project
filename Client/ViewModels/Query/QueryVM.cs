using System;
using Entities;
using Excelra.Utils.Library;
using System.Collections.ObjectModel;
using Client.Command;
using Telerik.Windows.Controls;
using Client.Views;

namespace Client.ViewModels.Query
{
    public class QueryVM : ViewModelBase
    {
        public QueryVM()
        {
            QueryType = QueryType.Doubt;
            OpenTan = new Command.DelegateCommand(DoOpenTan);
        }

        private int id, tanId;
        private string tanNumber, title, comment, page, documentPath;
        private QueryType queryType;
        public Command.DelegateCommand OpenTan { get; private set; }

        public int Id { get { return id; } set { SetProperty(ref id, value); } }
        public int TanId { get { return tanId; } set { SetProperty(ref tanId, value); } }
        public string DocumentPath { get { return documentPath; } set { SetProperty(ref documentPath, value); } }
        public string TanNumber { get { return tanNumber; } set { SetProperty(ref tanNumber, value); } }
        public string Title { get { return title; } set { SetProperty(ref title, value); } }
        public string Comment { get { return comment; } set { SetProperty(ref comment, value); } }
        public string Page { get { return page; } set { SetProperty(ref page, value); } }
        public QueryType QueryType { get { return queryType; } set { SetProperty(ref queryType, value); } }

        private void DoOpenTan(object obj)
        {
            QueryVM queryVm = obj as QueryVM;
            if (queryVm != null && queryVm.tanId > 0)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    new PdfReaderForm().OpenDocument(queryVm.DocumentPath, false);
                });
            }
        }
    }
}