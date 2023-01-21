using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RIS.Models;
using RISDB;

namespace RIS.Controllers
{
    public class ProcedureController : Controller
    {
        // GET: Procedure
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
            if (!RIS.Models.User.hasPerm(userId, Perms.ProcedureIndex))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }

            ViewData["PageName"] = Resources.Res.procs;
            List<Procedure> mtList = Procedure.getAll();
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
            if (!RIS.Models.User.hasPerm(userId, Perms.ProcedureCreate))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }

            ViewData["PageName"] = Resources.Res.addProc;
            Procedure u = new Procedure();
            return View(u);
        }

        [HttpPost]
        public ActionResult Create(Procedure u)
            
        {
            if (!string.IsNullOrEmpty(u.name))
            {
                return View();
            }
            u.num = OracleRIS.GetOracleSequenceValue("PROCEDURE_SEQ");
            try
            {
                // TODO: Add insert logic here

                if (ModelState.IsValid)
                {

                    string ex = Models.Procedure.addProcedure(u);
                    if (string.IsNullOrEmpty(ex))
                        return RedirectToAction("Index", new { });
                    else
                        ModelState.AddModelError("", ex);
                }
                return View(u);
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Edit(int i)
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
            if (!RIS.Models.User.hasPerm(userId, Perms.ProcedureEdit))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }

            ViewData["PageName"] = RIS.Resources.Res.editPrco.ToString();
            Models.Procedure p = Models.Procedure.select(i);
            return View(p);
        }

        [HttpPost]
        public ActionResult Edit(Procedure u)
        {
            //if (!string.IsNullOrEmpty(u.name))
            //{
            //    return View();
            //}
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
            if (!RIS.Models.User.hasPerm(userId, Perms.ProcedureEdit))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }

            try
            {
                // TODO: Add delete logic here
                string ex = "";
                if (ModelState.IsValid)
                {
                    ex = Models.Procedure.editProcedure(u);
                    if (string.IsNullOrEmpty(ex))
                        return RedirectToAction("Index", new { });
                }
                ModelState.AddModelError("", ex);
                return View(u);
            }
            catch
            {
                return View(u);
            }
        }

        public ActionResult Delete(int i)
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
            if (!RIS.Models.User.hasPerm(userId, Perms.ProcedureDelete))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }

            ViewData["PageName"] = RIS.Resources.Res.deleteProc.ToString();
            Models.Procedure u = Models.Procedure.select(i);
            return View(u);
        }

        [HttpPost]
        public ActionResult Delete(int i, FormCollection collection)
        {
            Procedure u = Models.Procedure.select(i);
            try
            {
                // TODO: Add delete logic here
                string ex = Models.Procedure.deleteProcedure(u.num);
                if (string.IsNullOrEmpty(ex))
                    return RedirectToAction("Index", new { });
                ModelState.AddModelError("", ex);
                return View(u);
            }
            catch
            {
                return View(u);
            }
        }


    }
}