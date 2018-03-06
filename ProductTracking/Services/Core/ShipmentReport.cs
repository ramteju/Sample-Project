using Entities;
using ProductTracking.Models;
using ProductTracking.Models.Core;
using ProductTracking.Models.Core.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace ProductTracking.Services.Core
{
    public class ShipmentReport
    {
        ApplicationDbContext db = new ApplicationDbContext();
        public object BatchReport(List<int> batchNos, ref Dictionary<string, ShipmentWiseReportVM> returnData)
        {
            var DeliveredBatches = db.DeliveryBatches.Include(t => t.Tans).Where(d => d.Delivered).Select(d => d.Tans.ToList()).ToList();
            List<Tan> DeliveredTans = new List<Tan>();
            foreach (var tans in DeliveredBatches)
                DeliveredTans.AddRange(tans);
            List<int> DeliveredTanIds = DeliveredTans.Select(d => d.Id).ToList();
            var allTans = db.Tans.Where(t => batchNos.Contains(t.BatchId) && t.TanState != null && !DeliveredTanIds.Contains(t.Id))
                                 .GroupBy(t => t.DocClass == null ? "Empty" : t.DocClass)
                                 .ToDictionary(t => t.Key, t => t.GroupBy(k => k.TanState)
                                 .ToDictionary(m => m.Key, m => new ShipmentWiseReportVM
                                 {
                                     TANsCount = m.Count(),
                                     RXNsCount = m.Select(r => r.RxnCount).Sum()
                                 }));
            //var DeliveredBatches = db.Tans.Include(t => t.DeliveryBatches).Where(d => batchNos.Contains(d.BatchId) && d.DeliveryBatches != null && d.DeliveryBatches.Where(b => b.Delivered).Any()).Select(t => t.DeliveryBatches.Select(d => d.Tans).ToList()).ToList();
            returnData = DeliveredTans.GroupBy(t => t.DocClass == null ? "Empty" : t.DocClass)
                                           .ToDictionary(m => m.Key, m => new ShipmentWiseReportVM
                                           {
                                               TANsCount = m.Count(),
                                               RXNsCount = m.Select(r => r.RxnCount).Sum()
                                           });
            return allTans;

        }
    }
}