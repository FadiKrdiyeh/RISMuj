using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Management;
using RISDB;

namespace RIS
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            //int iiii = OracleRIS.GetOracleSequenceValue("ORDERS_SEQ");

            //string xcv = ConnectionConfigs.GetMacAddress().ToString();

            //if (!xcv.Equals("BFEBFBFF000506E3()0TTDMJ/BN2NVB2/CN7016364801F8/()F48E3890ACFF"))  //dell
            //    return;

            ////Sama Server
            //if (!xcv.Equals("BFEBFBFF0001067ABFEBFBFF0001067A()0NX642..CN1374097B015K.()02BFC0A80308"))  //server2 :192.168.3.248
            //    return;

            // hi if (!xcv.Equals("BFEBFBFF0001067ABFEBFBFF0001067A()0NX642..CN1374097B00ZA.()02BFC0A80308"))  //server1 :192.168.3.249
            // hi return;

            //if (!xcv.Equals("BFEBFBFF0001067ABFEBFBFF0001067A()0NX642..CN1374097B015K.()02BFC0A80308") && !xcv.Equals("BFEBFBFF0001067ABFEBFBFF0001067A()0NX642..CN1374097B00ZA.()02BFC0A80308") && !xcv.Equals("178BFBFF00100F53()1455Base Board Serial Number()02004C4F4F50"))  //server1 and 2 :192.168.3.248/249
            //    return;

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute("SendHL7Message", "SendHL7Message", new { controller = "HL7", action = "SendHL7Message" });
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
