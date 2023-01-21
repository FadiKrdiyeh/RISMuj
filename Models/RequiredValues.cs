using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Oracle.DataAccess.Client;
using RISDB;

namespace RIS.Models
{
    /// <summary>
    /// Models Required Attributes Class, Required Attributes are directed by the system administrator
    /// </summary>
    public class RequiredValues : IEquatable<RequiredValues>
    {

        /// <summary>
        /// RequiredValue ID, primary key of requiredvalues table in database
        /// </summary>
        public int num { get; set; }

        /// <summary>
        /// Boolean represent if the attribute is required or not
        /// </summary>
        public Boolean requiredVal { get; set; }

        /// <summary>
        /// Number of objects in each view defined by the system administrator
        /// </summary>
        public int reqRowsPerPage { get; set; }

        /// <summary>
        /// attribute name
        /// </summary>
        public string value { set; get; }

        /// <summary>
        /// RequiredValues constructor
        /// </summary>
        public RequiredValues() { }

        /// <summary>
        /// RequiredValues constructor
        /// </summary>
        /// <param name="i">Attribute ID</param>
        /// <param name="n">Attribute name</param>
        public RequiredValues(int i, string n)
        {
            this.num = i;
            this.value = n;
        }

        /// <summary>
        /// RequiredValues constructor
        /// </summary>
        /// <param name="i">Attribute ID</param>
        /// <param name="n">Attribute name</param>
        /// <param name="req">number of objects in each view</param>
        public RequiredValues(int i, string n, int req)
        {
            this.num = i;
            this.value = n;
            this.reqRowsPerPage = req;
        }

        /// <summary>
        /// Checks if an Attribute equals other Attribute
        /// </summary>
        /// <param name="other">RequiredValue object to check equality with</param>
        /// <returns>true if they were equal, false if not</returns>
        public bool Equals(RequiredValues other)
        {
            return this.num == other.num && this.value == other.value && this.requiredVal == other.requiredVal;
        }


        /// <summary>
        /// Gets a list of all Attributes in database (required and not required)
        /// </summary>
        /// <returns>RequiredValues list contains all Attributes</returns>
        public static List<RequiredValues> getRequiredValuessList()
        {
            List<RequiredValues> gList = new List<RequiredValues>();

            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("SELECT * FROM REQUIREDVALUES where NUM <> " + (int)ReqPatientVals.rowsPerPage + " ORDER BY NUM", conn);
                OracleDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    RequiredValues mt = new RequiredValues();
                    #region Data
                    if (!dr.IsDBNull(0))
                        mt.num = int.Parse(dr.GetValue(0).ToString());
                    if (!dr.IsDBNull(1))
                        mt.value = dr.GetString(1);
                    if (!dr.IsDBNull(2))
                        mt.requiredVal = (int.Parse(dr.GetValue(2).ToString()) == 0) ? false : true;
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
        /// Gets a list of required Attributes only
        /// </summary>
        /// <returns>RequiredValues list contains all required Attributes</returns>
        public static List<RequiredValues> getActuallyRequiredValuessList()
        {
            List<RequiredValues> gList = new List<RequiredValues>();

            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("SELECT * FROM REQUIREDVALUES WHERE REQUIRED = 1 AND VALUE != 'middlename' AND VALUE != 'mothername' AND VALUE != 'age' AND VALUE != 'birthdate' AND VALUE != 'nationalidnumber' ", conn);
                OracleDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    RequiredValues mt = new RequiredValues();
                    #region Data
                    if (!dr.IsDBNull(0))
                        mt.num = int.Parse(dr.GetValue(0).ToString());
                    if (!dr.IsDBNull(1))
                        mt.value = dr.GetString(1);
                    if (!dr.IsDBNull(2))
                        mt.requiredVal = (int.Parse(dr.GetValue(2).ToString()) == 0) ? false : true;
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
        /// Checks if an Attribute is required or not
        /// </summary>
        /// <param name="att">Attribute name</param>
        /// <returns>true if attribute is required, false if not</returns>
        public static bool isAttRequired(string att)
        {
            bool b = false;
            int n = 0;
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            conn.Open();
            OracleCommand cmd = new OracleCommand("SELECT COUNT(*) FROM MODELSREQVALS where NAME = '" + att + "'", conn);
            OracleDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                if (!dr.IsDBNull(0))
                    n = Int16.Parse(dr.GetValue(0).ToString());
            }
            if (n != 0)
                b = true;
            return b;
        }

        /// <summary>
        /// Gets the number of objects in view defined by the system administrator
        /// </summary>
        /// <param name="pId">RequiredValue ID</param>
        /// <returns>RequiredValues object</returns>
        public static RequiredValues getRowsPerPgById(int pId)
        {
            RequiredValues u = new RequiredValues();
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());

            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("SELECT * FROM REQUIREDVALUES WHERE NUM= " + pId + " ORDER BY NUM", conn);
                OracleDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    if (!dr.IsDBNull(0))
                        u.num = Int32.Parse(dr.GetValue(0).ToString());
                    if (!dr.IsDBNull(1))
                        u.value = dr.GetValue(1).ToString();
                    if (!dr.IsDBNull(2))
                        u.reqRowsPerPage = Int32.Parse(dr.GetValue(2).ToString());
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


        //initialise required values
        /// <summary>
        /// Edits required attributes information in database
        /// </summary>
        /// <param name="rvl">list of RequiredValues objects contains the new required attributes information</param>
        /// <returns>exception message if there is any, empty string if there is none</returns>
        public static string update(List<RequiredValues> rvl)
        {
            string res = "";

            //initialize ReqPatientVals
            foreach (var item in rvl)
            {
                updateReqVal(item);
            }

            //

            return res;
        }


        /// <summary>
        /// Edits the RowsPerPage attribute in database
        /// </summary>
        /// <param name="rpp">integer define the new value of rows per page</param>
        /// <returns>exception message if there is any, empty string if there is none</returns>
        public static string updateRowspp(int rpp)
        {
            string res = "";

            updateRppVal(rpp);

            //

            return res;
        }

        /// <summary>
        /// Intializes the RequiredValues Table in database
        /// </summary>
        /// <returns>exception message if there is any, empty string if there is none</returns>
        public static string initializeReqs()
        {
            /*
            List<RequiredValues> old = getRequiredValuessList();
            if (old.Count > 0)
                return "";
*/

            string res = "";
            //OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            ////delete all perms
            //try
            //{
            //    conn.Open();
            //    string qr = "delete from REQUIREDVALUES where 1=1 ";
            //    OracleCommand cmd = new OracleCommand(qr, conn);
            //    cmd.ExecuteNonQuery();
            //}
            //catch (OracleException e)
            //{
            //    res = e.Message;
            //}
            //catch
            //{
            //    res = "حدث خطأ";
            //}
            //finally
            //{
            //    conn.Close();
            //}



            //initialize ReqPatientVals

            //addReqVal(new RequiredValues((int)ReqPatientVals.firstname, "firstname"));
            addReqVal(new RequiredValues((int)ReqPatientVals.middlename, "middlename"));
            //addReqVal(new RequiredValues((int)ReqPatientVals.lastname, "lastname"));
            addReqVal(new RequiredValues((int)ReqPatientVals.gendre, "gendre"));
            addReqVal(new RequiredValues((int)ReqPatientVals.mothername, "mothername"));

            addReqVal(new RequiredValues((int)ReqPatientVals.birthdate, "birthdate"));
            addReqVal(new RequiredValues((int)ReqPatientVals.age, "age"));
            addReqVal(new RequiredValues((int)ReqPatientVals.mobilephone, "mobilephone"));
            addReqVal(new RequiredValues((int)ReqPatientVals.landphone, "landphone"));
            addReqVal(new RequiredValues((int)ReqPatientVals.currentaddress, "currentaddress"));

            addReqVal(new RequiredValues((int)ReqPatientVals.residentaddress, "residentaddress"));
            addReqVal(new RequiredValues((int)ReqPatientVals.workphone, "workphone"));
            addReqVal(new RequiredValues((int)ReqPatientVals.workaddress, "workaddress"));
            addReqVal(new RequiredValues((int)ReqPatientVals.nearestperson, "nearestperson"));
            addReqVal(new RequiredValues((int)ReqPatientVals.nearestpersonphone, "nearestpersonphone"));
            addReqVal(new RequiredValues((int)ReqPatientVals.birthplace, "birthplace"));
            addReqVal(new RequiredValues((int)ReqPatientVals.nationalidnumber, "nationalidnumber"));

            addReqVal(new RequiredValues((int)ReqPatientVals.nationality, "nationality"));
            addReqVal(new RequiredValues((int)ReqPatientVals.worktype, "worktype"));
            addReqVal(new RequiredValues((int)ReqPatientVals.notes, "notes"));
            addReqVal(new RequiredValues((int)ReqPatientVals.martialstatus, "martialstatus"));
            addReqVal(new RequiredValues((int)ReqPatientVals.insertdate, "insertdate"));

            //for order
            addReqVal(new RequiredValues((int)ReqPatientVals.Doctor, "Doctor"));
            addReqVal(new RequiredValues((int)ReqPatientVals.DocumnetId, "DocumnetId"));

            //add default rows per page
            addDefaultRowsPerP(new RequiredValues((int)ReqPatientVals.rowsPerPage, "rowsPerPage", 20));

            //end

            //

            return res;
        }

        /// <summary>
        /// Inserts a required attribute information into RequiredValues table in database
        /// </summary>
        /// <param name="p">RequiredValues object contains the required attribute information</param>
        /// <returns>exception message if there is any, empty string if there is none</returns>
        public static string addReqVal(RequiredValues p)
        {
            string res = "";
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                string qr = "Begin" +
                            "  insert into REQUIREDVALUES" +
                            "( NUM, VALUE)" +
                            "values " +
                            "(:NUM,:VALUE);" +
                            "End;";
                OracleCommand cmd = new OracleCommand(qr, conn);
                OracleParameter[] param =  {
                                            new OracleParameter("NUM", p.num),
                                            new OracleParameter("VALUE", p.value),
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
        /// Inserts the default number of rows per page into database
        /// </summary>
        /// <param name="p">RequiredValues object</param>
        /// <returns>exception message if there is any, empty string if there is none</returns>
        public static string addDefaultRowsPerP(RequiredValues p)
        {
            string res = "";
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                string qr = "Begin" +
                            "  insert into REQUIREDVALUES" +
                            "( NUM, VALUE,REQUIRED)" +
                            "values " +
                            "(:NUM,:VALUE,:REQUIRED);" +
                            "End;";
                OracleCommand cmd = new OracleCommand(qr, conn);
                OracleParameter[] param =  {
                                            new OracleParameter("NUM", p.num),
                                            new OracleParameter("VALUE", p.value),
                                            new OracleParameter("REQUIRED", p.reqRowsPerPage),

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
        /// Deletes all required attributes information from database
        /// </summary>
        /// <returns>exception message if there is any, empty string if there is none</returns>
        public static string deleteReqVals()
        {
            string res = "";
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                OracleCommand c = new OracleCommand("DELETE MODELSREQVALS WHERE 1=1", conn);
                c.ExecuteNonQuery();
                conn.Close();
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
        /// Edits the information of required attribute in database
        /// </summary>
        /// <param name="p">RequiredValues object</param>
        /// <returns>exception message if there is any, empty string if there is none</returns>
        public static string updateReqVal(RequiredValues p)
        {
            string res = "";
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                int rv = 0;
                if (p.requiredVal)
                {
                    rv = 1;

                    conn.Open();
                    string q = "Begin" +
                            "  insert into MODELSREQVALS" +
                            "( NAME)" +
                            "values " +
                            "(:NAME);" +
                            "End;";
                    OracleCommand c = new OracleCommand(q, conn);

                    OracleParameter parameter = new OracleParameter("NAME", p.value);
                    c.Parameters.Add(parameter);
                    c.ExecuteNonQuery();
                    conn.Close();
                }

                conn.Open();
                string qr = " Begin" +
                            "  update REQUIREDVALUES" +
                            " set REQUIRED = :REQUIRED" +
                            " where " +
                            " NUM = :NUM;" +
                            " End;";
                OracleCommand cmd = new OracleCommand(qr, conn);
                OracleParameter[] param =  {
                                            new OracleParameter("REQUIRED", rv),
                                            new OracleParameter("NUM", p.num),
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
        /// Edits the required rows per page attribute in database
        /// </summary>
        /// <param name="count">the new value of rows per page</param>
        /// <returns>exception message if there is any, empty string if there is none</returns>
        public static string updateRppVal(int count)
        {
            string res = "";
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                string qr = " Begin" +
                            "  update REQUIREDVALUES" +
                            " set REQUIRED = :REQUIRED" +
                            " where " +
                            " NUM = :NUM;" +
                            " End;";

                //int rv = 0;
                if (count < 1)
                    count = 5;

                OracleCommand cmd = new OracleCommand(qr, conn);
                OracleParameter[] param =  {
                                            new OracleParameter("REQUIRED", count),
                                            new OracleParameter("NUM", (int)ReqPatientVals.rowsPerPage),
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