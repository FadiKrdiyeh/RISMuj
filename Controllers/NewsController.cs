using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RIS.Models;
using System.Net;

namespace RIS.Controllers
{
	public class NewsController : Controller
	{
		// GET: News
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

			// test if the user has the permission to access statistics
			int userId = RIS.Models.User.getUserByUname(uName).num;
			if (!RIS.Models.User.hasPerm(userId, Perms.NewsIndex))
			{
				TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
				return RedirectToAction("Index", "Home");
			}
			ViewData["PageName"] = RIS.Resources.Res.BreakingNewsList;
			var XNewsList = NewsElement.getNews();
			var newsList = new List<NewsElement>();
			foreach (var item in XNewsList)
			{
				var temp = new NewsElement();
				temp.ID = int.Parse(item.Attribute("ID").Value);
				temp.Title = item.Element("Title").Value;
				temp.Text = item.Element("Text").Value;
				temp.URL = item.Element("URL").Value;
                //temp.DepartementName = item.Element("Dept").Value;
                temp.DepartementName = Departement.select(int.Parse(item.Element("Dept").Value)).name;

                newsList.Add(temp);
			}
			return View(newsList);
		}

		// GET: News/Details/5
		public ActionResult Details(int id)
		{
			return View();
		}

		// GET: News/Create
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

			// test if the user has the permission to access statistics
			int userId = RIS.Models.User.getUserByUname(uName).num;
			if (!RIS.Models.User.hasPerm(userId, Perms.NewsIndex))
			{
				TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
				return RedirectToAction("Index", "Home");
			}
            ViewData["DepartementName"] = Departement.GetDepartementListNames(true, "");

			return View();
		}

		// POST: News/Create
		[HttpPost]
		public ActionResult Create(NewsElement NE)
		{
			try
			{
				if (ModelState.IsValid)
				{
					NewsElement.addNode(NE);
					return RedirectToAction("Index");
				}
				else
				{
					ModelState.AddModelError("Error", "المعطيات المدخلة غير مناسبة");
					return View(NE);
				}
			}
			catch (Exception e)
			{
				ModelState.AddModelError("Error", e.Message);
				return View();
			}
		}

		// GET: News/Edit/5
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

			// test if the user has the permission to access statistics
			int userId = RIS.Models.User.getUserByUname(uName).num;
			if (!RIS.Models.User.hasPerm(userId, Perms.NewsIndex))
			{
				TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
				return RedirectToAction("Index", "Home");
			}

            var NE = NewsElement.getNodeById(id);
            ViewData["DepartementName"] = Departement.GetDepartementListNames(true, Departement.select(int.Parse(NE.DepartementName)).name);

            if (NE == null)
				return new HttpStatusCodeResult(HttpStatusCode.NotFound, "لم يتم العثور على الخبر المطلوب");
			return View(NE);
		}

		// POST: News/Edit/5
		[HttpPost]
		public ActionResult Edit(NewsElement NE)
		{
			try
			{
				if (ModelState.IsValid)
				{
					NewsElement.editNode(NE);
					return RedirectToAction("Index");
				}
				else
				{
					ModelState.AddModelError("Error", "المعطيات المدخلة غير مناسبة");
					return View(NE);
				}
			}
			catch
			{
				return View();
			}
		}

		// GET: News/Delete/5
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

			// test if the user has the permission to access statistics
			int userId = RIS.Models.User.getUserByUname(uName).num;
			if (!RIS.Models.User.hasPerm(userId, Perms.NewsIndex))
			{
				TempData["message"] = RIS.Resources.Res.UnsPermErrorMessage;
				return RedirectToAction("Index", "Home");
			}
			var NE = NewsElement.getNodeById(id);
			if (NE == null)
				return new HttpStatusCodeResult(HttpStatusCode.NotFound, "لم يتم العثور على الخبر المطلوب");
			return View(NE);
		}

		// POST: News/Delete/5
		[HttpPost]
		public ActionResult Delete(NewsElement NE)
		{
			try
			{
				NewsElement.deleteNode(NE);
				return RedirectToAction("Index");
			}
			catch
			{
				return View();
			}
		}

		public ActionResult getNews()
		{
            var newsList = new List<NewsElement>();
            if (Session["userName"] != null)
            {
                string DeptId = (Session["userName"].ToString() == "HosAdmin") ? "0" : Models.User.getDepID(Session["userName"].ToString());
                var XNewsList = NewsElement.getNews();
                foreach (var item in XNewsList)
                {
                    var temp = new NewsElement();
                    if (item.Element("Dept").Value == DeptId || DeptId == "0" || item.Element("Dept").Value=="0")
                    {
                        temp.ID = int.Parse(item.Attribute("ID").Value);
                        temp.Title = item.Element("Title").Value;
                        temp.Text = item.Element("Text").Value;
                        temp.URL = item.Element("URL").Value;
                        temp.DepartementName = item.Element("Dept").Value;
                        newsList.Add(temp);
                    }
                }
            }
            else
            {
                var temp = new NewsElement();
                temp.ID = int.Parse("0");
                temp.Title = "t";
                temp.Text = "t";
                temp.URL = "u";
                temp.DepartementName = "0";
                newsList.Add(temp);
            }
            return Json(newsList, JsonRequestBehavior.AllowGet);
        }
    }
}
