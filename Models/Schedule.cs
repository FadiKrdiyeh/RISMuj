using Oracle.DataAccess.Client;
using RISDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class Schedule
    {
        
        public static SheduleData getOrdersByPeriod(string date)
        {
            SheduleData so = new Models.SheduleData();
            so.tdId = date;
            string[] temp = date.Split('_');
            string myDate = temp[0].Replace("-", "");
            string qr = "SELECT ORDERS.NUM from ORDERS where to_date(STARTDATE, 'yyyymmddhh24:mi:ss') >=to_date('"+myDate+temp[1]+temp[2]+"00"+"','yyyymmddhh24:mi:ss')";
            qr += " AND to_date(STARTDATE, 'yyyymmddhh24:mi:ss') <=to_date('" + myDate + temp[1] + (int.Parse(temp[2]) + 14).ToString() + "00" + "','yyyymmddhh24:mi:ss')";

            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());

            string res = "";

            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand(qr, conn);
                OracleDataReader dr=   cmd.ExecuteReader();
                if (dr.Read())
                    so.orderId = dr.GetValue(0).ToString();
                else
                    so.orderId = "-1";
                conn.Close();
            }
            catch(Exception e)
            {
                conn.Close();

            }
            conn.Close();

            return so;
        }

        public static bool getOrdersByDay(string date)
        {
            bool res = false;
           
            string startDate = date.Replace("-", "");
            string endDate = DateTime.Parse(date).AddDays(1).ToString("yyyy-MM-dd");
            endDate=endDate.Replace("-", "");
            string qr = "SELECT ORDERS.NUM from ORDERS where to_date(STARTDATE, 'yyyymmddhh24:mi:ss') >=to_date('" + startDate +  "000000" + "','yyyymmddhh24:mi:ss')";
            qr += " AND to_date(STARTDATE, 'yyyymmddhh24:mi:ss') <to_date('" + endDate  + "000000" + "','yyyymmddhh24:mi:ss')";

            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());

            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand(qr, conn);
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                    res = true;

            }
            catch (Exception e)
            {
                conn.Close();

            }
            conn.Close();

            return res; 
        }

        public static bool getOrdersByHour(string date,string hour)
        {
            bool res = false;

            string startDate = date.Replace("-", "");
            string endDate = DateTime.Parse(date).AddDays(1).ToString("yyyy-MM-dd");
            endDate = endDate.Replace("-", "");
            string qr = "SELECT ORDERS.NUM from ORDERS where to_date(STARTDATE, 'yyyymmddhh24:mi:ss') >=to_date('" + startDate + hour+"0000" + "','yyyymmddhh24:mi:ss')";
            qr += " AND to_date(STARTDATE, 'yyyymmddhh24:mi:ss') <=to_date('" + startDate + hour +"5959" + "','yyyymmddhh24:mi:ss')";

            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());

            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand(qr, conn);
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                    res = true;

            }
            catch (Exception e)
            {
                conn.Close();

            }
            conn.Close();

            return res;
        }

        public static string getOrdersByStep(string date, string hour,string min,string step,string mod)
        {
            string res = "-1";

            string startDate = date.Replace("-", "");
            string endDate = DateTime.Parse(date).AddDays(1).ToString("yyyy-MM-dd");
            endDate = endDate.Replace("-", "");
            string qr = "";
            if (string.IsNullOrEmpty(mod))
            {
                 qr = "SELECT ORDERS.NUM from ORDERS where to_date(STARTDATE, 'yyyymmddhh24:mi:ss') >=to_date('" + startDate + hour + min+"00" + "','yyyymmddhh24:mi:ss')";
                qr += " AND to_date(STARTDATE, 'yyyymmddhh24:mi:ss') <=to_date('" + startDate + hour +(int.Parse(min)+ int.Parse(step)).ToString("D2")+ "59" + "','yyyymmddhh24:mi:ss')";
            }
            else
            {
                 qr = "SELECT ORDERS.NUM from ORDERS where to_date(STARTDATE, 'yyyymmddhh24:mi:ss') >=to_date('" + startDate + hour + min + "00" + "','yyyymmddhh24:mi:ss')";
                qr += " AND to_date(STARTDATE, 'yyyymmddhh24:mi:ss') <=to_date('" + startDate + hour + (int.Parse(min) + int.Parse(step)).ToString("D2") + "59" + "','yyyymmddhh24:mi:ss') and MODALITYID='"+mod+"' ";
            }


            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());

            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand(qr, conn);
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                    res = dr.GetValue(0).ToString();

            }
            catch (Exception e)
            {
                conn.Close();

            }
            conn.Close();

            return res;
        }

        public static PreviewOrder previewOrder (int id)
        {
            PreviewOrder p = new Models.PreviewOrder();


            string qr = "SELECT ORDERS.MODALITYID,ORDERS.PATIENTID,to_date(ORDERS.STARTDATE, 'yyyymmddhh24:mi:ss') from ORDERS where NUM='" + id + "'";

            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            OracleCommand cmd = new OracleCommand();
            int mid = 0; int pid=0;
            string orderDate="";
            string res = "";

            try
            {
                conn.Open();
                cmd = new OracleCommand(qr, conn);
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    mid = int.Parse(dr.GetValue(0).ToString());
                    pid = int.Parse(dr.GetValue(1).ToString());
                    orderDate = (dr.GetValue(2).ToString());
                }
                conn.Close();
            }
            catch (Exception)
            {
                conn.Close();

            }

            string qr1 = "SELECT MODALITY.NAME from MODALITY where NUM='" + mid + "'";

            try
            {
                conn.Open();
                cmd = new OracleCommand(qr1, conn);
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    p.modalityName = dr.GetValue(0).ToString();
                }
                conn.Close();
            }
            catch (Exception)
            {
                conn.Close();

            }
            string qr2 = "SELECT PATIENT.FIRSTNAME,PATIENT.MIDDLENAME,PATIENT.LASTNAME from PATIENT where NUM='" + pid + "'";

            try
            {
                conn.Open();
                cmd = new OracleCommand(qr2, conn);
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    if (dr.GetValue(0) != null)
                        p.patientName = dr.GetValue(0).ToString();
                    if (dr.GetValue(1) != null)
                        p.patientName +=" "+ dr.GetValue(1).ToString();
                    if (dr.GetValue(2) != null)
                        p.patientName += " " + dr.GetValue(2).ToString();
                }
                conn.Close();
            }
            catch (Exception)
            {
                conn.Close();

            }

            p.id = id.ToString();
            p.orderDate = orderDate;

            return p;
        }

        public static string deleteOrder(int id)
        {
            string res = "";
            string qr = "delete from ORDERS where NUM='" + id + "'";

            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            OracleCommand cmd = new OracleCommand(qr,conn);
            try
            {
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();

            }
            catch (Exception e)
            {
                res = e.ToString();
            }
            return res;
        }

        public static string editOrder(int id,string orderDate)
        {
            string res = "";
            string orderDateVal = orderDate.Replace(":","");
            string qr = "update ORDERS set STARTDATE='"+ orderDateVal + "' "+
                "where NUM='"+id+"'";

            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            OracleCommand cmd = new OracleCommand(qr, conn);
            try
            {
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();

            }
            catch (Exception e)
            {
                res = e.ToString();
            }
            return res;
        }

    }

    public class ScheduleOrder
    {
        public int ID { set; get; }

        public string PatientID { set; get; }

        public string ModalityID { set; get; }

        public string ProcedureID { set; get; }

        public string StudyID { set; get; }

        public string StartDate { set; get; }

        public string EndDate { set; get; }

        public string Status { set; get; }

        public string Doctor { set; get; }

        public string AutoExpireDate { set; get; }

        public string AccessionNumber { set; get; }

        public string DepartementName { set; get; }

        public string DocumnetId { set; get; }

        public int Type { set; get; }

    }

    public class SheduleData
    {
        public string orderId { set; get; }
        public string tdId { set; get; }
    }

    public class PreviewOrder
    {
        public string id { set; get; }
        public string patientName { set; get; }
        public string modalityName { set; get; }
        public string orderDate { set; get; }

    }
}