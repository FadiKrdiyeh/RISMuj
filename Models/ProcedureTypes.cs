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
    public class ProcedureTypes
    {

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
        public int parentNum { set; get; }

        /// <summary>
        /// procedure's english name
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "procEnName")]
        public string englishName { set; get; }

        /// <summary>
        /// procedure constructor
        /// </summary>
        public ProcedureTypes() { }


        /// <summary>
        /// Inserts a new procedure into procedure table in database
        /// </summary>
        /// <param name="mt">procedure object contains the procedre information</param>
        /// <returns>exception message if there is any, empty string if none</returns>
        public static string addProcedure(ProcedureTypes mt)
        {
            string res = "";
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                string qr = "Begin" +
                            "  insert into PROCEDURETYPES" +

                            "( NUM, PARENTNUM, NAME, ENGLISHNAME) " +
                            " values " +
                            " (:NUM, :PARENTNUM, :NAME, :ENGLISHNAME); " +
                            "End;";
                OracleCommand cmd = new OracleCommand(qr, conn);
                OracleParameter[] param =  {
                                            new OracleParameter("NUM", mt.num),
                                            new OracleParameter("PARENTNUM", mt.parentNum),
                                            new OracleParameter("NAME", mt.name),
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
        public static string deleteProcedure(int num)
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
                    cmd = new OracleCommand("DELETE PROCEDURETYPES WHERE NUM = :NUM ", conn);
                    cmd.Parameters.Add(new OracleParameter("NUM", num));
                    cmd.ExecuteNonQuery();
                    //cmd = new OracleCommand("DELETE PROCEDURETOMODALITY WHERE PROCEDUREID = :PROCEDUREID ", conn);
                    //cmd.Parameters.Add(new OracleParameter("PROCEDUREID", num));
                    //cmd.ExecuteNonQuery();
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
                string qr = "  Update PROCEDURETYPES Set " +
                    " NAME = :NAME, " +
                    "ENGLISHNAME = :ENGLISHNAME " +
                    "WHERE NUM=:NUM ";
                OracleParameter[] param =  {
                                            new OracleParameter("NAME", mt.name),
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
        public static List<ProcedureTypes> getAll()
        {
            List<ProcedureTypes> proList = new List<ProcedureTypes>();

            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("SELECT * FROM PROCEDURETYPES ", conn);
                OracleDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {

                    ProcedureTypes pro = new ProcedureTypes();
                    #region Data
                    if (!dr.IsDBNull(0))
                        pro.num = Int32.Parse(dr.GetValue(0).ToString());
                    if (!dr.IsDBNull(1))
                        pro.parentNum = Int32.Parse(dr.GetValue(1).ToString());
                    if (!dr.IsDBNull(2))
                        pro.name = dr.GetString(2);
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



        public static List<ProcedureTypes> getAllByParent(int num)
        {
            List<ProcedureTypes> proList = new List<ProcedureTypes>();

            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("SELECT * FROM PROCEDURETYPES where PARENTNUM="+num, conn);
                OracleDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {

                    ProcedureTypes pro = new ProcedureTypes();
                    #region Data
                    if (!dr.IsDBNull(0))
                        pro.num = Int32.Parse(dr.GetValue(0).ToString());
                    if (!dr.IsDBNull(1))
                        pro.parentNum = Int32.Parse(dr.GetValue(1).ToString());
                    if (!dr.IsDBNull(2))
                        pro.name = dr.GetString(2);
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
            foreach (var item in ProcedureTypes.getAll())
            {
                items.Add(new SelectListItem { Text = item.name, Value = item.num.ToString() });
            }
            return new SelectList(items, "Value", "Text", string.IsNullOrEmpty(defaultValue) ? "0" : defaultValue);
        }


        public static SelectList GetProceduresListByParent(int num,bool withAllOption, string defaultValue)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            if (withAllOption)
                items.Add(new SelectListItem { Text = "", Value = "" });
            foreach (var item in ProcedureTypes.getAllByParent(num))
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

        /// <summary>
        /// Gets a defined procedure information from database based on procedure ID
        /// </summary>
        /// <param name="num">the procedure ID</param>
        /// <returns>procedure object contains the wanted procedure information</returns>
        public static ProcedureTypes select(int num)
        {
            ProcedureTypes u = new ProcedureTypes();

            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());

            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("SELECT * FROM PROCEDURETYPES WHERE NUM= " + num, conn);
                OracleDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    if (!dr.IsDBNull(0))
                        u.num = Int32.Parse(dr.GetValue(0).ToString());
                    if (!dr.IsDBNull(1))
                        u.parentNum = Int32.Parse(dr.GetValue(1).ToString());
                    if (!dr.IsDBNull(2))
                        u.name = dr.GetValue(2).ToString();
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



    }
}