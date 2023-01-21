using Oracle.DataAccess.Client;
using RISDB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace RIS.Models
{
    /// <summary>
    /// The class of user
    /// </summary>
    public class User
    {

        #region Attributes

        /// <summary>
        /// user ID, the primary key of user table in database
        /// </summary>
        [Required(ErrorMessage = "يجب إدخال الرقم"), DisplayName("رقم الستخدم")]
        public int num { set; get; }

        /// <summary>
        /// The user's name
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "UserName")]
        [Required(ErrorMessageResourceType = typeof(Resources.Res), ErrorMessageResourceName = "requiredusername")]
        public string username { set; get; }

        /// <summary>
        /// The user's password
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "Password")]
        [Required(ErrorMessageResourceType = typeof(Resources.Res), ErrorMessageResourceName = "requireduserpass")]
        public string pass { set; get; }

        /// <summary>
        /// The user language
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "Language")]
        public string language { set; get; }

        /// <summary>
        /// The user's role i.e. administrator or Employee
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "Role")]
        [Required(ErrorMessageResourceType = typeof(Resources.Res), ErrorMessageResourceName = "requiredrole")]
        public int role { set; get; }

        /// <summary>
        /// User's first name
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "userFn")]
        [Required(ErrorMessageResourceType = typeof(Resources.Res), ErrorMessageResourceName = "requiredfirstname")]
        public string firstName { set; get; }

        /// <summary>
        /// User's last name
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "userLn")]
        [Required(ErrorMessageResourceType = typeof(Resources.Res), ErrorMessageResourceName = "requiredlastname")]
        public string lastName { set; get; }

        /// <summary>
        /// User's department
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "Departement")]
        [Required(ErrorMessageResourceType = typeof(Resources.Res), ErrorMessageResourceName = "requireduserDept")]
        public string departement { set; get; }

        ///// <summary>
        ///// User's clinic
        ///// </summary>
        //[Display(ResourceType = typeof(Resources.Res), Name = "Clinic")]
        //[Required(ErrorMessageResourceType = typeof(Resources.Res), ErrorMessageResourceName = "ClinicRequired")]
        //public string clinic { set; get; }

        /// <summary>
        /// List of languages
        /// </summary>
        public List<string> languages = new List<string> { "العربية", "English" };
        #endregion

        /// <summary>
        /// The user constructor
        /// </summary>
        public User() { }

        /// <summary>
        /// Department object contains the user's department details
        /// </summary>
        public Departement userDepartement
        {
            get
            {
                return Departement.select(int.Parse(departement));

            }
        }

        /// <summary>
        /// Department object contains the user's clinic details
        /// </summary>
        //public string userClinic
        //{
        //    get
        //    {
        //        return GeniricIndex.select(int.Parse(clinic),"CLINIC").name;

        //    }
        //}

        /// <summary>
        /// String representing user's role i.e. Administrator or Employee
        /// </summary>
        public string userRole
        {
            get
            {
                if (role == 1)
                {
                    return RIS.Resources.Res.Administrator.ToString();
                }
                else
                {
                    return RIS.Resources.Res.Employee.ToString();
                }
            }
        }

        /// <summary>
        /// Gets the department of the user based on his name
        /// </summary>
        /// <param name="uName">the user's name</param>
        /// <returns>the user's department ID</returns>
        public static string getDepID(string uName)
        {
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            User u = new User();

            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("SELECT * FROM LOGGEDUSER WHERE USERNAME= '" + uName + "' ORDER BY NUM DESC", conn);
                OracleDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {

                    if (!dr.IsDBNull(0))
                        u.num = Int32.Parse(dr.GetValue(0).ToString());
                    if (!dr.IsDBNull(1))
                        u.username = dr.GetValue(1).ToString();
                    if (!dr.IsDBNull(2))
                        u.pass = dr.GetValue(2).ToString();
                    if (!dr.IsDBNull(3))
                        u.language = dr.GetValue(3).ToString();
                    if (!dr.IsDBNull(4))
                        u.role = Int32.Parse(dr.GetValue(4).ToString());
                    if (!dr.IsDBNull(5))
                        u.firstName = dr.GetValue(5).ToString();
                    if (!dr.IsDBNull(6))
                        u.lastName = dr.GetValue(6).ToString();
                    if (!dr.IsDBNull(7))
                        u.departement = dr.GetValue(7).ToString();

                }
            }
            catch (Exception es)
            {
                string ees = es.ToString();
                int ui = 0;
            }
            finally
            {
                conn.Close();
            }

            return u.departement;
        }

        /// <summary>
        /// Gets all users from database
        /// </summary>
        /// <returns>list of users contains all users details</returns>
        public static List<User> getAllUsers()
        {
            List<User> res = new List<User>();

            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());

            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("SELECT * FROM LOGGEDUSER ORDER BY NUM DESC", conn);
                OracleDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    User u = new User();
                    if (!dr.IsDBNull(0))
                        u.num = Int32.Parse(dr.GetValue(0).ToString());
                    if (!dr.IsDBNull(1))
                        u.username = dr.GetValue(1).ToString();
                    if (!dr.IsDBNull(2))
                        u.pass = dr.GetValue(2).ToString();
                    if (!dr.IsDBNull(3))
                        u.language = dr.GetValue(3).ToString();
                    if (!dr.IsDBNull(4))
                        u.role = Int32.Parse(dr.GetValue(4).ToString());
                    if (!dr.IsDBNull(5))
                        u.firstName = dr.GetValue(5).ToString();
                    if (!dr.IsDBNull(6))
                        u.lastName = dr.GetValue(6).ToString();
                    if (!dr.IsDBNull(7))
                        u.departement = dr.GetValue(7).ToString();
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

        /// <summary>
        /// Gets the details of a defined user from database by his ID
        /// </summary>
        /// <param name="num">user's ID</param>
        /// <returns>user object contains the wanted user details</returns>
        public static User select(int num)
        {
            User u = new User();

            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());

            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("SELECT * FROM LOGGEDUSER WHERE NUM= " + num + " ORDER BY NUM DESC", conn);
                OracleDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    if (!dr.IsDBNull(0))
                        u.num = Int32.Parse(dr.GetValue(0).ToString());
                    if (!dr.IsDBNull(1))
                        u.username = dr.GetValue(1).ToString();
                    if (!dr.IsDBNull(2))
                        u.pass = dr.GetValue(2).ToString();
                    if (!dr.IsDBNull(3))
                        u.language = dr.GetValue(3).ToString();
                    if (!dr.IsDBNull(4))
                        u.role = Int32.Parse(dr.GetValue(4).ToString());
                    if (!dr.IsDBNull(5))
                        u.firstName = dr.GetValue(5).ToString();
                    if (!dr.IsDBNull(6))
                        u.lastName = dr.GetValue(6).ToString();
                    if (!dr.IsDBNull(7))
                        u.departement = dr.GetValue(7).ToString();

                }
            }
            catch
            {

            }
            finally
            {
                conn.Close();
            }

            return u;
        }

        /// <summary>
        /// Gets the details of a defined user from database by his name
        /// </summary>
        /// <param name="name">user's name</param>
        /// <returns>user object contains the wanted user details</returns>
        public static User SelectByName(string name)
        {
            User u = new User();

            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());

            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("SELECT * FROM LOGGEDUSER WHERE USERNAME= '" + name + "' ", conn);
                OracleDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    if (!dr.IsDBNull(0))
                        u.num = Int32.Parse(dr.GetValue(0).ToString());
                    if (!dr.IsDBNull(1))
                        u.username = dr.GetValue(1).ToString();
                    if (!dr.IsDBNull(2))
                        u.pass = dr.GetValue(2).ToString();
                    if (!dr.IsDBNull(3))
                        u.language = dr.GetValue(3).ToString();
                    if (!dr.IsDBNull(4))
                        u.role = Int32.Parse(dr.GetValue(4).ToString());
                    if (!dr.IsDBNull(5))
                        u.firstName = dr.GetValue(5).ToString();
                    if (!dr.IsDBNull(6))
                        u.lastName = dr.GetValue(6).ToString();
                    if (!dr.IsDBNull(7))
                        u.departement = dr.GetValue(7).ToString();

                }
                else
                    return null;
            }
            catch
            {

            }
            finally
            {
                conn.Close();
            }

            return u;
        }

        /// <summary>
        /// Inserts new user details into database
        /// </summary>
        /// <param name="u">user object contains the user details</param>
        /// <returns>exception message if there is any, empty string if none</returns>
        public static string insertUser(User u)
        {
            string res = "";
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                string qr = "Begin" +
                            "  insert into LOGGEDUSER" +

                            "( NUM, USERNAME, PASS, LANGUAGE, ROLE, FIRSTNAME, LASTNAME, DEPARTEMENT) " +
                            " values " +
                            " (:NUM, :USERNAME, :PASS, :LANGUAGE, :ROLE, :FIRSTNAME, :LASTNAME, :DEPARTEMENT); " +
                            "End;";
                OracleCommand cmd = new OracleCommand(qr, conn);
                OracleParameter[] param = {
                                            new OracleParameter("NUM", u.num),
                                            new OracleParameter("USERNAME", u.username),
                                            new OracleParameter("PASS", u.pass),
                                            new OracleParameter("LANGUAGE", u.language),
                                            new OracleParameter("ROLE", u.role),
                                            new OracleParameter("FIRSTNAME", u.firstName),
                                            new OracleParameter("LASTNAME", u.lastName),
                                            new OracleParameter("DEPARTEMENT", u.departement)
                                           };
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
        /// checks if a defined user has done any activity on RIS
        /// </summary>
        /// <param name="num">user's ID</param>
        /// <returns>boolean, true if he didn't do any activity, false if he did</returns>
        public static bool checkUserActivities(int num)
        {
            bool res = true;
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            OracleCommand cmd = new OracleCommand();

            string qr = "select * from PATIENT where INSERTUSER='" + num + "' ";

            try
            {
                conn.Open();
                cmd = new OracleCommand(qr, conn);
                if (cmd.ExecuteReader().Read())
                {
                    conn.Close();
                    return false;
                }
            }
            catch
            {
                conn.Close();
            }
            conn.Close();

            qr = "select * from ORDERS where INSERTUSER='" + num + "' ";

            try
            {
                conn.Open();
                cmd = new OracleCommand(qr, conn);
                if (cmd.ExecuteReader().Read())
                {
                    conn.Close();
                    return false;
                }

            }
            catch
            {
                conn.Close();
            }
            conn.Close();

            qr = "select * from OLDORDERS where UPDATEDUSER='" + num + "' ";

            try
            {
                conn.Open();
                cmd = new OracleCommand(qr, conn);
                if (cmd.ExecuteReader().Read())
                {
                    conn.Close();
                    return false;
                }

            }
            catch
            {
                conn.Close();
            }
            conn.Close();

            qr = "select * from OLDPATIENT where UPDATEDUSER='" + num + "' ";

            try
            {
                conn.Open();
                cmd = new OracleCommand(qr, conn);
                if (cmd.ExecuteReader().Read())
                {
                    conn.Close();
                    return false;
                }

            }
            catch
            {
                conn.Close();
            }
            conn.Close();

            return res;
        }

        /// <summary>
        /// Deletes a defined user details from database
        /// </summary>
        /// <param name="u">user object contains the user details</param>
        /// <returns>exception message if there is any, empty string if none</returns>
        public static string deleteUser(User u)
        {
            if (!checkUserActivities(u.num))
                return RIS.Resources.Res.ErrorYouCant;
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());

            string res = "";

            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("DELETE LOGGEDUSER WHERE NUM = :NUM ", conn);
                cmd.Parameters.Add(new OracleParameter("NUM", u.num));
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
        /// Edits a defined user details in database
        /// </summary>
        /// <param name="u">user object contains the new user details</param>
        /// <returns>exception message if there is any, empty string if none</returns>
        public static string updateUser(User u)
        {
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            string res = "";
            try
            {
                conn.Open();
                string qr = "  Update LOGGEDUSER Set " +
                    " USERNAME = :USERNAME, " +
                    " PASS = :PASS, " +
                    " LANGUAGE = :LANGUAGE, " +
                    //" ROLE = :ROLE, " +
                    " FIRSTNAME = :FIRSTNAME, " +
                    " LASTNAME = :LASTNAME, " +
                    " DEPARTEMENT = :DEPARTEMENT " +
                   "  where NUM = :NUM ";
                OracleParameter[] param = {

                                            new OracleParameter("USERNAME", u.username),
                                            new OracleParameter("PASS", u.pass),
                                            new OracleParameter("LANGUAGE", u.language),
                                            //new OracleParameter("ROLE", u.role),
                                            new OracleParameter("FIRSTNAME", u.firstName),
                                            new OracleParameter("LASTNAME", u.lastName),
                                            new OracleParameter("DEPARTEMENT", u.departement),
                                            new OracleParameter("NUM", u.num)
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
        /// Checks if the name of the inserted user is duplicated or not
        /// </summary>
        /// <param name="un">enserted user name</param>
        /// <returns>boolean, true if duplicated, false if not</returns>
        public static bool checkDuplicate(string un)
        {
            //User u = new User();

            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());

            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("SELECT * FROM LOGGEDUSER WHERE USERNAME= '" + un + "' ORDER BY NUM DESC", conn);

                OracleDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    return false;
                }
            }
            catch
            {

            }
            finally
            {
                conn.Close();
            }

            return true;
        }

        //public static string updateUserLang(User u)
        //{
        //    OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
        //    string res = "";
        //    try
        //    {
        //        conn.Open();
        //        string qr = "  Update LOGGEDUSER Set " +
        //            " USERNAME = :USERNAME, " +
        //            " PASS = :PASS, " +
        //            " LANGUAGE = :LANGUAGE, " +
        //            " ROLE = :ROLE, " +
        //            " FIRSTNAME = :FIRSTNAME, " +
        //            " LASTNAME = :LASTNAME, " +
        //            " DEPARTEMENT = :DEPARTEMENT " +
        //           "  where NUM = :NUM ";
        //        OracleParameter[] param = {

        //                                    new OracleParameter("USERNAME", u.username),
        //                                    new OracleParameter("PASS", u.pass),
        //                                    new OracleParameter("LANGUAGE", u.language),
        //                                    new OracleParameter("ROLE", u.role),
        //                                    new OracleParameter("FIRSTNAME", u.firstName),
        //                                    new OracleParameter("LASTNAME", u.lastName),
        //                                    new OracleParameter("DEPARTEMENT", u.departement),
        //                                    new OracleParameter("NUM", u.num)
        //                                   };
        //        OracleCommand cmd = new OracleCommand(qr, conn);
        //        for (int j = 0; j < param.Length; j++)
        //        {
        //            cmd.Parameters.Add(param[j]);
        //        }
        //        int x = cmd.ExecuteNonQuery();


        //    }
        //    catch (OracleException e)
        //    {
        //        res = e.Message;
        //    }
        //    catch
        //    {
        //        res = "حدث خطأ";
        //    }
        //    finally
        //    {
        //        conn.Close();
        //    }

        //    return res;
        //}

        //get user perms

        /// <summary>
        /// Gets the permission list of the user
        /// </summary>
        /// <param name="id">user's ID</param>
        /// <returns>list of permissions of the user</returns>
        public static List<Permission> getUserPermissions(int id)
        {
            List<Permission> pList = new List<Permission>();
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();

                OracleCommand cmd = new OracleCommand("select distinct PERM.NUM,PERM.NAME from RIS.PERMISSIONS perm, RIS.GROUPPERMISSIONS gp where GP.PNUM= PERM.NUM and GP.GNUM in (select GRO.NUM from RIS.GROUPS gro, RIS.USERGROUPS ug where UG.UNUM = '" + id + "' and UG.GNUM = GRO.NUM) ", conn);
                OracleDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Permission mt = new Permission();
                    #region Data
                    if (!dr.IsDBNull(0))
                        mt.num = int.Parse(dr.GetValue(0).ToString());
                    if (!dr.IsDBNull(1))
                        mt.name = dr.GetString(1);
                    #endregion
                    pList.Add(mt);
                }
            }
            catch
            {
            }
            finally
            {
                conn.Close();
            }
            return pList;
        }

        //get user groups --> in  GROUP Model


        //is user has permission
        /// <summary>
        /// Checks if the user has a defined permission or not
        /// </summary>
        /// <param name="id">user's ID</param>
        /// <param name="perm">the tested permission</param>
        /// <returns>boolean, true if the user has this permission, false if not</returns>
        public static bool hasPerm(int id, int perm)
        {
            bool has = false;
            Permission p = Permission.getPermById(perm);
            List<Permission> userPerms = getUserPermissions(id);

            if (userPerms.Contains(p))
                has = true;
            return has;
        }

        /// <summary>
        /// Gets a defined user data from database by his name
        /// </summary>
        /// <param name="uName">user's name</param>
        /// <returns>user object contains the wanted user details</returns>
        public static User getUserByUname(string uName)
        {
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            User u = new User();

            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("SELECT * FROM LOGGEDUSER WHERE USERNAME= '" + uName + "' ORDER BY NUM DESC", conn);
                OracleDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {

                    if (!dr.IsDBNull(0))
                        u.num = Int32.Parse(dr.GetValue(0).ToString());
                    if (!dr.IsDBNull(1))
                        u.username = dr.GetValue(1).ToString();
                    if (!dr.IsDBNull(2))
                        u.pass = dr.GetValue(2).ToString();
                    if (!dr.IsDBNull(3))
                        u.language = dr.GetValue(3).ToString();
                    if (!dr.IsDBNull(4))
                        u.role = Int32.Parse(dr.GetValue(4).ToString());
                    if (!dr.IsDBNull(5))
                        u.firstName = dr.GetValue(5).ToString();
                    if (!dr.IsDBNull(6))
                        u.lastName = dr.GetValue(6).ToString();
                    if (!dr.IsDBNull(7))
                        u.departement = dr.GetValue(7).ToString();

                }
            }
            catch (Exception es)
            {
                string ees = es.ToString();
                int ui = 0;
            }
            finally
            {
                conn.Close();
            }

            return u;
        }

    }
}