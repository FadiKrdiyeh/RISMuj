using Oracle.DataAccess.Client;
using RISDB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RIS.Models
{
    /// <summary>
    /// Class for users' group
    /// </summary>
    public class Group : IEquatable<Group>
    {
        /// <summary>
        /// The group ID represent the primary key of group in databse
        /// </summary>
        public int num { get; set; }

        /// <summary>
        /// The group name
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "GroupName")]
        [Required(ErrorMessageResourceType = typeof(Resources.Res), ErrorMessageResourceName = "GroupNameError")]
        public string name { set; get; }

        /// <summary>
        /// Dfining the equality of tow groups
        /// </summary>
        /// <param name="other">a group object to compare with</param>
        /// <returns>boolean, true if they were equal, false if not</returns>
        public bool Equals(Group other)
        {
            return this.num == other.num && this.name == other.name;
        }

        /// <summary>
        /// Group constructor
        /// </summary>
        public Group() { }

        /// <summary>
        /// Gets all groups from database
        /// </summary>
        /// <returns>list of groups in the database</returns>
        public static List<Group> getData()
        {
            List<Group> gList = new List<Group>();

            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("SELECT * FROM GROUPS ORDER BY NUM DESC", conn);
                OracleDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Group mt = new Group();
                    #region Data
                    if (!dr.IsDBNull(0))
                        mt.num = int.Parse(dr.GetValue(0).ToString());
                    if (!dr.IsDBNull(1))
                        mt.name = dr.GetString(1);
                    #endregion
                    gList.Add(mt);
                }
            }
            catch
            {
            }
            finally
            {
                conn.Close();
            }
            return gList;
        }

        /// <summary>
        /// Gets the details of a defined by ID group from database
        /// </summary>
        /// <param name="num">the group ID</param>
        /// <returns>a group object contains the group details</returns>
        public static Group Select(int num)
        {
            Group mt = new Group();
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("SELECT * FROM GROUPS WHERE NUM= " + num, conn);
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    #region Get Data
                    if (!dr.IsDBNull(0))
                        mt.num = Int32.Parse(dr.GetValue(0).ToString());
                    if (!dr.IsDBNull(1))
                        mt.name = dr.GetString(1);
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
            return mt;
        }

        /// <summary>
        /// Gets the details of a defined by name group from database
        /// </summary>
        /// <param name="gName">the group name</param>
        /// <returns>a group object contains the group details</returns>
        public static Group SelectByName(string gName)
        {
            Group mt = new Group();
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("SELECT * FROM GROUPS WHERE NAME= '" + gName + "'", conn);
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    #region Get Data
                    if (!dr.IsDBNull(0))
                        mt.num = Int32.Parse(dr.GetValue(0).ToString());
                    if (!dr.IsDBNull(1))
                        mt.name = dr.GetString(1);
                    #endregion
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
            return mt;
        }

        /// <summary>
        /// Inserts a group details into database
        /// </summary>
        /// <param name="mt">a group object contains the group details</param>
        /// <returns>the exception message if there is any, an empty string if none</returns>
        public static string Insert(Group mt)
        {
            string res = "";
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                string qr = "Begin" +
                            "  insert into GROUPS" +
                            "( NUM, NAME)" +
                            "values " +
                            "(:NUM,:NAME);" +
                            "End;";
                OracleCommand cmd = new OracleCommand(qr, conn);
                OracleParameter[] param =  {
                                            new OracleParameter("NUM", mt.num),
                                            new OracleParameter("NAME", mt.name),
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
        /// Gets groups from database to be shown in a select list
        /// </summary>
        /// <param name="defaultValue">the default value of the select list</param>
        /// <returns>a select list contains the groups' names</returns>
        public static SelectList GetGroupsList(string defaultValue)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "", Value = "" });
            foreach (var item in Group.getData())
            {
                items.Add(new SelectListItem { Text = item.name, Value = item.num.ToString() });
            }
            return new SelectList(items, "Value", "Text", string.IsNullOrEmpty(defaultValue) ? "0" : defaultValue);
        }

        /// <summary>
        /// Gets groups from database
        /// </summary>
        /// <returns>a list of all groups in database</returns>
        public static List<Group> getAllGroups()
        {
            List<Group> gList = new List<Group>();

            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("SELECT * FROM GROUPS ORDER BY NUM DESC", conn);
                OracleDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Group mt = new Group();
                    #region Data
                    if (!dr.IsDBNull(0))
                        mt.num = int.Parse(dr.GetValue(0).ToString());
                    if (!dr.IsDBNull(1))
                        mt.name = dr.GetString(1);
                    #endregion
                    gList.Add(mt);
                }
            }
            catch
            {
            }
            finally
            {
                conn.Close();
            }
            return gList;
        }



        /// <summary>
        /// Gets the groups that a defined user is a member of them
        /// </summary>
        /// <param name="id">the user ID</param>
        /// <returns>list of groups of the user</returns>
        public static List<Group> getUserGroups(int id)
        {
            List<Group> pList = new List<Group>();
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("SELECT GROUPS.NUM, GROUPS.NAME FROM GROUPS, USERGROUPS WHERE GROUPS.NUM = USERGROUPS.GNUM AND USERGROUPS.UNUM = '" + id + "' ORDER BY GROUPS.NUM DESC", conn);
                OracleDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Group mt = new Group();
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

        /// <summary>
        /// Deletes a group from database
        /// </summary>
        /// <param name="num">the group ID</param>
        /// <returns>the exception message if there is any, an empty string if none</returns>
        public static string Delete(int num)
        {
            string res = "";
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("DELETE GROUPS WHERE NUM = :NUM ", conn);
                cmd.Parameters.Add(new OracleParameter("NUM", num));
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
        /// Edits the group information in database
        /// </summary>
        /// <param name="mt">a group object contains the new information</param>
        /// <returns>the exception message if there is any, an empty string if none</returns>
        public static string Edit(Group mt)
        {

            string res = "";
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                string qr = "  Update GROUPS Set " +
                            "  NAME =:NAME " +
                            "  where NUM =:NUM";
                OracleParameter[] param =  {
                                                new OracleParameter("NAME", mt.name),
                                                new OracleParameter("NUM", mt.num),
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
        /// Defines the user as amember of group
        /// </summary>
        /// <param name="u">the user ID</param>
        /// <param name="g">the group ID</param>
        /// <returns>the exception message if there is any, an empty string if none</returns>
        public static string assignUserGroup(int u, int g)
        {
            string res = "";
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                string qr = "Begin" +
                            "  insert into USERGROUPS" +
                            "( UNUM, GNUM)" +
                            "values " +
                            "(:UNUM,:GNUM);" +
                            "End;";
                OracleCommand cmd = new OracleCommand(qr, conn);
                OracleParameter[] param =  {
                                            new OracleParameter("UNUM", u),
                                            new OracleParameter("GNUM", g),
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
        /// deletes the membership of the user in all his group
        /// </summary>
        /// <param name="u">the user ID</param>
        /// <returns>the exception message if there is any, an empty string if none</returns>
        public static string deleteUserGroups(int u)
        {
            string res = "";
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                string qr = "Begin" +
                            "  delete from USERGROUPS where " +
                            " UNUM = " +
                            " :UNUM;" +
                            "End;";
                OracleCommand cmd = new OracleCommand(qr, conn);
                OracleParameter[] param =  {
                                            new OracleParameter("UNUM", u),
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

    }
}