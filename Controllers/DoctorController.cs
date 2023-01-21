using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RIS.Models;
using RISDB;

namespace RIS.Controllers
{
    /// <summary>
    /// This controller deals with all actions on doctors page
    /// </summary>
    public class DoctorController : Controller
    {
        // GET: Doctor
        /// <summary>
        /// This action is called when doctor's page is accessed
        /// </summary>
        /// <returns>the doctors index view</returns>
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
            if (!RIS.Models.User.hasPerm(userId, Perms.PermsIndex))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }

            List<Doctor> doctors = new List<Doctor>();
            doctors = Doctor.getDoctorsList();
            return View(doctors);
        }

        /// <summary>
        /// This action is called when a user wants to create a new doctor
        /// </summary>
        /// <returns>the create order view</returns>
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
            if (!RIS.Models.User.hasPerm(userId, Perms.PermsIndex))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }

            Doctor doc = new Doctor();
            ViewData["department"] = Departement.GetDepartementListNames(true, "");
            return View(doc);
        }

        /// <summary>
        /// This action creates anew doctor and inserts it into database
        /// </summary>
        /// <param name="doc">a doctor object contains the doctor's details</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(Doctor doc)
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
            if (!RIS.Models.User.hasPerm(userId, Perms.PermsIndex))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }

            try
            {
                if (ModelState.IsValid)
                {
                    doc.num = OracleRIS.GetOracleSequenceValue("DOCTORS_SEQ");
                    doc.insertDate = System.DateTime.Now;
                    doc.insertUser = userId;
                    string ex = Doctor.insert(doc);
                    if (string.IsNullOrEmpty(ex))
                        return RedirectToAction("Index", new { });
                    else
                        ModelState.AddModelError("", ex);
                }
                ViewData["department"] = Departement.GetDepartementListNames(true, "");
                return View(doc);
            }
            catch
            {
                return View();
            }            
            //return View();
        }

        /// <summary>
        /// This action is called when a user wants to delete a doctor
        /// </summary>
        /// <param name="id">the doctor's ID</param>
        /// <returns></returns>
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
            if (!RIS.Models.User.hasPerm(userId, Perms.PermsIndex))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }

            Doctor doc = Doctor.select(id);
            return View(doc);
        }

        /// <summary>
        /// This action deletes a doctor from database
        /// </summary>
        /// <param name="id">the doctor's ID</param>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
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
            if (!RIS.Models.User.hasPerm(userId, Perms.PermsIndex))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }

            Doctor doc = Doctor.select(id);
            try
            {
                // TODO: Add delete logic here
                string ex = Doctor.delete(id);
                if (string.IsNullOrEmpty(ex))
                    return RedirectToAction("Index", new { });
                ModelState.AddModelError("", ex);
                return View(doc);
            }
            catch
            {
                ModelState.AddModelError("", Resources.Res.Error);
                return View(doc);
            }
        }

        /// <summary>
        /// This action used for autocompletion of doctor's name in creating order pages
        /// </summary>
        /// <param name="Prefix">the first letters of doctor's name</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DocAutoComplete(string Prefix)
        {
            //Note : you can bind same list from database  
            List<Doctor> doctors = Doctor.getDoctorsList();
            //Searching records from list using LINQ query  
            var DocList = (from N in doctors
                            where N.name.ToUpper().Contains(Prefix.ToUpper())
                            select new { N.name });
            return Json(DocList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This action is called when the user wants to edit some doctor's information
        /// </summary>
        /// <param name="id">the doctor's ID</param>
        /// <returns>the edit doctor view</returns>
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
            if (!RIS.Models.User.hasPerm(userId, Perms.PermsIndex))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }

            Doctor d = Doctor.select(id);
            ViewData["department"] = Departement.GetDepartementListNames(true, d.department.ToString());

            return View(d);
        }

        /// <summary>
        /// This action edits the information of a doctor
        /// </summary>
        /// <param name="d">doctor object contains the new doctor's details</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(Doctor d)
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
            if (!RIS.Models.User.hasPerm(userId, Perms.PermsIndex))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }

            ViewData["department"] = Departement.GetDepartementListNames(true, d.department.ToString());
            if (ModelState.IsValid)
            {
                string ex = Models.Doctor.edit(d);
                if (string.IsNullOrEmpty(ex))
                    return RedirectToAction("Index", new { });
                ModelState.AddModelError("", ex);

            }


            return View(d);
        }
    }
}