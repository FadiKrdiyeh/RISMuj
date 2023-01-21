using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using RIS.Models;
using RIS;

namespace WebApplication1.Controllers
{
    public class ScheduleController : Controller
    {
        // GET: Schedule
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
            if (!RIS.Models.User.hasPerm(userId, Perms.RadiologyCreateSchedualed))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }


            ViewBag.Modalities=Modality.getData();
            return View();
        }

        [HttpPost]
        public ActionResult checkOccupancy(List<string> dates)
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
            List<SheduleData> sd = new List<SheduleData>();
            for (int i=0;i<dates.Count;i++)
            {
                string temp = dates[i];
                SheduleData s=Schedule.getOrdersByPeriod(temp);
                sd.Add(s);
            }
            var js = Json(new { data = sd }, JsonRequestBehavior.AllowGet);
            //JsonResult jj=Json()
            return js;
        }
        [HttpPost]
        public ActionResult pyramidSearch(List<string> dates,List<string> myShorthours, List<string> myminutes,string mod)
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
            List<SheduleData> sd = new List<SheduleData>();
            //string[] myShorthours = { "00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23" };
            string step = ((60 / myminutes.Count)-1).ToString();
            int period = 60 / myminutes.Count;
            int indexStep = (60 / period)*24;
            for (int i=0;i<dates.Count;i++)
            {
                sd.Add(new SheduleData { orderId="-1",tdId=dates[i]});
            }
            // Now get Days
            List<string> onlyDays = new List<string>();
            int n = 0;
            for (int i = 0; i < dates.Count; )
            {

                string[] temp = dates[i].Split('_');
                //string myDate = temp[0].Replace("-", "");
                //onlyDays.Add(temp[0]);
                bool res_day = Schedule.getOrdersByDay(temp[0]);
                if(res_day)
                {
                    for (int j = 0; j < 24; j++)
                    {
                        bool res_hour = Schedule.getOrdersByHour(temp[0], myShorthours[j]);
                        if(res_hour)
                        {
                            for (int k=0;k<myminutes.Count;k++)
                            {
                                string orderId = Schedule.getOrdersByStep(temp[0], myShorthours[j], myminutes[k],step,mod);
                                if(orderId!="-1")
                                {
                                    sd[(n* indexStep) + j* myminutes.Count + k].orderId = orderId;
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
        public ActionResult previewOrder(string id)
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
            PreviewOrder p = Schedule.previewOrder(int.Parse(id));
            List<PreviewOrder> lp = new List<PreviewOrder>();
            lp.Add(p);
            var js = Json(new { data = lp }, JsonRequestBehavior.AllowGet);
            //JsonResult jj=Json()
            return js;
        }

        public ActionResult deleteOrder(int id)
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
            string res = Schedule.deleteOrder(id);
            if (res == "")
                res = "تم الحذف بنجاح";
            else
                res = "حدث خطأ أثناء عملية الحذف";
            var js = Json(res , JsonRequestBehavior.AllowGet);
            //JsonResult jj=Json()
            return js;
        }

        public ActionResult editOrder(int id, string orderDate)
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
            string res = Schedule.editOrder( id,  orderDate);

            var js = Json(res, JsonRequestBehavior.AllowGet);
            //JsonResult jj=Json()
            return js;
        }

    }
}