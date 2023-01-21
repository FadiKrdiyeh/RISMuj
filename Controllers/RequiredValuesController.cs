using RIS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RISDB;

namespace RIS.Controllers
{
    public class RequiredValuesController : Controller
    {
        // GET: RequiredValues
        public ActionResult Index()
        {
            return View();
        }

        // GET: RequiredValues/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: RequiredValues/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RequiredValues/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        //TODO
        // GET: RequiredValues/Edit/5
        public ActionResult Manage()
        {
            string uName = "";
            try
            {
                uName = Session["userName"].ToString();
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
            //TODO ** edit perms add new perm for manage req vals
            int userId = RIS.Models.User.getUserByUname(uName).num;
            if (!RIS.Models.User.hasPerm(userId, Perms.GroupEdit))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }

            List<RequiredValues> reqVals = RequiredValues.getRequiredValuessList();
            ViewData["rppCount"] = RequiredValues.getRowsPerPgById((int)ReqPatientVals.rowsPerPage).reqRowsPerPage;
            ViewData["PageName"] = RIS.Resources.Res.reqFieldManage;
            
           // ViewBag.rvList = reqVals;
            
            return View(reqVals);
        }

        // POST: Group/Edit/5
        [HttpPost]
        public ActionResult Manage(List<RequiredValues> rvl,int rpp)
        {
            try
            {
                string s = RequiredValues.deleteReqVals();
                if(String.IsNullOrEmpty(s))
                {
                    string ex = RequiredValues.update(rvl);
                    RequiredValues.updateRowspp(rpp);
                    if (!string.IsNullOrEmpty(ex))
                    {
                        ModelState.AddModelError("", ex);
                        return RedirectToAction("Manage");
                    }
                    else
                    {
                        TempData["message"] = RIS.Resources.Res.doneSuccessfully;
                        ModelState.AddModelError("", "تم التعديل بنجاح");
                        return RedirectToAction("Manage");

                    }
                }
                else
                {
                    ModelState.AddModelError("", s);
                    return RedirectToAction("Manage");
                }
            }
            catch
            {
                return RedirectToAction("Manage");
            }

        }

        // GET: RequiredValues/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: RequiredValues/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
