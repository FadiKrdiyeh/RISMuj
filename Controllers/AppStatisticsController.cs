using HL7_TCP.Web;
using Microsoft.Reporting.WebForms;
using Oracle.DataAccess.Client;
using RIS.Models;
using RISDB;
using System.Data;
using System.Web.Mvc;

namespace RIS.Controllers
{

    /// <summary>
    /// This Conroller deals with all actions concerned to statistics of patients and Appoinments in RIS
    /// </summary>
    public class AppStatisticsController : Controller
    {


        /// <summary>
        /// This Action is called when apps statistics page is retrieved
        /// </summary>
        /// <permission cref="RIS.Perms"> The user should have the permission Perms.AppStatsIndex to access statistics </permission>
        /// <returns> The statistics index view </returns>
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

            // test if the user has the permission to access statistics
            int userId = RIS.Models.User.getUserByUname(uName).num;
            if (!RIS.Models.User.hasPerm(userId, Perms.AppStatsIndex))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }

            return View();
        }


        /// <summary>
        /// This action is called when apps patient's statistics page is retrieved
        /// </summary>
        /// <permission cref="RIS.Perms"> The user should have the permission Perms.AppStatsPatient to access statistics </permission>
        /// <returns> The patient's statistics view </returns>
        public ActionResult AppPatientStatics()
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

            // test if the user has the permission to access patient's statistics
            int userId = RIS.Models.User.getUserByUname(uName).num;
            if (!RIS.Models.User.hasPerm(userId, Perms.AppStatsPatient))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }

            ViewData["sex"] = "-1";

            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.SizeToReportContent = true;
            reportViewer.ZoomMode = ZoomMode.FullPage;

            ViewBag.clinicList = GeniricIndex.tableListNames("CLINIC");
            //ViewBag.doctorList = GeniricIndex.tableListNames("DOCTORS");
            reportViewer.LocalReport.Refresh();

            ViewBag.ReportViewer = reportViewer;

            ViewBag.selectedClinicList = null;
            //ViewBag.selectedDoctorList = null;
            return View();

        }



        /// <summary>
        /// This action is called when there is a post request for patient's statistics charts.
        /// </summary>
        /// <param name="ps"> a parmeter of type 'PatientStatisticModel' contains the filtering options
        /// defined by the user </param>
        /// <returns> The patient's statistics view with a report contains the patients numbers that meets the user's filtering
        /// options with a chart representing these results </returns>
        [HttpPost]
        public ActionResult AppPatientStatics(StatisticsClasses.PatientStatisticModel ps)
        {

            #region check user and initialize lists
            bool v1 = trialConfigs.checkperiod();
            bool v2 = trialConfigs.getOrderNumbers();
            if (!v1 || !v2)
            {
                return RedirectToAction("Index", "Home");
            }
            try
            {


                string u = Session["userName"].ToString();
            }

            catch
            {
                return RedirectToAction("Index", "Home", new { });
            }

            ViewBag.selectedClinicList = ps.clinics;
            //ViewBag.selectedDoctorList = ps.doctors;

            ReportParameter[] reportParameters = new ReportParameter[7];
            reportParameters[0] = new ReportParameter("PatientNumberParameter", RIS.Resources.Res.PatientsNumber, false);
            reportParameters[3] = new ReportParameter("MaleParameter", RIS.Resources.Res.male, false);
            reportParameters[4] = new ReportParameter("FemaleParameter", RIS.Resources.Res.female, false);
            string lang = Cookies.GetCookieVal("Language");
            string title = "";
            #endregion
            #region language is arabic
            if (lang == "ar")
            {
                if (ps.GroupingItem != null)
                {
                    switch(ps.GroupingItem)
                    {
                        case "Gendre":
                            title += "الإحصائيات حسب الجنس للمرضى";
                            break;
                        case "Age":
                            title += "الإحصائيات اليومية للمرضى";
                            break;
                        case "DID":
                            title += "الإحصائيات اليومية للمرضى";
                            break;
                        case "MID":
                        title += "الإحصائيات الشهرية للمرضى";
                            break;
                        case "YID":
                            title += "الإحصائيات السنوية للمرضى";
                            break;
                        default:
                            break;

                    }
                }

                if (ps.sex == 1)
                {
                    title += " الذكور";
                }
                else if (ps.sex == 0)
                {
                    title += " الإناث";
                }

                if (ps.age > 0 || ps.age < 0)
                {
                    title += " وعمرهم " + ps.age.ToString() + " عاماً";
                }


                if (ps.sBirthDate.Date.ToString("MM-dd-yyyy") != "01-01-0001" || ps.eBirthDate.Date.ToString("MM-dd-yyyy") != "01-01-0001")
                    title += " وتاريخ ولادتهم";

                if (ps.sBirthDate.Date.ToString("MM-dd-yyyy") != "01-01-0001")
                {
                    title += " منذ " + ps.sBirthDate.ToString("yyyy-MM-dd");
                }
                if (ps.eBirthDate.Date.ToString("MM-dd-yyyy") != "01-01-0001")
                {
                    title += " حتى " + ps.eBirthDate.ToString("yyyy-MM-dd");
                }

                title += " المسجلين في نظام تسجيل المرضى في مشفى دمشق";

                if (ps.sInsertDate.Date.ToString("MM-dd-yyyy") != "01-01-0001")
                {
                    title += " ابتداءً من تاريخ " + ps.sInsertDate.ToString("yyyy-MM-dd");
                }
                if (ps.eInsertDate.Date.ToString("MM-dd-yyyy") != "01-01-0001")
                {
                    title += " وحتى تاريخ " + ps.eInsertDate.ToString("yyyy-MM-dd");
                }

                string temp = "";

                if (ps.clinics != null)
                {
                    title += " اللذين تم فحصهم في العيادات\n";

                    string[] clinics = ps.clinics;

                    if (clinics.Length == 1)
                    {
                        temp = clinics[0].TrimStart();
                        temp = temp.TrimEnd();
                        title += GeniricIndex.select(StatisticsClasses.PatientStatisticModel.findID("CLINIC",temp), "CLINIC").name;
                    }

                    else
                    {
                        for (int i = 0; i < clinics.Length - 1; i++)
                        {
                            temp = clinics[i].TrimStart();
                            temp = temp.TrimEnd();
                            title += GeniricIndex.select(StatisticsClasses.PatientStatisticModel.findID("CLINIC", temp), "CLINIC").name + " أو ";
                        }
                        temp = clinics[clinics.Length - 1].TrimStart();
                        temp = temp.TrimEnd();
                        title += GeniricIndex.select(StatisticsClasses.PatientStatisticModel.findID("CLINIC", temp), "CLINIC").name;

                    }

                }
            }
            #endregion
            #region language is english

            else //lang == "En"
            {
                if (ps.GroupingItem != null)
                {
                    switch (ps.GroupingItem)
                    {
                        case "Gendre":
                            title += "Gender Patient Statistics";
                            break;
                        case "Age":
                            title += "Age Patient Statistics";
                            break;
                        case "DID":
                            title += "Daily Patient Statistics";
                            break;
                        case "MID":
                            title += "Monthly Patient Statistics";
                            break;
                        case "YID":
                            title += "Yearly Patient Statistics";
                            break;
                        default:
                            break;

                    }
                }

                if (ps.sex == 1)
                {
                    title += " for male patients";
                }
                else if (ps.sex == 0)
                {
                    title += " For female patients";
                }

                if (ps.age > 0 || ps.age < 0)
                {
                    title += " aged " + ps.age.ToString() + " Years Old";
                }


                if (ps.sBirthDate.Date.ToString("MM-dd-yyyy") != "01-01-0001" || ps.eBirthDate.Date.ToString("MM-dd-yyyy") != "01-01-0001")
                    title += " born";

                if (ps.sBirthDate.Date.ToString("MM-dd-yyyy") != "01-01-0001")
                {
                    title += " since " + ps.sBirthDate.ToString("yyyy-MM-dd");
                }
                if (ps.eBirthDate.Date.ToString("MM-dd-yyyy") != "01-01-0001")
                {
                    title += " untill " + ps.eBirthDate.ToString("yyyy-MM-dd");
                }

                title += " Registered in Damascus Hospital Radiology Information System";

                if (ps.sInsertDate.Date.ToString("MM-dd-yyyy") != "01-01-0001")
                {
                    title += " since " + ps.sInsertDate.ToString("yyyy-MM-dd");
                }
                if (ps.eInsertDate.Date.ToString("MM-dd-yyyy") != "01-01-0001")
                {
                    title += " Untill " + ps.eInsertDate.ToString("yyyy-MM-dd");
                }

                string temp = "";

                if (ps.clinics != null)
                {
                    title += " which has been treated in\n";

                    string[] clinics = ps.clinics;

                    if (clinics.Length == 1)
                    {
                        temp = clinics[0].TrimStart();
                        temp = temp.TrimEnd();
                        title += GeniricIndex.select(StatisticsClasses.PatientStatisticModel.findID("CLINIC", temp), "CLINIC").name;
                    }

                    else
                    {
                        for (int i = 0; i < clinics.Length - 1; i++)
                        {
                            temp = clinics[i].TrimStart();
                            temp = temp.TrimEnd();
                            title += GeniricIndex.select(StatisticsClasses.PatientStatisticModel.findID("CLINIC", temp), "CLINIC").name;
                        }
                        temp = clinics[clinics.Length - 1].TrimStart();
                        temp = temp.TrimEnd();
                        title += GeniricIndex.select(StatisticsClasses.PatientStatisticModel.findID("CLINIC", temp), "CLINIC").name;
                    }

                }
            }
            #endregion
            #region Query
            string appsQuerry = " (SELECT DISTINCT PATIENTID FROM APPOINMENT WHERE 1=1";
            ViewData["sex"] = ps.sex.ToString();
            string tmp = "";
            if (ps.clinics != null)
            {
                appsQuerry += " AND (";

                string[] clinics = ps.clinics;

                if (clinics.Length == 1)
                {
                    tmp = clinics[0].TrimStart();
                    tmp = tmp.TrimEnd();
                    appsQuerry += " APPOINMENT.CLINIC='" + StatisticsClasses.PatientStatisticModel.findID("CLINIC", tmp) + "' )";
                }

                else
                {
                    for (int i = 0; i < clinics.Length - 1; i++)
                    {
                        tmp = clinics[i].TrimStart();
                        tmp = tmp.TrimEnd();
                        appsQuerry += " APPOINMENT.CLINIC='" + StatisticsClasses.PatientStatisticModel.findID("CLINIC",tmp).ToString().Trim() + "' or";
                    }
                    tmp = clinics[clinics.Length - 1].TrimStart();
                    tmp = tmp.TrimEnd();
                    appsQuerry += " APPOINMENT.CLINIC='" + StatisticsClasses.PatientStatisticModel.findID("CLINIC", tmp).ToString().Trim() + "' )";

                }

            }

            appsQuerry += ") ";

            string searchParameter = "";

            if (ps.sex == 1 || ps.sex == 0)
                searchParameter += " AND PATIENT.gendre='" + ps.sex + "'";

            if (ps.age > 0)
                searchParameter += " AND PATIENT.AGE='" + ps.age + "'";


            if (ps.sBirthDate.Date.ToString("MM-dd-yyyy") != "01-01-0001")
                searchParameter += " AND PATIENT.BIRTHDATE >= Date '" + ps.sBirthDate.ToString("yyyy-MM-dd") + "'";
            if (ps.eBirthDate.Date.ToString("MM-dd-yyyy") != "01-01-0001")
                searchParameter += " AND PATIENT.BIRTHDATE <= Date '" + ps.eBirthDate.ToString("yyyy-MM-dd") + "'";

            if (ps.sInsertDate.Date.ToString("MM-dd-yyyy") != "01-01-0001")
                searchParameter += " AND to_date(insertdate,'dd-mm-yy') >= Date '" + ps.sInsertDate.ToString("yyyy-MM-dd") + "'";
            if (ps.eInsertDate.Date.ToString("MM-dd-yyyy") != "01-01-0001")
                searchParameter += " AND to_date(insertdate,'dd-mm-yy') <= Date '" + ps.eInsertDate.ToString("yyyy-MM-dd") + "'";

            string Querry = "";

            if (ps.GroupingItem != null)
            {
                if (ps.GroupingItem == "Gendre")
                {
                    reportParameters[1] = new ReportParameter("GroupingItemParameter", RIS.Resources.Res.PatientGendre, false);
                    reportParameters[2] = new ReportParameter("ChartTitleParameter", RIS.Resources.Res.groupByGendre, false);
                    reportParameters[5] = new ReportParameter("TypeParameter", "1", false);
                    Querry += "SELECT COUNT(NUM) AS PATIENTSNUMBER, GENDRE AS GROUPINGITEM FROM PATIENT WHERE NUM IN" + appsQuerry + searchParameter + " GROUP BY GENDRE";
                }
                if (ps.GroupingItem == "Age")
                {
                    Querry += "SELECT COUNT(NUM) AS PATIENTSNUMBER, AGE AS GROUPINGITEM FROM PATIENT WHERE NUM IN" + appsQuerry + searchParameter + " GROUP BY AGE ORDER BY AGE ASC";
                    reportParameters[1] = new ReportParameter("GroupingItemParameter", RIS.Resources.Res.PatientAge, false);
                    reportParameters[2] = new ReportParameter("ChartTitleParameter", RIS.Resources.Res.groupByAge, false);
                    reportParameters[5] = new ReportParameter("TypeParameter", "2", false);

                }
                if (ps.GroupingItem == "DID")
                {
                    Querry += "SELECT COUNT(NUM) AS PATIENTSNUMBER, to_char(to_date(insertdate,'dd/mm/yy'),'dd-mm-yyyy') AS GROUPINGITEM FROM PATIENT WHERE NUM IN" + appsQuerry + searchParameter + " GROUP BY to_date(insertdate,'dd/mm/yy') ORDER BY to_date(insertdate,'dd/mm/yy') DESC";
                    reportParameters[1] = new ReportParameter("GroupingItemParameter", RIS.Resources.Res.DID, false);
                    reportParameters[2] = new ReportParameter("ChartTitleParameter", RIS.Resources.Res.groupByDailyId, false);
                    reportParameters[5] = new ReportParameter("TypeParameter", "3", false);
                }
                if (ps.GroupingItem == "MID")
                {
                    reportParameters[1] = new ReportParameter("GroupingItemParameter", RIS.Resources.Res.MID, false);
                    reportParameters[2] = new ReportParameter("ChartTitleParameter", RIS.Resources.Res.groupByMonthlyId, false);
                    reportParameters[5] = new ReportParameter("TypeParameter", "4", false);
                    Querry += "SELECT COUNT(NUM) AS PATIENTSNUMBER,CONCAT ( CONCAT( EXTRACT(MONTH FROM to_date(insertdate, 'dd/mm/yy')),'-'),EXTRACT(YEAR FROM to_date(insertdate, 'dd/mm/yy'))) AS GROUPINGITEM " +
                              " FROM PATIENT WHERE NUM IN" + appsQuerry + searchParameter +
                              " GROUP BY EXTRACT(YEAR FROM to_date(insertdate, 'dd/mm/yy'))," +
                              " EXTRACT(MONTH FROM to_date(insertdate, 'dd/mm/yy'))" +
                              " ORDER BY EXTRACT(YEAR FROM to_date(insertdate, 'dd/mm/yy')) DESC," +
                              " EXTRACT(MONTH FROM to_date(insertdate, 'dd/mm/yy')) DESC";
                }
                if (ps.GroupingItem == "YID")
                {
                    reportParameters[1] = new ReportParameter("GroupingItemParameter", RIS.Resources.Res.YID, false);
                    reportParameters[2] = new ReportParameter("ChartTitleParameter", RIS.Resources.Res.groupByYearlyId, false);
                    reportParameters[5] = new ReportParameter("TypeParameter", "5", false);
                    Querry += "SELECT COUNT(NUM) AS PATIENTSNUMBER,EXTRACT(YEAR FROM to_date(insertdate, 'dd/mm/yy')) AS GROUPINGITEM " +
                              " FROM PATIENT WHERE NUM IN" + appsQuerry + searchParameter +
                              " GROUP BY EXTRACT(YEAR FROM to_date(insertdate, 'dd/mm/yy'))" +
                              " ORDER BY EXTRACT(YEAR FROM to_date(insertdate, 'dd/mm/yy')) DESC";
                }
            }
            #endregion
            reportParameters[6] = new ReportParameter("TitleParameter", title, false);

            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.SizeToReportContent = true;
            reportViewer.ZoomMode = ZoomMode.FullPage;

            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());

            OracleDataAdapter adp1 = new OracleDataAdapter(Querry, conn);

            DataTable dt1 = new DataTable();
            adp1.Fill(dt1);
            reportViewer.LocalReport.DataSources.Clear();


            if (ps.GroupingItem != null)
            {
                reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"StatisticalReports\PatientStatsReport.rdlc";

                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("PatientStatsDataSet", dt1));
                reportViewer.LocalReport.SetParameters(reportParameters);
                reportViewer.LocalReport.Refresh();
            }

            ViewBag.ReportViewer = reportViewer;

            ViewBag.clinicList = GeniricIndex.tableListNames("CLINIC");
            ViewBag.modList = Models.Modality.modListNames();

            return View();

        }


        /// <summary>
        /// This action is called when there is a post request for a detailed patient's list.
        /// </summary>
        /// <param name="ps"> a parmeter of type 'PatientStatisticModel' contains the filtering options
        /// defined by the user </param>
        /// <returns> The patient's statistics view with a report contains a detailed list of patients that meets the user's 
        /// filtering options </returns>
        /// <remarks> if there is no filtering options was inserted by the user, a list of all patients in RIS is returned </remarks>
        [HttpPost]
        public ActionResult AppPatientStaticsTable(StatisticsClasses.PatientStatisticModel ps)
        {

            #region check user and initilize variables
            bool v1 = trialConfigs.checkperiod();
            bool v2 = trialConfigs.getOrderNumbers();
            if (!v1 || !v2)
            {
                return RedirectToAction("Index", "Home");
            }
            try
            {


                string u = Session["userName"].ToString();
            }

            catch
            {
                return RedirectToAction("Index", "Home", new { });
            }

            string lang = Cookies.GetCookieVal("Language");
            string title = "";
            string searchParameter = "";
            string appsQuerry = "";
            string tmp = "";
            string Querry = "";
            #endregion
            if (lang == "ar")
            {
                title = "المرضى";

                if (ps.sex == 1)
                {
                    searchParameter += " AND PATIENT.gendre='" + ps.sex + "'";
                    title += " الذكور";
                }
                else if (ps.sex == 0)
                {
                    searchParameter += " AND PATIENT.gendre='" + ps.sex + "'";
                    title += " الإناث";
                }

                if (ps.age > 0 || ps.age < 0)
                {
                    searchParameter += " AND PATIENT.AGE='" + ps.age + "'";
                    title += " وعمرهم " + ps.age.ToString() + " عاماً";
                }


                if (ps.sBirthDate.Date.ToString("MM-dd-yyyy") != "01-01-0001" || ps.eBirthDate.Date.ToString("MM-dd-yyyy") != "01-01-0001")
                    title += " وتاريخ ولادتهم";

                if (ps.sBirthDate.Date.ToString("MM-dd-yyyy") != "01-01-0001")
                {
                    searchParameter += " AND PATIENT.BIRTHDATE >= Date '" + ps.sBirthDate.ToString("yyyy-MM-dd") + "'";
                    title += " منذ " + ps.sBirthDate.ToString("yyyy-MM-dd");
                }
                if (ps.eBirthDate.Date.ToString("MM-dd-yyyy") != "01-01-0001")
                {
                    searchParameter += " AND PATIENT.BIRTHDATE <= Date '" + ps.eBirthDate.ToString("yyyy-MM-dd") + "'";
                    title += " حتى " + ps.eBirthDate.ToString("yyyy-MM-dd");
                }

                title += " المسجلون في نظام تسجيل المرضى في مشفى دمشق";

                if (ps.sInsertDate.Date.ToString("MM-dd-yyyy") != "01-01-0001")
                {
                    searchParameter += " AND to_date(insertdate,'dd-mm-yy') >= Date '" + ps.sInsertDate.ToString("yyyy-MM-dd") + "'";
                    title += " ابتداءً من تاريخ " + ps.sInsertDate.ToString("yyyy-MM-dd");
                }
                if (ps.eInsertDate.Date.ToString("MM-dd-yyyy") != "01-01-0001")
                {
                    searchParameter += " AND to_date(insertdate,'dd-mm-yy') <= Date '" + ps.eInsertDate.ToString("yyyy-MM-dd") + "'";
                    title += " وحتى تاريخ " + ps.eInsertDate.ToString("yyyy-MM-dd");
                }

                appsQuerry = " (SELECT DISTINCT PATIENTID FROM APPOINMENT WHERE 1=1";
                tmp = "";
                if (ps.clinics != null)
                {
                    appsQuerry += " AND (";
                    title += " اللذين تم فحصهم في العيادات\n";

                    string[] clinics = ps.clinics;

                    if (clinics.Length == 1)
                    {
                        tmp = clinics[0].TrimStart();
                        tmp = tmp.TrimEnd();
                        appsQuerry += " APPOINMENT.CLINIC='" + StatisticsClasses.PatientStatisticModel.findID("CLINIC",tmp).ToString() + "' )";
                        title += GeniricIndex.select(StatisticsClasses.PatientStatisticModel.findID("CLINIC",tmp),"CLINIC").name;
                    }

                    else
                    {
                        for (int i = 0; i < clinics.Length - 1; i++)
                        {
                            tmp = clinics[i].TrimStart();
                            tmp = tmp.TrimEnd();
                            appsQuerry += " APPOINMENT.CLINIC='" + StatisticsClasses.PatientStatisticModel.findID("CLINIC", tmp).ToString() + "' or";
                            title += GeniricIndex.select(StatisticsClasses.PatientStatisticModel.findID("CLINIC", tmp), "CLINIC").name+" أو ";
                        }
                        tmp = clinics[clinics.Length - 1].TrimStart();
                        tmp = tmp.TrimEnd();
                        appsQuerry += " APPOINMENT.CLINIC='" + StatisticsClasses.PatientStatisticModel.findID("CLINIC", tmp).ToString() + "' )";
                        title += GeniricIndex.select(StatisticsClasses.PatientStatisticModel.findID("CLINIC", tmp), "CLINIC").name;
                    }

                }
                appsQuerry += ") ";

                title += "\nعدد المرضى : ";

                Querry += "SELECT NUM, ID, GIVENID, FIRSTNAME, MIDDLENAME, LASTNAME, GENDRE, MOTHERNAME, BIRTHDATE, AGE, MOBILEPHONE, LANDPHONE, CURRENTADDRESS, RESIDENTADDRESS, WORKPHONE," +
                         "WORKADDRESS, NEARESTPERSON, NEARESTPERSONPHONE, BIRTHPLACE, NATIONALIDNUMBER, NATIONALITY, WORKTYPE, NOTES, MARTIALSTATUS, TRANSLATEDFNAME, TRANSLATEDLNAME," +
                         "TRANSLATEDFATHERNAME, TRANSLATEDMOTHERNAME, to_char(INSERTDATE, 'dd-mm-yyyy hh24:mi:ss') AS INSERTDATE FROM PATIENT WHERE NUM IN" + appsQuerry + searchParameter + " ORDER BY PATIENT.INSERTDATE DESC";
            }
            else
            {
                if (ps.age > 0 || ps.age < 0)
                {
                    searchParameter += " AND PATIENT.AGE='" + ps.age + "'";
                    title += ps.age.ToString() + " years old ";
                }

                if (ps.sex == 1)
                {
                    searchParameter += " AND PATIENT.gendre='" + ps.sex + "'";
                    title += "Male Patients";
                }
                else if (ps.sex == 0)
                {
                    searchParameter += " AND PATIENT.gendre='" + ps.sex + "'";
                    title += "Female Patients";
                }
                else
                    title += "Patients";


                if (ps.sBirthDate.Date.ToString("MM-dd-yyyy") != "01-01-0001" || ps.eBirthDate.Date.ToString("MM-dd-yyyy") != "01-01-0001")
                    title += " born";

                if (ps.sBirthDate.Date.ToString("MM-dd-yyyy") != "01-01-0001")
                {
                    searchParameter += " AND PATIENT.BIRTHDATE >= Date '" + ps.sBirthDate.ToString("yyyy-MM-dd") + "'";
                    title += " since " + ps.sBirthDate.ToString("yyyy-MM-dd");
                }
                if (ps.eBirthDate.Date.ToString("MM-dd-yyyy") != "01-01-0001")
                {
                    searchParameter += " AND PATIENT.BIRTHDATE <= Date '" + ps.eBirthDate.ToString("yyyy-MM-dd") + "'";
                    title += " untill " + ps.eBirthDate.ToString("yyyy-MM-dd");
                }

                title += " Registered in RIS system in Damascus Hospital";

                if (ps.sInsertDate.Date.ToString("MM-dd-yyyy") != "01-01-0001")
                {
                    searchParameter += " AND to_date(insertdate,'dd-mm-yy') >= Date '" + ps.sInsertDate.ToString("yyyy-MM-dd") + "'";
                    title += " from " + ps.sInsertDate.ToString("yyyy-MM-dd");
                }
                if (ps.eInsertDate.Date.ToString("MM-dd-yyyy") != "01-01-0001")
                {
                    searchParameter += " AND to_date(insertdate,'dd-mm-yy') <= Date '" + ps.eInsertDate.ToString("yyyy-MM-dd") + "'";
                    title += " to " + ps.eInsertDate.ToString("yyyy-MM-dd");
                }

                appsQuerry = " (SELECT DISTINCT PATIENTID FROM APPOINMENT WHERE 1=1";
                tmp = "";
                if (ps.modalities != null || ps.clinics != null)
                    title += " which has been treated";

                if (ps.clinics != null)
                {
                    appsQuerry += " AND (";
                    title += " in\n";

                    string[] clinics = ps.clinics;

                    if (clinics.Length == 1)
                    {
                        tmp = clinics[0].TrimStart();
                        tmp = tmp.TrimEnd();
                        appsQuerry += " APPOINMENT.CLINIC='" + StatisticsClasses.PatientStatisticModel.findID("CLINIC", tmp).ToString() + "' )";
                        title += GeniricIndex.select(StatisticsClasses.PatientStatisticModel.findID("CLINIC", tmp), "CLINIC").name;
                    }

                    else
                    {
                        for (int i = 0; i < clinics.Length - 1; i++)
                        {
                            tmp = clinics[i].TrimStart();
                            tmp = tmp.TrimEnd();
                            appsQuerry += " APPOINMENT.CLINIC='" + StatisticsClasses.PatientStatisticModel.findID("CLINIC", tmp).ToString() + "' )";
                            title += GeniricIndex.select(StatisticsClasses.PatientStatisticModel.findID("CLINIC", tmp), "CLINIC").name;
                        }
                        tmp = clinics[clinics.Length - 1].TrimStart();
                        tmp = tmp.TrimEnd();
                        appsQuerry += " APPOINMENT.CLINIC='" + StatisticsClasses.PatientStatisticModel.findID("CLINIC", tmp).ToString() + "' )";
                        title += GeniricIndex.select(StatisticsClasses.PatientStatisticModel.findID("CLINIC", tmp), "CLINIC").name;
                    }

                }
                appsQuerry += ") ";

                title += "\nNumber of Patients :";

                Querry += "SELECT NUM, ID, GIVENID, FIRSTNAME, MIDDLENAME, LASTNAME, GENDRE, MOTHERNAME, BIRTHDATE, AGE, MOBILEPHONE, LANDPHONE, CURRENTADDRESS, RESIDENTADDRESS, WORKPHONE," +
                         "WORKADDRESS, NEARESTPERSON, NEARESTPERSONPHONE, BIRTHPLACE, NATIONALIDNUMBER, NATIONALITY, WORKTYPE, NOTES, MARTIALSTATUS, TRANSLATEDFNAME, TRANSLATEDLNAME," +
                         "TRANSLATEDFATHERNAME, TRANSLATEDMOTHERNAME, to_char(INSERTDATE, 'dd-mm-yyyy hh24:mi:ss') AS INSERTDATE FROM PATIENT WHERE NUM IN" + appsQuerry + searchParameter + " ORDER BY PATIENT.INSERTDATE DESC";
            }


            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.SizeToReportContent = true;
            reportViewer.ZoomMode = ZoomMode.FullPage;


            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());

            OracleDataAdapter adp1 = new OracleDataAdapter(Querry, conn);

            DataTable dt1 = new DataTable();
            adp1.Fill(dt1);

            title += dt1.Rows.Count.ToString();

            reportViewer.LocalReport.DataSources.Clear();

            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"StatisticalReports\PatientReport.rdlc";


            reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", dt1));
            ReportParameter[] reportParameters = new ReportParameter[10];
            reportParameters[0] = new ReportParameter("IDParameter", RIS.Resources.Res.PatientGivenId, false);
            reportParameters[1] = new ReportParameter("NameParameter", RIS.Resources.Res.PatientFirstname, false);
            reportParameters[2] = new ReportParameter("MotherNameParameter", RIS.Resources.Res.PatientMothername, false);
            reportParameters[3] = new ReportParameter("GenderParameter", RIS.Resources.Res.PatientGendre, false);
            reportParameters[4] = new ReportParameter("BirthDateParameter", RIS.Resources.Res.PatientBirhtDate, false);
            reportParameters[5] = new ReportParameter("AgeParameter", RIS.Resources.Res.PatientAge, false);
            reportParameters[6] = new ReportParameter("InsertDateParameter", RIS.Resources.Res.PatientInsertdate, false);
            reportParameters[7] = new ReportParameter("MaleParameter", RIS.Resources.Res.male, false);
            reportParameters[8] = new ReportParameter("FemaleParameter", RIS.Resources.Res.female, false);
            reportParameters[9] = new ReportParameter("PatientReportTitleParameter", title, false);
            reportViewer.LocalReport.SetParameters(reportParameters);
            reportViewer.LocalReport.Refresh();

            ViewBag.ReportViewer = reportViewer;

            ViewBag.selectedClinicList = ps.clinics;
            ViewBag.selectedModList = ps.modalities;
            ViewData["sex"] = ps.sex.ToString();
            ViewBag.clinicList = GeniricIndex.tableListNames("CLINIC");
            ViewBag.modList = Models.Modality.modListNames();
            ViewData["sex"] = ps.sex.ToString();

            return View("AppPatientStatics");

        }


        /// <summary>
        /// This action is called when appoinments' statistics page is retrieved
        /// </summary>
        /// <permission cref="RIS.Perms"> The user should have the permission Perms.StatsApps to access statistics </permission>
        /// <returns> The appoinments' statistics view </returns>
        public ActionResult AppsStatics()
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
            if (!RIS.Models.User.hasPerm(userId, Perms.StatsApps))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }

            ViewBag.selectedClinicList = null;
            ViewBag.selectedModList = null;
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.SizeToReportContent = true;
            reportViewer.ZoomMode = ZoomMode.FullPage;

            ViewBag.clinicList = GeniricIndex.tableListNames("CLINIC");
            ViewBag.modList = Models.Modality.modListNames();
            ViewBag.procedures = Models.Procedure.GetProcedureCodes(true, "");

            reportViewer.LocalReport.Refresh();

            ViewBag.ReportViewer = reportViewer;


            return View();

        }


        /// <summary>
        /// This action is called when there is a post request for appoinments' statistics charts.
        /// </summary>
        /// <param name="ps"> a parmeter of type 'AppsStatisticModel' contains the filtering options
        /// defined by the user </param>
        /// <returns> The appoinments' statistics view with a report contains the appoinments numbers that meets the user's filtering
        /// options with a chart representing these results </returns>
        [HttpPost]
        public ActionResult AppsStatics(StatisticsClasses.AppsStatisticModel ps)
        {


            bool v1 = trialConfigs.checkperiod();
            bool v2 = trialConfigs.getOrderNumbers();
            if (!v1 || !v2)
            {
                return RedirectToAction("Index", "Home");
            }
            try
            {


                string u = Session["userName"].ToString();
            }

            catch
            {
                return RedirectToAction("Index", "Home", new { });
            }

            ViewBag.selectedClinicList = ps.statsAppClinics;
            //ViewBag.selectedModList = ps.modalities;

            ReportParameter[] reportParameters = new ReportParameter[7];
            reportParameters[0] = new ReportParameter("PatientNumberParameter", RIS.Resources.Res.AppsNumber, false);
            reportParameters[3] = new ReportParameter("MaleParameter", RIS.Resources.Res.male, false);
            reportParameters[4] = new ReportParameter("FemaleParameter", RIS.Resources.Res.female, false);

            string lang = Cookies.GetCookieVal("Language");
            string temp = "";
            string title = "";

            if (lang == "ar")
            {
                if (ps.GroupingItem != null)
                {
                    if (ps.GroupingItem == "DID")
                    {
                        title += "الإحصائيات اليومية";
                    }
                    if (ps.GroupingItem == "MID")
                    {
                        title += "الإحصائيات الشهرية";
                    }
                    if (ps.GroupingItem == "YID")
                    {
                        title += "الإحصائيات السنوية";
                    }
                }

                title += " لمواعيد الفحص المسجلة عن طريق نظام تسجيل المرضى في مشفى دمشق";

                if (ps.statsAppFromDate.Date.ToString("MM-dd-yyyy") != "01-01-0001")
                {
                    title += " ابتداءً من تاريخ " + ps.statsAppFromDate.ToString("yyyy-MM-dd");
                }
                if (ps.statsAppToDate.Date.ToString("MM-dd-yyyy") != "01-01-0001")
                {
                    title += " وانتهاءً بتاريخ " + ps.statsAppToDate.ToString("yyyy-MM-dd");
                }
                //    if (ps.procedures != "" && ps.procedures != null)
                //    {
                //        title += " بوضعية " + Models.Procedure.selectByCode(ps.procedures).name;
                //    }


                if (ps.statsAppClinics != null)
                {
                    title += " في\n";

                    string[] clinics = ps.statsAppClinics;

                    if (clinics.Length == 1)
                    {
                        temp = clinics[0].TrimStart();
                        temp = temp.TrimEnd();
                        title += GeniricIndex.select(StatisticsClasses.PatientStatisticModel.findID("CLINIC", temp), "CLINIC").name;
                    }

                    else
                    {
                        for (int i = 0; i < clinics.Length - 1; i++)
                        {
                            temp = clinics[i].TrimStart();
                            temp = temp.TrimEnd();
                            title += GeniricIndex.select(StatisticsClasses.PatientStatisticModel.findID("CLINIC", temp), "CLINIC").name+"، ";
                        }
                        temp = clinics[clinics.Length - 1].TrimStart();
                        temp = temp.TrimEnd();
                        title += GeniricIndex.select(StatisticsClasses.PatientStatisticModel.findID("CLINIC", temp), "CLINIC").name;

                    }

                }

                //    if (ps.modalities != null)
                //    {
                //        string[] mods = ps.modalities;
                //        if (mods.Length == 1)
                //        {
                //            title += "\nعلى الآلة\n";
                //            temp = mods[0].TrimStart();
                //            temp = temp.TrimEnd();
                //            title += Models.Modality.Select(StatisticsClasses.PatientStatisticModel.findModID(temp)).name;

                //        }
                //        else
                //        {
                //            title += "\nعلى الآلات\n";
                //            for (int i = 0; i < mods.Length - 1; i++)
                //            {
                //                temp = mods[i].TrimStart();
                //                temp = temp.TrimEnd();
                //                title += Models.Modality.Select(StatisticsClasses.PatientStatisticModel.findModID(temp)).name + "، ";
                //            }
                //            temp = mods[mods.Length - 1].TrimStart();
                //            temp = temp.TrimEnd();
                //            title += Models.Modality.Select(StatisticsClasses.PatientStatisticModel.findModID(temp)).name;

                //        }
                //    }

            }
                else
                {
                    if (ps.GroupingItem != null)
                    {
                        if (ps.GroupingItem == "DID")
                        {
                            title += "Daily Statistics";
                        }
                        if (ps.GroupingItem == "MID")
                        {
                            title += "Monthly Statistics";
                        }
                        if (ps.GroupingItem == "YID")
                        {
                            title += "Yearly Statistics";
                        }
                    }

                    title += " for Radiology Appoinments in Damascus hospital radiology information system";

                    if (ps.statsAppFromDate.Date.ToString("MM-dd-yyyy") != "01-01-0001")
                    {
                        title += " since " + ps.statsAppFromDate.ToString("yyyy-MM-dd");
                    }
                    if (ps.statsAppToDate.Date.ToString("MM-dd-yyyy") != "01-01-0001")
                    {
                        title += " untill " + ps.statsAppToDate.ToString("yyyy-MM-dd");
                    }
                //if (ps.procedures != "" && ps.procedures != null)
                //{
                //    title += " under procedure " + Models.Procedure.selectByCode(ps.procedures).name;
                //}


                if (ps.statsAppClinics != null)
                {
                    title += " in\n";

                    string[] clinics = ps.statsAppClinics;

                    if (clinics.Length == 1)
                    {
                        temp = clinics[0].TrimStart();
                        temp = temp.TrimEnd();
                        title += GeniricIndex.select(StatisticsClasses.PatientStatisticModel.findID("CLINIC", temp), "CLINIC").name;
                    }

                    else
                    {
                        for (int i = 0; i < clinics.Length - 1; i++)
                        {
                            temp = clinics[i].TrimStart();
                            temp = temp.TrimEnd();
                            title += GeniricIndex.select(StatisticsClasses.PatientStatisticModel.findID("CLINIC", temp), "CLINIC").name + "، ";
                        }
                        temp = clinics[clinics.Length - 1].TrimStart();
                        temp = temp.TrimEnd();
                        title += GeniricIndex.select(StatisticsClasses.PatientStatisticModel.findID("CLINIC", temp), "CLINIC").name;

                    }

                }

                //    if (ps.modalities != null)
                //    {
                //        string[] mods = ps.modalities;
                //        if (mods.Length == 1)
                //        {
                //            title += "\nom modality\n";
                //            temp = mods[0].TrimStart();
                //            temp = temp.TrimEnd();
                //            title += Models.Modality.Select(StatisticsClasses.PatientStatisticModel.findModID(temp)).name;

                //        }
                //        else
                //        {
                //            title += "\non modalities\n";
                //            for (int i = 0; i < mods.Length - 1; i++)
                //            {
                //                temp = mods[i].TrimStart();
                //                temp = temp.TrimEnd();
                //                title += Models.Modality.Select(StatisticsClasses.PatientStatisticModel.findModID(temp)).name + "، ";
                //            }
                //            temp = mods[mods.Length - 1].TrimStart();
                //            temp = temp.TrimEnd();
                //            title += Models.Modality.Select(StatisticsClasses.PatientStatisticModel.findModID(temp)).name;

                //        }
                //    }
            }

                    string searchParameter = "";

                string tmp = "";
            if (ps.statsAppClinics != null)
            {
                searchParameter += " AND (";

                string[] clinics = ps.statsAppClinics;

                if (clinics.Length == 1)
                {
                    tmp = clinics[0].TrimStart();
                    tmp = tmp.TrimEnd();
                    searchParameter += " APPOINMENT.CLINIC='" + StatisticsClasses.PatientStatisticModel.findID("CLINIC", tmp) + "' )";
                }

                else
                {
                    for (int i = 0; i < clinics.Length - 1; i++)
                    {
                        tmp = clinics[i].TrimStart();
                        tmp = tmp.TrimEnd();
                        searchParameter += " APPOINMENT.CLINIC='" + StatisticsClasses.PatientStatisticModel.findID("CLINIC", tmp).ToString().Trim() + "' or";
                    }
                    tmp = clinics[clinics.Length - 1].TrimStart();
                    tmp = tmp.TrimEnd();
                    searchParameter += " APPOINMENT.CLINIC='" + StatisticsClasses.PatientStatisticModel.findID("CLINIC", tmp).ToString().Trim() + "' )";

                }

            }





            if (ps.statsAppFromDate.Date.ToString("MM-dd-yyyy") != "01-01-0001")
                searchParameter += " AND to_date(to_date(APPOINMENT.appdate,'yyyymmddhh24:mi:ss'),'dd-mm-yy') >= Date '" + ps.statsAppFromDate.ToString("yyyy-MM-dd") + "'";
            if (ps.statsAppToDate.Date.ToString("MM-dd-yyyy") != "01-01-0001")
                searchParameter += " AND to_date(to_date(APPOINMENT.appdate,'yyyymmddhh24:mi:ss'),'dd-mm-yy') <= Date '" + ps.statsAppToDate.ToString("yyyy-MM-dd") + "'";




            string Querry = "";

            if (ps.GroupingItem != null)
            {
                if (ps.GroupingItem == "DID")
                {
                    Querry += "SELECT COUNT(APPID) AS PATIENTSNUMBER, to_char(to_date(to_date(appdate,'yyyymmddhh24:mi:ss'),'dd/mm/yy'),'dd-mm-yyyy') AS GROUPINGITEM FROM APPOINMENT WHERE APPOINMENT.STATUS != " + ConnectionConfigs.Waiting + " " + searchParameter + " GROUP BY to_date(to_date(appdate,'yyyymmddhh24:mi:ss'),'dd/mm/yy') ORDER BY to_date(to_date(appdate,'yyyymmddhh24:mi:ss'),'dd/mm/yy') DESC";
                    reportParameters[1] = new ReportParameter("GroupingItemParameter", RIS.Resources.Res.DID, false);
                    reportParameters[2] = new ReportParameter("ChartTitleParameter", RIS.Resources.Res.groupByDailyId, false);
                    reportParameters[5] = new ReportParameter("TypeParameter", "3", false);
                }
                if (ps.GroupingItem == "MID")
                {
                    reportParameters[1] = new ReportParameter("GroupingItemParameter", RIS.Resources.Res.MID, false);
                    reportParameters[2] = new ReportParameter("ChartTitleParameter", RIS.Resources.Res.groupByMonthlyId, false);
                    reportParameters[5] = new ReportParameter("TypeParameter", "4", false);
                    Querry += "SELECT COUNT(APPID) AS PATIENTSNUMBER,CONCAT ( CONCAT( EXTRACT(MONTH FROM to_date(to_date(appdate,'yyyymmddhh24:mi:ss'),'dd/mm/yy')),'-'),EXTRACT(YEAR FROM to_date(to_date(appdate,'yyyymmddhh24:mi:ss'),'dd/mm/yy'))) AS GROUPINGITEM " +
                                " FROM APPOINMENT WHERE APPOINMENT.STATUS != " + ConnectionConfigs.Waiting + " " + searchParameter +
                                " GROUP BY EXTRACT(YEAR FROM to_date(to_date(appdate, 'yyyymmddhh24:mi:ss'), 'dd/mm/yy'))," +
                                " EXTRACT(MONTH FROM to_date(to_date(appdate, 'yyyymmddhh24:mi:ss'), 'dd/mm/yy'))" +
                                " ORDER BY EXTRACT(YEAR FROM to_date(to_date(appdate, 'yyyymmddhh24:mi:ss'), 'dd/mm/yy')) DESC," +
                                " EXTRACT(MONTH FROM to_date(to_date(appdate, 'yyyymmddhh24:mi:ss'), 'dd/mm/yy')) DESC";
                }
                if (ps.GroupingItem == "YID")
                {
                    reportParameters[1] = new ReportParameter("GroupingItemParameter", RIS.Resources.Res.YID, false);
                    reportParameters[2] = new ReportParameter("ChartTitleParameter", RIS.Resources.Res.groupByYearlyId, false);
                    reportParameters[5] = new ReportParameter("TypeParameter", "5", false);
                    Querry += "SELECT COUNT(APPID) AS PATIENTSNUMBER,EXTRACT(YEAR FROM to_date(to_date(appdate,'yyyymmddhh24:mi:ss'),'dd/mm/yy')) AS GROUPINGITEM" +
                              " FROM APPOINMENT WHERE APPOINMENT.STATUS != " + ConnectionConfigs.Waiting + " " + searchParameter +
                              " GROUP BY EXTRACT(YEAR FROM to_date(to_date(appdate, 'yyyymmddhh24:mi:ss'), 'dd/mm/yy'))" +
                              " ORDER BY EXTRACT(YEAR FROM to_date(to_date(appdate, 'yyyymmddhh24:mi:ss'), 'dd/mm/yy')) DESC";
                }
            }

            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.SizeToReportContent = true;
            reportViewer.ZoomMode = ZoomMode.FullPage;

            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());

            OracleDataAdapter adp1 = new OracleDataAdapter(Querry, conn);

            DataTable dt1 = new DataTable();
            adp1.Fill(dt1);
            reportViewer.LocalReport.DataSources.Clear();

            reportParameters[6] = new ReportParameter("TitleParameter", title, false);


            if (ps.GroupingItem != null)
            {
                reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"StatisticalReports\PatientStatsReport.rdlc";

                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("PatientStatsDataSet", dt1));
                reportViewer.LocalReport.SetParameters(reportParameters);
                reportViewer.LocalReport.Refresh();
            }



            ViewBag.ReportViewer = reportViewer;

            ViewBag.clinicList = GeniricIndex.tableListNames("CLINIC");
            ViewBag.modList = Models.Modality.modListNames();
            ViewBag.procedures = Models.Procedure.GetProcedureCodes(true, "");

            return View();

        }


        /// <summary>
        /// This action is called when there is a post request for a detailed appoinments' list.
        /// </summary>
        /// <param name="ps"> a parmeter of type 'AppsStatisticModel' contains the filtering options
        /// defined by the user </param>
        /// <returns> The apps' statistics view with a report contains a detailed list of APPOINMENTs that meets the user's 
        /// filtering options </returns>
        /// <remarks> if there is no filtering options was inserted by the user, a list of all APPOINMENTs in RIS is returned </remarks>
        [HttpPost]
        public ActionResult AppsStaticsTable(StatisticsClasses.AppsStatisticModel ps)
        {


            bool v1 = trialConfigs.checkperiod();
            bool v2 = trialConfigs.getOrderNumbers();
            if (!v1 || !v2)
            {
                return RedirectToAction("Index", "Home");
            }
            try
            {


                string u = Session["userName"].ToString();
            }

            catch
            {
                return RedirectToAction("Index", "Home", new { });
            }

            //ViewBag.selectedClinicList = ps.clinics;
            //ViewBag.selectedModList = ps.modalities;

            string lang = Cookies.GetCookieVal("Language");
            string searchParameter = "";
            string tmp = "";
            string title = "";

            if (lang == "ar")
            {
                title += " مواعيد الفحص المسجلة عن طريق نظام تسجيل المرضى في مشفى دمشق";

                if (ps.statsAppFromDate.Date.ToString("MM-dd-yyyy") != "01-01-0001")
                {
                    searchParameter += " AND to_date(to_date(APPOINMENT.appdate,'yyyymmddhh24:mi:ss'),'dd-mm-yy') >= Date '" + ps.statsAppFromDate.ToString("yyyy-MM-dd") + "'";
                    title += " ابتداءً من تاريخ " + ps.statsAppFromDate.ToString("yyyy-MM-dd");
                }
                if (ps.statsAppToDate.Date.ToString("MM-dd-yyyy") != "01-01-0001")
                {
                    searchParameter += " AND to_date(to_date(APPOINMENT.appdate,'yyyymmddhh24:mi:ss'),'dd-mm-yy') <= Date '" + ps.statsAppToDate.ToString("yyyy-MM-dd") + "'";
                    title += " وانتهاءً بتاريخ " + ps.statsAppToDate.ToString("yyyy-MM-dd");
                }
                if (ps.appPayType != 0)
                {
                    searchParameter += " AND APPOINMENT.PAYTYPE = " + ps.appPayType;
                }

                if (ps.statsAppClinics != null)
                {
                    searchParameter += " AND (";
                    title += " في\n";

                    string[] clinics = ps.statsAppClinics;

                    if (clinics.Length == 1)
                    {
                        tmp = clinics[0].TrimStart();
                        tmp = tmp.TrimEnd();
                        searchParameter += " APPOINMENT.CLINIC='" + StatisticsClasses.PatientStatisticModel.findID("CLINIC", tmp) + "' )";
                        title += GeniricIndex.select(StatisticsClasses.PatientStatisticModel.findID("CLINIC", tmp), "CLINIC").name;
                    }

                    else
                    {
                        for (int i = 0; i < clinics.Length - 1; i++)
                        {
                            tmp = clinics[i].TrimStart();
                            tmp = tmp.TrimEnd();
                            searchParameter += " APPOINMENT.CLINIC='" + StatisticsClasses.PatientStatisticModel.findID("CLINIC", tmp).ToString().Trim() + "' or";
                            title += GeniricIndex.select(StatisticsClasses.PatientStatisticModel.findID("CLINIC", tmp), "CLINIC").name + "، ";
                        }
                        tmp = clinics[clinics.Length - 1].TrimStart();
                        tmp = tmp.TrimEnd();
                        searchParameter += " APPOINMENT.CLINIC='" + StatisticsClasses.PatientStatisticModel.findDepID(tmp).ToString().Trim() + "' )";
                        title += GeniricIndex.select(StatisticsClasses.PatientStatisticModel.findID("CLINIC", tmp), "CLINIC").name;

                    }

                }

                //if (ps.modalities != null)
                //{
                //    searchParameter += " AND (";

                //    string[] mods = ps.modalities;
                //    if (mods.Length == 1)
                //    {
                //        title += "\nعلى الآلة\n";
                //        tmp = mods[0].TrimStart();
                //        tmp = tmp.TrimEnd();
                //        searchParameter += " APPOINMENT.modalityid='" + StatisticsClasses.PatientStatisticModel.findModID(tmp).ToString() + "' )";
                //        title += Models.Modality.Select(StatisticsClasses.PatientStatisticModel.findModID(tmp)).name;

                //    }
                //    else
                //    {
                //        title += "\nعلى الآلات\n";
                //        for (int i = 0; i < mods.Length - 1; i++)
                //        {
                //            tmp = mods[i].TrimStart();
                //            tmp = tmp.TrimEnd();
                //            searchParameter += " APPOINMENT.modalityid='" + StatisticsClasses.PatientStatisticModel.findModID(tmp).ToString() + "' or";
                //            title += Models.Modality.Select(StatisticsClasses.PatientStatisticModel.findModID(tmp)).name + "، ";
                //        }
                //        tmp = mods[mods.Length - 1].TrimStart();
                //        tmp = tmp.TrimEnd();
                //        searchParameter += " APPOINMENT.modalityid='" + StatisticsClasses.PatientStatisticModel.findModID(tmp).ToString() + "' )";
                //        title += Models.Modality.Select(StatisticsClasses.PatientStatisticModel.findModID(tmp)).name;

                //    }
                //}

                title += "\nعدد الفحوصات : ";
            }
            else
            {
                title += "Appoinments Registered via Hospital Information System in Damascus Hospital ";

                if (ps.statsAppFromDate.Date.ToString("MM-dd-yyyy") != "01-01-0001")
                {
                    searchParameter += " AND to_date(to_date(APPOINMENT.appdate,'yyyymmddhh24:mi:ss'),'dd-mm-yy') >= Date '" + ps.statsAppFromDate.ToString("yyyy-MM-dd") + "'";
                    title += " since " + ps.statsAppFromDate.ToString("yyyy-MM-dd");
                }
                if (ps.statsAppToDate.Date.ToString("MM-dd-yyyy") != "01-01-0001")
                {
                    searchParameter += " AND to_date(to_date(APPOINMENT.appdate,'yyyymmddhh24:mi:ss'),'dd-mm-yy') <= Date '" + ps.statsAppToDate.ToString("yyyy-MM-dd") + "'";
                    title += " untill " + ps.statsAppToDate.ToString("yyyy-MM-dd");
                }
                //if (ps.procedures != "" && ps.procedures != null)
                //{
                //    searchParameter += " AND APPOINMENT.procedureid = " + ps.procedures;
                //    title += " Under Procedure " + Models.Procedure.selectByCode(ps.procedures).name;
                //}

                if (ps.appPayType != 0)
                {
                    searchParameter += " AND APPOINMENT.PAYTYPE = " + ps.appPayType;
                }

                if (ps.statsAppClinics != null)
                {
                    searchParameter += " AND (";

                    string[] clinics = ps.statsAppClinics;

                    if (clinics.Length == 1)
                    {
                        tmp = clinics[0].TrimStart();
                        tmp = tmp.TrimEnd();
                        searchParameter += " APPOINMENT.CLINIC='" + StatisticsClasses.PatientStatisticModel.findID("CLINIC", tmp) + "' )";
                    }

                    else
                    {
                        for (int i = 0; i < clinics.Length - 1; i++)
                        {
                            tmp = clinics[i].TrimStart();
                            tmp = tmp.TrimEnd();
                            searchParameter += " APPOINMENT.CLINIC='" + StatisticsClasses.PatientStatisticModel.findID("CLINIC", tmp).ToString().Trim() + "' or";
                        }
                        tmp = clinics[clinics.Length - 1].TrimStart();
                        tmp = tmp.TrimEnd();
                        searchParameter += " APPOINMENT.CLINIC='" + StatisticsClasses.PatientStatisticModel.findID("CLINIC", tmp).ToString().Trim() + "' )";

                    }

                }

                //if (ps.modalities != null)
                //{
                //    searchParameter += " AND (";

                //    string[] mods = ps.modalities;
                //    if (mods.Length == 1)
                //    {
                //        title += "\non modality\n";
                //        tmp = mods[0].TrimStart();
                //        tmp = tmp.TrimEnd();
                //        searchParameter += " APPOINMENT.modalityid='" + StatisticsClasses.PatientStatisticModel.findModID(tmp).ToString() + "' )";
                //        title += Models.Modality.Select(StatisticsClasses.PatientStatisticModel.findModID(tmp)).name;

                //    }
                //    else
                //    {
                //        title += "\non modalities\n";
                //        for (int i = 0; i < mods.Length - 1; i++)
                //        {
                //            tmp = mods[i].TrimStart();
                //            tmp = tmp.TrimEnd();
                //            searchParameter += " APPOINMENT.modalityid='" + StatisticsClasses.PatientStatisticModel.findModID(tmp).ToString() + "' or";
                //            title += Models.Modality.Select(StatisticsClasses.PatientStatisticModel.findModID(tmp)).name + "، ";
                //        }
                //        tmp = mods[mods.Length - 1].TrimStart();
                //        tmp = tmp.TrimEnd();
                //        searchParameter += " APPOINMENT.modalityid='" + StatisticsClasses.PatientStatisticModel.findModID(tmp).ToString() + "' )";
                //        title += Models.Modality.Select(StatisticsClasses.PatientStatisticModel.findModID(tmp)).name;

                //    }
                //}

                title += "\nNumber Of Appoinments : ";
            }


            string Querry = "SELECT PATIENT.FIRSTNAME, PATIENT.MIDDLENAME, PATIENT.LASTNAME, to_char(to_date(APPOINMENT.APPDATE, 'yyyymmddhh24:mi:ss'), 'dd-mm-yyyy hh24:mi:ss') AS STARTDATE, CLINIC.NAME AS DNAME, to_char(to_date(APPOINMENT.appdate, 'yyyymmddhh24:mi:ss'), 'dd-mm-yyyy hh24:mi:ss') AS ENDDATE, APPID AS DOCID, DOCTORNAME AS MNAME, DOCTORNAME AS PNAME FROM PATIENT, APPOINMENT, CLINIC WHERE APPOINMENT.STATUS != 0 AND PATIENT.NUM = APPOINMENT.PATIENTID AND APPOINMENT.CLINIC = CLINIC.NUM"
                + searchParameter + " order by to_date(APPOINMENT.appdate,'yyyymmddhh24:mi:ss') DESC";


            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.SizeToReportContent = true;
            reportViewer.ZoomMode = ZoomMode.FullPage;


            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());

            OracleDataAdapter adp1 = new OracleDataAdapter(Querry, conn);

            DataTable dt1 = new DataTable();

            adp1.Fill(dt1);

            title += dt1.Rows.Count.ToString();

            reportViewer.LocalReport.DataSources.Clear();

            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"StatisticalReports\OrdersReport.rdlc";


            reportViewer.LocalReport.DataSources.Add(new ReportDataSource("OrdersDataSet", dt1));
            ReportParameter[] reportParameters = new ReportParameter[8];
            reportParameters[0] = new ReportParameter("DOCIDParameter", RIS.Resources.Res.DOCIDParameter, false);
            reportParameters[1] = new ReportParameter("PATIENTNAMEParameter", RIS.Resources.Res.PATIENTNAMEParameter, false);
            reportParameters[2] = new ReportParameter("MODALITYNAMEParameter", RIS.Resources.Res.DoctorName, false);
            reportParameters[3] = new ReportParameter("DEPTNAMEParameter", RIS.Resources.Res.CLINICParameter, false);
            reportParameters[4] = new ReportParameter("STARTDATEParameter", RIS.Resources.Res.STARTDATEParameter, false);
            reportParameters[5] = new ReportParameter("ENDDATEParameter", RIS.Resources.Res.OrderEndDate, false);
            reportParameters[6] = new ReportParameter("TitleParameter", title, false);
            reportParameters[7] = new ReportParameter("ProcNameParameter", RIS.Resources.Res.Diagnosis, false);
            reportViewer.LocalReport.SetParameters(reportParameters);
            reportViewer.LocalReport.Refresh();

            ViewBag.ReportViewer = reportViewer;

            ViewBag.clinicList = GeniricIndex.tableListNames("CLINIC");
            ViewBag.modList = Models.Modality.modListNames();
            ViewBag.procedures = Models.Procedure.GetProcedureCodes(true, "");

            return View("AppsStatics");
        }

    }
}