using System;
using System.Collections.Generic;
using Oracle.DataAccess.Client;
using RISDB;
using System.ComponentModel.DataAnnotations;

namespace RIS.Models
{
    /// <summary>
    /// Class for doctor
    /// </summary>
    public class Doctor
    {

        /// <summary>
        /// Doctor's ID that represent the primary key of doctors table in database.
        /// </summary>
        [Required]
        public int num { get; set; }

        /// <summary>
        /// The doctor's name.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(Resources.Res), ErrorMessageResourceName = "DoctorNameError")]
        [Display(ResourceType = typeof(Resources.Res), Name = "DoctorName")]
        public string name { get; set; }

        /// <summary>
        /// The ID of the department of the doctor.
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "Departement")]
        public int department { get; set; }

        /// <summary>
        /// The date when doctor's information was inserted in RIS.
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "InsertDateParameter")]
        public DateTime insertDate { get; set; }

        /// <summary>
        /// The ID of the user who inserted the doctor's information in RIS.
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "InsertUser")]
        public int insertUser { get; set; }

        /// <summary>
        /// The doctor's department name.
        /// </summary>
        public string docDepartmentName
        {
            get
            {
                return Departement.select(department).name;
            }
        }

        /// <summary>
        /// The name of the user who inserted the doctor's information.
        /// </summary>
        public string insertUserName
        {
            get
            {
                return User.select(insertUser).username;
            }
        }

        /// <summary>
        /// Some description about the doctor.
        /// </summary>
        [Display(ResourceType =typeof(Resources.Res),Name ="DoctorDescription")]
        public string description { get; set; }

        /// <summary>
        /// Doctor object constructor.
        /// </summary>
        public Doctor() { }

        /// <summary>
        /// Gets doctors' information from doctors table in database.
        /// </summary>
        /// <returns>a list of type Doctor to be shown in view</returns>
        public static List<Doctor> getDoctorsList()
        {
            List<Doctor> doctors = new List<Doctor>();
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("SELECT * FROM DOCTORS", conn);
                OracleDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Doctor d = new Doctor();
                    if (!dr.IsDBNull(0))
                        d.num = Int32.Parse(dr.GetValue(0).ToString());
                    if (!dr.IsDBNull(1))
                        d.name = dr.GetValue(1).ToString();
                    if (!dr.IsDBNull(2))
                        d.department = int.Parse(dr.GetValue(2).ToString());
                    if (!dr.IsDBNull(3))
                        d.insertDate = DateTime.Parse(dr.GetValue(3).ToString());
                    if (!dr.IsDBNull(4))
                        d.insertUser = int.Parse(dr.GetValue(4).ToString());
                    if (!dr.IsDBNull(5))
                        d.description = dr.GetValue(5).ToString();
                    doctors.Add(d);
                }
            }
            catch (Exception ex)
            {
                string reee = ex.ToString();
                return null;
            }
            finally
            {
                conn.Close();
            }
            return doctors;
        }

        /// <summary>
        /// Gets information stored in database for a defined doctor.
        /// </summary>
        /// <param name="id">the ID of targeted doctor.</param>
        /// <returns>an object of type Doctor containing the details of targeted doctor.</returns>
        public static Doctor select(int id)
        {
            Doctor d = new Doctor();
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("SELECT * FROM DOCTORS WHERE NUM =" + id, conn);
                OracleDataReader dr = cmd.ExecuteReader();
                if(dr.Read())
                {
                    if (!dr.IsDBNull(0))
                        d.num = Int32.Parse(dr.GetValue(0).ToString());
                    if (!dr.IsDBNull(1))
                        d.name = dr.GetString(1);
                    if (!dr.IsDBNull(2))
                        d.department = int.Parse(dr.GetValue(2).ToString());
                    if (!dr.IsDBNull(3))
                        d.insertDate = dr.GetDateTime(3);
                    if (!dr.IsDBNull(4))
                        d.insertUser = int.Parse(dr.GetValue(4).ToString());
                    if (!dr.IsDBNull(5))
                        d.description = (dr.GetValue(5).ToString()); ;
                }
            }
            catch
            {
                return null;
            }
            return d;
        }

        /// <summary>
        /// Inserts a new doctor information in doctors table in database.
        /// </summary>
        /// <param name="d">the ID of the inserted doctor.</param>
        /// <returns>exception message if there is any, empty string if none.</returns>
        public static string insert(Doctor d)
        {
            string s = "";
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                //d.insertDate = DateTime.Now;
                conn.Open();
                string query = "INSERT INTO DOCTORS ( " +
                    " NUM, NAME, DEPARTMENT, INSERTDATE, INSERTUSER, DESCRIPTION ) VALUES (" +
                     " :NUM, :NAME, :DEPARTMENT, :INSERTDATE, :INSERTUSER, :DESCRIPTION )";
                OracleCommand cmd = new OracleCommand(query, conn);
                OracleParameter[] param ={
                                            #region parameter
                                            new OracleParameter("NUM", d.num),
                                            new OracleParameter("NAME", d.name),
                                            new OracleParameter("DEPARTMENT", d.department),
                                            new OracleParameter("INSERTDATE", d.insertDate),
                                            new OracleParameter("INSERTUSER", d.insertUser),
                                            new OracleParameter("DESCRIPTION", d.description),
                                            #endregion parameter
                                           };
                for (int j = 0; j < param.Length; j++)
                    cmd.Parameters.Add(param[j]);

                cmd.ExecuteNonQuery();
            }
            catch
            {
                s = RIS.Resources.Res.Error;
            }
            finally
            {
                conn.Close();
            }
            return s;
        }

        /// <summary>
        /// Edits the doctor information in database.
        /// </summary>
        /// <param name="d">the ID of the edited doctor.</param>
        /// <returns>exception message if there is any, empty string if none.</returns>
        public static string edit(Doctor d)
        {
            string s = "";
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                //d.insertDate = DateTime.Now;
                conn.Open();
                string query = "UPDATE DOCTORS SET " +
                " NAME =:NAME, DEPARTMENT =:DEPARTMENT, DESCRIPTION =:DESCRIPTION where NUM=:NUM ";
                //" NAME = :NAME, DEPARTMENT = :DEPARTMENT, INSERTDATE = :INSERTDATE, INSERTUSER = :INSERTUSER, DESCRIPTION = :DESCRIPTION where NUM=:NUM ";

                OracleCommand cmd = new OracleCommand(query, conn);
                OracleParameter[] param ={
                                            #region parameter
                                            new OracleParameter("NAME", d.name),
                                            new OracleParameter("DEPARTMENT", int.Parse(d.department.ToString())),
                                            //new OracleParameter("INSERTDATE", d.insertDate),
                                            //new OracleParameter("INSERTUSER", d.insertUser),
                                            new OracleParameter("DESCRIPTION", d.description),
                                            new OracleParameter("NUM", int.Parse(d.num.ToString())),

                                            #endregion parameter
                                           };
                for (int j = 0; j < param.Length; j++)
                    cmd.Parameters.Add(param[j]);

                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                s = RIS.Resources.Res.Error;
            }
            finally
            {
                conn.Close();
            }
            return s;
        }

        /// <summary>
        /// Deletes doctors information from doctors table in database.
        /// </summary>
        /// <param name="id">The ID of targeted doctor.</param>
        /// <returns>exception message if there is any, empty string if none.</returns>
        public static string delete(int id)
        {
            string s = "";
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("DELETE DOCTORS WHERE NUM = " + id, conn);
                cmd.ExecuteNonQuery();               
            }
            catch
            {
                s = RIS.Resources.Res.Error;
            }
            finally
            {
                conn.Close();
            }

            return s;
        }

        //public static SelectList getDoctorsNames (bool withAllOption, string defaultValue)
        //{
        //    List<SelectListItem> items = new List<SelectListItem>();
        //    if (withAllOption)
        //        items.Add(new SelectListItem { Text = "", Value = "" });
        //    foreach (var item in Doctor.getDoctorsList())
        //    {
        //        items.Add(new SelectListItem { Text = item.name, Value = item.num.ToString() });
        //    }
        //    return new SelectList(items, "Value", "Text", string.IsNullOrEmpty(defaultValue) ? "0" : defaultValue);
        //}
    }
}