using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using RIS.Models;
using RISDB;

namespace RIS.Controllers
{
    public class DepartementController : Controller
    {
        // GET: Departement
        public ActionResult Index()
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

            int userId = RIS.Models.User.getUserByUname(uName).num;
            if (!RIS.Models.User.hasPerm(userId, Perms.DepartmentIndex))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }

            ViewData["PageName"] = "أنواع الآلات";
            List< Departement> mtList = Departement.getData();
            return View(mtList.ToList());
        }


        public ActionResult Create()
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

            int userId = RIS.Models.User.getUserByUname(uName).num;
            if (!RIS.Models.User.hasPerm(userId, Perms.DepartmentCreate))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }

            ViewData["PageName"] = "إضافة نوع آلة";
            Departement mt = new Departement();
            mt.num = OracleRIS.GetOracleSequenceValue("DEPT_SEQ");
            return View(mt);
        }

        // POST: ModalityType/Create
        [HttpPost]
        public ActionResult Create(Departement mt)
        {
            try
            {
                // TODO: Add insert logic here

                if (ModelState.IsValid)
                {
                    string ex = Departement.Insert(mt);
                    if (string.IsNullOrEmpty(ex))
                        return RedirectToAction("Index", new { });
                    else
                        ModelState.AddModelError("", ex);
                }
                return View(mt);
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Edit(int id)
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

            int userId = RIS.Models.User.getUserByUname(uName).num;
            if (!RIS.Models.User.hasPerm(userId, Perms.DepartmentEdit))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }

            ViewData["PageName"] = "تعديل قسم";
            Departement mt = Departement.select(id);
            return View(mt);
        }

        // POST: ModalityType/Edit/5
        [HttpPost]
        public ActionResult Edit(Departement mt)
        {
            try
            {
                // TODO: Add delete logic here
                string ex = Departement.Edit(mt);
                if (string.IsNullOrEmpty(ex))
                    return RedirectToAction("Index", new { });
                ModelState.AddModelError("", ex);
                return View(mt);
            }
            catch
            {
                return View(mt);
            }
        }

        // GET: ModalityType/Delete/5
        public ActionResult Delete(int id)
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

            int userId = RIS.Models.User.getUserByUname(uName).num;
            if (!RIS.Models.User.hasPerm(userId, Perms.DepartmentDelete))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }

            ViewData["PageName"] = "حذف قسم";
            Departement mt = Departement.select(id);
            return View(mt);
        }

        // POST: ModalityType/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            Departement mt = Departement.select(id);
            try
            {
                // TODO: Add delete logic here
                string ex = Departement.Delete(id);
                if (string.IsNullOrEmpty(ex))
                    return RedirectToAction("Index", new { });
                ModelState.AddModelError("", ex);
                return View(mt);
            }
            catch
            {
                return View(mt);
            }
        }

    }
}