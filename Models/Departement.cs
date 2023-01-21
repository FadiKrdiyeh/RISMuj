using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Oracle.DataAccess.Client;
using RISDB;

namespace RIS.Models
{
    /// <summary>
    /// This class for department
    /// </summary>
    public class Departement
    {
        /// <summary>
        /// Department ID, Primary key for department table in database
        /// </summary>
        public int num { set; get; }

        /// <summary>
        /// Department name
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "depName")]
        [Required(ErrorMessageResourceType = typeof(Resources.Res), ErrorMessageResourceName = "depReq")]
        public string name { set; get; }

        /// <summary>
        /// Department object constructor
        /// </summary>
        public Departement() { }

        /// <summary>
        /// Get all departments from database
        /// </summary>
        /// <returns>Liast of all departments in database</returns>
        public static List<Departement> getData()
        {
            List<Departement> depList = new List<Departement>();

            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());

           
            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("SELECT * FROM DEPARTMENT ", conn);
                OracleDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Departement dep = new Departement();
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
        /// Inserts a department into department table in database
        /// </summary>
        /// <param name="mt">department object contains department details</param>
        /// <returns>exception message if there is any, empty string if not</returns>
        public static string Insert(Departement mt)
        {
            Departement checkIfExists = SelectByName(mt.name);
            if (checkIfExists != null)
                return "";
            string res = "";
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                string qr = "Begin" +
                            "  insert into DEPARTMENT" +

                            "( NUM, NAME) " +
                            " values " +
                            " (:NUM, :NAME); " +
                            "End;";
                OracleCommand cmd = new OracleCommand(qr, conn);
                OracleParameter[] param =  {
                                            new OracleParameter("NUM", mt.num),
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
        /// Gets departments as selectlist
        /// </summary>
        /// <param name="withAllOption">to add an empty value</param>
        /// <param name="defaultValue">default value of selectlist</param>
        /// <returns>select list contains all departments</returns>
        public static SelectList GetDepartementList(bool withAllOption, string defaultValue)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            if (withAllOption)
                items.Add(new SelectListItem { Text = "", Value = "" });
            foreach (var item in Departement.getData())
            {
                items.Add(new SelectListItem { Text = item.name, Value = item.num.ToString() });
            }
            return new SelectList(items, "Value", "Text", string.IsNullOrEmpty(defaultValue) ? "0" : defaultValue);
        }
        

        /// <summary>
        /// Gets all departments names
        /// </summary>
        /// <returns>List of strings containing departments names</returns>
        public static List<string> depListNames()
        {
            List<string> items = new List<string>();
            foreach (var item in Departement.getData())
            {
                items.Add( item.name );
            }
            return items;
        }


        /// <summary>
        /// Gets departments of a defined user
        /// </summary>
        /// <param name="withAllOption"></param>
        /// <param name="depID">the user department</param>
        /// <param name="defaultValue"></param>
        /// <returns>select list containing the user department</returns>
        public static SelectList GetDepartementListofTheuser(bool withAllOption,string depID, string defaultValue)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            //if (withAllOption)
            //    items.Add(new SelectListItem { Text = "", Value = "" });
            foreach (var item in Departement.getData())
            {
                if(item.num.ToString()==depID)
                items.Add(new SelectListItem { Text = item.name, Value = item.num.ToString() });
            }
          
            return new SelectList(items, "Value", "Text", string.IsNullOrEmpty(defaultValue) ? "0" : defaultValue);
        }

        /// <summary>
        /// Gets departments names as a select list
        /// </summary>
        /// <param name="withAllOption">to add an empty value to the select list</param>
        /// <param name="defaultValue">setting selectlist default value</param>
        /// <returns></returns>
        public static SelectList GetDepartementListNames(bool withAllOption,string defaultValue)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            if (withAllOption)
                items.Add(new SelectListItem { Text = Resources.Res.All, Value = "0" });
            foreach (var item in Departement.getData())
            {
                items.Add(new SelectListItem { Text = item.name, Value = item.num.ToString() });
            }
            
            return new SelectList(items, "Value", "Text", string.IsNullOrEmpty(defaultValue) ? "0" : defaultValue);
        }

        /// <summary>
        /// Deletes a department from database
        /// </summary>
        /// <param name="i">department ID</param>
        /// <returns>exception message if there is any, empty string if none</returns>
        public static string Delete(int i)
        {
            string res = "";
            int count = 0;
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("SELECT COUNT(NUM) FROM ORDERS WHERE DEPTNAME = " + i, conn);
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    if (!dr.IsDBNull(0))
                        count = Int32.Parse(dr.GetValue(0).ToString());
                }

                cmd = new OracleCommand("SELECT COUNT(NUM) FROM LOGGEDUSER WHERE DEPARTEMENT = '" + i +"'", conn);
                dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    if (!dr.IsDBNull(0))
                        count += Int32.Parse(dr.GetValue(0).ToString());
                }

                cmd = new OracleCommand("SELECT COUNT(NUM) FROM MODALITY WHERE DEPARTEMENT = '" + i + "'", conn);
                dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    if (!dr.IsDBNull(0))
                        count += Int32.Parse(dr.GetValue(0).ToString());
                }

                if (count == 0)
                {
                    cmd = new OracleCommand("DELETE DEPARTMENT WHERE NUM = :NUM ", conn);
                    cmd.Parameters.Add(new OracleParameter("NUM", i));
                    cmd.ExecuteNonQuery();
                }
                else
                    res = RIS.Resources.Res.CantDeleteDepartment;
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
        /// Gets the details of a defined department from database based on its ID
        /// </summary>
        /// <param name="num">department ID</param>
        /// <returns>department objects contains the details of wanted department</returns>
        public static Departement select(int num)
        {
            Departement mt = new Departement();
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("SELECT * FROM DEPARTMENT WHERE NUM= " + num, conn);
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
        /// Gets the details of a defined department from database based on its name
        /// </summary>
        /// <param name="name">department's name</param>
        /// <returns>department objects contains the details of wanted department</returns>
        public static Departement SelectByName(string name)
        {
            Departement mt = new Departement();
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("SELECT * FROM DEPARTMENT WHERE NAME='" + name+"'", conn);
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
        /// Edits department details in database
        /// </summary>
        /// <param name="mt">department object contains the new department details</param>
        /// <returns>exception message if there is any, empty string if none</returns>
        public static string Edit(Departement mt)
        {

            string res = "";
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                string qr = "  Update DEPARTMENT Set " +
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