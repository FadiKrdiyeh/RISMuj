using Oracle.DataAccess.Client;
using RISDB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Data;
using System.IO;

namespace RIS.Models
{
    /// <summary>
    /// Class of patient
    /// </summary>
    public class Patient
    {
        #region Attributes

        /// <summary>
        /// Patient ID, primary key of patient table in database
        /// </summary>
        public int num { set; get; }

        /// <summary>
        /// unique ID for patient
        /// </summary>
        public int id { set; get; }

        /// <summary>
        /// The GivenID of patient represent a patient number in RIS system
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "PatientGivenId")]
        //[Required(ErrorMessageResourceType = typeof(Resources.Res), ErrorMessageResourceName = "PatientGivenIdError")]
        public string givenid { set; get; }



        /// <summary>
        /// patient's first name
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "PatientFirstname")]
        [Required(ErrorMessageResourceType = typeof(Resources.Res), ErrorMessageResourceName = "PatientFirstnameError")]
        public string firstname { set; get; }

        /// <summary>
        /// patient middle name
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "PatientMiddlename")]
        public string middlename { set; get; }

        /// <summary>
        /// patient's last name
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "PatientLastname")]
        [Required(ErrorMessageResourceType = typeof(Resources.Res), ErrorMessageResourceName = "PatientLastnameError")]
        public string lastname { set; get; }

        /// <summary>
        /// patient's gender
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "PatientGendre")]
        public int gendre { set; get; }

        /// <summary>
        /// patient's mother name
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "PatientMothername")]
        public string mothername { set; get; }

        /// <summary>
        /// patient's birth date
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "PatientBirhtDate")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? birthdate { set; get; }

        /// <summary>
        /// patient's age
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "PatientAge")]
        public int? age { set; get; }
        [Required(ErrorMessage = "هذا الحقل اجباري")]

        [Display(ResourceType = typeof(Resources.Res), Name = "tshAcceptanceType")]
        public int acceptanceType { set; get; }

        /// <summary>
        /// patient's mobile phone
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "PatientMobilephone")]
        public string mobilephone { set; get; }

        /// <summary>
        /// patient's land phone
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "PatientLandphone")]
        public string landphone { set; get; }

        /// <summary>
        /// patient's current address
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "PatientCurrentaddress")]
        public string currentaddress { set; get; }

        /// <summary>
        /// patient's residence address
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "PatientResidentaddress")]
        public string residentaddress { set; get; }

        /// <summary>
        /// patient's work phone
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "PatientWorkphone")]
        public string workphone { set; get; }

        /// <summary>
        /// patient's work address
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "PatientWorkaddress")]
        public string workaddress { set; get; }

        /// <summary>
        /// patient's nearest person name
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "PatientNearestperson")]
        public string nearestperson { set; get; }

        /// <summary>
        /// patient's nearest person phone
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "PatientNearestpersonphone")]
        public string nearestpersonphone { set; get; }

        /// <summary>
        /// patient's birth place
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "PatientBirthplace")]
        public string birthplace { set; get; }

        /// <summary>
        /// national number of the patient
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "PatientNationalidnumber")]
        public string nationalidnumber { set; get; }

        /// <summary>
        /// patient's nationality
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "PatientNationality")]
        public string nationality { set; get; }

        /// <summary>
        /// patient's work type
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "PatientWorktype")]
        public string worktype { set; get; }

        /// <summary>
        /// notes about the patient
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "PatientNotes")]
        public string notes { set; get; }

        /// <summary>
        /// patient's marital status
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "PatientMartialstatus")]
        public int? martialstatus { set; get; }

        /// <summary>
        /// BarCode for the patient based on his GivenID
        /// </summary>
        public Byte[] barCode
        {
            get
            {
                try {
                    using (var ms = new MemoryStream())
                    {
                        BarcodeLib.Barcode bc = new BarcodeLib.Barcode();
                        BarcodeLib.AlignmentPositions align = BarcodeLib.AlignmentPositions.CENTER;
                        BarcodeLib.TYPE type = BarcodeLib.TYPE.CODE128C;
                        bc.Alignment = align;
                        bc.IncludeLabel = true;
                        bc.LabelPosition = BarcodeLib.LabelPositions.BOTTOMCENTER;
                        System.Drawing.Image im = bc.Encode(type, givenid, 200, 75);
                        im.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        return ms.ToArray();
                    }
                }
                catch { return null; }
            }
        }

        /// <summary>
        /// first name of the patient translated to english
        /// </summary>
        public string translatedFname { set; get; }

        /// <summary>
        /// last name of the patient translated to english
        /// </summary>
        public string translatedLname { set; get; }

        /// <summary>
        /// middle name of the patient translated to english
        /// </summary>
        public string translatedFathername { set; get; }

        /// <summary>
        /// mother name of the patient translated to english
        /// </summary>
        public string translatedMothername { set; get; }

        /// <summary>
        /// The date when the user information inserted into RIS
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "PatientInsertdate")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy H:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime? insertdate { set; get; }

    
        //for audit delete update


        /// <summary>
        /// the date when the information of the patient were updated or deleted
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "UpdateDate")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy H:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime? updateDate { set; get; }

        /// <summary>
        /// status of patient;s information i.e. primary, edited, deleted
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "OrderRegStatus")]
        public int? regStatus { set; get; }

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

        /// <summary>
        /// Text explaining the reason of edition or deletion of the patient's information
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "UpdateDeleteReason")]
        public string updateDeleteReason { set; get; }

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
                if (updateUser != null)
                    return User.select((int)updateUser);
                else
                    return null;
            }
        }

        // end for audit delete update



        #endregion

        #region Functions
        // bar code

        /// <summary>
        /// patient object constructor
        /// </summary>
        public Patient() { }

        /// <summary>
        /// Gets all patients 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="count"></param>
        /// <param name="RowsPerPage"></param>
        /// <returns></returns>
        public static List<Patient> getData(double page, out double count, double RowsPerPage)
        {
            List<Patient> ptList = new List<Patient>();

            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                string countQuery = "SELECT COUNT(*) FROM  PATIENT ";
                OracleCommand cmdCnt = new OracleCommand(countQuery, conn);
                count = Math.Ceiling(double.Parse(cmdCnt.ExecuteScalar().ToString()) / RowsPerPage);
                //  OracleCommand cmd = new OracleCommand("SELECT * FROM PATIENT ", conn);

                string idx1 = (RowsPerPage * (page - 1) + 1).ToString();
                string idx2 = (RowsPerPage * page).ToString();
                string whereStr = "SELECT * FROM  PATIENT order by NUM desc ";
                string sql = "SELECT * FROM (SELECT T.*, ROWNUM RID FROM (" + whereStr + ") T)WHERE RID BETWEEN " + idx1 + " AND " + idx2;

                OracleCommand cmd = new OracleCommand(sql, conn);
                OracleDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {

                    Patient pt = new Patient();
                    #region Data
                    if (!dr.IsDBNull(0))
                        pt.num = Int32.Parse(dr.GetValue(0).ToString());
                    if (!dr.IsDBNull(1))
                        pt.id = Int32.Parse(dr.GetValue(1).ToString());
                    if (!dr.IsDBNull(2))
                        pt.givenid = dr.GetString(2);
                    if (!dr.IsDBNull(3))
                        pt.firstname = dr.GetString(3);
                    if (!dr.IsDBNull(4))
                        pt.middlename = dr.GetString(4);
                    if (!dr.IsDBNull(5))
                        pt.lastname = dr.GetString(5);
                    if (!dr.IsDBNull(6))
                        pt.gendre = Int32.Parse(dr.GetValue(6).ToString());
                    if (!dr.IsDBNull(7))
                        pt.mothername = dr.GetString(7);
                    if (!dr.IsDBNull(8))
                        pt.birthdate = dr.GetDateTime(8);
                    if (!dr.IsDBNull(9))
                        pt.age = Int32.Parse(dr.GetValue(9).ToString());
                    if (!dr.IsDBNull(10))
                        pt.mobilephone = dr.GetString(10);
                    if (!dr.IsDBNull(11))
                        pt.landphone = dr.GetString(11);
                    if (!dr.IsDBNull(12))
                        pt.currentaddress = dr.GetString(12);
                    if (!dr.IsDBNull(13))
                        pt.residentaddress = dr.GetString(13);
                    if (!dr.IsDBNull(14))
                        pt.workphone = dr.GetString(14);
                    if (!dr.IsDBNull(15))
                        pt.workaddress = dr.GetString(15);
                    if (!dr.IsDBNull(16))
                        pt.nearestperson = dr.GetString(16);
                    if (!dr.IsDBNull(17))
                        pt.nearestpersonphone = dr.GetString(17);
                    if (!dr.IsDBNull(18))
                        pt.birthplace = dr.GetString(18);
                    if (!dr.IsDBNull(19))
                        pt.nationalidnumber = dr.GetString(19);
                    if (!dr.IsDBNull(20))
                        pt.nationality = dr.GetString(20);
                    if (!dr.IsDBNull(21))
                        pt.worktype = dr.GetString(21);
                    if (!dr.IsDBNull(22))
                        pt.notes = dr.GetString(22);
                    if (!dr.IsDBNull(23))
                        pt.martialstatus = Int32.Parse(dr.GetValue(23).ToString());
                    if (!dr.IsDBNull(24))
                        pt.translatedFname = dr.GetString(24);
                    if (!dr.IsDBNull(25))
                        pt.translatedLname = dr.GetString(25);
                    if (!dr.IsDBNull(26))
                        pt.translatedFathername = dr.GetString(26);
                    if (!dr.IsDBNull(27))
                        pt.translatedMothername = dr.GetString(27);
                    if (!dr.IsDBNull(28))
                        pt.insertdate = dr.GetDateTime(28);
                    //audit
                    if (!dr.IsDBNull(29))
                        pt.insertUser = Int32.Parse(dr.GetValue(29).ToString());
                    if (!dr.IsDBNull(30))
                        pt.updateUser = Int32.Parse(dr.GetValue(30).ToString());
                    if (!dr.IsDBNull(31))
                        pt.updateDate = dr.GetDateTime(31);
                    if (!dr.IsDBNull(32))
                        pt.updateDeleteReason = dr.GetString(32);
                    if (!dr.IsDBNull(33))
                        pt.regStatus = Int32.Parse(dr.GetValue(33).ToString());
                    //end audit



                    #endregion
                    ptList.Add(pt);
                }
            }
            finally
            {
                //string countQuery = "SELECT COUNT(*) FROM  PATIENT ";
                //OracleCommand cmdCnt = new OracleCommand(countQuery, conn);
                //count = Math.Ceiling(double.Parse(cmdCnt.ExecuteScalar().ToString()) / RowsPerPage);
                conn.Close();
            }
            return ptList;
        }

        /// <summary>
        /// Gets patients from database based on searching parameters intered by the user
        /// </summary>
        /// <param name="page">page number</param>
        /// <param name="count">total number of pages</param>
        /// <param name="RowsPerPage">number of patients per page</param>
        /// <param name="firstname">patient's first name as a search parameter</param>
        /// <param name="middlename">patient's middle name as a search parameter</param>
        /// <param name="lastname">patient's last name as a search parameter</param>
        /// <param name="mothername">patient's mother name as a search parameter</param>
        /// <param name="givenid">patient's givenid name as a search parameter</param>
        /// <param name="gender">patient's gender name as a search parameter</param>
        /// <returns>list of patients matching the search parameters</returns>
        public static List<Patient> getSearchData(double page, out double count, double RowsPerPage, string firstname, string middlename, string lastname, string mothername, string givenid, int? gender,int? recp)
        {
            List<Patient> ptList = new List<Patient>();

            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();

                //  OracleCommand cmd = new OracleCommand("SELECT * FROM PATIENT ", conn);

                string idx1 = (RowsPerPage * (page - 1) + 1).ToString();
                string idx2 = (RowsPerPage * page).ToString();
                string whereStr = "SELECT * FROM  PATIENT  ";

                //search param GIVENID, FIRSTNAME, MIDDLENAME, LASTNAME, GENDRE, MOTHERNAME


                //  whereStr += " WHERE FIRSTNAME like '%" + firstname + "%' And MIDDLENAME like '%" + middlename + "%' And LASTNAME like '%" + lastname + "%' And MOTHERNAME like '%" + mothername + "%' And GIVENID like '%" + givenid + "%' ";
                whereStr += " WHERE upper(FIRSTNAME) like upper('%" + firstname + "%') ";
                if (middlename != null && middlename != "")
                    whereStr += " And upper(MIDDLENAME)  like upper('%" + middlename + "%') ";
                whereStr += " And upper(LASTNAME) like upper('%" + lastname + "%') ";
                if (mothername != null && mothername != "")
                    whereStr += " And upper(MOTHERNAME) like upper('%" + mothername + "%') "; ;
                whereStr += " And upper(GIVENID) like upper('%" + givenid + "%') ";
                if (gender != null)
                    whereStr += " And GENDRE = " + gender;


                if(recp !=null)
                    whereStr += " And INSERTUSER = " + recp;



                whereStr += " order by NUM desc ";

                string sql = "SELECT * FROM (SELECT T.*, ROWNUM RID FROM (" + whereStr + ") T)WHERE RID BETWEEN " + idx1 + " AND " + idx2;

                string countQuery = "SELECT COUNT(*) FROM (" + whereStr + ")";
                OracleCommand cmdCnt = new OracleCommand(countQuery, conn);
                count = Math.Ceiling(double.Parse(cmdCnt.ExecuteScalar().ToString()) / RowsPerPage);


                OracleCommand cmd = new OracleCommand(sql, conn);
                OracleDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {

                    Patient pt = new Patient();
                    #region Data
                    if (!dr.IsDBNull(0))
                        pt.num = Int32.Parse(dr.GetValue(0).ToString());
                    if (!dr.IsDBNull(1))
                        pt.id = Int32.Parse(dr.GetValue(1).ToString());
                    if (!dr.IsDBNull(2))
                        pt.givenid = dr.GetString(2);
                    if (!dr.IsDBNull(3))
                        pt.firstname = dr.GetString(3);
                    if (!dr.IsDBNull(4))
                        pt.middlename = dr.GetString(4);
                    if (!dr.IsDBNull(5))
                        pt.lastname = dr.GetString(5);
                    if (!dr.IsDBNull(6))
                        pt.gendre = Int32.Parse(dr.GetValue(6).ToString());
                    if (!dr.IsDBNull(7))
                        pt.mothername = dr.GetString(7);
                    if (!dr.IsDBNull(8))
                        pt.birthdate = dr.GetDateTime(8);
                    if (!dr.IsDBNull(9))
                        pt.age = Int32.Parse(dr.GetValue(9).ToString());
                    if (!dr.IsDBNull(10))
                        pt.mobilephone = dr.GetString(10);
                    if (!dr.IsDBNull(11))
                        pt.landphone = dr.GetString(11);
                    if (!dr.IsDBNull(12))
                        pt.currentaddress = dr.GetString(12);
                    if (!dr.IsDBNull(13))
                        pt.residentaddress = dr.GetString(13);
                    if (!dr.IsDBNull(14))
                        pt.workphone = dr.GetString(14);
                    if (!dr.IsDBNull(15))
                        pt.workaddress = dr.GetString(15);
                    if (!dr.IsDBNull(16))
                        pt.nearestperson = dr.GetString(16);
                    if (!dr.IsDBNull(17))
                        pt.nearestpersonphone = dr.GetString(17);
                    if (!dr.IsDBNull(18))
                        pt.birthplace = dr.GetString(18);
                    if (!dr.IsDBNull(19))
                        pt.nationalidnumber = dr.GetString(19);
                    if (!dr.IsDBNull(20))
                        pt.nationality = dr.GetString(20);
                    if (!dr.IsDBNull(21))
                        pt.worktype = dr.GetString(21);
                    if (!dr.IsDBNull(22))
                        pt.notes = dr.GetString(22);
                    if (!dr.IsDBNull(23))
                        pt.martialstatus = Int32.Parse(dr.GetValue(23).ToString());
                    if (!dr.IsDBNull(24))
                        pt.translatedFname = dr.GetString(24);
                    if (!dr.IsDBNull(25))
                        pt.translatedLname = dr.GetString(25);
                    if (!dr.IsDBNull(26))
                        pt.translatedFathername = dr.GetString(26);
                    if (!dr.IsDBNull(27))
                        pt.translatedMothername = dr.GetString(27);
                    if (!dr.IsDBNull(28))
                        pt.insertdate = dr.GetDateTime(28);
                    //audit
                    if (!dr.IsDBNull(29))
                        pt.insertUser = Int32.Parse(dr.GetValue(29).ToString());
                    if (!dr.IsDBNull(30))
                        pt.updateUser = Int32.Parse(dr.GetValue(30).ToString());
                    if (!dr.IsDBNull(31))
                        pt.updateDate = dr.GetDateTime(31);
                    if (!dr.IsDBNull(32))
                        pt.updateDeleteReason = dr.GetString(32);
                    if (!dr.IsDBNull(33))
                        pt.regStatus = Int32.Parse(dr.GetValue(33).ToString());
                    //end audit

                    #endregion
                    ptList.Add(pt);
                }
            }
            finally
            {
                //string countQuery = "SELECT COUNT(*) FROM  PATIENT ";
                //OracleCommand cmdCnt = new OracleCommand(countQuery, conn);
                //count = Math.Ceiling(double.Parse(cmdCnt.ExecuteScalar().ToString()) / RowsPerPage);
                conn.Close();
            }
            return ptList;
        }

        /// <summary>
        /// Gets details of a defined patient from database by his ID
        /// </summary>
        /// <param name="num">patient's ID</param>
        /// <returns>patient object contains the details of the wanted patient</returns>
        public static Patient Select(int num)
        {
            Patient pt = new Patient();
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("SELECT * FROM PATIENT WHERE NUM= " + num, conn);
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    #region Data
                    if (!dr.IsDBNull(0))
                        pt.num = Int32.Parse(dr.GetValue(0).ToString());
                    if (!dr.IsDBNull(1))
                        pt.id = Int32.Parse(dr.GetValue(1).ToString());
                    if (!dr.IsDBNull(2))
                        pt.givenid = dr.GetString(2);
                    if (!dr.IsDBNull(3))
                        pt.firstname = dr.GetString(3);
                    if (!dr.IsDBNull(4))
                        pt.middlename = dr.GetString(4);
                    if (!dr.IsDBNull(5))
                        pt.lastname = dr.GetString(5);
                    if (!dr.IsDBNull(6))
                        pt.gendre = Int32.Parse(dr.GetValue(6).ToString());
                    if (!dr.IsDBNull(7))
                        pt.mothername = dr.GetString(7);
                    if (!dr.IsDBNull(8))
                        pt.birthdate = dr.GetDateTime(8);
                    if (!dr.IsDBNull(9))
                        pt.age = Int32.Parse(dr.GetValue(9).ToString());
                    if (!dr.IsDBNull(10))
                        pt.mobilephone = dr.GetString(10);
                    if (!dr.IsDBNull(11))
                        pt.landphone = dr.GetString(11);
                    if (!dr.IsDBNull(12))
                        pt.currentaddress = dr.GetString(12);
                    if (!dr.IsDBNull(13))
                        pt.residentaddress = dr.GetString(13);
                    if (!dr.IsDBNull(14))
                        pt.workphone = dr.GetString(14);
                    if (!dr.IsDBNull(15))
                        pt.workaddress = dr.GetString(15);
                    if (!dr.IsDBNull(16))
                        pt.nearestperson = dr.GetString(16);
                    if (!dr.IsDBNull(17))
                        pt.nearestpersonphone = dr.GetString(17);
                    if (!dr.IsDBNull(18))
                        pt.birthplace = dr.GetString(18);
                    if (!dr.IsDBNull(19))
                        pt.nationalidnumber = dr.GetString(19);
                    if (!dr.IsDBNull(20))
                        pt.nationality = dr.GetString(20);
                    if (!dr.IsDBNull(21))
                        pt.worktype = dr.GetString(21);
                    if (!dr.IsDBNull(22))
                        pt.notes = dr.GetString(22);
                    if (!dr.IsDBNull(23))
                        pt.martialstatus = Int32.Parse(dr.GetValue(23).ToString());
                    if (!dr.IsDBNull(24))
                        pt.translatedFname = dr.GetString(24);
                    if (!dr.IsDBNull(25))
                        pt.translatedLname = dr.GetString(25);
                    if (!dr.IsDBNull(26))
                        pt.translatedFathername = dr.GetString(26);
                    if (!dr.IsDBNull(27))
                        pt.translatedMothername = dr.GetString(27);
                    if (!dr.IsDBNull(28))
                        pt.insertdate = dr.GetDateTime(28);
                    //audit
                    if (!dr.IsDBNull(29))
                        pt.insertUser = Int32.Parse(dr.GetValue(29).ToString());
                    if (!dr.IsDBNull(30))
                        pt.updateUser = Int32.Parse(dr.GetValue(30).ToString());
                    if (!dr.IsDBNull(31))
                        pt.updateDate = dr.GetDateTime(31);
                    if (!dr.IsDBNull(32))
                        pt.updateDeleteReason = dr.GetString(32);
                    if (!dr.IsDBNull(33))
                        pt.regStatus = Int32.Parse(dr.GetValue(33).ToString());

                    //end audit
                    if (!dr.IsDBNull(34))
                        pt.acceptanceType = Int32.Parse(dr.GetValue(34).ToString());
                    #endregion
                }
            }
            catch
            {
            }
            finally
            {
                conn.Close();
            }
            return pt;
        }

        /// <summary>
        /// Gets details of a defined patient from database by his GivenID
        /// </summary>
        /// <param name="num">patient's GivenID</param>
        /// <returns>patient object contains the details of the wanted patient</returns>
        public static Patient SelectByGid(string num)
        {
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("SELECT * FROM PATIENT WHERE GIVENID= '" + num + "'", conn);
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.Read())

                {
                    Patient pt = new Patient();
                    #region Data
                    if (!dr.IsDBNull(0))
                        pt.num = Int32.Parse(dr.GetValue(0).ToString());
                    if (!dr.IsDBNull(1))
                        pt.id = Int32.Parse(dr.GetValue(1).ToString());
                    if (!dr.IsDBNull(2))
                        pt.givenid = dr.GetString(2);
                    if (!dr.IsDBNull(3))
                        pt.firstname = dr.GetString(3);
                    if (!dr.IsDBNull(4))
                        pt.middlename = dr.GetString(4);
                    if (!dr.IsDBNull(5))
                        pt.lastname = dr.GetString(5);
                    if (!dr.IsDBNull(6))
                        pt.gendre = Int32.Parse(dr.GetValue(6).ToString());
                    if (!dr.IsDBNull(7))
                        pt.mothername = dr.GetString(7);
                    if (!dr.IsDBNull(8))
                        pt.birthdate = dr.GetDateTime(8);
                    if (!dr.IsDBNull(9))
                        pt.age = Int32.Parse(dr.GetValue(9).ToString());
                    if (!dr.IsDBNull(10))
                        pt.mobilephone = dr.GetString(10);
                    if (!dr.IsDBNull(11))
                        pt.landphone = dr.GetString(11);
                    if (!dr.IsDBNull(12))
                        pt.currentaddress = dr.GetString(12);
                    if (!dr.IsDBNull(13))
                        pt.residentaddress = dr.GetString(13);
                    if (!dr.IsDBNull(14))
                        pt.workphone = dr.GetString(14);
                    if (!dr.IsDBNull(15))
                        pt.workaddress = dr.GetString(15);
                    if (!dr.IsDBNull(16))
                        pt.nearestperson = dr.GetString(16);
                    if (!dr.IsDBNull(17))
                        pt.nearestpersonphone = dr.GetString(17);
                    if (!dr.IsDBNull(18))
                        pt.birthplace = dr.GetString(18);
                    if (!dr.IsDBNull(19))
                        pt.nationalidnumber = dr.GetString(19);
                    if (!dr.IsDBNull(20))
                        pt.nationality = dr.GetString(20);
                    if (!dr.IsDBNull(21))
                        pt.worktype = dr.GetString(21);
                    if (!dr.IsDBNull(22))
                        pt.notes = dr.GetString(22);
                    if (!dr.IsDBNull(23))
                        pt.martialstatus = Int32.Parse(dr.GetValue(23).ToString());
                    if (!dr.IsDBNull(24))
                        pt.translatedFname = dr.GetString(24);
                    if (!dr.IsDBNull(25))
                        pt.translatedLname = dr.GetString(25);
                    if (!dr.IsDBNull(26))
                        pt.translatedFathername = dr.GetString(26);
                    if (!dr.IsDBNull(27))
                        pt.translatedMothername = dr.GetString(27);
                    if (!dr.IsDBNull(28))
                        pt.insertdate = dr.GetDateTime(28);
                    //audit
                    if (!dr.IsDBNull(29))
                        pt.insertUser = Int32.Parse(dr.GetValue(29).ToString());
                    if (!dr.IsDBNull(30))
                        pt.updateUser = Int32.Parse(dr.GetValue(30).ToString());
                    if (!dr.IsDBNull(31))
                        pt.updateDate = dr.GetDateTime(31);
                    if (!dr.IsDBNull(32))
                        pt.updateDeleteReason = dr.GetString(32);
                    if (!dr.IsDBNull(33))
                        pt.regStatus = Int32.Parse(dr.GetValue(33).ToString());
                    //end audit
                    conn.Close();

                    return pt;
                    #endregion
                }

            }
            catch
            {

            }
            finally
            {
                conn.Close();
            }
            return null;
        }

        /// <summary>
        /// Inserts a new patient information into database
        /// </summary>
        /// <param name="pt">patient object containing the patient's details</param>
        /// <returns>exception message if there is any, empty string if none</returns>
        public static string Insert(Patient pt)
        {
            string res = "";
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {

                pt.insertdate = DateTime.Now;

                conn.Open();
                string qr = "INSERT INTO PATIENT ( " +
                    " NUM, ID, GIVENID, FIRSTNAME, MIDDLENAME, LASTNAME, GENDRE, MOTHERNAME, BIRTHDATE, AGE, MOBILEPHONE, LANDPHONE, " +
                    " CURRENTADDRESS, RESIDENTADDRESS, WORKPHONE, WORKADDRESS, NEARESTPERSON, NEARESTPERSONPHONE, BIRTHPLACE, " +
                    " NATIONALIDNUMBER, NATIONALITY, WORKTYPE, NOTES, MARTIALSTATUS, TRANSLATEDFNAME, TRANSLATEDLNAME, TRANSLATEDFATHERNAME, TRANSLATEDMOTHERNAME, INSERTDATE, INSERTUSER, ACCEPTANCETYPE ) VALUES (" +
                     " :NUM, :ID, :GIVENID, :FIRSTNAME, :MIDDLENAME, :LASTNAME, :GENDRE, :MOTHERNAME, :BIRTHDATE, :AGE, :MOBILEPHONE, :LANDPHONE, " +
                    " :CURRENTADDRESS, :RESIDENTADDRESS, :WORKPHONE, :WORKADDRESS, :NEARESTPERSON, :NEARESTPERSONPHONE, :BIRTHPLACE, " +
                    " :NATIONALIDNUMBER, :NATIONALITY, :WORKTYPE, :NOTES, :MARTIALSTATUS, :TRANSLATEDFNAME, :TRANSLATEDLNAME, :TRANSLATEDFATHERNAME, :TRANSLATEDMOTHERNAME, :INSERTDATE, :INSERTUSER, :ACCEPTANCETYPE )";
                OracleCommand cmd = new OracleCommand(qr, conn);
                OracleParameter[] param ={
                                            #region parameter
                                            new OracleParameter("NUM", pt.num),
                                            new OracleParameter("ID", pt.id),
                                            new OracleParameter("GIVENID", pt.givenid),
                                            new OracleParameter("FIRSTNAME", pt.firstname),
                                            new OracleParameter("MIDDLENAME", pt.middlename),
                                            new OracleParameter("LASTNAME", pt.lastname),
                                            new OracleParameter("GENDRE", pt.gendre),
                                            new OracleParameter("MOTHERNAME", pt.mothername),
                                            new OracleParameter("BIRTHDATE", pt.birthdate),
                                            new OracleParameter("AGE", pt.age),
                                            new OracleParameter("MOBILEPHONE", pt.mobilephone),
                                            new OracleParameter("LANDPHONE", pt.landphone),
                                            new OracleParameter("CURRENTADDRESS", pt.currentaddress),
                                            new OracleParameter("RESIDENTADDRESS", pt.residentaddress),
                                            new OracleParameter("WORKPHONE", pt.workphone),
                                            new OracleParameter("WORKADDRESS", pt.workaddress),
                                            new OracleParameter("NEARESTPERSON", pt.nearestperson),
                                            new OracleParameter("NEARESTPERSONPHONE", pt.nearestpersonphone),
                                            new OracleParameter("BIRTHPLACE", pt.birthplace),
                                            new OracleParameter("NATIONALIDNUMBER", pt.nationalidnumber),
                                            new OracleParameter("NATIONALITY", pt.nationality),
                                            new OracleParameter("WORKTYPE", pt.worktype),
                                            new OracleParameter("NOTES", pt.notes),
                                            new OracleParameter("MARTIALSTATUS", pt.martialstatus),
                                            new OracleParameter("TRANSLATEDFNAME", pt.translatedFname),
                                            new OracleParameter("TRANSLATEDLNAME", pt.translatedLname),
                                            new OracleParameter("TRANSLATEDFATHERNAME", pt.translatedFathername),
                                            new OracleParameter("TRANSLATEDMOTHERNAME", pt.translatedMothername),

                                            new OracleParameter("INSERTDATE", pt.insertdate),
                                            new OracleParameter("INSERTUSER", pt.insertUser),
                                            new OracleParameter("ACCEPTANCETYPE", pt.acceptanceType),

                                         
                                            #endregion parameter
                                           };
                for (int j = 0; j < param.Length; j++)
                    cmd.Parameters.Add(param[j]);

                //cmd.Parameters.Add("INSERTDATE", OracleDbType.Date, pt.insertdate, ParameterDirection.Input);
                //cmd.Parameters.Add("INSERTUSER", OracleDbType.Int32, pt.insertUser, ParameterDirection.Input);

                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
              //  res = e.Message;
                   res = "حدث خطأ في الإدخال";
            }
            finally
            {
                conn.Close();
            }
            return res;
        }

        /// <summary>
        /// Deletes a defined patient's information from database and inserts the deleted patient details into patient
        /// auditing table
        /// </summary>
        /// <param name="pt">patient object contains the patient information</param>
        /// <returns>exception message if there is any, empty string if none</returns>
        public static string delete(Patient pt)
        {
            string res = "";
            int count=0;
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                int oldNum = pt.num;
                conn.Open();
                string qr = "SELECT COUNT(NUM) FROM ORDERS WHERE PATIENTID = " + pt.num;
                OracleCommand cmd = new OracleCommand(qr, conn);
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    if (!dr.IsDBNull(0))
                        count = Int32.Parse(dr.GetValue(0).ToString());
                }

                if (count == 0)
                {
                    
                    int sqNum = OracleRIS.GetOracleSequenceValue("OLDPATIENT_SEQ");
                    pt.num = sqNum;
                    InsertOld(pt);
                    //end
                    qr = "DELETE PATIENT WHERE NUM = :NUM ";
                    OracleParameter param = new OracleParameter("NUM", oldNum);
                    cmd = new OracleCommand(qr, conn);
                    cmd.Parameters.Add(param);
                    cmd.ExecuteNonQuery();
                }
                else
                    res = RIS.Resources.Res.CantDeletePatient;
                
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
        /// Edits patient information in database and inserts the old patient information into patient auditing table
        /// </summary>
        /// <param name="pt">patient object contains the patient information<</param>
        /// <returns>exception message if there is any, empty string if none</returns>
        public static string Edit(Patient pt)
        {

            string res = "";
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                string qr = "  Update PATIENT Set " +
                    "ID = :ID, " +
                    " GIVENID = :GIVENID, " +
                    " FIRSTNAME = :FIRSTNAME, " +
                    " MIDDLENAME = :MIDDLENAME, " +
                    " LASTNAME = :LASTNAME, " +
                    " GENDRE = :GENDRE, " +
                    " MOTHERNAME = :MOTHERNAME, " +
                    " BIRTHDATE = :BIRTHDATE, " +
                    "AGE = :AGE, " +
                    "MOBILEPHONE = :MOBILEPHONE, " +
                    "LANDPHONE = :LANDPHONE, " +
                    "CURRENTADDRESS = :CURRENTADDRESS, " +
                    "RESIDENTADDRESS = :RESIDENTADDRESS, " +
                    "WORKPHONE = :WORKPHONE, " +
                    "WORKADDRESS = :WORKADDRESS, " +
                    "NEARESTPERSON = :NEARESTPERSON, " +
                    "NEARESTPERSONPHONE = :NEARESTPERSONPHONE, " +
                    "BIRTHPLACE = :BIRTHPLACE, " +
                    "NATIONALIDNUMBER =:NATIONALIDNUMBER, " +
                    "NATIONALITY = :NATIONALITY, " +
                    "WORKTYPE = :WORKTYPE, " +
                    "NOTES = :NOTES, " +
                    "MARTIALSTATUS = :MARTIALSTATUS, " +
                    "TRANSLATEDFNAME = :TRANSLATEDFNAME, " +
                    "TRANSLATEDLNAME = :TRANSLATEDLNAME, " +
                    "TRANSLATEDFATHERNAME = :TRANSLATEDFATHERNAME, " +
                    "TRANSLATEDMOTHERNAME = :TRANSLATEDMOTHERNAME, " +
                    "ACCEPTANCETYPE = :ACCEPTANCETYPE " +
                            "  where NUM = :NUM ";
                OracleParameter[] param =  {
                                            #region parameter
                                            new OracleParameter("ID", pt.id),
                                            new OracleParameter("GIVENID", pt.givenid),
                                            new OracleParameter("FIRSTNAME", pt.firstname),
                                            new OracleParameter("MIDDLENAME", pt.middlename),
                                            new OracleParameter("LASTNAME", pt.lastname),
                                            new OracleParameter("GENDRE", pt.gendre),
                                            new OracleParameter("MOTHERNAME", pt.mothername),
                                            new OracleParameter("BIRTHDATE", pt.birthdate),
                                            new OracleParameter("AGE", pt.age),
                                            new OracleParameter("MOBILEPHONE", pt.mobilephone),
                                            new OracleParameter("LANDPHONE", pt.landphone),
                                            new OracleParameter("CURRENTADDRESS", pt.currentaddress),
                                            new OracleParameter("RESIDENTADDRESS", pt.residentaddress),
                                            new OracleParameter("WORKPHONE", pt.workphone),
                                            new OracleParameter("WORKADDRESS", pt.workaddress),
                                            new OracleParameter("NEARESTPERSON", pt.nearestperson),
                                            new OracleParameter("NEARESTPERSONPHONE", pt.nearestpersonphone),
                                            new OracleParameter("BIRTHPLACE", pt.birthplace),
                                            new OracleParameter("NATIONALIDNUMBER", pt.nationalidnumber),
                                            new OracleParameter("NATIONALITY", pt.nationality),
                                            new OracleParameter("WORKTYPE", pt.worktype),
                                            new OracleParameter("NOTES", pt.notes),
                                            new OracleParameter("MARTIALSTATUS", pt.martialstatus),
                                            new OracleParameter("TRANSLATEDFNAME", pt.translatedFname),
                                            new OracleParameter("TRANSLATEDLNAME", pt.translatedLname),
                                            new OracleParameter("TRANSLATEDFATHERNAME", pt.translatedFathername),
                                            new OracleParameter("TRANSLATEDMOTHERNAME", pt.translatedMothername),
                                            new OracleParameter("ACCEPTANCETYPE", pt.acceptanceType),
                                            new OracleParameter("NUM", pt.num),
                                            #endregion parameter
                                           };
                OracleCommand cmd = new OracleCommand(qr, conn);
                for (int j = 0; j < param.Length; j++)
                {
                    cmd.Parameters.Add(param[j]);
                }
                int x = cmd.ExecuteNonQuery();
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
        /// Gets patient's gender selectlist
        /// </summary>
        /// <param name="defaultValue">setting the default value of selectlist</param>
        /// <returns>selectlist of patient's gender</returns>
        public static SelectList GetGenderList(string defaultValue)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "", Value = "" });
            items.Add(new SelectListItem { Text = RIS.Resources.Res.female, Value = "0" });
            items.Add(new SelectListItem { Text = RIS.Resources.Res.male, Value = "1" });
            return new SelectList(items, "Value", "Text", string.IsNullOrEmpty(defaultValue) ? "" : defaultValue);
        }

        /// <summary>
        /// Gets patient's Registering status selectlist
        /// </summary>
        /// <param name="defaultValue">setting the default value of selectlist</param>
        /// <returns>selectlist of patient's Registering status</returns>
        public static SelectList GetRegList(string defaultValue)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "", Value = "" });

            items.Add(new SelectListItem { Text = RIS.Resources.Res.Delete, Value = ((int)(RegStatus.delete)).ToString() });
            items.Add(new SelectListItem { Text = RIS.Resources.Res.edit, Value = ((int)(RegStatus.update)).ToString() });
            return new SelectList(items, "Value", "Text", string.IsNullOrEmpty(defaultValue) ? "" : defaultValue);
        }

        /// <summary>
        /// Gets a defined patient by his national number
        /// </summary>
        /// <param name="nNum">patient's national number</param>
        /// <returns>patient object contains the wanted patient information</returns>
        public static Patient GetByNatioanlNum(string nNum)
        {
            Patient pt = new Patient();
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("SELECT * FROM PATIENT WHERE NATIONALIDNUMBER= '" + nNum + "' ", conn);
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    #region Data
                    if (!dr.IsDBNull(0))
                        pt.num = Int32.Parse(dr.GetValue(0).ToString());
                    if (!dr.IsDBNull(1))
                        pt.id = Int32.Parse(dr.GetValue(1).ToString());
                    if (!dr.IsDBNull(2))
                        pt.givenid = dr.GetString(2);
                    if (!dr.IsDBNull(3))
                        pt.firstname = dr.GetString(3);
                    if (!dr.IsDBNull(4))
                        pt.middlename = dr.GetString(4);
                    if (!dr.IsDBNull(5))
                        pt.lastname = dr.GetString(5);
                    if (!dr.IsDBNull(6))
                        pt.gendre = Int32.Parse(dr.GetValue(6).ToString());
                    if (!dr.IsDBNull(7))
                        pt.mothername = dr.GetString(7);
                    if (!dr.IsDBNull(8))
                        pt.birthdate = dr.GetDateTime(8);
                    if (!dr.IsDBNull(9))
                        pt.age = Int32.Parse(dr.GetValue(9).ToString());
                    if (!dr.IsDBNull(10))
                        pt.mobilephone = dr.GetString(10);
                    if (!dr.IsDBNull(11))
                        pt.landphone = dr.GetString(11);
                    if (!dr.IsDBNull(12))
                        pt.currentaddress = dr.GetString(12);
                    if (!dr.IsDBNull(13))
                        pt.residentaddress = dr.GetString(13);
                    if (!dr.IsDBNull(14))
                        pt.workphone = dr.GetString(14);
                    if (!dr.IsDBNull(15))
                        pt.workaddress = dr.GetString(15);
                    if (!dr.IsDBNull(16))
                        pt.nearestperson = dr.GetString(16);
                    if (!dr.IsDBNull(17))
                        pt.nearestpersonphone = dr.GetString(17);
                    if (!dr.IsDBNull(18))
                        pt.birthplace = dr.GetString(18);
                    if (!dr.IsDBNull(19))
                        pt.nationalidnumber = dr.GetString(19);
                    if (!dr.IsDBNull(20))
                        pt.nationality = dr.GetString(20);
                    if (!dr.IsDBNull(21))
                        pt.worktype = dr.GetString(21);
                    if (!dr.IsDBNull(22))
                        pt.notes = dr.GetString(22);
                    if (!dr.IsDBNull(23))
                        pt.martialstatus = Int32.Parse(dr.GetValue(23).ToString());
                    if (!dr.IsDBNull(24))
                        pt.translatedFname = dr.GetString(24);
                    if (!dr.IsDBNull(25))
                        pt.translatedLname = dr.GetString(25);
                    if (!dr.IsDBNull(26))
                        pt.translatedFathername = dr.GetString(26);
                    if (!dr.IsDBNull(27))
                        pt.translatedMothername = dr.GetString(27);
                    if (!dr.IsDBNull(28))
                        pt.insertdate = dr.GetDateTime(28);
                    #endregion
                }
            }
            catch
            {
            }
            finally
            {
                conn.Close();
            }
            return pt;
        }


        /// <summary>
        /// Gets similar patients who has a similar values to that inserted by the user based on the listed parameters
        /// </summary>
        /// <param name="firstname">similar first name</param>
        /// <param name="lastname">similar last name</param>
        /// <param name="middlename">similar middle name</param>
        /// <param name="mothername">similar mother name</param>
        /// <returns>list of similar patients</returns>
        public static List<Patient> getSemilarData(string firstname, string lastname, string middlename, string mothername)
        {
            List<Patient> ptList = new List<Patient>();

            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();

                //  OracleCommand cmd = new OracleCommand("SELECT * FROM PATIENT ", conn);

                //  string idx1 = (RowsPerPage * (page - 1) + 1).ToString();
                //  string idx2 = (RowsPerPage * page).ToString();
                string whereStr = "SELECT * FROM  PATIENT  ";

                //search param GIVENID, FIRSTNAME, MIDDLENAME, LASTNAME, GENDRE, MOTHERNAME


                //  whereStr += " WHERE FIRSTNAME like '%" + firstname + "%' And MIDDLENAME like '%" + middlename + "%' And LASTNAME like '%" + lastname + "%' And MOTHERNAME like '%" + mothername + "%' And GIVENID like '%" + givenid + "%' ";
                whereStr += " WHERE FIRSTNAME like '%" + firstname + "%' ";
                if (middlename != null && middlename != "")
                    whereStr += " And MIDDLENAME like '%" + middlename + "%' ";
                whereStr += " And LASTNAME like '%" + lastname + "%' ";
                if (mothername != null && mothername != "")
                    whereStr += " And MOTHERNAME like '%" + mothername + "%' "; ;
                //  whereStr += " And GIVENID like '%" + givenid + "%' ";
                //   if (gender != null)
                //      whereStr += " And GENDRE = " + gender;


                whereStr += " order by NUM desc ";

                //   string sql = "SELECT * FROM (SELECT T.*, ROWNUM RID FROM (" + whereStr + ") T)WHERE RID BETWEEN " + idx1 + " AND " + idx2;

                //   string countQuery = "SELECT COUNT(*) FROM (" + whereStr + ")";
                //   OracleCommand cmdCnt = new OracleCommand(countQuery, conn);
                //   count = Math.Ceiling(double.Parse(cmdCnt.ExecuteScalar().ToString()) / RowsPerPage);


                OracleCommand cmd = new OracleCommand(whereStr, conn);
                OracleDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {

                    Patient pt = new Patient();
                    #region Data
                    if (!dr.IsDBNull(0))
                        pt.num = Int32.Parse(dr.GetValue(0).ToString());
                    if (!dr.IsDBNull(1))
                        pt.id = Int32.Parse(dr.GetValue(1).ToString());
                    if (!dr.IsDBNull(2))
                        pt.givenid = dr.GetString(2);
                    if (!dr.IsDBNull(3))
                        pt.firstname = dr.GetString(3);
                    if (!dr.IsDBNull(4))
                        pt.middlename = dr.GetString(4);
                    if (!dr.IsDBNull(5))
                        pt.lastname = dr.GetString(5);
                    if (!dr.IsDBNull(6))
                        pt.gendre = Int32.Parse(dr.GetValue(6).ToString());
                    if (!dr.IsDBNull(7))
                        pt.mothername = dr.GetString(7);
                    if (!dr.IsDBNull(8))
                        pt.birthdate = dr.GetDateTime(8);
                    if (!dr.IsDBNull(9))
                        pt.age = Int32.Parse(dr.GetValue(9).ToString());
                    if (!dr.IsDBNull(10))
                        pt.mobilephone = dr.GetString(10);
                    if (!dr.IsDBNull(11))
                        pt.landphone = dr.GetString(11);
                    if (!dr.IsDBNull(12))
                        pt.currentaddress = dr.GetString(12);
                    if (!dr.IsDBNull(13))
                        pt.residentaddress = dr.GetString(13);
                    if (!dr.IsDBNull(14))
                        pt.workphone = dr.GetString(14);
                    if (!dr.IsDBNull(15))
                        pt.workaddress = dr.GetString(15);
                    if (!dr.IsDBNull(16))
                        pt.nearestperson = dr.GetString(16);
                    if (!dr.IsDBNull(17))
                        pt.nearestpersonphone = dr.GetString(17);
                    if (!dr.IsDBNull(18))
                        pt.birthplace = dr.GetString(18);
                    if (!dr.IsDBNull(19))
                        pt.nationalidnumber = dr.GetString(19);
                    if (!dr.IsDBNull(20))
                        pt.nationality = dr.GetString(20);
                    if (!dr.IsDBNull(21))
                        pt.worktype = dr.GetString(21);
                    if (!dr.IsDBNull(22))
                        pt.notes = dr.GetString(22);
                    if (!dr.IsDBNull(23))
                        pt.martialstatus = Int32.Parse(dr.GetValue(23).ToString());
                    if (!dr.IsDBNull(24))
                        pt.translatedFname = dr.GetString(24);
                    if (!dr.IsDBNull(25))
                        pt.translatedLname = dr.GetString(25);
                    if (!dr.IsDBNull(26))
                        pt.translatedFathername = dr.GetString(26);
                    if (!dr.IsDBNull(27))
                        pt.translatedMothername = dr.GetString(27);
                    if (!dr.IsDBNull(28))
                        pt.insertdate = dr.GetDateTime(28);
                    #endregion
                    ptList.Add(pt);
                }
            }
            finally
            {
                //string countQuery = "SELECT COUNT(*) FROM  PATIENT ";
                //OracleCommand cmdCnt = new OracleCommand(countQuery, conn);
                //count = Math.Ceiling(double.Parse(cmdCnt.ExecuteScalar().ToString()) / RowsPerPage);
                conn.Close();
            }
            return ptList;
        }

        //MMMMMMMMMMMMMMMMMMMMMModar
        public static int getTaxByPatientAccType(int accType)
        {
            int taxValue = 0;

            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("SELECT DISCOUNT FROM ACCEPTANCETYPE WHERE NUM=" + accType, conn);
                OracleDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    if (!dr.IsDBNull(0))
                        taxValue = int.Parse(dr.GetValue(0).ToString());
                }
            }
            catch
            {
            }
            finally
            {
                conn.Close();
            }
            return taxValue;
        }

        public static string getAccTypeName(int accType)
        {
            string accTypeName = "";

            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("SELECT NAME FROM ACCEPTANCETYPE WHERE NUM=" + accType, conn);
                OracleDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    if (!dr.IsDBNull(0))
                        accTypeName = dr.GetValue(0).ToString();
                }
            }
            catch
            {
            }
            finally
            {
                conn.Close();
            }
            return accTypeName;
        }

        /// <summary>
        /// Inserts old patient details into audit patient table in database when a patient is edited or deleted
        /// </summary>
        /// <param name="pt">patient object contains old patient's details</param>
        /// <returns>exception message if there is any, empty string if none</returns>
        public static string InsertOld(Patient pt)
        {
            string res = "";
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                string qr = "INSERT INTO OLDPATIENT ( " +
                    " NUM, ID, GIVENID, FIRSTNAME, MIDDLENAME, LASTNAME, GENDRE, MOTHERNAME, BIRTHDATE, AGE, MOBILEPHONE, LANDPHONE, " +
                    " CURRENTADDRESS, RESIDENTADDRESS, WORKPHONE, WORKADDRESS, NEARESTPERSON, NEARESTPERSONPHONE, BIRTHPLACE, " +
                    " NATIONALIDNUMBER, NATIONALITY, WORKTYPE, NOTES, MARTIALSTATUS, TRANSLATEDFNAME, TRANSLATEDLNAME, TRANSLATEDFATHERNAME, TRANSLATEDMOTHERNAME, INSERTDATE, INSERTUSER"+
                    " ,UPDATEDUSER, UPDATEDATE, REASON, REGSTATUS ) VALUES (" +
                     " :NUM, :ID, :GIVENID, :FIRSTNAME, :MIDDLENAME, :LASTNAME, :GENDRE, :MOTHERNAME, :BIRTHDATE, :AGE, :MOBILEPHONE, :LANDPHONE, " +
                    " :CURRENTADDRESS, :RESIDENTADDRESS, :WORKPHONE, :WORKADDRESS, :NEARESTPERSON, :NEARESTPERSONPHONE, :BIRTHPLACE, " +
                    " :NATIONALIDNUMBER, :NATIONALITY, :WORKTYPE, :NOTES, :MARTIALSTATUS, :TRANSLATEDFNAME, :TRANSLATEDLNAME, :TRANSLATEDFATHERNAME, :TRANSLATEDMOTHERNAME, :INSERTDATE, :INSERTUSER"+
                    " ,:UPDATEDUSER, :UPDATEDATE, :REASON, :REGSTATUS )";
                

                OracleCommand cmd = new OracleCommand(qr, conn);
                OracleParameter[] param ={
                                            #region parameter
                                            new OracleParameter("NUM", pt.num),
                                            new OracleParameter("ID", pt.id),
                                            new OracleParameter("GIVENID", pt.givenid),
                                            new OracleParameter("FIRSTNAME", pt.firstname),
                                            new OracleParameter("MIDDLENAME", pt.middlename),
                                            new OracleParameter("LASTNAME", pt.lastname),
                                            new OracleParameter("GENDRE", pt.gendre),
                                            new OracleParameter("MOTHERNAME", pt.mothername),
                                            new OracleParameter("BIRTHDATE", pt.birthdate),
                                            new OracleParameter("AGE", pt.age),
                                            new OracleParameter("MOBILEPHONE", pt.mobilephone),
                                            new OracleParameter("LANDPHONE", pt.landphone),
                                            new OracleParameter("CURRENTADDRESS", pt.currentaddress),
                                            new OracleParameter("RESIDENTADDRESS", pt.residentaddress),
                                            new OracleParameter("WORKPHONE", pt.workphone),
                                            new OracleParameter("WORKADDRESS", pt.workaddress),
                                            new OracleParameter("NEARESTPERSON", pt.nearestperson),
                                            new OracleParameter("NEARESTPERSONPHONE", pt.nearestpersonphone),
                                            new OracleParameter("BIRTHPLACE", pt.birthplace),
                                            new OracleParameter("NATIONALIDNUMBER", pt.nationalidnumber),
                                            new OracleParameter("NATIONALITY", pt.nationality),
                                            new OracleParameter("WORKTYPE", pt.worktype),
                                            new OracleParameter("NOTES", pt.notes),
                                            new OracleParameter("MARTIALSTATUS", pt.martialstatus),
                                            new OracleParameter("TRANSLATEDFNAME", pt.translatedFname),
                                            new OracleParameter("TRANSLATEDLNAME", pt.translatedLname),
                                            new OracleParameter("TRANSLATEDFATHERNAME", pt.translatedFathername),
                                            new OracleParameter("TRANSLATEDMOTHERNAME", pt.translatedMothername),

                                            new OracleParameter("INSERTDATE", pt.insertdate),
                                            new OracleParameter("INSERTUSER", pt.insertUser),

                                            new OracleParameter("UPDATEDUSER", pt.updateUser),
                                            new OracleParameter("UPDATEDATE", pt.updateDate),
                                            new OracleParameter("REASON", pt.updateDeleteReason),
                                            new OracleParameter("REGSTATUS", pt.regStatus),

                                            
                                         
                                            #endregion parameter
                                           };
                for (int j = 0; j < param.Length; j++)
                    cmd.Parameters.Add(param[j]);

                //cmd.Parameters.Add("INSERTDATE", OracleDbType.Date, pt.insertdate, ParameterDirection.Input);
                //cmd.Parameters.Add("INSERTUSER", OracleDbType.Int32, pt.insertUser, ParameterDirection.Input);

                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                res = e.Message;
                //   res = "حدث خطأ في الإدخال";
            }
            finally
            {
                conn.Close();
            }
            return res;
        }

        /// <summary>
        /// Gets audit patient list based on search parameters entered by the user
        /// </summary>
        /// <param name="page">page number</param>
        /// <param name="count">total page number</param>
        /// <param name="RowsPerPage">number of patients per page</param>
        /// <param name="firstname">patient's first name</param>
        /// <param name="middlename">patient's middle name</param>
        /// <param name="lastname">patient's last name</param>
        /// <param name="mothername">patient's mother name</param>
        /// <param name="givenid">patient's GivenID</param>
        /// <param name="gender">patient's gender</param>
        /// <param name="regStat">patient's registery status</param>
        /// <param name="beginDate">the beginning audit date</param>
        /// <param name="endDate">the end audit date</param>
        /// <returns>list of patients contains the search results</returns>
        public static List<Patient> getAuditSearchData(double page, out double count, double RowsPerPage, string firstname, string middlename, string lastname, string mothername, string givenid, int? gender,int? regStat, string beginDate, string endDate)
        {
            List<Patient> ptList = new List<Patient>();

            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();

                //  OracleCommand cmd = new OracleCommand("SELECT * FROM PATIENT ", conn);

                string idx1 = (RowsPerPage * (page - 1) + 1).ToString();
                string idx2 = (RowsPerPage * page).ToString();
                string whereStr = "SELECT * FROM  OLDPATIENT  ";

                //search param GIVENID, FIRSTNAME, MIDDLENAME, LASTNAME, GENDRE, MOTHERNAME


                //  whereStr += " WHERE FIRSTNAME like '%" + firstname + "%' And MIDDLENAME like '%" + middlename + "%' And LASTNAME like '%" + lastname + "%' And MOTHERNAME like '%" + mothername + "%' And GIVENID like '%" + givenid + "%' ";
                whereStr += " WHERE FIRSTNAME like '%" + firstname + "%' ";
                if (middlename != null && middlename != "")
                    whereStr += " And MIDDLENAME like '%" + middlename + "%' ";
                whereStr += " And LASTNAME like '%" + lastname + "%' ";
                if (mothername != null && mothername != "")
                    whereStr += " And MOTHERNAME like '%" + mothername + "%' "; 
                whereStr += " And GIVENID like '%" + givenid + "%' ";
                if (gender != null)
                    whereStr += " And GENDRE = " + gender;

                //get by date
                //string pacsTime = ConnectionConfigs.getPacsTime();
                //string date1 = "";
                //string date2 = "";
                /*
                if (beginDate == "")
                    date1 = pacsTime;
                else
                    date1 = beginDate;

                if (endDate == "")
                    date2 = pacsTime;
                else
                    date2 = endDate;*/


                if (!String.IsNullOrEmpty(beginDate))
                    whereStr += " And UPDATEDATE >= to_date('" + beginDate + "', 'yyyymmddhh24:mi:ss') ";
                if (!String.IsNullOrEmpty(endDate))
                    whereStr += " AND UPDATEDATE <= to_date('" + endDate + "', 'yyyymmddhh24:mi:ss')  ";
                //
             //   whereStr += " order by to_date(STARTDATE, 'yyyymmddhh24:mi:ss') desc ";

               
                //end get by date




                if (regStat != null)
                    whereStr += " And REGSTATUS = " + regStat;


                whereStr += " order by NUM desc ";

                string sql = "SELECT * FROM (SELECT T.*, ROWNUM RID FROM (" + whereStr + ") T)WHERE RID BETWEEN " + idx1 + " AND " + idx2;

                string countQuery = "SELECT COUNT(*) FROM (" + whereStr + ")";
                OracleCommand cmdCnt = new OracleCommand(countQuery, conn);
                count = Math.Ceiling(double.Parse(cmdCnt.ExecuteScalar().ToString()) / RowsPerPage);


                OracleCommand cmd = new OracleCommand(sql, conn);
                OracleDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {

                    Patient pt = new Patient();
                    #region Data
                    if (!dr.IsDBNull(0))
                        pt.num = Int32.Parse(dr.GetValue(0).ToString());
                    if (!dr.IsDBNull(1))
                        pt.id = Int32.Parse(dr.GetValue(1).ToString());
                    if (!dr.IsDBNull(2))
                        pt.givenid = dr.GetString(2);
                    if (!dr.IsDBNull(3))
                        pt.firstname = dr.GetString(3);
                    if (!dr.IsDBNull(4))
                        pt.middlename = dr.GetString(4);
                    if (!dr.IsDBNull(5))
                        pt.lastname = dr.GetString(5);
                    if (!dr.IsDBNull(6))
                        pt.gendre = Int32.Parse(dr.GetValue(6).ToString());
                    if (!dr.IsDBNull(7))
                        pt.mothername = dr.GetString(7);
                    if (!dr.IsDBNull(8))
                        pt.birthdate = dr.GetDateTime(8);
                    if (!dr.IsDBNull(9))
                        pt.age = Int32.Parse(dr.GetValue(9).ToString());
                    if (!dr.IsDBNull(10))
                        pt.mobilephone = dr.GetString(10);
                    if (!dr.IsDBNull(11))
                        pt.landphone = dr.GetString(11);
                    if (!dr.IsDBNull(12))
                        pt.currentaddress = dr.GetString(12);
                    if (!dr.IsDBNull(13))
                        pt.residentaddress = dr.GetString(13);
                    if (!dr.IsDBNull(14))
                        pt.workphone = dr.GetString(14);
                    if (!dr.IsDBNull(15))
                        pt.workaddress = dr.GetString(15);
                    if (!dr.IsDBNull(16))
                        pt.nearestperson = dr.GetString(16);
                    if (!dr.IsDBNull(17))
                        pt.nearestpersonphone = dr.GetString(17);
                    if (!dr.IsDBNull(18))
                        pt.birthplace = dr.GetString(18);
                    if (!dr.IsDBNull(19))
                        pt.nationalidnumber = dr.GetString(19);
                    if (!dr.IsDBNull(20))
                        pt.nationality = dr.GetString(20);
                    if (!dr.IsDBNull(21))
                        pt.worktype = dr.GetString(21);
                    if (!dr.IsDBNull(22))
                        pt.notes = dr.GetString(22);
                    if (!dr.IsDBNull(23))
                        pt.martialstatus = Int32.Parse(dr.GetValue(23).ToString());
                    if (!dr.IsDBNull(24))
                        pt.translatedFname = dr.GetString(24);
                    if (!dr.IsDBNull(25))
                        pt.translatedLname = dr.GetString(25);
                    if (!dr.IsDBNull(26))
                        pt.translatedFathername = dr.GetString(26);
                    if (!dr.IsDBNull(27))
                        pt.translatedMothername = dr.GetString(27);
                    if (!dr.IsDBNull(28))
                        pt.insertdate = dr.GetDateTime(28);
                    //audit
                    if (!dr.IsDBNull(29))
                        pt.insertUser = Int32.Parse(dr.GetValue(29).ToString());
                    if (!dr.IsDBNull(30))
                        pt.updateUser = Int32.Parse(dr.GetValue(30).ToString());
                    if (!dr.IsDBNull(31))
                        pt.updateDate = dr.GetDateTime(31);
                    if (!dr.IsDBNull(32))
                        pt.updateDeleteReason = dr.GetString(32);
                    if (!dr.IsDBNull(33))
                        pt.regStatus = Int32.Parse(dr.GetValue(33).ToString());
                    //end audit

                    #endregion
                    ptList.Add(pt);
                }
            }
            finally
            {
                //string countQuery = "SELECT COUNT(*) FROM  PATIENT ";
                //OracleCommand cmdCnt = new OracleCommand(countQuery, conn);
                //count = Math.Ceiling(double.Parse(cmdCnt.ExecuteScalar().ToString()) / RowsPerPage);
                conn.Close();
            }
            return ptList;
        }

        /// <summary>
        /// Gets the old data of a defined patient
        /// </summary>
        /// <param name="num">patient's ID</param>
        /// <returns>patient object contains the old patient details</returns>
        public static Patient SelectOld(int num)
        {
            Patient pt = new Patient();
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("SELECT * FROM OLDPATIENT WHERE NUM= " + num, conn);
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    #region Data
                    if (!dr.IsDBNull(0))
                        pt.num = Int32.Parse(dr.GetValue(0).ToString());
                    if (!dr.IsDBNull(1))
                        pt.id = Int32.Parse(dr.GetValue(1).ToString());
                    if (!dr.IsDBNull(2))
                        pt.givenid = dr.GetString(2);
                    if (!dr.IsDBNull(3))
                        pt.firstname = dr.GetString(3);
                    if (!dr.IsDBNull(4))
                        pt.middlename = dr.GetString(4);
                    if (!dr.IsDBNull(5))
                        pt.lastname = dr.GetString(5);
                    if (!dr.IsDBNull(6))
                        pt.gendre = Int32.Parse(dr.GetValue(6).ToString());
                    if (!dr.IsDBNull(7))
                        pt.mothername = dr.GetString(7);
                    if (!dr.IsDBNull(8))
                        pt.birthdate = dr.GetDateTime(8);
                    if (!dr.IsDBNull(9))
                        pt.age = Int32.Parse(dr.GetValue(9).ToString());
                    if (!dr.IsDBNull(10))
                        pt.mobilephone = dr.GetString(10);
                    if (!dr.IsDBNull(11))
                        pt.landphone = dr.GetString(11);
                    if (!dr.IsDBNull(12))
                        pt.currentaddress = dr.GetString(12);
                    if (!dr.IsDBNull(13))
                        pt.residentaddress = dr.GetString(13);
                    if (!dr.IsDBNull(14))
                        pt.workphone = dr.GetString(14);
                    if (!dr.IsDBNull(15))
                        pt.workaddress = dr.GetString(15);
                    if (!dr.IsDBNull(16))
                        pt.nearestperson = dr.GetString(16);
                    if (!dr.IsDBNull(17))
                        pt.nearestpersonphone = dr.GetString(17);
                    if (!dr.IsDBNull(18))
                        pt.birthplace = dr.GetString(18);
                    if (!dr.IsDBNull(19))
                        pt.nationalidnumber = dr.GetString(19);
                    if (!dr.IsDBNull(20))
                        pt.nationality = dr.GetString(20);
                    if (!dr.IsDBNull(21))
                        pt.worktype = dr.GetString(21);
                    if (!dr.IsDBNull(22))
                        pt.notes = dr.GetString(22);
                    if (!dr.IsDBNull(23))
                        pt.martialstatus = Int32.Parse(dr.GetValue(23).ToString());
                    if (!dr.IsDBNull(24))
                        pt.translatedFname = dr.GetString(24);
                    if (!dr.IsDBNull(25))
                        pt.translatedLname = dr.GetString(25);
                    if (!dr.IsDBNull(26))
                        pt.translatedFathername = dr.GetString(26);
                    if (!dr.IsDBNull(27))
                        pt.translatedMothername = dr.GetString(27);
                    if (!dr.IsDBNull(28))
                        pt.insertdate = dr.GetDateTime(28);
                    //audit
                    if (!dr.IsDBNull(29))
                        pt.insertUser = Int32.Parse(dr.GetValue(29).ToString());
                    if (!dr.IsDBNull(30))
                        pt.updateUser = Int32.Parse(dr.GetValue(30).ToString());
                    if (!dr.IsDBNull(31))
                        pt.updateDate = dr.GetDateTime(31);
                    if (!dr.IsDBNull(32))
                        pt.updateDeleteReason = dr.GetString(32);
                    if (!dr.IsDBNull(33))
                        pt.regStatus = Int32.Parse(dr.GetValue(33).ToString());
                    //end audit
                    #endregion
                }
            }
            catch
            {
            }
            finally
            {
                conn.Close();
            }
            return pt;
        }


        #endregion
    }
}