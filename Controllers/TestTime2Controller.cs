using Oracle.DataAccess.Client;
using RISDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RIS.Controllers
{
    public class TestTime2Controller : Controller
    {
        // GET: TestTime2
        public ActionResult Index()
        {

            string res = "";
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                string qr = "Begin" +
                            "  insert into TESTSRV2" +

                            "( NUM, VAL) " +
                            " values " +
                            " (:NUM, :VAL); " +
                            "End;";
                int s = OracleRIS.GetOracleSequenceValue("TESTSRV2_SEQ");
                OracleCommand cmd = new OracleCommand(qr, conn);
                OracleParameter[] param =  {
                                            new OracleParameter("NUM", s),
                                            new OracleParameter("VAL", DateTime.Now)

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
            //    return res;



            return View();
        }

        
        
            public JsonResult getVals()
        {
            string isExists = "false";
            List<tt2> ptList = new List<tt2>();


            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();

                //  OracleCommand cmd = new OracleCommand("SELECT * FROM PATIENT ", conn);

                //  string idx1 = (RowsPerPage * (page - 1) + 1).ToString();
                //  string idx2 = (RowsPerPage * page).ToString();
                string whereStr = "SELECT * FROM  TESTSRV2  ";

               

                OracleCommand cmd = new OracleCommand(whereStr, conn);
                OracleDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {

                    tt2 pt = new tt2();
                    #region Data
                    if (!dr.IsDBNull(0))
                        pt.y = Int32.Parse(dr.GetValue(0).ToString());
                    if (!dr.IsDBNull(1))
                        pt.x = DateTime.Parse(dr.GetValue(1).ToString());
                    
                    #endregion
                    ptList.Add(pt);
                }
            }
            finally
            {
                conn.Close();
            }




            //foreach (Patient p in mtList)
            //{

            //}
            //if (pt.num > 0)
            //{
            //    Patient pp = Patient.Select(pt.num);
            //    isExists = pt.num + "-.-" + pt.firstname + "-.-" + pt.lastname;
            //}
            return Json(ptList);
            //   return isExists;
        }
    }
    public class tt2
    {
        public DateTime x;
        public int y;
    }
}