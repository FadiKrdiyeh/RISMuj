using RIS.Models;
using RISDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RIS.Controllers
{
    /// <summary>
    /// This controller deals with all actions in the group's page
    /// </summary>
    public class GroupController : Controller
    {
        // GET: GROUP
        /// <summary>
        /// This action is called when the group index page is accessed
        /// </summary>
        /// <permission cref="Perms.GroupIndex">the user has to have the GroupIndex permission to access this action</permission>
        /// <returns>The group index view</returns>
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

            int userId = RIS.Models.User.getUserByUname(uName).num;
            if (!RIS.Models.User.hasPerm(userId, Perms.GroupIndex))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }

            ViewData["PageName"] = "المجموعات";
            List<Group> mtList = Group.getData();
            return View(mtList.ToList());
        }

        // GET: ModalityType/Details/5
        /// <summary>
        /// This action is called when the group details page is accessed
        /// </summary>
        /// <permission cref="Perms.GroupDetailsPerm">the user has to have the GroupDetailsPerm permission to access this action</permission>
        /// <param name="id">the group's ID</param>
        /// <returns>the group details view</returns>
        public ActionResult Details(int id)
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
            if (!RIS.Models.User.hasPerm(userId, Perms.GroupDetailsPerm))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }

            Group g = Group.Select(id);
            ViewBag.groupID = id;
            ViewBag.groupName = g.name;
            List<Permission> pList = Permission.getPermissionsOfGroup(id);
            ViewBag.pList = pList;
            return View(g);
        }

        // GET: ModalityType/Create
        /// <summary>
        /// This action is called when the user wants to create a new group
        /// </summary>
        /// <permission cref="Perms.GroupCreate">the user has to have the GroupCreate permission to access this action</permission>
        /// <returns>the create group view</returns>
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
            if (!RIS.Models.User.hasPerm(userId, Perms.GroupCreate))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }

            ViewData["PageName"] = "إضافة مجموعة";
            Group mt = new Group();
            mt.num = OracleRIS.GetOracleSequenceValue("GROUP_SEQ");
            List<Permission> pList = Permission.getPermissionsList();
            ViewBag.pList = pList;
            return View(mt);
        }

        // POST: ModalityType/Create
        /// <summary>
        /// This action creates a new group and inserts its information into database
        /// </summary>
        /// <param name="mt">group object contains the group information</param>
        /// <param name="gPerms">integers representing the permissions assigned to this group</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(Group mt, int[] gPerms)
        {
            try
            {
                // TODO: Add insert logic here
               
                List<Permission> pList = Permission.getPermissionsList();
                ViewBag.pList = pList;
                if (gPerms == null)
                {
                    ModelState.AddModelError("", "يجب اختيار صلاحيات للمجموعة");
                    return View(mt);
                }
                if (ModelState.IsValid)
                {
                    string ex = Group.Insert(mt);

                    Permission.deleteGroupPerms(mt.num);
                    for (int i = 0; i < gPerms.Length; i++)
                    {
                        // if (!PERMISSION.isGroupHasPerm(mt.num, gPerms[i]))//check if permission is not assigned to group
                        Permission.assignGroupPerm(mt.num, gPerms[i]);
                    }
                    if (string.IsNullOrEmpty(ex))
                        return RedirectToAction("Index", new { });
                    ModelState.AddModelError("", ex);

                    if (string.IsNullOrEmpty(ex))
                        return RedirectToAction("Index", new { });
                    else
                        ModelState.AddModelError("", ex);
                }
                return View(mt);
            }
            catch
            {
                return View();
            }
        }

        // GET: ModalityType/Edit/5
        /// <summary>
        /// This action is called when the user wants to edit a group information
        /// </summary>
        /// <permission cref="Perms.GroupEdit">the user has to have the GroupEdit permission to access this action</permission>
        /// <param name="id">the group ID</param>
        /// <returns>the edit group view</returns>
        public ActionResult Edit(int id)
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
            if (!RIS.Models.User.hasPerm(userId, Perms.GroupEdit))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }

            Group admiGrp = Group.SelectByName(ConfigVar.adminGroup);
            Group ReciptionGrp = Group.SelectByName(ConfigVar.recepGroup);

            if (admiGrp.num == id || ReciptionGrp.num == id)
            {
                ViewData["PageName"] = "المجموعات";
                List<Group> mtList = Group.getData();
                TempData["message"] = RIS.Resources.Res.ErrorYouCant;
                return RedirectToAction("Index");
            }

            ViewData["PageName"] = "تعديل مجموعة";
            Group mt = Group.Select(id);
            List<Permission> pList = Permission.getPermissionsList();
            ViewBag.pList = pList;
            List<Permission> groupPList = Permission.getPermissionsOfGroup(id);
            ViewBag.groupPList = groupPList;

            return View(mt);
        }

        // POST: Group/Edit/5
        /// <summary>
        /// This action updates a group information in database
        /// </summary>
        /// <param name="mt">group object contains the new group infromation</param>
        /// <param name="gPerms">integers representing the new permissions assigned to this group</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(Group mt,int[] gPerms)
        {
            try
            {

                List<Permission> pList = Permission.getPermissionsList();
                ViewBag.pList = pList;
                List<Permission> groupPList = Permission.getPermissionsOfGroup(mt.num);
                ViewBag.groupPList = groupPList;
                if (gPerms == null)
                {
                    ModelState.AddModelError("", "يجب اختيار صلاحيات للمجموعة");
                    return View(mt);
                }

                // TODO: Add delete logic here
                string ex = Group.Edit(mt);

                Permission.deleteGroupPerms(mt.num);
                for (int i = 0; i < gPerms.Length; i++)
                {
                   // if (!PERMISSION.isGroupHasPerm(mt.num, gPerms[i]))//check if permission is not assigned to group
                        Permission.assignGroupPerm(mt.num, gPerms[i]);
                }
                if (string.IsNullOrEmpty(ex))
                    return RedirectToAction("Index", new { });
                ModelState.AddModelError("", ex);
                return View(mt);
            }
            catch
            {
                return View(mt);
            }
        }

        // GET: ModalityType/Delete/5
        /// <summary>
        /// This action is called when the user wants to delete a group
        /// </summary>
        /// <permission cref="Perms.GroupDelete">the user has to have the GroupDelete permission to access this action</permission>
        /// <param name="id">the group ID</param>
        /// <returns>the group delete view</returns>
        public ActionResult Delete(int id)
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
            if (!RIS.Models.User.hasPerm(userId, Perms.GroupDelete))
            {
                TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
                return RedirectToAction("Index", "Home");
            }
            Group admiGrp = Group.SelectByName(ConfigVar.adminGroup);
            Group ReciptionGrp = Group.SelectByName(ConfigVar.recepGroup);

            if (admiGrp.num == id || ReciptionGrp.num == id)
            {
                ViewData["PageName"] = "المجموعات";
                List<Group> mtList = Group.getData();
                TempData["message"] = RIS.Resources.Res.ErrorYouCant;
                return RedirectToAction("Index");
            }

            ViewData["PageName"] = "حذف مجموعة";
            Group mt = Group.Select(id);
            return View(mt);
        }

        // POST: ModalityType/Delete/5
        /// <summary>
        /// This action deletes a group from database
        /// </summary>
        /// <param name="id">the group ID</param>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            Group mt = Group.Select(id);
            try
            {
                // TODO: Add delete logic here
                string ex = Group.Delete(id);
                if (string.IsNullOrEmpty(ex))
                    return RedirectToAction("Index", new { });
                ModelState.AddModelError("", ex);
                return View(mt);
            }
            catch
            {
                return View(mt);
            }
        }
    }
}