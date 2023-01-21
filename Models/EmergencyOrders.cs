using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Oracle.DataAccess.Client;
using RISDB;


namespace RIS.Models
{
    public class EmergencyOrders
    {

        public int Id { set; get; }
        public int OrderId { set; get; }
        public string OrderDate { set; get; }

        public EmergencyOrders() { }

        public static string addCashOrder(CashOrders co)
        {
            string res = "";
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                string qr = "Begin " +
                            "  insert into EMERGENCYORDERS " +

                            "( NUM, ORDERID, ORDERDATE) " +
                            " values " +
                            "( :NUM, :ORDERID, :ORDERDATE); " +
                            " End;";
                OracleCommand cmd = new OracleCommand(qr, conn);
                OracleParameter[] param =  {
                                            new OracleParameter("NUM", co.Id),
                                            new OracleParameter("ORDERID", co.OrderId),
                                            new OracleParameter("ORDERDATE", co.OrderDate)
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

        public static string DeleteCashOrder(int i)
        {
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());

            string res = "";

            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("DELETE EMERGENCYORDERS WHERE NUM = :NUM ", conn);
                cmd.Parameters.Add(new OracleParameter("NUM", i));
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

        public static string UpdateCashOrder(CashOrders co)
        {
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            string res = "";
            try
            {
                conn.Open();
                string qr = " Update EMERGENCYORDERS Set " +

                            "( NUM, ORDERID, ORDERDATE) " +
                            " values " +
                            "( :NUM, :ORDERID, :ORDERDATE); " +
                            "End;";
                OracleParameter[] param = {
                                            new OracleParameter("NUM", co.Id),
                                            new OracleParameter("ORDERID", co.OrderId),
                                            new OracleParameter("ORDERDATE", co.OrderDate)
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

        public static List<EmergencyOrders> SelectByDate(DateTime dd)
        {
            List<EmergencyOrders> res = new List<EmergencyOrders>();

            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());

            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("SELECT * FROM EMERGENCYORDERS WHERE ORDERDATE= " + dd, conn);
                OracleDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    EmergencyOrders u = new EmergencyOrders();
                    if (!dr.IsDBNull(0))
                        u.Id = Int32.Parse(dr.GetValue(0).ToString());
                    if (!dr.IsDBNull(1))
                        u.OrderId = Int32.Parse(dr.GetValue(1).ToString());
                    if (!dr.IsDBNull(2))
                        u.OrderDate = dr.GetValue(3).ToString();
                                        

                    res.Add(u);
                }

                return res;
            }
            catch
            {
                return null;
            }

            finally
            {
                conn.Close();
            }

        }

        public static List<EmergencyOrders> selectAll()
        {
            List<EmergencyOrders> res = new List<EmergencyOrders>();

            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());

            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("SELECT * FROM EMERGENCYORDERS ", conn);
                OracleDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    EmergencyOrders u = new EmergencyOrders();
                    if (!dr.IsDBNull(0))
                        u.Id = Int32.Parse(dr.GetValue(0).ToString());
                    if (!dr.IsDBNull(1))
                        u.OrderId = Int32.Parse(dr.GetValue(1).ToString());
                    if (!dr.IsDBNull(2))
                        u.OrderDate = dr.GetValue(2).ToString();

                    res.Add(u);
                }

                return res;
            }
            catch
            {
                return null;
            }

            finally
            {
                conn.Close();
            }
        }

    }
}