using Oracle.DataAccess.Client;
using RIS.Validations;
using RISDB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RIS.Models
{
    public class Appoinments
    {
        public int appID { get; set; }
        [Display(ResourceType = typeof(Resources.Res), Name = "PATIENTNAMEParameter")]
        public int patientID { get; set; }
        [Display(ResourceType = typeof(Resources.Res), Name = "AccNum")]
        public int accNum { get; set; }
        [Required(ErrorMessage = "reqField")]
        [Display(ResourceType = typeof(Resources.Res), Name = "AppDate")]
        public string appDate { get; set; }
        [Display(ResourceType = typeof(Resources.Res), Name = "UpdateDate")]
        public DateTime? updateDate { get; set; }
        [Display(ResourceType = typeof(Resources.Res), Name = "UpdateUser")]
        public string UpdatetUser { get; set; }
        /// <summary>
        /// The ID of the user who inserted the patient's information
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "InsertUser")]
        public int? insertUser { set; get; }

        /// <summary>
        /// The ID of the user who updated or deleted the patient's information
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "UpdateUser")]
        public int? updateUser { set; get; }

        [Display(ResourceType = typeof(Resources.Res), Name = "UpdateDeleteReason")]
        public string UpdateDeleteReason { get; set; }

        [Display(ResourceType = typeof(Resources.Res), Name = "status")]

        public string appStatus { get; set; }
        public string depName { get; set; }

        [Display(ResourceType = typeof(Resources.Res), Name = "InsertDateParameter")]
        public DateTime? appInsertDate { get; set; }
        [Display(ResourceType = typeof(Resources.Res), Name = "depName")]
        [Required(ErrorMessageResourceType = typeof(Resources.Res), ErrorMessageResourceName = "depReq")]
        public int CLINIC { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources.Res), ErrorMessageResourceName = "AppCost")]
        //public int appCost { get; set; }
        /// <summary>
        /// The regestration status of the order, i.e. primary, edited, deleted.
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "mnmRegStatus")]
        public int? regStatus { set; get; }
        public int appCost
        {
            get
            {
                if (this.appPayType == 0)
                    return Appoinments.getClinicCost(this.CLINIC);
                else
                    return appCost = 0;
            }
            set { }
        }
        public static int getClinicCost(int clinicId)
        {
            int res = 0;
            string qr = "SELECT CLINIC.COST from CLINIC where num=" + clinicId;

            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());

            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand(qr, conn);
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                    res = int.Parse(dr.GetValue(0).ToString()); ;

            }
            catch (Exception e)
            {
                conn.Close();
            }
            conn.Close();

            return res;
        }
        public Patient parentR
        {
            get
            {
                return Patient.Select(patientID);
            }
        }
        [Display(ResourceType = typeof(Resources.Res), Name = "ClinicName")]
        [Required(ErrorMessageResourceType = typeof(Resources.Res), ErrorMessageResourceName = "depReq")]

        public string clinicName
        {
            get
            {
                return GeniricIndex.select(CLINIC, "CLINIC").name;
            }
        }
        /// <summary>
        /// The app type.
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "orderType")]
        [Required(ErrorMessageResourceType = typeof(Resources.Res), ErrorMessageResourceName = "orderTypeReq")]
        public int Type { set; get; }


        [Display(ResourceType = typeof(Resources.Res), Name = "appPayType")]
        [Required(ErrorMessageResourceType = typeof(Resources.Res), ErrorMessageResourceName = "patTypeError")]
        public int appPayType { set; get; }


        [Display(ResourceType = typeof(Resources.Res), Name = "appPayReason")]
        [Required(ErrorMessageResourceType = typeof(Resources.Res), ErrorMessageResourceName = "patTypeError")]
        public string appPayReason { set; get; }
        public class OrderType
        {
            public int TypeValue { get; set; }
            public string Name { get; set; }
        }

        public static SelectList AppsTypes(string defaultValue)
        {

            List<SelectListItem> items = new List<SelectListItem>();


            items.Add(new SelectListItem { Text = RIS.Resources.Res.normalOrder, Value = "1" });
            items.Add(new SelectListItem { Text = RIS.Resources.Res.emergencyOrder, Value = "2" });
            return new SelectList(items, "Value", "Text", string.IsNullOrEmpty(defaultValue) ? "0" : defaultValue);

        }

        public static SelectList AppsPayTypes(string defaultValue)
        {

            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = RIS.Resources.Res.patPayed, Value = "0" });
            items.Add(new SelectListItem { Text = RIS.Resources.Res.patFree, Value = "1" });
            return new SelectList(items, "Value", "Text", string.IsNullOrEmpty(defaultValue) ? "0" : defaultValue);

        }
        /// <summary>
        /// The doctor who requested the order.
        /// </summary>
        [ESValidation]
        [Display(ResourceType = typeof(Resources.Res), Name = "RadDoc")]
        public string Doctor { get; set; }
 
        /// <summary>
        /// User object contains the details of the user who inserted the patient's information
        /// </summary>
        public User insUser
        {
            get
            {
                if (insertUser != null)
                    return User.select((int)insertUser);
                else
                    return null;
            }
        }

        /// <summary>
        /// User object contains the details of the user who updated or deleted the patient's information
        /// </summary>
        public User updUser
        {
            get
            {
                if (UpdatetUser != null)
                    return User.select(int.Parse(UpdatetUser));
                else
                    return null;
            }
        }
        public static string AddApp(Appoinments app)
        {
            string res = "";
            app.appInsertDate = DateTime.Now;
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                string qr = "Begin " +
                            "  insert into APPOINMENT " +
                            "( APPID, APPDATE, PATIENTID, STATUS, ACCESSIONNUMBER, DEPTNAME, INSERTDATE, CLINIC, DOCTORNAME, TYPE, PAYTYPE, PAYREASON) " +
                            " values " +
                            "( :APPID, :APPDATE, :PATIENTID, :STATUS, :ACCESSIONNUMBER, :DEPTNAME, :INSERTDATE, :CLINIC, :DOCTORNAME, :TYPE, :PAYTYPE, :PAYREASON); " +
                            " End;";
                OracleCommand cmd = new OracleCommand(qr, conn);
                OracleParameter[] param =  {
                                            new OracleParameter("APPID", app.appID),
                                            new OracleParameter("APPDATE", app.appDate),
                                            new OracleParameter("PATIENTID", app.patientID),
                                            new OracleParameter("STATUS", app.appStatus),
                                            new OracleParameter("ACCESSIONNUMBER",app.accNum),
                                            new OracleParameter("DEPTNAME", app.depName),
                                            new OracleParameter("INSERTDATE",app.appInsertDate),
                                            new OracleParameter("CLINIC",app.CLINIC),
                                            new OracleParameter("DOCTORNAME",app.Doctor),
                                            new OracleParameter("TYPE",app.Type),
                                            new OracleParameter("PAYTYPE",app.appPayType),
                                            new OracleParameter("PAYREASON",app.appPayReason)
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
        /// Gets Appoinments APPOINMENT from database based on the search options inserted by the user.
        /// </summary>
        /// <remarks> if no search options was inserted, this function gets all APPOINMENT. </remarks>
        /// <param name="page">the page APPIDber for paging in the view.</param>
        /// <param name="count">total APPIDber of pages.</param>
        /// <param name="RowsPerPage">APPIDber of APPOINMENT contained in single page.</param>
        /// <param name="firstname">the first name of the patient.</param>
        /// <param name="lastname">the last name of the patient.</param>
        /// <returns> a list of type Radiology APPOINMENT. </returns>
        public static List<Appoinments> getApps(double page, out double count, double RowsPerPage, string firstname, string lastname, int? clinic, int? AppStatus, string appDate)
        {
            List<Appoinments> res = new List<Appoinments>();

            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());

            try
            {
                conn.Open();
                string idx1 = (RowsPerPage * (page - 1) + 1).ToString();
                string idx2 = (RowsPerPage * page).ToString();
                string whereStr = "SELECT APPOINMENT.APPID, APPOINMENT.PATIENTID, APPOINMENT.STATUS, APPOINMENT.ACCESSIONNUMBER, " +
                    "APPOINMENT.INSERTDATE, APPOINMENT.CLINIC, APPOINMENT.APPDATE FROM APPOINMENT WHERE 1=1 ";

                string patQr = "select NUM from PATIENT where 1=1 ";
                string patQr_temp = "select NUM from PATIENT where 1=1 ";

                if (firstname != null && firstname != "")
                    patQr += " AND PATIENT.FIRSTNAME like '%" + firstname + "%' ";
                if (lastname != null && lastname != "")
                    patQr += " And PATIENT.LASTNAME like '%" + lastname + "%' ";
                if (patQr != patQr_temp)
                    whereStr += " AND APPOINMENT.PATIENTID in (" + patQr + ") ";
                if (AppStatus != null)
                    whereStr += " And APPOINMENT.STATUS = '" + AppStatus + "' ";
                if (appDate != null && appDate != "")
                    whereStr += " And to_date(APPOINMENT.APPDATE,'yyyymmddhh24:mi:ss')-1 <= to_date('" + appDate + "','yyyymmddhh24:mi:ss') And to_date(APPOINMENT.APPDATE,'yyyymmddhh24:mi:ss') >= to_date('" + appDate + "','yyyymmddhh24:mi:ss') ";
                //whereStr += " And to_date(APPOINMENT.APPDATE,'yyyymmddhh24:mi:ss')-1 <= to_date('" + appDate + "','yyyymmdd') And to_date(APPOINMENT.APPDATE,'yyyymmddhh24:mi:ss')+1 >= to_date('" + appDate + "','yyyymmdd') ";
                if (clinic != -1)
                    whereStr += " And APPOINMENT.CLINIC = '" + clinic + "' ";
                whereStr += " ORDER BY APPOINMENT.INSERTDATE DESC";
                string sql = "SELECT * FROM (SELECT T.*, ROWNUM RID FROM (" + whereStr + ") T)WHERE RID BETWEEN " + idx1 + " AND " + idx2;
                string countQuery = "SELECT COUNT(*) FROM (" + whereStr + ")";
                OracleCommand cmdCnt = new OracleCommand(countQuery, conn);
                count = Math.Ceiling(double.Parse(cmdCnt.ExecuteScalar().ToString()) / RowsPerPage);


                OracleCommand cmd = new OracleCommand(sql, conn);
                OracleDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Appoinments u = new Appoinments();
                    if (!dr.IsDBNull(0))
                        u.appID = int.Parse(dr.GetValue(0).ToString());
                    if (!dr.IsDBNull(1))
                        u.patientID = int.Parse(dr.GetValue(1).ToString());
                    if (!dr.IsDBNull(2))
                        u.appStatus = dr.GetValue(2).ToString();
                    if (!dr.IsDBNull(3))
                        u.accNum = int.Parse(dr.GetValue(3).ToString());
                    if (!dr.IsDBNull(4))
                        u.appInsertDate = dr.GetDateTime(4);
                    if (!dr.IsDBNull(5))
                        u.CLINIC = int.Parse(dr.GetValue(5).ToString());
                    if (!dr.IsDBNull(6))
                        u.appDate = dr.GetValue(6).ToString();
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
        public static List<Appoinments> getUpdatedApps(double page, out double count, double RowsPerPage, string firstname, string lastname, int? clinic, int? AppStatus, string appDate,int? regStatus)
        {
            List<Appoinments> res = new List<Appoinments>();

            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());

            try
            {
                conn.Open();
                string idx1 = (RowsPerPage * (page - 1) + 1).ToString();
                string idx2 = (RowsPerPage * page).ToString();
                string whereStr = "SELECT OLDAPPOINMENT.APPID, OLDAPPOINMENT.PATIENTID, OLDAPPOINMENT.STATUS, OLDAPPOINMENT.ACCESSIONNUMBER, " +
                    "OLDAPPOINMENT.INSERTDATE, OLDAPPOINMENT.CLINIC, OLDAPPOINMENT.APPDATE," +
                    "OLDAPPOINMENT.UPDATEDATE, OLDAPPOINMENT.UPDATEUSER, OLDAPPOINMENT.UPDATEDELETEREASON,OLDAPPOINMENT.REGSTATUS FROM OLDAPPOINMENT WHERE 1=1 ";

                string patQr = "select NUM from PATIENT where 1=1 ";
                string patQr_temp = "select NUM from PATIENT where 1=1 ";

                if (firstname != null && firstname != "")
                    patQr += " AND PATIENT.FIRSTNAME like '%" + firstname + "%' ";
                if (lastname != null && lastname != "")
                    patQr += " And PATIENT.LASTNAME like '%" + lastname + "%' ";
                if (patQr != patQr_temp)
                    whereStr += " AND OLDAPPOINMENT.PATIENTID in (" + patQr + ") ";
                if (AppStatus != null)
                    whereStr += " And OLDAPPOINMENT.STATUS = '" + AppStatus + "' ";
                if (appDate != null && appDate != "")
                    whereStr += " And to_date(APPOINMENT.APPDATE,'yyyymmddhh24:mi:ss')-1 <= to_date('" + appDate + "','yyyymmddhh24:mi:ss') And to_date(APPOINMENT.APPDATE,'yyyymmddhh24:mi:ss') >= to_date('" + appDate + "','yyyymmddhh24:mi:ss') ";
                if (clinic != -1)
                    whereStr += " And OLDAPPOINMENT.CLINIC = '" + clinic + "' ";
                if (regStatus != null)
                    whereStr += " And OLDAPPOINMENT.REGSTATUS = '" + regStatus + "' ";

                string sql = "SELECT * FROM (SELECT T.*, ROWNUM RID FROM (" + whereStr + ") T)WHERE RID BETWEEN " + idx1 + " AND " + idx2;

                string countQuery = "SELECT COUNT(*) FROM (" + whereStr + ")";
                OracleCommand cmdCnt = new OracleCommand(countQuery, conn);
                count = Math.Ceiling(double.Parse(cmdCnt.ExecuteScalar().ToString()) / RowsPerPage);


                OracleCommand cmd = new OracleCommand(sql, conn);
                OracleDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Appoinments u = new Appoinments();
                    if (!dr.IsDBNull(0))
                        u.appID = int.Parse(dr.GetValue(0).ToString());
                    if (!dr.IsDBNull(1))
                        u.patientID = int.Parse(dr.GetValue(1).ToString());
                    if (!dr.IsDBNull(2))
                        u.appStatus = dr.GetValue(2).ToString();
                    if (!dr.IsDBNull(3))
                        u.accNum = int.Parse(dr.GetValue(3).ToString());
                    if (!dr.IsDBNull(4))
                        u.appInsertDate = dr.GetDateTime(4);
                    if (!dr.IsDBNull(5))
                        u.CLINIC = int.Parse(dr.GetValue(5).ToString());
                    if (!dr.IsDBNull(6))
                        u.appDate = dr.GetValue(6).ToString();
                    if (!dr.IsDBNull(7))
                        u.updateDate = dr.GetDateTime(7);
                    if (!dr.IsDBNull(8))
                        u.UpdatetUser = dr.GetValue(8).ToString();
                    if (!dr.IsDBNull(9))
                        u.UpdateDeleteReason = dr.GetValue(9).ToString();
                    if (!dr.IsDBNull(10))
                        u.regStatus = int.Parse(dr.GetValue(10).ToString());
                    res.Add(u);
                }

                return res;
            }

            finally
            {
                conn.Close();
            }
        }
        public static Appoinments SelectExactApp(int appId)
        {
            Appoinments app = new Appoinments();
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("SELECT APPID, PATIENTID, STATUS, ACCESSIONNUMBER, INSERTDATE, CLINIC, APPDATE, DOCTORNAME, PAYTYPE, PAYREASON, UPDATEDELETEREASON FROM APPOINMENT WHERE APPID=" + appId, conn);
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    if (dr.Read())
                    {
                        #region Data
                        if (!dr.IsDBNull(0))
                            app.appID = int.Parse(dr.GetValue(0).ToString());
                        if (!dr.IsDBNull(1))
                            app.patientID = int.Parse(dr.GetValue(1).ToString());
                        if (!dr.IsDBNull(2))
                            app.appStatus = dr.GetValue(2).ToString();
                        if (!dr.IsDBNull(3))
                            app.accNum = int.Parse(dr.GetValue(3).ToString());
                        if (!dr.IsDBNull(4))
                            app.appInsertDate = dr.GetDateTime(4);
                        if (!dr.IsDBNull(5))
                            app.CLINIC = dr.GetInt16(5);
                        if (!dr.IsDBNull(6))
                            app.appDate = dr.GetValue(6).ToString();
                        if (!dr.IsDBNull(7))
                            app.Doctor = dr.GetValue(7).ToString();
                        if (!dr.IsDBNull(8))
                            app.appPayType = int.Parse(dr.GetValue(8).ToString());
                        if (!dr.IsDBNull(9))
                            app.appPayReason = dr.GetValue(9).ToString();
                        if (!dr.IsDBNull(10))
                            app.UpdateDeleteReason = dr.GetValue(10).ToString();
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
            return app;
        }
        public static List<Appoinments> getPatientApps(int patientId)
        {
            List<Appoinments> res = new List<Appoinments>();

            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());

            try
            {
                conn.Open();
                string sql = "SELECT APPOINMENT.APPID, APPOINMENT.PATIENTID, APPOINMENT.STATUS, APPOINMENT.ACCESSIONNUMBER, " +
                    "APPOINMENT.INSERTDATE, APPOINMENT.CLINIC, APPOINMENT.APPDATE, APPOINMENT.DOCTORNAME FROM APPOINMENT WHERE PATIENTID=" + patientId +
                    " ORDER BY APPOINMENT.INSERTDATE DESC";

                OracleCommand cmd = new OracleCommand(sql, conn);
                OracleDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Appoinments u = new Appoinments();
                    if (!dr.IsDBNull(0))
                        u.appID = int.Parse(dr.GetValue(0).ToString());
                    if (!dr.IsDBNull(1))
                        u.patientID = int.Parse(dr.GetValue(1).ToString());
                    if (!dr.IsDBNull(2))
                        u.appStatus = dr.GetValue(2).ToString();
                    if (!dr.IsDBNull(3))
                        u.accNum = int.Parse(dr.GetValue(3).ToString());
                    if (!dr.IsDBNull(4))
                        u.appInsertDate = dr.GetDateTime(4);
                    if (!dr.IsDBNull(5))
                        u.CLINIC = int.Parse(dr.GetValue(5).ToString());
                    if (!dr.IsDBNull(6))
                        u.appDate = dr.GetValue(6).ToString();
                    if (!dr.IsDBNull(7))
                        u.Doctor = dr.GetValue(7).ToString();
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

        public static string UpdateApp(Appoinments editedApp)
        {
            int? ins = null;
            if (editedApp.UpdatetUser != null)
                ins = int.Parse(editedApp.UpdatetUser);
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            string res = "";
            try
            {
                conn.Open();
                string qr = " Update APPOINMENT Set " +
                            " APPDATE = :APPDATE " +
                            ", CLINIC = :CLINIC " +
                            ", DEPTNAME = :DEPTNAME " +
                            ", STATUS = :STATUS " +
                            ", UPDATEDATE = :UPDATEDATE " +
                            ", UPDATEUSER = :UPDATEUSER " +
                            ", UPDATEDELETEREASON = :UPDATEDELETEREASON " +
                            ", DOCTORNAME = :DOCTORNAME " +
                            ", REGSTATUS = :REGSTATUS " +
                            "  WHERE APPID = :APPID ";

                OracleParameter[] param =  {
                                            new OracleParameter("APPDATE", editedApp.appDate),
                                            new OracleParameter("CLINIC", editedApp.CLINIC),
                                            new OracleParameter("DEPTNAME", editedApp.depName),
                                            new OracleParameter("STATUS", editedApp.appStatus),
                                            new OracleParameter("UPDATEDATE",editedApp.updateDate),
                                            new OracleParameter("UPDATEUSER",editedApp.UpdatetUser),
                                            new OracleParameter("UPDATEDELETEREASON",editedApp.UpdateDeleteReason),
                                            new OracleParameter("DOCTORNAME",editedApp.Doctor),
                                            new OracleParameter("REGSTATUS",editedApp.regStatus),
                                            new OracleParameter("APPID", editedApp.appID)
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
            catch (Exception exs)
            {
                res = "حدث خطأ";
            }
            finally
            {
                conn.Close();
            }
            return res;
        }

        // Modar for Accounting
        public static List<Appoinments> getPatientsAppsByDate(int id, string appDate)
        {
            List<Appoinments> apps = new List<Appoinments>();
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                string sql = "SELECT APPID, to_date(APPDATE, 'yyyymmddhh24:mi:ss'), STATUS, DOCTORNAME, TYPE," +
                    " PAYTYPE, PAYREASON, CLINIC FROM APPOINMENT WHERE PATIENTID=" + id + " AND to_date(APPDATE, 'yyyymmddhh24:mi:ss') BETWEEN to_date('" + appDate +
                    "', 'yyyymmddhh24:mi:ss') AND to_date('" + appDate + "', 'yyyymmddhh24:mi:ss')+1 ORDER BY to_date(APPDATE, 'yyyymmddhh24:mi:ss') DESC";
                OracleCommand cmd = new OracleCommand(sql, conn);
                OracleDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Appoinments app = new Appoinments();
                    #region Data
                    if (!dr.IsDBNull(0))
                        app.appID = int.Parse(dr.GetValue(0).ToString());
                    if (!dr.IsDBNull(1))
                        app.appDate = dr.GetValue(1).ToString();
                    if (!dr.IsDBNull(2))
                        app.appStatus = dr.GetValue(2).ToString();
                    if (!dr.IsDBNull(3))
                        app.Doctor = dr.GetValue(3).ToString();
                    if (!dr.IsDBNull(4))
                        app.Type = int.Parse(dr.GetValue(4).ToString());
                    if (!dr.IsDBNull(5))
                        app.appPayType = int.Parse(dr.GetValue(5).ToString());
                    if (!dr.IsDBNull(6))
                        app.appPayReason = dr.GetValue(6).ToString();
                    if (!dr.IsDBNull(7))
                        app.CLINIC = int.Parse(dr.GetValue(7).ToString());
                    #endregion

                    apps.Add(app);
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
            return apps;
        }

        public static List<Appoinments> getPatientsAppsBetweenDates(int id, string fromDate, string toDate)
        {
            List<Appoinments> apps = new List<Appoinments>();
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                string sql = "SELECT APPID, to_date(APPDATE, 'yyyymmddhh24:mi:ss'), STATUS, DOCTORNAME, TYPE," +
                    " PAYTYPE, PAYREASON, CLINIC FROM APPOINMENT WHERE PATIENTID=" + id + " AND to_date(APPDATE, 'yyyymmddhh24:mi:ss') BETWEEN to_date('" + fromDate +
                    "', 'yyyymmddhh24:mi:ss') AND to_date('" + toDate + "', 'yyyymmddhh24:mi:ss')+1 ORDER BY to_date(APPDATE, 'yyyymmddhh24:mi:ss') DESC";
                OracleCommand cmd = new OracleCommand(sql, conn);
                OracleDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Appoinments app = new Appoinments();
                    #region Data
                    if (!dr.IsDBNull(0))
                        app.appID = int.Parse(dr.GetValue(0).ToString());
                    if (!dr.IsDBNull(1))
                        app.appDate = dr.GetValue(1).ToString();
                    if (!dr.IsDBNull(2))
                        app.appStatus = dr.GetValue(2).ToString();
                    if (!dr.IsDBNull(3))
                        app.Doctor = dr.GetValue(3).ToString();
                    if (!dr.IsDBNull(4))
                        app.Type = int.Parse(dr.GetValue(4).ToString());
                    if (!dr.IsDBNull(5))
                        app.appPayType = int.Parse(dr.GetValue(5).ToString());
                    if (!dr.IsDBNull(6))
                        app.appPayReason = dr.GetValue(6).ToString();
                    if (!dr.IsDBNull(7))
                        app.CLINIC = int.Parse(dr.GetValue(7).ToString());
                    #endregion

                    apps.Add(app);
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
            return apps;
        }

        /// <summary>
        /// Inserts an order into oldorders table in database.
        /// </summary>
        /// <remarks>this function used to store old order's data when an order is edited or deleted.</remarks>
        /// <param name="oldApp">object of type Radiology containing the order's details to be inserted in database.</param>
        /// <returns>the exception message if there is one, empty string if not.</returns>
        public static string AddToOldApps(Appoinments oldApp, int newID)
        {
            string res = "";
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                string qr = "Begin " +
                            "  insert into OLDAPPOINMENT " +
                            "( APPID, PATIENTID, APPDATE, STATUS, ACCESSIONNUMBER, DEPTNAME, INSERTDATE,UPDATEDATE, UPDATEUSER, UPDATEDELETEREASON, CLINIC, APPNEWID) " +
                            " values " +
                            "( :APPID, :PATIENTID, :APPDATE, :STATUS, :ACCESSIONNUMBER, :DEPTNAME, :INSERTDATE,:UPDATEDATE, :UPDATEUSER, :UPDATEDELETEREASON, :CLINIC, :APPNEWID); " +
                            " End;";
                OracleCommand cmd = new OracleCommand(qr, conn);
                OracleParameter[] param =  {
                                            new OracleParameter("APPID", oldApp.appID),
                                            new OracleParameter("PATIENTID", oldApp.patientID),
                                            new OracleParameter("APPDATE", oldApp.appDate),
                                            new OracleParameter("STATUS", oldApp.appStatus),
                                            new OracleParameter("ACCESSIONNUMBER",oldApp.accNum),
                                            new OracleParameter("DEPTNAME", oldApp.depName),
                                            new OracleParameter("INSERTDATE",oldApp.appInsertDate),
                                            new OracleParameter("UPDATEDATE",oldApp.updateDate),
                                            new OracleParameter("UPDATEUSER",int.Parse(oldApp.UpdatetUser)),
                                            new OracleParameter("UPDATEDELETEREASON",oldApp.UpdateDeleteReason),
                                            new OracleParameter("CLINIC",oldApp.CLINIC),
                                            new OracleParameter("UPDATEDELETEREASON",newID),

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
        public static string DeleteApp(Appoinments app)
        {
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());

            string res = "";

            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("delete from  APPOINMENT WHERE APPID = :APPID ", conn);
                // cmd.Parameters.Add(new OracleParameter("REGSTATUS", ro.regStatus));
                cmd.Parameters.Add(new OracleParameter("APPID", app.appID));
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


        public static AppSheduleData getAppsByPeriod(string date)
        {
            AppSheduleData so = new Models.AppSheduleData();
            so.tdId = date;
            string[] temp = date.Split('_');
            string myDate = temp[0].Replace("-", "");
            string qr = "SELECT APPOINMENT.APPID from APPOINMENT where to_date(APPDATE, 'yyyymmddhh24:mi:ss') >=to_date('" + myDate + temp[1] + temp[2] + "00" + "','yyyymmddhh24:mi:ss')";
            qr += " AND to_date(APPDATE, 'yyyymmddhh24:mi:ss') <=to_date('" + myDate + temp[1] + (int.Parse(temp[2]) + 14).ToString() + "00" + "','yyyymmddhh24:mi:ss')";

            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand(qr, conn);
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                    so.appId = dr.GetValue(0).ToString();
                else
                    so.appId = "-1";
                conn.Close();
            }
            catch (Exception e)
            {
                conn.Close();

            }
            conn.Close();

            return so;
        }

        public static bool getAppsByDay(string date)
        {
            bool res = false;
            string appDate = date.Replace("-", "");
            string endDate = DateTime.Parse(date).AddDays(1).ToString("yyyy-MM-dd");
            endDate = endDate.Replace("-", "");
            string qr = "SELECT APPOINMENT.APPID from APPOINMENT where to_date(APPDATE, 'yyyymmddhh24:mi:ss') >=to_date('" + appDate + "000000" + "','yyyymmddhh24:mi:ss')";
            qr += " AND to_date(APPDATE, 'yyyymmddhh24:mi:ss') <to_date('" + endDate + "000000" + "','yyyymmddhh24:mi:ss')";

            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());

            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand(qr, conn);
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                    res = true;

            }
            catch (Exception e)
            {
                conn.Close();

            }
            conn.Close();

            return res;
        }

        public static bool getAppsByHour(string date, string hour)
        {
            bool res = false;

            string APPDATE = date.Replace("-", "");
            string endDate = DateTime.Parse(date).AddDays(1).ToString("yyyy-MM-dd");
            endDate = endDate.Replace("-", "");
            string qr = "SELECT APPOINMENT.APPID from APPOINMENT where to_date(APPDATE, 'yyyymmddhh24:mi:ss') >=to_date('" + APPDATE + hour + "0000" + "','yyyymmddhh24:mi:ss')";
            qr += " AND to_date(APPDATE, 'yyyymmddhh24:mi:ss') <=to_date('" + APPDATE + hour + "5959" + "','yyyymmddhh24:mi:ss')";

            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());

            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand(qr, conn);
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                    res = true;

            }
            catch (Exception e)
            {
                conn.Close();

            }
            conn.Close();

            return res;
        }

        public static string getAppsByStep(string date, string hour, string min, string step, string clinic)
        {
            string res = "-1";

            string APPDATE = date.Replace("-", "");
            string endDate = DateTime.Parse(date).AddDays(1).ToString("yyyy-MM-dd");
            endDate = endDate.Replace("-", "");
            string qr = "";
            if (string.IsNullOrEmpty(clinic))
            {
                qr = "SELECT APPOINMENT.APPID from APPOINMENT where to_date(APPDATE, 'yyyymmddhh24:mi:ss') >=to_date('" + APPDATE + hour + min + "00" + "','yyyymmddhh24:mi:ss')";
                qr += " AND to_date(APPDATE, 'yyyymmddhh24:mi:ss') <=to_date('" + APPDATE + hour + (int.Parse(min) + int.Parse(step)).ToString("D2") + "59" + "','yyyymmddhh24:mi:ss')";
            }
            else
            {
                qr = "SELECT APPOINMENT.APPID from APPOINMENT where to_date(APPDATE, 'yyyymmddhh24:mi:ss') >=to_date('" + APPDATE + hour + min + "00" + "','yyyymmddhh24:mi:ss')";
                qr += " AND to_date(APPDATE, 'yyyymmddhh24:mi:ss') <=to_date('" + APPDATE + hour + (int.Parse(min) + int.Parse(step)).ToString("D2") + "59" + "','yyyymmddhh24:mi:ss') and clinic='" + clinic + "' ";
            }


            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());

            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand(qr, conn);
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                    res = dr.GetValue(0).ToString();

            }
            catch (Exception e)
            {
                conn.Close();

            }
            conn.Close();

            return res;
        }


        public static PreviewApp previewApp(int id)
        {
            PreviewApp p = new Models.PreviewApp();


            string qr = "SELECT APPOINMENT.CLINIC,APPOINMENT.PATIENTID,to_date(APPOINMENT.APPDATE, 'yyyymmddhh24:mi:ss') from APPOINMENT where APPID='" + id + "'";

            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            OracleCommand cmd = new OracleCommand();
            int cid = 0; int pid = 0;
            string appDate = "";
            try
            {
                conn.Open();
                cmd = new OracleCommand(qr, conn);
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    cid = int.Parse(dr.GetValue(0).ToString());
                    pid = int.Parse(dr.GetValue(1).ToString());
                    appDate = (dr.GetValue(2).ToString());
                }
                conn.Close();
            }
            catch (Exception e)
            {
                string s = e.ToString();
                conn.Close();

            }

            string qr1 = "SELECT CLINIC.NAME from CLINIC where num='" + cid + "'";

            try
            {
                conn.Open();
                cmd = new OracleCommand(qr1, conn);
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    p.clinicName = dr.GetValue(0).ToString();
                }
                conn.Close();
            }
            catch (Exception e)
            {
                string s = e.ToString();

                conn.Close();

            }
            string qr2 = "SELECT PATIENT.FIRSTNAME,PATIENT.MIDDLENAME,PATIENT.LASTNAME from PATIENT where ID='" + pid + "'";

            try
            {
                conn.Open();
                cmd = new OracleCommand(qr2, conn);
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    string f = "", m = "", l = "", nam = "";
                    if (dr.GetValue(0) != null)
                        f = dr.GetValue(0).ToString();
                    if (dr.GetValue(1) != null)
                        m = " " + dr.GetValue(1).ToString();
                    if (dr.GetValue(2) != null)
                        l = " " + dr.GetValue(2).ToString();
                    p.patientName = f + " " + m + " " + l;
                }
                conn.Close();
            }
            catch (Exception e)
            {
                string s = e.ToString();

                conn.Close();

            }

            p.id = id.ToString();
            p.appDate = appDate;

            return p;
        }


    }

    public class PreviewApp
    {
        public string id { set; get; }

        public string patientName { get; set; }
        public string clinicName { get; set; }

        public string appDate { set; get; }

    }

    public class ScheduleOrder
    {
        public int ID { set; get; }

        public string PatientID { set; get; }

        public string ModalityID { set; get; }

        public string ProcedureID { set; get; }

        public string StudyID { set; get; }

        public string APPDATE { set; get; }

        public string EndDate { set; get; }

        public string Status { set; get; }

        public string Doctor { set; get; }

        public string AutoExpireDate { set; get; }

        public string AccessionNumber { set; get; }

        public string DepartementName { set; get; }

        public string DocumnetId { set; get; }

        public int Type { set; get; }

    }

    public class AppSheduleData
    {
        public string appId { set; get; }
        public string tdId { set; get; }
    }

}