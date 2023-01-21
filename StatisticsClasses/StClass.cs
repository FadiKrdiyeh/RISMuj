using Oracle.DataAccess.Client;
using RIS.Models;
using RISDB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RIS.StatisticsClasses
{
    public class StClass
    {
    }

    [Serializable]
    public class PatientStatisticModel
    {
        [Display(ResourceType = typeof(Resources.Res), Name = "PatientGendre")]
        public int sex { set; get; }

        [Display(ResourceType = typeof(Resources.Res), Name = "PatientAge")]

        public int age { set; get; }

        [Display(ResourceType = typeof(Resources.Res), Name = "sinceDate")]


        public DateTime sBirthDate { set; get; }

        [Display(ResourceType = typeof(Resources.Res), Name = "toDate")]

        public DateTime eBirthDate { set; get; }

        [Display(ResourceType = typeof(Resources.Res), Name = "deps")]

        public string[] departements { set; get; }

        [Display(ResourceType = typeof(Resources.Res), Name = "mods")]

        public string[] modalities { set; get; }

        [Display(ResourceType = typeof(Resources.Res), Name = "sinceDate")]

        public DateTime sInsertDate { set; get; }

        [Display(ResourceType = typeof(Resources.Res), Name = "toDate")]

        public DateTime eInsertDate { set; get; }



        public string GroupingItem { get; set; }

        // new for app stats
        [Display(ResourceType = typeof(Resources.Res), Name = "DoctorName")]

        public string[] doctors { set; get; }

        [Display(ResourceType = typeof(Resources.Res), Name = "ChooseClinic")]

        public string[] clinics { set; get; }
        public static List<Models.Patient> getPatientStatistics(PatientStatisticModel ps)
        {
            List<Models.Patient> res = new List<Models.Patient>();

            string querry = "SELECT * FROM PATIENT ";

            //string searchParameter = "WHERE NUM>-1";
            string searchParameter = querry + "JOIN ORDERS ON PATIENT.NUM= ORDERS.PATIENTID";
            /*
                        if (ps.sex==1)
                            searchParameter += " AND PATIENT.SEX= '" + ps.sex + "'";

                        if (ps.age != 0)
                            searchParameter += " AND PATIENT.AGE= '" + ps.age + "'";
                        if (ps.sBirthDate != null)
                            searchParameter += " AND PATIENT.BIRTHDATE >= Date '" + ps.sBirthDate.ToString("yyyy-MM-dd")+"'";
                        if (ps.eBirthDate != null)
                            searchParameter += " AND PATIENT.BIRTHDATE <= Date '" + ps.eBirthDate.ToString("yyyy-MM-dd") + "'";
                        if(ps.sInsertDate!=null)
                            searchParameter += " AND PATIENT.INSERTDATE >= Date '" + ps.eBirthDate.ToString("yyyy-MM-dd") + "'";
                        if (ps.eInsertDate != null)
                            searchParameter += " AND PATIENT.INSERTDATE <= Date '" + ps.eBirthDate.ToString("yyyy-MM-dd") + "'";

                        if (ps.departements != null)
                        {
                            searchParameter += " and (";

                            string[] deps = ps.departements.Split(',');
                            if (deps.Length==1)
                                searchParameter += " ORDERS.DEPTNAME='" + findDepID(deps[0]).ToString() + "' )";
                            else
                            {
                                for (int i = 0; i < deps.Length-1; i++)
                                {
                                    searchParameter += " ORDERS.DEPTNAME='" + findDepID(deps[i]).ToString() + "' or";
                                }

                                searchParameter += " ORDERS.DEPTNAME='" + findDepID(deps[deps.Length - 1]).ToString() + "' )";

                            }

                        }
                        if (ps.modalities!=null)
                        {
                            searchParameter += " and (";

                            string[] mods = ps.modalities.Split(',');
                            if (mods.Length == 1)
                                searchParameter += " ORDERS.DEPTNAME='" + findModID(mods[0]).ToString() + "' )";
                            else
                            {
                                for (int i = 0; i < mods.Length - 1; i++)
                                {
                                    searchParameter += " ORDERS.DEPTNAME='" + findModID(mods[i]).ToString() + "' or";
                                }

                                searchParameter += " ORDERS.DEPTNAME='" + findModID(mods[mods.Length - 1]).ToString() + "' )";

                            }
                        }
                        */
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            conn.Open();
            OracleCommand cmd = new OracleCommand(searchParameter, conn);
            OracleDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                Models.Patient p = new Models.Patient();

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
                    res.Add(pt);
                }

            }

            return res;

        }

        public static int findDepID(string depname)
        {
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            conn.Open();
            string qr = "select * from DEPARTMENT where NAME like '%" + depname + "%'";
            OracleCommand cmd = new OracleCommand(qr, conn);
            OracleDataReader dr = cmd.ExecuteReader();
            //Models.Departement d = new Models.Departement();
            while (dr.Read())
            {
                if (!dr.IsDBNull(0))
                    return int.Parse(dr.GetValue(0).ToString());
            }
            return -1;
        }

        public static int findID(string tableName, string name)
        {
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            conn.Open();
            string qr = "select * from " + tableName + " where NAME like '%" + name + "%'";
            OracleCommand cmd = new OracleCommand(qr, conn);
            OracleDataReader dr = cmd.ExecuteReader();
            //Models.Departement d = new Models.Departement();
            while (dr.Read())
            {
                if (!dr.IsDBNull(0))
                    return int.Parse(dr.GetValue(0).ToString());
            }
            return -1;
        }


        public static int findModID(string modname)
        {
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            conn.Open();
            string qr = "select * from MODALITY where NAME like '%" + modname + "%'";
            OracleCommand cmd = new OracleCommand(qr, conn);
            OracleDataReader dr = cmd.ExecuteReader();
            //Models.Departement d = new Models.Departement();
            while (dr.Read())
            {
                if (!dr.IsDBNull(0))
                    return int.Parse(dr.GetValue(0).ToString());
            }
            return -1;
        }

    }


    public class OrdersStatisticModel
    {


        [Display(ResourceType = typeof(Resources.Res), Name = "sinceDate")]


        public DateTime sStartDate { set; get; }

        [Display(ResourceType = typeof(Resources.Res), Name = "toDate")]

        public DateTime eStartDate { set; get; }

        [Display(ResourceType = typeof(Resources.Res), Name = "deps")]

        public string[] departements { set; get; }

        [Display(ResourceType = typeof(Resources.Res), Name = "mods")]

        public string[] modalities { set; get; }

        [Display(ResourceType = typeof(Resources.Res), Name = "procedureName")]
        public string procedures { set; get; }


        [Display(ResourceType = typeof(Resources.Res), Name = "radPayType")]
        public int radPayType { set; get; }

        //[Display(ResourceType = typeof(Resources.Res), Name = "orderType")]

        //public int[] type { set; get; }


        public string GroupingItem { get; set; }

        public static int findDepID(string depname)
        {
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            conn.Open();
            string qr = "select * from DEPARTMENT where NAME like '%" + depname + "%'";
            OracleCommand cmd = new OracleCommand(qr, conn);
            OracleDataReader dr = cmd.ExecuteReader();
            //Models.Departement d = new Models.Departement();
            while (dr.Read())
            {
                if (!dr.IsDBNull(0))
                    return int.Parse(dr.GetValue(0).ToString());
            }
            return -1;
        }

        public static int findModID(string modname)
        {
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            conn.Open();
            string qr = "select * from MODALITY where NAME like '%" + modname + "%'";
            OracleCommand cmd = new OracleCommand(qr, conn);
            OracleDataReader dr = cmd.ExecuteReader();
            //Models.Departement d = new Models.Departement();
            while (dr.Read())
            {
                if (!dr.IsDBNull(0))
                    return int.Parse(dr.GetValue(0).ToString());
            }
            return -1;
        }

            public static string findPatientName(int pid)
            {
                OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
                conn.Open();
                string qr = "select * from PATIENT where NUM=" +pid  + "";
                OracleCommand cmd = new OracleCommand(qr, conn);
                OracleDataReader dr = cmd.ExecuteReader();
            //Models.Departement d = new Models.Departement();
            string res = "";
                while (dr.Read())
                {
                    if (!dr.IsDBNull(3))
                        res=res+" " + dr.GetString(3);
                if (!dr.IsDBNull(4))
                    res = res + " " + dr.GetString(3);
                if (!dr.IsDBNull(5))
                    res = res + " " + dr.GetString(3);
            }
                return res;
            }


    }


    // new stats for appoinments
    public class AppsStatisticModel
    {


        [Display(ResourceType = typeof(Resources.Res), Name = "sinceDate")]
        public DateTime statsAppFromDate { set; get; }

        [Display(ResourceType = typeof(Resources.Res), Name = "toDate")]

        public DateTime statsAppToDate { set; get; }

        [Display(ResourceType = typeof(Resources.Res), Name = "deps")]

        public string[] statsAppDepartements { set; get; }

        [Display(ResourceType = typeof(Resources.Res), Name = "mods")]

        public string[] statsAppClinics { set; get; }

        [Display(ResourceType = typeof(Resources.Res), Name = "ClinicName")]
        public int statsAppClinic { set; get; }


        [Display(ResourceType = typeof(Resources.Res), Name = "appPayType")]
        public int appPayType { set; get; }

        public string GroupingItem { get; set; }

        public static int findDepID(string depname)
        {
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            conn.Open();
            string qr = "select * from DEPARTMENT where NAME like '%" + depname + "%'";
            OracleCommand cmd = new OracleCommand(qr, conn);
            OracleDataReader dr = cmd.ExecuteReader();
            //Models.Departement d = new Models.Departement();
            while (dr.Read())
            {
                if (!dr.IsDBNull(0))
                    return int.Parse(dr.GetValue(0).ToString());
            }
            return -1;
        }

        public static string findPatientName(int pid)
        {
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            conn.Open();
            string qr = "select * from PATIENT where NUM=" + pid + "";
            OracleCommand cmd = new OracleCommand(qr, conn);
            OracleDataReader dr = cmd.ExecuteReader();
            //Models.Departement d = new Models.Departement();
            string res = "";
            while (dr.Read())
            {
                if (!dr.IsDBNull(3))
                    res = res + " " + dr.GetString(3);
                if (!dr.IsDBNull(4))
                    res = res + " " + dr.GetString(3);
                if (!dr.IsDBNull(5))
                    res = res + " " + dr.GetString(3);
            }
            return res;
        }


    }

}