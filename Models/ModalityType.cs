using Oracle.DataAccess.Client;
using RISDB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Web;
using System.Web.Mvc;

namespace RIS.Models
{
    /// <summary>
    /// Class of ModalityType
    /// </summary>
    public class ModalityType
    {
        #region Attributes
      
        /// <summary>
        /// ModalityType ID, primary key of ModalitType table in database
        /// </summary>
        public int num { set; get; }

        /// <summary>
        /// ModalityType name
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "ModalityTypename")]
        [Required(ErrorMessageResourceType = typeof(Resources.Res), ErrorMessageResourceName = "ModalityTypenameError")]
        public string name { set; get; }

        #endregion
        #region Functions

        /// <summary>
        /// ModalityType Constructor
        /// </summary>
        public  ModalityType(){}

        /// <summary>
        /// Gets all modaltiy types listed in database
        /// </summary>
        /// <returns>list of all modality types</returns>
        public static List<ModalityType> getData()
        {
            List<ModalityType> mtList = new List<ModalityType>();
            
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("SELECT * FROM MODALITYTYPE", conn);
                OracleDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    ModalityType mt = new ModalityType();
                    #region Data
                    if (!dr.IsDBNull(0))
                        mt.num = int.Parse(dr.GetValue(0).ToString());
                    if (!dr.IsDBNull(1))
                        mt.name = dr.GetString(1);
                    #endregion
                    mtList.Add(mt);
                }
            }
            catch
            {
            }
            finally
            {
                conn.Close();
            }
            return mtList;
        }

        /// <summary>
        /// Gets a defined modality type information from database by its ID
        /// </summary>
        /// <param name="num">the wanted modality type ID</param>
        /// <returns>ModalityType object contains the wanted modality type information</returns>
        public static ModalityType Select(int num)
        {
            ModalityType mt = new ModalityType();
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("SELECT * FROM MODALITYTYPE WHERE ID= " + num , conn);
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
        /// Gets a defined modality type information from database by its name
        /// </summary>
        /// <param name="name">the wanted modality type name</param>
        /// <returns>ModalityType object contains the wanted modality type information</returns>
        public static ModalityType SelectByName(string name)
        {
            ModalityType mt = null;
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("SELECT * FROM MODALITYTYPE WHERE NAME='" + name+"'", conn);
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
        /// Inserts a new modality type information into database
        /// </summary>
        /// <param name="mt">modalityType object contains the modality type information</param>
        /// <returns>exception message if there is any, empty string if none</returns>
        public static string Insert(ModalityType mt)
        {
            ModalityType checkIfExists = SelectByName(mt.name);
            if (checkIfExists != null)
                return "";
            string res = "";
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                string qr = "Begin" +
                            "  insert into MODALITYTYPE" +
                            "( ID, NAME)" +
                            "values " +
                            "(:ID,:NAME);" +
                            "End;";
                OracleCommand cmd = new OracleCommand(qr, conn);
                OracleParameter[] param =  {
                                            new OracleParameter("ID", mt.num),
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
        /// Gets all modality types from database as a selectlist
        /// </summary>
        /// <param name="defaultValue">to add an empty option to the selectlist</param>
        /// <returns>selectlist of all modality types</returns>
        public static SelectList GetModalityTypesList(string defaultValue)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "", Value = "" });
            foreach (var item in ModalityType.getData())
            {
                items.Add(new SelectListItem { Text = item.name, Value = item.num.ToString() });
            }
            return new SelectList(items, "Value", "Text", string.IsNullOrEmpty(defaultValue) ? "0" : defaultValue);
        }

        /// <summary>
        /// Deletes a modality type information from database
        /// </summary>
        /// <param name="num">modality type ID</param>
        /// <returns>exception message if there is any, empty string if none</returns>
        public static string Delete(int num)
        {
            string res = "";
            int count = 0;
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("SELECT COUNT(NUM) FROM MODALITY WHERE TYPE = " + num, conn);
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    if (!dr.IsDBNull(0))
                        count = Int32.Parse(dr.GetValue(0).ToString());
                }

                if (count == 0)
                {
                    cmd = new OracleCommand("DELETE MODALITYTYPE WHERE ID = :ID ", conn);
                    cmd.Parameters.Add(new OracleParameter("NUM", num));
                    cmd.ExecuteNonQuery();
                }
                else
                    res = RIS.Resources.Res.CantDeleteModalityType;
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
        /// Edits a defined modality type information in database
        /// </summary>
        /// <param name="mt">modalityType object contains the new information</param>
        /// <returns>exception message if there is any, empty string if none</returns>
        public static string Edit(ModalityType mt)
            {

            string res = "";
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                string qr = "  Update MODALITYTYPE Set " +
                            "  NAME =:NAME " +
                            "  where ID =:ID";
                OracleParameter[] param =  {
                                                new OracleParameter("NAME", mt.name),
                                                new OracleParameter("ID", mt.num),
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
        #endregion
    }
}