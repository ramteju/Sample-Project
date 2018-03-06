using DTO;
using Entities;
using Newtonsoft.Json;
using ProductTracking.Models;
using ProductTracking.Models.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;

namespace ProductTracking.Controllers.Core
{
    public class DeliveryController : Controller
    {
        public ActionResult Index()
        {
            return Content(String.Empty);
        }
    }
}