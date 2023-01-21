using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HL7_TCP.Extensions;
//using RIS.RIS.Resources;
namespace RIS.Models
{
    public class SendHL7ViewModel
    {
        public SendHL7ViewModel()
        {
            NumMessages = 1;
        }
        public int? NumMessages { get; set; }
        public string HL7MessageToSend { get; set; }
        public int? patientId { get; set; }

        public string patientIdList { get; set; }

        // [Display(ResourceType = typeof(Res), Name = "patientGivenName")]
        public string patientGivenName { get; set; }
        public string patientFamilyName { get; set; }
        public int? commOrderPONum { get; set; }
        public string startDateTime { get; set; }
        public int? obsOrderPFNum { get; set; }
        public string DestinationServer { get; set; }
        public int? DestinationPort { get; set; }

        public string aeTitle { get; set; }
        public string sStationName { get; set; }

        public string DestinationDetails { get { return "{0}:{1}".FormatWith(DestinationServer, DestinationPort); } }
    }
}