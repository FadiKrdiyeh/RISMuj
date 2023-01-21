using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HL7_TCP.Web;
using locale.helpers;
using RISDB;
using Oracle.DataAccess.Client;
using System.Threading;
using System.Globalization;

namespace RIS.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.ddd = "";
            return View();
        }

        [HttpPost]
        public ActionResult Index(string logUserName, string logInPassword)
        {

            //chech connection
            String connString = OracleRIS.GetConnectionString();
            OracleConnection conn=null;
            try
            {
                conn = new OracleConnection(connString);
                
                conn.Open();
                conn.Close();
                string te = RIS.Models.User.getUserByUname(logUserName).num.ToString();
                Session["mnmUserId"] = RIS.Models.User.getUserByUname(logUserName).num.ToString();
                Session["userClinicId"] = RIS.Models.User.getUserByUname(logUserName).departement.ToString();
            }
            catch (Exception ee)
            {
                ModelState.AddModelError("",Resources.Res.ConnectError);
                ViewBag.ddd = ee.ToString()+"oracle server does not respond!!! check connection configurations ";
                return View();
            }
            //end check connection

            try
            {
                bool v1 = trialConfigs.checkperiod();
                bool v2 = trialConfigs.getOrderNumbers();
                if (!v1 || !v2)
                {
                    if (v1)
                        Response.AppendToLog("#### ***MYLOG*** Trial Period is Out //" + DateTime.Now.ToShortTimeString() + "//");
                    else
                        Response.AppendToLog("#### ***MYLOG*** Trial Orders Number is OUT //" + DateTime.Now.ToShortTimeString() + "//");

                    return RedirectToAction("Index", "Home");
                }

                if (RIS.Models.LogIn.vallidateUser(logUserName, logInPassword))
                {

                 //   Cookies.SetCookie("Language", Session["userLang"]);


                    //change apperance by lang rtl ltr
                    if (Session["userLang"] != null)
                    {

                        Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(Session["userLang"].ToString());
                        Thread.CurrentThread.CurrentUICulture = new CultureInfo(Session["userLang"].ToString());

                    }
                    HttpCookie cookie = new HttpCookie("Language");
                    cookie.Value = Session["userLang"].ToString();
                    Response.Cookies.Add(cookie);
                    //



                    Response.AppendToLog("#### ***MYLOG*** User " + logUserName + " Has Logged In //" + DateTime.Now.ToShortTimeString() + "//");
                    //if (Session["userType"].ToString() == "1")
                    //if user is Reception
                   // bool isReception = false;
                    RIS.Models.User uu = RIS.Models.User.SelectByName(logUserName);
                    List<RIS.Models.Group> g = RIS.Models.Group.getUserGroups(uu.num);
                    RIS.Models.Group recepGrp = RIS.Models.Group.SelectByName(ConfigVar.recepGroup);
                    if (g.Contains(recepGrp) && g.Count ==1)
                    {
                        return RedirectToAction("Index", "Patient");
                    }else
                        return RedirectToAction("Index", "MainPage");

                    //return RedirectToAction("Index", "Patient");
                }
                else
                {
                    TempData["message"] = RIS.Resources.Res.LoginFaild; 
                    return View();
                }
            }
            catch (Exception ex)
            {
                string rrr = ex.ToString();
                Response.AppendToLog("#### ***MYLOG*** Error while user " + logUserName + " Logging //" + DateTime.Now.ToShortTimeString() + "// " + ex.Message);
                return View();
            }
            //Response.Write("Welcome");
            //if (logUserName == "m" && logInPassword == "m")
            //{
            //    Cookies.SetCookie("userName",logUserName);
            //    return RedirectToAction("Index", "MainPage");

            //}
            //if (logUserName == "h" && logInPassword == "h")
            //{
            //    Cookies.SetCookie("userName", logUserName);
            //    return RedirectToAction("Index", "MainPage");

            //}
            
            return View();
        }


        public ActionResult Logout(int x)
        {
            Cookies.SetCookie("userName", "");
            Session["userName"] = null;
            Session["userType"] = null;
            Session["PatientPerm"] = null;
           
            Session["RadiologyPerm"] = null;
            
            Session["RadiologyStatusPerm"] = null;
           
           Session["PermsPerm"] = null;
            
           Session["UserPerm"] = null;
            
            Session["ModalityPerm"] = null;
            
            Session["ModalityTypePerm"] = null;
            
            Session["ProcedurePerm"] = null;
            
            Session["DepartmentPerm"] = null;

            Session["StatsPerm"] = null;
            Session["AppStatsPerm"] = null;

            Session["AppsPerm"] = null;

			Session["NewsPerm"] = null;

			Session["BillsPerm"] = null;
            Session["PatientDelPerm"] = null;
            
            Session["ClinicAppoinmentIndex"] = null;

            Session["AppStatsIndex"] = null;
            Session["ClinicAppoinmentDelete"] = null;
            Session["ClinicAppoinmentEdit"] = null;
            Session["AppStatsPatient"] = null;


            return RedirectToAction("Index", "Home");

            //    return View("~/Views/Home/Index.cshtml");
        }

        //public ActionResult SetCulture(string culture)
        //{
        //    // Validate input
        //    culture = CultureHelper.GetImplementedCulture(culture);
        //    // Save culture in a cookie
        //    HttpCookie cookie = Request.Cookies["_culture"];
        //    if (cookie != null)
        //        cookie.Value = culture;   // update cookie value
        //    else
        //    {
        //        cookie = new HttpCookie("_culture");
        //        cookie.Value = culture;
        //        cookie.Expires = DateTime.Now.AddYears(1);
        //    }
        //    Response.Cookies.Add(cookie);
        //    return RedirectToAction("Index");
        //}
    }
}