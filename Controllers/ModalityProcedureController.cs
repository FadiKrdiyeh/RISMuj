using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RIS.Models;
using RISDB;

namespace RIS.Controllers
{
    public class ModalityProcedureController : Controller
    {
        // GET: ModalityProcedure
        public ActionResult Index(int mid)
        {
            try
            {
                bool v1 = trialConfigs.checkperiod();
                bool v2 = trialConfigs.getOrderNumbers();
                if (!v1 || !v2)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch
            {
                return RedirectToAction("Index", "Home");

            }
            //try
            //{
            //    //string t = Session["userType"].ToString();
            //    //if (t != "1")
            //    //{
            //    //    return RedirectToAction("Index", "Home", new { });

            //    //}

            //    string u = Session["userName"].ToString();
            //}

            //catch
            //{
            //    return RedirectToAction("Index", "Home", new { });
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
            if (!RIS.Models.User.hasPerm(userId, Perms.ProcedureIndex))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }


            Modality m = Modality.Select(mid);
            ViewData["ModalityName"] = m.name.ToString();
            ViewData["ModalityID_"] = m.num.ToString();
            List<ModalityProcedure> prs = ModalityProcedure.selectModProc(mid);
            return View(prs);
        }

        public ActionResult Create(int mid)
        {
            //try
            //{
            //    //string t = Session["userType"].ToString();
            //    //if (t != "1")
            //    //{
            //    //    return RedirectToAction("Index", "Home", new { });

            //    //}

            //    string un = Session["userName"].ToString();
            //}

            //catch
            //{
            //    return RedirectToAction("Index", "Home", new { });
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
            if (!RIS.Models.User.hasPerm(userId, Perms.ProcedureCreate))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }


            Modality m = Modality.Select(mid);
            ViewData["PageName"] = Resources.Res.addProc;
            ViewData["ModalityName"] = m.name.ToString();
            ViewData["ModalityID_"] = mid.ToString();
            ViewData["ProcedureID"] = Procedure.GetProceduresList(true,"");
            ModalityProcedure u = new ModalityProcedure();
            u.ModalityId = mid;
            return View(u);
        }

        [HttpPost]
        public ActionResult Create(ModalityProcedure u)
        {
            u.num = OracleRIS.GetOracleSequenceValue("MODAITYPROCEDURE_SEQ");
            Modality m = Modality.Select(u.ModalityId);
            bool mp1 = ModalityProcedure.checkDuplicate(u.ModalityId, u.ProcedureId);

            if (!mp1)
                return RedirectToAction("Index", new { mid = u.ModalityId }); ;
            //  ViewData["ModalityName"] = m.name.ToString();

            //u.ModalityId = Int32.Parse(ViewData["ModalityID_"].ToString());
            try
            {

                if (ModelState.IsValid)
                {

                    string ex = Models.ModalityProcedure.addProcToMod(u);
                    ViewData["ModalityID_"] = u.ModalityId.ToString();
                    ViewData["ProcedureID"] = Procedure.GetProceduresList(true, "");
                    if (string.IsNullOrEmpty(ex))
                        return RedirectToAction("Index", new { mid = u.ModalityId });
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

        public ActionResult Delete(int id, int mid)
        {
            //try
            //{
            //    string t = Session["userType"].ToString();
            //    if (t != "1")
            //    {
            //        return RedirectToAction("Index", "Home", new { });

            //    }

            //    string u = Session["userName"].ToString();
            //}

            //catch
            //{
            //    return RedirectToAction("Index", "Home", new { });
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
            if (!RIS.Models.User.hasPerm(userId, Perms.ProcedureDelete))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }


            ModalityProcedure.deleteModProc(id);
            return RedirectToAction("Index", "ModalityProcedure", new { mid = mid });
            //return Redirect("ModalityProcedure?="+ mid);
            //return RedirectToAction("Create", "Radiology", new { pId = pt.id });
         
        }
    }
}