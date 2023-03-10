using RIS.Models;
using System;
using System.Diagnostics;
using System.Web.Mvc;

namespace HL7_TCP.Web
{
    public class HL7Send
    {
        public HL7Send() { }

        //public string SendBatchMessages(RIS.Models.SendHL7ViewModel model)
        //{
        //    var tcpSender = new HL7_TCP.TcpSender { DestinationServer = model.DestinationServer, DestinationPort = model.DestinationPort.Value };

        //    if (tcpSender.DestinationTestConnect())
        //    {
        //        TcpSendResults results = SendMessages(model, tcpSender);

        //        if (results.ExceptionDuringSend.IsNullOrEmpty())
        //        {
        //            return "Successfully sent {0} message{2} to {1}.\r\nTotal time taken: {3}".FormatWith(model.NumMessages,
        //                                                                                                 model.DestinationDetails,
        //                                                                                                 (results.NumberMsgsSent > 1) ? "s" : "",
        //                                                                                                 results.TimeElapsed.ToReadableString());
        //        }
        //        else
        //        {
        //            return "We had a problem sending message {0} to {1}.   {2}".FormatWith(results.NumberMsgsSent + 1,
        //                                                                                    model.DestinationDetails,
        //                                                                                    results.ExceptionDuringSend);
        //        }
        //    }
        //    else
        //    {
        //        return "Couldn't make a connection to {0}.".FormatWith(model.DestinationDetails);
        //    }
        //}
        public bool SendBatchMessages(RIS.Models.SendHL7ViewModel model,string pacsServerIp)
        {
            var tcpSender = new HL7_TCP.TcpSender { DestinationServer = model.DestinationServer, DestinationPort = model.DestinationPort.Value };

            if (tcpSender.DestinationTestConnect())
            {
                TcpSendResults results = SendMessages(model, tcpSender, pacsServerIp);

                if (results.ExceptionDuringSend.IsNullOrEmpty())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public TcpSendResults SendMessages(SendHL7ViewModel model, TcpSender tcpSender,string pacsServerIp)
        {
            int sendMsgCounter = 0;
            Stopwatch s = new Stopwatch();
            try
            {
                s.Start();
                for (sendMsgCounter = 0; sendMsgCounter < model.NumMessages; sendMsgCounter++)
                {
                    tcpSender.SendHL7(model.HL7MessageToSend, pacsServerIp);
                }
                s.Stop();
                return new TcpSendResults { TimeElapsed = s.Elapsed, NumberMsgsSent = sendMsgCounter };
            }
            catch (Exception e)
            {
                return new TcpSendResults { TimeElapsed = s.Elapsed, NumberMsgsSent = sendMsgCounter, ExceptionDuringSend = e.Message };
            }
        }
    }
}