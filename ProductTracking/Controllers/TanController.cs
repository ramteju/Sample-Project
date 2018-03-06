using Entities;
using Newtonsoft.Json;
using ProductTracking.Logging;
using ProductTracking.Models;
using ProductTracking.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace ProductTracking.Controllers
{
    [Authorize]
    public class TanController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
          
            try
            {
              
                ViewBag.tans = (from t in db.Tans.Include("LastAccessedBy") select t).ToList();
                return View();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }
        public ActionResult DuplicateTans()
        {
           
            try
            {
               
                ViewBag.batches = db.Batches.OrderByDescending(b => b.DateCreated).ToList();
                return View();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }
        public ActionResult DuplicateTansList(int BatchId)
        {
           
            try
            {
               
                ViewBag.batchWiseTans = db.Tans.Where(x => x.BatchId == BatchId && x.IsDuplicate == "Y").ToList();
                return PartialView("~/Views/Tan/_DuplicateTansList.cshtml");
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }
        public ActionResult TanKeyword(int? id)
        {
           
            try
            {
                
                if (id != null)
                {
                    TanKeywords tanKeyword = db.TanKeywords.Find(id);
                    ViewBag.TanKeyword = tanKeyword;

                }
                return View();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        public ActionResult DeleteTanKeyword(int? id)
        {
           
            try
            {
               
                if (id != null)
                {
                    TanKeywords tankeyword = db.TanKeywords.Where(x => x.Id == id).FirstOrDefault();
                    db.TanKeywords.Remove(tankeyword);
                    db.SaveChanges();
                }
                return RedirectPermanent("~/Tan/TanKeyword");
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        public ActionResult LoadKeywordsList()
        {
           
            try
            {
                
                ViewBag.TanKeywords = db.TanKeywords.ToList();
                return PartialView("~/Views/Tan/_TanKeywordsList.cshtml");
            }
            catch (Exception ex)
            {
                  Log.Error(ex);
                throw;
            }
        }

        public ActionResult MaintainKeyword(int? id, string Keyword)
        {
           
            try
            {
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(30)))
                {
                  
                    TanKeywords tankewords = new TanKeywords();
                    if (id != null)
                        tankewords = db.TanKeywords.Find(id);

                    tankewords.Id = id == null ? 0 : id.Value;
                    tankewords.keyword = Keyword;
                    if (id == null)
                    {
                        db.TanKeywords.Add(tankewords);
                    }

                    db.SaveChanges();
                    scope.Complete();
                    return Content("Success");
                }
            }
            catch (Exception ex)
            {
                  Log.Error(ex);
                throw;
            }


        }
    }
}