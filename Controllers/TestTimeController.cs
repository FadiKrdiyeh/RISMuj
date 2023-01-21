using Oracle.DataAccess.Client;
using RISDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RIS.Controllers
{
    public class TestTimeController : Controller
    {
        // GET: TestTime
        public ActionResult Index()
        {
            string res = "";
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                string qr = "Begin" +
                            "  insert into TESTSRV1" +

                            "( NUM, VAL) " +
                            " values " +
                            " (:NUM, :VAL); " +
                            "End;";
                int s = OracleRIS.GetOracleSequenceValue("TESTSRV1_SEQ");
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
    }
}