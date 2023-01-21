using Oracle.DataAccess.Client;
using RISDB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RIS.Models
{
    
    /// <summary>
    /// This class for permission administraton
    /// </summary>
    public class Permission : IEquatable<Permission>
    {
        /// <summary>
        /// The ID of permission used as primary key in database
        /// </summary>
        public int num { get; set; }

        /// <summary>
        /// The name of the permission
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "PermissionName")]
        public string name { set; get; }

        /// <summary>
        /// Permission Constructor
        /// </summary>
        public Permission() { }

        /// <summary>
        /// Permission Constructor
        /// </summary>
        /// <param name="i">the permission ID</param>
        /// <param name="n">the permission name</param>
        public Permission(int i,string n) {
            this.num=i;
            this.name=n;
        }

        /// <summary>
        /// Defining permissions equality
        /// </summary>
        /// <param name="other">object of class permission</param>
        /// <returns>boolean, true if they were equals, false if not</returns>
        public bool Equals(Permission other)
        {
            return this.num == other.num && this.name == other.name;
        }

        /// <summary>
        /// Adding permission to some group
        /// </summary>
        /// <param name="g">the group ID</param>
        /// <param name="p">the permission ID</param>
        /// <returns>exception message if there is any, empty string if not</returns>
        public static string assignGroupPerm(int g,int p)
        {
            string res = "";
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                string qr = "Begin" +
                            "  insert into GROUPPERMISSIONS" +
                            "( GNUM, PNUM)" +
                            "values " +
                            "(:GNUM,:PNUM);" +
                            "End;";
                OracleCommand cmd = new OracleCommand(qr, conn);
                OracleParameter[] param =  {
                                            new OracleParameter("GNUM", g),
                                            new OracleParameter("PNUM", p),
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

        /// <summary>
        /// Deletes the permissions of some group
        /// </summary>
        /// <param name="g">the group ID</param>
        /// <returns>exception message if there is any, empty string if not</returns>
        public static string deleteGroupPerms(int g)
        {
            string res = "";
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                string qr = "Begin" +
                            "  delete from GROUPPERMISSIONS where " +
                            " GNUM = " +
                            " :GNUM;" +
                            "End;";
                OracleCommand cmd = new OracleCommand(qr, conn);
                OracleParameter[] param =  {
                                            new OracleParameter("GNUM", g),
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

        /// <summary>
        /// Tests if this group has this permission or not
        /// </summary>
        /// <param name="g">the group ID</param>
        /// <param name="p">the permission ID</param>
        /// <returns>boolean, true if the group has this permission, false if not</returns>
        public static bool isGroupHasPerm(int g,int p)
        {
            bool t = false;
            List<Permission> gp = getPermissionsOfGroup(g);
            if (gp.Any(s => s.num == p))
                t = true;
            return t;
        }

        /// <summary>
        /// Gets pemissions list from database
        /// </summary>
        /// <returns>list of permissions defined in database</returns>
        public static List<Permission> getPermissionsList()
        {
            List<Permission> gList = new List<Permission>();

            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("SELECT * FROM PERMISSIONS ORDER BY NUM", conn);
                OracleDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Permission mt = new Permission();
                    #region Data
                    if (!dr.IsDBNull(0))
                        mt.num = int.Parse(dr.GetValue(0).ToString());
                    if (!dr.IsDBNull(1))
                        mt.name = dr.GetString(1);
                    #endregion
                    gList.Add(mt);
                }
            }
            catch
            {
            }
            finally
            {
                conn.Close();
            }
            return gList;
        }

        /// <summary>
        /// Gets the permissions given to some group
        /// </summary>
        /// <param name="id">the group ID</param>
        /// <returns>list of permissions assigned to this group</returns>
        public static List<Permission> getPermissionsOfGroup (int id)
        {
            List<Permission> pList = new List<Permission>();
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("SELECT PERMISSIONS.NUM, PERMISSIONS.NAME FROM GROUPS, GROUPPERMISSIONS, PERMISSIONS WHERE GROUPS.NUM = GROUPPERMISSIONS.GNUM AND GROUPPERMISSIONS.PNUM = PERMISSIONS.NUM AND GROUPS.NUM = '" + id+ "' ORDER BY PERMISSIONS.NUM", conn);
                OracleDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Permission mt = new Permission();
                    #region Data
                    if (!dr.IsDBNull(0))
                        mt.num = int.Parse(dr.GetValue(0).ToString());
                    if (!dr.IsDBNull(1))
                        mt.name = dr.GetString(1);
                    #endregion
                    pList.Add(mt);
                }
            }
            catch
            {
            }
            finally
            {
                conn.Close();
            }
            return pList;
        }

        /// <summary>
        /// Gets a defined permission from database
        /// </summary>
        /// <param name="pId">the permission ID</param>
        /// <returns>permission object that has this ID</returns>
        public static Permission getPermById(int pId)
        {
            Permission u = new Permission();
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());

            try
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand("SELECT * FROM PERMISSIONS WHERE NUM= " + pId + " ORDER BY NUM", conn);
                OracleDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    if (!dr.IsDBNull(0))
                        u.num = Int32.Parse(dr.GetValue(0).ToString());
                    if (!dr.IsDBNull(1))
                        u.name = dr.GetValue(1).ToString();
                }
            }
            catch
            {

            }
            finally
            {
                conn.Close();
            }
            return u;

        }

        /// <summary>
        /// Inserts a new permission into database
        /// </summary>
        /// <param name="p">permission object</param>
        /// <returns>exception message if there is any, empty string if not</returns>
        public static string addPerm(Permission p)
        {
            string res = "";
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            try
            {
                conn.Open();
                string qr = "Begin" +
                            "  insert into PERMISSIONS" +
                            "( NUM, NAME)" +
                            "values " +
                            "(:NUM,:NAME);" +
                            "End;";
                OracleCommand cmd = new OracleCommand(qr, conn);
                OracleParameter[] param =  {
                                            new OracleParameter("NUM", p.num),
                                            new OracleParameter("NAME", p.name),
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

        /// <summary>
        /// Every time the system starts. it deletes and regenerate the permission list into database
        /// </summary>
        /// <returns>exception message if there is any, empty string if not</returns>
        public static string initializePerms()
        {
            string res = "";
            OracleConnection conn = new OracleConnection(OracleRIS.GetConnectionString());
            //delete all perms
            try
            {
                conn.Open();
                string qr = "delete from PERMISSIONS where 1=1 ";
                OracleCommand cmd = new OracleCommand(qr, conn);
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
            //initialize perms

            addPerm(new Permission(Perms.UserIndex, "UserIndex"));
            addPerm(new Permission(Perms.UserCreate, "UserCreate"));
            addPerm(new Permission(Perms.UserEdit, "UserEdit"));
            addPerm(new Permission(Perms.UserDelete, "UserDelete"));
            addPerm(new Permission(Perms.UserDtails, "UserDtails"));

            addPerm(new Permission(Perms.PatientIndex, "PatientIndex"));
            addPerm(new Permission(Perms.PatientCreate, "PatientCreate"));
            addPerm(new Permission(Perms.PatientEdit, "PatientEdit"));
            addPerm(new Permission(Perms.PatientDelete, "PatientDelete"));
            addPerm(new Permission(Perms.PatientDetails, "PatientDetails"));

            addPerm(new Permission(Perms.RadiologyIndex, "RadiologyIndex"));
            addPerm(new Permission(Perms.RadiologyCreate, "RadiologyCreate"));
            addPerm(new Permission(Perms.RadiologyCreateSchedualed, "RadiologyCreateSchedualed"));
            addPerm(new Permission(Perms.RadiologyEdit, "RadiologyEdit"));
            addPerm(new Permission(Perms.RadiologyDelete, "RadiologyDelete"));
            addPerm(new Permission(Perms.RadiologyDetails, "RadiologyDetails"));
            addPerm(new Permission(Perms.RadiologyOrderStatus, "RadiologyOrderStatus"));

            addPerm(new Permission(Perms.DepartmentIndex, "DepartmentIndex"));
            addPerm(new Permission(Perms.DepartmentCreate, "DepartmentCreate"));
            addPerm(new Permission(Perms.DepartmentEdit, "DepartmentEdit"));
            addPerm(new Permission(Perms.DepartmentDelete, "DepartmentDelete"));

            addPerm(new Permission(Perms.ModalityIndex, "ModalityIndex"));
            addPerm(new Permission(Perms.ModalityCreate, "ModalityCreate"));
            addPerm(new Permission(Perms.ModalityEdit, "ModalityEdit"));
            addPerm(new Permission(Perms.ModalityDelete, "ModalityDelete"));

            addPerm(new Permission(Perms.ModalityTypeIndex, "ModalityTypeIndex"));
            addPerm(new Permission(Perms.ModalityTypeCreate, "ModalityTypeCreate"));
            addPerm(new Permission(Perms.ModalityTypeEdit, "ModalityTypeEdit"));
            addPerm(new Permission(Perms.ModalityTypeDelete, "ModalityTypeDelete"));

            addPerm(new Permission(Perms.GroupIndex, "GroupIndex"));
            addPerm(new Permission(Perms.GroupCreate, "GroupCreate"));
            addPerm(new Permission(Perms.GroupEdit, "GroupEdit"));
            addPerm(new Permission(Perms.GroupDelete, "GroupDelete"));
            addPerm(new Permission(Perms.GroupDetailsPerm, "GroupDetailsPerm"));

            addPerm(new Permission(Perms.ProcedureIndex, "ProcedureIndex"));
            addPerm(new Permission(Perms.ProcedureCreate, "ProcedureCreate"));
            addPerm(new Permission(Perms.ProcedureEdit, "ProcedureEdit"));
            addPerm(new Permission(Perms.ProcedureDelete, "ProcedureDelete"));

            addPerm(new Permission(Perms.StatsIndex, "StatsIndex"));
            addPerm(new Permission(Perms.StatsPatient, "StatsPatient"));
            addPerm(new Permission(Perms.StatsOrders, "StatsOrders"));

            addPerm(new Permission(Perms.ClinicAppoinmentIndex, "ClinicAppoinmentIndex"));
            addPerm(new Permission(Perms.ClinicAppoinmentCreate, "ClinicAppoinmentCreate"));
            addPerm(new Permission(Perms.ClinicAppoinmentEdit, "ClinicAppoinmentEdit"));
            addPerm(new Permission(Perms.ClinicAppoinmentDelete, "ClinicAppoinmentDelete"));
            addPerm(new Permission(Perms.ClinicAppoinmentDetails, "ClinicAppoinmentDetails"));

            addPerm(new Permission(Perms.BillsIndex, "BillsIndex"));
            addPerm(new Permission(Perms.BillsCreate, "BillsCreate"));
            addPerm(new Permission(Perms.BillsEdit, "BillsEdit"));
            addPerm(new Permission(Perms.BillsDelete, "BillsDelete"));
            addPerm(new Permission(Perms.BillsDetails, "BillsDetails"));

            addPerm(new Permission(Perms.StatsApps, "StatsApps"));
            addPerm(new Permission(Perms.AppStatsIndex, "AppStatsIndex"));
            addPerm(new Permission(Perms.AppStatsPatient, "AppStatsPatient"));
            addPerm(new Permission(Perms.ClinicAppoinmentAudit, "ClinicAppoinmentAudit"));

            addPerm(new Permission(Perms.PermsIndex, "PermsIndex"));

            //add admin group مدير النظام
            Group admiGrp = new Group();
            admiGrp = Group.SelectByName(ConfigVar.adminGroup);
            if (admiGrp == null)
            {
                admiGrp = new Group();
                admiGrp.num = OracleRIS.GetOracleSequenceValue("GROUP_SEQ");
                admiGrp.name = ConfigVar.adminGroup;
                string ex = Group.Insert(admiGrp);
            }

                List<Permission> pList = Permission.getPermissionsList();
                Permission.deleteGroupPerms(admiGrp.num);
                for (int i = 0; i < pList.Count; i++)
                {
                    Permission.assignGroupPerm(admiGrp.num, pList[i].num);
                }
            //end add admin group

            //add reception group الاستقبال
            Group ReciptionGrp = new Group();
            ReciptionGrp = Group.SelectByName(ConfigVar.recepGroup);
            if (ReciptionGrp == null)
            {
                ReciptionGrp = new Group();
                ReciptionGrp.num = OracleRIS.GetOracleSequenceValue("GROUP_SEQ");
                ReciptionGrp.name = ConfigVar.recepGroup;
                string exRecp = Group.Insert(ReciptionGrp);
            }
            
            List<int> recList = new List<int>();

            recList.Add(Perms.PatientIndex);
            recList.Add(Perms.PatientCreate);
            recList.Add(Perms.PatientEdit);
            recList.Add(Perms.PatientDetails);
           // recList.Add(Perms.RadiologyIndex);
            recList.Add(Perms.RadiologyCreate);
           // recList.Add(Perms.RadiologyEdit);


            Permission.deleteGroupPerms(ReciptionGrp.num);
            for (int i = 0; i < recList.Count; i++)
            {
                Permission.assignGroupPerm(ReciptionGrp.num, recList[i]);
            }
            //end add reception group


            //create HosAdmin and add to AdminGroup مدير النظام

            User admiUser = new User();
            admiUser = User.SelectByName(ConfigVar.adminUser);
            if (admiUser == null)
            {
                admiUser = new User();
                admiUser.num = OracleRIS.GetOracleSequenceValue("USER_SEQ");
                admiUser.username = ConfigVar.adminUser;
                admiUser.pass = ConfigVar.adminUser+"123";
                admiUser.role = 1;

                admiUser.firstName = ConfigVar.adminUser;
                admiUser.lastName = ConfigVar.adminUser;
                admiUser.language = "ar";
                admiUser.departement = "102";
                string ex = User.insertUser(admiUser);
            }
            Group.deleteUserGroups(admiUser.num);
            admiGrp = Group.SelectByName(ConfigVar.adminGroup);
            Group.assignUserGroup(admiUser.num, admiGrp.num);
            //end

            //

            return res;
        }
    }
}