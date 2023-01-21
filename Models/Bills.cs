using Oracle.DataAccess.Client;
using RIS.ViewModels;
using RISDB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RIS.Models
{
    public class Bills
    {
        public PatientDetails ptDetails { get; set; }
        public int billId { get; set; }
        [Display(ResourceType = typeof(Resources.Res), Name = "PATIENTNAMEParameter")]
        public int patientID { get; set; }
        [Display(ResourceType = typeof(Resources.Res), Name = "AppDate")]
        public string billDate { get; set; }
        [Display(ResourceType = typeof(Resources.Res), Name = "AppDate")]
        public string fromDate { get; set; }
        [Display(ResourceType = typeof(Resources.Res), Name = "tshAcceptanceType")]
        public string accTypeName { get; set; }

        [Display(ResourceType = typeof(Resources.Res), Name = "mnmfrom")]
        public string toDate { get; set; }
        [Display(ResourceType = typeof(Resources.Res), Name = "mnmto")]
        public string billInsertUser { get; set; }
        [Display(ResourceType = typeof(Resources.Res), Name = "status")]
        public string billStatus { get; set; }
        public int billTotValue { get; set; }
        public int discountValue { get; set; }
        public int taxValue { get; set; }
        public int billValue { get; set; }
        [Display(ResourceType = typeof(Resources.Res), Name = "InsertDateParameter")]
        public DateTime? billInsertDate { get; set; }
        [Display(ResourceType = typeof(Resources.Res), Name = "UpdateDate")]
        public DateTime? updateDate { get; set; }
        [Display(ResourceType = typeof(Resources.Res), Name = "UpdateUser")]
        public string UpdatetUser { get; set; }
        [Display(ResourceType = typeof(Resources.Res), Name = "UpdateDeleteReason")]
        public string UpdateDeleteReason { get; set; }
        public Patient parentR
        {
            get
            {
                return Patient.Select(patientID);
            }
        }
        [Display(ResourceType = typeof(Resources.Res), Name = "AdditionalCosts")]

        public int additCosts { get; set; }
        public static List<Bills> getBillItemsByPatient(double page, out double count, double RowsPerPage, int patientId, int? billStatus, string billDate)
        {
            List<Bills> res = new List<Bills>();

            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());

            try
            {
                conn.Open();
                string idx1 = (RowsPerPage * (page - 1) + 1).ToString();
                string idx2 = (RowsPerPage * page).ToString();
                string whereStr = "SELECT BILLS.BILLID, BILLS.STATUS, BILLS.INSERTDATE, BILLS.TOTBILLVALUE, BILLS.BILLDATE FROM BILLS WHERE BILLS.PATIENTID =" + patientId;

                if (billStatus != null)
                    whereStr += " And BILLS.STATUS = '" + billStatus + "' ";
                if (billDate != null && billDate != "")
                    whereStr += " And to_date(BILLS.BILLDATE,'yyyymmddhh24:mi:ss')-1 <= to_date('" + billDate + "','yyyymmdd') And to_date(BILLS.BILLDATE,'yyyymmddhh24:mi:ss')+1 >= to_date('" + billDate + "','yyyymmdd') ";

                whereStr += " ORDER BY BILLS.INSERTDATE DESC";
                string sql = "SELECT * FROM (SELECT T.*, ROWNUM RID FROM (" + whereStr + ") T)WHERE RID BETWEEN " + idx1 + " AND " + idx2;

                string countQuery = "SELECT COUNT(*) FROM (" + whereStr + ")";
                OracleCommand cmdCnt = new OracleCommand(countQuery, conn);
                count = Math.Ceiling(double.Parse(cmdCnt.ExecuteScalar().ToString()) / RowsPerPage);


                OracleCommand cmd = new OracleCommand(sql, conn);
                OracleDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Bills u = new Bills();
                    if (!dr.IsDBNull(0))
                        u.billId = int.Parse(dr.GetValue(0).ToString());
                    if (!dr.IsDBNull(1))
                        u.billStatus = dr.GetValue(1).ToString();
                    if (!dr.IsDBNull(2))
                        u.billInsertDate = dr.GetDateTime(4);
                    if (!dr.IsDBNull(5))
                        u.billTotValue = int.Parse(dr.GetValue(5).ToString());
                    if (!dr.IsDBNull(6))
                        u.billDate = dr.GetValue(6).ToString();
                    res.Add(u);
                }

                return res;
            }

            finally
            {
                conn.Close();
            }
        }

        public static string Insert(Bills bill)
        {
            string res = "";
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                string qr = "Begin" +
                            "  insert into BILLS" +

                            "( BILLID, BILLVALUE, TAXVALUE, DISCOUNTVALUE, TOTBILLVALUE, PATIENTID, BILLDATE, STATUS, INSERTDATE, INSERTUSER) " +
                            " values " +
                            " (:BILLID, :BILLVALUE, :TAXVALUE, :DISCOUNTVALUE, :TOTBILLVALUE, :PATIENTID, :BILLDATE, :STATUS, :INSERTDATE, :INSERTUSER); " +
                            "End;";
                OracleCommand cmd = new OracleCommand(qr, conn);
                OracleParameter[] param =  {
                                            new OracleParameter("BILLID", bill.billId),
                                            new OracleParameter("BILLVALUE", bill.billValue),
                                            new OracleParameter("TAXVALUE", bill.taxValue),
                                            new OracleParameter("DISCOUNTVALUE", bill.discountValue),
                                            new OracleParameter("TOTBILLVALUE", bill.discountValue),
                                            new OracleParameter("PATIENTID", bill.patientID),
                                            new OracleParameter("BILLDATE", bill.billDate),
                                            new OracleParameter("STATUS", bill.billStatus),
                                            new OracleParameter("INSERTDATE", bill.billInsertDate),
                                            new OracleParameter("INSERTUSER",bill.billInsertUser)
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
        public static int calculateBillValue(Bills bill)
        {
            int v = 0;
            List<Appoinments> listApps = bill.ptDetails.patientApps;
            List<Radiology> listRads = bill.ptDetails.patientOrders;
            if (listApps != null)
                foreach (var va in listApps)
                {
                    v = v + va.appCost;
                }
            if (listRads != null)
                foreach (var va in listRads)
                {
                    v = v + va.radCost;
                }
            return v;
        }

    }
}