using RIS.Models;
using RISDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web;


namespace RIS.Controllers
{
    public class ModalityController : Controller
    {
        // GET: Modality
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
            if (!RIS.Models.User.hasPerm(userId, Perms.ModalityIndex))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }

            //ViewData["PageName"] = "الآلات";
            ViewData["PageName"] = Resources.Res.Modalities;
            List<Modality> mtList = Modality.getData();
            return View(mtList.ToList());
        }

        // GET: Modality/Details/5
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
            if (!RIS.Models.User.hasPerm(userId, Perms.ModalityIndex))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // GET: Modality/Create
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
            if (!RIS.Models.User.hasPerm(userId, Perms.ModalityCreate))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }

            ViewData["PageName"] = Resources.Res.addMod;
            Modality mt = new Modality();
            ViewData["type"] = ModalityType.GetModalityTypesList(mt.type.ToString());
            mt.num = OracleRIS.GetOracleSequenceValue("Modality_SEQ");
            ViewData["departement"] = Departement.GetDepartementList(true, null);
            return View(mt);
        }

        // POST: Modality/Create
        [HttpPost]
        public ActionResult Create(Modality mt)
        {

            try
            {
                // TODO: Add insert logic here

                if (ModelState.IsValid)
                {
                  
                    string ex = Modality.Insert(mt);
                    if (string.IsNullOrEmpty(ex))
                        return RedirectToAction("Index", new { });
                    else
                        ModelState.AddModelError("", ex);
                }
                ViewData["type"] = ModalityType.GetModalityTypesList(mt.type.ToString());
                ViewData["departement"] = Departement.GetDepartementList(true, mt.departement);
                return View(mt);
            }
            catch
            {
                return View();
            }
        }

        // GET: Modality/Edit/5
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
            if (!RIS.Models.User.hasPerm(userId, Perms.ModalityEdit))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }

            ViewData["PageName"] = Resources.Res.editMod;
            Modality mt = Modality.Select(id);
            ViewData["type"] = ModalityType.GetModalityTypesList(mt.type.ToString());
            try
            {
                ViewData["departement"] = Departement.GetDepartementList(false, mt.modalityDepartement.num.ToString());

            }

            catch
            {
                ViewData["departement"] = Departement.GetDepartementList(true, "");

            }
            return View(mt);
        }

        // POST: Modality/Edit/5
        [HttpPost]
        public ActionResult Edit(Modality mt)
        {
            try
            {
                // TODO: Add delete logic here
                string ex = "";
                if (ModelState.IsValid)
                {
                     ex= Modality.Edit(mt);
                    if (string.IsNullOrEmpty(ex))
                        return RedirectToAction("Index", new { });
                }

                ModelState.AddModelError("", ex);
                ViewData["type"] = ModalityType.GetModalityTypesList(mt.type.ToString());
                try
                {
                    ViewData["departement"] = Departement.GetDepartementList(false, mt.modalityDepartement.num.ToString());

                }

                catch
                {
                    ViewData["departement"] = Departement.GetDepartementList(true, "");

                }

                return View(mt);
            }
            catch
            {
                return View(mt);
            }
        }

        // GET: Modality/Delete/5
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
            if (!RIS.Models.User.hasPerm(userId, Perms.ModalityDelete))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }

            ViewData["PageName"] = Resources.Res.deleteMod;
            Modality mt = Modality.Select(id);
            return View(mt);
        }

        // POST: Modality/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            Modality mt = Modality.Select(id);
            try
            {
                // TODO: Add delete logic here
                string ex = Modality.Delete(id);
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
        public JsonResult getModalityQcode(int modId)
        {
            string ModalityQcode = Modality.getModalityQcode(modId);

            return Json(ModalityQcode);
        }
    }
}
