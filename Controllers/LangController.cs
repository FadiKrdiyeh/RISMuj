using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using System.Threading;
using System.Web.Services;

namespace locale.Controllers
{
    public class LangController : Controller
    {
        //
        // GET: /Lang/

        public ActionResult Index()
        {
            return View();
        }

        [WebMethod()]
        public void Change(string lang)
        {
            if (lang != null)
            {

                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(lang);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);

            }

            HttpCookie cookie = new HttpCookie("Language");
            cookie.Value = lang;
            Response.Cookies.Add(cookie);

            //return View();
        }

    }
}
