using Microsoft.AspNet.SignalR;
using ProductTracking.Hubs;
using ProductTracking.Models;
using System.Linq;
using System.Web.Mvc;

namespace ProductTracking.Controllers
{
    [System.Web.Mvc.Authorize]
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Notification()
        {
            ViewBag.NotificationTemplates = db.NotificationTemplates.ToList();
            return View();
        }
        [ValidateInput(false)]
        public ActionResult Notify(string msg)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<LiveHub>();
            context.Clients.All.notification(msg);
            return Content("Notification Sent");
        }
        public ActionResult GetNotificationText(int id)
        {
            string htmlcontent = (from data in db.NotificationTemplates where data.Id == id select data.NotifactionMessage).FirstOrDefault();
            return Content(htmlcontent);
        }

    }
}