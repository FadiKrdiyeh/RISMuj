using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RIS.Models;
using RISDB;
using HL7_TCP.Web;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.ComponentModel;
using System.Threading;
using System.Net.NetworkInformation;

namespace RIS.Controllers
{
    /// <summary>
    /// This controller deals with all actions on the radiology webpage
    /// </summary>
    public class RadiologyController : Controller
    {

        const string NewLineToken = @"{-}";
        // GET: Radiology
        /// <summary>
        /// This action is called when radiology webpage is accessed and for paging
        /// </summary>
        /// <permission cref="Perms.RadiologyIndex">the user has to have the RadiologyIndex permission to access
        /// the radiology orders page</permission>
        /// <param name="page">the page number</param>
        /// <param name="count">total number of pages</param>
        /// <param name="docId">the order's document ID</param>
        /// <param name="firstname">first name of the patient that the order belongs to</param>
        /// <param name="lastname">first name of the patient that the order belongs to</param>
        /// <param name="departments">the department ID</param>
        /// <param name="modalities">the modality ID</param>
        /// <returns>radiology orders index view</returns>
        public ActionResult Index([DefaultValue(1.0)] double page, [DefaultValue(1.0)] double count, string docId, string firstname, string lastname, int? departments, int? modalities)
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
            if (!RIS.Models.User.hasPerm(userId, Perms.RadiologyIndex))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }


            string s = Session["userType"].ToString();
            string d = (s == "1") ? departments.ToString() : Session["userDep"].ToString();

            double RowsPerPage = RequiredValues.getRowsPerPgById((int)ReqPatientVals.rowsPerPage).reqRowsPerPage;

            List<Radiology> ordrs = Radiology.getOrders(page, out count, RowsPerPage, docId, firstname, lastname, d, modalities.ToString());

            ViewData["PageCnt"] = count;
            ViewData["count"] = count;
            ViewData["pageNBr"] = page;
            ViewData["page"] = page;
            ViewData["stayInSeach"] = "0";

            //ViewData["departments"] = Departement.GetDepartementListNames(true, "");
            //ViewData["modalities"] = Modality.GetModalitysList(true, "");
            ViewBag.departments = (s == "1") ? Departement.GetDepartementListNames(true, "") : Departement.GetDepartementListofTheuser(true, d, "");
            ViewBag.modalities = (s == "1") ? Modality.GetModalitysList(true, "") : Modality.GetModalitysListByDepId(true, d, "");
            ViewData["ListParameters"] = new { page, count };

            return View(ordrs);
        }

        /// <summary>
        /// This action is called when the user is doing search in orders page
        /// </summary>
        /// <param name="page">the page number</param>
        /// <param name="count">total number of pages</param>
        /// <param name="docId">the order's document ID</param>
        /// <param name="firstname">first name of the patient that the order belongs to</param>
        /// <param name="lastname">first name of the patient that the order belongs to</param>
        /// <param name="departments">the department ID</param>
        /// <param name="modalities">the modality ID</param>
        /// <param name="ss">just to defferentiate it from the GET action</param>
        /// <returns>radiology orders index view containing the search results</returns>
        [HttpPost]
        public ActionResult Index([DefaultValue(1.0)] double page, [DefaultValue(1.0)] double count, string docId, string firstname, string lastname, string departments, string modalities, string ss)
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
            if (!RIS.Models.User.hasPerm(userId, Perms.RadiologyIndex))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }
            ViewData["stayInSeach"] = "1";

            string s = Session["userType"].ToString();
            string d = (s == "1") ? "" : Session["userDep"].ToString();

            double RowsPerPage = RequiredValues.getRowsPerPgById((int)ReqPatientVals.rowsPerPage).reqRowsPerPage;
            List<Radiology> ordrs = Radiology.getOrders(page, out count, RowsPerPage, docId, firstname, lastname, departments, modalities);

            ViewData["docId"] = (docId == null) ? "" : docId;
            ViewData["firstname"] = (firstname == null) ? "" : firstname;
            ViewData["lastname"] = (lastname == null) ? "" : lastname;

            if (s == "1")
            {
                ViewData["departments"] = (departments == "") ? Departement.GetDepartementListNames(true, "") : Departement.GetDepartementListNames(true, Departement.select(int.Parse(departments)).name);
            }
            else
            {
                ViewData["departments"] = (departments == "") ? Departement.GetDepartementListofTheuser(true, departments, "") : Departement.GetDepartementListofTheuser(true, departments, Departement.select(int.Parse(departments)).name);
            }

            if (s == "1")
            {
                ViewData["modalities"] = (modalities == "") ? Modality.GetModalitysList(true, "") : Modality.GetModalitysList(true, Modality.Select(int.Parse(modalities)).name);
            }
            else
            {
                ViewData["modalities"] = (modalities == "") ? Modality.GetModalitysListByDepId(true, departments, "") : Modality.GetModalitysListByDepId(true, departments, Modality.Select(int.Parse(modalities)).name);
            }


            ViewData["PageCnt"] = count;
            ViewData["count"] = count;
            ViewData["pageNBr"] = page;
            ViewData["page"] = page;
            ViewData["ListParameters"] = new { page, count };

            return View(ordrs);
        }

        /// <summary>
        /// This action is called when waiting orders page is accessed
        /// </summary>
        /// <permission cref="Perms.RadiologyIndex">the user has to have the RadiologyIndex permission to access
        /// the radiology orders page</permission>
        /// <param name="page">the page number</param>
        /// <param name="count">total number of pages</param>
        /// <param name="docId">the order's document ID</param>
        /// <param name="firstname">first name of the patient that the order belongs to</param>
        /// <param name="lastname">first name of the patient that the order belongs to</param>
        /// <param name="departments">the department ID</param>
        /// <param name="modalities">the modality ID</param>
        /// <returns>waiting orders view</returns>
        public ActionResult Waiting([DefaultValue(1.0)] double page, [DefaultValue(1.0)] double count, string docId, string firstname, string lastname, int? departments, int? modalities)
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
            if (!RIS.Models.User.hasPerm(userId, Perms.RadiologyIndex))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }


            string s = Session["userType"].ToString();
            string d = (s == "1") ? departments.ToString() : Session["userDep"].ToString();

            double RowsPerPage = RequiredValues.getRowsPerPgById((int)ReqPatientVals.rowsPerPage).reqRowsPerPage;

            List<Radiology> ordrs = Radiology.getWaitingOrders(page, out count, RowsPerPage, docId, firstname, lastname, d, modalities.ToString());

            ViewData["PageCnt"] = count;
            ViewData["count"] = count;
            ViewData["pageNBr"] = page;
            ViewData["page"] = page;
            ViewData["stayInSeach"] = "0";

            //ViewData["departments"] = Departement.GetDepartementListNames(true, "");
            //ViewData["modalities"] = Modality.GetModalitysList(true, "");
            ViewBag.departments = (s == "1") ? Departement.GetDepartementListNames(true, "") : Departement.GetDepartementListofTheuser(true, d, "");
            ViewBag.modalities = (s == "1") ? Modality.GetModalitysList(true, "") : Modality.GetModalitysListByDepId(true, d, "");
            ViewData["ListParameters"] = new { page, count };

            return View(ordrs);
        }


        /// <summary>
        /// This action is called when the user wants to create an order for some patient immediately
        /// </summary>
        /// <permission cref="Perms.RadiologyCreate">the user has to have the RadiologyCreate permission to create an
        /// immediate order for a patient</permission>
        /// <param name="pId">the ID of the patient</param>
        /// <returns>the create order view</returns>
        public ActionResult Create(int pId)
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
                {
                }
            });
            check_Connection.Start();
            check_Connection.Join();
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
            if (!RIS.Models.User.hasPerm(userId, Perms.RadiologyCreate))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }

            List<RequiredValues> rvl = RequiredValues.getRequiredValuessList();
            RequiredValues doct = (RequiredValues)(rvl.Find(tt => tt.num == (int)ReqPatientVals.Doctor));
            ViewData["rvlDoct"] = false;
            if (doct.requiredVal)
                ViewData["rvlDoct"] = true;

            RequiredValues doc = (RequiredValues)rvl.Find(t => t.num == (int)ReqPatientVals.DocumnetId);
            ViewData["rvlDoc"] = false;
            if (doc.requiredVal)
                ViewData["rvlDoc"] = true;

            ViewData["PageName"] = RIS.Resources.Res.CreateRad.ToString();

            //ViewData["ModalityID"] = Modality.GetModalitysList(true,"");           

            ViewData["ProcedureID"] = Procedure.GetProcedureCodes(true, "");

            //ViewData["DepartementName"] = Departement.GetDepartementList(true, "");

            string depID = Models.User.getDepID(Session["userName"].ToString()); //get the depId of the user.
            string s = Session["userType"].ToString();

            if (s == "1")
            {
                ViewData["ModalityID"] = Models.Modality.GetModalitysList(true, "");
                ViewData["DepartementName"] = Departement.GetDepartementListNames(true, "");
            }
            else
            {
                ViewData["ModalityID"] = Models.Modality.GetModalitysListByDepId(true, depID, "");
                ViewData["DepartementName"] = Departement.GetDepartementListofTheuser(true, depID, "");
            }


            ViewData["Type"] = Radiology.OrdersTypes("");

            //ViewData["Procedures"] = 
            Patient p = Patient.Select(pId);
            ViewData["_Patient"] = p.firstname + " " + p.lastname;
            ViewData["_PatientNB"] = p.givenid;

            Radiology r = new Radiology();
            r.PatientID = pId.ToString();

            ViewData["_notes"] = p.notes;


            //   r.PatientID = ViewData["pID"].ToString();
            return View(r);
        }

        /// <summary>
        /// This action creates an order for some patient and inserts it into database, also send it to the modality to be
        /// executed
        /// </summary>
        /// <param name="r">radiology object contains the order details</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(Radiology r)
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
            if (!RIS.Models.User.hasPerm(userId, Perms.RadiologyCreate))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }

            // for (int oo=0;oo<325;oo++)
            {
                r.ID = OracleRIS.GetOracleSequenceValue("ORDERS_SEQ");
                r.AccessionNumber = DateTime.Now.ToString("ssmm") + r.ID.ToString();
                r.InsertUser = (Session["mnmUserId"].ToString());
                r.StartDate = ConnectionConfigs.getPacsTime();
                //r.StartDate = DateTime.Now.ToString("yyyyMMddHmmss");
                //r.EndDate = r.StartDate;
                r.AutoExpireDate = r.StartDate;
                r.StudyID = "1.2.4.0.13.1.4.2252867." + r.ID.ToString();
                r.Status = ConnectionConfigs.SCHEDUALED;

                

                Patient popy = Patient.Select(int.Parse(r.PatientID));
                Modality mody = Modality.Select(int.Parse(r.ModalityID));

                //ViewData["ModalityID"] = Modality.GetModalitysList(true, r.ModalityID);
                string depID = Models.User.getDepID(Session["userName"].ToString()); //get the depId of the user.
                //ViewData["ModalityID"] = Models.Modality.GetModalitysListByDepId(true, depID, "");
                ViewData["ProcedureID"] = Procedure.GetProcedureCodes(true, r.ProcedureID);

                ViewData["Type"] = Radiology.OrdersTypes(r.Type.ToString());

                HL7Send hl7Service = new HL7Send();

                SendHL7ViewModel msg2send = new SendHL7ViewModel();

                //msg2send.DestinationServer="";
                ConnectionConfigs risConfig = ConnectionConfigs.getConfig();
                msg2send.DestinationServer = risConfig.pacsIp;
                msg2send.DestinationPort = risConfig.pacsPort;

                msg2send.NumMessages = 1;


                msg2send.patientId = 1;
                msg2send.patientIdList = popy.givenid;
                msg2send.patientGivenName = popy.translatedFname;
                msg2send.patientFamilyName = popy.translatedLname;
                msg2send.commOrderPONum = r.ID;
                // msg2send.startDateTime = r.StartDate;

                msg2send.startDateTime = r.StartDate;
                msg2send.aeTitle = mody.aeTitle;
                msg2send.sStationName = mody.name;
                msg2send.obsOrderPFNum = r.ID;
                Procedure procDes = Procedure.selectByCode(r.ProcedureID);

                string patientGender = (popy.gendre == 0) ? "F" : "M";
                string patientBdate = "";

                if (popy.birthdate != null)
                    patientBdate = ((DateTime)popy.birthdate).Date.ToString("yyyyMMdd");


                //Cookies.SetCookie("NumMsgs", msg2send.NumMessages);
                //////HHHH

                String hL7Message = @"MSH|^~\&|DCM4CHEE|DCM4CHEE|DCM4CHEE|DCM4CHEE|" + msg2send.startDateTime.ToString().Replace(":", "") + "||ORM^O01^ORM_O01|168715|P|2.5" + NewLineToken
              + "PID||" + msg2send.patientId + "|" + msg2send.patientIdList + "||" + msg2send.patientGivenName + "^" + msg2send.patientFamilyName + NewLineToken
              + "ORC|NW|" + msg2send.commOrderPONum + "|||||^^^" + msg2send.startDateTime.ToString().Replace(":", "") + NewLineToken
              + "OBR|1|1|2|" + r.ProcedureID + "^^^" + r.ProcedureID + "^" + procDes.englishName + "|||||||||||||" + msg2send.aeTitle + "|" + msg2send.sStationName + "|" + msg2send.obsOrderPFNum + "|" + msg2send.obsOrderPFNum + "|" + r.AccessionNumber + "|||" + mody.parentMT.name.ToString() + "||||||||||PERFOMING_TECH" + "|||||||||||" + NewLineToken
              + "ZDS|" + r.StudyID + "^DCM4CHEE^StationName";


                //String hL7Message = @"MSH|^~\&|DCM4CHEE|DCM4CHEE|DCM4CHEE|DCM4CHEE|" + msg2send.startDateTime.ToString().Replace(":", "") + "||ORM^O01^ORM_O01|168715|P|2.5" + NewLineToken
                //+ "PID||" + msg2send.patientId + "|" + msg2send.patientIdList + "||" + msg2send.patientGivenName + "^" + popy.translatedFathername + "^" + msg2send.patientFamilyName+"|"+popy.translatedMothername+"|"+patientBdate + "|" + patientGender + NewLineToken
                //+ "ORC|NW|" + msg2send.commOrderPONum + "|||||^^^" + msg2send.startDateTime.ToString().Replace(":", "") + NewLineToken
                //+ "OBR|1|1|2|" + r.ProcedureID + "^^^" + r.ProcedureID + "^" + procDes.englishName + "|||||||||||||" + msg2send.aeTitle + "|" + msg2send.sStationName + "|" + msg2send.obsOrderPFNum + "|" + msg2send.obsOrderPFNum + "|" + r.AccessionNumber + "|||" + mody.parentMT.name.ToString() + "||||||||||PERFOMING_TECH" + "|||||||||||" + NewLineToken
                //+ "ZDS|" + r.StudyID + "^DCM4CHEE^StationName";

                msg2send.HL7MessageToSend = hL7Message.Replace(NewLineToken, Environment.NewLine);
                //Cookies.SetCookie("HL7Message-", hL7Message);

                //Cookies.SetCookie("Port", msg2send.DestinationPort);
                //Cookies.SetCookie("Server", msg2send.DestinationServer);
                //String currentDate = DateTime.Now.ToString("yyyymmddH:mm:ss");
                //Cookies.SetCookie("NumMsgs", msg2send.NumMessages);

                string ssss = Session["pacsSrvrIp"].ToString();

                bool res = hl7Service.SendBatchMessages(msg2send, Session["pacsSrvrIp"].ToString());

                List<RequiredValues> rvl = RequiredValues.getRequiredValuessList();
                RequiredValues doct = (RequiredValues)(rvl.Find(tt => tt.num == (int)ReqPatientVals.Doctor));
                ViewData["rvlDoct"] = false;
                if (doct.requiredVal)
                    ViewData["rvlDoct"] = true;

                RequiredValues doc = (RequiredValues)rvl.Find(t => t.num == (int)ReqPatientVals.DocumnetId);
                ViewData["rvlDoc"] = false;
                if (doc.requiredVal)
                    ViewData["rvlDoc"] = true;

                if (res)
                {
                    try
                    {
                        // TODO: Add insert logic here

                        if (ModelState.IsValid)
                        {
                            r.insertDate = DateTime.Now;

                            string ex = Models.Radiology.AddOrder(r);
                            Response.AppendToLog("#### ***Rad inserted *** //" + DateTime.Now.ToShortTimeString() + "//" + ex);

                            //add ehaleh
                            //
                            if (!string.IsNullOrEmpty(ex))
                                //    return RedirectToAction("Index", new { });
                                //else
                                ModelState.AddModelError("", ex);
                        }
                        //return View(r);
                    }
                    catch (Exception e)
                    {
                        Response.AppendToLog("#### ***Rad inserted *** //" + DateTime.Now.ToShortTimeString() + "//" + e.Message);

                        //return View();
                    }

                    return RedirectToAction("Index", "Patient", new { });

                }
                else
                {
                    //ViewData["DepartementName"] = Departement.GetDepartementList(true, "");
                    string s = Session["userType"].ToString();

                    if (s == "1")
                    {
                        ViewData["ModalityID"] = Models.Modality.GetModalitysList(true, "");
                        ViewData["DepartementName"] = Departement.GetDepartementListNames(true, "");
                    }
                    else
                    {
                        ViewData["ModalityID"] = Models.Modality.GetModalitysListByDepId(true, depID, "");
                        ViewData["DepartementName"] = Departement.GetDepartementListofTheuser(true, depID, "");
                    }
                    //ViewData["DepartementName"] = Departement.GetDepartementListofTheuser(true, depID, "");
                    ModelState.AddModelError("", RIS.Resources.Res.RadSendError);

                }
                return View(r);

            }
        }

        /// <summary>
        /// This action is called when the user wants to create a schedualed order for some patient
        /// </summary>
        /// <permission cref="Perms.RadiologyCreateSchedualed">the user has to have the RadiologyCreateSchedualed
        /// permission to create a schedualed order for some patient</permission>
        /// <param name="pId">the ID of the patient</param>
        /// <returns>the create schedualed order view</returns>
        public ActionResult CreateSchedualed(int pId)
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

            ViewBag.Modalities = Modality.getData();

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

            ViewData["PageName"] = RIS.Resources.Res.CreateRad.ToString();

            List<RequiredValues> rvl = RequiredValues.getRequiredValuessList();
            RequiredValues doct = (RequiredValues)(rvl.Find(tt => tt.num == (int)ReqPatientVals.Doctor));
            ViewData["rvlDoct"] = false;
            if (doct.requiredVal)
                ViewData["rvlDoct"] = true;

            RequiredValues doc = (RequiredValues)rvl.Find(t => t.num == (int)ReqPatientVals.DocumnetId);
            ViewData["rvlDoc"] = false;
            if (doc.requiredVal)
                ViewData["rvlDoc"] = true;

            //ViewData["ModalityID"] = Modality.GetModalitysList(true,"");
            string depID = Models.User.getDepID(Session["userName"].ToString()); //get the depId of the user.
            string s = Session["userType"].ToString();

            if (s == "1")
            {
                ViewData["ModalityID"] = Models.Modality.GetModalitysList(true, "");
                ViewData["DepartementName"] = Departement.GetDepartementListNames(true, "");
            }
            else
            {
                ViewData["ModalityID"] = Models.Modality.GetModalitysListByDepId(true, depID, "");
                ViewData["DepartementName"] = Departement.GetDepartementListofTheuser(true, depID, "");
            }
            //ViewData["ModalityID"] = Models.Modality.GetModalitysListByDepId(true, depID, "");
            ViewData["ProcedureID"] = Procedure.GetProcedureCodes(true, "");
            //ViewData["DepartementName"] = Departement.GetDepartementList(true, "");

            //ViewData["DepartementName"] = Departement.GetDepartementListofTheuser(true, depID, "");

            ViewData["Type"] = Radiology.OrdersTypes("");

            //ViewData["Procedures"] = 
            Patient p = Patient.Select(pId);
            ViewData["_Patient"] = p.firstname + " " + p.lastname;
            ViewData["_PatientNB"] = p.givenid;


            Radiology r = new Radiology();
            r.PatientID = pId.ToString();

            ViewData["_notes"] = p.notes;


            //   r.PatientID = ViewData["pID"].ToString();
            return View(r);
        }

        /// <summary>
        /// This action creates a schedualed order for some patient and inserts it into database, also send it to the modality to be
        /// executed
        /// </summary>
        /// <param name="r">radiology object contains the order details</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateSchedualed(Radiology r)
        {
            ViewBag.Modalities = Modality.getData();

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

            List<RequiredValues> rvl = RequiredValues.getRequiredValuessList();
            RequiredValues doct = (RequiredValues)(rvl.Find(tt => tt.num == (int)ReqPatientVals.Doctor));
            ViewData["rvlDoct"] = false;
            if (doct.requiredVal)
                ViewData["rvlDoct"] = true;

            RequiredValues doc = (RequiredValues)rvl.Find(t => t.num == (int)ReqPatientVals.DocumnetId);
            ViewData["rvlDoc"] = false;
            if (doc.requiredVal)
                ViewData["rvlDoc"] = true;


            // for (int oo=0;oo<325;oo++)
            {
                r.ID = OracleRIS.GetOracleSequenceValue("ORDERS_SEQ");
                r.AccessionNumber = DateTime.Now.ToString("ssmm") + r.ID.ToString();
                //r.StartDate = ConnectionConfigs.getPacsTime();
                //r.StartDate = DateTime.Now.ToString("yyyyMMddH:mm:ss");
                r.EndDate = r.StartDate;
                r.AutoExpireDate = r.StartDate;
                r.StudyID = "1.2.4.0.13.1.4.2252867." + r.ID.ToString();

                r.InsertUser = (Session["mnmUserId"].ToString());


                Patient popy = Patient.Select(int.Parse(r.PatientID));
                Modality mody = Modality.Select(int.Parse(r.ModalityID));

                //ViewData["ModalityID"] = Modality.GetModalitysList(true, r.ModalityID);
                string depID = Models.User.getDepID(Session["userName"].ToString()); //get the depId of the user.
                string s = Session["userType"].ToString();
                //ViewData["ModalityID"] = Models.Modality.GetModalitysListByDepId(true, depID, "");
                ViewData["ProcedureID"] = Procedure.GetProcedureCodes(true, r.ProcedureID);

                ViewData["Type"] = Radiology.OrdersTypes(r.Type.ToString());


                if (r.StartDate == null)
                {
                    r.Status = ConnectionConfigs.Waiting;
                    string ex = Models.Radiology.AddWaitingOrder(r);
                    if (!String.IsNullOrEmpty(ex))
                    {
                        ModelState.AddModelError("", RIS.Resources.Res.Error);

                        if (s == "1")
                        {
                            ViewData["ModalityID"] = Models.Modality.GetModalitysList(true, "");
                            ViewData["DepartementName"] = Departement.GetDepartementListNames(true, "");
                        }
                        else
                        {
                            ViewData["ModalityID"] = Models.Modality.GetModalitysListByDepId(true, depID, "");
                            ViewData["DepartementName"] = Departement.GetDepartementListofTheuser(true, depID, "");
                        }
                        //ViewData["DepartementName"] = Departement.GetDepartementListofTheuser(true, depID, "");
                        return View();
                    }
                    else
                    {
                        return RedirectToAction("Waiting");
                    }
                }

                r.Status = ConnectionConfigs.SCHEDUALED;

                HL7Send hl7Service = new HL7Send();

                SendHL7ViewModel msg2send = new SendHL7ViewModel();

                //msg2send.DestinationServer="";
                ConnectionConfigs risConfig = ConnectionConfigs.getConfig();
                msg2send.DestinationServer = risConfig.pacsIp;
                msg2send.DestinationPort = risConfig.pacsPort;

                msg2send.NumMessages = 1;


                msg2send.patientId = 1;
                msg2send.patientIdList = popy.givenid;
                msg2send.patientGivenName = popy.translatedFname;
                msg2send.patientFamilyName = popy.translatedLname;
                msg2send.commOrderPONum = r.ID;
                // msg2send.startDateTime = r.StartDate;
                msg2send.startDateTime = r.StartDate;
                msg2send.aeTitle = mody.aeTitle;
                msg2send.sStationName = mody.name;
                msg2send.obsOrderPFNum = r.ID;
                Procedure procDes = Procedure.selectByCode(r.ProcedureID);




                //Cookies.SetCookie("NumMsgs", msg2send.NumMessages);
                String hL7Message = @"MSH|^~\&|DCM4CHEE|DCM4CHEE|DCM4CHEE|DCM4CHEE|" + msg2send.startDateTime.ToString().Replace(":", "") + "||ORM^O01^ORM_O01|168715|P|2.5" + NewLineToken
                + "PID||" + msg2send.patientId + "|" + msg2send.patientIdList + "||" + msg2send.patientGivenName + "^" + msg2send.patientFamilyName + NewLineToken
                + "ORC|NW|" + msg2send.commOrderPONum + "|||||^^^" + msg2send.startDateTime.ToString().Replace(":", "") + NewLineToken
                + "OBR|1|1|2|" + r.ProcedureID + "^^^" + r.ProcedureID + "^" + procDes.englishName + "|||||||||||||" + msg2send.aeTitle + "|" + msg2send.sStationName + "|" + msg2send.obsOrderPFNum + "|" + msg2send.obsOrderPFNum + "|" + r.AccessionNumber + "|||" + mody.parentMT.name.ToString() + "||||||||||PERFOMING_TECH" + "|||||||||||" + NewLineToken
                + "ZDS|" + r.StudyID + "^DCM4CHEE^StationName";

                msg2send.HL7MessageToSend = hL7Message.Replace(NewLineToken, Environment.NewLine);
                //Cookies.SetCookie("HL7Message-", hL7Message);

                //Cookies.SetCookie("Port", msg2send.DestinationPort);
                //Cookies.SetCookie("Server", msg2send.DestinationServer);
                //String currentDate = DateTime.Now.ToString("yyyymmddH:mm:ss");
                //Cookies.SetCookie("NumMsgs", msg2send.NumMessages);

                bool res = hl7Service.SendBatchMessages(msg2send, Session["pacsSrvrIp"].ToString());

                if (res)
                {
                    try
                    {
                        // TODO: Add insert logic here

                        if (ModelState.IsValid)
                        {

                            string ex = Models.Radiology.AddOrder(r);
                            //add ehaleh
                            //
                            if (!string.IsNullOrEmpty(ex))
                                //    return RedirectToAction("Index", new { });
                                //else
                                ModelState.AddModelError("", ex);
                        }
                        else
                        {
                            ViewData["DepartementName"] = Departement.GetDepartementListofTheuser(true, depID, "");
                            return View(r);
                        }

                    }
                    catch
                    {
                        //return View();
                    }

                    return RedirectToAction("Index", "Patient", new { });

                }
                else
                {
                    //ViewData["DepartementName"] = Departement.GetDepartementList(true, "");
                    ViewData["DepartementName"] = Departement.GetDepartementListofTheuser(true, depID, "");
                    ModelState.AddModelError("", RIS.Resources.Res.RadSendError);

                }
                return View(r);

            }
        }

        /// <summary>
        /// This action is called when we need to edit some order's details
        /// </summary>
        /// <permission cref="Perms.RadiologyEdit">the user has to have the RadiologyEdit permission to edit an order</permission>
        /// <param name="pId">the order's ID</param>
        /// <returns>the edit order view</returns>
        public ActionResult Edit(int pId)
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
            if (!RIS.Models.User.hasPerm(userId, Perms.RadiologyEdit))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }

            Radiology r = Radiology.SelectExact(pId);

            List<RequiredValues> rvl = RequiredValues.getRequiredValuessList();
            RequiredValues doct = (RequiredValues)(rvl.Find(tt => tt.num == (int)ReqPatientVals.Doctor));
            ViewData["rvlDoct"] = false;
            if (doct.requiredVal)
                ViewData["rvlDoct"] = true;

            RequiredValues doc = (RequiredValues)rvl.Find(t => t.num == (int)ReqPatientVals.DocumnetId);
            ViewData["rvlDoc"] = false;
            if (doc.requiredVal)
                ViewData["rvlDoc"] = true;

            //ViewData["ModalityID"] = Modality.GetModalitysList(true,"");
            string depID = Models.User.getDepID(Session["userName"].ToString()); //get the depId of the user.
            string s = Session["userType"].ToString();

            if (s == "1")
            {
                ViewData["ModalityID"] = Models.Modality.GetModalitysListByDepId(true, r.DepartementName, r.ModalityID);
                ViewData["DepartementName"] = Departement.GetDepartementListNames(true, r.DepartementName);
            }
            else
            {
                ViewData["ModalityID"] = Models.Modality.GetModalitysListByDepId(true, depID, r.ModalityID);
                ViewData["DepartementName"] = Departement.GetDepartementListofTheuser(true, depID, r.DepartementName);
            }
            //ViewBag.currentModality = r.ModalityID;
            ViewBag.Modalities = Modality.getData();
            ViewBag.selectedModality = r.ModalityID;
            //ViewData["ModalityID"] = Modality.GetModalitysList(true,"");
            //string depID = Models.User.getDepID(Session["userName"].ToString()); //get the depId of the user.
            //ViewData["ModalityID"] = Models.Modality.GetModalitysListByDepId(true, depID, r.ModalityID);
            ViewData["ProcedureID"] = Procedure.GetProcedureCodes(true, r.ProcedureID);
            //ViewData["DepartementName"] = Departement.GetDepartementList(true, "");

            //ViewData["DepartementName"] = Departement.GetDepartementListofTheuser(true, depID, r.DepartementName);

            ViewData["Type"] = Radiology.OrdersTypes("");

            //ViewData["Procedures"] = 
            ViewData["_Patient"] = r.parentR.firstname + " " + r.parentR.lastname;
            ViewData["_PatientNB"] = r.parentR.givenid;

            return View(r);
        }

        /// <summary>
        /// This action edits the details of an order
        /// </summary>
        /// <param name="r">a radiology object contains the new details of the order</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(Radiology r)
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
            if (!RIS.Models.User.hasPerm(userId, Perms.RadiologyEdit))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }

            {
                string tempStartDate = "";
                // This try catch for empty dates (waiting orders)
                try
                {
                     tempStartDate = r.StartDate.Replace(":", "");
                    r.StartDate = tempStartDate;
                }
                catch(Exception e)
                { }

                Patient popy = Patient.Select(int.Parse(r.PatientID));
                Modality mody = Modality.Select(int.Parse(r.ModalityID));
                ViewBag.Modalities = Modality.getData();

                List<RequiredValues> rvl = RequiredValues.getRequiredValuessList();
                RequiredValues doct = (RequiredValues)(rvl.Find(tt => tt.num == (int)ReqPatientVals.Doctor));
                ViewData["rvlDoct"] = false;
                if (doct.requiredVal)
                    ViewData["rvlDoct"] = true;

                RequiredValues doc = (RequiredValues)rvl.Find(t => t.num == (int)ReqPatientVals.DocumnetId);
                ViewData["rvlDoc"] = false;
                if (doc.requiredVal)
                    ViewData["rvlDoc"] = true;

                string depID = Models.User.getDepID(Session["userName"].ToString()); //get the depId of the user.
                string s = Session["userType"].ToString();

                if (r.Status != ConnectionConfigs.SCHEDUALED && r.Status != ConnectionConfigs.Waiting)
                {
                    ViewData["PageName"] = RIS.Resources.Res.EditRad.ToString();

                    if (s == "1")
                    {
                        ViewData["ModalityID"] = Models.Modality.GetModalitysList(true, r.ModalityID);
                        ViewData["DepartementName"] = Departement.GetDepartementListNames(true, r.DepartementName);
                    }
                    else
                    {
                        ViewData["ModalityID"] = Models.Modality.GetModalitysListByDepId(true, depID, r.ModalityID);
                        ViewData["DepartementName"] = Departement.GetDepartementListofTheuser(true, depID, r.DepartementName);
                    }
                    ViewData["ProcedureID"] = Procedure.GetProcedureCodes(true, r.ProcedureID);
                    ViewData["Type"] = Radiology.OrdersTypes("");
                    ViewData["_Patient"] = r.parentR.firstname + " " + r.parentR.lastname;
                    ViewData["_PatientNB"] = r.parentR.givenid;

                    ViewBag.selectedModality = r.ModalityID;
                    ModelState.AddModelError("", "لا يمكن تعديل الطلب");
                    return View(r);

                }

                if (r.Status == ConnectionConfigs.Waiting)
                {

                    if (string.IsNullOrEmpty(r.StartDate))
                    {
                        if (s == "1")
                        {
                            ViewData["ModalityID"] = Models.Modality.GetModalitysList(true, r.ModalityID);
                            ViewData["DepartementName"] = Departement.GetDepartementListNames(true, r.DepartementName);
                        }
                        else
                        {
                            ViewData["ModalityID"] = Models.Modality.GetModalitysListByDepId(true, depID, r.ModalityID);
                            ViewData["DepartementName"] = Departement.GetDepartementListofTheuser(true, depID, r.DepartementName);
                        }
                        ViewData["ProcedureID"] = Procedure.GetProcedureCodes(true, r.ProcedureID);

                        ViewData["Type"] = Radiology.OrdersTypes(r.Type.ToString());

                        try
                        {
                            // TODO: Add insert logic here

                            if (ModelState.IsValid)
                            {
                                // <MNM>
                                // This procedure is to get the original order and insert it in [OLDORDERS]
                                Radiology r1 = Radiology.SelectExact(r.ID);

                                // </MNM>


                                string ex = Models.Radiology.UpdateOrder(r);
                                //add ehaleh
                                //
                                if (string.IsNullOrEmpty(ex))
                                {
                                    r1.UpdatetUser = Session["mnmUserId"].ToString();
                                    r1.updateDate = DateTime.Now;
                                    r1.UpdateDeleteReason = r.UpdateDeleteReason;
                                    r1.regStatus = 1;
                                    r1.NewID = r.ID;
                                    Radiology.AddToOldOrder(r1);
                                }
                                //    return RedirectToAction("Index", new { });
                                //else
                                ModelState.AddModelError("", ex);
                            }
                            //return View(r);
                        }
                        catch
                        {
                            //return View();
                        }
                        return RedirectToAction("Waiting");
                    } else
                    {
                        ///send waiting to pacs
                        /// 






                        try
                        {
                            tempStartDate = r.StartDate.Replace(":", "");
                            r.StartDate = tempStartDate;
                        }
                        catch (Exception e)
                        { }

                        r.StudyID = Radiology.SelectExact(r.ID).StudyID;
                        HL7Send hl7Service2 = new HL7Send();

                        SendHL7ViewModel msg2send2 = new SendHL7ViewModel();

                        //msg2send.DestinationServer="";
                        ConnectionConfigs risConfig2 = ConnectionConfigs.getConfig();
                        msg2send2.DestinationServer = risConfig2.pacsIp;
                        msg2send2.DestinationPort = risConfig2.pacsPort;

                        msg2send2.NumMessages = 1;


                        msg2send2.patientId = 1;
                        msg2send2.patientIdList = popy.givenid;
                        msg2send2.patientGivenName = popy.translatedFname;
                        msg2send2.patientFamilyName = popy.translatedLname;
                        msg2send2.commOrderPONum = r.ID;
                        // msg2send.startDateTime = r.StartDate;
                        msg2send2.startDateTime = r.StartDate;
                        msg2send2.aeTitle = mody.aeTitle;
                        msg2send2.sStationName = mody.name;
                        msg2send2.obsOrderPFNum = r.ID;
                        Procedure procDes2 = Procedure.selectByCode(r.ProcedureID);




                        //Cookies.SetCookie("NumMsgs", msg2send.NumMessages);
                        String hL7Message2 = @"MSH|^~\&|DCM4CHEE|DCM4CHEE|DCM4CHEE|DCM4CHEE|" + msg2send2.startDateTime.ToString().Replace(":", "") + "||ORM^O01^ORM_O01|168715|P|2.5" + NewLineToken
                        + "PID||" + msg2send2.patientId + "|" + msg2send2.patientIdList + "||" + msg2send2.patientGivenName + "^" + msg2send2.patientFamilyName + NewLineToken
                        + "ORC|NW|" + msg2send2.commOrderPONum + "|||||^^^" + msg2send2.startDateTime.ToString().Replace(":", "") + NewLineToken
                        + "OBR|1|1|2|" + r.ProcedureID + "^^^" + r.ProcedureID + "^" + procDes2.englishName + "|||||||||||||" + msg2send2.aeTitle + "|" + msg2send2.sStationName + "|" + msg2send2.obsOrderPFNum + "|" + msg2send2.obsOrderPFNum + "|" + r.AccessionNumber + "|||" + mody.parentMT.name.ToString() + "||||||||||PERFOMING_TECH" + "|||||||||||" + NewLineToken
                        + "ZDS|" + r.StudyID + "^DCM4CHEE^StationName";

                        msg2send2.HL7MessageToSend = hL7Message2.Replace(NewLineToken, Environment.NewLine);
                        //Cookies.SetCookie("HL7Message-", hL7Message);

                        //Cookies.SetCookie("Port", msg2send.DestinationPort);
                        //Cookies.SetCookie("Server", msg2send.DestinationServer);
                        //String currentDate = DateTime.Now.ToString("yyyymmddH:mm:ss");
                        //Cookies.SetCookie("NumMsgs", msg2send.NumMessages);

                        bool res2 = hl7Service2.SendBatchMessages(msg2send2, Session["pacsSrvrIp"].ToString());

                        if (res2)

                        ///
                        {
                            // TODO: Add insert logic here

                            if (ModelState.IsValid)
                            {
                                // <MNM>
                                // This procedure is to get the original order and insert it in [OLDORDERS]
                                Radiology r1 = Radiology.SelectExact(r.ID);

                                // </MNM>

                                r.Status = ConnectionConfigs.SCHEDUALED;
                                string ex = Models.Radiology.UpdateOrder(r);
                                //add ehaleh
                                //
                                if (string.IsNullOrEmpty(ex))
                                {
                                    r1.UpdatetUser = Session["mnmUserId"].ToString();
                                    r1.updateDate = DateTime.Now;
                                    r1.UpdateDeleteReason = r.UpdateDeleteReason;
                                    r1.regStatus = 1;
                                    r1.NewID = r.ID;
                                    Radiology.AddToOldOrder(r1);
                                }
                                //    return RedirectToAction("Index", new { });
                                //else
                                ModelState.AddModelError("", ex);
                            }
                            //return View(r);
                        }
                        return RedirectToAction("Waiting");

                    }





                }
                    //ViewData["PageName"] = RIS.Resources.Res.EditRad.ToString();

                    //if (s == "1")
                    //{
                    //    ViewData["ModalityID"] = Models.Modality.GetModalitysList(true, r.ModalityID);
                    //    ViewData["DepartementName"] = Departement.GetDepartementListNames(true, r.DepartementName);
                    //}
                    //else
                    //{
                    //    ViewData["ModalityID"] = Models.Modality.GetModalitysListByDepId(true, depID, r.ModalityID);
                    //    ViewData["DepartementName"] = Departement.GetDepartementListofTheuser(true, depID, r.DepartementName);
                    //}
                    //ViewData["ProcedureID"] = Procedure.GetProcedureCodes(true, r.ProcedureID);
                    //ViewData["Type"] = Radiology.OrdersTypes("");
                    //ViewData["_Patient"] = r.parentR.firstname + " " + r.parentR.lastname;
                    //ModelState.AddModelError("", "لا يمكن تعديل الطلب");
                    //return View(r);

                

                if (s == "1")
                {
                    ViewData["ModalityID"] = Models.Modality.GetModalitysList(true, r.ModalityID);
                    ViewData["DepartementName"] = Departement.GetDepartementListNames(true, r.DepartementName);
                }
                else
                {
                    ViewData["ModalityID"] = Models.Modality.GetModalitysListByDepId(true, depID, r.ModalityID);
                    ViewData["DepartementName"] = Departement.GetDepartementListofTheuser(true, depID, r.DepartementName);
                }
                ViewData["ProcedureID"] = Procedure.GetProcedureCodes(true, r.ProcedureID);

                ViewData["Type"] = Radiology.OrdersTypes(r.Type.ToString());

                
                if (string.IsNullOrEmpty(r.StartDate))
                {
                    ViewData["PageName"] = RIS.Resources.Res.EditRad.ToString();

                    if (s == "1")
                    {
                        ViewData["ModalityID"] = Models.Modality.GetModalitysList(true, r.ModalityID);
                        ViewData["DepartementName"] = Departement.GetDepartementListNames(true, r.DepartementName);
                    }
                    else
                    {
                        ViewData["ModalityID"] = Models.Modality.GetModalitysListByDepId(true, depID, r.ModalityID);
                        ViewData["DepartementName"] = Departement.GetDepartementListofTheuser(true, depID, r.DepartementName);
                    }
                    ViewData["ProcedureID"] = Procedure.GetProcedureCodes(true, r.ProcedureID);
                    ViewData["Type"] = Radiology.OrdersTypes("");
                    ViewData["_Patient"] = r.parentR.firstname + " " + r.parentR.lastname;
                    ViewData["_PatientNB"] = r.parentR.givenid;

                    ViewBag.selectedModality = r.ModalityID;
                    ModelState.AddModelError("", "يجب تحديد تاريخ بدء الطلب");
                    return View(r);
                }
                    else
                {
                    try
                    {
                        tempStartDate = r.StartDate.Replace(":", "");
                        r.StartDate = tempStartDate;
                    }
                    catch (Exception e)
                    { }

                    r.Status = ConnectionConfigs.SCHEDUALED;
                }

                //r.StartDate = (String.IsNullOrEmpty(r.StartDate)) ? ConnectionConfigs.getPacsTime() : r.StartDate;

                //update order in pacs then in ris

                HL7Send hl7Service = new HL7Send();

                SendHL7ViewModel msg2send = new SendHL7ViewModel();

                //msg2send.DestinationServer="";
                ConnectionConfigs risConfig = ConnectionConfigs.getConfig();
                msg2send.DestinationServer = risConfig.pacsIp;
                msg2send.DestinationPort = risConfig.pacsPort;

                msg2send.NumMessages = 1;


                msg2send.patientId = 1;
                msg2send.patientIdList = popy.givenid;
                msg2send.patientGivenName = popy.translatedFname;
                msg2send.patientFamilyName = popy.translatedLname;
                msg2send.commOrderPONum = r.ID;
                msg2send.startDateTime = r.StartDate;
                msg2send.aeTitle = mody.aeTitle;
                msg2send.sStationName = mody.name;
                msg2send.obsOrderPFNum = r.ID;
                Procedure procDes = Procedure.selectByCode(r.ProcedureID);
                //String hL7Message = @"MSH|^~\&|DCM4CHEE|DCM4CHEE|DCM4CHEE|DCM4CHEE|" + msg2send.startDateTime.ToString().Replace(":", "") + "||ORM^O01^ORM_O01|168715|P|2.5" + NewLineToken
                //+ "PID||" + msg2send.patientId + "|" + msg2send.patientIdList + "||" + msg2send.patientGivenName + "^" + msg2send.patientFamilyName + NewLineToken
                //+ "ORC|XO|" + msg2send.commOrderPONum + "|||||^^^" + msg2send.startDateTime.ToString().Replace(":", "") + NewLineToken
                //+ "OBR|1|1|2|" + r.ProcedureID + "^^^" + r.ProcedureID + "^" + procDes.englishName + "|||||||||||||" + msg2send.aeTitle + "|" + msg2send.sStationName + "|" + msg2send.obsOrderPFNum + "|" + msg2send.obsOrderPFNum + "|" + r.AccessionNumber + "|||" + mody.parentMT.name.ToString() + "||||||||||PERFOMING_TECH" + "|||||||||||" + NewLineToken
                //+ "ZDS|" + r.StudyID + "^DCM4CHEE^StationName";

                String hL7Message = @"MSH|^~\&|DCM4CHEE|DCM4CHEE|DCM4CHEE|DCM4CHEE|" + msg2send.startDateTime.ToString().Replace(":", "") + "||ORM^O01^ORM_O01|168715|P|2.5" + NewLineToken
               + "PID||" + msg2send.patientId + "|" + msg2send.patientIdList + "||" + msg2send.patientGivenName + "^" + msg2send.patientFamilyName + NewLineToken
               + "ORC|XO|" + msg2send.commOrderPONum + "|||||^^^" + msg2send.startDateTime.ToString().Replace(":", "") + NewLineToken
               + "OBR|1|1|2|" + r.ProcedureID + "^^^" + r.ProcedureID + "^" + procDes.englishName + "|||||||||||||" + msg2send.aeTitle + "|" + msg2send.sStationName + "|" + msg2send.obsOrderPFNum + "|" + msg2send.obsOrderPFNum + "|" + r.AccessionNumber + "|||" + mody.parentMT.name.ToString() + "||||||||||PERFOMING_TECH" + "|||||||||||" + NewLineToken
               + "ZDS|" + r.StudyID + "^DCM4CHEE^StationName";

                msg2send.HL7MessageToSend = hL7Message.Replace(NewLineToken, Environment.NewLine);
                bool res = hl7Service.SendBatchMessages(msg2send, Session["pacsSrvrIp"].ToString());

                if (res)
                {
                    try
                    {
                        // TODO: Add insert logic here

                        if (ModelState.IsValid)
                        {
                            // <MNM>
                                // This procedure is to get the original order and insert it in [OLDORDERS]
                                Radiology r1 = Radiology.SelectExact(r.ID);

                            // </MNM>


                            string ex = Models.Radiology.UpdateOrder(r);
                            //add ehaleh
                            //
                            if (string.IsNullOrEmpty(ex))
                            {
                                r1.UpdatetUser = Session["mnmUserId"].ToString();
                                r1.updateDate = DateTime.Now;
                                r1.UpdateDeleteReason = r.UpdateDeleteReason;
                                r1.regStatus = 1;
                                r1.NewID = r.ID;
                                Radiology.AddToOldOrder(r1);
                            }
                                //    return RedirectToAction("Index", new { });
                                //else
                                ModelState.AddModelError("", ex);
                        }
                        //return View(r);
                    }
                    catch
                    {
                        //return View();
                    }

                }
                else
                {
                    //ViewData["DepartementName"] = Departement.GetDepartementList(true, "");
                    ViewData["DepartementName"] = Departement.GetDepartementListofTheuser(true, depID, "");
                    ModelState.AddModelError("", RIS.Resources.Res.RadSendError);

                }


                //

                ViewBag.selectedModality = r.ModalityID;


                return RedirectToAction("Index", new { id = r.ID });

            }
        }

        /// <summary>
        /// This action is called when the details of an order is accessed
        /// </summary>
        /// <permission cref="Perms.RadiologyDetails">the user has to have the RadiologyDetails permission to access
        /// the details of an order</permission>
        /// <param name="id">the order ID</param>
        /// <returns>the order details view</returns>
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
            if (!RIS.Models.User.hasPerm(userId, Perms.RadiologyDetails))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }

            Radiology rad = Radiology.Select(id);
            return View(rad);
        }

        /// <summary>
        /// This action is called when the user wants to delete an order
        /// </summary>
        /// <permission cref="Perms.RadiologyDelete">the user has to have the RadiologyDelete pemission to delete
        /// an order</permission>
        /// <param name="id">the order's ID</param>
        /// <returns>the delete order view</returns>
        public ActionResult Delete(int id)
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
            if (!RIS.Models.User.hasPerm(userId, Perms.RadiologyDelete))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }

            Radiology rad = Radiology.Select(id);
            return View(rad);
        }

        /// <summary>
        /// This action deletes an order from database
        /// </summary>
        /// <remarks>a completed order can't be deleted also an order under execution</remarks>
        /// <param name="id">the order's ID</param>
        /// <param name="collection"></param>
        /// <param name="UpdateDeleteReason">a string that user entered to explain the reason of deletion</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection,string UpdateDeleteReason)
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
            if (!RIS.Models.User.hasPerm(userId, Perms.RadiologyDelete))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }

            Radiology r = Radiology.SelectExact(id);
            Patient popy = Patient.Select(int.Parse(r.PatientID));
            Modality mody = Modality.Select(int.Parse(r.ModalityID));
            string depID = Models.User.getDepID(Session["userName"].ToString()); //get the depId of the user.

            if(r.Status == ConnectionConfigs.Waiting)
            {
                try
                {
                    // <MNM>
                    // This procedure is to get the original order and insert it in [OLDORDERS]
                    Radiology r1 = Radiology.SelectExact(r.ID);

                    // </MNM>
                    string ex = Radiology.DeleteOrder(r);
                    if (string.IsNullOrEmpty(ex))
                    {
                        r1.UpdatetUser = Session["mnmUserId"].ToString();
                        r1.updateDate = DateTime.Now;
                        r1.UpdateDeleteReason = UpdateDeleteReason;
                        r1.regStatus = 2;
                        Radiology.AddToOldOrder(r1);
                        return RedirectToAction("Index", new { });

                    }
                    ModelState.AddModelError("", ex);
                    return View(r);
                }
                catch
                {
                    ModelState.AddModelError("", RIS.Resources.Res.Error);
                    return View(r);
                }
            }

            if (r.Status != ConnectionConfigs.SCHEDUALED)
            {

                ModelState.AddModelError("", "لا يمكن حذف الطلب");
                return View(r);
            }


            ViewData["ModalityID"] = Models.Modality.GetModalitysListByDepId(true, depID, "");
            ViewData["ProcedureID"] = Procedure.GetProcedureCodes(true, r.ProcedureID);
            ViewData["Type"] = Radiology.OrdersTypes(r.Type.ToString());

            HL7Send hl7Service = new HL7Send();
            SendHL7ViewModel msg2send = new SendHL7ViewModel();
            ConnectionConfigs risConfig = ConnectionConfigs.getConfig();
            msg2send.DestinationServer = risConfig.pacsIp;
            msg2send.DestinationPort = risConfig.pacsPort;
            msg2send.NumMessages = 1;
            msg2send.patientId = 1;
            msg2send.patientIdList = popy.givenid;
            msg2send.patientGivenName = popy.translatedFname;
            msg2send.patientFamilyName = popy.translatedLname;
            msg2send.commOrderPONum = r.ID;
            msg2send.startDateTime = r.StartDate;
            msg2send.aeTitle = mody.aeTitle;
            msg2send.sStationName = mody.name;
            msg2send.obsOrderPFNum = r.ID;
            Procedure procDes = Procedure.selectByCode(r.ProcedureID);

            String hL7Message = @"MSH|^~\&|DCM4CHEE|DCM4CHEE|DCM4CHEE|DCM4CHEE|" + msg2send.startDateTime.ToString().Replace(":", "") + "||ORM^O01^ORM_O01|168715|P|2.5" + NewLineToken
           + "PID||" + msg2send.patientId + "|" + msg2send.patientIdList + "||" + msg2send.patientGivenName + "^" + msg2send.patientFamilyName + NewLineToken
           + "ORC|OC|" + msg2send.commOrderPONum + "|||||^^^" + msg2send.startDateTime.ToString().Replace(":", "") + NewLineToken
           + "OBR|1|1|2|" + r.ProcedureID + "^^^" + r.ProcedureID + "^" + procDes.englishName + "|||||||||||||" + msg2send.aeTitle + "|" + msg2send.sStationName + "|" + msg2send.obsOrderPFNum + "|" + msg2send.obsOrderPFNum + "|" + r.AccessionNumber + "|||" + mody.parentMT.name.ToString() + "||||||||||PERFOMING_TECH" + "|||||||||||" + NewLineToken
           + "ZDS|" + r.StudyID + "^DCM4CHEE^StationName";

            msg2send.HL7MessageToSend = hL7Message.Replace(NewLineToken, Environment.NewLine);
            bool res = hl7Service.SendBatchMessages(msg2send, Session["pacsSrvrIp"].ToString());

            if (res)
            {
                try
                {
                    // <MNM>
                    // This procedure is to get the original order and insert it in [OLDORDERS]
                    Radiology r1 = Radiology.SelectExact(r.ID);
                    
                    // </MNM>
                    string ex = Radiology.DeleteOrder(r);
                    if (string.IsNullOrEmpty(ex))
                    {
                        r1.UpdatetUser = Session["mnmUserId"].ToString();
                        r1.updateDate = DateTime.Now;
                        r1.UpdateDeleteReason = UpdateDeleteReason;
                        r1.regStatus = 2;
                        Radiology.AddToOldOrder(r1);
                        return RedirectToAction("Index", new { });

                    }
                    ModelState.AddModelError("", ex);
                    return View(r);
                }
                catch
                {
                    ModelState.AddModelError("", RIS.Resources.Res.Error);
                    return View(r);
                }
            }
            return View(r);
        }

        //change status
        public ActionResult ShowOrdrsStatus([DefaultValue(1.0)] double page, [DefaultValue(1.0)] double count, string docId, string firstname, string lastname, int? departments, int? modalities)
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
            if (!RIS.Models.User.hasPerm(userId, Perms.RadiologyOrderStatus))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }


            string s = Session["userType"].ToString();
            string d = (s == "1") ? departments.ToString() : Session["userDep"].ToString();

            double RowsPerPage = RequiredValues.getRowsPerPgById((int)ReqPatientVals.rowsPerPage).reqRowsPerPage;

            List<Radiology> ordrs = Radiology.getOrdersWithStatus(page, out count, RowsPerPage, docId, firstname, lastname, d, modalities.ToString(), "", "");

            ViewData["PageCnt"] = count;
            ViewData["count"] = count;
            ViewData["pageNBr"] = page;
            ViewData["page"] = page;
            ViewBag.departments = (s == "1") ? Departement.GetDepartementListNames(true, "") : Departement.GetDepartementListofTheuser(true, d, "");
            ViewBag.modalities = (s == "1") ? Modality.GetModalitysList(true, "") : Modality.GetModalitysListByDepId(true, d, "");
            ViewData["ListParameters"] = new { page, count };

            return View(ordrs);
        }

        [HttpPost]
        public ActionResult ShowOrdrsStatus([DefaultValue(1.0)] double page, [DefaultValue(1.0)] double count, string docId, string firstname, string lastname, string departments, string modalities, string beginDate, string endDate)
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
            if (!RIS.Models.User.hasPerm(userId, Perms.RadiologyOrderStatus))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }

            string s = Session["userType"].ToString();
            string d = (s == "1") ? "" : Session["userDep"].ToString();

            double RowsPerPage = RequiredValues.getRowsPerPgById((int)ReqPatientVals.rowsPerPage).reqRowsPerPage;
            List<Radiology> ordrs = Radiology.getOrdersWithStatus(page, out count, RowsPerPage, docId, firstname, lastname, departments, modalities, beginDate, endDate);

            ViewData["docId"] = (docId == null) ? "" : docId;
            ViewData["firstname"] = (firstname == null) ? "" : firstname;
            ViewData["lastname"] = (lastname == null) ? "" : lastname;

            ViewData["beginDate"] = (beginDate == null) ? "" : beginDate;
            ViewData["endDate"] = (endDate == null) ? "" : endDate;

            if (s == "1")
            {
                ViewData["departments"] = (departments == "") ? Departement.GetDepartementListNames(true, "") : Departement.GetDepartementListNames(true, Departement.select(int.Parse(departments)).name);
            }
            else
            {
                ViewData["departments"] = (departments == "") ? Departement.GetDepartementListofTheuser(true, departments, "") : Departement.GetDepartementListofTheuser(true, departments, Departement.select(int.Parse(departments)).name);
            }

            if (s == "1")
            {
                ViewData["modalities"] = (modalities == "") ? Modality.GetModalitysList(true, "") : Modality.GetModalitysList(true, Modality.Select(int.Parse(modalities)).name);
            }
            else
            {
                ViewData["modalities"] = (modalities == "") ? Modality.GetModalitysListByDepId(true, departments, "") : Modality.GetModalitysListByDepId(true, departments, Modality.Select(int.Parse(modalities)).name);
            }


            ViewData["PageCnt"] = count;
            ViewData["count"] = count;
            ViewData["pageNBr"] = page;
            ViewData["page"] = page;
            ViewData["ListParameters"] = new { page, count };

            return View(ordrs);
        }

        [HttpPost]
        public void ChangeStatus(int orderId, String orderStatus)
        {
            string chaneDate = ConnectionConfigs.getPacsTime();
            Radiology rad = Radiology.SelectExact(orderId);
            if (rad.Status == ConnectionConfigs.SCHEDUALED)
                rad.Status = orderStatus;
            rad.StartDate = System.DateTime.Now.ToString("yyyyMMddHmmss");
            string editedUser = Session["userName"].ToString();
            try
            {
                string ex = Models.Radiology.UpdateOrder(rad);
            }
            catch
            {
            }

        }

        //end change status



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
            if (t != null)
                if (Regex.IsMatch(t, @"\p{IsArabic}"))
                    s = true;
            return s;
        }
        public JsonResult getDeptModalities(string dept)
        {
            List<Modality> mtList = Modality.GetModalitysListByDepId(dept);
            
            return Json(mtList);
        }


        // <MNM>
        public ActionResult IndexAudit([DefaultValue(1.0)] double page, [DefaultValue(1.0)] double count, string docId, string firstname, string lastname, int? departments, int? modalities, string oldOrderStatus, string mnmfrom, string mnmto)
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
            if (!RIS.Models.User.hasPerm(userId, Perms.RadiologyIndex))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }


            string s = Session["userType"].ToString();
            string d = (s == "1") ? departments.ToString() : Session["userDep"].ToString();

            double RowsPerPage = RequiredValues.getRowsPerPgById((int)ReqPatientVals.rowsPerPage).reqRowsPerPage;

            List<Radiology> ordrs = Radiology.getAuditOrders(page, out count, RowsPerPage, docId, firstname, lastname, d, modalities.ToString(),oldOrderStatus,mnmfrom,mnmto);

            ViewData["PageCnt"] = count;
            ViewData["count"] = count;
            ViewData["pageNBr"] = page;
            ViewData["page"] = page;
            ViewData["stayInSeach"] = "0";

            //ViewData["departments"] = Departement.GetDepartementListNames(true, "");
            //ViewData["modalities"] = Modality.GetModalitysList(true, "");
            ViewBag.departments = (s == "1") ? Departement.GetDepartementListNames(true, "") : Departement.GetDepartementListofTheuser(true, d, "");
            ViewBag.modalities = (s == "1") ? Modality.GetModalitysList(true, "") : Modality.GetModalitysListByDepId(true, d, "");
            ViewData["ListParameters"] = new { page, count };

            return View(ordrs);
        }

        [HttpPost]
        public ActionResult IndexAudit([DefaultValue(1.0)] double page, [DefaultValue(1.0)] double count, string docId, string firstname, string lastname, string departments, string modalities, string ss,string oldOrderStatus,string mnmfrom,string mnmto)
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
            if (!RIS.Models.User.hasPerm(userId, Perms.RadiologyIndex))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }

            string s = Session["userType"].ToString();
            string d = (s == "1") ? "" : Session["userDep"].ToString();

            double RowsPerPage = RequiredValues.getRowsPerPgById((int)ReqPatientVals.rowsPerPage).reqRowsPerPage;
            List<Radiology> ordrs = Radiology.getAuditOrders(page, out count, RowsPerPage, docId, firstname, lastname, departments, modalities, oldOrderStatus,  mnmfrom,  mnmto);

            ViewData["docId"] = (docId == null) ? "" : docId;
            ViewData["firstname"] = (firstname == null) ? "" : firstname;
            ViewData["lastname"] = (lastname == null) ? "" : lastname;
            ViewData["stayInSeach"] = "1";

            if (s == "1")
            {
                ViewData["departments"] = (departments == "") ? Departement.GetDepartementListNames(true, "") : Departement.GetDepartementListNames(true, Departement.select(int.Parse(departments)).name);
            }
            else
            {
                ViewData["departments"] = (departments == "") ? Departement.GetDepartementListofTheuser(true, departments, "") : Departement.GetDepartementListofTheuser(true, departments, Departement.select(int.Parse(departments)).name);
            }

            if (s == "1")
            {
                ViewData["modalities"] = (modalities == "") ? Modality.GetModalitysList(true, "") : Modality.GetModalitysList(true, Modality.Select(int.Parse(modalities)).name);
            }
            else
            {
                ViewData["modalities"] = (modalities == "") ? Modality.GetModalitysListByDepId(true, departments, "") : Modality.GetModalitysListByDepId(true, departments, Modality.Select(int.Parse(modalities)).name);
            }
            ViewBag.oldOrderStatus = oldOrderStatus;
            ViewBag.mnmfrom = mnmfrom;
            ViewBag.mnmto = mnmto;
            
            ViewData["PageCnt"] = count;
            ViewData["count"] = count;
            ViewData["pageNBr"] = page;
            ViewData["page"] = page;
            ViewData["ListParameters"] = new { page, count };

            return View(ordrs);
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
            if (!RIS.Models.User.hasPerm(userId, Perms.RadiologyDetails))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }

            Radiology rad = Radiology.SelectOld(id);
            return View(rad);
        }

        // </MNM>

    }
}