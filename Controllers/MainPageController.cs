using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RIS.Controllers
{
    public class MainPageController : Controller
    {
        // GET: MainPage
        public ActionResult Index()
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
            try
            {
                string u = Session["userName"].ToString();
            }
            catch
            {
                return RedirectToAction("Index", "Home", new { });
            }

            return View();
        }
    }
}
