using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RIS
{
    /// <summary>
    /// Class for permissions in RIS
    /// </summary>
    public class Perms
    {
        /// <summary>
        /// permission of accessing users index view
        /// </summary>
        public static int UserIndex = 1;
        /// <summary>
        /// permission of creating a new user
        /// </summary>
        public static int UserCreate = 2;
        /// <summary>
        /// permission of editing user's information
        /// </summary>
        public static int UserEdit = 3;
        /// <summary>
        /// permission of deleting user's information
        /// </summary>
        public static int UserDelete = 4;
        /// <summary>
        /// permission of accessing user's details
        /// </summary>
        public static int UserDtails = 5;

        /// <summary>
        /// permission of accessing patients index view
        /// </summary>
        public static int PatientIndex = 6;
        /// <summary>
        /// permission of creating a new patient
        /// </summary>
        public static int PatientCreate = 7;
        /// <summary>
        /// permission of editing patient's information
        /// </summary>
        public static int PatientEdit = 8;
        /// <summary>
        /// permission of deleting a patient
        /// </summary>
        public static int PatientDelete = 9;
        /// <summary>
        /// permission of accessing patient's details
        /// </summary>
        public static int PatientDetails = 10;

        /// <summary>
        /// permission of accessing radiology orders index view
        /// </summary>
        public static int RadiologyIndex = 11;
        /// <summary>
        /// permission of creationg a new immediate order
        /// </summary>
        public static int RadiologyCreate = 12;
        /// <summary>
        /// permission of creating a schedualed order
        /// </summary>
        public static int RadiologyCreateSchedualed = 13;
        /// <summary>
        /// permission of editing an order
        /// </summary>
        public static int RadiologyEdit = 14;
        /// <summary>
        /// permission of deleting an order
        /// </summary>
        public static int RadiologyDelete = 15;
        /// <summary>
        /// permission of accessing an order details
        /// </summary>
        public static int RadiologyDetails = 16;
        /// <summary>
        /// permission of accessing radiology orders status index view
        /// </summary>
        public static int RadiologyOrderStatus = 17;

        /// <summary>
        /// permission of accessing the departments index view
        /// </summary>
        public static int DepartmentIndex = 18;
        /// <summary>
        /// permission of creating a new department
        /// </summary>
        public static int DepartmentCreate = 19;
        /// <summary>
        /// permission of editing a department information
        /// </summary>
        public static int DepartmentEdit = 20;
        /// <summary>
        /// permission of deleting a department
        /// </summary>
        public static int DepartmentDelete = 21;

        /// <summary>
        /// permission of accessing the modality index view
        /// </summary>
        public static int ModalityIndex = 22;
        /// <summary>
        /// permission of creating a new modality
        /// </summary>
        public static int ModalityCreate = 23;
        /// <summary>
        /// permission of editing a modality
        /// </summary>
        public static int ModalityEdit = 24;
        /// <summary>
        /// permission of deleting a modality
        /// </summary>
        public static int ModalityDelete = 25;

        /// <summary>
        /// permission of accessing the modality types index view
        /// </summary>
        public static int ModalityTypeIndex = 26;
        /// <summary>
        /// permission of creating a new modality type
        /// </summary>
        public static int ModalityTypeCreate = 27;
        /// <summary>
        /// permission of editing a modality type
        /// </summary>
        public static int ModalityTypeEdit = 28;
        /// <summary>
        /// permission of deleting a modality type
        /// </summary>
        public static int ModalityTypeDelete = 29;

        /// <summary>
        /// permission of the groups index view
        /// </summary>
        public static int GroupIndex = 30;
        /// <summary>
        /// permission of creating a new group
        /// </summary>
        public static int GroupCreate = 31;
        /// <summary>
        /// permission of editing a group
        /// </summary>
        public static int GroupEdit = 32;
        /// <summary>
        /// permission of deleting a group
        /// </summary>
        public static int GroupDelete = 33;
        /// <summary>
        /// permission of accessing the group details
        /// </summary>
        public static int GroupDetailsPerm = 34;

        /// <summary>
        /// permission of accessing the procedure index view
        /// </summary>
        public static int ProcedureIndex = 35;
        /// <summary>
        /// permission of creating a new procedure
        /// </summary>
        public static int ProcedureCreate = 36;
        /// <summary>
        /// permission of editing a procedure
        /// </summary>
        public static int ProcedureEdit = 37;
        /// <summary>
        /// permission of deleting a procedure
        /// </summary>
        public static int ProcedureDelete = 38;

        /// <summary>
        /// permission of accessing the statistics view
        /// </summary>
        public static int StatsIndex = 39;
        /// <summary>
        /// permission of accessing the patients' statistics view
        /// </summary>
        public static int StatsPatient = 40;
        /// <summary>
        /// permission of accessing the orders' statistics view
        /// </summary>
        public static int StatsOrders = 41;

        /// <summary>
        /// permission of accessing the permissions view
        /// </summary>
        public static int PermsIndex = 42;

        public static int ClinicAppoinmentIndex = 156;

        /// <summary>
        /// permission of editing an order
        /// </summary>
        public static int ClinicAppoinmentEdit = 157;
        /// <summary>
        /// permission of deleting an order
        /// </summary>
        public static int ClinicAppoinmentDelete = 158;
        /// <summary>
        /// permission of accessing an order details
        /// </summary>
        public static int ClinicAppoinmentDetails = 159;



        public static int ClinicAppoinmentCreate = 160;
        public static int BillsIndex = 61;
		public static int BillsEdit = 62;
		public static int BillsDetails = 63;
		public static int BillsDelete = 64;
		public static int BillsCreate = 65;

		public static int NewsIndex = 66;

        public static int AppStatsIndex = 167;
        public static int AppStatsPatient = 168;

        public static int StatsApps = 169;
        public static int ClinicAppoinmentAudit = 170;


    }
}