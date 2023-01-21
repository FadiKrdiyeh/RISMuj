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
    /// Class of modality
    /// </summary>
    public class Modality
    {
        #region Attributes
      
        /// <summary>
        /// modality ID, primary key of modality table in database
        /// </summary>
        public int num { set; get; }
      
        /// <summary>
        /// modality name
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "Modalityname")]
        [Required(ErrorMessageResourceType = typeof(Resources.Res), ErrorMessageResourceName = "ModalitynameError")]
        public string name { set; get; }
        /// <summary>
        /// ModaliteQcode
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "ModalityQcode")]
        [Required(ErrorMessageResourceType = typeof(Resources.Res), ErrorMessageResourceName = "ModalityQcodeError")]
        public string qcode { set; get; }

        /// <summary>
        /// modality AETitle
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "AeTitle")]
        [Required(ErrorMessageResourceType = typeof(Resources.Res), ErrorMessageResourceName = "AeTitleError")]
        public string aeTitle { set; get; }
      
        /// <summary>
        /// IP address of the modality
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "ipAddress")]
        public string ipAddress { set; get; }

        /// <summary>
        /// modality port
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "port")]
        public int port { set; get; }
     
        /// <summary>
        /// the ID of modality type
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "ModalityTypename")]
        [Required(ErrorMessageResourceType = typeof(Resources.Res), ErrorMessageResourceName = "ModalityTypenameError")]
        public int type { set; get; }
     
        /// <summary>
        /// modality description
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "description")]
        public string description { set; get; }

        /// <summary>
        /// the ID of the department containing the modality
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "depName")]
        [Required(ErrorMessageResourceType = typeof(Resources.Res), ErrorMessageResourceName = "ModalityDepNameError")]
        public string departement { set; get; }

        /// <summary>
        /// ModaltyType object contains the modality type information
        /// </summary>
        public ModalityType parentMT
        {
            get
            {
                return ModalityType.Select(type);
            }
        }

        /// <summary>
        /// Department object contains the department information
        /// </summary>
        public Departement modalityDepartement
        {
            get
            {
                if (departement !=null)
                    return Departement.select(int.Parse(departement));
                else return null;

            }
        }


        #endregion
        #region Functions

        /// <summary>
        /// Modality object constructor
        /// </summary>
        public Modality() { }
        public static int getModalityCost(int modId)
        {
            int res = 0;
            string qr = "SELECT MODALITY.COST from MODALITY where num=" + modId;

            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());

            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand(qr, conn);
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                    res = int.Parse(dr.GetValue(0).ToString());

            }
            catch (Exception e)
            {
                conn.Close();
            }
            conn.Close();

            return res;
        }
        public static string getModalityQcode(int modId)
        {
            string res = "";
            string qr = "SELECT MODALITY.QCODE from MODALITY where num=" + modId;

            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());

            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand(qr, conn);
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                    res = dr.GetValue(0).ToString();

            }
            catch (Exception e)
            {
                conn.Close();
            }
            conn.Close();

            return res;
        }

        /// <summary>
        /// Gets modalities list from database
        /// </summary>
        /// <returns>list of modality contains all modalities information</returns>
        public static List<Modality> getData()
        {
            List<Modality> mtList = new List<Modality>();

            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("SELECT * FROM MODALITY ", conn);
                OracleDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {

                    Modality mt = new Modality();
                    #region Data
                    if (!dr.IsDBNull(0))
                        mt.num = Int32.Parse(dr.GetValue(0).ToString());
                    if (!dr.IsDBNull(1))
                        mt.name = dr.GetString(1);
                    if (!dr.IsDBNull(2))
                        mt.aeTitle = dr.GetString(2);
                    if (!dr.IsDBNull(3))
                        mt.ipAddress = dr.GetString(3);
                    if (!dr.IsDBNull(4))
                        mt.port = Int32.Parse(dr.GetValue(4).ToString());
                    if (!dr.IsDBNull(5))
                        mt.type = Int32.Parse(dr.GetValue(5).ToString());
                    if (!dr.IsDBNull(6))
                        mt.description = dr.GetString(6);
                    if (!dr.IsDBNull(7))
                        mt.departement = dr.GetString(7);
                    if (!dr.IsDBNull(8))
                        mt.qcode = dr.GetString(8);
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
        /// Gets the details of a defined modality from database by its ID
        /// </summary>
        /// <param name="num">modality ID</param>
        /// <returns>modality object contains the wanted modality details</returns>
        public static Modality Select(int num)
        {
            Modality mt = new Modality();
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("SELECT * FROM MODALITY WHERE NUM= " + num, conn);
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    #region Get Data
                    if (!dr.IsDBNull(0))
                        mt.num = int.Parse(dr.GetValue(0).ToString());
                    if (!dr.IsDBNull(1))
                        mt.name = dr.GetString(1);
                    if (!dr.IsDBNull(2))
                        mt.aeTitle = dr.GetString(2);
                    if (!dr.IsDBNull(3))
                        mt.ipAddress = dr.GetString(3);
                    if (!dr.IsDBNull(4))
                        mt.port = int.Parse(dr.GetValue(4).ToString());
                    if (!dr.IsDBNull(5))
                        mt.type = int.Parse(dr.GetValue(5).ToString());
                    if (!dr.IsDBNull(6))
                        mt.description = dr.GetString(6);
                    if (!dr.IsDBNull(7))
                        mt.departement = dr.GetString(7);
                    if (!dr.IsDBNull(8))
                        mt.qcode = dr.GetString(8);
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
        /// Gets the details of a defined modality from database by its AETitle
        /// </summary>
        /// <param name="ae">modality AETitle</param>
        /// <returns>modality object contains the wanted modality details</returns>
        public static Modality SelectByAeTitle(string ae)
        {
            Modality mt = null;
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("SELECT * FROM MODALITY WHERE AETITLE='" + ae+"'", conn);
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    #region Get Data
                    if (!dr.IsDBNull(0))
                        mt.num = int.Parse(dr.GetValue(0).ToString());
                    if (!dr.IsDBNull(1))
                        mt.name = dr.GetString(1);
                    if (!dr.IsDBNull(2))
                        mt.aeTitle = dr.GetString(2);
                    if (!dr.IsDBNull(3))
                        mt.ipAddress = dr.GetString(3);
                    if (!dr.IsDBNull(4))
                        mt.port = int.Parse(dr.GetValue(4).ToString());
                    if (!dr.IsDBNull(5))
                        mt.type = int.Parse(dr.GetValue(5).ToString());
                    if (!dr.IsDBNull(6))
                        mt.description = dr.GetString(6);
                    if (!dr.IsDBNull(7))
                        mt.departement = dr.GetString(7);
                    if (!dr.IsDBNull(8))
                        mt.qcode = dr.GetString(8);
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
        /// Inserts a new modality details into database
        /// </summary>
        /// <param name="mt">modality object contains the modality details</param>
        /// <returns>exception string if there is any, empty string if none</returns>
        public static string Insert(Modality mt)
        {
            Modality checkIfExsists = SelectByAeTitle(mt.aeTitle);
            if(checkIfExsists!=null)
            {
                return "هنالك آلة تصوير أخرى لها نفس تطبيق الآلة";
            }
            string res = "";
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                string qr = "Begin" +
                            "  insert into MODALITY" +

                            "( NUM, NAME, AETITLE, IPADDRESS, PORT, TYPE, DESCRIPTION, DEPARTEMENT, QCODE) " +
                            " values " +
                            " (:NUM, :NAME, :AETITLE, :IPADDRESS, :PORT, :TYPE, :DESCRIPTION,:DEPARTEMENT, :QCODE); " +
                            "End;";
                OracleCommand cmd = new OracleCommand(qr, conn);
                OracleParameter[] param =  {
                                            new OracleParameter("NUM", mt.num),
                                            new OracleParameter("NAME", mt.name),
                                            new OracleParameter("AETITLE", mt.aeTitle),
                                            new OracleParameter("IPADDRESS", mt.ipAddress),
                                            new OracleParameter("PORT", mt.port),
                                            new OracleParameter("TYPE", mt.type),
                                            new OracleParameter("DESCRIPTION", mt.description),
                                            new OracleParameter("DEPARTEMENT",mt.departement),
                                            new OracleParameter("QCODE",mt.qcode)

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
        /// Gets all modalities from database as a selectlist
        /// </summary>
        /// <param name="withAllOption">to add an empty option to the selectlist</param>
        /// <param name="defaultValue">setting the default value in the selectlist</param>
        /// <returns>a select list contains all the modalities</returns>
        public static SelectList GetModalitysList(bool withAllOption, string defaultValue)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            if (withAllOption)
                items.Add(new SelectListItem { Text = "", Value = "" });
            foreach (var item in Modality.getData())
            {
                items.Add(new SelectListItem { Text = item.name, Value = item.num.ToString() });
            }
            return new SelectList(items, "Value", "Text", string.IsNullOrEmpty(defaultValue) ? "0" : defaultValue);
        }

        /// <summary>
        /// Gets all modalities names from database
        /// </summary>
        /// <returns>List of modalities nemes</returns>
        public static List<string> modListNames()
        {
            List<string> items = new List<string>();

            foreach (var item in Modality.getData())
            {
                items.Add(item.name);
            }
            return items;
        }

        /// <summary>
        /// Deletes a modality from database
        /// </summary>
        /// <param name="num">modality ID</param>
        /// <returns>exception string if there is any, empty string if none</returns>
        public static string Delete(int num)
        {
            string res = "";
            int count = 0;
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("SELECT COUNT(NUM) FROM ORDERS WHERE MODALITYID = " + num, conn);
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    if (!dr.IsDBNull(0))
                        count = Int32.Parse(dr.GetValue(0).ToString());
                }

                if (count == 0)
                {
                    cmd = new OracleCommand("DELETE MODALITY WHERE NUM = :NUM ", conn);
                    cmd.Parameters.Add(new OracleParameter("NUM", num));
                    cmd.ExecuteNonQuery();
                }
                else
                    res = RIS.Resources.Res.CantDeleteModality;
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
        /// Edits modality details in database
        /// </summary>
        /// <param name="mt">modality object contains the new details</param>
        /// <returns>exception string if there is any, empty string if none</returns>
        public static string Edit(Modality mt)
        {

            string res = "";
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                string qr = "  Update MODALITY Set " +
                    "NAME = :NAME, " +
                    "AETITLE = :AETITLE, " +
                    "IPADDRESS = :IPADDRESS, " +
                    "PORT = :PORT, " +
                    "TYPE = :TYPE, " +
                    "DESCRIPTION = :DESCRIPTION, " +
                    "DEPARTEMENT= :DEPARTEMENT, " +
                    "QCODE= :QCODE" +
                            "  where NUM = :NUM ";
                OracleParameter[] param =  {
                                            new OracleParameter("NAME", mt.name),
                                            new OracleParameter("AETITLE", mt.aeTitle),
                                            new OracleParameter("IPADDRESS", mt.ipAddress),
                                            new OracleParameter("PORT", mt.port),
                                            new OracleParameter("TYPE", mt.type),
                                            new OracleParameter("DESCRIPTION", mt.description),
                                            new OracleParameter("DEPARTEMENT",mt.departement),
                                            new OracleParameter("QCODE",mt.qcode),
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
        /// Gets the modalities of a defined department
        /// </summary>
        /// <param name="withAllOption">to add an empty option to the </param>
        /// <param name="depID">the department ID</param>
        /// <param name="defaultValue">setting the default value of the selectlist</param>
        /// <returns>selectlist contains all modalities in a defined department</returns>
        public static SelectList GetModalitysListByDepId(bool withAllOption, string depID,string defaultValue)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            if (withAllOption)
                items.Add(new SelectListItem { Text = "", Value = "" });
            foreach (var item in Modality.getData())
            {
                if (item.departement==depID)
                items.Add(new SelectListItem { Text = item.name, Value = item.num.ToString() });
            }
            return new SelectList(items, "Value", "Text", string.IsNullOrEmpty(defaultValue) ? "0" : defaultValue);
        }

        /// <summary>
        /// Gets the modalities of a defined department
        /// </summary>
        /// <param name="depID">the department ID</param>
        /// <returns>list contains all modalities in a defined department</returns>
        public static List<Modality> GetModalitysListByDepId(string depID)
        {
            List<Modality> items = new List<Modality>();
            foreach (var item in Modality.getData())
            {
                if (item.departement == depID)
                    items.Add(item);
            }
            return items;
        }
        #endregion
    }
}
