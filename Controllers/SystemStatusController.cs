using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Web;
using System.Web.Mvc;
using RIS.Resources;
using System.Net;

namespace RIS.Controllers
{
    public class SystemStatusController : Controller
    {

        
        public ActionResult Index()
        {
            ConnectionConfigs risConfig = ConnectionConfigs.getConfig();

           

            #region  check pacs

            String serverIsOnline = Res.NoNetworkOnPacs;
            try
            {
                Ping myPing = new Ping();
                PingReply reply = myPing.Send(risConfig.pacsIp, 1000);
                if (reply.Status.ToString() == "Success")
                {
                    //check if service is online
                    //  bool serverIsOnline = true;
                    TcpClient tc = null;
                    try
                    {
                        tc = new TcpClient(risConfig.pacsIp, 104);
                        Console.Write("Master is Oline\r\n");
                        tc.Close();
                        serverIsOnline = Res.PacsServiceOn;
                    }
                    catch (SocketException se)
                    {
                        //check slave
                        //try
                        //{
                        //    tc = new TcpClient(risConfig.pacsIp2, 104);
                        //    Console.Write("Master is Oline\r\n");
                        //    tc.Close();
                        //}
                        serverIsOnline = Res.PacsServiceOff;
                        Console.Write("Master is Offline\r\n");
                    }
                    finally
                    {
                        if (tc != null)
                        {
                            tc.Close();
                        }
                    }


                    //
                }
            }
            catch
            {

            }
            ViewData["PacsStatus"] = serverIsOnline;

            #endregion



            #region  check ris
            string risServer = "192.168.3.8";
             serverIsOnline = Res.NoNetworkOnPacs;
            try
            {
                Ping myPing = new Ping();
                PingReply reply = myPing.Send(risServer, 1000);
                if (reply.Status.ToString() == "Success")
                {
                    //check if service is online
                    //  bool serverIsOnline = true;
                    TcpClient tc = null;
                    try
                    {
                        tc = new TcpClient(risServer, 1004);
                        Console.Write("Master is Oline\r\n");
                        tc.Close();
                        serverIsOnline = Res.PacsServiceOn;
                    }
                    catch (SocketException se)
                    {
                        //check slave
                        //try
                        //{
                        //    tc = new TcpClient(risConfig.pacsIp2, 104);
                        //    Console.Write("Master is Oline\r\n");
                        //    tc.Close();
                        //}
                        serverIsOnline = Res.PacsServiceOff;
                        Console.Write("Master is Offline\r\n");
                    }
                    finally
                    {
                        if (tc != null)
                        {
                            tc.Close();
                        }
                    }


                    //
                }
            }
            catch
            {

            }
            ViewData["RISStatus"] = serverIsOnline;

            #endregion



            #region  check Oracle

            serverIsOnline = Res.NoNetworkOnPacs;
            try
            {
                Ping myPing = new Ping();
                PingReply reply = myPing.Send(risConfig.oracleIp, 1000);
                if (reply.Status.ToString() == "Success")
                {
                    //check if service is online
                    //  bool serverIsOnline = true;
                    TcpClient tc = null;
                    try
                    {
                        tc = new TcpClient(risConfig.oracleIp, risConfig.oraclePort);
                        Console.Write("Master is Oline\r\n");
                        tc.Close();
                        serverIsOnline = Res.PacsServiceOn;
                    }
                    catch (SocketException se)
                    {
                        //check slave
                        //try
                        //{
                        //    tc = new TcpClient(risConfig.pacsIp2, 104);
                        //    Console.Write("Master is Oline\r\n");
                        //    tc.Close();
                        //}
                        serverIsOnline = Res.PacsServiceOff;
                        Console.Write("Master is Offline\r\n");
                    }
                    finally
                    {
                        if (tc != null)
                        {
                            tc.Close();
                        }
                    }


                    //
                }
            }
            catch
            {

            }
            ViewData["OracleStatus"] = serverIsOnline;

            #endregion


            //details
            serverIsOnline = Res.NoNetworkOnPacs;

            try
            {
                Ping myPing = new Ping();
                PingReply reply = myPing.Send("", 1000);
                if (reply.Status.ToString() == "Success")
                {
                    
                    TcpClient tc = null;
                    try
                    {
                        tc = new TcpClient("", 104);
                        Console.Write("Master is Oline\r\n");
                        tc.Close();
                        serverIsOnline = Res.PacsServiceOn;
                    }
                    catch (SocketException se)
                    {
                        serverIsOnline = Res.PacsServiceOff;
                        Console.Write("Master is Offline\r\n");
                    }
                    finally
                    {
                        if (tc != null)
                        {
                            tc.Close();
                        }
                    }


                    //
                }
            }
            catch
            {

            }
            ViewData["pacs1"] = serverIsOnline;





            //pacs2 change  ip

            string pacs2 = "192.168.3.251";

            //details
            serverIsOnline = Res.NoNetworkOnPacs;

            try
            {
                ViewData["pacs2"] = 0;
                Ping myPing = new Ping();
                PingReply reply = myPing.Send(pacs2, 1000);
                if (reply.Status.ToString() == "Success")
                {
                    ViewData["pacs2"] = serverIsOnline;

                    TcpClient tc = null;
                    try
                    {
                        tc = new TcpClient(pacs2, 104);
                        Console.Write("Master is Oline\r\n");
                        tc.Close();
                        serverIsOnline = Res.PacsServiceOn;
                    }
                    catch (SocketException se)
                    {
                        serverIsOnline = Res.PacsServiceOff;
                        Console.Write("Master is Offline\r\n");
                    }
                    finally
                    {
                        if (tc != null)
                        {
                            tc.Close();
                        }
                    }


                    //
                }
            }
            catch
            {

            }
            ViewData["pacs2"] = serverIsOnline;



            //oracle1
            string oracle1 = "192.168.3.9";
            serverIsOnline = Res.NoNetworkOnPacs;
            try
            {
                Ping myPing = new Ping();
                PingReply reply = myPing.Send(oracle1, 1000);
                if (reply.Status.ToString() == "Success")
                {
                    //check if service is online
                    TcpClient tc = null;
                    try
                    {
                        tc = new TcpClient(oracle1, risConfig.oraclePort);
                        Console.Write("Master is Oline\r\n");
                        tc.Close();
                        serverIsOnline = Res.PacsServiceOn;
                    }
                    catch (SocketException se)
                    {
                        serverIsOnline = Res.PacsServiceOff;
                        Console.Write("Master is Offline\r\n");
                    }
                    finally
                    {
                        if (tc != null)
                        {
                            tc.Close();
                        }
                    }


                    //
                }
            }
            catch
            {

            }
            ViewData["oracle1"] = serverIsOnline;

            //oracle1 
            //todo change ip
            string oracle2 = "192.168.3.242";
            serverIsOnline = Res.NoNetworkOnPacs;
            try
            {
                Ping myPing = new Ping();
                PingReply reply = myPing.Send(oracle2, 1000);
                if (reply.Status.ToString() == "Success")
                {
                    //check if service is online
                    TcpClient tc = null;
                    try
                    {
                        tc = new TcpClient(oracle2, risConfig.oraclePort);
                        Console.Write("Master is Oline\r\n");
                        tc.Close();
                        serverIsOnline = Res.PacsServiceOn;
                    }
                    catch (SocketException se)
                    {
                        serverIsOnline = Res.PacsServiceOff;
                        Console.Write("Master is Offline\r\n");
                    }
                    finally
                    {
                        if (tc != null)
                        {
                            tc.Close();
                        }
                    }


                    //
                }
            }
            catch
            {

            }
            ViewData["oracle2"] = serverIsOnline;


            //ris1
            serverIsOnline = Res.NoNetworkOnPacs;
            string ris1 = "192.168.3.249";
            try
            {
                Ping myPing = new Ping();
                PingReply reply = myPing.Send(ris1, 1000);
                if (reply.Status.ToString() == "Success")
                {

                    TcpClient tc = null;
                    try
                    {
                        tc = new TcpClient(ris1, 80);
                        Console.Write("Master is Oline\r\n");
                        tc.Close();
                        serverIsOnline = Res.PacsServiceOn;
                    }
                    catch (SocketException se)
                    {
                        serverIsOnline = Res.PacsServiceOff;
                        Console.Write("Master is Offline\r\n");
                    }
                    finally
                    {
                        if (tc != null)
                        {
                            tc.Close();
                        }
                    }


                    //
                }
            }
            catch
            {

            }
            ViewData["ris1"] = serverIsOnline;

            //ris2
            serverIsOnline = Res.NoNetworkOnPacs;
            string ris2 = "192.168.3.248";
            try
            {
                Ping myPing = new Ping();
                PingReply reply = myPing.Send(ris2, 1000);
                if (reply.Status.ToString() == "Success")
                {

                    TcpClient tc = null;
                    try
                    {
                        tc = new TcpClient(ris2, 80);
                        Console.Write("Master is Oline\r\n");
                        tc.Close();
                        serverIsOnline = Res.PacsServiceOn;
                    }
                    catch (SocketException se)
                    {
                        serverIsOnline = Res.PacsServiceOff;
                        Console.Write("Master is Offline\r\n");
                    }
                    finally
                    {
                        if (tc != null)
                        {
                            tc.Close();
                        }
                    }


                    //
                }
            }
            catch
            {

            }
            ViewData["ris2"] = serverIsOnline;



            return View();
        }
    }
}