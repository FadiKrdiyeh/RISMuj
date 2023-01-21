using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RIS.Models;
using RISDB;
using Oracle.DataAccess;
using System.ComponentModel.DataAnnotations;
using Oracle.DataAccess.Client;
using System.Text;
using Oracle.DataAccess.Types;
using System.IO;
using System.Text.RegularExpressions;

namespace RIS.Models
{
    public class Report
    {

        public int NUM { set; get; }
        public int ORDERNUM { set; get; }
        public string PATIENTNUM { set; get; }
        public DateTime IMAGEDATE { set; get; }
        public int REFERINGDOCTORID { set; get; } // doctor who sends the order
        public int DOCTORID { set; get; }  // doctor who writes the report
        public int REFERINGPHYSICIANID { set; get; } // physcian who perform the image
        public DateTime REPORTDATE { set; get; }

        public string REPORTDATESTRING
        {
            get
            {
                return REPORTDATE.ToString();
            }
        }
        public string TITLE { set; get; }
        
        public string MEIDCALHISTORY { set; get; }

        private string _MEIDCALHISTORY1252;

        public string ALERGY
        { set; get; }

        public string REPORTBODY
        { set; get; }


        public string NOTES
        { set; get; }


        public string AUDIOPATH { set; get; }
        public int? PARENTREPORT { set; get; }
        public string SERIESNUMBER { set; get; }
        public int APPROVED { set; get; }
        public int APPROVEDDOCTORID { set; get; }

        public Patient PATIENT
        {
            get
            {
                return Patient.Select(int.Parse(PATIENTNUM));
            }
        }

        public Radiology RADIOLOGY
        {
            get
            {
                return Radiology.Select(ORDERNUM);
            }
        }

        public User REFERINGDOCTOR
        {
            get
            {
                return User.select(REFERINGDOCTORID);
            }
        }

        public Doctor DOCTOR
        {
            //get
            //{
            //    return User.select(DOCTORID);
            //}
            get
            {
                try
                {
                    Doctor u = (DOCTORID == null) ? null : RIS.Models.Doctor.select((int)DOCTORID);
                    return u;
                }
                catch
                {
                    return null;
                }
            }
        }

        public User APPROVEDDOCTOR
        {
            get
            {
                return User.select(APPROVEDDOCTORID);
            }
        }
        public Doctor REFERINGPHYSICIAN
        {
            get
            {
                return Doctor.select(REFERINGPHYSICIANID);
            }
        }
        public static string Insert(Report ro)
        {

            if (!string.IsNullOrEmpty(ro.AUDIOPATH) && ro.AUDIOPATH != "-1") // if the report has an audio file and the reprort is not approved (approving reports need "ReportsAdmin" role), we do not keep the audio file after approving the report from the chief of doctors.
            {
                // reaching this piece of code means that we are going to save a audio file for the report.

                // the file will be saved in a path that has the following pattern "AudioFiles/Patient_Id/DateTimeNow.wav"

                string strMappath1 = Regex.Replace(ro.PATIENTNUM.ToString(), @"[^0-9a-zA-Z]+", ""); //  to make the name of the directory accepatable from windows.
                string strMappath = ConnectionConfigs.getConfig().audioFilesDirectory + "/" + strMappath1; // get the AudioFiles path.

                if (!Directory.Exists(strMappath)) // if this is the first audio file of the patient then create a new directory for him.
                {
                    DirectoryInfo di = Directory.CreateDirectory(strMappath);

                }
                // TODO: Convert to mp3
                string audioName = DateTime.Now.ToString("yyyyMMddHHmm") + ".wav"; ; // the audio file name 
                string fullAudioPath = strMappath + "/" + audioName; // full path of the audio file.

                // write the physical audio file 
                FileStream fs = new FileStream(fullAudioPath, FileMode.Create, FileAccess.ReadWrite);
                string audioData = ro.AUDIOPATH.Replace("data:audio/wav;base64,", "");
                try
                {
                    fs.Write(Convert.FromBase64String(audioData), 0, (int)Convert.FromBase64String(audioData).Length);
                    fs.Close();
                }
                catch
                {
                    fs.Close();
                }
                ro.AUDIOPATH = audioName; // to save the audio file name in the database.
            }
            else
            {
                ro.AUDIOPATH = null;
            }


            //ro.NUM = OracleRIS.GetOracleSequenceValue("REPORT_SEQ"); // to create the id of the report.
            string res = "";
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            conn.Open();

            // to save the conclusion of the report we use BLOB.
            //if (ro.TITLE == null) ro.TITLE = " ";
            //byte[] fndng = Encoding.UTF8.GetBytes(ro.TITLE); // convert the string into bytes array.
            //OracleBlob lobTITLE = new OracleBlob(conn); // define the blob.
            //lobTITLE.Write(fndng, 0, (int)fndng.Length); // write the bytes array to the blob.

            // the same thing as conclusion.
            //if (ro.MEIDCALHISTORY == null) ro.MEIDCALHISTORY = " ";
            //byte[] rqst = Encoding.UTF8.GetBytes(ro.MEIDCALHISTORY);
            //OracleBlob lobMEIDCALHISTORY = new OracleBlob(conn);
            //lobMEIDCALHISTORY.Write(rqst, 0, (int)rqst.Length);

            //// the same thing as conclusion.
            //if (ro.ALERGY == null) ro.ALERGY = " ";
            //byte[] hstr = Encoding.UTF8.GetBytes(ro.ALERGY);
            //OracleBlob lobALERGY = new OracleBlob(conn);
            //lobALERGY.Write(hstr, 0, (int)hstr.Length);

            //// the same thing as conclusion.
            //if (ro.REPORTBODY == null) ro.REPORTBODY = " ";
            //byte[] dss = Encoding.UTF8.GetBytes(ro.REPORTBODY);
            //OracleBlob lobREPORTBODY = new OracleBlob(conn);
            //lobREPORTBODY.Write(dss, 0, (int)dss.Length);

            // the same thing as conclusion.
            //if (ro.NOTES == null) ro.NOTES = " ";
            //byte[] sig = Encoding.UTF8.GetBytes(ro.NOTES);
            //OracleBlob lobNOTES = new OracleBlob(conn);
            //lobNOTES.Write(sig, 0, (int)sig.Length);

            if (ro.PARENTREPORT == 0) ro.PARENTREPORT = -1; // if the report is not edited from any report.
            try
            {
                string qr = " " +
                            "  insert into REPORT " +

                            "( NUM, ORDERNUM, PATIENTNUM, IMAGEDATE, REFERINGDOCTORID, DOCTORID, REFERINGPHYSICIANID, REPORTDATE, TITLE, MEIDCALHISTORY, ALERGY, REPORTBODY, NOTES, AUDIOPATH, PARENTREPORT, SERIESNUMBER, APPROVED, APPROVEDDOCTORID ) " +
                            " values " +
                            "( :NUM, :ORDERNUM, :PATIENTNUM, :IMAGEDATE, :REFERINGDOCTORID, :DOCTORID, :REFERINGPHYSICIANID, :REPORTDATE, :TITLE, :MEIDCALHISTORY, :ALERGY, :REPORTBODY, :NOTES, :AUDIOPATH, :PARENTREPORT, :SERIESNUMBER, :APPROVED, :APPROVEDDOCTORID ) " +

                            " ";
                OracleCommand cmd = new OracleCommand(qr, conn);
                OracleParameter[] param =  {
                                            new OracleParameter("NUM", ro.NUM),
                                            new OracleParameter("ORDERNUM", ro.ORDERNUM),
                                            new OracleParameter("PATIENTNUM",ro.PATIENTNUM),
                                            new OracleParameter("IMAGEDATE", ro.IMAGEDATE),
                                            new OracleParameter("REFERINGDOCTORID", ro.REFERINGDOCTORID),
                                            new OracleParameter("DOCTORID", ro.DOCTORID),
                                            new OracleParameter("REFERINGPHYSICIANID", ro.REFERINGPHYSICIANID),
                                            new OracleParameter("REPORTDATE", ro.REPORTDATE),
                                            new OracleParameter("TITLE", ro.TITLE),
                                            new OracleParameter("MEIDCALHISTORY", ro.MEIDCALHISTORY),
                                            new OracleParameter("ALERGY",ro.ALERGY),
                                            new OracleParameter("REPORTBODY",ro.REPORTBODY),
                                            new OracleParameter("NOTES",ro.NOTES),
                                            new OracleParameter("AUDIOPATH", ro.AUDIOPATH),
                                            new OracleParameter("PARENTREPORT", ro.PARENTREPORT),
                                            new OracleParameter("SERIESNUMBER", ro.SERIESNUMBER),
                                            new OracleParameter("APPROVED", ro.APPROVED),
                                            new OracleParameter("APPROVEDDOCTORID", ro.APPROVEDDOCTORID)

                                           };
                //  must be added for BLOB type.
                //param[8].Direction = System.Data.ParameterDirection.InputOutput;
                //param[9].Direction = System.Data.ParameterDirection.InputOutput;
                //param[10].Direction = System.Data.ParameterDirection.InputOutput;
                //param[11].Direction = System.Data.ParameterDirection.InputOutput;
                //param[12].Direction = System.Data.ParameterDirection.InputOutput;


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
        public static string Edit(Report ro)
        {
            string res = "";
            //ro.PARENTREPORT = ro.NUM;
            //ro.NUM = OracleRIS.GetOracleSequenceValue("REPORT_SEQ"); // to create the id of the report.
            if (Delete(ro.NUM))
            {
                res = Insert(ro);
            }
            else
            {
                res = "حدث خطأ";
            }

            return res;

        }
        public static bool Delete(int id)
        {
            Report r2 = Select(id);

            try
            {
                string strMappath1 = Regex.Replace(r2.PATIENTNUM.ToString(), @"[^0-9a-zA-Z]+", "");
                string audioFullPath = ConnectionConfigs.getConfig().audioFilesDirectory + "/" + strMappath1 + "/" + r2.AUDIOPATH;
                if (System.IO.File.Exists(audioFullPath))
                {
                    System.IO.File.Delete(audioFullPath);
                }
            }
            catch
            { }


            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());

            try
            {
                conn.Open();
                string qr = "DELETE REPORT WHERE NUM = :NUM ";
                OracleParameter param = new OracleParameter("NUM", id);
                OracleCommand cmd = new OracleCommand(qr, conn);
                cmd.Parameters.Add(param);
                cmd.ExecuteNonQuery();
            }
            catch (OracleException e)
            {
                return false;
            }
            catch
            {
                return false;
            }
            finally
            {
                conn.Close();
            }
            return true;


        }
        public static Report Select(int id)
        {
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());

            string qr = "select * from REPORT where NUM=" + "'" + id + "'";
            conn.Open();

            OracleCommand cmd = new OracleCommand(qr, conn);

            OracleDataReader dr = cmd.ExecuteReader();
            Report p = new Report();

            try
            {
                if (dr.Read())
                {
                    p.NUM = int.Parse(dr.GetValue(0).ToString());
                    if (!dr.IsDBNull(1))
                        p.ORDERNUM = int.Parse(dr.GetValue(1).ToString());
                    if (!dr.IsDBNull(2))
                    {
                        p.PATIENTNUM = (dr.GetValue(2).ToString());
                    }
                    if (!dr.IsDBNull(3))
                        p.IMAGEDATE = DateTime.Parse((dr.GetValue(3).ToString()));
                    if (!dr.IsDBNull(4))
                    {
                        p.REFERINGDOCTORID = int.Parse(dr.GetValue(4).ToString());
                    }
                    if (!dr.IsDBNull(5))
                        p.DOCTORID = int.Parse(dr.GetValue(5).ToString());
                    if (!dr.IsDBNull(6))
                    {
                        p.REFERINGPHYSICIANID = int.Parse(dr.GetValue(6).ToString());
                    }
                    if (!dr.IsDBNull(7))
                        p.REPORTDATE = DateTime.Parse((dr.GetValue(7).ToString()));
                    if (!dr.IsDBNull(8))
                    {
                        //byte[] bb1 = (byte[])dr.GetValue(8); // because it is saved as blob
                        //p.TITLE = Encoding.UTF8.GetString(bb1);

                        p.TITLE = dr.GetString(8).ToString();

                    }
                    if (!dr.IsDBNull(9))
                    {
                        //byte[] bb1 = (byte[])dr.GetValue(9); // because it is saved as blob
                        //p.MEIDCALHISTORY = Encoding.UTF8.GetString(bb1);

                        p.MEIDCALHISTORY = dr.GetString(9).ToString();

                    }

                    if (!dr.IsDBNull(10))
                    {
                        //byte[] bb2 = (byte[])dr.GetValue(10);// because it is saved as blob
                        //p.ALERGY = Encoding.UTF8.GetString(bb2);

                        p.ALERGY = dr.GetString(10).ToString();

                    }
                    if (!dr.IsDBNull(11))
                    {
                        //byte[] aa = (byte[])dr.GetValue(11);// because it is saved as blob
                        //p.REPORTBODY = Encoding.UTF8.GetString(aa);

                        p.REPORTBODY = dr.GetString(11).ToString();


                    }
                    if (!dr.IsDBNull(12))
                    {
                        //byte[] aa = (byte[])dr.GetValue(12);// because it is saved as blob
                        //p.NOTES = Encoding.UTF8.GetString(aa);

                        p.NOTES = dr.GetString(12).ToString();


                    }
                    if (!dr.IsDBNull(13))
                    {
                        p.AUDIOPATH = dr.GetString(13).ToString();
                    }

                    if (!dr.IsDBNull(14))
                    {
                        p.PARENTREPORT = int.Parse(dr.GetValue(14).ToString());

                    }
                    if (!dr.IsDBNull(15))
                    {
                        p.SERIESNUMBER = dr.GetValue(15).ToString();
                    }

                    if (!dr.IsDBNull(16))
                    {
                        p.APPROVED = int.Parse(dr.GetValue(16).ToString());
                    }
                    if (!dr.IsDBNull(17))
                    {
                        p.APPROVEDDOCTORID = int.Parse(dr.GetValue(16).ToString());
                    }
                }
                conn.Close();
                return p;
            }
            catch (Exception ex)
            {
                string s = ex.ToString();
                conn.Close();
                return p;
            }
        }
        public static List<Report> getOrderReport(int repId)
        {
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());

            string qr = "select * from REPORT where ORDERNUM=" + "'" + repId + "'";
            conn.Open();

            OracleCommand cmd = new OracleCommand(qr, conn);

            OracleDataReader dr = cmd.ExecuteReader();

            List<Report> res = new List<Models.Report>();
            try
            {
                while (dr.Read())
                {
                    Report p = new Report();
                    p.NUM = int.Parse(dr.GetValue(0).ToString());
                    if (!dr.IsDBNull(1))
                        p.ORDERNUM = int.Parse(dr.GetValue(1).ToString());
                    if (!dr.IsDBNull(2))
                    {
                        p.PATIENTNUM = (dr.GetValue(2).ToString());
                    }
                    if (!dr.IsDBNull(3))
                        p.IMAGEDATE = DateTime.Parse((dr.GetValue(3).ToString()));
                    if (!dr.IsDBNull(4))
                    {
                        p.REFERINGDOCTORID = int.Parse(dr.GetValue(4).ToString());
                    }
                    if (!dr.IsDBNull(5))
                        p.DOCTORID = int.Parse(dr.GetValue(5).ToString());
                    if (!dr.IsDBNull(6))
                    {
                        p.REFERINGPHYSICIANID = int.Parse(dr.GetValue(6).ToString());
                    }
                    if (!dr.IsDBNull(7))
                        p.REPORTDATE = DateTime.Parse((dr.GetValue(7).ToString()));
                    //if (!dr.IsDBNull(8))
                    //{
                    //    byte[] bb1 = (byte[])dr.GetValue(8); // because it is saved as blob
                    //    p.TITLE = Encoding.UTF8.GetString(bb1);
                    //}
                    //if (!dr.IsDBNull(9))
                    //{
                    //    byte[] bb1 = (byte[])dr.GetValue(9); // because it is saved as blob
                    //    p.MEIDCALHISTORY = Encoding.UTF8.GetString(bb1);
                    //}

                    //if (!dr.IsDBNull(10))
                    //{
                    //    byte[] bb2 = (byte[])dr.GetValue(10);// because it is saved as blob
                    //    p.ALERGY = Encoding.UTF8.GetString(bb2);
                    //}
                    //if (!dr.IsDBNull(11))
                    //{
                    //    byte[] aa = (byte[])dr.GetValue(11);// because it is saved as blob
                    //    p.REPORTBODY = Encoding.UTF8.GetString(aa);

                    //}
                    //if (!dr.IsDBNull(12))
                    //{
                    //    byte[] aa = (byte[])dr.GetValue(12);// because it is saved as blob
                    //    p.NOTES = Encoding.UTF8.GetString(aa);

                    //}

                    if (!dr.IsDBNull(8))
                    {
                        //byte[] bb1 = (byte[])dr.GetValue(8); // because it is saved as blob
                        //p.TITLE = Encoding.UTF8.GetString(bb1);

                        p.TITLE = dr.GetString(8).ToString();

                    }
                    if (!dr.IsDBNull(9))
                    {
                        //byte[] bb1 = (byte[])dr.GetValue(9); // because it is saved as blob
                        //p.MEIDCALHISTORY = Encoding.UTF8.GetString(bb1);

                        p.MEIDCALHISTORY = dr.GetString(9).ToString();

                    }

                    if (!dr.IsDBNull(10))
                    {
                        //byte[] bb2 = (byte[])dr.GetValue(10);// because it is saved as blob
                        //p.ALERGY = Encoding.UTF8.GetString(bb2);

                        p.ALERGY = dr.GetString(10).ToString();

                    }
                    if (!dr.IsDBNull(11))
                    {
                        //byte[] aa = (byte[])dr.GetValue(11);// because it is saved as blob
                        //p.REPORTBODY = Encoding.UTF8.GetString(aa);

                        p.REPORTBODY = dr.GetString(11).ToString();


                    }
                    if (!dr.IsDBNull(12))
                    {
                        //byte[] aa = (byte[])dr.GetValue(12);// because it is saved as blob
                        //p.NOTES = Encoding.UTF8.GetString(aa);

                        p.NOTES = dr.GetString(12).ToString();


                    }

                    if (!dr.IsDBNull(13))
                    {
                        p.AUDIOPATH = dr.GetString(13).ToString();
                    }

                    if (!dr.IsDBNull(14))
                    {
                        p.PARENTREPORT = int.Parse(dr.GetValue(14).ToString());

                    }
                    if (!dr.IsDBNull(15))
                    {
                        p.SERIESNUMBER = dr.GetValue(15).ToString();
                    }

                    if (!dr.IsDBNull(16))
                    {
                        p.APPROVED = int.Parse(dr.GetValue(16).ToString());
                    }
                    if (!dr.IsDBNull(17))
                    {
                        p.APPROVEDDOCTORID = int.Parse(dr.GetValue(16).ToString());
                    }
                    res.Add(p);
                }
                conn.Close();
                return res;
            }
            catch (Exception ex)
            {
                string s = ex.ToString();
                conn.Close();
                return res;
            }
        }
        public static List<Report> getPatientReport(int patId)
        {
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());

            string qr = "select * from STANDALONEDIAGNOSTICREPORTS where PATIENTNUM=" + "'" + patId + "'";
            conn.Open();

            OracleCommand cmd = new OracleCommand(qr, conn);

            OracleDataReader dr = cmd.ExecuteReader();

            List<Report> res = new List<Models.Report>();
            try
            {
                while (dr.Read())
                {
                    Report p = new Report();
                    p.NUM = int.Parse(dr.GetValue(0).ToString());
                    if (!dr.IsDBNull(1))
                        p.ORDERNUM = int.Parse(dr.GetValue(1).ToString());
                    if (!dr.IsDBNull(2))
                    {
                        p.PATIENTNUM = (dr.GetValue(2).ToString());
                    }
                    if (!dr.IsDBNull(3))
                        p.IMAGEDATE = DateTime.Parse((dr.GetValue(3).ToString()));
                    if (!dr.IsDBNull(4))
                    {
                        p.REFERINGDOCTORID = int.Parse(dr.GetValue(4).ToString());
                    }
                    if (!dr.IsDBNull(5))
                        p.DOCTORID = int.Parse(dr.GetValue(5).ToString());
                    if (!dr.IsDBNull(6))
                    {
                        p.REFERINGPHYSICIANID = int.Parse(dr.GetValue(6).ToString());
                    }
                    if (!dr.IsDBNull(7))
                        p.REPORTDATE = DateTime.Parse((dr.GetValue(7).ToString()));


                    //if (!dr.IsDBNull(8))
                    //{
                    //    byte[] bb1 = (byte[])dr.GetValue(9); // because it is saved as blob
                    //    p.TITLE = Encoding.UTF8.GetString(bb1);
                    //}
                    //if (!dr.IsDBNull(9))
                    //{
                    //    byte[] bb1 = (byte[])dr.GetValue(9); // because it is saved as blob
                    //    p.MEIDCALHISTORY = Encoding.UTF8.GetString(bb1);
                    //}

                    //if (!dr.IsDBNull(10))
                    //{
                    //    byte[] bb2 = (byte[])dr.GetValue(10);// because it is saved as blob
                    //    p.ALERGY = Encoding.UTF8.GetString(bb2);
                    //}
                    //if (!dr.IsDBNull(11))
                    //{
                    //    byte[] aa = (byte[])dr.GetValue(11);// because it is saved as blob
                    //    p.REPORTBODY = Encoding.UTF8.GetString(aa);

                    //}
                    //if (!dr.IsDBNull(12))
                    //{
                    //    byte[] aa = (byte[])dr.GetValue(12);// because it is saved as blob
                    //    p.NOTES = Encoding.UTF8.GetString(aa);

                    //}

                    if (!dr.IsDBNull(8))
                    {
                        //byte[] bb1 = (byte[])dr.GetValue(8); // because it is saved as blob
                        //p.TITLE = Encoding.UTF8.GetString(bb1);

                        p.TITLE = dr.GetString(8).ToString();

                    }
                    if (!dr.IsDBNull(9))
                    {
                        //byte[] bb1 = (byte[])dr.GetValue(9); // because it is saved as blob
                        //p.MEIDCALHISTORY = Encoding.UTF8.GetString(bb1);

                        p.MEIDCALHISTORY = dr.GetString(9).ToString();

                    }

                    if (!dr.IsDBNull(10))
                    {
                        //byte[] bb2 = (byte[])dr.GetValue(10);// because it is saved as blob
                        //p.ALERGY = Encoding.UTF8.GetString(bb2);

                        p.ALERGY = dr.GetString(10).ToString();

                    }
                    if (!dr.IsDBNull(11))
                    {
                        //byte[] aa = (byte[])dr.GetValue(11);// because it is saved as blob
                        //p.REPORTBODY = Encoding.UTF8.GetString(aa);

                        p.REPORTBODY = dr.GetString(11).ToString();


                    }
                    if (!dr.IsDBNull(12))
                    {
                        //byte[] aa = (byte[])dr.GetValue(12);// because it is saved as blob
                        //p.NOTES = Encoding.UTF8.GetString(aa);

                        p.NOTES = dr.GetString(12).ToString();


                    }

                    if (!dr.IsDBNull(13))
                    {
                        p.AUDIOPATH = dr.GetString(13).ToString();
                    }

                    if (!dr.IsDBNull(14))
                    {
                        p.PARENTREPORT = int.Parse(dr.GetValue(14).ToString());

                    }
                    if (!dr.IsDBNull(15))
                    {
                        p.SERIESNUMBER = dr.GetValue(15).ToString();
                    }

                    if (!dr.IsDBNull(16))
                    {
                        p.APPROVED = int.Parse(dr.GetValue(16).ToString());
                    }
                    if (!dr.IsDBNull(17))
                    {
                        p.APPROVEDDOCTORID = int.Parse(dr.GetValue(16).ToString());
                    }
                    res.Add(p);
                }
                conn.Close();
                return res;
            }
            catch (Exception ex)
            {
                string s = ex.ToString();
                conn.Close();
                return res;
            }
        }

    }



}