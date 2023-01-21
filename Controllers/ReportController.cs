using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RIS.Models;
using RISDB;
using System.Text.RegularExpressions;

namespace RIS.Controllers
{
    public class ReportController : Controller
    {

        [HttpGet]
        public ActionResult Create(int radId)
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
            if (!RIS.Models.User.hasPerm(userId, Perms.ReportCreatePerms))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }




            ViewBag.RadId = radId;

            Radiology rad = Radiology.Select(radId);

            Report rep = new Report();

            rep.ORDERNUM = (radId);

            rep.PATIENTNUM = rad.PatientID;
            try
            {
                rep.IMAGEDATE = DateTime.Parse(rad.EndDate);
            }
            catch
            {

            }
            try
            {
                rep.REFERINGPHYSICIANID = Doctor.select(int.Parse(rad.ZDS)).num;
            }
            catch
            {

            }

            rep.REPORTDATE = DateTime.Now;

            rep.DOCTORID = int.Parse(Session["mnmUserId"].ToString());


            return View(rep);
        }

        [HttpPost]
        public ActionResult Create(Report r)
        {
            r.NUM = OracleRIS.GetOracleSequenceValue("REPORT_SEQ");
            r.REPORTDATE = DateTime.Now;
            Report.Insert(r);

            Radiology rad = Radiology.SelectExact(r.ORDERNUM);
            rad.Status = ConnectionConfigs.REPORTED;
            Radiology.UpdateOrderReported(rad);

            return View("Close");
        }

        public ActionResult ReportPartial(Report r)
        {
            return PartialView("ReportPartial", r);
        }

        public ActionResult getPatientRadiologies(int patId)
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
            List<Radiology> ptOrders = Radiology.getPatientsOrders(patId);
            var js = Json(new { data = ptOrders }, JsonRequestBehavior.AllowGet);
            return js; // we return data as JSON to be used in dataTable.
        }

        public ActionResult GetReportByStudyID(int radId)
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
            List<Report> ptOrders = Report.getOrderReport(radId);
            var js = Json(ptOrders, JsonRequestBehavior.AllowGet);
            return js; // we return data as JSON to be used in dataTable.
        }

        public JsonResult getFileToPlay(int repId)
        {

            Report rep = Report.Select(repId);// find the report by Id in the database.

            // construct the path of the file.
            string strMappath1 = Regex.Replace(rep.PATIENTNUM.ToString(), @"[^0-9a-zA-Z]+", "");
            string audioFullPath = ConnectionConfigs.getConfig().audioFilesDirectory + "/" + strMappath1 + "/" + rep.AUDIOPATH;

            // read the data of file.
            byte[] fileBytes = System.IO.File.ReadAllBytes(audioFullPath);
            string aud = Convert.ToBase64String(fileBytes);

            // convert the file to base64
            aud = "data:audio/wav;base64," + aud;


            // return the file as a JSON object
            var jsonResult = Json(aud, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public FileResult downloadFile(int repId)
        {
            Report rep = Report.Select(repId);// find the report by Id in the database.


            // construct the path of the file.
            string strMappath1 = Regex.Replace(rep.PATIENTNUM.ToString(), @"[^0-9a-zA-Z]+", "");
            string audioFullPath = ConnectionConfigs.getConfig().audioFilesDirectory + "/" + strMappath1 + "/" + rep.AUDIOPATH;

            // read the data of file.
            byte[] fileBytes = System.IO.File.ReadAllBytes(audioFullPath);
            string fileName = "requestedFileName.wav";

            // return the file data.
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }


        [HttpGet]
        public ActionResult Edit(int repId)
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
            if (!RIS.Models.User.hasPerm(userId, Perms.ReportEditPerms))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }



            Report rep = Report.Select(repId);

            Radiology rad = Radiology.Select(rep.ORDERNUM);

            ViewBag.RadId = rad.ID;

            rep.PARENTREPORT = repId;

            try
            {
                rep.IMAGEDATE = DateTime.Parse(rad.EndDate);
            }
            catch
            {

            }

            rep.DOCTORID = int.Parse(Session["mnmUserId"].ToString());

            if (rep.AUDIOPATH != null)
            {
                string strMappath1 = Regex.Replace(rep.PATIENTNUM.ToString(), @"[^0-9a-zA-Z]+", "");
                string audioFullPath = ConnectionConfigs.getConfig().audioFilesDirectory + "/" + strMappath1 + "/" + rep.AUDIOPATH;

                // read the data of file.
                byte[] fileBytes = System.IO.File.ReadAllBytes(audioFullPath);
                string aud = Convert.ToBase64String(fileBytes);

                // convert the file to base64
                aud = "data:audio/wav;base64," + aud;
                ViewBag.Audio = aud;
            }


            return View(rep);
        }

        [HttpPost]
        public ActionResult Edit(Report r)
        {
            r.REPORTDATE = DateTime.Now;
            Report.Edit(r);
            return View("Close");
        }

        [HttpGet]
        public ActionResult Details(int repId)
        {


            Report rep = Report.Select(repId);

            Radiology rad = Radiology.Select(rep.ORDERNUM);

            ViewBag.RadId = rad.ID;

            try
            {
                rep.IMAGEDATE = DateTime.Parse(rad.EndDate);
            }
            catch
            {

            }

            rep.DOCTORID = int.Parse(Session["mnmUserId"].ToString());

            if (rep.AUDIOPATH != null)
            {
                string strMappath1 = Regex.Replace(rep.PATIENTNUM.ToString(), @"[^0-9a-zA-Z]+", "");
                string audioFullPath = ConnectionConfigs.getConfig().audioFilesDirectory + "/" + strMappath1 + "/" + rep.AUDIOPATH;

                // read the data of file.
                byte[] fileBytes = System.IO.File.ReadAllBytes(audioFullPath);
                string aud = Convert.ToBase64String(fileBytes);

                // convert the file to base64
                aud = "data:audio/wav;base64," + aud;
                ViewBag.Audio = aud;
            }

            ViewBag.rT = rep.TITLE.Replace("\r\n","<br />");
            ViewBag.rB = rep.REPORTBODY.Replace("\r\n", "<br />");
            ViewBag.rN = rep.NOTES.Replace("\r\n", "<br />");
            return View(rep);
        }


        public ActionResult canAddReport(int radId)
        {
            try
            {
                Radiology r = Radiology.Select(radId);
                if (r.Status == ConnectionConfigs.VIEWED || r.Status==ConnectionConfigs.REPORTED)
                {
                    int userId = int.Parse(Session["mnmUserId"].ToString());
                    User u = RIS.Models.User.select(userId);
                    if (/*u.isDoctor == 1 &&*/ RIS.Models.User.hasPerm(u.num,Perms.ReportCreatePerms))
                    {
                        var data = 0;
                        return Json(data, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var data = 2;
                        return Json(data, JsonRequestBehavior.AllowGet); // no enough permission
                    }
                }
                else
                {
                    var data = 1;
                    return Json(data, JsonRequestBehavior.AllowGet); // image is not previewed
                }

            }
            catch
            {
                return Json(new { data = 1 }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult canPreviewImage(int radId)
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
            try
            {
                Radiology r = Radiology.Select(radId);
                if (r.Status == ConnectionConfigs.VIEWED || r.Status == ConnectionConfigs.REPORTED || r.Status == ConnectionConfigs.COMPLETED)
                {
                    int userId = int.Parse(Session["mnmUserId"].ToString());
                    User u = RIS.Models.User.select(userId);
                //    if (u.isDoctor == 1)
                    {
                        var data = 0;
                        return Json(data, JsonRequestBehavior.AllowGet);
                    }
                    //else
                    //{
                    //    var data = 2;
                    //    return Json(data, JsonRequestBehavior.AllowGet); // no enough permission
                    //}
                }
                else
                {
                    var data = 1;
                    return Json(data, JsonRequestBehavior.AllowGet); // image is not previewed
                }

            }
            catch
            {
                return Json(new { data = 1 }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult canViewReport(int radId)
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
            try
            {
                Radiology r = Radiology.Select(radId);

                int userId = int.Parse(Session["mnmUserId"].ToString());
                User u = RIS.Models.User.select(userId);
                // Check permissions
                if (/*u.isDoctor == 1 &&*/ RIS.Models.User.hasPerm(u.num, Perms.DetailsWithReportsPerms) && r.Status == ConnectionConfigs.REPORTED)
                {
                    var data = 0;
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var data = 1;
                    return Json(data, JsonRequestBehavior.AllowGet); // no enough permission
                }
            }
            catch
            {
                return Json(new { data = 1 }, JsonRequestBehavior.AllowGet);
            }

        }


        public ActionResult DeleteReportByID(string _repId)
        {

            if (Report.Delete(int.Parse(_repId)))
            {
                var repList = "Report Has Been Deleted";
                var js = Json(repList, JsonRequestBehavior.AllowGet);
                //JsonResult jj=Json()
                return js;
            }
            else
            {
                var repList = "Report Has Not Been Deleted";

                var js = Json(repList, JsonRequestBehavior.AllowGet);
                //JsonResult jj=Json()
                return js;
            }
        }

    }


}