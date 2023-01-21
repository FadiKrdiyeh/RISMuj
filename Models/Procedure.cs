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
    /// Class of Procedure
    /// </summary>
    public class Procedure
    {
        /// <summary>
        /// proceduse ID, primary key of procedure table in database
        /// </summary>
        public int num { set; get; }

        /// <summary>
        /// procedure name
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "procedureName")]
        [Required(ErrorMessageResourceType = typeof(Resources.Res), ErrorMessageResourceName = "EmptyProcedueError")]
        public string name { set; get; }

        /// <summary>
        /// procedure code
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "procedureCode")]
        public string code { set; get; }

        /// <summary>
        /// procedure's english name
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "procEnName")]
        public string englishName { set; get; }

        /// <summary>
        /// procedure constructor
        /// </summary>
        public Procedure () {}

        /// <summary>
        /// Inserts a new procedure into procedure table in database
        /// </summary>
        /// <param name="mt">procedure object contains the procedre information</param>
        /// <returns>exception message if there is any, empty string if none</returns>
        public static string addProcedure(Procedure mt)
        {
            string res = "";
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                string qr = "Begin" +
                            "  insert into PROCEDUREDESCRIPTION" +

                            "( NUM, NAME, CODE, ENGLISHNAME) " +
                            " values " +
                            " (:NUM, :NAME, :CODE, :ENGLISHNAME); " +
                            "End;";
                OracleCommand cmd = new OracleCommand(qr, conn);
                OracleParameter[] param =  {
                                            new OracleParameter("NUM", mt.num),
                                            new OracleParameter("NAME", mt.name),
                                            new OracleParameter("CODE", mt.code),
                                            new OracleParameter("ENGLISHNAME", mt.englishName)
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
        /// Deletes a defined procedure information from database
        /// </summary>
        /// <param name="num">procedure ID</param>
        /// <returns>exception message if there is any, empty string if none</returns>
        public static string deleteProcedure (int num)
        {
            string res = "";
            int count = 0;
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("SELECT COUNT(NUM) FROM ORDERS WHERE PROCEDUREID = " + num, conn);
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    if (!dr.IsDBNull(0))
                        count = Int32.Parse(dr.GetValue(0).ToString());
                }

                if (count == 0)
                {
                    cmd = new OracleCommand("DELETE PROCEDUREDESCRIPTION WHERE NUM = :NUM ", conn);
                    cmd.Parameters.Add(new OracleParameter("NUM", num));
                    cmd.ExecuteNonQuery();
                    cmd = new OracleCommand("DELETE PROCEDURETOMODALITY WHERE PROCEDUREID = :PROCEDUREID ", conn);
                    cmd.Parameters.Add(new OracleParameter("PROCEDUREID", num));
                    cmd.ExecuteNonQuery();
                }
                else
                    res = RIS.Resources.Res.CantDeleteProcedure;
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
        /// Edits a defined procedure information in database
        /// </summary>
        /// <param name="mt">procedure object contains the procedure information</param>
        /// <returns>exception message if there is any, empty string if none</returns>
        public static string editProcedure(Procedure mt)
        {
            string res = "";
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                string qr = "  Update PROCEDUREDESCRIPTION Set " +
                    " NAME = :NAME, " +
                    " CODE = :CODE, "+
                    "ENGLISHNAME = :ENGLISHNAME " +
                    "WHERE NUM=:NUM ";
                OracleParameter[] param =  {
                                            new OracleParameter("NAME", mt.name),
                                            new OracleParameter("CODE", mt.code),
                                             new OracleParameter("ENGLISHNAME", mt.englishName),
                                            new OracleParameter("NUM", mt.num)
                                           
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
        /// Gets a list of all procedures from database
        /// </summary>
        /// <returns>list of all procedures</returns>
        public static List<Procedure> getAll()
        {
            List<Procedure> proList = new List<Procedure>();

            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("SELECT * FROM PROCEDUREDESCRIPTION ", conn);
                OracleDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {

                    Procedure pro = new Procedure();
                    #region Data
                    if (!dr.IsDBNull(0))
                        pro.num = Int32.Parse(dr.GetValue(0).ToString());
                    if (!dr.IsDBNull(1))
                        pro.name = dr.GetString(1);
                    if (!dr.IsDBNull(2))
                        pro.code = dr.GetString(2);
                    if (!dr.IsDBNull(3))
                        pro.englishName = dr.GetString(3);
                    #endregion
                    proList.Add(pro);
                }
            }
            catch
            {
            }
            finally
            {
                conn.Close();
            }
            return proList;
        }

        /// <summary>
        /// Gets a selectlist of all procedures from database
        /// </summary>
        /// <param name="withAllOption">to add an empty option to the selectlist</param>
        /// <param name="defaultValue">setting the default value of the selectlist</param>
        /// <returns>a selectlist contains all procedures</returns>
        public static SelectList GetProceduresList(bool withAllOption, string defaultValue)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            if (withAllOption)
                items.Add(new SelectListItem { Text = "", Value = "" });
            foreach (var item in Procedure.getAll())
            {
                items.Add(new SelectListItem { Text = item.name, Value = item.num.ToString() });
            }
            return new SelectList(items, "Value", "Text", string.IsNullOrEmpty(defaultValue) ? "0" : defaultValue);
        }

        /// <summary>
        /// Gets all procedures codes from database as a selectlist
        /// </summary>
        /// <param name="withAllOption">to add an empty option to the selectlist</param>
        /// <param name="defaultValue">setting the default value of the selectlist</param>
        /// <returns>selectlist of all procedures codes</returns>
        public static SelectList GetProcedureCodes(bool withAllOption, string defaultValue)
        {
            List<SelectListItem> items = new List<SelectListItem>();

            if (withAllOption)
                items.Add(new SelectListItem { Text = "", Value = "" });

            foreach (var item in Procedure.getAll())
            {
                items.Add(new SelectListItem { Text = item.name, Value = item.code });
            }
            return new SelectList(items, "Value", "Text", string.IsNullOrEmpty(defaultValue) ? "0" : defaultValue);
        }

        /// <summary>
        /// Gets a defined procedure information from database based on procedure ID
        /// </summary>
        /// <param name="num">the procedure ID</param>
        /// <returns>procedure object contains the wanted procedure information</returns>
        public static Procedure select(int num)
        {
            Procedure u = new Procedure();

            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());

            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("SELECT * FROM PROCEDUREDESCRIPTION WHERE NUM= " + num, conn);
                OracleDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    if (!dr.IsDBNull(0))
                        u.num = Int32.Parse(dr.GetValue(0).ToString());
                    if (!dr.IsDBNull(1))
                        u.name = dr.GetValue(1).ToString();
                    if (!dr.IsDBNull(2))
                        u.code = dr.GetValue(2).ToString();
                    if (!dr.IsDBNull(3))
                        u.englishName = dr.GetValue(3).ToString();
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
        /// Gets a defined procedure information from database based on procedure code
        /// </summary>
        /// <param name="code">the procedure code</param>
        /// <returns>procedure object contains the wanted procedure information</returns>
        public static Procedure selectByCode(String code)
        {
            Procedure u = new Procedure();

            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());

            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("SELECT * FROM PROCEDUREDESCRIPTION WHERE CODE= '" + code+"'", conn);
                OracleDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    if (!dr.IsDBNull(0))
                        u.num = Int32.Parse(dr.GetValue(0).ToString());
                    if (!dr.IsDBNull(1))
                        u.name = dr.GetValue(1).ToString();
                    if (!dr.IsDBNull(2))
                        u.code = dr.GetValue(2).ToString();
                    if (!dr.IsDBNull(3))
                        u.englishName = dr.GetValue(3).ToString();
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


    }
}