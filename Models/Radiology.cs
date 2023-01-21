using Oracle.DataAccess.Client;
using RISDB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using RIS.Validations;


namespace RIS.Models
{
    /// <summary>
    /// Class for radiology orders.
    /// </summary>
    public class Radiology
    {

        /// <summary>
        /// Order's ID which stands for the primary key in orders table in database.
        /// </summary>
        public int ID { set; get; }

        /// <summary>
        /// Order's ID which stands for the primary key in old orders table in database for auditing.
        /// </summary>
        public int NewID { set; get; }

        /// <summary>
        /// The ID of the patient which the order belongs to.
        /// </summary>
        public string PatientID { set; get; }

        /// <summary>
        /// The ID of the modality that executing the order.
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "RadMod")]
        [Required(ErrorMessageResourceType = typeof(Resources.Res), ErrorMessageResourceName = "RadModError")]
        public string ModalityID { set; get; }

        /// <summary>
        /// The order's procedure ID.
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "RadProce")]
        [Required(ErrorMessageResourceType = typeof(Resources.Res), ErrorMessageResourceName = "RadProceError")]
        public string ProcedureID { set; get; }

        [Required(ErrorMessageResourceType = typeof(Resources.Res), ErrorMessageResourceName = "RadCost")]
        //public int appCost { get; set; }
        public int radCost
        {
            get
            {
                if (this.radPayType == 0 && ModalityID != null)
                    return Modality.getModalityCost(int.Parse(ModalityID));
                else
                    return radCost = 0;
            }
            set { }
        }

        public string qcode
        {
            get
            {
                if ( ModalityID != null)
                    return Modality.Select(int.Parse(ModalityID)).qcode;
                else
                    return "NoQCODE";
            }
            set { }
        }
        /// <summary>
        /// The study ID.
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "studyID")]
        public string StudyID { set; get; }

        /// <summary>
        /// The date when the order executing starts.
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "STARTDATEParameter")]
        public string StartDate { set; get; }

        /// <summary>
        /// The date when the order executing finishes. 
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "OrderEndDate")]
        public string EndDate { set; get; }

        /// <summary>
        /// The order status i.e. completed, schedualed, being executed or waiting.
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "OrderStatus")]
        public string Status { set; get; }

        /// <summary>
        /// The doctor who requested the order.
        /// </summary>
        [ESValidation]
        [Display(ResourceType = typeof(Resources.Res), Name = "RadDoc")]
        public string Doctor { set; get; }

        /// <summary>
        /// The order's expiring date.
        /// </summary>
        public string AutoExpireDate { set; get; }

        /// <summary>
        /// Needed for HL7 message.
        /// </summary>
        public string ZDS { set; get; }
        
        /// <summary>
        /// The order's accession number.
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "AccNum")]
        public string AccessionNumber { set; get; }

        /// <summary>
        /// The department ID where order had been added.
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "depName")]
        [Required(ErrorMessageResourceType = typeof(Resources.Res), ErrorMessageResourceName = "depReq")]
        public string DepartementName { set; get; }

        /// <summary>
        /// An ID for the order added by the user.
        /// </summary>
        [ESValidation]
        [Display(ResourceType = typeof(Resources.Res), Name = "docId")]
        public string DocumnetId { set; get; }

        /// <summary>
        /// The order type.
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "orderType")]
        [Required(ErrorMessageResourceType = typeof(Resources.Res), ErrorMessageResourceName = "orderTypeReq")]
        public int Type { set; get; }
        public int MODALITYTYPE { set; get; }
        public ModalityType radModType
        {
            get
            {

                return ModalityType.Select(MODALITYTYPE);

            }
        }

        public string ProcedureType
        {
            get
            {
                return Procedure.select(parentProc.num).name;
            }
        }

        [Display(ResourceType = typeof(Resources.Res), Name = "radPayType")]
        [Required(ErrorMessageResourceType = typeof(Resources.Res), ErrorMessageResourceName = "patTypeError")]
        public int radPayType { set; get; }


        [Display(ResourceType = typeof(Resources.Res), Name = "radPayReason")]
        [Required(ErrorMessageResourceType = typeof(Resources.Res), ErrorMessageResourceName = "RadModError")]
        public string radPayReason { set; get; }

        /// <summary>
        /// The regestration status of the order, i.e. primary, edited, deleted.
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "mnmRegStatus")]
        public int? regStatus { set; get; }

        /// <summary>
        /// The creation date of the order in RIS.
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "InsertDateParameter")]
        public DateTime? insertDate { set; get; }

        /// <summary>
        /// The ID of the user who created the order.
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "InsertUser")]
        public string InsertUser { set; get; }

        /// <summary>
        /// The reason of the edition or deletion of the order.
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "UpdateDeleteReason")]
        public string UpdateDeleteReason { set; get; }

        /// <summary>
        /// The date of edition or deletion of the order.
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "UpdateDate")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy H:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime? updateDate { set; get; }

        /// <summary>
        /// The ID of the user who has updated or deleted an order.
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "UpdateUser")]
        public string UpdatetUser { set; get; }


        /// <summary>
        /// returns the user who created the order.
        /// </summary>
        /// <remarks> returns null if the user is unknown. </remarks>
        public User insUser
        {
            get
            {
                if (InsertUser != null)
                    return User.select((int.Parse(InsertUser)));
                else
                    return null;
            }
        }

        /// <summary>
        /// returns the user who updated or deleted the order as an integer.
        /// </summary>
        /// <remarks> returns null if the user is unknown. </remarks>
        public User updUser
        {
            get
            {
                if (UpdatetUser != null)
                    return User.select((int.Parse(UpdatetUser)));
                else
                    return null;
            }
        }


        /// <summary>
        /// Radiology order constructor.
        /// </summary>
        public Radiology() { }

        public class OrderType
        {
            public int TypeValue { get; set; }
            public string Name { get; set; }
        }

        public static SelectList OrdersTypes(string defaultValue)
        {

            List<SelectListItem> items = new List<SelectListItem>();


            items.Add(new SelectListItem { Text = RIS.Resources.Res.normalOrder, Value = "1" });
            items.Add(new SelectListItem { Text = RIS.Resources.Res.emergencyOrder, Value = "2" });
            items.Add(new SelectListItem { Text = RIS.Resources.Res.referralOrder, Value = "3" });
            items.Add(new SelectListItem { Text = RIS.Resources.Res.insuranceOrder, Value = "4" });

            return new SelectList(items, "Value", "Text", string.IsNullOrEmpty(defaultValue) ? "0" : defaultValue);

        }

        /// <summary>
        /// Represents the patient that this order belongs to.
        /// </summary>
        /// <see cref="Patient.Select(int)"/>
        public Patient parentR
        {
            get
            {
                return Patient.Select(int.Parse(PatientID));
            }
        }

        /// <summary>
        /// Represents the modality that executing the order.
        /// </summary>
        /// <see cref="Modality.Select(int)"/>
        public Modality parentMod
        {
            get
            {
                return Modality.Select(int.Parse(ModalityID));
            }
        }

        /// <summary>
        /// returns the department where the order was created.
        /// </summary>
        /// <see cref="Departement.select(int)"/>
        public Departement parentDep
        {
            get
            {
                return Departement.select(int.Parse(DepartementName));
            }
        }

        /// <summary>
        /// returns the procedure of the order.
        /// </summary>
        /// <see cref="Procedure.selectByCode(string)"/>
        public Procedure parentProc
        {
            get
            {
                return Procedure.selectByCode(ProcedureID);
            }
        }

        /// <summary>
        /// Gets an order from database.
        /// </summary>
        /// <remarks> startdate and enddate of the order is transformed from string to date in the query. </remarks>
        /// <param name="num"> The ID of the order that represent the primary key in orders table. </param>
        /// <returns> instance of the type Radiology containting the targeted order details. </returns>
        public static Radiology Select(int num)
        {
            Radiology pt = new Radiology();
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("SELECT NUM, PATIENTID, MODALITYID, PROCEDUREID, STUDYID,"+
                    " to_date(STARTDATE, 'yyyymmddhh24:mi:ss'), to_date(ENDDATE, 'yyyymmddhh24:mi:ss') , STATUS, DOCTOR," +
                    " AUTOEXPIREDATE, ZDS, ACCESSIONNUMBER, DEPTNAME, DOCID, TYPE," +
                    " REGSTATUS, INSERTDATE, INSERTUSER, UPDATEDELETEREASON, PAYTYPE, PAYREASON FROM ORDERS WHERE num='" + num + "' ", conn);
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    #region Data
                    if (!dr.IsDBNull(0))
                        pt.ID = int.Parse(dr.GetValue(0).ToString());
                    if (!dr.IsDBNull(1))
                        pt.PatientID = dr.GetValue(1).ToString();
                    if (!dr.IsDBNull(2))
                        pt.ModalityID = dr.GetValue(2).ToString();
                    if (!dr.IsDBNull(3))
                        pt.ProcedureID = dr.GetValue(3).ToString();
                    if (!dr.IsDBNull(4))
                        pt.StudyID = dr.GetString(4);
                    if (!dr.IsDBNull(5))
                        pt.StartDate = dr.GetValue(5).ToString();
                    if (!dr.IsDBNull(6))
                        pt.EndDate = dr.GetValue(6).ToString();
                    if (!dr.IsDBNull(7))
                        pt.Status = dr.GetString(7);
                    if (!dr.IsDBNull(8))
                        pt.Doctor = dr.GetString(8);
                    if (!dr.IsDBNull(9))
                        pt.AutoExpireDate = dr.GetString(9);
                    if (!dr.IsDBNull(10))
                        pt.ZDS = dr.GetString(10);
                    if (!dr.IsDBNull(11))
                        pt.AccessionNumber = dr.GetString(11);
                    if (!dr.IsDBNull(12))
                        pt.DepartementName = dr.GetValue(12).ToString();
                    if (!dr.IsDBNull(13))
                        pt.DocumnetId = dr.GetString(13);
                    if (!dr.IsDBNull(14))
                        pt.Type = int.Parse(dr.GetValue(14).ToString());
                    if (!dr.IsDBNull(15))
                        pt.regStatus = int.Parse(dr.GetValue(15).ToString());
                    if (!dr.IsDBNull(16))
                        pt.insertDate = dr.GetDateTime(16);
                    if (!dr.IsDBNull(17))
                        pt.InsertUser = dr.GetValue(17).ToString();
                    if (!dr.IsDBNull(18))
                        pt.UpdateDeleteReason = dr.GetString(18);
                    
                    if (!dr.IsDBNull(19))
                        pt.radPayType = int.Parse(dr.GetValue(19).ToString());

                    if (!dr.IsDBNull(20))
                        pt.radPayReason = dr.GetString(20);

                    #endregion
                }
            }
            catch(OracleException e)
            {
                String s=e.Message;
            }
            finally
            {
                conn.Close();
            }
            return pt;
        }

        /// <summary>
        /// Gets all the orders related to a defined patient.
        /// </summary>
        /// <param name="id"> the ID of the patient whose orders wanted. </param>
        /// <returns> List of type Radiology containg all the orders of the targeted patient. </returns>
        public static List<Radiology> getPatientsOrders(int id)
        {
            List<Radiology> orders = new List<Radiology>();
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("SELECT NUM, PATIENTID, MODALITYID, PROCEDUREID, STUDYID,"+
                    " to_date(STARTDATE, 'yyyymmddhh24:mi:ss'), to_date(ENDDATE, 'yyyymmddhh24:mi:ss'),"+
                    " STATUS, DOCTOR, AUTOEXPIREDATE, ZDS, ACCESSIONNUMBER, DEPTNAME, DOCID, TYPE,"+
                    " REGSTATUS, INSERTDATE, INSERTUSER, UPDATEDELETEREASON , PAYTYPE FROM ORDERS WHERE PATIENTID=" + id +
                    "ORDER BY to_date(STARTDATE, 'yyyymmddhh24:mi:ss') DESC", conn);
                OracleDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Radiology pt = new Radiology();
                    #region Data
                    if (!dr.IsDBNull(0))
                        pt.ID = int.Parse(dr.GetValue(0).ToString());
                    if (!dr.IsDBNull(1))
                        pt.PatientID = dr.GetValue(1).ToString();
                    if (!dr.IsDBNull(2))
                        pt.ModalityID = dr.GetValue(2).ToString();
                    if (!dr.IsDBNull(3))
                        pt.ProcedureID = dr.GetValue(3).ToString();
                    if (!dr.IsDBNull(4))
                        pt.StudyID = dr.GetString(4);
                    if (!dr.IsDBNull(5))
                        pt.StartDate = dr.GetValue(5).ToString();
                    if (!dr.IsDBNull(6))
                        pt.EndDate = dr.GetValue(6).ToString();
                    if (!dr.IsDBNull(7))
                        pt.Status = dr.GetString(7);
                    if (!dr.IsDBNull(8))
                        pt.Doctor = dr.GetString(8);
                    if (!dr.IsDBNull(9))
                        pt.AutoExpireDate = dr.GetString(9);
                    if (!dr.IsDBNull(10))
                        pt.ZDS = dr.GetString(10);
                    if (!dr.IsDBNull(11))
                        pt.AccessionNumber = dr.GetString(11);
                    if (!dr.IsDBNull(12))
                        pt.DepartementName = dr.GetValue(12).ToString();
                    if (!dr.IsDBNull(13))
                        pt.DocumnetId = dr.GetString(13);
                    if (!dr.IsDBNull(14))
                        pt.Type = int.Parse(dr.GetValue(14).ToString());
                    if (!dr.IsDBNull(15))
                        pt.regStatus = dr.GetInt16(15);
                    if (!dr.IsDBNull(16))
                        pt.insertDate = dr.GetDateTime(16);
                    if (!dr.IsDBNull(17))
                        pt.InsertUser = dr.GetValue(17).ToString();
                    if (!dr.IsDBNull(18))
                        pt.UpdateDeleteReason = dr.GetString(18);
                    if (!dr.IsDBNull(19))
                        pt.radPayType = int.Parse(dr.GetValue(19).ToString());
                    #endregion

                    orders.Add(pt);
                }
            }
            catch(Exception e)
            {
                string s = e.Message;
            }
            return orders;
        }
        // Modar for Accounting
        public static List<Radiology> getCompletedPatientsOrdersByDate(int id, string orderDate)
        {
            List<Radiology> orders = new List<Radiology>();
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("SELECT NUM, PATIENTID, MODALITYID, PROCEDUREID, STUDYID," +
                    " to_date(STARTDATE, 'yyyymmddhh24:mi:ss'), to_date(ENDDATE, 'yyyymmddhh24:mi:ss')," +
                    " STATUS, DOCTOR, AUTOEXPIREDATE, ZDS, ACCESSIONNUMBER, DEPTNAME, DOCID, TYPE," +
                    " REGSTATUS, INSERTDATE, INSERTUSER, UPDATEDELETEREASON , PAYTYPE, PAYREASON,updateDate" +
                    " FROM ORDERS WHERE PATIENTID=" + id + " AND to_date(ENDDATE, 'yyyymmddhh24:mi:ss')='" + orderDate + "' AND STATUS in (2,3,4)" +
                    " ORDER BY to_date(STARTDATE, 'yyyymmddhh24:mi:ss') DESC", conn);
                //string sql = "SELECT NUM, PATIENTID, MODALITYID, PROCEDUREID, STUDYID," +
                //    " to_date(STARTDATE, 'yyyymmddhh24:mi:ss'), to_date(ENDDATE, 'yyyymmddhh24:mi:ss')," +
                //    " STATUS, DOCTOR, AUTOEXPIREDATE, ZDS, ACCESSIONNUMBER, DEPTNAME, DOCID, TYPE," +
                //    " REGSTATUS, INSERTDATE, INSERTUSER, UPDATEDELETEREASON , PAYTYPE, PAYREASON, MODALITYTYPE,DOCTORID,updateDate, Summary," +
                //    " Recommendation FROM ORDERS WHERE PATIENTID=" + id + " AND to_date(ENDDATE, 'yyyymmddhh24:mi:ss')='" + orderDate + "' AND STATUS in (2,3,4)" +
                //    " ORDER BY to_date(STARTDATE, 'yyyymmddhh24:mi:ss') DESC";
                OracleDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Radiology pt = new Radiology();
                    #region Data
                    if (!dr.IsDBNull(0))
                        pt.ID = int.Parse(dr.GetValue(0).ToString());
                    if (!dr.IsDBNull(1))
                        pt.PatientID = dr.GetValue(1).ToString();
                    if (!dr.IsDBNull(2))
                        pt.ModalityID = dr.GetValue(2).ToString();
                    if (!dr.IsDBNull(3))
                        pt.ProcedureID = dr.GetValue(3).ToString();
                    if (!dr.IsDBNull(4))
                        pt.StudyID = dr.GetString(4);
                    if (!dr.IsDBNull(5))
                        pt.StartDate = dr.GetValue(5).ToString();
                    if (!dr.IsDBNull(6))
                        pt.EndDate = dr.GetValue(6).ToString();
                    if (!dr.IsDBNull(7))
                        pt.Status = dr.GetString(7);
                    if (!dr.IsDBNull(8))
                        pt.Doctor = dr.GetString(8);
                    if (!dr.IsDBNull(9))
                        pt.AutoExpireDate = dr.GetString(9);
                    if (!dr.IsDBNull(10))
                        pt.ZDS = dr.GetString(10);
                    if (!dr.IsDBNull(11))
                        pt.AccessionNumber = dr.GetString(11);
                    if (!dr.IsDBNull(12))
                        pt.DepartementName = dr.GetValue(12).ToString();
                    if (!dr.IsDBNull(13))
                        pt.DocumnetId = dr.GetString(13);
                    if (!dr.IsDBNull(14))
                        pt.Type = int.Parse(dr.GetValue(14).ToString());
                    if (!dr.IsDBNull(15))
                        pt.regStatus = dr.GetInt16(15);
                    if (!dr.IsDBNull(16))
                        pt.insertDate = dr.GetDateTime(16);
                    if (!dr.IsDBNull(17))
                        pt.InsertUser = dr.GetValue(17).ToString();
                    if (!dr.IsDBNull(18))
                        pt.UpdateDeleteReason = dr.GetString(18);
                    if (!dr.IsDBNull(19))
                        pt.radPayType = int.Parse(dr.GetValue(19).ToString());
                    if (!dr.IsDBNull(20))
                        pt.radPayReason = dr.GetString(20);
                    if (!dr.IsDBNull(21))
                        pt.updateDate = DateTime.Parse(dr.GetValue(23).ToString());
                    #endregion

                    orders.Add(pt);
                }
            }
            catch (OracleException e)
            {
                string res = e.Message;
            }
            catch (Exception e)
            {
                string s = e.Message;
            }
            return orders;
        }

        public static List<Radiology> getCompletedPatientsOrdersBetweenDates(int id, string fromDate, string toDate)
        {
            List<Radiology> orders = new List<Radiology>();
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                string sql = "SELECT NUM, PATIENTID, MODALITYID, PROCEDUREID, STUDYID," +
                    " to_date(STARTDATE, 'yyyymmddhh24:mi:ss'), to_date(ENDDATE, 'yyyymmddhh24:mi:ss')," +
                    " STATUS, DOCTOR, AUTOEXPIREDATE, ZDS, ACCESSIONNUMBER, DEPTNAME, DOCID, TYPE," +
                    " REGSTATUS, INSERTDATE, INSERTUSER, UPDATEDELETEREASON , PAYTYPE, PAYREASON, MODALITYTYPE,DOCTORID,updateDate, Summary," +
                    " Recommendation FROM ORDERS WHERE PATIENTID=" + id + " AND to_date(ENDDATE, 'yyyymmddhh24:mi:ss') BETWEEN to_date('" + fromDate +
                    "', 'yyyymmddhh24:mi:ss') AND to_date('" + toDate + "', 'yyyymmddhh24:mi:ss')+1 AND STATUS in (2,3,4)" +
                    " ORDER BY to_date(STARTDATE, 'yyyymmddhh24:mi:ss') DESC";

                OracleCommand cmd = new OracleCommand("SELECT NUM, PATIENTID, MODALITYID, PROCEDUREID, STUDYID," +
                    " to_date(STARTDATE, 'yyyymmddhh24:mi:ss'), to_date(ENDDATE, 'yyyymmddhh24:mi:ss')," +
                    " STATUS, DOCTOR, AUTOEXPIREDATE, ZDS, ACCESSIONNUMBER, DEPTNAME, DOCID, TYPE," +
                    " REGSTATUS, INSERTDATE, INSERTUSER, UPDATEDELETEREASON , PAYTYPE, PAYREASON, MODALITYTYPE,DOCTORID,updateDate, Summary," +
                    " Recommendation FROM ORDERS WHERE PATIENTID=" + id + " AND to_date(ENDDATE, 'yyyymmddhh24:mi:ss') BETWEEN to_date('" + fromDate +
                    "', 'yyyymmddhh24:mi:ss') AND to_date('" + toDate + "', 'yyyymmddhh24:mi:ss')+1 AND STATUS in (2,3,4)" +
                    " ORDER BY to_date(STARTDATE, 'yyyymmddhh24:mi:ss') DESC", conn);
                //string sql = "SELECT NUM, PATIENTID, MODALITYID, PROCEDUREID, STUDYID," +
                //    " to_date(STARTDATE, 'yyyymmddhh24:mi:ss'), to_date(ENDDATE, 'yyyymmddhh24:mi:ss')," +
                //    " STATUS, DOCTOR, AUTOEXPIREDATE, ZDS, ACCESSIONNUMBER, DEPTNAME, DOCID, TYPE," +
                //    " REGSTATUS, INSERTDATE, INSERTUSER, UPDATEDELETEREASON , PAYTYPE, PAYREASON, MODALITYTYPE,DOCTORID,updateDate, Summary," +
                //    " Recommendation FROM ORDERS WHERE PATIENTID=" + id + " AND to_date(ENDDATE, 'yyyymmddhh24:mi:ss')='" + orderDate + "' AND STATUS in (2,3,4)" +
                //    " ORDER BY to_date(STARTDATE, 'yyyymmddhh24:mi:ss') DESC";
                OracleDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Radiology pt = new Radiology();
                    #region Data
                    if (!dr.IsDBNull(0))
                        pt.ID = int.Parse(dr.GetValue(0).ToString());
                    if (!dr.IsDBNull(1))
                        pt.PatientID = dr.GetValue(1).ToString();
                    if (!dr.IsDBNull(2))
                        pt.ModalityID = dr.GetValue(2).ToString();
                    if (!dr.IsDBNull(3))
                        pt.ProcedureID = dr.GetValue(3).ToString();
                    if (!dr.IsDBNull(4))
                        pt.StudyID = dr.GetString(4);
                    if (!dr.IsDBNull(5))
                        pt.StartDate = dr.GetValue(5).ToString();
                    if (!dr.IsDBNull(6))
                        pt.EndDate = dr.GetValue(6).ToString();
                    if (!dr.IsDBNull(7))
                        pt.Status = dr.GetString(7);
                    if (!dr.IsDBNull(8))
                        pt.Doctor = dr.GetString(8);
                    if (!dr.IsDBNull(9))
                        pt.AutoExpireDate = dr.GetString(9);
                    if (!dr.IsDBNull(10))
                        pt.ZDS = dr.GetString(10);
                    if (!dr.IsDBNull(11))
                        pt.AccessionNumber = dr.GetString(11);
                    if (!dr.IsDBNull(12))
                        pt.DepartementName = dr.GetValue(12).ToString();
                    if (!dr.IsDBNull(13))
                        pt.DocumnetId = dr.GetString(13);
                    if (!dr.IsDBNull(14))
                        pt.Type = int.Parse(dr.GetValue(14).ToString());
                    if (!dr.IsDBNull(15))
                        pt.regStatus = dr.GetInt16(15);
                    if (!dr.IsDBNull(16))
                        pt.insertDate = dr.GetDateTime(16);
                    if (!dr.IsDBNull(17))
                        pt.InsertUser = dr.GetValue(17).ToString();
                    if (!dr.IsDBNull(18))
                        pt.UpdateDeleteReason = dr.GetString(18);
                    if (!dr.IsDBNull(19))
                        pt.radPayType = int.Parse(dr.GetValue(19).ToString());
                    if (!dr.IsDBNull(20))
                        pt.radPayReason = dr.GetString(20);
                    if (!dr.IsDBNull(21))
                        pt.updateDate = DateTime.Parse(dr.GetValue(23).ToString());
                    #endregion

                    orders.Add(pt);
                }
            }
            catch (OracleException e)
            {
                string res = e.Message;
            }
            catch (Exception e)
            {
                string s = e.Message;
            }
            return orders;
        }

        /// <summary>
        /// Gets an order from database.
        /// </summary>
        /// <remarks> startdate and enddate of the order returned as string as in the database. </remarks>
        /// <param name="num"> The ID of the order that represent the primary key in orders table. </param>
        /// <returns> instance of the type Radiology containting the targeted order details. </returns>
        public static Radiology SelectExact(int num)
        {
            Radiology pt = new Radiology();
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("SELECT NUM, PATIENTID, MODALITYID, PROCEDUREID, STUDYID,"+
                    " STARTDATE, ENDDATE, STATUS, DOCTOR, AUTOEXPIREDATE, ZDS, ACCESSIONNUMBER, DEPTNAME, DOCID,"+
                    " TYPE, REGSTATUS, INSERTDATE, INSERTUSER, UPDATEDELETEREASON, PAYTYPE, PAYREASON  FROM ORDERS WHERE num=" + num, conn);
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    if (dr.Read())
                    {
                        #region Data
                        if (!dr.IsDBNull(0))
                          //  pt.ID = dr.GetInt32(0);
                         pt.ID = int.Parse(dr.GetValue(0).ToString());
                        if (!dr.IsDBNull(1))
                            pt.PatientID = dr.GetValue(1).ToString();
                        if (!dr.IsDBNull(2))
                            pt.ModalityID = dr.GetValue(2).ToString();
                        if (!dr.IsDBNull(3))
                            pt.ProcedureID = dr.GetValue(3).ToString();
                        if (!dr.IsDBNull(4))
                            pt.StudyID = dr.GetString(4);
                        if (!dr.IsDBNull(5))
                            pt.StartDate = dr.GetString(5);
                        if (!dr.IsDBNull(6))
                            pt.EndDate = dr.GetString(6);
                        if (!dr.IsDBNull(7))
                            pt.Status = dr.GetString(7);
                        if (!dr.IsDBNull(8))
                            pt.Doctor = dr.GetString(8);
                        if (!dr.IsDBNull(9))
                            pt.AutoExpireDate = dr.GetString(9);
                        if (!dr.IsDBNull(10))
                            pt.ZDS = dr.GetString(10);
                        if (!dr.IsDBNull(11))
                            pt.AccessionNumber = dr.GetString(11);
                        if (!dr.IsDBNull(12))
                            pt.DepartementName = dr.GetValue(12).ToString();
                        if (!dr.IsDBNull(13))
                            pt.DocumnetId = dr.GetString(13);
                        if (!dr.IsDBNull(14))
                            pt.Type =int.Parse(dr.GetValue(14).ToString());
                        if (!dr.IsDBNull(15))
                            pt.regStatus = dr.GetInt16(15);
                        if (!dr.IsDBNull(16))
                            pt.insertDate = dr.GetDateTime(16);
                        if (!dr.IsDBNull(17))
                            pt.InsertUser = dr.GetValue(17).ToString();
                        if (!dr.IsDBNull(18))
                            pt.UpdateDeleteReason = dr.GetString(18);
                        if (!dr.IsDBNull(19))
                            pt.radPayType = int.Parse(dr.GetValue(19).ToString());

                        if (!dr.IsDBNull(20))
                            pt.radPayReason = dr.GetString(20);
                        #endregion
                    }
                }
                else
                {
                    Console.WriteLine("No rows found.");
                }
            }
            catch (Exception e)
            {
                String s = e.Message;
            }
            finally
            {
                conn.Close();
            }
            return pt;
        }

        /// <summary>
        /// Gets orders with startdate equal to today in database.
        /// </summary>
        /// <returns>a list of today's orders.</returns>
        public static List<Radiology> getTodayOrders()
        {
            List<Radiology> res = new List<Radiology>();

            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());

            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("SELECT * FROM ORDERS WHERE STARTDATE= " + DateTime.Now.Date, conn);
                OracleDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Radiology u = new Radiology();
                    if (!dr.IsDBNull(0))
                        u.ID = int.Parse(dr.GetValue(0).ToString());
                    if (!dr.IsDBNull(1))
                        u.PatientID = dr.GetValue(1).ToString();
                    if (!dr.IsDBNull(2))
                        u.ModalityID = dr.GetValue(2).ToString();
                    if (!dr.IsDBNull(3))
                        u.ProcedureID = dr.GetValue(3).ToString();
                    if (!dr.IsDBNull(4))
                        u.StudyID = dr.GetValue(4).ToString();
                    if (!dr.IsDBNull(5))
                        u.StartDate = dr.GetValue(5).ToString();
                    if (!dr.IsDBNull(6))
                        u.EndDate = dr.GetValue(6).ToString();
                    if (!dr.IsDBNull(7))
                        u.Status = dr.GetValue(7).ToString();
                    if (!dr.IsDBNull(8))
                        u.Doctor = dr.GetValue(8).ToString();
                    if (!dr.IsDBNull(9))
                        u.AutoExpireDate = dr.GetValue(9).ToString();
                    if (!dr.IsDBNull(10))
                        u.AccessionNumber = dr.GetValue(10).ToString();
                    res.Add(u);
                }

                return res;
            }
            catch
            {
                return null;
            }

            finally
            {
                conn.Close();
            }
        }

        //public static List<Radiology> getOrders(double page, out double count, double RowsPerPage, string docid, string firstname, string lastname, string depid, string modid)
        //{
        //    List<Radiology> res = new List<Radiology>();

        //    OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());

        //    try
        //    {
        //        conn.Open();

        //        string idx1 = (RowsPerPage * (page - 1) + 1).ToString();
        //        string idx2 = (RowsPerPage * page).ToString();
        //        string whereStr = "SELECT ORDERS.NUM, ORDERS.PATIENTID, ORDERS.MODALITYID, ORDERS.PROCEDUREID,"+
        //            " ORDERS.STUDYID, to_date(ORDERS.STARTDATE, 'yyyymmddhh24:mi:ss'), ORDERS.ENDDATE, ORDERS.STATUS,"+
        //            " ORDERS.DOCTOR, ORDERS.AUTOEXPIREDATE, ORDERS.ZDS, ORDERS.ACCESSIONNUMBER, ORDERS.DEPTNAME,"+
        //            " ORDERS.DOCID, ORDERS.TYPE, ORDERS.REGSTATUS, ORDERS.INSERTDATE, ORDERS.INSERTUSER,"+
        //            " ORDERS.UPDATEDELETEREASON FROM ORDERS JOIN PATIENT ON PATIENT.NUM=ORDERS.PATIENTID WHERE 1=1 " +
        //            " AND ORDERS.STATUS != " + ConnectionConfigs.Waiting + " or ORDERS.STATUS IS NULL " ;

        //        if (firstname != null && firstname != "")
        //            whereStr += " AND PATIENT.FIRSTNAME like '%" + firstname + "%' ";
        //        if (lastname != null && lastname != "")
        //            whereStr += " And PATIENT.LASTNAME like '%" + lastname + "%' ";
        //        if (docid != null && docid != "")
        //            whereStr += " And ORDERS.DOCID = '" + docid + "' ";
        //        if (depid != null && depid != "")
        //            whereStr += " And ORDERS.DEPTNAME = " + depid + " ";
        //        if (modid != null && modid != "")
        //            whereStr += " And ORDERS.MODALITYID = " + modid + " ";

        //        whereStr += " order by to_date(STARTDATE, 'yyyymmddhh24:mi:ss') desc ";

        //        string sql = "SELECT * FROM (SELECT T.*, ROWNUM RID FROM (" + whereStr + ") T)WHERE RID BETWEEN " + idx1 + " AND " + idx2;

        //        string countQuery = "SELECT COUNT(*) FROM (" + whereStr + ")";
        //        OracleCommand cmdCnt = new OracleCommand(countQuery, conn);
        //        count = Math.Ceiling(double.Parse(cmdCnt.ExecuteScalar().ToString()) / RowsPerPage);


        //        OracleCommand cmd = new OracleCommand(sql, conn);
        //        OracleDataReader dr = cmd.ExecuteReader();

        //        while (dr.Read())
        //        {
        //            Radiology u = new Radiology();
        //            if (!dr.IsDBNull(0))
        //                u.ID = int.Parse(dr.GetValue(0).ToString());
        //            if (!dr.IsDBNull(1))
        //                u.PatientID = dr.GetValue(1).ToString();
        //            if (!dr.IsDBNull(2))
        //                u.ModalityID = dr.GetValue(2).ToString();
        //            if (!dr.IsDBNull(3))
        //                u.ProcedureID = dr.GetValue(3).ToString();
        //            if (!dr.IsDBNull(4))
        //                u.StudyID = dr.GetValue(4).ToString();
        //            if (!dr.IsDBNull(5))
        //                u.StartDate = dr.GetValue(5).ToString();
        //            if (!dr.IsDBNull(6))
        //                u.EndDate = dr.GetValue(6).ToString();
        //            if (!dr.IsDBNull(7))
        //                u.Status = dr.GetValue(7).ToString();
        //            if (!dr.IsDBNull(8))
        //                u.Doctor = dr.GetValue(8).ToString();
        //            if (!dr.IsDBNull(9))
        //                u.AutoExpireDate = dr.GetValue(9).ToString();
        //            if (!dr.IsDBNull(10))
        //                u.ZDS = dr.GetValue(10).ToString();
        //            if (!dr.IsDBNull(11))
        //                u.AccessionNumber = dr.GetValue(11).ToString();
        //            if (!dr.IsDBNull(12))
        //                u.DepartementName = dr.GetValue(12).ToString();
        //            if (!dr.IsDBNull(13))
        //                u.DocumnetId = dr.GetValue(13).ToString();
        //            if (!dr.IsDBNull(14))
        //                u.Type = int.Parse(dr.GetValue(14).ToString());
        //            if (!dr.IsDBNull(15))
        //                u.regStatus = int.Parse(dr.GetValue(15).ToString());
        //            if (!dr.IsDBNull(16))
        //                u.insertDate = dr.GetDateTime(16);
        //            if (!dr.IsDBNull(17))
        //                u.InsertUser = dr.GetValue(17).ToString();
        //            if (!dr.IsDBNull(18))
        //                u.UpdateDeleteReason = dr.GetString(18);
        //            res.Add(u);
        //        }

        //        return res;
        //    }
        //    //catch
        //    //{
        //    //    return null;
        //    //}

        //    finally
        //    {
        //        conn.Close();
        //    }
        //}


        /// <summary>
        /// Gets completed, under executing and schedualed orders from database based on the search options inserted by the user.
        /// </summary>
        /// <remarks> if no search options was inserted, this function gets all orders. </remarks>
        /// <param name="page">the page number for paging in the view.</param>
        /// <param name="count">total number of pages.</param>
        /// <param name="RowsPerPage">number of orders contained in single page.</param>
        /// <param name="docid">order's document ID.</param>
        /// <param name="firstname">the first name of the patient.</param>
        /// <param name="lastname">the last name of the patient.</param>
        /// <param name="depid">order's department ID</param>
        /// <param name="modid">order's modality ID.</param>
        /// <returns> a list of type Radiology orders. </returns>
        public static List<Radiology> getOrders(double page, out double count, double RowsPerPage, string docid, string firstname, string lastname, string depid, string modid)
        {
            List<Radiology> res = new List<Radiology>();

            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());

            try
            {
                conn.Open();

                string idx1 = (RowsPerPage * (page - 1) + 1).ToString();
                string idx2 = (RowsPerPage * page).ToString();
                string whereStr = "SELECT ORDERS.NUM, ORDERS.PATIENTID, ORDERS.MODALITYID, ORDERS.PROCEDUREID," +
                    " ORDERS.STUDYID, to_date(ORDERS.STARTDATE, 'yyyymmddhh24miss'), ORDERS.ENDDATE, ORDERS.STATUS," +
                    " ORDERS.DOCTOR, ORDERS.AUTOEXPIREDATE, ORDERS.ZDS, ORDERS.ACCESSIONNUMBER, ORDERS.DEPTNAME," +
                    " ORDERS.DOCID, ORDERS.TYPE, ORDERS.REGSTATUS, ORDERS.INSERTDATE, ORDERS.INSERTUSER," +
                    " ORDERS.UPDATEDELETEREASON, ORDERS.PAYTYPE FROM ORDERS WHERE 1=1 " +
                    " AND ORDERS.STATUS != " + ConnectionConfigs.Waiting + " ";

                string patQr = "select num from PATIENT where 1=1 ";
                string patQr_temp = "select num from PATIENT where 1=1 ";

                if (firstname != null && firstname != "")
                    patQr += " AND PATIENT.FIRSTNAME like '%" + firstname + "%' ";
                if (lastname != null && lastname != "")
                    patQr += " And PATIENT.LASTNAME like '%" + lastname + "%' ";
                if (patQr != patQr_temp)
                    whereStr += " AND ORDERS.PATIENTID in ("+patQr+") ";
                if (docid != null && docid != "")
                    whereStr += " And ORDERS.DOCID = '" + docid + "' ";
                if (depid != null && depid != "")
                    whereStr += " And ORDERS.DEPTNAME = " + depid + " ";
                if (modid != null && modid != "")
                    whereStr += " And ORDERS.MODALITYID = " + modid + " ";

                whereStr += " order by to_date(STARTDATE, 'yyyymmddhh24miss') desc ";

                string sql = "SELECT * FROM (SELECT T.*, ROWNUM RID FROM (" + whereStr + ") T)WHERE RID BETWEEN " + idx1 + " AND " + idx2;

                string countQuery = "SELECT COUNT(*) FROM (" + whereStr + ")";
                OracleCommand cmdCnt = new OracleCommand(countQuery, conn);
                count = Math.Ceiling(double.Parse(cmdCnt.ExecuteScalar().ToString()) / RowsPerPage);


                OracleCommand cmd = new OracleCommand(sql, conn);
                OracleDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Radiology u = new Radiology();
                    if (!dr.IsDBNull(0))
                        u.ID = int.Parse(dr.GetValue(0).ToString());
                    if (!dr.IsDBNull(1))
                        u.PatientID = dr.GetValue(1).ToString();
                    if (!dr.IsDBNull(2))
                        u.ModalityID = dr.GetValue(2).ToString();
                    if (!dr.IsDBNull(3))
                        u.ProcedureID = dr.GetValue(3).ToString();
                    if (!dr.IsDBNull(4))
                        u.StudyID = dr.GetValue(4).ToString();
                    if (!dr.IsDBNull(5))
                        u.StartDate = dr.GetValue(5).ToString();
                    if (!dr.IsDBNull(6))
                        u.EndDate = dr.GetValue(6).ToString();
                    if (!dr.IsDBNull(7))
                        u.Status = dr.GetValue(7).ToString();
                    if (!dr.IsDBNull(8))
                        u.Doctor = dr.GetValue(8).ToString();
                    if (!dr.IsDBNull(9))
                        u.AutoExpireDate = dr.GetValue(9).ToString();
                    if (!dr.IsDBNull(10))
                        u.ZDS = dr.GetValue(10).ToString();
                    if (!dr.IsDBNull(11))
                        u.AccessionNumber = dr.GetValue(11).ToString();
                    if (!dr.IsDBNull(12))
                        u.DepartementName = dr.GetValue(12).ToString();
                    if (!dr.IsDBNull(13))
                        u.DocumnetId = dr.GetValue(13).ToString();
                    if (!dr.IsDBNull(14))
                        u.Type = int.Parse(dr.GetValue(14).ToString());
                    if (!dr.IsDBNull(15))
                        u.regStatus = int.Parse(dr.GetValue(15).ToString());
                    if (!dr.IsDBNull(16))
                        u.insertDate = dr.GetDateTime(16);
                    if (!dr.IsDBNull(17))
                        u.InsertUser = dr.GetValue(17).ToString();
                    if (!dr.IsDBNull(18))
                        u.UpdateDeleteReason = dr.GetString(18);
                    if (!dr.IsDBNull(19))
                        u.radPayType =int.Parse( dr.GetValue(19).ToString());
                    res.Add(u);
                }

                return res;
            }
            //catch
            //{
            //    return null;
            //}

            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// Gets orders which have the status "Waiting" from database based on the search options inserted by the user.
        /// </summary>
        /// <remarks> if no search options was inserted, this function gets all orders have the "Waiting" status. </remarks>
        /// <param name="page">the page number for paging in the view.</param>
        /// <param name="count">total number of pages.</param>
        /// <param name="RowsPerPage">number of orders contained in single page.</param>
        /// <param name="docid">order's document ID.</param>
        /// <param name="firstname">the first name of the patient.</param>
        /// <param name="lastname">the last name of the patient.</param>
        /// <param name="depid">order's department ID</param>
        /// <param name="modid">order's modality ID.</param>
        /// <returns> a list of type Radiology orders. </returns>
        public static List<Radiology> getWaitingOrders(double page, out double count, double RowsPerPage, string docid, string firstname, string lastname, string depid, string modid)
        {
            List<Radiology> res = new List<Radiology>();

            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());

            try
            {
                conn.Open();

                string idx1 = (RowsPerPage * (page - 1) + 1).ToString();
                string idx2 = (RowsPerPage * page).ToString();
                string whereStr = "SELECT ORDERS.NUM, ORDERS.PATIENTID, ORDERS.MODALITYID, ORDERS.PROCEDUREID,"+
                    " ORDERS.STUDYID, to_date(ORDERS.STARTDATE, 'yyyymmddhh24:mi:ss'), ORDERS.ENDDATE, ORDERS.STATUS,"+
                    " ORDERS.DOCTOR, ORDERS.AUTOEXPIREDATE, ORDERS.ZDS, ORDERS.ACCESSIONNUMBER, ORDERS.DEPTNAME,"+
                    " ORDERS.DOCID, ORDERS.TYPE, ORDERS.REGSTATUS, ORDERS.INSERTDATE, ORDERS.INSERTUSER,"+
                    " ORDERS.UPDATEDELETEREASON, ORDERS.PAYTYPE FROM ORDERS WHERE 1=1  ";

                string patQr = "select num from PATIENT where 1=1 ";
                string patQr_temp = "select num from PATIENT where 1=1 ";

                if (firstname != null && firstname != "")
                    patQr += " AND PATIENT.FIRSTNAME like '%" + firstname + "%' ";
                if (lastname != null && lastname != "")
                    patQr += " And PATIENT.LASTNAME like '%" + lastname + "%' ";
                if (patQr != patQr_temp)
                    whereStr += " AND ORDERS.PATIENTID in (" + patQr + ") ";
                if (docid != null && docid != "")
                    whereStr += " And ORDERS.DOCID = '" + docid + "' ";
                if (depid != null && depid != "")
                    whereStr += " And ORDERS.DEPTNAME = " + depid + " ";
                if (modid != null && modid != "")
                    whereStr += " And ORDERS.MODALITYID = " + modid + " ";

                whereStr += " And ORDERS.STARTDATE IS NULL order by ORDERS.NUM desc ";

                string sql = "SELECT * FROM (SELECT T.*, ROWNUM RID FROM (" + whereStr + ") T)WHERE RID BETWEEN " + idx1 + " AND " + idx2;

                string countQuery = "SELECT COUNT(*) FROM (" + whereStr + ")";
                OracleCommand cmdCnt = new OracleCommand(countQuery, conn);
                count = Math.Ceiling(double.Parse(cmdCnt.ExecuteScalar().ToString()) / RowsPerPage);


                OracleCommand cmd = new OracleCommand(sql, conn);
                OracleDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Radiology u = new Radiology();
                    if (!dr.IsDBNull(0))
                        u.ID = int.Parse(dr.GetValue(0).ToString());
                    if (!dr.IsDBNull(1))
                        u.PatientID = dr.GetValue(1).ToString();
                    if (!dr.IsDBNull(2))
                        u.ModalityID = dr.GetValue(2).ToString();
                    if (!dr.IsDBNull(3))
                        u.ProcedureID = dr.GetValue(3).ToString();
                    if (!dr.IsDBNull(4))
                        u.StudyID = dr.GetValue(4).ToString();
                    if (!dr.IsDBNull(5))
                        u.StartDate = dr.GetValue(5).ToString();
                    if (!dr.IsDBNull(6))
                        u.EndDate = dr.GetValue(6).ToString();
                    if (!dr.IsDBNull(7))
                        u.Status = dr.GetValue(7).ToString();
                    if (!dr.IsDBNull(8))
                        u.Doctor = dr.GetValue(8).ToString();
                    if (!dr.IsDBNull(9))
                        u.AutoExpireDate = dr.GetValue(9).ToString();
                    if (!dr.IsDBNull(10))
                        u.ZDS = dr.GetValue(10).ToString();
                    if (!dr.IsDBNull(11))
                        u.AccessionNumber = dr.GetValue(11).ToString();
                    if (!dr.IsDBNull(12))
                        u.DepartementName = dr.GetValue(12).ToString();
                    if (!dr.IsDBNull(13))
                        u.DocumnetId = dr.GetValue(13).ToString();
                    if (!dr.IsDBNull(14))
                        u.Type = int.Parse(dr.GetValue(14).ToString());
                    if (!dr.IsDBNull(15))
                        u.regStatus = int.Parse(dr.GetValue(15).ToString());
                    if (!dr.IsDBNull(16))
                        u.insertDate = dr.GetDateTime(16);
                    if (!dr.IsDBNull(17))
                        u.InsertUser = dr.GetValue(17).ToString();
                    if (!dr.IsDBNull(18))
                        u.UpdateDeleteReason = dr.GetString(18);
                    if (!dr.IsDBNull(19))
                        u.radPayType = int.Parse(dr.GetValue(19).ToString());
                    res.Add(u);
                }

                return res;
            }
            //catch
            //{
            //    return null;
            //}

            finally
            {
                conn.Close();
            }
        }

        public static List<Radiology> getOrdersWithStatus(double page, out double count, double RowsPerPage, string docid, string firstname, string lastname, string depid, string modid, string beginDate, string endDate)
        {
            List<Radiology> res = new List<Radiology>();

            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());

            try
            {
                conn.Open();

                string idx1 = (RowsPerPage * (page - 1) + 1).ToString();
                string idx2 = (RowsPerPage * page).ToString();
                string whereStr = "SELECT ORDERS.NUM, ORDERS.PATIENTID, ORDERS.MODALITYID, ORDERS.PROCEDUREID,"+
                    " ORDERS.STUDYID, to_date(ORDERS.STARTDATE, 'yyyymmddhh24:mi:ss'), ORDERS.ENDDATE, ORDERS.STATUS,"+
                    " ORDERS.DOCTOR, ORDERS.AUTOEXPIREDATE, ORDERS.ZDS, ORDERS.ACCESSIONNUMBER, ORDERS.DEPTNAME,"+
                    " ORDERS.DOCID, ORDERS.TYPE, ORDERS.REGSTATUS, ORDERS.INSERTDATE, ORDERS.INSERTUSER,"+
                    " ORDERS.UPDATEDELETEREASON FROM ORDERS WHERE 1=1  ";

                string patQr = "select num from PATIENT where 1=1 ";
                string patQr_temp = "select num from PATIENT where 1=1 ";

                if (firstname != null && firstname != "")
                    patQr += " AND PATIENT.FIRSTNAME like '%" + firstname + "%' ";
                if (lastname != null && lastname != "")
                    patQr += " And PATIENT.LASTNAME like '%" + lastname + "%' ";
                if (patQr != patQr_temp)
                    whereStr += " AND ORDERS.PATIENTID in (" + patQr + ") ";

                if (docid != null && docid != "")
                    whereStr += " And ORDERS.DOCID = '" + docid + "' ";
                if (depid != null && depid != "")
                    whereStr += " And ORDERS.DEPTNAME = " + depid + " ";
                if (modid != null && modid != "")
                    whereStr += " And ORDERS.MODALITYID = " + modid + " ";
                //get not completed order 
                whereStr += " And ORDERS.STATUS <> " + ConnectionConfigs.COMPLETED + " ";
                //
                //get recent orders 
                string pacsTime = ConnectionConfigs.getPacsTime();
                string date1 = "";
                string date2 = "";

                if (beginDate == "")
                    date1 = pacsTime;
                else
                    date1 = beginDate;

                if (endDate == "")
                    date2 = pacsTime;
                else
                    date2 = endDate;



                whereStr += " And to_date(STARTDATE, 'yyyymmddhh24:mi:ss') >= to_date('"+ date1 + "', 'yyyymmddhh24:mi:ss') - 1 AND TO_DATE(STARTDATE, 'yyyymmddhh24:mi:ss') <= to_date('" + date2 + "', 'yyyymmddhh24:mi:ss') + 1 ";
                //
                whereStr += " order by to_date(STARTDATE, 'yyyymmddhh24:mi:ss') desc ";

                string sql = "SELECT * FROM (SELECT T.*, ROWNUM RID FROM (" + whereStr + ") T)WHERE RID BETWEEN " + idx1 + " AND " + idx2;

                string countQuery = "SELECT COUNT(*) FROM (" + whereStr + ")";
                OracleCommand cmdCnt = new OracleCommand(countQuery, conn);
                count = Math.Ceiling(double.Parse(cmdCnt.ExecuteScalar().ToString()) / RowsPerPage);


                OracleCommand cmd = new OracleCommand(sql, conn);
                OracleDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Radiology u = new Radiology();
                    if (!dr.IsDBNull(0))
                        u.ID = int.Parse(dr.GetValue(0).ToString());
                    if (!dr.IsDBNull(1))
                        u.PatientID = dr.GetValue(1).ToString();
                    if (!dr.IsDBNull(2))
                        u.ModalityID = dr.GetValue(2).ToString();
                    if (!dr.IsDBNull(3))
                        u.ProcedureID = dr.GetValue(3).ToString();
                    if (!dr.IsDBNull(4))
                        u.StudyID = dr.GetValue(4).ToString();
                    if (!dr.IsDBNull(5))
                        u.StartDate = dr.GetValue(5).ToString();
                    if (!dr.IsDBNull(6))
                        u.EndDate = dr.GetValue(6).ToString();
                    if (!dr.IsDBNull(7))
                        u.Status = dr.GetValue(7).ToString();
                    if (!dr.IsDBNull(8))
                        u.Doctor = dr.GetValue(8).ToString();
                    if (!dr.IsDBNull(9))
                        u.AutoExpireDate = dr.GetValue(9).ToString();
                    if (!dr.IsDBNull(10))
                        u.ZDS = dr.GetValue(10).ToString();
                    if (!dr.IsDBNull(11))
                        u.AccessionNumber = dr.GetValue(11).ToString();
                    if (!dr.IsDBNull(12))
                        u.DepartementName = dr.GetValue(12).ToString();
                    if (!dr.IsDBNull(13))
                        u.DocumnetId = dr.GetValue(13).ToString();
                    if (!dr.IsDBNull(14))
                        u.Type = int.Parse(dr.GetValue(14).ToString());
                    if (!dr.IsDBNull(15))
                        u.regStatus = int.Parse(dr.GetValue(15).ToString());
                    if (!dr.IsDBNull(16))
                        u.insertDate = dr.GetDateTime(16);
                    if (!dr.IsDBNull(17))
                        u.InsertUser = dr.GetValue(17).ToString();
                    if (!dr.IsDBNull(18))
                        u.UpdateDeleteReason = dr.GetString(18);
                    res.Add(u);
                }

                return res;
            }
            //catch
            //{
            //    return null;
            //}

            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// Inserts an order into orders table in database.
        /// </summary>
        /// <param name="ro">object of type Radiology containing the order's details to be inserted in database.</param>
        /// <returns>the exception message if there is one, empty string if not.</returns>
        public static string AddOrder(Radiology ro)
        {
            string res = "";
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                string qr = "Begin " +
                            "  insert into ORDERS " +

                            "( NUM, PATIENTID, MODALITYID, PROCEDUREID, STUDYID, STARTDATE, ENDDATE, STATUS, DOCTOR, AUTOEXPIREDATE, ACCESSIONNUMBER, DEPTNAME, DOCID, TYPE, REGSTATUS, INSERTDATE, INSERTUSER, UPDATEDELETEREASON, PAYTYPE, PAYREASON) " +
                            " values " +
                            "( :NUM, :PATIENTID, :MODALITYID, :PROCEDUREID, :STUDYID, :STARTDATE, :ENDDATE, :STATUS, :DOCTOR, :AUTOEXPIREDATE, :ACCESSIONNUMBER, :DEPTNAME, :DOCID, :TYPE, :REGSTATUS, :INSERTDATE, :INSERTUSER, :UPDATEDELETEREASON, :PAYTYPE, :PAYREASON); " +
                            " End;";
                OracleCommand cmd = new OracleCommand(qr, conn);
                OracleParameter[] param =  {
                                            new OracleParameter("NUM", ro.ID),
                                            new OracleParameter("PATIENTID", ro.PatientID),
                                            new OracleParameter("MODALITYID",int.Parse( ro.ModalityID)),
                                            new OracleParameter("PROCEDUREID", ro.ProcedureID),
                                            new OracleParameter("STUDYID", ro.StudyID),
                                            new OracleParameter("STARTDATE", ro.StartDate),
                                            new OracleParameter("ENDDATE", null),
                                            new OracleParameter("STATUS", ro.Status),
                                            new OracleParameter("DOCTOR", ro.Doctor),
                                            new OracleParameter("AUTOEXPIREDATE", ro.AutoExpireDate),
                                            new OracleParameter("ACCESSIONNUMBER",ro.AccessionNumber),
                                            new OracleParameter("DEPTNAME", ro.DepartementName),
                                            new OracleParameter("DOCID",ro.DocumnetId),
                                            new OracleParameter("TYPE",ro.Type),
                                            new OracleParameter("REGSTATUS",ro.regStatus),
                                            new OracleParameter("INSERTDATE",ro.insertDate),
                                            new OracleParameter("INSERTUSER",ro.InsertUser),
                                            new OracleParameter("UPDATEDELETEREASON",ro.UpdateDeleteReason),
                                            new OracleParameter("PAYTYPE",ro.radPayType),
                                            new OracleParameter("PAYREASON",ro.radPayReason)
                                           };
                for (int j = 0; j < param.Length; j++)
                    cmd.Parameters.Add(param[j]);
                cmd.ExecuteNonQuery();
            }
            catch (OracleException e)
            {
                res = e.Message;
            }
            catch(Exception ee)
            {
                res = ee.Message;
            }
            finally
            {
                conn.Close();
            }
            return res;
        }

        /// <summary>
        /// Inserts an order into oldorders table in database.
        /// </summary>
        /// <remarks>this function used to store old order's data when an order is edited or deleted.</remarks>
        /// <param name="ro">object of type Radiology containing the order's details to be inserted in database.</param>
        /// <returns>the exception message if there is one, empty string if not.</returns>
        public static string AddToOldOrder(Radiology ro)
        {
            ro.ID = OracleRIS.GetOracleSequenceValue("OLDORDERS_SEQ");
            string res = "";
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                string qr = "Begin " +
                            "  insert into OLDORDERS " +

                            "( NUM, PATIENTID, MODALITYID, PROCEDUREID, STUDYID, STARTDATE, ENDDATE, STATUS, DOCTOR, AUTOEXPIREDATE, ACCESSIONNUMBER, DEPTNAME, DOCID, TYPE, REGSTATUS, INSERTDATE, INSERTUSER,UPDATEDATE, UPDATEDUSER, REASON,ID) " +
                            " values " +
                            "( :NUM, :PATIENTID, :MODALITYID, :PROCEDUREID, :STUDYID, :STARTDATE, :ENDDATE, :STATUS, :DOCTOR, :AUTOEXPIREDATE, :ACCESSIONNUMBER, :DEPTNAME, :DOCID, :TYPE, :REGSTATUS, :INSERTDATE, :INSERTUSER,:UPDATEDATE, :UPDATEDUSER, :REASON, :ID); " +
                            " End;";
                OracleCommand cmd = new OracleCommand(qr, conn);
                OracleParameter[] param =  {
                                            new OracleParameter("NUM", ro.ID),
                                            new OracleParameter("PATIENTID", ro.PatientID),
                                            new OracleParameter("MODALITYID",int.Parse( ro.ModalityID)),
                                            new OracleParameter("PROCEDUREID", ro.ProcedureID),
                                            new OracleParameter("STUDYID", ro.StudyID),
                                            new OracleParameter("STARTDATE", ro.StartDate),
                                            new OracleParameter("ENDDATE", null),
                                            new OracleParameter("STATUS", ro.Status),
                                            new OracleParameter("DOCTOR", ro.Doctor),
                                            new OracleParameter("AUTOEXPIREDATE", ro.AutoExpireDate),
                                            new OracleParameter("ACCESSIONNUMBER",ro.AccessionNumber),
                                            new OracleParameter("DEPTNAME", ro.DepartementName),
                                            new OracleParameter("DOCID",ro.DocumnetId),
                                            new OracleParameter("TYPE",ro.Type),
                                            new OracleParameter("REGSTATUS",ro.regStatus),
                                            new OracleParameter("INSERTDATE",ro.insertDate),
                                            new OracleParameter("INSERTUSER",int.Parse(ro.InsertUser)),
                                            new OracleParameter("UPDATEDATE",ro.updateDate),
                                            new OracleParameter("UPDATEDUSER",int.Parse(ro.UpdatetUser)),
                                            new OracleParameter("REASON",ro.UpdateDeleteReason),
                                            new OracleParameter("ID",ro.NewID)
                                           };
                for (int j = 0; j < param.Length; j++)
                    cmd.Parameters.Add(param[j]);
                cmd.ExecuteNonQuery();
            }
            catch (OracleException e)
            {
                res = e.Message;
            }
            catch (Exception ee)
            {
                res = ee.Message;
            }
            finally
            {
                conn.Close();
            }
            return res;
        }

        public static string AddWaitingOrder(Radiology ro)
        {
            string res = "";
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                string qr = "Begin " +
                            "  insert into ORDERS " +

                            "( NUM, PATIENTID, MODALITYID, PROCEDUREID, STUDYID, STATUS, DOCTOR, DEPTNAME, DOCID,TYPE,REGSTATUS, INSERTDATE, INSERTUSER, UPDATEDELETEREASON, PAYTYPE, PAYREASON) " +
                            " values " +
                            "( :NUM, :PATIENTID, :MODALITYID, :PROCEDUREID, :STUDYID, :STATUS, :DOCTOR, :DEPTNAME, :DOCID,:TYPE,:REGSTATUS, :INSERTDATE, :INSERTUSER, :UPDATEDELETEREASON, :PAYTYPE, :PAYREASON); " +
                            " End;";
                OracleCommand cmd = new OracleCommand(qr, conn);
                OracleParameter[] param =  {
                                            new OracleParameter("NUM", ro.ID),
                                            new OracleParameter("PATIENTID", ro.PatientID),
                                            new OracleParameter("MODALITYID",int.Parse( ro.ModalityID)),
                                            new OracleParameter("PROCEDUREID", ro.ProcedureID),
                                            new OracleParameter("STUDYID", ro.StudyID),
                                            new OracleParameter("STUDYID", ro.Status),
                                            new OracleParameter("DOCTOR", ro.Doctor),
                                            new OracleParameter("DEPTNAME", ro.DepartementName),
                                            new OracleParameter("DOCID",ro.DocumnetId),
                                            new OracleParameter("TYPE",ro.Type),
                                            new OracleParameter("REGSTATUS",ro.regStatus),
                                            new OracleParameter("INSERTDATE",ro.insertDate),
                                            new OracleParameter("INSERTUSER",ro.InsertUser),
                                            new OracleParameter("UPDATEDELETEREASON",ro.UpdateDeleteReason),
                                            new OracleParameter("PAYTYPE",ro.radPayType),
                                            new OracleParameter("PAYREASON",ro.radPayReason)
                                           };
                for (int j = 0; j < param.Length; j++)
                    cmd.Parameters.Add(param[j]);
                cmd.ExecuteNonQuery();
            }
            catch (OracleException e)
            {
                res = e.Message;
            }
            catch (Exception ee)
            {
                res = ee.Message;
            }
            finally
            {
                conn.Close();
            }
            return res;
        }

        /// <summary>
        /// Updates order details in the database.
        /// </summary>
        /// <remarks>used when we need to edit an order.</remarks>
        /// <param name="ro">object of type Radiology containing the new order's details to be inserted in database.</param>
        /// <returns>the exception message if there is one, empty string if not.</returns>
        public static string UpdateOrder(Radiology ro)
        {
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            string res = "";
            try
            {
                conn.Open();
                string qr = " Update ORDERS Set " +

                            " MODALITYID = :MODALITYID " +
                            ", PROCEDUREID = :PROCEDUREID " +
                            ", STARTDATE = :STARTDATE " +
                            ", ENDDATE = :ENDDATE " +
                            ", AUTOEXPIREDATE = :AUTOEXPIREDATE " +
                            ", DEPTNAME = :DEPTNAME " +
                            ", DOCID = :DOCID " +
                            ", DOCTOR = :DOCTOR " +
                            ", TYPE = :TYPE " +
                            ", STATUS = :STATUS " +
                            ", REGSTATUS = :REGSTATUS " +
                            ", UPDATEDATE = :UPDATEDATE " +
                            ", UPDATEUSER = :UPDATEUSER " +
                            ", UPDATEDELETEREASON = :UPDATEDELETEREASON " +
                            ", PAYTYPE = :PAYTYPE " +
                            ", PAYREASON = :PAYREASON " +
                            "  WHERE NUM = :NUM ";

                OracleParameter[] param =  {
                                            //new OracleParameter("PATIENTID", ro.PatientID),
                                            new OracleParameter("MODALITYID",int.Parse( ro.ModalityID)),
                                            new OracleParameter("PROCEDUREID", ro.ProcedureID),
                                            //new OracleParameter("STUDYID", ro.StudyID),
                                            new OracleParameter("STARTDATE", ro.StartDate),
                                            new OracleParameter("ENDDATE", null),
                                            new OracleParameter("AUTOEXPIREDATE", ro.AutoExpireDate),
                                            new OracleParameter("DEPTNAME", ro.DepartementName),
                                            new OracleParameter("DOCID",ro.DocumnetId),
                                            new OracleParameter("DOCTOR", ro.Doctor),
                                            new OracleParameter("TYPE",ro.Type),
                                            new OracleParameter("STATUS", ro.Status),
                                            new OracleParameter("REGSTATUS",ro.regStatus),
                                            new OracleParameter("UPDATEDATE",ro.updateDate),
                                            new OracleParameter("UPDATEUSER",ro.UpdatetUser),
                                            new OracleParameter("UPDATEDELETEREASON",ro.UpdateDeleteReason),
                                            new OracleParameter("PAYTYPE",ro.radPayType),
                                            new OracleParameter("PAYREASON",ro.radPayReason),
                                            //new OracleParameter("ACCESSIONNUMBER",ro.AccessionNumber),
                                            new OracleParameter("NUM", ro.ID)
                                           };

                OracleCommand cmd = new OracleCommand(qr, conn);

                for (int j = 0; j < param.Length; j++)
                    cmd.Parameters.Add(param[j]);
                cmd.ExecuteNonQuery();
            }
            catch (OracleException e)
            {
                res = e.Message;
            }
            catch
            {
                res = "حدث خطأ";
            }
            finally
            {
                conn.Close();
            }
            return res;
        }

        /// <summary>
        /// Deletes order details from the database.
        /// </summary>
        /// <remarks>used when we need to delete an order.</remarks>
        /// <param name="ro">object of type Radiology containing theorder's details to be deleted from database.</param>
        /// <returns>the exception message if there is one, empty string if not.</returns>
        public static string DeleteOrder(Radiology ro)
        {
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());

            string res = "";

            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("delete from  ORDERS WHERE NUM = :NUM ", conn);
               // cmd.Parameters.Add(new OracleParameter("REGSTATUS", ro.regStatus));
                cmd.Parameters.Add(new OracleParameter("NUM", ro.ID));
                cmd.ExecuteNonQuery();

            }
            catch (OracleException e)
            {
                res = e.Message;
            }
            catch
            {
                res = "حدث خطأ";
            }
            finally
            {
                conn.Close();
            }
            return res;
        }



        // Other Functions: 

        // This Function gets a list of (Modality Names)

        //public static SelectList GetModalityNamesList(string defaultValue)
        //{
        //    List<SelectListItem> items = new List<SelectListItem>();
        //    items.Add(new SelectListItem { Text = "", Value = "" });
        //    foreach (var item in Modality.getData())
        //    {
        //        items.Add(new SelectListItem { Text = item.name, Value = item.num.ToString() });
        //    }
        //    return new SelectList(items, "Value", "Text", string.IsNullOrEmpty(defaultValue) ? "0" : defaultValue);
        //}

        // <MNM>
        public static List<Radiology> getAuditOrders(double page, out double count, double RowsPerPage, string docid, string firstname, string lastname, string depid, string modid,string orderStatus, string mnmfrom, string mnmto)
        {
            List<Radiology> res = new List<Radiology>();

            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());

            try
            {
                conn.Open();

                string idx1 = (RowsPerPage * (page - 1) + 1).ToString();
                string idx2 = (RowsPerPage * page).ToString();
                string whereStr = "";
                if (!string.IsNullOrEmpty(orderStatus))
                {
                     whereStr = "SELECT OLDORDERS.NUM, OLDORDERS.PATIENTID, OLDORDERS.MODALITYID, OLDORDERS.PROCEDUREID," +
                        " OLDORDERS.STUDYID, to_date(OLDORDERS.STARTDATE, 'yyyymmddhh24:mi:ss'), OLDORDERS.ENDDATE, OLDORDERS.STATUS," +
                        " OLDORDERS.DOCTOR, OLDORDERS.AUTOEXPIREDATE, OLDORDERS.ZDS, OLDORDERS.ACCESSIONNUMBER, OLDORDERS.DEPTNAME," +
                        " OLDORDERS.DOCID, OLDORDERS.TYPE, OLDORDERS.REGSTATUS, OLDORDERS.INSERTDATE, OLDORDERS.INSERTUSER," +
                        " OLDORDERS.UPDATEDUSER,OLDORDERS.UPDATEDATE, " +
                        " OLDORDERS.REASON, OLDORDERS.ID FROM OLDORDERS WHERE 1=1 " +
                        " AND OLDORDERS.REGSTATUS = '" + orderStatus + "' ";
                }
                else
                {
                     whereStr = "SELECT OLDORDERS.NUM, OLDORDERS.PATIENTID, OLDORDERS.MODALITYID, OLDORDERS.PROCEDUREID," +
                        " OLDORDERS.STUDYID, to_date(OLDORDERS.STARTDATE, 'yyyymmddhh24:mi:ss'), OLDORDERS.ENDDATE, OLDORDERS.STATUS," +
                        " OLDORDERS.DOCTOR, OLDORDERS.AUTOEXPIREDATE, OLDORDERS.ZDS, OLDORDERS.ACCESSIONNUMBER, OLDORDERS.DEPTNAME," +
                        " OLDORDERS.DOCID, OLDORDERS.TYPE, OLDORDERS.REGSTATUS, OLDORDERS.INSERTDATE, OLDORDERS.INSERTUSER," +
                        " OLDORDERS.UPDATEDUSER,OLDORDERS.UPDATEDATE, " +
                        " OLDORDERS.REASON, OLDORDERS.ID FROM OLDORDERS WHERE 1=1 " +
                        "  ";
                }

                string patQr = "select num from PATIENT where 1=1 ";
                string patQr_temp = "select num from PATIENT where 1=1 ";

                if (firstname != null && firstname != "")
                    patQr += " AND PATIENT.FIRSTNAME like '%" + firstname + "%' ";
                if (lastname != null && lastname != "")
                    patQr += " And PATIENT.LASTNAME like '%" + lastname + "%' ";
                if (patQr != patQr_temp)
                    whereStr += " AND OLDORDERS.PATIENTID in (" + patQr + ") ";
                if (docid != null && docid != "")
                    whereStr += " And OLDORDERS.DOCID = '" + docid + "' ";
                if (depid != null && depid != "")
                    whereStr += " And OLDORDERS.DEPTNAME = " + int.Parse(depid) + " ";
                if (modid != null && modid != "")
                    whereStr += " And OLDORDERS.MODALITYID = " + modid + " ";
                if (!string.IsNullOrEmpty(mnmfrom))
                {
                    string d1 = mnmfrom.Replace("-", "");
                    whereStr += " And OLDORDERS.UPDATEDATE >=to_date('" + d1 + "000000" + "','yyyymmddhh24:mi:ss') ";
                }
                    
                if (!string.IsNullOrEmpty(mnmto))
                {
                    string d2 = mnmto.Replace("-", "");

                    whereStr += " And OLDORDERS.UPDATEDATE <=to_date('" + d2 + "000000" + "','yyyymmddhh24:mi:ss') ";
                }
                    


                whereStr += " order by OLDORDERS.UPDATEDATE desc ";

                string sql = "SELECT * FROM (SELECT T.*, ROWNUM RID FROM (" + whereStr + ") T)WHERE RID BETWEEN " + idx1 + " AND " + idx2;

                string countQuery = "SELECT COUNT(*) FROM (" + whereStr + ")";
                OracleCommand cmdCnt = new OracleCommand(countQuery, conn);
                count = Math.Ceiling(double.Parse(cmdCnt.ExecuteScalar().ToString()) / RowsPerPage);


                OracleCommand cmd = new OracleCommand(sql, conn);
                OracleDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Radiology u = new Radiology();
                    if (!dr.IsDBNull(0))
                        u.ID = int.Parse(dr.GetValue(0).ToString());
                    if (!dr.IsDBNull(1))
                        u.PatientID = dr.GetValue(1).ToString();
                    if (!dr.IsDBNull(2))
                        u.ModalityID = dr.GetValue(2).ToString();
                    if (!dr.IsDBNull(3))
                        u.ProcedureID = dr.GetValue(3).ToString();
                    if (!dr.IsDBNull(4))
                        u.StudyID = dr.GetValue(4).ToString();
                    if (!dr.IsDBNull(5))
                        u.StartDate = dr.GetValue(5).ToString();
                    if (!dr.IsDBNull(6))
                        u.EndDate = dr.GetValue(6).ToString();
                    if (!dr.IsDBNull(7))
                        u.Status = dr.GetValue(7).ToString();
                    if (!dr.IsDBNull(8))
                        u.Doctor = dr.GetValue(8).ToString();
                    if (!dr.IsDBNull(9))
                        u.AutoExpireDate = dr.GetValue(9).ToString();
                    if (!dr.IsDBNull(10))
                        u.ZDS = dr.GetValue(10).ToString();
                    if (!dr.IsDBNull(11))
                        u.AccessionNumber = dr.GetValue(11).ToString();
                    if (!dr.IsDBNull(12))
                        u.DepartementName = dr.GetValue(12).ToString();
                    if (!dr.IsDBNull(13))
                        u.DocumnetId = dr.GetValue(13).ToString();
                    if (!dr.IsDBNull(14))
                        u.Type = int.Parse(dr.GetValue(14).ToString());
                    if (!dr.IsDBNull(15))
                        u.regStatus = int.Parse(dr.GetValue(15).ToString());
                    if (!dr.IsDBNull(16))
                        u.insertDate = dr.GetDateTime(16);
                    if (!dr.IsDBNull(17))
                        u.InsertUser = dr.GetValue(17).ToString();
                    if (!dr.IsDBNull(18))
                        u.UpdatetUser = dr.GetValue(18).ToString();
                    if (!dr.IsDBNull(19))
                        u.updateDate = dr.GetDateTime(19) ;
                    if (!dr.IsDBNull(20))
                        u.UpdateDeleteReason = dr.GetValue(20).ToString();
                    if (!dr.IsDBNull(21))
                        u.NewID = int.Parse(dr.GetValue(21).ToString());
                    res.Add(u);
                }

                return res;
            }
            //catch
            //{
            //    return null;
            //}

            finally
            {
                conn.Close();
            }
        }

        public static Radiology SelectOld(int num)
        {
            Radiology u = new Radiology();
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                string qr = "SELECT OLDORDERS.NUM, OLDORDERS.PATIENTID, OLDORDERS.MODALITYID, OLDORDERS.PROCEDUREID," +
                           " OLDORDERS.STUDYID, to_date(OLDORDERS.STARTDATE, 'yyyymmddhh24:mi:ss'), OLDORDERS.ENDDATE, OLDORDERS.STATUS," +
                           " OLDORDERS.DOCTOR, OLDORDERS.AUTOEXPIREDATE, OLDORDERS.ZDS, OLDORDERS.ACCESSIONNUMBER, OLDORDERS.DEPTNAME," +
                           " OLDORDERS.DOCID, OLDORDERS.TYPE, OLDORDERS.REGSTATUS, OLDORDERS.INSERTDATE, OLDORDERS.INSERTUSER," +
                           " OLDORDERS.UPDATEDUSER,OLDORDERS.UPDATEDATE, " +
                           " OLDORDERS.REASON, OLDORDERS.ID FROM OLDORDERS " +
                           " WHERE OLDORDERS.NUM = '" + num + "' ";
                conn.Open();
                OracleCommand cmd = new OracleCommand(qr, conn);
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    #region Data
                    if (!dr.IsDBNull(0))
                        u.ID = int.Parse(dr.GetValue(0).ToString());
                    if (!dr.IsDBNull(1))
                        u.PatientID = dr.GetValue(1).ToString();
                    if (!dr.IsDBNull(2))
                        u.ModalityID = dr.GetValue(2).ToString();
                    if (!dr.IsDBNull(3))
                        u.ProcedureID = dr.GetValue(3).ToString();
                    if (!dr.IsDBNull(4))
                        u.StudyID = dr.GetValue(4).ToString();
                    if (!dr.IsDBNull(5))
                        u.StartDate = dr.GetValue(5).ToString();
                    if (!dr.IsDBNull(6))
                        u.EndDate = dr.GetValue(6).ToString();
                    if (!dr.IsDBNull(7))
                        u.Status = dr.GetValue(7).ToString();
                    if (!dr.IsDBNull(8))
                        u.Doctor = dr.GetValue(8).ToString();
                    if (!dr.IsDBNull(9))
                        u.AutoExpireDate = dr.GetValue(9).ToString();
                    if (!dr.IsDBNull(10))
                        u.ZDS = dr.GetValue(10).ToString();
                    if (!dr.IsDBNull(11))
                        u.AccessionNumber = dr.GetValue(11).ToString();
                    if (!dr.IsDBNull(12))
                        u.DepartementName = dr.GetValue(12).ToString();
                    if (!dr.IsDBNull(13))
                        u.DocumnetId = dr.GetValue(13).ToString();
                    if (!dr.IsDBNull(14))
                        u.Type = int.Parse(dr.GetValue(14).ToString());
                    if (!dr.IsDBNull(15))
                        u.regStatus = int.Parse(dr.GetValue(15).ToString());
                    if (!dr.IsDBNull(16))
                        u.insertDate = dr.GetDateTime(16);
                    if (!dr.IsDBNull(17))
                        u.InsertUser = dr.GetValue(17).ToString();
                    if (!dr.IsDBNull(18))
                        u.UpdatetUser = dr.GetValue(18).ToString();
                    if (!dr.IsDBNull(19))
                        u.updateDate = dr.GetDateTime(19);
                    if (!dr.IsDBNull(20))
                        u.UpdateDeleteReason = dr.GetValue(20).ToString();
                    if (!dr.IsDBNull(21))
                        u.NewID = int.Parse(dr.GetValue(21).ToString());
                    #endregion
                }
            }
            catch (Exception e)
            {
                String s = e.Message;
            }
            finally
            {
                conn.Close();
            }
            return u;
        }

        // </MNM>


    }
   
}