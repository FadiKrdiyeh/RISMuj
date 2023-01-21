using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RIS.Models;

namespace RIS.ViewModels
{
    public class PatientDetails
    {
        public Patient patient { get; set; }

        public List<Radiology> patientOrders { get; set; }
        public List<Appoinments> patientApps { get; set; }

        public PatientDetails() { }

        public PatientDetails(Patient pt, List<Radiology> orders, List<Appoinments> apps)
        {
            this.patient = pt;
            this.patientOrders = orders;
            this.patientApps = apps;
        }
    }
}