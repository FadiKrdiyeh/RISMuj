using RIS.Models;
using RIS.ViewModels;
using RISDB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RIS.Controllers
{
    public class BillsController : Controller
    {
        [HttpGet]
        public ActionResult Test(int patientId)
        {
            return View();
        }
        public ActionResult TestBarcode()
        {
            return View();
        }
        // GET: Bills
        // GET: Appoinments
        /// <summary>
        /// This action is called when Appoinments webpage is accessed and for paging
        /// </summary>
        /// <permission cref="Perms.BillsIndex">the user has to have the BillsIndex permission to access
        /// Clinics Appoinments page</permission>
        /// <param name="page">the page number</param>
        /// <param name="count">total number of pages</param>
        /// <param name="patientId">id of the patient that the bill belongs to</param>
        /// <returns> Clinic Appoinment Index view</returns>
        public ActionResult Index([DefaultValue(1.0)] double page, [DefaultValue(1.0)] double count, int patientId, int? pagging, int? billStatus, string billDate)
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

            if (!RIS.Models.User.hasPerm(userId, Perms.BillsIndex))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }


            string s = Session["userType"].ToString();
            double RowsPerPage = RequiredValues.getRowsPerPgById((int)ReqPatientVals.rowsPerPage).reqRowsPerPage;

            List<Bills> bills = Bills.getBillItemsByPatient(page, out count, RowsPerPage, patientId, billStatus, billDate);

            ViewData["PageCnt"] = count;
            ViewData["count"] = count;
            ViewData["pageNBr"] = page;
            ViewData["page"] = page;
            ViewData["stayInSeach"] = "0";
            ViewData["ListParameters"] = new { page, count };

            return View(bills);
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
        public ActionResult Index([DefaultValue(1.0)] double page, [DefaultValue(1.0)] double count, int patientId, int? billStatus, string billDate)
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
            if (!RIS.Models.User.hasPerm(userId, Perms.BillsIndex))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }
            ViewData["stayInSeach"] = "1";

            string s = Session["userType"].ToString();
            string d = (s == "1") ? "" : Session["userDep"].ToString();

            double RowsPerPage = RequiredValues.getRowsPerPgById((int)ReqPatientVals.rowsPerPage).reqRowsPerPage;
            List<Bills> apps = Bills.getBillItemsByPatient(page, out count, RowsPerPage, patientId, billStatus, billDate);
            ViewData["clinic"] = GeniricIndex.GetClinicIndexList(true, Session["userClinicId"].ToString() + "", "CLINIC");

            ViewData["PageCnt"] = count;
            ViewData["count"] = count;
            ViewData["pageNBr"] = page;
            ViewData["page"] = page;
            ViewData["ListParameters"] = new { page, count };

            return View(apps);
        }


        // GET: Bills/Details/5
        public ActionResult Details(int id)
        {
            //Bills a = getBillItemsByPatient(id);

            return View();
        }

        [HttpGet]
        public ActionResult Create(int patientId)
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
            if (!RIS.Models.User.hasPerm(userId, Perms.BillsCreate))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }

            ViewBag.RadId = patientId;
            string date = ConnectionConfigs.getPacsTime();
            string appdate = date.Substring(0, 8) + "00:00:00";
            Patient pt = Patient.Select(patientId);
            List<Radiology> rads = Radiology.getCompletedPatientsOrdersByDate(patientId, date);
            List<Appoinments> apps = Appoinments.getPatientsAppsByDate(patientId, appdate);

            Bills bill = new Bills();
            bill.billInsertDate = DateTime.Now;
            bill.ptDetails = new PatientDetails(pt, rads, apps);
            bill.billValue = Bills.calculateBillValue(bill);
            bill.taxValue = Patient.getTaxByPatientAccType(pt.acceptanceType);
            bill.billTotValue = bill.billValue * bill.taxValue / 100;
            bill.accTypeName = Patient.getAccTypeName(pt.acceptanceType);
            return View(bill);
        }
        // searched
        [HttpPost]
        public ActionResult Create(int patientId, string fromDate, string toDate, int? i)
        {
            //fromDate = (fromDate != "" ): fromDate? getPacsTime;
            Patient pt = Patient.Select(patientId);
            fromDate = (fromDate != "") ? fromDate : "1999010100:00:00";
            toDate = (toDate != "") ? toDate : "2222123100:00:00";
            List<Radiology> rads = Radiology.getCompletedPatientsOrdersBetweenDates(patientId, fromDate.Substring(0, 8) + "000000", toDate.Substring(0, 8) + "000000");

            List<Appoinments> apps = Appoinments.getPatientsAppsBetweenDates(patientId, fromDate, toDate);

            Bills bill = new Bills();
            bill.patientID = patientId;
            bill.billInsertDate = DateTime.Now;
            bill.ptDetails = new PatientDetails(pt, rads, apps);
            bill.billValue = Bills.calculateBillValue(bill);
            bill.taxValue = Patient.getTaxByPatientAccType(pt.acceptanceType);
            bill.billTotValue = bill.billValue * bill.taxValue / 100;
            bill.accTypeName = Patient.getAccTypeName(pt.acceptanceType);
            if (i == 0)
                return View(bill);
            else
            {
                int a = int.Parse(Request.Form["billAddCosts"]);
                bill.billTotValue = bill.billTotValue + a;
                bill.billId = OracleRIS.GetOracleSequenceValue("BILLS_SEQ");
                Bills.Insert(bill);
                return RedirectToAction("Create", "Bills", new { patientId = bill.patientID});
            }
        }


        public ActionResult getPatientApps(int patientId)
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
            string fromDate = "1999010100:00:00";
            string toDate = "2222123100:00:00";
            //fromDate = (fromDate != "") ? fromDate : "1999010100:00:00";
            //toDate = (toDate != "") ? toDate : "2222123100:00:00";
            List<Appoinments> ptOrders = Appoinments.getPatientsAppsBetweenDates(patientId, fromDate, toDate);
            var js = Json(new { data = ptOrders }, JsonRequestBehavior.AllowGet);
            return js; // we return data as JSON to be used in dataTable.
        }

        // GET: Bills/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Bills/Edit/5
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

        // GET: Bills/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Bills/Delete/5
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
