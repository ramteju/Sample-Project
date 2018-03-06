using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductTracking.Services.Core
{
    public class ServiceResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }

    }
}