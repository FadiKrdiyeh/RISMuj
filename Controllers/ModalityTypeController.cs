using RIS.Models;
using RISDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RIS.Controllers
{
    public class ModalityTypeController : Controller
    {
        // GET: ModalityType
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
            if (!RIS.Models.User.hasPerm(userId, Perms.ModalityTypeIndex))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }

            ViewData["PageName"] = "أنواع الآلات";
            List<ModalityType> mtList = ModalityType.getData();
            return View(mtList.ToList());
        }

        // GET: ModalityType/Details/5
        public ActionResult Details(int id)
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
            if (!RIS.Models.User.hasPerm(userId, Perms.ModalityTypeIndex))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // GET: ModalityType/Create
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
            if (!RIS.Models.User.hasPerm(userId, Perms.ModalityTypeCreate))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }

            ViewData["PageName"] = "إضافة نوع آلة";
            ModalityType mt = new ModalityType();
            mt.num = OracleRIS.GetOracleSequenceValue("ModType_SEQ");
            return View(mt);
        }

        // POST: ModalityType/Create
        [HttpPost]
        public ActionResult Create(ModalityType mt)
        {
            try
            {
                // TODO: Add insert logic here

                if (ModelState.IsValid)
                {
                    string ex = ModalityType.Insert(mt);
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

        // GET: ModalityType/Edit/5
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
            if (!RIS.Models.User.hasPerm(userId, Perms.ModalityTypeEdit))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }

            ViewData["PageName"] = "تعديل نوع آلة";
            ModalityType mt = ModalityType.Select(id);
            return View(mt);
        }

        // POST: ModalityType/Edit/5
        [HttpPost]
        public ActionResult Edit(ModalityType mt)
        {
            try
            {
                // TODO: Add delete logic here
                string ex = "";
                if (ModelState.IsValid)
                {
                    ex = ModalityType.Edit(mt);
                    if (string.IsNullOrEmpty(ex))
                        return RedirectToAction("Index", new { });
                }
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
            if (!RIS.Models.User.hasPerm(userId, Perms.ModalityTypeDelete))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }

            ViewData["PageName"] = "حذف نوع آلة";
            ModalityType mt = ModalityType.Select(id);
            return View(mt);
        }

        // POST: ModalityType/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            ModalityType mt = ModalityType.Select(id);
            try
            {
                // TODO: Add delete logic here
                string ex = ModalityType.Delete(id);
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
