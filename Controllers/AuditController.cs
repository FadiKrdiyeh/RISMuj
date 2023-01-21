using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RIS.Controllers
{
    public class AuditController : Controller
    {
        // GET: Audit
        public ActionResult Index()
        {
            bool v1 = trialConfigs.checkperiod();
            bool v2 = trialConfigs.getOrderNumbers();
            if (!v1 || !v2)
            {
                return RedirectToAction("Index", "Home");
            }
            string uName = "";

            try
            {
                uName = Session["userName"].ToString();
            }

            catch
            {
                return RedirectToAction("Index", "Home", new { });
            }
            int userId = RIS.Models.User.getUserByUname(uName).num;
            if (RIS.Models.User.hasPerm(userId, Perms.ClinicAppoinmentAudit))
            {
                TempData["clinicPerm"] = 1;
            }
            else
                TempData["clinicPerm"] = 0;
            return View();
        }

        // GET: Audit/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Audit/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Audit/Create
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

        // GET: Audit/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Audit/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Audit/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Audit/Delete/5
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
