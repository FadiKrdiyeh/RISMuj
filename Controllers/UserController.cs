using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RIS.Models;
using RISDB;

namespace RIS.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            string uName = "";
            try
            {
                uName = Session["userName"].ToString();
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }

            int userId=RIS.Models.User.getUserByUname(uName).num;
            if (!RIS.Models.User.hasPerm(userId, Perms.UserIndex))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }
            //try
            //{

            //bool v1 = trialConfigs.checkperiod();
            //bool v2 = trialConfigs.getOrderNumbers();
            //if (!v1 || !v2)
            //{
            //    return RedirectToAction("Index", "Home");
            //}
            //}
            //catch
            //{
            //    return RedirectToAction("Index", "Home");

            //}
            //try
            //{
            //    string t = Session["userType"].ToString();
            //    if (t != "1")
            //    {
            //        return RedirectToAction("Index", "Home", new { });

            //    }

            //    string u = Session["userName"].ToString();
            //}

            //catch
            //{
            //    return RedirectToAction("Index", "Home", new { });
            //}
            ViewData["Title"] = RIS.Resources.Res.UsersList.ToString();
            List<User> users = Models.User.getAllUsers();
            return View(users);
        }



        public ActionResult Create()
        {
            string uName = "";
            try
            {
                uName = Session["userName"].ToString();
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }

            int userId = RIS.Models.User.getUserByUname(uName).num;
            if (!RIS.Models.User.hasPerm(userId, Perms.UserCreate))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }

            User u = new User();
            ViewData["departement"] = Departement.GetDepartementList(true, "");
            //ViewData["clinic"] = GeniricIndex.GetIndexListNames(true,"","CLINIC");
            List<Group> gList = Group.getAllGroups();
            ViewBag.gList = gList;
            return View(u);
        }

        [HttpPost]
        public ActionResult Create(User u, int[] uGroups)
        {
            bool v = RIS.Models.User.checkDuplicate(u.username);
            ViewData["departement"] = Departement.GetDepartementList(true, u.departement);
            List<Group> gList = Group.getAllGroups();
            ViewBag.gList = gList;
            if (!v)
            {
                ModelState.AddModelError("", RIS.Resources.Res.userDupl);
             //   ViewData["departement"] = Departement.GetDepartementList(true, "");
                
                return View(u);

            }
            if (uGroups == null)
            {
                ModelState.AddModelError("", "يجب اختيار مجموعة");
                return View(u);
            }
            u.num = OracleRIS.GetOracleSequenceValue("USER_SEQ");
            try
            {
                // TODO: Add insert logic here
                
                if (ModelState.IsValid)
                {
                    int admiGrp = Group.SelectByName(ConfigVar.adminGroup).num;
                    if (uGroups.Contains(admiGrp))
                        u.role = 1;
                    else
                        u.role = 0;
                    string ex= Models.User.insertUser(u);
                    Group.deleteUserGroups(u.num);
                    for (int i = 0; i < uGroups.Length; i++)
                    {
                        Group.assignUserGroup(u.num, uGroups[i]);
                    }
                    if (string.IsNullOrEmpty(ex))
                        return RedirectToAction("Index", new { });
                    else
                        ModelState.AddModelError("", ex);
                }
                return View(u);
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Edit(int i)
        {
            
            string uName = "";
            try
            {
                uName = Session["userName"].ToString();
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }

            int userId = RIS.Models.User.getUserByUname(uName).num;
            if (!RIS.Models.User.hasPerm(userId, Perms.UserEdit))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }

            ViewData["PageName"] = RIS.Resources.Res.EditUserData.ToString();

            Models.User u = Models.User.select(i);

            try
            {
                ViewData["departement"] = Departement.GetDepartementListNames(true, u.departement);

            }
            catch
            {
                ViewData["departement"] = Departement.GetDepartementListNames(true, "");

            }
            
            List<Group> gList = Group.getAllGroups();
            ViewBag.gList = gList;
            List<Group> uGroupPList = Group.getUserGroups(i);
            ViewBag.uGroupPList = uGroupPList;

            return View(u);

        }

        [HttpPost]
        public ActionResult Edit (User u,int[] uGroups)
        {
            //if (u.username == ConfigVar.adminUser)
            //{
            //    ViewData["departement"] = Departement.GetDepartementList(false, u.userDepartement.ToString());
            //    List<Group> gList = Group.getAllGroups();
            //    ViewBag.gList = gList;
            //    List<Group> uGroupPList = Group.getUserGroups(u.num);
            //    ViewBag.uGroupPList = uGroupPList;
            //    ModelState.AddModelError("",RIS.Resources.Res.ErrorYouCant);
            //    return View(u);
            //}


            ViewData["departement"] = Departement.GetDepartementListNames(true, u.departement);
            List<Group> gList = Group.getAllGroups();
            ViewBag.gList = gList;
            List<Group> uGroupPList = Group.getUserGroups(u.num);
            ViewBag.uGroupPList = uGroupPList;

            if (uGroups == null)
            {
                ModelState.AddModelError("", "يجب اختيار مجموعة");
                return View(u);
            }


            try
            {
                // TODO: Add delete logic here
                if (ModelState.IsValid)
                {
                    int admiGrp = Group.SelectByName(ConfigVar.adminGroup).num;
                    if (uGroups.Contains(admiGrp))
                        u.role = 1;
                    else
                        u.role = 0;
                    string ex = Models.User.updateUser(u);


                    if (u.username != ConfigVar.adminUser)
                    {
                        Group.deleteUserGroups(u.num);
                        for (int i = 0; i < uGroups.Length; i++)
                        {
                            Group.assignUserGroup(u.num, uGroups[i]);
                        }
                    }



                    if (string.IsNullOrEmpty(ex))
                        return RedirectToAction("Index", new { });
                    ModelState.AddModelError("", ex);
                    try
                    {
                        ViewData["departement"] = Departement.GetDepartementList(false, u.userDepartement.ToString());

                    }

                    catch
                    {
                        ViewData["departement"] = Departement.GetDepartementList(true, "");

                    }
                   
                }
                return View(u);
            }
            catch
            {
                return View(u);
            }
        }

        public ActionResult Details(int i)
        {
            string uName = "";
            try
            {
                uName = Session["userName"].ToString();
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }

            int userId = RIS.Models.User.getUserByUname(uName).num;
            if (!RIS.Models.User.hasPerm(userId, Perms.UserDtails))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }

            ViewData["PageName"] = RIS.Resources.Res.userDetails.ToString();

            Models.User u = Models.User.select(i);

            ViewData["departement"] = Departement.GetDepartementListofTheuser(true, u.departement, "");
                        
            List<Group> uGroupPList = Group.getUserGroups(i);
            ViewBag.uGroupPList = uGroupPList;

            return View(u);

        }

        public ActionResult Delete(int i)
        {
            string uName = "";
            try
            {
                uName = Session["userName"].ToString();
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }

            int userId = RIS.Models.User.getUserByUname(uName).num;
            if (!RIS.Models.User.hasPerm(userId, Perms.UserDelete))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }

            ViewData["PageName"] = RIS.Resources.Res.deleteuser.ToString();
            Models.User u = Models.User.select(i);
            return View(u);
        }

        [HttpPost]
        public ActionResult Delete(int i, FormCollection collection)
        {
            
            User u = Models.User.select(i);
            User uAdmin = RIS.Models.User.SelectByName(ConfigVar.adminUser);
            if (uAdmin.num == i)
            {
                //  ViewData["departement"] = Departement.GetDepartementList(false, u.userDepartement.ToString());

                ModelState.AddModelError("", RIS.Resources.Res.ErrorYouCant);
                return View(u);
            }
            try
            {
                // TODO: Add delete logic here
                string ex = Models.User.deleteUser(u);
                if (string.IsNullOrEmpty(ex))
                    return RedirectToAction("Index", new { });
                ModelState.AddModelError("", ex);
                return View(u);
            }
            catch
            {
                return View(u);
            }
        }


        public ActionResult EditMine()
        {

            string uName = "";
            try
            {
                uName = Session["userName"].ToString();
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }

            int userId = RIS.Models.User.getUserByUname(uName).num;
            

            ViewData["PageName"] = RIS.Resources.Res.EditUserData.ToString();

            Models.User u = Models.User.select(userId);

            try
            {
                ViewData["departement"] = Departement.GetDepartementListNames(true, u.departement);

            }
            catch
            {
                ViewData["departement"] = Departement.GetDepartementListNames(true, "");

            }

            List<Group> gList = Group.getAllGroups();
            ViewBag.gList = gList;
            List<Group> uGroupPList = Group.getUserGroups(userId);
            ViewBag.uGroupPList = uGroupPList;

            return View(u);

        }

        [HttpPost]
        public ActionResult EditMine(User u)
        {
            
            ViewData["departement"] = Departement.GetDepartementListNames(true, u.departement);
            List<Group> gList = Group.getAllGroups();
            ViewBag.gList = gList;
            List<Group> uGroupPList = Group.getUserGroups(u.num);
            ViewBag.uGroupPList = uGroupPList;
            
            try
            {
                
             //   if (ModelState.IsValid)
                {
                   
                    string ex = Models.User.updateUser(u);



                    if (string.IsNullOrEmpty(ex))
                    {
                        @TempData["message"] = RIS.Resources.Res.doneSuccessfully;
                        return View(u);
                    }
                    ModelState.AddModelError("", ex);
                    try
                    {
                        ViewData["departement"] = Departement.GetDepartementList(false, u.userDepartement.ToString());

                    }

                    catch
                    {
                        ViewData["departement"] = Departement.GetDepartementList(true, "");

                    }

                }
                
                return View(u);
            }
            catch(Exception ee)
            {
                
                return View(u);
            }
        }


    }
}