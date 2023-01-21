using RIS.Models;
using RISDB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace RIS.Controllers
{
    public class AppoinmentsController : Controller
    {
        // GET: Appoinments
        /// <summary>
        /// This action is called when Appoinments webpage is accessed and for paging
        /// </summary>
        /// <permission cref="Perms.ClinicAppoinmentIndex">the user has to have the ClinicAppoinmentIndex permission to access
        /// Clinics Appoinments page</permission>
        /// <param name="page">the page number</param>
        /// <param name="count">total number of pages</param>
        /// <param name="docId">the appoinment's document ID</param>
        /// <param name="firstname">first name of the patient that the appoinment belongs to</param>
        /// <param name="lastname">first name of the patient that the appoinment belongs to</param>
        /// <param name="clinic">the department ID</param>
        /// <returns> Clinic Appoinment Index view</returns>
        public ActionResult Index([DefaultValue(1.0)] double page, [DefaultValue(1.0)] double count, string firstname, string lastname, int? departments, int? clinic, int? pagging, int? AppStatus, string appDate)
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

            //  int userId = RIS.Models.User.getUserByUname(uName).num;
            RIS.Models.User lu = RIS.Models.User.getUserByUname(uName);
            int userId = lu.num;
            //    int? userClinic = (pagging == null) ? int.Parse(lu.departement) : clinic;
            int? userClinic = (clinic != null) ? clinic : int.Parse(Session["userClinicId"].ToString());

            ViewData["clinic"] = GeniricIndex.GetClinicIndexList(true, Session["userClinicId"].ToString() + "", "CLINIC");


            if (!RIS.Models.User.hasPerm(userId, Perms.ClinicAppoinmentIndex))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }


            string s = Session["userType"].ToString();
            string d = (s == "1") ? departments.ToString() : Session["userDep"].ToString();

            double RowsPerPage = RequiredValues.getRowsPerPgById((int)ReqPatientVals.rowsPerPage).reqRowsPerPage;
            string date = ConnectionConfigs.getPacsTime();
            appDate = date.Substring(0, 8) + "00:00:00";
            //appDate = appDate.Substring(6, 2) + "/" + appDate.Substring(4, 2) + "/"+ appDate.Substring(0, 4)+" "+ appDate.Substring(8, 8);
            List<Appoinments> apps = Appoinments.getApps(page, out count, RowsPerPage, firstname, lastname, -1, AppStatus, appDate);

            ViewData["PageCnt"] = count;
            ViewData["count"] = count;
            ViewData["pageNBr"] = page;
            ViewData["page"] = page;
            ViewData["stayInSeach"] = "0";
            //ViewData["appDate"] = 
            //string ssss = (DateTime.Today.ToString());
            //DateTime dada= DateTime.ParseExact(DateTime.Today.ToString(),"yyyyMMddhh24:mm:ss",CultureInfo.InvariantCulture);

            //ViewData["departments"] = Departement.GetDepartementListNames(true, "");
            //ViewData["modalities"] = Modality.GetModalitysList(true, "");
            ViewData["ListParameters"] = new { page, count };

            return View(apps);
        }

        /// <summary>
        /// This action is called when the user is doing search in appoinments page
        /// </summary>
        /// <param name="page">the page number</param>
        /// <param name="count">total number of pages</param>
        /// <param name="docId">the appoinment's document ID</param>
        /// <param name="firstname">first name of the patient that the appoinment belongs to</param>
        /// <param name="lastname">first name of the patient that the appoinment belongs to</param>
        /// <param name="clinic">the department ID</param>
        /// <param name="ss">just to defferentiate it from the GET action</param>
        /// <returns> Clinic Appoinments Index view containing the search results</returns>
        [HttpPost]
        public ActionResult Index([DefaultValue(1.0)] double page, [DefaultValue(1.0)] double count, string firstname, string lastname, int? departments, int? clinic, int? AppStatus, string appDate)
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
            if (!RIS.Models.User.hasPerm(userId, Perms.ClinicAppoinmentIndex))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }
            ViewData["stayInSeach"] = "1";

            string s = Session["userType"].ToString();
            string d = (s == "1") ? "" : Session["userDep"].ToString();

            double RowsPerPage = RequiredValues.getRowsPerPgById((int)ReqPatientVals.rowsPerPage).reqRowsPerPage;
            appDate = (appDate == "") ? appDate : (appDate.Substring(0, 8) + "00:00:00");
            List<Appoinments> apps = Appoinments.getApps(page, out count, RowsPerPage, firstname, lastname, -1, AppStatus, appDate);

            ViewData["firstname"] = (firstname == null) ? "" : firstname;
            ViewData["lastname"] = (lastname == null) ? "" : lastname;

            ViewData["clinic"] = GeniricIndex.GetClinicIndexList(true, Session["userClinicId"].ToString() + "", "CLINIC");

            ViewData["PageCnt"] = count;
            ViewData["count"] = count;
            ViewData["pageNBr"] = page;
            ViewData["page"] = page;
            ViewData["ListParameters"] = new { page, count };

            return View(apps);
        }

        public ActionResult Schedule()
        {

            ConnectionConfigs risConfig = ConnectionConfigs.getConfig();
            Session["pacsSrvrIp"] = risConfig.pacsIp;
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
            if (!RIS.Models.User.hasPerm(userId, Perms.ClinicAppoinmentIndex))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }
            ViewBag.clinics = GeniricIndex.getData("CLINIC");
            ViewData["CLINIC"] = GeniricIndex.GetGeniricIndexList(true, "", "CLINIC");

            Appoinments app = new Appoinments();
            return View(app);
        }

        // GET: Appoinments/Details/5
        public ActionResult Details(int id)
        {
            Appoinments app = Appoinments.SelectExactApp(id);
            return View(app);
        }

        /// <summary>
        /// This action is called when the user wants to create an appoinment for some patient
        /// </summary>
        /// <permission cref="Perms.ClinicAppoinmentCreate">the user has to have the ClinicAppoinmentCreate permission to create an
        /// immediate appoinment for a patient</permission>
        /// <param name="pId">the ID of the patient</param>
        /// <returns>the create appoinment view</returns>
        public ActionResult Create(int pId)
        {

            ConnectionConfigs risConfig = ConnectionConfigs.getConfig();
            Session["pacsSrvrIp"] = risConfig.pacsIp;
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
            if (!RIS.Models.User.hasPerm(userId, Perms.ClinicAppoinmentCreate))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }
            ViewBag.clinics = GeniricIndex.getData("CLINIC");
            ViewData["CLINIC"] = GeniricIndex.GetGeniricIndexList(true, "", "CLINIC");
            //ViewBag.ClinicList = GeniricIndex.getData("CLINIC");
            string depID = Models.User.getDepID(Session["userName"].ToString()); //get the depId of the user.

            ViewData["DepartementName"] = Departement.GetDepartementListNames(true, "");
            ViewData["Type"] = Appoinments.AppsTypes("");

            Patient p = Patient.Select(pId);

            p = Patient.Select(pId);
            ViewData["_Patient"] = p.firstname + " " + p.lastname;
            ViewData["_PatientNB"] = p.givenid;

            Appoinments app = new Appoinments();
            ViewData["payType"] = Appoinments.AppsPayTypes("");

            app.patientID = pId;
            ViewData["_notes"] = p.notes;
            TempData["clinicId"] = null;
            return View(app);
        }




        /// <summary>
        /// This action creates an appoinment for some patient and inserts it into database
        /// executed
        /// </summary>
        /// <param name="app">an appoinment object contains the appoinment details</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(Appoinments app)
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
            if (!RIS.Models.User.hasPerm(userId, Perms.ClinicAppoinmentCreate))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }
            {
                RIS.Models.User loggedUser = RIS.Models.User.getUserByUname(uName);
                //string a = "20000405123012";
                //DateTime d = DateTime.ParseExact("10/19/2017 12:00:00 PM", "MM/dd/yyyy HH:mm:ss tt", CultureInfo.InvariantCulture);
                ViewBag.clinics = GeniricIndex.GetGeniricIndexList(true, "", "CLINIC");

                ViewData["CLINIC"] = GeniricIndex.GetGeniricIndexList(true, "", "CLINIC");

                //app.CLINIC = int.Parse(Request.Form["depId"]);
                app.appID = OracleRIS.GetOracleSequenceValue("APPS_SEQ");
                //app.appDate = DateTime.ParseExact(app.appDate, "MM/dd/yyyy h:mm:ss", CultureInfo.InvariantCulture).ToString();
                app.accNum = app.appID + DateTime.Today.Second;
                app.appStatus = ConnectionConfigs.SCHEDUALED;
                //r.StartDate = DateTime.Now.ToString("yyyyMMddHmmss");
                //r.EndDate = r.StartDate;
                ViewData["Type"] = Appoinments.AppsTypes(app.Type.ToString());
                ViewData["payType"] = Appoinments.AppsPayTypes(app.appPayType.ToString());


                Patient popy = Patient.Select(app.patientID);
                {
                    try
                    {
                        if (ModelState.IsValid)
                        {
                            app.appInsertDate = DateTime.Now;

                            string ex = Models.Appoinments.AddApp(app);
                            Response.AppendToLog("#### ***Rad inserted *** //" + DateTime.Now.ToShortTimeString() + "//" + ex);
                            //if (!string.IsNullOrEmpty(ex))
                            //    ModelState.AddModelError("", ex);
                        }
                    }
                    catch (Exception e)
                    {
                        Response.AppendToLog("#### ***Rad inserted *** //" + DateTime.Now.ToShortTimeString() + "//" + e.Message);
                        return View(app);
                    }
                    return RedirectToAction("Index", "Patient", new { });

                }

            }
        }

        /// <summary>
        /// This action is called when we need to edit some appoinment's details
        /// </summary>
        /// <permission cref="Perms.AppoinmentsEdit">the user has to have the AppoinmentsEdit permission to edit an appoinment</permission>
        /// <param name="apId">the appoinment's ID</param>
        /// <returns>the edit appoinment view</returns>
        public ActionResult Edit(int id)
        {
            ConnectionConfigs risConfig = ConnectionConfigs.getConfig();
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
            if (!RIS.Models.User.hasPerm(userId, Perms.ClinicAppoinmentEdit))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }
            //ViewBag.ClinicList = GeniricIndex.getData("CLINIC");
            Appoinments app = Appoinments.SelectExactApp(id);
            ViewData["Type"] = Appoinments.AppsTypes(app.Type.ToString());
            ViewData["payType"] = Appoinments.AppsPayTypes(app.appPayType.ToString());

            ViewData["CLINIC"] = GeniricIndex.GetGeniricIndexList(true, app.clinicName, "CLINIC");

            string depID = Models.User.getDepID(Session["userName"].ToString());
            app.UpdatetUser = uName;
            app.updateDate = DateTime.Now;
            string s = Session["userType"].ToString();
            ViewData["_Patient"] = app.parentR.firstname + " " + app.parentR.lastname;
            ViewData["_PatientNB"] = app.parentR.givenid;
            return View(app);
        }

        /// <summary>
        /// This action edits the details of an appoinment
        /// </summary>
        /// <param name="app">an appoinment object contains the new details of the appoinment</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(Appoinments app)
        {
            #region user
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
            if (!RIS.Models.User.hasPerm(userId, Perms.ClinicAppoinmentEdit))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }
            #endregion

            {
                Patient popy = Patient.Select(app.patientID);
                string depID = Models.User.getDepID(Session["userName"].ToString()); //get the depId of the user.
                string s = Session["userType"].ToString();
                ViewData["CLINIC"] = GeniricIndex.GetGeniricIndexList(true, app.clinicName, "CLINIC");
                ViewData["Type"] = Appoinments.AppsTypes(app.Type.ToString());
                ViewData["payType"] = Appoinments.AppsPayTypes(app.appPayType.ToString());


                //ViewBag.ClinicList = GeniricIndex.getData("CLINIC");

                if (app.appStatus != ConnectionConfigs.SCHEDUALED)
                {
                    ViewData["PageName"] = RIS.Resources.Res.EditRad.ToString();
                    ViewData["_Patient"] = app.parentR.firstname + " " + app.parentR.lastname;
                    ViewData["_PatientNB"] = app.parentR.givenid;

                    ModelState.AddModelError("", "لا يمكن تعديل الطلب");
                    return View(app);
                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        Appoinments app1 = Appoinments.SelectExactApp(app.appID);
                        string ex = Appoinments.UpdateApp(app);
                        if (string.IsNullOrEmpty(ex))
                        {
                            app1.UpdatetUser = Session["mnmUserId"].ToString();
                            app1.updateDate = DateTime.Now;
                            app1.regStatus = 1;
                            app1.UpdateDeleteReason = app.UpdateDeleteReason;
                            int newID = OracleRIS.GetOracleSequenceValue("OLDAPPS_SEQ");
                            Appoinments.AddToOldApps(app1, newID);
                        }

                        ModelState.AddModelError("", ex);
                    }
                }
                return RedirectToAction("Details", "Appoinments", new { id = app.appID });
            }
        }


        /// <summary>
        /// This action is called when we need to Delete some appoinment's details
        /// </summary>
        /// <permission cref="Perms.AppoinmentsEdit">the user has to have the AppoinmentsDelete permission to edit an appoinment</permission>
        /// <param name="apId">the appoinment's ID</param>
        /// <returns>the Delete appoinment view</returns>
        public ActionResult Delete(int id)
        {
            ConnectionConfigs risConfig = ConnectionConfigs.getConfig();
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
            if (!RIS.Models.User.hasPerm(userId, Perms.ClinicAppoinmentDelete))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }
            Appoinments r = Appoinments.SelectExactApp(id);
            string depID = Models.User.getDepID(Session["userName"].ToString());
            r.UpdatetUser = uName;
            r.updateDate = DateTime.Now;
            string s = Session["userType"].ToString();
            ViewData["_Patient"] = r.parentR.firstname + " " + r.parentR.lastname;
            ViewData["_PatientNB"] = r.parentR.givenid;
            return View(r);
        }

        /// <summary>
        /// This action Deletes the details of an appoinment
        /// </summary>
        /// <param name="app">an appoinment object contains the new details of the appoinment</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            #region user
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
            if (!RIS.Models.User.hasPerm(userId, Perms.ClinicAppoinmentDelete))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }
            #endregion

            {
                Appoinments app = Appoinments.SelectExactApp(id);
                Patient popy = Patient.Select(app.patientID);
                string depID = Models.User.getDepID(Session["userName"].ToString()); //get the depId of the user.
                string s = Session["userType"].ToString();
                ViewBag.ClinicList = GeniricIndex.getData("CLINIC");

                if (app.appStatus != ConnectionConfigs.SCHEDUALED)
                {
                    ViewData["PageName"] = RIS.Resources.Res.EditRad.ToString();
                    ViewData["_Patient"] = app.parentR.firstname + " " + app.parentR.lastname;
                    ViewData["_PatientNB"] = app.parentR.givenid;

                    ModelState.AddModelError("", "لا يمكن حذف الموعد");
                    return View(app);
                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        Appoinments app1 = Appoinments.SelectExactApp(app.appID);

                        string ex = Appoinments.DeleteApp(app);
                        if (string.IsNullOrEmpty(ex))
                        {
                            string reason = collection["reason"].ToString();
                            app1.UpdatetUser = Session["mnmUserId"].ToString();
                            app1.updateDate = DateTime.Now;
                            app1.regStatus = 2;
                            app1.UpdateDeleteReason = app.UpdateDeleteReason;
                            int newId = OracleRIS.GetOracleSequenceValue("OLDAPPS_SEQ");
                            Appoinments.AddToOldApps(app1, newId);
                        }

                        ModelState.AddModelError("", ex);
                    }
                }
                return RedirectToAction("Index", "Appoinments", new { id = app.appID });
            }
        }

        /// <summary>
        /// This action is called when Appoinments webpage is accessed and for paging
        /// </summary>
        /// <permission cref="Perms.ClinicAppoinmentIndex">the user has to have the ClinicAppoinmentIndex permission to access
        /// Clinics Appoinments page</permission>
        /// <param name="page">the page number</param>
        /// <param name="count">total number of pages</param>
        /// <param name="docId">the appoinment's document ID</param>
        /// <param name="firstname">first name of the patient that the appoinment belongs to</param>
        /// <param name="lastname">first name of the patient that the appoinment belongs to</param>
        /// <param name="clinic">the department ID</param>
        /// <returns> Clinic Appoinment Index view</returns>
        public ActionResult IndexAudit([DefaultValue(1.0)] double page, [DefaultValue(1.0)] double count, string firstname, string lastname, int? departments, int? clinic, int? pagging, int? AppStatus, string appDate, int? regStatus)
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

            RIS.Models.User lu = RIS.Models.User.getUserByUname(uName);
            int userId = lu.num;
            int? userClinic = (clinic != null) ? clinic : int.Parse(Session["userClinicId"].ToString());

            ViewData["clinic"] = GeniricIndex.GetClinicIndexList(true, Session["userClinicId"].ToString() + "", "CLINIC");


            if (!RIS.Models.User.hasPerm(userId, Perms.ClinicAppoinmentIndex))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }


            string s = Session["userType"].ToString();
            string d = (s == "1") ? departments.ToString() : Session["userDep"].ToString();

            double RowsPerPage = RequiredValues.getRowsPerPgById((int)ReqPatientVals.rowsPerPage).reqRowsPerPage;

            List<Appoinments> apps = Appoinments.getUpdatedApps(page, out count, RowsPerPage, firstname, lastname, userClinic, AppStatus, appDate, regStatus);

            ViewData["PageCnt"] = count;
            ViewData["count"] = count;
            ViewData["pageNBr"] = page;
            ViewData["page"] = page;
            ViewData["stayInSeach"] = "0";

            //ViewData["departments"] = Departement.GetDepartementListNames(true, "");
            //ViewData["modalities"] = Modality.GetModalitysList(true, "");
            ViewData["ListParameters"] = new { page, count };

            return View(apps);
        }

        /// <summary>
        /// This action is called when the user is doing search in appoinments page
        /// </summary>
        /// <param name="page">the page number</param>
        /// <param name="count">total number of pages</param>
        /// <param name="docId">the appoinment's document ID</param>
        /// <param name="firstname">first name of the patient that the appoinment belongs to</param>
        /// <param name="lastname">first name of the patient that the appoinment belongs to</param>
        /// <param name="clinic">the department ID</param>
        /// <param name="ss">just to defferentiate it from the GET action</param>
        /// <returns> Clinic Appoinments Index view containing the search results</returns>
        [HttpPost]
        public ActionResult IndexAudit([DefaultValue(1.0)] double page, [DefaultValue(1.0)] double count, string firstname, string lastname, string ss, int? clinic, int? AppStatus, string appDate, int? regStatus)
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
            if (!RIS.Models.User.hasPerm(userId, Perms.ClinicAppoinmentIndex))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }
            ViewData["stayInSeach"] = "1";

            string s = Session["userType"].ToString();
            string d = (s == "1") ? "" : Session["userDep"].ToString();

            double RowsPerPage = RequiredValues.getRowsPerPgById((int)ReqPatientVals.rowsPerPage).reqRowsPerPage;
            List<Appoinments> apps = Appoinments.getUpdatedApps(page, out count, RowsPerPage, firstname, lastname, clinic, AppStatus, appDate, regStatus);

            ViewData["firstname"] = (firstname == null) ? "" : firstname;
            ViewData["lastname"] = (lastname == null) ? "" : lastname;

            ViewData["clinic"] = GeniricIndex.GetClinicIndexList(true, Session["userClinicId"].ToString() + "", "CLINIC");



            ViewData["PageCnt"] = count;
            ViewData["count"] = count;
            ViewData["pageNBr"] = page;
            ViewData["page"] = page;
            ViewData["ListParameters"] = new { page, count };

            return View(apps);
        }

        public ActionResult pyramidSearch(List<string> dates, List<string> myShorthours, List<string> myminutes, string clinic)
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
            List<AppSheduleData> sd = new List<AppSheduleData>();
            string step = ((60 / myminutes.Count) - 1).ToString();
            int period = 60 / myminutes.Count;
            int indexStep = (60 / period) * 24;
            for (int i = 0; i < dates.Count; i++)
            {
                sd.Add(new AppSheduleData { appId = "-1", tdId = dates[i] });
            }
            // Now get Days
            List<string> onlyDays = new List<string>();
            int n = 0;
            for (int i = 0; i < dates.Count;)
            {

                string[] temp = dates[i].Split('_');
                bool res_day = Appoinments.getAppsByDay(temp[0]);
                if (res_day)
                {
                    for (int j = 0; j < 24; j++)
                    {
                        bool res_hour = Appoinments.getAppsByHour(temp[0], myShorthours[j]);
                        if (res_hour)
                        {
                            for (int k = 0; k < myminutes.Count; k++)
                            {
                                string appId = Appoinments.getAppsByStep(temp[0], myShorthours[j], myminutes[k], step, clinic);
                                if (appId != "-1")
                                {
                                    sd[(n * indexStep) + j * myminutes.Count + k].appId = appId;
                                }
                            }
                        }
                    }
                }
                i += indexStep;
                n++;

            }



            var js = Json(new { data = sd }, JsonRequestBehavior.AllowGet);
            //JsonResult jj=Json()
            return js;
        }

        public ActionResult previewApp(string id)
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
            PreviewApp p = Appoinments.previewApp(int.Parse(id));
            List<PreviewApp> lp = new List<PreviewApp>();
            lp.Add(p);
            var js = Json(new { data = lp }, JsonRequestBehavior.AllowGet);
            return js;
        }

		[HttpPost]
		public void ChangeStatus(int appId, String appStatus)
		{
			string changeDate = ConnectionConfigs.getPacsTime();
			Appoinments app = Appoinments.SelectExactApp(appId);
			if (app.appStatus == ConnectionConfigs.SCHEDUALED)
				app.appStatus = appStatus;
			app.appDate = System.DateTime.Now.ToString("yyyyMMddH:mm:ss");
			string editedUser = Session["userName"].ToString();
			try
			{
				string ex = Appoinments.UpdateApp(app);
			}
			catch
			{
			}

		}

		//end change status

	}
}
