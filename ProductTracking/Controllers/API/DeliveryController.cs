using Entities;
using Microsoft.AspNet.Identity;
using Microsoft.Practices.Unity;
using ProductTracking.Models;
using ProductTracking.Services.Core;
using System.Web.Http;
using System.Linq;

namespace ProductTracking.Controllers.API
{
    [Authorize]
    public class DeliveryController : ApiController
    {
        [Dependency("DeliveryService")]
        public DeliveryService deliveryService { get; set; }
        public object GenerateXML(int id)
        {
            return deliveryService.GenerateXML(id);
        }
        public object GenerateZip(int id, long maxZipSize)
        {
            string userName = string.Empty;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                userName = User.Identity.GetUserName();
            }
            if (!string.IsNullOrEmpty(userName))
                return deliveryService.GenerateZip(id, userName, maxZipSize);
            return string.Empty;
        }
        public object GenerateEmail(int id)
        {
            return deliveryService.GenerateEmail(id);
        }
        [HttpGet]
        public object Batches()
        {
            return deliveryService.Batches();
        }
        public object BatchWiseTans()
        {
            return deliveryService.BatchWiseTans();
        }
        public object TansOfDelivery(int id, bool isZeroRxns, bool isQueried)
        {
            return deliveryService.TansOfDelivery(id, isZeroRxns, isQueried);
        }
        public object GenerateNextBatchNumber(int batchId)
        {
            return deliveryService.GenerateNextBatchNumber(batchId);
        }
        public object Revert(int id, Role role, string msg)
        {
            return deliveryService.Revert(id, role, msg);
        }
    }
}
