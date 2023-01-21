using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Oracle.DataAccess.Client;
using RISDB;

namespace RIS.Models
{
    public class ModalityProcedure
    {
        public int num {set; get;}

        public int ModalityId { set; get; }

        [Required(ErrorMessageResourceType = typeof(Resources.Res), ErrorMessageResourceName = "modProcError")]

        public int ProcedureId { set; get; }


        public ModalityProcedure () { }

        public Procedure parentMP
        {
            get
            {
                return Procedure.select(ProcedureId);
            }
        }

        // This Function is used to add a procedure to a modality
        public static string addProcToMod(ModalityProcedure u)
        {
            string res = "";
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                string qr = "Begin" +
                            "  insert into PROCEDURETOMODALITY" +

                            "( NUM, MODALITYID, PROCEDUREID) " +
                            " values " +
                            " (:NUM, :MODALITYID, :PROCEDUREID); " +
                            "End;";
                OracleCommand cmd = new OracleCommand(qr, conn);
                OracleParameter[] param = {
                                            new OracleParameter("NUM", u.num),
                                            new OracleParameter("USERNAME", u.ModalityId),
                                            new OracleParameter("PASS", u.ProcedureId)
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



        // This function is used to get the procedures of a Modaity
        public static List<ModalityProcedure> selectModProc(int mid)
        {
            List<ModalityProcedure> prs = new List<ModalityProcedure>();

            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("SELECT * FROM PROCEDURETOMODALITY WHERE MODALITYID= " + mid, conn);
                OracleDataReader dr = cmd.ExecuteReader();

                //ModalityProcedure u = new ModalityProcedure();

                while (dr.Read())
                {
                    ModalityProcedure mp = new ModalityProcedure();

                    if (!dr.IsDBNull(2))
                        mp.num = int.Parse(dr.GetValue(2).ToString());
                    if (!dr.IsDBNull(0))
                        mp.ModalityId = int.Parse(dr.GetValue(0).ToString());
                    if (!dr.IsDBNull(1))
                        mp.ProcedureId = int.Parse(dr.GetValue(1).ToString());
                  
                    prs.Add(mp);
                }
            }
            catch
            {

            }
            finally
            {
                conn.Close();
            }

            return prs;

        }

        public static string editModProc(ModalityProcedure mp)
        {
            string res = "";
            
            OracleConnection con = new OracleConnection(OracleRIS.GetConnectionString());

            try
            {
                con.Open();
                string qr = "UPDATE PROCEDURETOMODALITY SET" +
                    "MODALITYID=:MODALITYID" +
                    "PROCEDURE=:PROCEDURE";
                OracleCommand cmd = new OracleCommand(qr, con);
                OracleParameter[] parm =
                {
                    new OracleParameter ("MODALITYID",mp.ModalityId),
                    new OracleParameter ("PROCEDURE",mp.ProcedureId)
                };

                for (int i = 0; i < parm.Length; i++)
                {
                    cmd.Parameters.Add(parm[i]);
                }
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
                con.Close();
            }
            return res;
        }


        public static bool checkDuplicate(int mid,int pid)
        {
            ModalityProcedure pt = new ModalityProcedure();
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("SELECT * FROM PROCEDURETOMODALITY WHERE MODALITYID= " + mid + " AND PROCEDUREID= " +pid, conn);
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    return false;
                }
               

            }
            catch
            {
            }
            finally
            {
                conn.Close();
            }
             return true;

        }
        public static string deleteModProc(int num)
        {
            string res = "";

            OracleConnection con = new OracleConnection(RISDB.OracleRIS.GetConnectionString());

            try
            {
                con.Open();
                OracleCommand cmd = new OracleCommand("DELETE PROCEDURETOMODALITY WHERE NUM = :NUM ", con);
                cmd.Parameters.Add(new OracleParameter("NUM", num));
                cmd.ExecuteNonQuery();
            }
            catch(OracleException ex)
            {
                res = ex.ToString();
            }
            catch
            {
                res = "حدث خطأ";
            }
            finally
            {
                con.Close();
            }

            return res;
                 
        }

        public static ModalityProcedure select(int num)
        {
            ModalityProcedure pt = new ModalityProcedure();
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("SELECT * FROM PROCEDURETOMODALITY WHERE NUM= " + num, conn);
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    
                    if (!dr.IsDBNull(2))
                        pt.num = Int32.Parse(dr.GetValue(2).ToString());
                    if (!dr.IsDBNull(0))
                        pt.ModalityId= Int32.Parse(dr.GetValue(0).ToString());
                    if (!dr.IsDBNull(1))
                        pt.ProcedureId = Int32.Parse(dr.GetValue(1).ToString());
                }
            }
            catch
            {
            }
            finally
            {
                conn.Close();
            }
            return pt;

        }

    }
}