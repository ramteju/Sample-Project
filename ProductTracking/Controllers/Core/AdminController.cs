using Entities;
using Microsoft.AspNet.SignalR;
using Microsoft.Practices.Unity;
using ProductTracking.Hubs;
using ProductTracking.Models;
using ProductTracking.Models.Core;
using ProductTracking.Services.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;
using ProductTracking.Logging;
using System.Collections;
using ProductTracking.Models.Core.ViewModels;
using System.Data.Entity;
using Excelra.Utils.Library;
using DTO;
using ProductTracking.Filter;

namespace ProductTracking.Controllers.Core
{
    [AuthorizeRoles(Role.ToolManager,Role.ProjectManger)]
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [Dependency("UserService")]
        public UserService userService { get; set; }

        public ActionResult Index()
        {
            try
            {
                ViewBag.userCount = db.Users.Count();
                return View();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return View("Error");
            }
        }

        public ActionResult CVTsCrud()
        {
            return View();
        }

        public ActionResult DashBoard()
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<LiveHub>();
            try
            {
                ViewBag.TotalTansCount = db.Tans.Count();
                ViewBag.CurationCompletedTansCount = (from tan in db.Tans where tan.TanState == TanState.Curation_Submitted select tan).ToList().Count();
                context.Clients.All.progress(db.Tans.Count());
                return View();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return View("Error");
            }
        }
        public ActionResult UserRoles()
        {
            try
            {
                var users = db.Users.OrderBy(u => u.UserName).ToList();
                ViewBag.users = users;
                ViewBag.userWiseRoles = userService.UserWiseRoles();
                return View();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return View("Error");
            }
        }

        public ActionResult UserBenchMarks()
        {
            try
            {
                var users = db.Users.OrderBy(u => u.UserName).ToList();
                ViewBag.users = users;
                ViewBag.userWiseRoles = userService.UserWiseRoles();
                return View();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return View("Error");
            }
        }

        public ActionResult SaveUserBenchMarks()
        {
            try
            {
                var usersById = (from u in db.Users group u by u.Id into byIds select new { id = byIds.Key, user = byIds.ToList() }).ToDictionary(d => d.id, d => d.user.First());
                List<int> analystIds = new List<int>();
                using (TransactionScope scope = new TransactionScope())
                {
                    foreach (var userId in usersById.Select(u => u.Key))
                    {
                        var userAnalystId = HttpContext.Request.Params.Get(userId + "_AnalystId");
                        int analystId;
                        if (!string.IsNullOrEmpty(userAnalystId) && int.TryParse(userAnalystId, out analystId))
                            analystIds.Add(analystId);
                    }
                    if (analystIds.GroupBy(id => id).Count() == analystIds.Count())
                    {
                        foreach (var userId in usersById.Select(u => u.Key))
                        {
                            ApplicationUser user = usersById[userId];
                            var userBenchark = HttpContext.Request.Params.Get(userId + "_BenchMark");
                            float benchMark;
                            if (!string.IsNullOrEmpty(userBenchark) && float.TryParse(userBenchark, out benchMark))
                                user.BenchMark = benchMark;
                            var userAnalystId = HttpContext.Request.Params.Get(userId + "_AnalystId");
                            int analystId;
                            if (!string.IsNullOrEmpty(userAnalystId) && int.TryParse(userAnalystId, out analystId))
                                user.AnalystId = analystId;
                            var AllowUserForCuration = HttpContext.Request.Params.Get(userId + "_AllowCuration");
                            if (!string.IsNullOrEmpty(AllowUserForCuration))
                                user.AllowedForCuration = true;
                            else
                                user.AllowedForCuration = false;
                            var AllowUserForReview = HttpContext.Request.Params.Get(userId + "_AllowReview");
                            if (!string.IsNullOrEmpty(AllowUserForReview))
                                user.AllowedForReview = true;
                            else
                                user.AllowedForReview = false;
                            var AllowUserForQC = HttpContext.Request.Params.Get(userId + "_AllowQC");
                            if (!string.IsNullOrEmpty(AllowUserForQC))
                                user.AllowedForQC = true;
                            else
                                user.AllowedForQC = false;
                        }
                        db.SaveChanges();
                        scope.Complete();
                    }
                    else
                        return Content("Analyst Ids Must be Unique");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return View("Error");
            }
            var AllUserRoles = db.UserRoles.ToList();
            return RedirectToAction("UserBenchMarks");
        }

        public ActionResult CVTFreeTextMaster()
        {
            try
            {
                var cvts = db.CVT.Where(cvt => !cvt.IsIgnorableInDelivery).Select(cvt => cvt.CVTS + (!string.IsNullOrEmpty(cvt.AssociatedFreeText) ? "::" + cvt.AssociatedFreeText : string.Empty)).ToList();
                var Ignorablecvts = db.CVT.Where(cvt => cvt.IsIgnorableInDelivery).Select(cvt => cvt.CVTS + (!string.IsNullOrEmpty(cvt.AssociatedFreeText) ? "::" + cvt.AssociatedFreeText : string.Empty)).ToList();
                var freetexts = db.FreeText.Select(freetext => freetext.FreeTexts).ToList();
                ViewBag.cvts = cvts;
                ViewBag.freetexts = freetexts;
                ViewBag.Ignorablecvts = Ignorablecvts;
                return View();

            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return View("Error");
            }
        }

        public ActionResult DelivarableCVTs()
        {
            ArrayList list = new ArrayList();
            var cvts = db.CVT.Where(cvt => !cvt.IsIgnorableInDelivery);

            foreach (var cvt in cvts)
            {
                list.Add(new
                {
                    id = cvt.Id,
                    CVT = cvt.CVTS,
                    AssociatedFreetext = cvt.AssociatedFreeText
                });
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DelivarableCVTsCrud(DelivarableCVTsVM model)
        {
            if (model.oper == "add")
            {
                if (ModelState.IsValid)
                {
                    CVT cvtToAdd = new CVT
                    {
                        CVTS = model.CVT,
                        AssociatedFreeText = model.AssociatedFreetext,
                        IsIgnorableInDelivery = false
                    };
                    var existingcvt = db.CVT.Where(cvt => cvt.CVTS.SafeEqualsLower(cvtToAdd.CVTS)).FirstOrDefault();
                    if (existingcvt == null)
                    {
                        db.Entry(cvtToAdd).State = EntityState.Added;
                        db.SaveChanges();
                        return Content("Created");
                    }
                    else
                        return Content("CVT already exist");
                }
            }

            if (model.oper == "edit")
            {
                if (ModelState.IsValid)
                {
                    string id = HttpContext.Request.Params.Get("id");
                    if (id != null)
                    {
                        CVT cvtToEdit = db.CVT.Find(Convert.ToInt32(id));
                        cvtToEdit.CVTS = model.CVT;
                        cvtToEdit.AssociatedFreeText = model.AssociatedFreetext;
                        db.Entry(cvtToEdit).State = EntityState.Modified;
                        db.SaveChanges();
                        return Content("Updated");
                    }
                }
            }

            if (model.oper == "del")
            {
                string id = HttpContext.Request.Params.Get("id");
                if (id != null)
                {
                    CVT cvtToEdit = db.CVT.Find(Convert.ToInt32(id));
                    db.CVT.Remove(cvtToEdit);
                    db.SaveChanges();
                    return Content("Deleted");
                }
            }
            return Content("Invalid request");
        }

        public ActionResult Freetexts()
        {
            ArrayList list = new ArrayList();
            foreach (var freetext in db.FreeText)
            {
                list.Add(new
                {
                    id = freetext.Id,
                    Freetext = freetext.FreeTexts
                });
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FreetextsCrud(DelivarablefreeTextVM model)
        {
            if (model.oper == "add")
            {
                if (ModelState.IsValid)
                {
                    FreeText FreetextToAdd = new FreeText
                    {
                        FreeTexts = model.Freetext
                    };
                    var existingFreetext = db.FreeText.Where(cvt => cvt.FreeTexts.SafeEqualsLower(FreetextToAdd.FreeTexts));
                    if (existingFreetext == null)
                    {
                        db.Entry(FreetextToAdd).State = EntityState.Added;
                        db.SaveChanges();
                        return Content("Created");
                    }
                    else
                        return Content("CVT already exist");
                }
            }

            if (model.oper == "edit")
            {
                if (ModelState.IsValid)
                {
                    string id = HttpContext.Request.Params.Get("id");
                    if (id != null)
                    {
                        FreeText cvtToEdit = db.FreeText.Find(Convert.ToInt32(id));
                        cvtToEdit.FreeTexts = model.Freetext;
                        db.Entry(cvtToEdit).State = EntityState.Modified;
                        db.SaveChanges();
                        return Content("Updated");
                    }
                }
            }

            if (model.oper == "del")
            {
                string id = HttpContext.Request.Params.Get("id");
                if (id != null)
                {
                    FreeText FreetextToEdit = db.FreeText.Find(Convert.ToInt32(id));
                    db.FreeText.Remove(FreetextToEdit);
                    db.SaveChanges();
                    return Content("Deleted");
                }
            }
            return Content("Invalid request");
        }

        public ActionResult IgnorableCVTs()
        {
            ArrayList list = new ArrayList();
            var cvts = db.CVT.Where(cvt => cvt.IsIgnorableInDelivery);

            foreach (var cvt in cvts)
            {
                list.Add(new
                {
                    id = cvt.Id,
                    CVT = cvt.CVTS,
                    AssociatedFreetext = cvt.AssociatedFreeText,
                    ExistingType = cvt.ExistingType.ToString(),
                    NewType = cvt.NewType.ToString()
                });
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ParticipantTypes()
        {
            //return your dropdown list html code
            var participantTypes = Enum.GetValues(typeof(ParticipantType))
                                       .Cast<ParticipantType>()
                                       .Select(v => $"<option value={v.ToString()}>{v.ToString()}</option>")
                                       .ToList();
            return Content($"<select>{string.Join("", participantTypes)}</select>");
        }

        public ActionResult ChemicalTypes()
        {
            //return your dropdown list html code
            var chemicalTypes = Enum.GetValues(typeof(ChemicalType))
                                       .Cast<ChemicalType>()
                                       .Select(v => $"<option value={v.ToString()}>{v.ToString()}</option>")
                                       .ToList();
            return Content($"<select>{string.Join("", chemicalTypes)}</select>");
        }

        public ActionResult IgnorableCVTsCrud(IgnorableCVTsVM model)
        {
            if (model.oper == "add")
            {
                if (ModelState.IsValid)
                {
                    CVT cvtToAdd = new CVT
                    {
                        CVTS = model.CVT,
                        AssociatedFreeText = model.AssociatedFreetext,
                        IsIgnorableInDelivery = true,
                        NewType = model.NewType,
                        ExistingType = model.ExistingType
                    };
                    var existingcvt = db.CVT.Where(cvt => cvt.CVTS == model.CVT).FirstOrDefault();
                    if (existingcvt == null)
                    {
                        db.Entry(cvtToAdd).State = EntityState.Added;
                        db.SaveChanges();
                        return Content("Created");
                    }
                    else
                        return Content("CVT already exist");
                }
            }

            if (model.oper == "edit")
            {
                if (ModelState.IsValid)
                {
                    string id = HttpContext.Request.Params.Get("id");
                    if (id != null)
                    {
                        CVT cvtToEdit = db.CVT.Find(Convert.ToInt32(id));
                        cvtToEdit.CVTS = model.CVT;
                        cvtToEdit.AssociatedFreeText = model.AssociatedFreetext;
                        cvtToEdit.ExistingType = model.ExistingType;
                        cvtToEdit.NewType = model.NewType;
                        db.Entry(cvtToEdit).State = EntityState.Modified;
                        db.SaveChanges();
                        return Content("Updated");
                    }
                }
            }

            if (model.oper == "del")
            {
                string id = HttpContext.Request.Params.Get("id");
                if (id != null)
                {
                    CVT cvtToEdit = db.CVT.Find(Convert.ToInt32(id));
                    db.CVT.Remove(cvtToEdit);
                    db.SaveChanges();
                    return Content("Deleted");
                }
            }
            return Content("Invalid request");
        }

        public ActionResult SaveIgnorableCVTs(string CVTText)
        {
            try
            {
                using (TransactionScope sc = new TransactionScope())
                {
                    if (!string.IsNullOrEmpty(CVTText))
                    {
                        List<string> cvts = CVTText.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        var cvtTexts = cvts.Select(s => s.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries)[0]).GroupBy(s => s).ToList();

                        if (cvtTexts.Count == cvts.Count)
                        {
                            List<CVT> cvtsToAdd = new List<CVT>();
                            foreach (var cvt in cvts)
                            {
                                string cvttext = string.Empty;
                                string AssociatedFreetext = string.Empty;
                                if (cvt.Contains("::"))
                                {
                                    cvttext = cvt.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries)[0];
                                    AssociatedFreetext = cvt.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries).Count() > 1 ? cvt.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries)[1] : string.Empty;
                                    if (cvttext.Equals(AssociatedFreetext))
                                        return Content($"CVT and Associated Freetext can't be same in {cvt}");
                                }
                                var cvtToAdd = new CVT
                                {
                                    CVTS = cvt.Contains("::") ? cvt.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries)[0] : cvt,
                                    AssociatedFreeText = cvt.Contains("::") && cvt.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries).Count() > 1 ? cvt.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries)[1] : string.Empty,
                                    IsIgnorableInDelivery = true
                                };
                                cvtsToAdd.Add(cvtToAdd);
                            }
                            var existingCVts = db.CVT.Where(cvt => cvt.IsIgnorableInDelivery);
                            db.CVT.RemoveRange(existingCVts);
                            db.CVT.AddRange(cvtsToAdd);
                        }
                        else
                            return Content("CVTs Contains Duplicate Texts");
                    }

                    db.SaveChanges();
                    sc.Complete();
                }
                return Content("Success");
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return View("Error");
            }
        }

        public ActionResult SaveCVTFreeTexts()
        {
            try
            {
                var CVTsFromUI = HttpContext.Request.Params.Get("cvtTexts");
                var freetextsFromUI = HttpContext.Request.Params.Get("FreeTexts");
                using (TransactionScope sc = new TransactionScope())
                {
                    if (!string.IsNullOrEmpty(CVTsFromUI))
                    {
                        List<string> cvts = CVTsFromUI.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        var cvtTexts = cvts.Select(s => s.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries)[0]).GroupBy(s => s).ToList();

                        List<string> freetexts = freetextsFromUI.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        var GroupedFreetexts = freetexts.GroupBy(s => s).ToList();
                        if (cvtTexts.Count == cvts.Count)
                        {
                            if (GroupedFreetexts.Count == freetexts.Count)
                            {
                                List<CVT> cvtsToAdd = new List<CVT>();
                                foreach (var cvt in cvts)
                                {
                                    string cvttext = string.Empty;
                                    string AssociatedFreetext = string.Empty;
                                    if (cvt.Contains("::"))
                                    {
                                        cvttext = cvt.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries)[0];
                                        AssociatedFreetext = cvt.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries).Count() > 1 ? cvt.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries)[1] : string.Empty;
                                        if (cvttext.Equals(AssociatedFreetext))
                                            return Content($"CVT and Associated Freetext can't be same in {cvt}");
                                    }
                                    var cvtToAdd = new CVT
                                    {
                                        CVTS = cvt.Contains("::") ? cvt.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries)[0] : cvt,
                                        AssociatedFreeText = cvt.Contains("::") && cvt.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries).Count() > 1 ? cvt.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries)[1] : string.Empty
                                    };
                                    cvtsToAdd.Add(cvtToAdd);
                                }
                                List<FreeText> freetextsToAdd = new List<FreeText>();
                                foreach (var freetext in freetexts)
                                    freetextsToAdd.Add(new FreeText { FreeTexts = freetext });
                                var existingCVts = db.CVT.Where(cvt => !cvt.IsIgnorableInDelivery);
                                var existingFreetexts = db.FreeText;
                                db.CVT.RemoveRange(existingCVts);
                                db.FreeText.RemoveRange(existingFreetexts);
                                db.CVT.AddRange(cvtsToAdd);
                                db.FreeText.AddRange(freetextsToAdd);
                            }
                            else
                                return Content("Freetexts Contains Duplicate Texts");
                        }
                        else
                            return Content("CVTs Contains Duplicate Texts");
                    }
                    db.SaveChanges();
                    sc.Complete();
                }
                return RedirectToAction("CVTFreeTextMaster");
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return View("Error");
            }
        }

        public ActionResult SaveUserRoles()
        {
            try
            {
                var userIds = HttpContext.Request.Params.GetValues("user");
                var roleIds = HttpContext.Request.Params.GetValues("role");
                var usersById = (from u in db.Users group u by u.Id into byIds select new { id = byIds.Key, user = byIds.ToList() }).ToDictionary(d => d.id, d => d.user.First());
                var rolesById = (from r in db.ApplicationRoles group r by r.Id into byIds select new { id = byIds.Key, user = byIds.ToList() }).ToDictionary(d => d.id, d => d.user.First());
                using (TransactionScope scope = new TransactionScope())
                {
                    foreach (var userId in userIds)
                    {
                        if (usersById.ContainsKey(userId))
                        {
                            ApplicationUser user = usersById[userId];
                            if (user != null)
                            {
                                foreach (var roleId in roleIds)
                                {
                                    var userRoleId = HttpContext.Request.Params.Get("userRole_" + userId + "_" + roleId);
                                    if (!string.IsNullOrEmpty(userRoleId) && Int32.Parse(roleId) == (int)Role.Curator)
                                        AddUserRole(user, Role.Curator);
                                    else if (!string.IsNullOrEmpty(userRoleId) && Int32.Parse(roleId) == (int)Role.Reviewer)
                                        AddUserRole(user, Role.Reviewer);
                                    else if (!string.IsNullOrEmpty(userRoleId) && Int32.Parse(roleId) == (int)Role.QC)
                                        AddUserRole(user, Role.QC);
                                    else if (!string.IsNullOrEmpty(userRoleId) && Int32.Parse(roleId) == (int)Role.ToolManager)
                                        AddUserRole(user, Role.ToolManager);
                                    else if (!string.IsNullOrEmpty(userRoleId) && Int32.Parse(roleId) == (int)Role.ProjectManger)
                                        AddUserRole(user, Role.ProjectManger);
                                }
                            }
                        }
                    }
                    db.SaveChanges();
                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return View("Error");
            }
            return RedirectToAction("UserRoles");
        }
        public void AddUserRole(ApplicationUser user, Role userRole)
        {
            var AllRoles = db.UserRoles;
            var role = AllRoles.Where(ar => ar.ApplicationUser.Id == user.Id && ar.Role == userRole).FirstOrDefault();
            if (role == null)
            {
                var RoleToAdd = new UserRole { ApplicationUser = user, Role = userRole, UserId = user.Id };
                AllRoles.Add(RoleToAdd);
                db.UserDefaultDensities.Add(new UserDefaultDensities { UserRole = RoleToAdd, UserRXNDensity = (int)userRole });
            }
        }
    }
}