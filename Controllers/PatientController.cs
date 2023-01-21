using HL7_TCP.Web;
using RIS.Models;
using RISDB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using RIS.ViewModels;
using System.Threading;
using System.Net.NetworkInformation;

namespace RIS.Controllers
{
    public class PatientController : Controller
    {
        const string NewLineToken = @"{-}";
        // GET: Patient
        public ActionResult Index( [DefaultValue(1.0)] double page, [DefaultValue(1.0)] double count, string firstname, string middlename, string lastname, string mothername, string givenid, int? gender)
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
            if (!RIS.Models.User.hasPerm(userId, Perms.PatientIndex))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }


            //if reception get registered by him
            Models.Group ReciptionGrp = Models.Group.SelectByName(ConfigVar.recepGroup);
            Boolean Isreception = false;
            List<Models.Group> gl= Models.Group.getUserGroups(userId);
            if (gl.Contains(ReciptionGrp))
            {
                Isreception = true;
            }



            double RowsPerPage = RequiredValues.getRowsPerPgById((int)ReqPatientVals.rowsPerPage).reqRowsPerPage;
            List<Patient> mtList = new List<Patient>();
            if(Isreception)
             mtList = Patient.getSearchData(page, out count, RowsPerPage, firstname, middlename, lastname, mothername, givenid, gender, userId);
            else
                mtList = Patient.getSearchData(page, out count, RowsPerPage, firstname, middlename, lastname, mothername, givenid, gender,null);

            ViewData["PageCnt"] = count;
            ViewData["count"] = count;
            ViewData["pageNBr"] = page;
            ViewData["page"] = page;
            ViewData["gender"] = Patient.GetGenderList("");
            ViewData["ListParameters"] = new { page, count };
            return View(mtList.ToList());
        }

        [HttpPost]
        public ActionResult Index([DefaultValue(1.0)] double page, [DefaultValue(1.0)] double count, string firstname, string middlename, string lastname, string mothername, string givenid, int? gender, string ss)
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
            if (!RIS.Models.User.hasPerm(userId, Perms.PatientIndex))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }

            double RowsPerPage = RequiredValues.getRowsPerPgById((int)ReqPatientVals.rowsPerPage).reqRowsPerPage;

          //  List<Patient> mtList = Patient.getData(page, out count, RowsPerPage);
            List<Patient> mtList = Patient.getSearchData(page, out count, RowsPerPage, firstname, middlename, lastname, mothername, givenid,  gender,null);
            
            ViewData["firstname"] = (firstname == null) ? "" : firstname;
            ViewData["middlename"] = (middlename == null) ? "" : middlename;
            ViewData["lastname"] = (lastname == null) ? "" : lastname;
            ViewData["middlename"] = (mothername == null) ? "" : mothername;
            ViewData["middlename"] = (givenid == null) ? "" : givenid;
            ViewData["gender"] = Patient.GetGenderList(gender.ToString());
            ViewData["PageCnt"] = count;
            ViewData["count"] = count;
            ViewData["pageNBr"] = page;
            ViewData["page"] = page;
            ViewData["ListParameters"] = new { page, count };
            return View(mtList.ToList());
        }

        public ActionResult Create()
        {
            Session["pacsSrvrIp"] = "";
            Thread check_Connection = new Thread(() =>
            {
                try
                {
                    String pacsSrvrIp = "192.168.3.250";
                    Ping myPing = new Ping();
                    PingReply reply = myPing.Send("192.168.3.251", 1000);
                    if (reply.Status.ToString() == "Success")
                    {
                        pacsSrvrIp = "192.168.3.251";
                    }
                    Session["pacsSrvrIp"] = pacsSrvrIp;
                }
                catch
                { }
            });
            check_Connection.Start();
            check_Connection.Join();
            string uName = "";
            try
            {
                uName = Session["userName"].ToString();
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
            ViewData["acceptanceType"] = GeniricIndex.GetGeniricIndexList(true, "", "acceptanceType");

            int userId = RIS.Models.User.getUserByUname(uName).num;
            if (!RIS.Models.User.hasPerm(userId, Perms.PatientCreate))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }
            //required fields
            List<RequiredValues> rvl = RequiredValues.getRequiredValuessList();
            List<RequiredValues> Arvl = RequiredValues.getActuallyRequiredValuessList();
            //RequiredValues doct = (RequiredValues)(rvl.Find(tt => tt.num == (int)ReqPatientVals.doctor));
            ViewData["rvl"] = rvl;
            ViewData["Arvl"] = Arvl;
            ViewBag.arvl = Arvl.Count;
            Patient pt = new Patient();
            

               return View(pt);
            
        }

        [HttpPost]
        public ActionResult Create(Patient pt)
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
            if (!RIS.Models.User.hasPerm(userId, Perms.PatientCreate))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }

            //TODO server side validation
            List<RequiredValues> rvl = RequiredValues.getRequiredValuessList();
            ViewData["rvl"] = rvl;

            ViewData["acceptanceType"] = GeniricIndex.GetGeniricIndexList(true, "", "acceptanceType");

            //
            pt.insertUser = userId;
            int sqNum = OracleRIS.GetOracleSequenceValue("PATIENT_SEQ");
            pt.num = sqNum;
            //  String currentDate = DateTime.Now.ToString("ssmmHH");
            //   String currentDate = DateTime.Now.ToString("ssmm");
            String currentDate = DateTime.Now.ToString("ss");
            //   pt.givenid = "A-" + sqNum + currentDate;
            pt.givenid =  sqNum + currentDate;
            pt.id = sqNum;

            //first name
            if (isArabic(pt.firstname))
            {
                pt.translatedFname = toEnglish(pt.firstname);
            }
            else {
                pt.translatedFname = pt.firstname;
            }

            //last name
            if (isArabic(pt.lastname))
            {
                pt.translatedLname = toEnglish(pt.lastname);
            }
            else
            {
                pt.translatedLname = pt.lastname;
            }

            //father name
            if (isArabic(pt.middlename))
            {
                pt.translatedFathername = toEnglish(pt.middlename);
            }
            else
            {
                pt.translatedFathername = pt.middlename;
            }

            //mother name
            if (isArabic(pt.mothername))
            {
                pt.translatedMothername = toEnglish(pt.mothername);
            }
            else
            {
                pt.translatedMothername = pt.mothername;
            }

            try
            {
                if (ModelState.IsValid)
                {

                    string ex = Patient.Insert(pt);
                    if (string.IsNullOrEmpty(ex))
                    {
                        if(Models.User.hasPerm(userId, Perms.RadiologyCreate))
                            return RedirectToAction("Create", "Radiology", new { pId = pt.id });
                        else if (Models.User.hasPerm(userId, Perms.ClinicAppoinmentCreate))
                            return RedirectToAction("Create", "Appoinments", new { pId = pt.id });
                    }
                    else
                    {
                        ModelState.AddModelError("", RIS.Resources.Res.addPatientError);
                        Response.AppendToLog("#### ***MYLOG*** //" + DateTime.Now.ToShortTimeString() + "//" + ex);
                    }
                }
                return View(pt);
            }
            catch (Exception e)
            {
                Response.AppendToLog("#### ***MYLOG*** //" + DateTime.Now.ToShortTimeString() + "//" + e.Message);
                return View(pt);
            }
          //  return RedirectToAction("Create", "Radiology", new { pId = pt.id });
        }

        public ActionResult Edit(int id)
        {
            Session["pacsSrvrIp"] = "";
            Thread check_Connection = new Thread(() =>
            {
                try
                {
                    String pacsSrvrIp = "192.168.3.250";
                    Ping myPing = new Ping();
                    PingReply reply = myPing.Send("192.168.3.251", 1000);
                    if (reply.Status.ToString() == "Success")
                    {
                        pacsSrvrIp = "192.168.3.251";
                    }
                    Session["pacsSrvrIp"] = pacsSrvrIp;
                }
                catch
                { }
            });
            check_Connection.Start();
            check_Connection.Join();
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
            if (!RIS.Models.User.hasPerm(userId, Perms.PatientEdit))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }

            List<RequiredValues> rvl = RequiredValues.getRequiredValuessList();
            List<RequiredValues> Arvl = RequiredValues.getActuallyRequiredValuessList();
            ViewData["rvl"] = rvl;
            ViewData["Arvl"] = Arvl;
            ViewBag.arvl = Arvl.Count;

            Patient pt = Patient.Select(id);
            ViewData["acceptanceType"] = GeniricIndex.GetGeniricIndexList(true, Patient.getAccTypeName(pt.acceptanceType), "acceptanceType");
            ViewData["ATypeName"]= Patient.getAccTypeName(pt.acceptanceType);
            return View(pt);
        }
        
        [HttpPost]
        public ActionResult Edit(Patient pt)
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
            if (!RIS.Models.User.hasPerm(userId, Perms.PatientEdit))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }

            //first name
            if (isArabic(pt.firstname))
            {
                pt.translatedFname = toEnglish(pt.firstname);
            }
            else
            {
                pt.translatedFname = pt.firstname;
            }

            //last name
            if (isArabic(pt.lastname))
            {
                pt.translatedLname = toEnglish(pt.lastname);
            }
            else
            {
                pt.translatedLname = pt.lastname;
            }

            //father name
            if (isArabic(pt.middlename))
            {
                pt.translatedFathername = toEnglish(pt.middlename);
            }
            else
            {
                pt.translatedFathername = pt.middlename;
            }

            //mother name
            if (isArabic(pt.mothername))
            {
                pt.translatedMothername = toEnglish(pt.mothername);
            }
            else
            {
                pt.translatedMothername = pt.mothername;
            }
            List<RequiredValues> rvl = RequiredValues.getRequiredValuessList();
            List<RequiredValues> Arvl = RequiredValues.getActuallyRequiredValuessList();
            ViewData["rvl"] = rvl;
            ViewData["Arvl"] = Arvl;
            ViewBag.arvl = Arvl.Count;

            ViewData["acceptanceType"] = GeniricIndex.GetGeniricIndexList(true, Patient.getAccTypeName(pt.acceptanceType), "acceptanceType");

            HL7Send hl7Service = new HL7Send();
            SendHL7ViewModel msg2send = new SendHL7ViewModel();

            ConnectionConfigs risConfig = ConnectionConfigs.getConfig();
            msg2send.DestinationServer = risConfig.pacsIp;
            msg2send.DestinationPort = risConfig.pacsPort;
            
            msg2send.NumMessages = 1;

            string patientGender = (pt.gendre == 0) ? "F" : "M";
            string patientBdate = "";

            if(pt.birthdate !=null)
            patientBdate = ((DateTime)pt.birthdate).Date.ToString("yyyyMMdd");

            String hL7Message = @"MSH|^~\&|DCM4CHEE|DCM4CHEE|DCM4CHEE|DCM4CHEE|" + ConnectionConfigs.getPacsTime().ToString().Replace(":", "") + "||ADT^A08|168715|P|2.5" + NewLineToken
                + "PID||1|" + pt.givenid + "||" + pt.translatedFname + "^" + pt.translatedFathername + "^" + pt.translatedLname  + "|" + pt.translatedMothername + "|" +patientBdate+"|"+ patientGender + NewLineToken;

            msg2send.HL7MessageToSend = hL7Message.Replace(NewLineToken, Environment.NewLine);
 

            bool res = hl7Service.SendBatchMessages(msg2send, Session["pacsSrvrIp"].ToString());

            if (!res)
            {
                Response.AppendToLog("#### ***MYLOG*** //" + DateTime.Now.ToShortTimeString() + "// update patient not exists in pacs ");
                //return View(pt);
            }
            try
                {
                    try
                    {
                    //store in old patient
                    
                    Patient oldPt = Patient.Select(pt.num);
                    int sqNum = OracleRIS.GetOracleSequenceValue("OLDPATIENT_SEQ");
                    oldPt.num = sqNum;
                    oldPt.updateDate = DateTime.Now;
                    oldPt.updateUser = userId;
                    oldPt.regStatus = (int)RegStatus.update;

                    //end store in old patient
                    string storeOld = Patient.InsertOld(oldPt);
                    string ex = "";
                    if (string.IsNullOrEmpty(storeOld))
                    {
                        ex = Patient.Edit(pt);
                        if (string.IsNullOrEmpty(ex))
                        {
                            return RedirectToAction("Index", new { });
                        }
                    }
                        ModelState.AddModelError("", RIS.Resources.Res.Error);
                        Response.AppendToLog("#### ***MYLOG*** //" + DateTime.Now.ToShortTimeString() + "// " + ex);
                    Response.AppendToLog("#### ***MYLOG*** //" + DateTime.Now.ToShortTimeString() + "// " + storeOld);

                    return View(pt);
                    }
                    catch (Exception e)
                    {
                        Response.AppendToLog("#### ***MYLOG*** //" + DateTime.Now.ToShortTimeString() + "// " + e.Message);
                        return View(pt);
                    }
                }
                catch (Exception ee)
                {
                    Response.AppendToLog("#### ***Pat edit in pacs *** //" + DateTime.Now.ToShortTimeString() + "//" + ee.Message);

                    //return View();
                }
                return View(pt);
            
            }
                
        
        public ActionResult Delete(int id)
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
            if (!RIS.Models.User.hasPerm(userId, Perms.PatientDelete))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }

            Patient mt = Patient.Select(id);
            return View(mt);
        }
        
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
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

            Patient mt = Patient.Select(id);
            try
            {
                
                string reason = collection["reason"].ToString();


                mt.updateDate = DateTime.Now;
                mt.updateUser = userId;
                mt.regStatus = (int)RegStatus.delete;
                mt.updateDeleteReason = reason;
                string ex = Patient.delete(mt);
                if (string.IsNullOrEmpty(ex))
                    return RedirectToAction("Index", new { });
                if(ex == RIS.Resources.Res.CantDeletePatient)
                    ModelState.AddModelError("", ex);
                else
                {
                    ModelState.AddModelError("", RIS.Resources.Res.Error);
                    Response.AppendToLog("#### ***MYLOG*** //" + DateTime.Now.ToShortTimeString() + "// " + ex);
                }
                return View(mt);
            }
            catch (Exception e)
            {
                Response.AppendToLog("#### ***MYLOG*** //" + DateTime.Now.ToShortTimeString() + "// " + e.Message);
                return View(mt);
            }
        }

        public ActionResult Details(int id)
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
            if (!RIS.Models.User.hasPerm(userId, Perms.PatientDetails))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }

            Patient pt = Patient.Select(id);
            if (pt.id == 0)
            {
                TempData["Message"] = "تم حذف بيانات هذا المريض";
                return View();

            }
            List<Radiology> ptOrders = Radiology.getPatientsOrders(pt.id);
            List<Appoinments> ptApps = Appoinments.getPatientApps(pt.id);
            PatientDetails pd = new PatientDetails(pt, ptOrders, ptApps);
            return View(pd);
        }

        public ActionResult PatientByBarcode(string givenid)
        {
            if (givenid == "")
                return RedirectToAction("Index");
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
            if (!RIS.Models.User.hasPerm(userId, Perms.PatientDetails))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }

            string gId = givenid;

            if (givenid[0] == '0')
                gId = givenid.TrimStart('0');

            Patient pt = Patient.SelectByGid(gId);
            PatientDetails pd = new PatientDetails();
            pd.patient = pt;
            pd.patientOrders = Radiology.getPatientsOrders(pt.num);

            if (pd == null)
                return RedirectToAction("Index");
            return View("Details", pd);
            //return RedirectToAction("Details", new { id=pt.id});
        }


        public string toEnglish(string araString)
        {
            string result = "";
            StringBuilder sb = new StringBuilder(araString);
            #region Mapping letters
            sb.Replace("ا", "a");
            sb.Replace(" ", " ");

            sb.Replace("ء", "a");
            sb.Replace("ؤ", "ou");
            sb.Replace("ئ", "e");
            sb.Replace("أ", "a");
            sb.Replace("ة", "a");
            sb.Replace("إ", "e");
            sb.Replace("ى", "a");
            sb.Replace("ب", "b");
            sb.Replace("ت", "t");
            sb.Replace("ث", "th");
            sb.Replace("ج", "g");
            sb.Replace("ح", "h");
            sb.Replace("خ", "kh");
            sb.Replace("د", "d");
            sb.Replace("ذ", "th");
            sb.Replace("ر", "r");
            sb.Replace("ز", "z");
            sb.Replace("س", "s");
            sb.Replace("ش", "sh");
            sb.Replace("ص", "s");
            sb.Replace("ض", "d");
            sb.Replace("ط", "t");
            sb.Replace("ظ", "z");
            sb.Replace("ع", "a");
            sb.Replace("غ", "g");
            sb.Replace("ف", "f");
            sb.Replace("ق", "k");
            sb.Replace("ك", "k");
            sb.Replace("ل", "l");
            sb.Replace("م", "m");
            sb.Replace("ن", "n");
            sb.Replace("ه", "h");
            sb.Replace("و", "w");
            sb.Replace("ي", "y");
            #endregion
            result = sb.ToString();
            return result;
        }
        public Boolean isArabic(string t)
        {
            Boolean s = false;
            if(t !=null)
            if (Regex.IsMatch(t, @"\p{IsArabic}"))
                s = true;
            return s;
        }
        public string CheckNationNumExists(string nNum)
        {
            string isExists = "false";
            nNum = nNum.Trim();
            Patient pt = Patient.GetByNatioanlNum(nNum);
            if (pt.num > 0)
            {
                Patient pp = Patient.Select(pt.num);
                isExists = pt.num + "-.-" + pt.firstname + "-.-" + pt.lastname;
            }

            return isExists;
        }
        //CheckNationNumExistsEdit
            public string CheckNationNumExistsEdit(string nNum,string CurrentPat)
        {
            string isExists = "false";
            nNum = nNum.Trim();
            int cp = int.Parse(CurrentPat);
            Patient pt = Patient.GetByNatioanlNum(nNum);
            if (pt.num > 0 && pt.num != cp)
            {
                Patient pp = Patient.Select(pt.num);
                isExists = pt.num + "-.-" + pt.firstname + "-.-" + pt.lastname;
            }

            return isExists;
        }


        //CheckSimilarPatiens 
        public JsonResult CheckSimilarPatiens(string firstname, string lastname, string middlename, string mothername)
        {
            string isExists = "false";
            List<Patient> mtList = Patient.getSemilarData(firstname, lastname, middlename, mothername);
      
            //foreach (Patient p in mtList)
            //{

            //}
            //if (pt.num > 0)
            //{
            //    Patient pp = Patient.Select(pt.num);
            //    isExists = pt.num + "-.-" + pt.firstname + "-.-" + pt.lastname;
            //}
            return Json(mtList);
         //   return isExists;
        }

        //audit Patient


        public ActionResult IndexAudit([DefaultValue(1.0)] double page, [DefaultValue(1.0)] double count, string firstname, string middlename, string lastname, string mothername, string givenid, int? gender, int? regStat, string beginDate, string endDate)
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
            if (!RIS.Models.User.hasPerm(userId, Perms.PatientIndex))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }

            double RowsPerPage = RequiredValues.getRowsPerPgById((int)ReqPatientVals.rowsPerPage).reqRowsPerPage;

            List<Patient> mtList = Patient.getAuditSearchData(page, out count, RowsPerPage, firstname, middlename, lastname, mothername, givenid, gender, regStat, beginDate, endDate);
            ViewData["PageCnt"] = count;
            ViewData["count"] = count;
            ViewData["pageNBr"] = page;
            ViewData["page"] = page;
            ViewData["gender"] = Patient.GetGenderList("");
            ViewData["regStat"] = Patient.GetRegList("");
            ViewData["stayInSeach"] = "0";

            ViewData["ListParameters"] = new { page, count };
            return View(mtList.ToList());
        }

        [HttpPost]
        public ActionResult IndexAudit([DefaultValue(1.0)] double page, [DefaultValue(1.0)] double count, string firstname, string middlename, string lastname, string mothername, string givenid, int? gender, string ss, int? regStat, string beginDate, string endDate)
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
            if (!RIS.Models.User.hasPerm(userId, Perms.PatientIndex))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }

            double RowsPerPage = RequiredValues.getRowsPerPgById((int)ReqPatientVals.rowsPerPage).reqRowsPerPage;

            //  List<Patient> mtList = Patient.getData(page, out count, RowsPerPage);
            List<Patient> mtList = Patient.getAuditSearchData(page, out count, RowsPerPage, firstname, middlename, lastname, mothername, givenid, gender, regStat, beginDate, endDate);

            ViewData["firstname"] = (firstname == null) ? "" : firstname;
            ViewData["middlename"] = (middlename == null) ? "" : middlename;
            ViewData["lastname"] = (lastname == null) ? "" : lastname;
            ViewData["mothername"] = (mothername == null) ? "" : mothername;
            ViewData["givenid"] = (givenid == null) ? "" : givenid;
            ViewData["gender"] = Patient.GetGenderList(gender.ToString());
            ViewData["beginDate"] = (beginDate == null) ? "" : beginDate;
            ViewData["endDate"] = (endDate == null) ? "" : endDate;
            ViewData["regStat"] = Patient.GetRegList(regStat.ToString());
            ViewData["stayInSeach"] = "1";

            ViewData["PageCnt"] = count;
            ViewData["count"] = count;
            ViewData["pageNBr"] = page;
            ViewData["page"] = page;
            ViewData["ListParameters"] = new { page, count };
            return View(mtList.ToList());
        }

        public ActionResult DetailsAudit(int id)
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
            if (!RIS.Models.User.hasPerm(userId, Perms.PatientDetails))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }

            Patient pt = Patient.SelectOld(id);
            List<Radiology> ptOrders = Radiology.getPatientsOrders(pt.id);
            List<Appoinments> ptApps = Appoinments.getPatientApps(pt.id);
            PatientDetails pd = new PatientDetails(pt, ptOrders, ptApps);
            return View(pd);
        }


        //end Audit Patient

    }
}