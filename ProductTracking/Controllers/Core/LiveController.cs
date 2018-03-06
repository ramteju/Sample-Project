using Microsoft.Practices.Unity;
using ProductTracking.Hubs;
using ProductTracking.Services.Core;
using System;
using System.Linq;
using System.Web.Mvc;
using ProductTracking.Logging;
using Entities;
using ProductTracking.Filter;

namespace ProductTracking.Controllers.Core
{
    public class LiveController : Controller
    {
        // GET: Live
        [Dependency("tanService")]
        public TanService tanService { get; set; }
        [Dependency("userService")]
        public UserService userService { get; set; }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ShipmentsCount()
        {
            try
            {
                return Content(tanService.ShipmentCount().ToString());
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return View("Error");
            }
        }

        public ActionResult TansCountToday(int Role)
        {
            try
            {
                return Content(tanService.TansCuratedCount(Role).ToString());
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return View("Error");
            }
        }

        public ActionResult ToatlCuratedTansCount()
        {
            try
            {
                return Content(tanService.ToatlCuratedTansCount().ToString());
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return View("Error");
            }
        }

        public ActionResult TotalTans()
        {
            try
            {
                return Content(tanService.TotalTans().ToString());
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return View("Error");
            }
        }

        public ActionResult CurationProgress()
        {
            try
            {
                return Content(tanService.curationProgress().ToString());
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return View("Error");
            }
        }
        public ActionResult RoleWiseCount(int Role)
        {
            try
            {
                return Content(userService.RoleWiseCount(Role).ToString());
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return View("Error");
            }
        }
        public ActionResult Online()
        {
            try
            {
                return Content(LiveHub.UserConnectionIds.Count().ToString());
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return View("Error");
            }
        }
        public ActionResult totalReactions(bool today)
        {
            try
            {
                return Content(tanService.totalReactions(today).ToString("N0"));
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return View("Error");
            }
        }

        public ActionResult TotalCuratedTans()
        {
            try
            {
                return Content(tanService.TotalCuratedTans().ToString("N0"));
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return View("Error");
            }
        }

        public ActionResult TotalReviewedTans()
        {
            try
            {
                return Content(tanService.TotalReviewedTans().ToString("N0"));
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return View("Error");
            }
        }

        public ActionResult TotalQCedTans()
        {
            try
            {
                return Content(tanService.TotalQCedTans().ToString("N0"));
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return View("Error");
            }
        }//

        public ActionResult TansInprogress()
        {
            try
            {
                return Content(tanService.TansInprogress().ToString("N0"));
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return View("Error");
            }
        }//TansInprogress

        [HttpGet]
        public ActionResult StateWiseGraph(int id)
        {
            try
            {
                var pastFewDaysCount = tanService.PastFewDaysCount(id);
                return this.Json(pastFewDaysCount, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return View("Error");
            }
        }
    }
}