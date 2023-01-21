using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Oracle.DataAccess.Client;
using RISDB;

namespace RIS.Models
{
    /// <summary>
    /// Log In Class
    /// </summary>
    public class LogIn
    {

        /// <summary>
        /// logging username
        /// </summary>
        public string userName { set; get; }

        /// <summary>
        /// logging password
        /// </summary>
        public string passWord { set; get; }


        /// <summary>
        /// validating the logging in parameters
        /// </summary>
        /// <param name="un">logging username</param>
        /// <param name="pw">logging password</param>
        /// <returns>boolean, true if the logging is valid, false if not</returns>
        public static bool vallidateUser(string un, string pw)
        {
            OracleConnection con = new OracleConnection(OracleRIS.GetConnectionString());

            string qr = "SELECT * FROM LOGGEDUSER WHERE USERNAME= '" + un + "'" + " AND PASS= '" + pw + "'";
            OracleCommand cmd = new OracleCommand(qr, con);

            try
            {
                con.Open();

                OracleDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    if (!dr.IsDBNull(4))
                        HttpContext.Current.Session["userType"] = dr.GetValue(4).ToString();

                    if (!dr.IsDBNull(7))
                        HttpContext.Current.Session["userDep"] = dr.GetValue(7).ToString();

                    if (!dr.IsDBNull(1))
                        HttpContext.Current.Session["userName"] = dr.GetValue(1).ToString();
                    if (!dr.IsDBNull(3))
                        HttpContext.Current.Session["userLang"] = dr.GetValue(3).ToString();
                    else
                        HttpContext.Current.Session["userLang"] = "Ar";

                    //Nav bar Perms
                    string uName = "";
                    try
                    {
                        uName = HttpContext.Current.Session["userName"].ToString();
                    }
                    catch
                    {
                        return false;
                    }
                    finally
                    {
                        con.Close();
                    }

                    int userId = User.getUserByUname(uName).num;

                    HttpContext.Current.Session["PatientPerm"] = "0";
                    if (User.hasPerm(userId, Perms.PatientIndex))
                        HttpContext.Current.Session["PatientPerm"] = "1";

                    HttpContext.Current.Session["RadiologyPerm"] = "0";
                    if (User.hasPerm(userId, Perms.RadiologyIndex))
                        HttpContext.Current.Session["RadiologyPerm"] = "1";

                    HttpContext.Current.Session["RadiologyStatusPerm"] = "0";
                    if (User.hasPerm(userId, Perms.RadiologyOrderStatus))
                        HttpContext.Current.Session["RadiologyStatusPerm"] = "1";

                    HttpContext.Current.Session["PermsPerm"] = "0";
                    if (User.hasPerm(userId, Perms.PermsIndex))
                        HttpContext.Current.Session["PermsPerm"] = "1";

                    HttpContext.Current.Session["UserPerm"] = "0";
                    if (User.hasPerm(userId, Perms.UserIndex))
                        HttpContext.Current.Session["UserPerm"] = "1";

                    HttpContext.Current.Session["ModalityPerm"] = "0";
                    if (User.hasPerm(userId, Perms.ModalityIndex))
                        HttpContext.Current.Session["ModalityPerm"] = "1";

                    HttpContext.Current.Session["ModalityTypePerm"] = "0";
                    if (User.hasPerm(userId, Perms.ModalityTypeIndex))
                        HttpContext.Current.Session["ModalityTypePerm"] = "1";

                    HttpContext.Current.Session["ProcedurePerm"] = "0";
                    if (User.hasPerm(userId, Perms.ProcedureIndex))
                        HttpContext.Current.Session["ProcedurePerm"] = "1";

                    HttpContext.Current.Session["DepartmentPerm"] = "0";
                    if (User.hasPerm(userId, Perms.DepartmentIndex))
                        HttpContext.Current.Session["DepartmentPerm"] = "1";

                    HttpContext.Current.Session["StatsPerm"] = "0";
                    if (User.hasPerm(userId, Perms.StatsIndex))
                        HttpContext.Current.Session["StatsPerm"] = "1";

                    HttpContext.Current.Session["AppStatsPerm"] = "0";
                    if (User.hasPerm(userId, Perms.AppStatsIndex))
                        HttpContext.Current.Session["AppStatsPerm"] = "1";
                    //delete patient
                    HttpContext.Current.Session["PatientDelPerm"] = "0";
                    if (User.hasPerm(userId, Perms.PatientDelete ))
                        HttpContext.Current.Session["PatientDelPerm"] = "1";

                    HttpContext.Current.Session["RadiologyCreate"] = "0";
                    if (User.hasPerm(userId, Perms.RadiologyCreate))
                        HttpContext.Current.Session["RadiologyCreate"] = "1";

                    HttpContext.Current.Session["RadiologyCreateSchedualed"] = "0";
                    if (User.hasPerm(userId, Perms.RadiologyCreateSchedualed))
                        HttpContext.Current.Session["RadiologyCreateSchedualed"] = "1";
					HttpContext.Current.Session["AppsPerm"] = "0";

					if (User.hasPerm(userId, Perms.ClinicAppoinmentIndex))
						HttpContext.Current.Session["AppsPerm"] = "1";

					HttpContext.Current.Session["NewsPerm"] = "0";

					if (User.hasPerm(userId, Perms.NewsIndex))
						HttpContext.Current.Session["NewsPerm"] = "1";

					HttpContext.Current.Session["BillsPerm"] = "0";
					if (User.hasPerm(userId, Perms.BillsIndex))
						HttpContext.Current.Session["BillsPerm"] = "1";

                    HttpContext.Current.Session["ClinicAppoinmentIndex"] = "0";
                    if (User.hasPerm(userId, Perms.ClinicAppoinmentIndex))
                        HttpContext.Current.Session["ClinicAppoinmentIndex"] = "1";

                    HttpContext.Current.Session["ClinicAppoinmentCreate"] = "0";
                    if (User.hasPerm(userId, Perms.ClinicAppoinmentCreate))
                        HttpContext.Current.Session["ClinicAppoinmentCreate"] = "1";

                    HttpContext.Current.Session["ClinicAppoinmentEdit"] = "0";
                    if (User.hasPerm(userId, Perms.ClinicAppoinmentEdit))
                        HttpContext.Current.Session["ClinicAppoinmentEdit"] = "1";

                    HttpContext.Current.Session["ClinicAppoinmentDelete"] = "0";
                    if (User.hasPerm(userId, Perms.ClinicAppoinmentDelete))
                        HttpContext.Current.Session["ClinicAppoinmentDelete"] = "1";

                    HttpContext.Current.Session["ClinicAppoinmentDetails"] = "0";
                    if (User.hasPerm(userId, Perms.ClinicAppoinmentDetails))
                        HttpContext.Current.Session["ClinicAppoinmentDetails"] = "1";

                    HttpContext.Current.Session["AppStatsIndex"] = "0";
                    if (User.hasPerm(userId, Perms.AppStatsIndex))
                        HttpContext.Current.Session["AppStatsIndex"] = "1";

                    HttpContext.Current.Session["AppStatsPatient"] = "0";
                    if (User.hasPerm(userId, Perms.AppStatsPatient))
                        HttpContext.Current.Session["AppStatsPatient"] = "1";

                    HttpContext.Current.Session["StatsApps"] = "0";
                    if (User.hasPerm(userId, Perms.StatsApps))
                        HttpContext.Current.Session["StatsApps"] = "1";

                    HttpContext.Current.Session["BillsCreate"] = "0";
                    if (User.hasPerm(userId, Perms.BillsCreate))
                        HttpContext.Current.Session["BillsCreate"] = "1";

                    HttpContext.Current.Session["BillsEdit"] = "0";
                    if (User.hasPerm(userId, Perms.BillsEdit))
                        HttpContext.Current.Session["BillsEdit"] = "1";

                    HttpContext.Current.Session["BillsDelete"] = "0";
                    if (User.hasPerm(userId, Perms.BillsDelete))
                        HttpContext.Current.Session["BillsDelete"] = "1";

                    HttpContext.Current.Session["BillsDetails"] = "0";
                    if (User.hasPerm(userId, Perms.BillsDetails))
                        HttpContext.Current.Session["BillsDetails"] = "1";

                    con.Close();
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
            finally
            {
                con.Close();
            }



        }
    }
}