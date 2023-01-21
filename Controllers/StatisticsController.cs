using HL7_TCP.Web;
using Microsoft.Reporting.WebForms;
using Oracle.DataAccess.Client;
using RISDB;
using System.Data;
using System.Web.Mvc;

namespace RIS.Controllers
{

    /// <summary>
    /// This Conroller deals with all actions concerned to statistics of patients and orders in RIS
    /// </summary>
    public class StatisticsController : Controller
    {


        /// <summary>
        /// This Action is called when statistics page is retrieved
        /// </summary>
        /// <permission cref="RIS.Perms"> The user should have the permission Perms.StatsIndex to access statistics </permission>
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
            if (!RIS.Models.User.hasPerm(userId, Perms.StatsIndex))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }

            return View();
        }


        /// <summary>
        /// This action is called when patient's statistics page is retrieved
        /// </summary>
        /// <permission cref="RIS.Perms"> The user should have the permission Perms.StatsPatient to access statistics </permission>
        /// <returns> The patient's statistics view </returns>
        public ActionResult PatientStatics()
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
            if (!RIS.Models.User.hasPerm(userId, Perms.StatsPatient))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }

            ViewData["sex"] = "-1";

            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.SizeToReportContent = true;
            reportViewer.ZoomMode = ZoomMode.FullPage;

            ViewBag.depList = Models.Departement.depListNames();
            ViewBag.modList = Models.Modality.modListNames();

            reportViewer.LocalReport.Refresh();

            ViewBag.ReportViewer = reportViewer;

            ViewBag.selectedDepList = null;
            ViewBag.selectedModList = null;
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
        public ActionResult PatientStatics(StatisticsClasses.PatientStatisticModel ps)
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

            ViewBag.selectedDepList = ps.departements;
            ViewBag.selectedModList = ps.modalities;

            ReportParameter[] reportParameters = new ReportParameter[7];
            reportParameters[0] = new ReportParameter("PatientNumberParameter", RIS.Resources.Res.PatientsNumber, false);
            reportParameters[3] = new ReportParameter("MaleParameter", RIS.Resources.Res.male, false);
            reportParameters[4] = new ReportParameter("FemaleParameter", RIS.Resources.Res.female, false);

            string lang = Cookies.GetCookieVal("Language");
            string title = "";
            if (lang == "ar")
            {
                if (ps.GroupingItem != null)
                {
                    if (ps.GroupingItem == "Gendre")
                    {
                        title += "الإحصائيات حسب الجنس للمرضى";
                    }
                    if (ps.GroupingItem == "Age")
                    {
                        title += "الإحصائيات العمرية للمرضى";

                    }
                    if (ps.GroupingItem == "DID")
                    {
                        title += "الإحصائيات اليومية للمرضى";
                    }
                    if (ps.GroupingItem == "MID")
                    {
                        title += "الإحصائيات الشهرية للمرضى";
                    }
                    if (ps.GroupingItem == "YID")
                    {
                        title += "الإحصائيات السنوية للمرضى";
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

                if (ps.departements != null)
                {
                    title += " المصورين في\n";

                    string[] deps = ps.departements;

                    if (deps.Length == 1)
                    {
                        temp = deps[0].TrimStart();
                        temp = temp.TrimEnd();
                        title += Models.Departement.select(StatisticsClasses.PatientStatisticModel.findDepID(temp)).name;
                    }

                    else
                    {
                        for (int i = 0; i < deps.Length - 1; i++)
                        {
                            temp = deps[i].TrimStart();
                            temp = temp.TrimEnd();
                            title += Models.Departement.select(StatisticsClasses.PatientStatisticModel.findDepID(temp)).name + ", ";
                        }
                        temp = deps[deps.Length - 1].TrimStart();
                        temp = temp.TrimEnd();
                        title += Models.Departement.select(StatisticsClasses.PatientStatisticModel.findDepID(temp)).name;

                    }

                }

                if (ps.modalities != null)
                {
                    string[] mods = ps.modalities;
                    if (mods.Length == 1)
                    {
                        temp = mods[0].TrimStart();
                        temp = temp.TrimEnd();
                        title += "\nعلى الآلة\n" + Models.Modality.Select(StatisticsClasses.PatientStatisticModel.findModID(temp)).name;

                    }
                    else
                    {
                        title += "\nعلى الآلات\n";
                        for (int i = 0; i < mods.Length - 1; i++)
                        {
                            temp = mods[i].TrimStart();
                            temp = temp.TrimEnd();
                            title += Models.Modality.Select(StatisticsClasses.PatientStatisticModel.findModID(temp)).name + ", ";
                        }
                        temp = mods[mods.Length - 1].TrimStart();
                        temp = temp.TrimEnd();
                        title += Models.Modality.Select(StatisticsClasses.PatientStatisticModel.findModID(temp)).name;

                    }
                }
            }
            else
            {
                if (ps.GroupingItem != null)
                {
                    if (ps.GroupingItem == "Gendre")
                    {
                        title += "Gender Patient Statistics";
                    }
                    if (ps.GroupingItem == "Age")
                    {
                        title += "Age Patient Statistics";

                    }
                    if (ps.GroupingItem == "DID")
                    {
                        title += "Daily Patient Statistics";
                    }
                    if (ps.GroupingItem == "MID")
                    {
                        title += "Monthly Patient Statistics";
                    }
                    if (ps.GroupingItem == "YID")
                    {
                        title += "Yearly Patient Statistics";
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

                if (ps.departements != null)
                {
                    title += " which has been treated in\n";

                    string[] deps = ps.departements;

                    if (deps.Length == 1)
                    {
                        temp = deps[0].TrimStart();
                        temp = temp.TrimEnd();
                        title += Models.Departement.select(StatisticsClasses.PatientStatisticModel.findDepID(temp)).name;
                    }

                    else
                    {
                        for (int i = 0; i < deps.Length - 1; i++)
                        {
                            temp = deps[i].TrimStart();
                            temp = temp.TrimEnd();
                            title += Models.Departement.select(StatisticsClasses.PatientStatisticModel.findDepID(temp)).name + ", ";
                        }
                        temp = deps[deps.Length - 1].TrimStart();
                        temp = temp.TrimEnd();
                        title += Models.Departement.select(StatisticsClasses.PatientStatisticModel.findDepID(temp)).name;

                    }

                }

                if (ps.modalities != null)
                {
                    string[] mods = ps.modalities;
                    if (mods.Length == 1)
                    {
                        temp = mods[0].TrimStart();
                        temp = temp.TrimEnd();
                        title += "\non modality\n" + Models.Modality.Select(StatisticsClasses.PatientStatisticModel.findModID(temp)).name;

                    }
                    else
                    {
                        title += "\non modalities\n";
                        for (int i = 0; i < mods.Length - 1; i++)
                        {
                            temp = mods[i].TrimStart();
                            temp = temp.TrimEnd();
                            title += Models.Modality.Select(StatisticsClasses.PatientStatisticModel.findModID(temp)).name + ", ";
                        }
                        temp = mods[mods.Length - 1].TrimStart();
                        temp = temp.TrimEnd();
                        title += Models.Modality.Select(StatisticsClasses.PatientStatisticModel.findModID(temp)).name;

                    }
                }
            }

            string ordersQuerry = " (SELECT DISTINCT PATIENTID FROM ORDERS WHERE 1=1";
            ViewData["sex"] = ps.sex.ToString();
            string tmp = "";
            if (ps.departements != null)
            {
                ordersQuerry += " AND (";

                string[] deps = ps.departements;

                if (deps.Length == 1)
                {
                    tmp = deps[0].TrimStart();
                    tmp = tmp.TrimEnd();
                    ordersQuerry += " ORDERS.DEPTNAME='" + StatisticsClasses.PatientStatisticModel.findDepID(tmp).ToString() + "' )";
                }

                else
                {
                    for (int i = 0; i < deps.Length - 1; i++)
                    {
                        tmp = deps[i].TrimStart();
                        tmp = tmp.TrimEnd();
                        ordersQuerry += " ORDERS.DEPTNAME='" + StatisticsClasses.PatientStatisticModel.findDepID(tmp).ToString().Trim() + "' or";
                    }
                    tmp = deps[deps.Length - 1].TrimStart();
                    tmp = tmp.TrimEnd();
                    ordersQuerry += " ORDERS.DEPTNAME='" + StatisticsClasses.PatientStatisticModel.findDepID(tmp).ToString().Trim() + "' )";

                }

            }

            if (ps.modalities != null)
            {
                ordersQuerry += " AND (";

                string[] mods = ps.modalities;
                if (mods.Length == 1)
                {
                    tmp = mods[0].TrimStart();
                    tmp = tmp.TrimEnd();
                    ordersQuerry += " ORDERS.modalityid='" + StatisticsClasses.PatientStatisticModel.findModID(tmp).ToString() + "' )";

                }
                else
                {
                    for (int i = 0; i < mods.Length - 1; i++)
                    {
                        tmp = mods[i].TrimStart();
                        tmp = tmp.TrimEnd();
                        ordersQuerry += " ORDERS.modalityid='" + StatisticsClasses.PatientStatisticModel.findModID(tmp).ToString() + "' or";
                    }
                    tmp = mods[mods.Length - 1].TrimStart();
                    tmp = tmp.TrimEnd();
                    ordersQuerry += " ORDERS.modalityid='" + StatisticsClasses.PatientStatisticModel.findModID(tmp).ToString() + "' )";

                }
            }
            ordersQuerry += ") ";

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
                    Querry += "SELECT COUNT(NUM) AS PATIENTSNUMBER, GENDRE AS GROUPINGITEM FROM PATIENT WHERE NUM IN" + ordersQuerry + searchParameter + " GROUP BY GENDRE";
                }
                if (ps.GroupingItem == "Age")
                {
                    Querry += "SELECT COUNT(NUM) AS PATIENTSNUMBER, AGE AS GROUPINGITEM FROM PATIENT WHERE NUM IN" + ordersQuerry + searchParameter + " GROUP BY AGE ORDER BY AGE ASC";
                    reportParameters[1] = new ReportParameter("GroupingItemParameter", RIS.Resources.Res.PatientAge, false);
                    reportParameters[2] = new ReportParameter("ChartTitleParameter", RIS.Resources.Res.groupByAge, false);
                    reportParameters[5] = new ReportParameter("TypeParameter", "2", false);

                }
                if (ps.GroupingItem == "DID")
                {
                    Querry += "SELECT COUNT(NUM) AS PATIENTSNUMBER, to_char(to_date(insertdate,'dd/mm/yy'),'dd-mm-yyyy') AS GROUPINGITEM FROM PATIENT WHERE NUM IN" + ordersQuerry + searchParameter + " GROUP BY to_date(insertdate,'dd/mm/yy') ORDER BY to_date(insertdate,'dd/mm/yy') DESC";
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
                              " FROM PATIENT WHERE NUM IN" + ordersQuerry + searchParameter +
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
                              " FROM PATIENT WHERE NUM IN" + ordersQuerry + searchParameter +
                              " GROUP BY EXTRACT(YEAR FROM to_date(insertdate, 'dd/mm/yy'))" +
                              " ORDER BY EXTRACT(YEAR FROM to_date(insertdate, 'dd/mm/yy')) DESC";
                }
            }

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

            ViewBag.depList = Models.Departement.depListNames();
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
        public ActionResult PatientStaticsTable(StatisticsClasses.PatientStatisticModel ps)
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

            string lang = Cookies.GetCookieVal("Language");
            string title = "";
            string searchParameter = "";
            string ordersQuerry = "";
            string tmp = "";
            string Querry = "";

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

                ordersQuerry = " (SELECT DISTINCT PATIENTID FROM ORDERS WHERE 1=1";
                tmp = "";
                if (ps.departements != null)
                {
                    ordersQuerry += " AND (";
                    title += " المصورون في\n";

                    string[] deps = ps.departements;

                    if (deps.Length == 1)
                    {
                        tmp = deps[0].TrimStart();
                        tmp = tmp.TrimEnd();
                        ordersQuerry += " ORDERS.DEPTNAME='" + StatisticsClasses.PatientStatisticModel.findDepID(tmp).ToString() + "' )";
                        title += Models.Departement.select(StatisticsClasses.PatientStatisticModel.findDepID(tmp)).name;
                    }

                    else
                    {
                        for (int i = 0; i < deps.Length - 1; i++)
                        {
                            tmp = deps[i].TrimStart();
                            tmp = tmp.TrimEnd();
                            ordersQuerry += " ORDERS.DEPTNAME='" + StatisticsClasses.PatientStatisticModel.findDepID(tmp).ToString().Trim() + "' or";
                            title += Models.Departement.select(StatisticsClasses.PatientStatisticModel.findDepID(tmp)).name + ", ";
                        }
                        tmp = deps[deps.Length - 1].TrimStart();
                        tmp = tmp.TrimEnd();
                        ordersQuerry += " ORDERS.DEPTNAME='" + StatisticsClasses.PatientStatisticModel.findDepID(tmp).ToString().Trim() + "' )";
                        title += Models.Departement.select(StatisticsClasses.PatientStatisticModel.findDepID(tmp)).name;

                    }

                }

                if (ps.modalities != null)
                {
                    ordersQuerry += " AND (";

                    string[] mods = ps.modalities;
                    if (mods.Length == 1)
                    {
                        tmp = mods[0].TrimStart();
                        tmp = tmp.TrimEnd();
                        ordersQuerry += " ORDERS.modalityid='" + StatisticsClasses.PatientStatisticModel.findModID(tmp).ToString() + "' )";
                        title += "\nعلى الآلة\n" + Models.Modality.Select(StatisticsClasses.PatientStatisticModel.findModID(tmp)).name;

                    }
                    else
                    {
                        title += "\nعلى الآلات\n";
                        for (int i = 0; i < mods.Length - 1; i++)
                        {
                            tmp = mods[i].TrimStart();
                            tmp = tmp.TrimEnd();
                            ordersQuerry += " ORDERS.modalityid='" + StatisticsClasses.PatientStatisticModel.findModID(tmp).ToString() + "' or";
                            title += Models.Modality.Select(StatisticsClasses.PatientStatisticModel.findModID(tmp)).name + ", ";
                        }
                        tmp = mods[mods.Length - 1].TrimStart();
                        tmp = tmp.TrimEnd();
                        ordersQuerry += " ORDERS.modalityid='" + StatisticsClasses.PatientStatisticModel.findModID(tmp).ToString() + "' )";
                        title += Models.Modality.Select(StatisticsClasses.PatientStatisticModel.findModID(tmp)).name;

                    }
                }
                ordersQuerry += ") ";

                title += "\nعدد المرضى : ";

                Querry += "SELECT NUM, ID, GIVENID, FIRSTNAME, MIDDLENAME, LASTNAME, GENDRE, MOTHERNAME, BIRTHDATE, AGE, MOBILEPHONE, LANDPHONE, CURRENTADDRESS, RESIDENTADDRESS, WORKPHONE," +
                         "WORKADDRESS, NEARESTPERSON, NEARESTPERSONPHONE, BIRTHPLACE, NATIONALIDNUMBER, NATIONALITY, WORKTYPE, NOTES, MARTIALSTATUS, TRANSLATEDFNAME, TRANSLATEDLNAME," +
                         "TRANSLATEDFATHERNAME, TRANSLATEDMOTHERNAME, to_char(INSERTDATE, 'dd-mm-yyyy hh24:mi:ss') AS INSERTDATE FROM PATIENT WHERE NUM IN" + ordersQuerry + searchParameter + " ORDER BY PATIENT.INSERTDATE DESC";
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

                ordersQuerry = " (SELECT DISTINCT PATIENTID FROM ORDERS WHERE 1=1";
                tmp = "";
                if (ps.modalities != null || ps.departements != null)
                    title += " which has been treated";

                if (ps.departements != null)
                {
                    ordersQuerry += " AND (";
                    title += " in\n";

                    string[] deps = ps.departements;

                    if (deps.Length == 1)
                    {
                        tmp = deps[0].TrimStart();
                        tmp = tmp.TrimEnd();
                        ordersQuerry += " ORDERS.DEPTNAME='" + StatisticsClasses.PatientStatisticModel.findDepID(tmp).ToString() + "' )";
                        title += Models.Departement.select(StatisticsClasses.PatientStatisticModel.findDepID(tmp)).name;
                    }

                    else
                    {
                        for (int i = 0; i < deps.Length - 1; i++)
                        {
                            tmp = deps[i].TrimStart();
                            tmp = tmp.TrimEnd();
                            ordersQuerry += " ORDERS.DEPTNAME='" + StatisticsClasses.PatientStatisticModel.findDepID(tmp).ToString().Trim() + "' or";
                            title += Models.Departement.select(StatisticsClasses.PatientStatisticModel.findDepID(tmp)).name + " , ";
                        }
                        tmp = deps[deps.Length - 1].TrimStart();
                        tmp = tmp.TrimEnd();
                        ordersQuerry += " ORDERS.DEPTNAME='" + StatisticsClasses.PatientStatisticModel.findDepID(tmp).ToString().Trim() + "' )";
                        title += Models.Departement.select(StatisticsClasses.PatientStatisticModel.findDepID(tmp)).name;

                    }

                }

                if (ps.modalities != null)
                {
                    ordersQuerry += " AND (";

                    string[] mods = ps.modalities;
                    if (mods.Length == 1)
                    {
                        tmp = mods[0].TrimStart();
                        tmp = tmp.TrimEnd();
                        ordersQuerry += " ORDERS.modalityid='" + StatisticsClasses.PatientStatisticModel.findModID(tmp).ToString() + "' )";
                        title += "\non modality\n" + Models.Modality.Select(StatisticsClasses.PatientStatisticModel.findModID(tmp)).name;

                    }
                    else
                    {
                        title += "\non modalities\n";
                        for (int i = 0; i < mods.Length - 1; i++)
                        {
                            tmp = mods[i].TrimStart();
                            tmp = tmp.TrimEnd();
                            ordersQuerry += " ORDERS.modalityid='" + StatisticsClasses.PatientStatisticModel.findModID(tmp).ToString() + "' or";
                            title += Models.Modality.Select(StatisticsClasses.PatientStatisticModel.findModID(tmp)).name + ", ";
                        }
                        tmp = mods[mods.Length - 1].TrimStart();
                        tmp = tmp.TrimEnd();
                        ordersQuerry += " ORDERS.modalityid='" + StatisticsClasses.PatientStatisticModel.findModID(tmp).ToString() + "' )";
                        title += Models.Modality.Select(StatisticsClasses.PatientStatisticModel.findModID(tmp)).name;

                    }
                }
                ordersQuerry += ") ";

                title += "\nNumber of Patients :";

                Querry += "SELECT NUM, ID, GIVENID, FIRSTNAME, MIDDLENAME, LASTNAME, GENDRE, MOTHERNAME, BIRTHDATE, AGE, MOBILEPHONE, LANDPHONE, CURRENTADDRESS, RESIDENTADDRESS, WORKPHONE," +
                         "WORKADDRESS, NEARESTPERSON, NEARESTPERSONPHONE, BIRTHPLACE, NATIONALIDNUMBER, NATIONALITY, WORKTYPE, NOTES, MARTIALSTATUS, TRANSLATEDFNAME, TRANSLATEDLNAME," +
                         "TRANSLATEDFATHERNAME, TRANSLATEDMOTHERNAME, to_char(INSERTDATE, 'dd-mm-yyyy hh24:mi:ss') AS INSERTDATE FROM PATIENT WHERE NUM IN" + ordersQuerry + searchParameter + " ORDER BY PATIENT.INSERTDATE DESC";
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

            ViewBag.selectedDepList = ps.departements;
            ViewBag.selectedModList = ps.modalities;
            ViewData["sex"] = ps.sex.ToString();
            ViewBag.depList = Models.Departement.depListNames();
            ViewBag.modList = Models.Modality.modListNames();
            ViewData["sex"] = ps.sex.ToString();

            return View("PatientStatics");

        }


        /// <summary>
        /// This action is called when orders' statistics page is retrieved
        /// </summary>
        /// <permission cref="RIS.Perms"> The user should have the permission Perms.StatsOrders to access statistics </permission>
        /// <returns> The orders' statistics view </returns>
        public ActionResult OrdersStatics()
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
            if (!RIS.Models.User.hasPerm(userId, Perms.StatsOrders))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }

            ViewBag.selectedDepList = null;
            ViewBag.selectedModList = null;
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.SizeToReportContent = true;
            reportViewer.ZoomMode = ZoomMode.FullPage;

            ViewBag.depList = Models.Departement.depListNames();
            ViewBag.modList = Models.Modality.modListNames();
            ViewBag.procedures = Models.Procedure.GetProcedureCodes(true, "");

            reportViewer.LocalReport.Refresh();

            ViewBag.ReportViewer = reportViewer;


            return View();

        }


        /// <summary>
        /// This action is called when there is a post request for orders' statistics charts.
        /// </summary>
        /// <param name="ps"> a parmeter of type 'OrdersStatisticModel' contains the filtering options
        /// defined by the user </param>
        /// <returns> The orders' statistics view with a report contains the orders numbers that meets the user's filtering
        /// options with a chart representing these results </returns>
        [HttpPost]
        public ActionResult OrdersStatics(StatisticsClasses.OrdersStatisticModel ps)
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

            ViewBag.selectedDepList = ps.departements;
            ViewBag.selectedModList = ps.modalities;

            ReportParameter[] reportParameters = new ReportParameter[7];
            reportParameters[0] = new ReportParameter("PatientNumberParameter", RIS.Resources.Res.OrdersNumber, false);
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

                title += " لطلبات التصوير المسجلة عن طريق نظام تسجيل المرضى في مشفى دمشق";

                if (ps.sStartDate.Date.ToString("MM-dd-yyyy") != "01-01-0001")
                {
                    title += " ابتداءً من تاريخ " + ps.sStartDate.ToString("yyyy-MM-dd");
                }
                if (ps.eStartDate.Date.ToString("MM-dd-yyyy") != "01-01-0001")
                {
                    title += " وانتهاءً بتاريخ " + ps.eStartDate.ToString("yyyy-MM-dd");
                }
                if (ps.procedures != "" && ps.procedures != null)
                {
                    title += " بوضعية " + Models.Procedure.selectByCode(ps.procedures).name;
                }


                if (ps.departements != null)
                {
                    title += " في\n";

                    string[] deps = ps.departements;

                    if (deps.Length == 1)
                    {
                        temp = deps[0].TrimStart();
                        temp = temp.TrimEnd();
                        title += Models.Departement.select(StatisticsClasses.PatientStatisticModel.findDepID(temp)).name;
                    }

                    else
                    {
                        for (int i = 0; i < deps.Length - 1; i++)
                        {
                            temp = deps[i].TrimStart();
                            temp = temp.TrimEnd();
                            title += Models.Departement.select(StatisticsClasses.PatientStatisticModel.findDepID(temp)).name + "، ";
                        }
                        temp = deps[deps.Length - 1].TrimStart();
                        temp = temp.TrimEnd();
                        title += Models.Departement.select(StatisticsClasses.PatientStatisticModel.findDepID(temp)).name;

                    }

                }

                if (ps.modalities != null)
                {
                    string[] mods = ps.modalities;
                    if (mods.Length == 1)
                    {
                        title += "\nعلى الآلة\n";
                        temp = mods[0].TrimStart();
                        temp = temp.TrimEnd();
                        title += Models.Modality.Select(StatisticsClasses.PatientStatisticModel.findModID(temp)).name;

                    }
                    else
                    {
                        title += "\nعلى الآلات\n";
                        for (int i = 0; i < mods.Length - 1; i++)
                        {
                            temp = mods[i].TrimStart();
                            temp = temp.TrimEnd();
                            title += Models.Modality.Select(StatisticsClasses.PatientStatisticModel.findModID(temp)).name + "، ";
                        }
                        temp = mods[mods.Length - 1].TrimStart();
                        temp = temp.TrimEnd();
                        title += Models.Modality.Select(StatisticsClasses.PatientStatisticModel.findModID(temp)).name;

                    }
                }
                
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

                title += " for Radiology orders in Damascus hospital radiology information system";

                if (ps.sStartDate.Date.ToString("MM-dd-yyyy") != "01-01-0001")
                {
                    title += " since " + ps.sStartDate.ToString("yyyy-MM-dd");
                }
                if (ps.eStartDate.Date.ToString("MM-dd-yyyy") != "01-01-0001")
                {
                    title += " untill " + ps.eStartDate.ToString("yyyy-MM-dd");
                }
                if (ps.procedures != "" && ps.procedures != null)
                {
                    title += " under procedure " + Models.Procedure.selectByCode(ps.procedures).name;
                }


                if (ps.departements != null)
                {
                    title += " in\n";

                    string[] deps = ps.departements;

                    if (deps.Length == 1)
                    {
                        temp = deps[0].TrimStart();
                        temp = temp.TrimEnd();
                        title += Models.Departement.select(StatisticsClasses.PatientStatisticModel.findDepID(temp)).name;
                    }

                    else
                    {
                        for (int i = 0; i < deps.Length - 1; i++)
                        {
                            temp = deps[i].TrimStart();
                            temp = temp.TrimEnd();
                            title += Models.Departement.select(StatisticsClasses.PatientStatisticModel.findDepID(temp)).name + "، ";
                        }
                        temp = deps[deps.Length - 1].TrimStart();
                        temp = temp.TrimEnd();
                        title += Models.Departement.select(StatisticsClasses.PatientStatisticModel.findDepID(temp)).name;

                    }

                }

                if (ps.modalities != null)
                {
                    string[] mods = ps.modalities;
                    if (mods.Length == 1)
                    {
                        title += "\nom modality\n";
                        temp = mods[0].TrimStart();
                        temp = temp.TrimEnd();
                        title += Models.Modality.Select(StatisticsClasses.PatientStatisticModel.findModID(temp)).name;

                    }
                    else
                    {
                        title += "\non modalities\n";
                        for (int i = 0; i < mods.Length - 1; i++)
                        {
                            temp = mods[i].TrimStart();
                            temp = temp.TrimEnd();
                            title += Models.Modality.Select(StatisticsClasses.PatientStatisticModel.findModID(temp)).name + "، ";
                        }
                        temp = mods[mods.Length - 1].TrimStart();
                        temp = temp.TrimEnd();
                        title += Models.Modality.Select(StatisticsClasses.PatientStatisticModel.findModID(temp)).name;

                    }
                }
            }

            string searchParameter = "";

            string tmp = "";
            if (ps.departements != null)
            {
                searchParameter += " AND (";

                string[] deps = ps.departements;

                if (deps.Length == 1)
                {
                    tmp = deps[0].TrimStart();
                    tmp = tmp.TrimEnd();
                    searchParameter += " ORDERS.DEPTNAME='" + StatisticsClasses.PatientStatisticModel.findDepID(tmp).ToString() + "' )";
                }

                else
                {
                    for (int i = 0; i < deps.Length - 1; i++)
                    {
                        tmp = deps[i].TrimStart();
                        tmp = tmp.TrimEnd();
                        searchParameter += " ORDERS.DEPTNAME='" + StatisticsClasses.PatientStatisticModel.findDepID(tmp).ToString().Trim() + "' or";
                    }
                    tmp = deps[deps.Length - 1].TrimStart();
                    tmp = tmp.TrimEnd();
                    searchParameter += " ORDERS.DEPTNAME='" + StatisticsClasses.PatientStatisticModel.findDepID(tmp).ToString().Trim() + "' )";

                }

            }

            if (ps.modalities != null)
            {
                searchParameter += " AND (";

                string[] mods = ps.modalities;
                if (mods.Length == 1)
                {
                    tmp = mods[0].TrimStart();
                    tmp = tmp.TrimEnd();
                    searchParameter += " ORDERS.modalityid='" + StatisticsClasses.PatientStatisticModel.findModID(tmp).ToString() + "' )";

                }
                else
                {
                    for (int i = 0; i < mods.Length - 1; i++)
                    {
                        tmp = mods[i].TrimStart();
                        tmp = tmp.TrimEnd();
                        searchParameter += " ORDERS.modalityid='" + StatisticsClasses.PatientStatisticModel.findModID(tmp).ToString() + "' or";
                    }
                    tmp = mods[mods.Length - 1].TrimStart();
                    tmp = tmp.TrimEnd();
                    searchParameter += " ORDERS.modalityid='" + StatisticsClasses.PatientStatisticModel.findModID(tmp).ToString() + "' )";

                }
            }




            if (ps.sStartDate.Date.ToString("MM-dd-yyyy") != "01-01-0001")
                searchParameter += " AND to_date(to_date(orders.startdate,'yyyymmddhh24:mi:ss'),'dd-mm-yy') >= Date '" + ps.sStartDate.ToString("yyyy-MM-dd") + "'";
            if (ps.eStartDate.Date.ToString("MM-dd-yyyy") != "01-01-0001")
                searchParameter += " AND to_date(to_date(orders.startdate,'yyyymmddhh24:mi:ss'),'dd-mm-yy') <= Date '" + ps.eStartDate.ToString("yyyy-MM-dd") + "'";
            if (ps.procedures != "" && ps.procedures != null)
                searchParameter += " AND orders.procedureid = " + ps.procedures;




            string Querry = "";

            if (ps.GroupingItem != null)
            {
                if (ps.GroupingItem == "DID")
                {
                    Querry += "SELECT COUNT(NUM) AS PATIENTSNUMBER, to_char(to_date(to_date(startdate,'yyyymmddhh24:mi:ss'),'dd/mm/yy'),'dd-mm-yyyy') AS GROUPINGITEM FROM ORDERS WHERE ORDERS.STATUS != "+ ConnectionConfigs.Waiting + " " + searchParameter + " GROUP BY to_date(to_date(startdate,'yyyymmddhh24:mi:ss'),'dd/mm/yy') ORDER BY to_date(to_date(startdate,'yyyymmddhh24:mi:ss'),'dd/mm/yy') DESC";
                    reportParameters[1] = new ReportParameter("GroupingItemParameter", RIS.Resources.Res.DID, false);
                    reportParameters[2] = new ReportParameter("ChartTitleParameter", RIS.Resources.Res.groupByDailyId, false);
                    reportParameters[5] = new ReportParameter("TypeParameter", "3", false);
                }
                if (ps.GroupingItem == "MID")
                {
                    reportParameters[1] = new ReportParameter("GroupingItemParameter", RIS.Resources.Res.MID, false);
                    reportParameters[2] = new ReportParameter("ChartTitleParameter", RIS.Resources.Res.groupByMonthlyId, false);
                    reportParameters[5] = new ReportParameter("TypeParameter", "4", false);
                    Querry += "SELECT COUNT(NUM) AS PATIENTSNUMBER,CONCAT ( CONCAT( EXTRACT(MONTH FROM to_date(to_date(startdate,'yyyymmddhh24:mi:ss'),'dd/mm/yy')),'-'),EXTRACT(YEAR FROM to_date(to_date(startdate,'yyyymmddhh24:mi:ss'),'dd/mm/yy'))) AS GROUPINGITEM " +
                                " FROM ORDERS WHERE ORDERS.STATUS != " + ConnectionConfigs.Waiting + " " + searchParameter +
                                " GROUP BY EXTRACT(YEAR FROM to_date(to_date(startdate, 'yyyymmddhh24:mi:ss'), 'dd/mm/yy'))," +
                                " EXTRACT(MONTH FROM to_date(to_date(startdate, 'yyyymmddhh24:mi:ss'), 'dd/mm/yy'))" +
                                " ORDER BY EXTRACT(YEAR FROM to_date(to_date(startdate, 'yyyymmddhh24:mi:ss'), 'dd/mm/yy')) DESC," +
                                " EXTRACT(MONTH FROM to_date(to_date(startdate, 'yyyymmddhh24:mi:ss'), 'dd/mm/yy')) DESC";
                }
                if (ps.GroupingItem == "YID")
                {
                    reportParameters[1] = new ReportParameter("GroupingItemParameter", RIS.Resources.Res.YID, false);
                    reportParameters[2] = new ReportParameter("ChartTitleParameter", RIS.Resources.Res.groupByYearlyId, false);
                    reportParameters[5] = new ReportParameter("TypeParameter", "5", false);
                    Querry += "SELECT COUNT(NUM) AS PATIENTSNUMBER,EXTRACT(YEAR FROM to_date(to_date(startdate,'yyyymmddhh24:mi:ss'),'dd/mm/yy')) AS GROUPINGITEM" +
                              " FROM ORDERS WHERE ORDERS.STATUS != " + ConnectionConfigs.Waiting + " " + searchParameter +
                              " GROUP BY EXTRACT(YEAR FROM to_date(to_date(startdate, 'yyyymmddhh24:mi:ss'), 'dd/mm/yy'))" +
                              " ORDER BY EXTRACT(YEAR FROM to_date(to_date(startdate, 'yyyymmddhh24:mi:ss'), 'dd/mm/yy')) DESC";
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

            ViewBag.depList = Models.Departement.depListNames();
            ViewBag.modList = Models.Modality.modListNames();
            ViewBag.procedures = Models.Procedure.GetProcedureCodes(true, "");

            return View();

        }


        /// <summary>
        /// This action is called when there is a post request for a detailed orders' list.
        /// </summary>
        /// <param name="ps"> a parmeter of type 'OrdersStatisticModel' contains the filtering options
        /// defined by the user </param>
        /// <returns> The orders' statistics view with a report contains a detailed list of orders that meets the user's 
        /// filtering options </returns>
        /// <remarks> if there is no filtering options was inserted by the user, a list of all orders in RIS is returned </remarks>
        [HttpPost]
        public ActionResult OrdersStaticsTable(StatisticsClasses.OrdersStatisticModel ps)
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

            ViewBag.selectedDepList = ps.departements;
            ViewBag.selectedModList = ps.modalities;

            string lang = Cookies.GetCookieVal("Language");
            string searchParameter = "";
            string tmp = "";
            string title = "";

            if (lang == "ar")
            {
                title += " طلبات التصوير المسجلة عن طريق نظام تسجيل المرضى في مشفى دمشق";

                if (ps.sStartDate.Date.ToString("MM-dd-yyyy") != "01-01-0001")
                {
                    searchParameter += " AND to_date(to_date(orders.startdate,'yyyymmddhh24miss'),'dd-mm-yy') >= Date '" + ps.sStartDate.ToString("yyyy-MM-dd") + "'";
                    title += " ابتداءً من تاريخ " + ps.sStartDate.ToString("yyyy-MM-dd");
                }
                if (ps.eStartDate.Date.ToString("MM-dd-yyyy") != "01-01-0001")
                {
                    searchParameter += " AND to_date(to_date(orders.startdate,'yyyymmddhh24miss'),'dd-mm-yy') <= Date '" + ps.eStartDate.ToString("yyyy-MM-dd") + "'";
                    title += " وانتهاءً بتاريخ " + ps.eStartDate.ToString("yyyy-MM-dd");
                }
                if (ps.procedures != "" && ps.procedures != null)
                {
                    searchParameter += " AND orders.procedureid = " + ps.procedures;
                    title += " بوضعية " + Models.Procedure.selectByCode(ps.procedures).name;
                }


                if (ps.radPayType != 0)
                {
                    searchParameter += " AND orders.PAYTYPE = " + ps.radPayType;
                }

                if (ps.departements != null)
                {
                    searchParameter += " AND (";
                    title += " في\n";

                    string[] deps = ps.departements;

                    if (deps.Length == 1)
                    {
                        tmp = deps[0].TrimStart();
                        tmp = tmp.TrimEnd();
                        searchParameter += " ORDERS.DEPTNAME='" + StatisticsClasses.PatientStatisticModel.findDepID(tmp).ToString() + "' )";
                        title += Models.Departement.select(StatisticsClasses.PatientStatisticModel.findDepID(tmp)).name;
                    }

                    else
                    {
                        for (int i = 0; i < deps.Length - 1; i++)
                        {
                            tmp = deps[i].TrimStart();
                            tmp = tmp.TrimEnd();
                            searchParameter += " ORDERS.DEPTNAME='" + StatisticsClasses.PatientStatisticModel.findDepID(tmp).ToString().Trim() + "' or";
                            title += Models.Departement.select(StatisticsClasses.PatientStatisticModel.findDepID(tmp)).name + "، ";
                        }
                        tmp = deps[deps.Length - 1].TrimStart();
                        tmp = tmp.TrimEnd();
                        searchParameter += " ORDERS.DEPTNAME='" + StatisticsClasses.PatientStatisticModel.findDepID(tmp).ToString().Trim() + "' )";
                        title += Models.Departement.select(StatisticsClasses.PatientStatisticModel.findDepID(tmp)).name;

                    }

                }

                if (ps.modalities != null)
                {
                    searchParameter += " AND (";

                    string[] mods = ps.modalities;
                    if (mods.Length == 1)
                    {
                        title += "\nعلى الآلة\n";
                        tmp = mods[0].TrimStart();
                        tmp = tmp.TrimEnd();
                        searchParameter += " ORDERS.modalityid='" + StatisticsClasses.PatientStatisticModel.findModID(tmp).ToString() + "' )";
                        title += Models.Modality.Select(StatisticsClasses.PatientStatisticModel.findModID(tmp)).name;

                    }
                    else
                    {
                        title += "\nعلى الآلات\n";
                        for (int i = 0; i < mods.Length - 1; i++)
                        {
                            tmp = mods[i].TrimStart();
                            tmp = tmp.TrimEnd();
                            searchParameter += " ORDERS.modalityid='" + StatisticsClasses.PatientStatisticModel.findModID(tmp).ToString() + "' or";
                            title += Models.Modality.Select(StatisticsClasses.PatientStatisticModel.findModID(tmp)).name + "، ";
                        }
                        tmp = mods[mods.Length - 1].TrimStart();
                        tmp = tmp.TrimEnd();
                        searchParameter += " ORDERS.modalityid='" + StatisticsClasses.PatientStatisticModel.findModID(tmp).ToString() + "' )";
                        title += Models.Modality.Select(StatisticsClasses.PatientStatisticModel.findModID(tmp)).name;

                    }
                }

                title += "\nعدد الطلبات : ";
            }
            else
            {
                title += "Orders Registered via Radiology Information System in Damascus Hospital ";

                if (ps.sStartDate.Date.ToString("MM-dd-yyyy") != "01-01-0001")
                {
                    searchParameter += " AND to_date(to_date(orders.startdate,'yyyymmddhh24miss'),'dd-mm-yy') >= Date '" + ps.sStartDate.ToString("yyyy-MM-dd") + "'";
                    title += " since " + ps.sStartDate.ToString("yyyy-MM-dd");
                }
                if (ps.eStartDate.Date.ToString("MM-dd-yyyy") != "01-01-0001")
                {
                    searchParameter += " AND to_date(to_date(orders.startdate,'yyyymmddhh24miss'),'dd-mm-yy') <= Date '" + ps.eStartDate.ToString("yyyy-MM-dd") + "'";
                    title += " untill " + ps.eStartDate.ToString("yyyy-MM-dd");
                }
                if (ps.procedures != "" && ps.procedures != null)
                {
                    searchParameter += " AND orders.procedureid = " + ps.procedures;
                    title += " Under Procedure " + Models.Procedure.selectByCode(ps.procedures).name;
                }

                if(ps.radPayType!=0)
                {
                    searchParameter += " AND orders.PAYTYPE = " + ps.radPayType;
                }

                if (ps.departements != null)
                {
                    searchParameter += " AND (";
                    title += " in\n";

                    string[] deps = ps.departements;

                    if (deps.Length == 1)
                    {
                        tmp = deps[0].TrimStart();
                        tmp = tmp.TrimEnd();
                        searchParameter += " ORDERS.DEPTNAME='" + StatisticsClasses.PatientStatisticModel.findDepID(tmp).ToString() + "' )";
                        title += Models.Departement.select(StatisticsClasses.PatientStatisticModel.findDepID(tmp)).name;
                    }

                    else
                    {
                        for (int i = 0; i < deps.Length - 1; i++)
                        {
                            tmp = deps[i].TrimStart();
                            tmp = tmp.TrimEnd();
                            searchParameter += " ORDERS.DEPTNAME='" + StatisticsClasses.PatientStatisticModel.findDepID(tmp).ToString().Trim() + "' or";
                            title += Models.Departement.select(StatisticsClasses.PatientStatisticModel.findDepID(tmp)).name + "، ";
                        }
                        tmp = deps[deps.Length - 1].TrimStart();
                        tmp = tmp.TrimEnd();
                        searchParameter += " ORDERS.DEPTNAME='" + StatisticsClasses.PatientStatisticModel.findDepID(tmp).ToString().Trim() + "' )";
                        title += Models.Departement.select(StatisticsClasses.PatientStatisticModel.findDepID(tmp)).name;

                    }

                }

                if (ps.modalities != null)
                {
                    searchParameter += " AND (";

                    string[] mods = ps.modalities;
                    if (mods.Length == 1)
                    {
                        title += "\non modality\n";
                        tmp = mods[0].TrimStart();
                        tmp = tmp.TrimEnd();
                        searchParameter += " ORDERS.modalityid='" + StatisticsClasses.PatientStatisticModel.findModID(tmp).ToString() + "' )";
                        title += Models.Modality.Select(StatisticsClasses.PatientStatisticModel.findModID(tmp)).name;

                    }
                    else
                    {
                        title += "\non modalities\n";
                        for (int i = 0; i < mods.Length - 1; i++)
                        {
                            tmp = mods[i].TrimStart();
                            tmp = tmp.TrimEnd();
                            searchParameter += " ORDERS.modalityid='" + StatisticsClasses.PatientStatisticModel.findModID(tmp).ToString() + "' or";
                            title += Models.Modality.Select(StatisticsClasses.PatientStatisticModel.findModID(tmp)).name + "، ";
                        }
                        tmp = mods[mods.Length - 1].TrimStart();
                        tmp = tmp.TrimEnd();
                        searchParameter += " ORDERS.modalityid='" + StatisticsClasses.PatientStatisticModel.findModID(tmp).ToString() + "' )";
                        title += Models.Modality.Select(StatisticsClasses.PatientStatisticModel.findModID(tmp)).name;

                    }
                }

                title += "\nNumber Of Orders : ";
            }


            string Querry = "SELECT PATIENT.FIRSTNAME, PATIENT.MIDDLENAME, PATIENT.LASTNAME, to_char(to_date(ORDERS.STARTDATE, 'yyyymmddhh24miss'), 'dd-mm-yyyy hh24:mi:ss') AS STARTDATE, ORDERS.DOCID, DEPARTMENT.NAME AS DNAME, MODALITY.NAME AS MNAME, PROCEDUREDESCRIPTION.NAME AS PNAME, to_char(to_date(ORDERS.ENDDATE, 'yyyymmddhh24miss'), 'dd-mm-yyyy hh24:mi:ss') AS ENDDATE FROM PATIENT, ORDERS, MODALITY, DEPARTMENT, PROCEDUREDESCRIPTION WHERE ORDERS.STATUS != " + ConnectionConfigs.Waiting + " AND PATIENT.NUM = ORDERS.PATIENTID AND ORDERS.MODALITYID = MODALITY.NUM AND ORDERS.DEPTNAME = DEPARTMENT.NUM AND ORDERS.PROCEDUREID = PROCEDUREDESCRIPTION.CODE"
                + searchParameter + " order by to_date(orders.startdate,'yyyymmddhh24miss') DESC";


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
            reportParameters[2] = new ReportParameter("MODALITYNAMEParameter", RIS.Resources.Res.MODALITYNAMEParameter, false);
            reportParameters[3] = new ReportParameter("DEPTNAMEParameter", RIS.Resources.Res.DEPTNAMEParameter, false);
            reportParameters[4] = new ReportParameter("STARTDATEParameter", RIS.Resources.Res.STARTDATEParameter, false);
            reportParameters[5] = new ReportParameter("ENDDATEParameter", RIS.Resources.Res.OrderEndDate, false);
            reportParameters[6] = new ReportParameter("TitleParameter", title, false);
            reportParameters[7] = new ReportParameter("ProcNameParameter", RIS.Resources.Res.procedureName, false);
            reportViewer.LocalReport.SetParameters(reportParameters);
            reportViewer.LocalReport.Refresh();

            ViewBag.ReportViewer = reportViewer;

            ViewBag.depList = Models.Departement.depListNames();
            ViewBag.modList = Models.Modality.modListNames();
            ViewBag.procedures = Models.Procedure.GetProcedureCodes(true, "");

            return View("OrdersStatics");
        }

    }
}