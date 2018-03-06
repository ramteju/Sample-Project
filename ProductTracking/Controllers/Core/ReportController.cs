using Entities;
using Microsoft.Practices.Unity;
using ProductTracking.Filter;
using ProductTracking.Models;
using ProductTracking.Models.Core;
using ProductTracking.Models.Core.ViewModels;
using ProductTracking.Services.Core;
using ProductTracking.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace ProductTracking.Controllers.Core
{
    [AuthorizeRoles(Role.ToolManager, Role.ProjectManger)]
    public class ReportController : Controller
    {
        [Dependency("Tanservice")]
        public TanService TanService { get; set; }
        [Dependency("ShipmentService")]
        public ShipmentService ShipmentService { get; set; }
        [Dependency("ShipmentReport")]
        public ShipmentReport ShipmentReport { get; set; }
        [Dependency("UserService")]
        public UserService UserService { get; set; }
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: Report
        public ActionResult Index()
        {
            ArrayList list = new ArrayList();
            var chemicalTypes = Enum.GetValues(typeof(TanState))
                                      .Cast<TanState>()
                                      .Where(v => v != TanState.Not_Assigned && v != TanState.QC_Rejected && v != TanState.Review_Rejected)
                                      .Select(v => v).OrderByDescending(v => v)
                                      .ToList();
            foreach (var state in chemicalTypes)
            {
                list.Add(new
                {
                    Id = state,
                    State = state.ToString()
                });
            }
            ViewBag.TanStates = list;
            return View();
        }

        public ActionResult UserWiseSummary1(string FromDate, string ToDate, string tanStates, bool fromUpdatedDate, bool fromCompletedDate)
        {
            List<UserWiseReports> userwiseReports = new List<UserWiseReports>();
            //var dateStrings = dates.Split(',');
            var dateList = new List<DateTime>();
            DateTime dtFromDate = DateTime.ParseExact(FromDate.Trim(), "MM/dd/yyyy", CultureInfo.InvariantCulture);
            DateTime dtToDate = DateTime.ParseExact(ToDate.Trim(), "MM/dd/yyyy", CultureInfo.InvariantCulture);
            var range = dtFromDate.DateRange(dtToDate);
            dateList = range.ToList();
            List<string> TanStates = new List<string>();
            if (!string.IsNullOrEmpty(tanStates))
                TanStates = tanStates.Split(',').ToList();
            //foreach (var d in dateStrings)
            //    dateList.Add(DateTime.ParseExact(d.Trim(), "dd-MM-yyyy", CultureInfo.InvariantCulture));
            var users = db.Users.Include(user => user.ApplicationRoles);
            var CuratorsData = TanService.UserWiseTanNumbers(Role.Curator, TanStates);
            var ReviewersData = TanService.UserWiseTanNumbers(Role.Reviewer, TanStates);
            var QCsData = TanService.UserWiseTanNumbers(Role.QC, TanStates);
            foreach (var user in users)
            {
                var curationdata = CuratorsData.ContainsKey(user.Id) ? TanService.ReactionsCountDateWisewithStages(TanService.GetTanNumbers(1, user.Id, true), 1, fromUpdatedDate, fromCompletedDate) : new Dictionary<DateTime, ReactionsCount>();
                var Reviewdata = ReviewersData.ContainsKey(user.Id) ? TanService.ReactionsCountDateWisewithStages(TanService.GetTanNumbers(2, user.Id, true), 2, fromUpdatedDate, fromCompletedDate) : new Dictionary<DateTime, ReactionsCount>();
                var Qcdata = QCsData.ContainsKey(user.Id) ? TanService.ReactionsCountDateWisewithStages(TanService.GetTanNumbers(3, user.Id, true), 3, fromUpdatedDate, fromCompletedDate) : new Dictionary<DateTime, ReactionsCount>();
                UserWiseReports uwr = new UserWiseReports();
                foreach (var date in dateList)
                {
                    if (curationdata != null && curationdata.ContainsKey(date))
                    {
                        uwr.UserName = user.UserName;
                        uwr.CuratedReactionsCount += curationdata[date].TotalReactionsCount;
                        uwr.CuratedAnalogousReactionsCount += curationdata[date].AnalogousReactionsCount;
                        uwr.CuratedStagesCount = curationdata[date].StagesCount;
                    }
                    if (Reviewdata != null && Reviewdata.ContainsKey(date))
                    {
                        uwr.UserName = user.UserName;
                        uwr.ReviewedReactionsCount += Reviewdata[date].TotalReactionsCount;
                        uwr.ReviewedAnalogousReactionsCount += Reviewdata[date].AnalogousReactionsCount;
                        uwr.ReviewedStagesCount += Reviewdata[date].StagesCount;
                    }
                    if (Qcdata != null && Qcdata.ContainsKey(date))
                    {
                        uwr.UserName = user.UserName;
                        uwr.QcReactionsCount += Qcdata[date].TotalReactionsCount;
                        uwr.QcAnalogousReactionsCount += Qcdata[date].AnalogousReactionsCount;
                        uwr.QcStagesCount += Qcdata[date].StagesCount;
                    }
                }
                if (!string.IsNullOrEmpty(uwr.UserName))
                    userwiseReports.Add(uwr);
            }
            ViewBag.UserWiseReports = userwiseReports.OrderByDescending(ur => ur.CuratedReactionsCount).ToList();
            return PartialView("~/Views/Report/_userWiseReports.cshtml");
        }

        public ActionResult UserWiseSummary(string FromDate, string ToDate, string tanStates, bool fromUpdatedDate, bool fromCompletedDate)
        {
            List<UserWiseReports> userwiseReports = new List<UserWiseReports>();
            //var dateStrings = dates.Split(',');
            var dateList = new List<DateTime>();
            DateTime dtFromDate = DateTime.ParseExact(FromDate.Trim(), "MM/dd/yyyy", CultureInfo.InvariantCulture);
            DateTime dtToDate = DateTime.ParseExact(ToDate.Trim(), "MM/dd/yyyy", CultureInfo.InvariantCulture);
            var range = dtFromDate.DateRange(dtToDate);
            dateList = range.ToList();
            List<string> TanStates = new List<string>();
            if (!string.IsNullOrEmpty(tanStates))
                TanStates = tanStates.Split(',').ToList();
            var Report = db.DateWiseRXNCount.Include(t => t.Tan)
                                            .Where(t => tanStates.ToUpper().Contains("ALL") ? true : TanStates.Contains(t.Tan.TanState.ToString()))
                                            .GroupBy(t => DbFunctions.TruncateTime(t.UpdatedDate))
                                            .ToDictionary(t => t.Key, t => t.GroupBy(k => k.UserId).ToDictionary(k => k.Key, k => k.ToList()));
            //foreach (var d in dateStrings)
            //    dateList.Add(DateTime.ParseExact(d.Trim(), "dd-MM-yyyy", CultureInfo.InvariantCulture));
            var users = db.Users.Include(user => user.ApplicationRoles);
            foreach (var user in users)
            {
                UserWiseReports uwr = new UserWiseReports();
                foreach (var date in dateList)
                    if (Report.ContainsKey(date) && Report[date].ContainsKey(user.Id))
                    {
                        uwr.UserName = user.UserName;
                        uwr.CuratedReactionsCount += Report[date][user.Id].Where(r => r.Role == Role.Curator).Sum(r => r.RxnCount);
                        uwr.ReviewedReactionsCount += Report[date][user.Id].Where(r => r.Role == Role.Reviewer).Sum(r => r.RxnCount);
                    }
                if (!string.IsNullOrEmpty(uwr.UserName))
                    userwiseReports.Add(uwr);
            }
            ViewBag.UserWiseReports = userwiseReports.OrderByDescending(ur => ur.CuratedReactionsCount).ToList();
            return PartialView("~/Views/Report/_userWiseReports.cshtml");
        }

        public ActionResult ShipmentWiseReport()
        {
            ArrayList list = new ArrayList();
            var data = ShipmentService.Batches();
            foreach (var batch in data)
            {
                list.Add(new
                {
                    Id = batch.Id,
                    BatchName = batch.Name
                });
            }
            ViewBag.Batches = list;
            return View();
        }

        public ActionResult ShipmentAbstractReport()
        {
            ArrayList list = new ArrayList();
            var data = ShipmentService.Batches();
            for (int i = 2017; i < 2025; i++)
            {
                list.Add(new
                {
                    Id = i,
                    Year = i
                });
            }
            ViewBag.Years = list;
            return View();
        }

        public ActionResult ShipmenWisetReport(string BatchIds)
        {
            if (!string.IsNullOrEmpty(BatchIds))
            {
                List<int> Batchids = BatchIds.Split(',').Select(k => int.Parse(k)).ToList();
                Dictionary<string, ShipmentWiseReportVM> DeliveredTansData = new Dictionary<string, ShipmentWiseReportVM>();
                ViewBag.GroupedData = ShipmentReport.BatchReport(Batchids, ref DeliveredTansData);
                ViewBag.DeliveredData = DeliveredTansData;
                var chemicalTypes = Enum.GetValues(typeof(TanState))
                                      .Cast<TanState>()
                                      .Where(v => v != TanState.QC_Rejected && v != TanState.Review_Rejected)
                                      .Select(v => v)
                                      .ToList();
                ViewBag.TanStates = chemicalTypes;
                ViewBag.ReadyToDeliever = new List<TanState> { TanState.QC_Accepted, TanState.QC_Assigned, TanState.QC_InProgress, TanState.QC_ReAssigned, TanState.Review_Accepted };
            }
            return PartialView("~/Views/Report/_shipmentWiseReport.cshtml");
        }
        public ActionResult AbstractReport(int Year)
        {
            List<DeliveryReportAbstract> DeliveryReportAbstracts = new List<DeliveryReportAbstract>();
            var batches = db.Batches.Where(b => b.DateCreated.Value.Year == Year).ToList();
            var GroupedTans = db.Tans.GroupBy(t => t.BatchId).ToDictionary(t => t.Key, t => t.ToList());
            //var DeliveredTans = db.DeliveryBatches.Include(t=>t.Tans).Where(d=>d.Delivered)
            foreach (var batch in batches)
            {
                var data = db.ShippmentUploadedExcels.Where(s => s.Batches.Select(b => b.Id).Contains(batch.Id)).FirstOrDefault();

                DeliveryReportAbstract DeliveryReportAbstract = new DeliveryReportAbstract();
                DeliveryReportAbstract.BatchReceiveddate = data.RecievedDate.Value;
                DeliveryReportAbstract.SpreadSheetName = data.SpreadSheetName;
                DeliveryReportAbstract.batRange = data.BatchNumber;
                DeliveryReportAbstract.TANCount = GroupedTans[batch.Id].Count();
                DeliveryReportAbstract.NumsCount = GroupedTans[batch.Id].Sum(t => t.NumsCount);
                DeliveryReportAbstract.RXNCount = GroupedTans[batch.Id].Sum(t => t.RxnCount);
                DeliveryReportAbstract.ZeroRxnCount = GroupedTans[batch.Id].Where(t => t.RxnCount == 0).Count();
                DeliveryReportAbstract.NumsDensity = DeliveryReportAbstract.NumsCount / DeliveryReportAbstract.TANCount;
                DeliveryReportAbstract.RXNDensity = DeliveryReportAbstract.RXNCount / DeliveryReportAbstract.TANCount;
                DeliveryReportAbstract.JournelTanCount = GroupedTans[batch.Id].Where(t => t.TanType == "ENJ_FF_PW").Count();
                DeliveryReportAbstract.JournelRxnCount = GroupedTans[batch.Id].Where(t => t.TanType == "ENJ_FF_PW").Sum(t => t.RxnCount);
                DeliveryReportAbstract.JournelZeroRxnTanCount = GroupedTans[batch.Id].Where(t => t.TanType == "ENJ_FF_PW" && t.RxnCount == 0).Count();
                DeliveryReportAbstract.PatentTanCount = GroupedTans[batch.Id].Where(t => t.TanType == "ENP_FF_PW").Count();
                DeliveryReportAbstract.PatentRxnCount = GroupedTans[batch.Id].Where(t => t.TanType == "ENP_FF_PW").Sum(t => t.RxnCount);
                DeliveryReportAbstract.PatentZeroRxnTanCount = GroupedTans[batch.Id].Where(t => t.TanType == "ENP_FF_PW" && t.RxnCount == 0).Count();
                DeliveryReportAbstract.DestroyDate = DeliveryReportAbstract.BatchReceiveddate.AddDays(180);
                DeliveryReportAbstracts.Add(DeliveryReportAbstract);
            }
            return Content("InProgress");
        }

        public ActionResult GetTodayHourWiseCount()
        {
            try
            {
                var pastFewDaysCount = UserService.GetTodayHourWiseCount();
                return this.Json(pastFewDaysCount, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //Log.Error(ex);
                return View("Error");
            }
        }
    }
}