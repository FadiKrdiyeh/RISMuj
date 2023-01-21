using System;
using Oracle.DataAccess.Client;
using System.Text;
using System.Security.Cryptography;
using System.Data;
using RIS;

namespace RISDB
{
    public static class OracleRIS
    {
        public static bool CheckConnection(OracleConnection conn)
        {
                try
                {
                conn.Open();
                conn.Close();
                return true;
                }
                catch
                {
                    return false;
                }
        }
        public static string GetConnectionString()
        {
            ConnectionConfigs risConfig = ConnectionConfigs.getConfig();

            //    return "Data Source=" + risConfig.oracleIp + ":" + risConfig.oraclePort.ToString() + "/ORCL;Persist Security Info=True;User ID=RIS;Password=RIS";
           string ds= "Data Source=" + risConfig.oracleIp + ":" + risConfig.oraclePort.ToString() + "/orcl;Persist Security Info=True;User ID=RIS;Password=RIS";
            //OracleConnection conn = new OracleConnection(ds);
            //if(conn.State.)
            //if (!OracleRIS.CheckConnection(new OracleConnection(ds)))
            //    ds = "Data Source=" + "" + ":" + "1521" + "/orcl;Persist Security Info=True;User ID=RIS;Password=RIS";
            return ds;
        }

        public static int GetOracleSequenceValue(string seqName)
        {
            int seqVal = -1;

            String connString = GetConnectionString();
            OracleConnection conn = new OracleConnection(connString);
            conn.Open();

            OracleCommand cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandText = "select "+ seqName + ".nextval from dual";
            cmd.CommandType = CommandType.Text;
            try {
                seqVal = Int32.Parse(cmd.ExecuteScalar().ToString());
            }
            finally
            {
                conn.Close();
            }
            return seqVal;
        }
        public static int GetOracleCurrentValue(string seqName)
        {
            int seqVal = -1;
            String connString = GetConnectionString();
            OracleConnection conn = new OracleConnection(connString);
            try
            {

            conn.Open();

            OracleCommand cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandText = "select max(NUM)  from ORDERS";
            cmd.CommandType = CommandType.Text;

                seqVal = Int32.Parse(cmd.ExecuteScalar().ToString());
            }
            catch
            {
                return seqVal;
            }
            finally
            {
                conn.Close();
            }
            return seqVal;
        }
    }
}
