using Oracle.DataAccess.Client;
using RISDB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace RIS.Models
{
    public class GeniricIndex
    {

        /// <summary>
        ///  ID, Primary key for table in database
        /// </summary>
        public int num { set; get; }

        /// <summary>
        ///  name
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "tshIndexName")]
        [Required(ErrorMessageResourceType = typeof(Resources.Res), ErrorMessageResourceName = "tshIndexNameReq")]
        public string name { get; set; }

        /// <summary>
        /// object constructor
        /// </summary>
        public GeniricIndex() { }

        /// <summary>
        /// Get all Index from database
        /// </summary>
        /// <returns>List of all index in database</returns>
        public static List<GeniricIndex> getData(string tableName)
        {
            List<GeniricIndex> depList = new List<GeniricIndex>();

            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());


            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("SELECT * FROM " + tableName + " ", conn);
                OracleDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    GeniricIndex dep = new GeniricIndex();
                    if (!dr.IsDBNull(0))
                        dep.num = Int32.Parse(dr.GetValue(0).ToString());
                    if (!dr.IsDBNull(1))
                        dep.name = dr.GetString(1);

                    depList.Add(dep);
                }
            }

            catch
            {
            }
            finally
            {
                conn.Close();

            }
            return depList;
        }

        /// <summary>
        /// Inserts  into  table in database
        /// </summary>
        /// <param name="mt"> object contains  details</param>
        /// <returns>exception message if there is any, empty string if not</returns>
        public static string Insert(GeniricIndex mt, string tableName)
        {



            GeniricIndex checkIfExists = SelectByName(mt.name, tableName);
            if (checkIfExists.name != null)
                return "";
            string res = "";
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                string qr = "Begin" +
                            "  insert into " + tableName + " " +

                            "( NUM, NAME) " +
                            " values " +
                            " (:NUM, :NAME); " +
                            "End;";
                OracleCommand cmd = new OracleCommand(qr, conn);
                OracleParameter[] param =  {
                                            new OracleParameter("NUM",mt.num),
                                            new OracleParameter("NAME", mt.name)
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
        /// Gets  as selectlist
        /// </summary>
        /// <param name="withAllOption">to add an empty value</param>
        /// <param name="defaultValue">default value of selectlist</param>
        /// <returns>select list contains all </returns>
        public static SelectList GetGeniricIndexList(bool withAllOption, string defaultValue, string tableName)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            if (withAllOption)
                items.Add(new SelectListItem { Text = "", Value = "" });
            foreach (var item in GeniricIndex.getData(tableName))
            {
                items.Add(new SelectListItem { Text = item.name, Value = item.num.ToString() });
            }
            SelectList s =new SelectList(items, "Value", "Text", string.IsNullOrEmpty(defaultValue) ? "0" : defaultValue);
            return s;
        }
        public static SelectList GetClinicIndexList(bool withAllOption, string defaultValue, string tableName)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            if (withAllOption)
                items.Add(new SelectListItem { Text = Resources.Res.All, Value = "-1" });//todo add resource
            foreach (var item in GeniricIndex.getData(tableName))
            {
                items.Add(new SelectListItem { Text = item.name, Value = item.num.ToString() });
            }
            return new SelectList(items, "Value", "Text", string.IsNullOrEmpty(defaultValue) ? "0" : defaultValue);
        }

        /// <summary>
        /// Gets all index names
        /// </summary>
        /// <returns>List of strings containing indexes names</returns>
        public static List<string> tableListNames(string tableName)
        {
            List<string> items = new List<string>();
            foreach (var item in GeniricIndex.getData(tableName))
            {
                items.Add(item.name);
            }
            return items;
        }

        public static int findID(string tableName, string name)
        {
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            conn.Open();
            string qr = "select * from "+ tableName + " where NAME like '%" + name + "%'";
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


        /// <summary>
        /// Gets indexes names as a select list
        /// </summary>
        /// <param name="withAllOption">to add an empty value to the select list</param>
        /// <param name="defaultValue">setting selectlist default value</param>
        /// <returns></returns>
        public static SelectList GetIndexListNames(bool withAllOption, string defaultValue, string tableName)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            if (withAllOption)
                items.Add(new SelectListItem { Text = "", Value = "" });
            foreach (var item in GeniricIndex.getData(tableName))
            {
                items.Add(new SelectListItem { Text = item.name, Value = item.num.ToString() });
            }

            return new SelectList(items, "Value", "Text", string.IsNullOrEmpty(defaultValue) ? "0" : defaultValue);
        }

        /// <summary>
        /// Deletes a indexes from database
        /// </summary>
        /// <param name="i">index ID</param>
        /// <returns>exception message if there is any, empty string if none</returns>
        public static string Delete(int i, string tableName)
        {
            string res = "";
            int count = 0;
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("SELECT COUNT(NUM) FROM PATIENT WHERE " + tableName + " = " + i, conn);
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    if (!dr.IsDBNull(0))
                        count = Int32.Parse(dr.GetValue(0).ToString());
                }

                cmd = new OracleCommand("SELECT COUNT(NUM) FROM OLDPATIENT WHERE " + tableName + " = " + i, conn);
                dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    if (!dr.IsDBNull(0))
                        count += Int32.Parse(dr.GetValue(0).ToString());
                }


                if (count == 0)
                {
                    cmd = new OracleCommand("DELETE " + tableName + " WHERE NUM = :NUM ", conn);
                    cmd.Parameters.Add(new OracleParameter("NUM", i));
                    cmd.ExecuteNonQuery();
                }
                else
                    res = RIS.Resources.Res.ErrorYouCant;
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
        /// Deletes a indexes from database
        /// </summary>
        /// <param name="i">index ID</param>
        /// <returns>exception message if there is any, empty string if none</returns>
        public static string DeleteClinic(int i, string tableName)
        {
            string res = "";
            int count = 0;
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("SELECT COUNT(NUM) FROM LOGGEDUSER WHERE  DEPARTEMENT  = " + i, conn);
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    if (!dr.IsDBNull(0))
                        count = Int32.Parse(dr.GetValue(0).ToString());
                }

                if (count == 0)
                {
                    cmd = new OracleCommand("DELETE " + tableName + " WHERE NUM = :NUM ", conn);
                    cmd.Parameters.Add(new OracleParameter("NUM", i));
                    cmd.ExecuteNonQuery();
                }
                else
                    res = RIS.Resources.Res.ErrorYouCant;
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
        /// Gets the details of a defined index from database based on its ID
        /// </summary>
        /// <param name="num">index ID</param>
        /// <returns>index objects contains the details of wanted index</returns>
        public static GeniricIndex select(int num, string tableName)
        {
            GeniricIndex mt = new GeniricIndex();
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("SELECT * FROM " + tableName + " WHERE NUM= " + num, conn);
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    #region Get Data
                    if (!dr.IsDBNull(0))
                        mt.num = int.Parse(dr.GetValue(0).ToString());
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
        /// Gets the details of a defined idex from database based on its name
        /// </summary>
        /// <param name="name">index's name</param>
        /// <returns> objects contains the details of wanted index</returns>
        public static GeniricIndex SelectByName(string name, string tableName)
        {
            GeniricIndex mt = new GeniricIndex();
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("SELECT * FROM " + tableName + " WHERE NAME='" + name + "'", conn);
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    #region Get Data
                    if (!dr.IsDBNull(0))
                        mt.num = int.Parse(dr.GetValue(0).ToString());
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
        /// Edits index details in database
        /// </summary>
        /// <param name="mt"> object contains the new index details</param>
        /// <returns>exception message if there is any, empty string if none</returns>
        public static string Edit(GeniricIndex mt, string tableName)
        {

            string res = "";
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                string qr = "  Update " + tableName + " Set " +
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
    }
}