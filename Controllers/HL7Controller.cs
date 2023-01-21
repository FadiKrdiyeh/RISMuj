using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RIS.Models;
using HL7_TCP;
using HL7_TCP.Extensions;
using HL7_TCP.Web;

namespace RIS.Controllers
{
    public class HL7Controller : Controller
    {
        /// <summary>
        /// JavaScript replaces newline \r\n with underscore. Use this value as a replacement to for newlines. 
        /// Replace in when saving to cookie, and replace out with newline when reading from cookie/
        /// </summary>
        const string NewLineToken = @"{-}";
        public ActionResult Index()
        {
            var vm = GatherValuesFromCookies();
            return View(vm);
        }

        private SendHL7ViewModel GatherValuesFromCookies()
        {
            return new SendHL7ViewModel
            {
                DestinationServer = Cookies.GetCookieVal("Server"),
                HL7MessageToSend = Cookies.GetCookieVal("HL7Message-").Replace(NewLineToken, Environment.NewLine),
                DestinationPort = Cookies.GetCookieVal("Port").ToNullableInt(),
                NumMessages = Cookies.GetCookieVal("NumMsgs").ToNullableInt()
            };
        }

        [HttpPost]
        public ActionResult SendHL7Message(SendHL7ViewModel model)
        {
            if (TryValidateModel(model))
            {
                SetUserValuesToCookie(model);

                HL7Send hl7Service = new HL7Send();
                ViewBag.Message = hl7Service.SendBatchMessages(model, Session["pacsSrvrIp"].ToString());
            }
            return View("Index", model);
        }

        private void SetUserValuesToCookie(SendHL7ViewModel model)
        {
            // 20170629 095023
            String currentDate = DateTime.Now.ToString("yyyymmddH:mm:ss");
            Cookies.SetCookie("NumMsgs", model.NumMessages);
            String hL7Message = @"MSH|^~\&|DCM4CHEE|DCM4CHEE|DCM4CHEE|DCM4CHEE|" + model.startDateTime + "||ORM^O01^ORM_O01|168715|P|2.5" + NewLineToken
            + "PID||" + model.patientId + "|" + model.patientIdList + "||" + model.patientGivenName + "^" + model.patientFamilyName + NewLineToken
            + "ORC|NW|" + model.commOrderPONum + "|||||^^^" + model.startDateTime + NewLineToken
            + "OBR|1|1|2|1100|||||||||||||"+model.aeTitle+ "|" + model.sStationName + "|" + model.obsOrderPFNum + "|SPS_ID||||CT||||||||||PERFOMING_TECH" + NewLineToken
            + "ZDS|1.2.4.0.13.1.432252867.1552647.1^DCM4CHEE^StationName";

//            hL7Message = @"MSH|^~\&|DCM4CHEE|DCM4CHEE|DCM4CHEE|DCM4CHEE|20160629095023||ORM^O01^ORM_O01|168715|P|2.5" +
//"PID || 1 | A - 10002 || Ryad ^ Shaker" +
//"PV1 || RAD ||||| REF_PHYS_ID ^ REF_PHYS_FIRST ^ REF_PHYS_LAST |^ ReferringPhysLast ^ ReferringPhysFirst" +
//"ORC | NW | 1 |||||^^^ 20150414120000" +
//"OBR | 1 | 1 | 2 | 1100 ||||||||||||||| 2 | SPS_ID |||| CT |||||||||| PERFOMING_TECH" +
//"ZDS | 1.2.4.0.13.1.432252867.1552647.1 ^ DCM4CHEE ^ StationName";

            Cookies.SetCookie("HL7Message-", hL7Message);

            model.HL7MessageToSend = hL7Message.Replace(NewLineToken, Environment.NewLine);
            //model.HL7MessageToSend = hL7Message;
            //  String convertedMsg = hL7Message;
            //  convertedMsg = model.HL7MessageToSend;

            Cookies.SetCookie("Port", model.DestinationPort);
            Cookies.SetCookie("Server", model.DestinationServer);
        }
    }

}