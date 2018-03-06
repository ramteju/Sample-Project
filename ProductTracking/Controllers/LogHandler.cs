using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace ProductTracking.Controllers 
{
    public class LogHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage, CancellationToken cancellationToken)
        {
            DateTime startTime = DateTime.Now;
            var result = await base.SendAsync(requestMessage, cancellationToken);
            var duration = DateTime.Now - startTime;
            return result;
        }
    }
}