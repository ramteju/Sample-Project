using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace R_CADViewXService
{
    public class RestStatus
    {
        public HttpStatusCode HttpCode { get; set; }
        public string HttpResponse { get; set; }
        public string StatusMessage { get; set; }
        public object UserObject { get; set; }
    }
}
