using Entities.DTO;
using Microsoft.Practices.Unity;
using ProductTracking.Services.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ProductTracking.Controllers.API
{
    public class ReportController : ApiController
    {
        [Dependency("TanService")]
        public TanService tanservice { get; set; }
        [Dependency("ClaimService")]
        public ClaimService claimServices { get; set; }

        [HttpPost]
        public object GetErrorPercentage(ErrorPercentageDto errPercdto)
        {
            return tanservice.GetErrorPercReport(errPercdto);
        }
    }
}
