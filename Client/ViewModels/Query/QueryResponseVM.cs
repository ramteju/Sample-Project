using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels.Query
{
    public class QueryResponseVM : ViewModelBase
    {
        private int id;
        private string response, user;
        private DateTime timestamp;

        public int Id { get { return id; } set { SetProperty(ref id, value); } }
        public string Response { get { return response; } set { SetProperty(ref response, value); } }
        public string User { get { return user; } set { SetProperty(ref user, value); } }
        public DateTime Timestamp { get { return timestamp; } set { SetProperty(ref timestamp, value); } }
    }
}
