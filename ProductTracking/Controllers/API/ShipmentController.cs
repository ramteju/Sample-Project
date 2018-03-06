using Entities;
using Entities.DTO;
using Microsoft.Practices.Unity;
using ProductTracking.Logging;
using ProductTracking.Services.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace ProductTracking.Controllers.API
{

    public class ShipmentController : ApiController
    {
        [Dependency("ClaimService")]
        public ClaimService claimServices { get; set; }
        [Dependency("ShipmentService")]
        public ShipmentService ShipmentService { get; set; }

        private string GetIP()
        {
            try
            {
                return System.Web.HttpContext.Current != null ?
                        System.Web.HttpContext.Current.Request.UserHostAddress :
                        String.Empty;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public object Shipments()
        {
            try
            {
                return ShipmentService.Batches();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }
        [HttpGet]
        public object TanKeyWords()
        {
            return ShipmentService.TanKeyWords();
        }
        [HttpGet]
        public object TansBetweenBatches(int fromBatchNumber, int toBatchNumber, int tanCategory)
        {
            return ShipmentService.TansBetweenBatches(fromBatchNumber, toBatchNumber, tanCategory);
        }
        [HttpPost]
        public object TanFromBatches([FromBody]List<int> batchNos, [FromUri]int tanCategory)
        {
            return ShipmentService.TansFromBatches(batchNos, tanCategory);
        }
        public object MoveToCategory(MoveTansDTO moveTansDto)
        {
            return ShipmentService.MoveTansToCategory(moveTansDto);
        }
        public object MoveToDelivery(MoveTansDTO moveTansDto)
        {
            return ShipmentService.MoveTansToDelivery(moveTansDto);
        }
        public object S8000NameLocations(int batchId, int tanCategory)
        {
            return ShipmentService.S8000NameLocations(batchId, tanCategory);
        }
        public object S8580Comments(int batchId, int tanCategory)
        {
            return ShipmentService.S8580Comments(batchId, tanCategory);
        }
        public object ExtractRSN(int batchId, int tanCategory)
        {
            return ShipmentService.ExtractRSN(batchId, tanCategory);
        }
        public object UpdateBulkFreeText(FreeTextBulkDto bulkDto)
        {
            var id = claimServices.UserId((ClaimsIdentity)User.Identity);
            return ShipmentService.UpdateBulkFreeText(bulkDto, id, GetIP());
        }
        public object UpdateReviewAllowTag(List<int> batchIds)
        {
            return ShipmentService.UpdateReviewAllowTag(batchIds);
        }
        [HttpPost]
        public string UpdateDeliveryStatus([FromUri]int batchId)
        {
            return ShipmentService.UpdateDeliveryStatsu(batchId);
        }

        public object GetShipmentUploadStatus()
        {
            return ShipmentService.GetShipmentUploadStatus();
        }
        public object UpdateShipmentUploadStatus(ShippmentUploadStatus ShippmentUploadStatus)
        {
            return ShipmentService.UpdateShipmentUploadStatus(ShippmentUploadStatus);
        }
    }
}
