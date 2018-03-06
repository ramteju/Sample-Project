using Entities;
using ProductTracking.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace ProductTracking.Controllers.Core
{
    public class NamePrioritiesController : Controller
    {
        // GET: NamePriorities
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult NamePriorities()
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var list = new ArrayList();
                var nameprioritiess = db.NamePriorities;
                foreach (var result in nameprioritiess)
                {
                    list.Add(new
                    {
                        id = result.Id,
                        Name = result.Name,
                        RegNumber = result.RegNumber,
                        ChemicalType = result.ChemicalType.ToString()
                    });
                }
                return Json(list, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult NamePrioritiesCRUD(NamePrioritiesViewModel vm)
        {
            if (vm.oper == "add" && ModelState.IsValid)
            {
                using (TransactionScope scope = new TransactionScope())
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    NamePriorities obj = new NamePriorities();
                    obj.Name = vm.Name;
                    obj.RegNumber = vm.RegNumber;
                    obj.ChemicalType = vm.ChemicalType;
                    db.NamePriorities.Add(obj);
                    db.SaveChanges();
                    scope.Complete();
                    return Content("Created");
                }
            }
            if (vm.oper == "edit" && ModelState.IsValid)
            {
                string id = HttpContext.Request.Params.Get("Id");
                if (!string.IsNullOrEmpty(id))
                {
                    using (TransactionScope scope = new TransactionScope())
                    using (ApplicationDbContext db = new ApplicationDbContext())
                    {
                        NamePriorities obj = db.NamePriorities.Find(Int32.Parse(id));
                        obj.Name = vm.Name;
                        obj.RegNumber = vm.RegNumber;
                        obj.ChemicalType = vm.ChemicalType;
                        db.SaveChanges();
                        scope.Complete();
                        return Content("Updated");
                    }
                }
            }
            if (vm.oper == "del")
            {
                using (TransactionScope scope = new TransactionScope())
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    NamePriorities obj = db.NamePriorities.Find(Int32.Parse(HttpContext.Request.Params.Get("Id")));
                    db.NamePriorities.Remove(obj);
                    db.SaveChanges();
                    scope.Complete();
                    return Content("Deleted");
                }
            }
            return Content("Invalid request");
        }
    }
}