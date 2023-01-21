using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Xml.Serialization; //Add 

using Oracle.DataAccess.Client;
using System.Text;
using System.Security.Cryptography;
using System.Data;
using RIS;

using RISDB;
using System.Xml;
using System.Net.NetworkInformation;
using System.Management;

namespace RIS
{
    [Serializable]
    [XmlRoot("ConnectionConfigs"), XmlType("ConnectionConfigs")]
    public class ConnectionConfigs
    {
        public string pacsIp { get; set; }
        public int pacsPort { get; set; }

        public string oracleIp { get; set; }
        public int oraclePort { get; set; }

        public static string SCHEDUALED = "0";
        public static string STARTED = "1";
        public static string COMPLETED = "2";
        public static string Waiting = "5";
        public static string VIEWED = "3";
        public static string REPORTED = "4";

        public static string PRIMARY = "0";
        public static string UPDATED = "1";
        public static string DELETED = "2";

        public static string FirstCharToUpper(string input)
        {
            switch (input)
            {
                case null: throw new ArgumentNullException(nameof(input));
                case "": throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
                default: return input.First().ToString().ToUpper() + input.Substring(1);
            }
        }

        public static ConnectionConfigs getConfig()
        {
            string xmldata = HttpContext.Current.Server.MapPath("~/PACS_SERVER_CONF.xml");
            DataSet ds = new DataSet();
            //read the xml data from file using dataset
            ds.ReadXml(xmldata);
            //get data from dataset,convert and store it in list. 
            var risConfig = new List<ConnectionConfigs>();
            risConfig = (from rows in ds.Tables[0].AsEnumerable()
                         select new ConnectionConfigs
                         {
                             pacsIp = (rows[0].ToString()),
                             pacsPort = int.Parse(rows[1].ToString()),
                             oracleIp = rows[2].ToString(),
                             oraclePort = int.Parse(rows[3].ToString())
                         }).ToList();
            return risConfig[0];
        }

        public static string GetMacAddress()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT ProcessorId FROM Win32_Processor");
            string xcv = "";
            ManagementObjectCollection information = searcher.Get();
            foreach (ManagementObject obj in information)
            {
                foreach (PropertyData data in obj.Properties)
                    xcv += (data.Value.ToString());
            }
            xcv = xcv + "()";
            searcher = new ManagementObjectSearcher("SELECT Product, SerialNumber FROM Win32_BaseBoard");

            information = searcher.Get();

            foreach (ManagementObject obj in information)
            {
                foreach (PropertyData data in obj.Properties)
                    xcv += (data.Value.ToString());
            }
            xcv = xcv + "()";

            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                // Only consider Ethernet network interfaces
                if (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet &&
                    nic.OperationalStatus == OperationalStatus.Up)
                {
                    nic.GetPhysicalAddress();
                    return xcv = xcv + nic.GetPhysicalAddress().ToString();

                }
            }
            return null;
        }

        public static string getPacsTime()
        {
            ManagementObjectSearcher searcher1 = new ManagementObjectSearcher("SELECT * FROM Win32_LocalTime");

            string pacsDateTime = "";
            ManagementObjectCollection information = searcher1.Get();
            foreach (ManagementObject queryObj in searcher1.Get())
            {

                //     Console.WriteLine("Date: {0}-{1}-{2}", queryObj["Year"], queryObj["Month"], queryObj["Day"]);
                //    Console.WriteLine("Time: {0}:{1}:{2}", queryObj["Hour"], queryObj["Minute"], queryObj["Second"]);
                string mo = queryObj["Month"].ToString();
                if (queryObj["Month"].ToString().Length == 1)
                    mo = "0" + queryObj["Month"].ToString();
                string da = queryObj["Day"].ToString();
                if (queryObj["Day"].ToString().Length == 1)
                    da = "0" + queryObj["Day"].ToString();
                string ho = queryObj["Hour"].ToString();
                if (queryObj["Hour"].ToString().Length == 1)
                    ho = "0" + queryObj["Hour"].ToString();
                string mi = queryObj["Minute"].ToString();
                if (queryObj["Minute"].ToString().Length == 1)
                    mi = "0" + queryObj["Minute"].ToString();
                string se = queryObj["Second"].ToString();
                if (queryObj["Second"].ToString().Length == 1)
                    se = "0" + queryObj["Second"].ToString();
                pacsDateTime = queryObj["Year"].ToString() + mo + da + ho + mi + se;

                // DateTime dod = DateTime.Parse(pacsDateTime);
                // return pacsDateTime;

            }

            return pacsDateTime;





            //return System.DateTime.Now.ToString("yyyyMMddHmmss");
            /*
            string pacsDateTime = "";
            try
            {
                string pc = "";
            ConnectionOptions connection = new ConnectionOptions();
            connection.Username = "Server-5\\Administrator";
            connection.Password = "p@ssw0rd";
            //connection.Authority = "ntlmdomain:" + domain;

            //string wmipath = string.Format("\\\\{0}\\root\\CIMV2", pc);
            //ManagementScope scope = new ManagementScope(wmipath);
            ManagementScope scope = new ManagementScope(
            string.Format("\\\\{0}\\root\\CIMV2", pc), connection);

            scope.Connect();

            ObjectQuery query = new ObjectQuery(
                "SELECT * FROM Win32_LocalTime");

            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);

            foreach (ManagementObject queryObj in searcher.Get())
            {

                    //     Console.WriteLine("Date: {0}-{1}-{2}", queryObj["Year"], queryObj["Month"], queryObj["Day"]);
                    //    Console.WriteLine("Time: {0}:{1}:{2}", queryObj["Hour"], queryObj["Minute"], queryObj["Second"]);
                    string mo = queryObj["Month"].ToString();
                    if (queryObj["Month"].ToString().Length == 1)
                        mo = "0" + queryObj["Month"].ToString();
                    string da = queryObj["Day"].ToString();
                    if (queryObj["Day"].ToString().Length == 1)
                        da = "0" + queryObj["Day"].ToString();
                    string ho = queryObj["Hour"].ToString();
                    if (queryObj["Hour"].ToString().Length == 1)
                        ho = "0" + queryObj["Hour"].ToString();
                    string mi = queryObj["Minute"].ToString();
                    if (queryObj["Minute"].ToString().Length == 1)
                        mi= "0" + queryObj["Minute"].ToString();
                    string se= queryObj["Second"].ToString();
                    if (queryObj["Second"].ToString().Length == 1)
                        se = "0" + queryObj["Second"].ToString();
                    pacsDateTime = queryObj["Year"].ToString() + mo + da + ho+mi+se;
                    // DateTime dod = DateTime.Parse(pacsDateTime);
                    return pacsDateTime;

                }
            }
            catch (ManagementException err)
            {
                Console.WriteLine("An error occurred while querying for WMI data: " + err.Message);
            }
            catch (System.UnauthorizedAccessException unauthorizedErr)
            {
                Console.WriteLine("Connection error (user name or password might be incorrect): " + unauthorizedErr.Message);
            }





            try
            {
                string pc = "192.168.3.251";
                ConnectionOptions connection = new ConnectionOptions();
                connection.Username = "Server-6\\Administrator";
                connection.Password = "p@ssw0rd";
                //connection.Authority = "ntlmdomain:" + domain;

                //string wmipath = string.Format("\\\\{0}\\root\\CIMV2", pc);
                //ManagementScope scope = new ManagementScope(wmipath);
                ManagementScope scope = new ManagementScope(
                string.Format("\\\\{0}\\root\\CIMV2", pc), connection);

                scope.Connect();

                ObjectQuery query = new ObjectQuery(
                    "SELECT * FROM Win32_LocalTime");

                ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);

                foreach (ManagementObject queryObj in searcher.Get())
                {

                    //     Console.WriteLine("Date: {0}-{1}-{2}", queryObj["Year"], queryObj["Month"], queryObj["Day"]);
                    //    Console.WriteLine("Time: {0}:{1}:{2}", queryObj["Hour"], queryObj["Minute"], queryObj["Second"]);
                    string mo = queryObj["Month"].ToString();
                    if (queryObj["Month"].ToString().Length == 1)
                        mo = "0" + queryObj["Month"].ToString();
                    string da = queryObj["Day"].ToString();
                    if (queryObj["Day"].ToString().Length == 1)
                        da = "0" + queryObj["Day"].ToString();
                    string ho = queryObj["Hour"].ToString();
                    if (queryObj["Hour"].ToString().Length == 1)
                        ho = "0" + queryObj["Hour"].ToString();
                    string mi = queryObj["Minute"].ToString();
                    if (queryObj["Minute"].ToString().Length == 1)
                        mi = "0" + queryObj["Minute"].ToString();
                    string se = queryObj["Second"].ToString();
                    if (queryObj["Second"].ToString().Length == 1)
                        se = "0" + queryObj["Second"].ToString();
                    pacsDateTime = queryObj["Year"].ToString() + mo + da + ho + mi + se;
                    // DateTime dod = DateTime.Parse(pacsDateTime);
                }
            }
            catch (ManagementException err)
            {
                Console.WriteLine("An error occurred while querying for WMI data: " + err.Message);
            }
            catch (System.UnauthorizedAccessException unauthorizedErr)
            {
                Console.WriteLine("Connection error (user name or password might be incorrect): " + unauthorizedErr.Message);
            }

            return pacsDateTime;


        }
        */
        }

		public string audioFilesDirectory { set; get; }		//usama
	}

    [Serializable]
    [XmlRoot("trialConfigs"), XmlType("trialConfigs")]
    public class trialConfigs
    {
        public static bool getOrderNumbers()
        {
            return true;
            int x = OracleRIS.GetOracleCurrentValue("ORDERS_SEQ");

            if (x < 80000)
                return true;

            return false;
        }

        public static bool checkperiod()
        {
            return true;
            int x = int.Parse(DateTime.Now.ToString("dd"));		// the day number in month

            var doc = new XmlDocument();
            string xmldata = HttpContext.Current.Server.MapPath("~/Language.xml");
            doc.Load(xmldata);
            XmlNode node = doc.SelectSingleNode("trialParameters");

            XmlNode c = node.SelectSingleNode("count");
            XmlNode d = node.SelectSingleNode("dayCount");

            int cc = int.Parse(c.InnerText.ToString());
            int dd = int.Parse(d.InnerText.ToString());

            if (cc >= 1)
                return false;

            if (x != dd)
            {
                d.InnerText = x.ToString();
                cc++;
                c.InnerText = cc.ToString();
            }

            doc.Save(xmldata);

            return true;
        }
    }
}