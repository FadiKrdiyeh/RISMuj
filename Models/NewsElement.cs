using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace RIS.Models
{
	public class NewsElement
	{
		#region Attributes
		public int ID { get; set; }

		[Display(ResourceType = typeof(Resources.Res), Name = "NewsTitle")]
		[DisplayFormat(ConvertEmptyStringToNull = false)]
		public string Title { get; set; }

		[Required(ErrorMessage = "هذا الحقل يجب ألا يكون فارغاً")]
		[Display(ResourceType = typeof(Resources.Res), Name = "NewsText")]
		public string Text { get; set; }

		[Display(ResourceType = typeof(Resources.Res), Name = "NewsLink")]
		[DisplayFormat(ConvertEmptyStringToNull = false)]
		public string URL { get; set; }
        /// <summary>
        /// The department ID where order had been added.
        /// </summary>
        [Display(ResourceType = typeof(Resources.Res), Name = "depName")]
        [Required(ErrorMessageResourceType = typeof(Resources.Res), ErrorMessageResourceName = "depReq")]
        public string DepartementName { set; get; }
        public static object item { get; private set; }
		#endregion

		#region Functions
		public static List<XElement> getNews()
		{
			string xmlFilePath = HttpContext.Current.Server.MapPath("~/NewsFeed/News.xml");
			XDocument xmlDoc = XDocument.Load(xmlFilePath);
			var NewsList = xmlDoc.Descendants("NewsElement").ToList();
			return NewsList;
		}

		public static void addNode(NewsElement NE)
		{
			string xmlFilePath = HttpContext.Current.Server.MapPath("~/NewsFeed/News.xml");
			XDocument xmlDoc = XDocument.Load(xmlFilePath);
			var maxIDstring = xmlDoc.Descendants("NewsElement").Max(x => x.Attribute("ID").Value);
			int newID = maxIDstring == null ? 1 : int.Parse(maxIDstring) + 1;
			xmlDoc.Root.Add(new XElement(
				"NewsElement", new XAttribute("ID", newID), new XElement("Title", NE.Title),
				new XElement("Text", NE.Text), new XElement("URL", NE.URL), new XElement("Dept",NE.DepartementName)
				));
			xmlDoc.Save(xmlFilePath);
		}

		public static void editNode(NewsElement NE)
		{
			string xmlFilePath = HttpContext.Current.Server.MapPath("~/NewsFeed/News.xml");
			XDocument xmlDoc = XDocument.Load(xmlFilePath);
			var target = xmlDoc.Descendants("NewsElement").Where(x => x.Attribute("ID").Value == NE.ID.ToString()).Single();
			target.Element("Title").Value = NE.Title;
			target.Element("Text").Value = NE.Text;
			target.Element("URL").Value = NE.URL;
            target.Element("Dept").Value = NE.DepartementName;

            xmlDoc.Save(xmlFilePath);
		}

		public static void deleteNode(NewsElement NE)
		{
			string xmlFilePath = HttpContext.Current.Server.MapPath("~/NewsFeed/News.xml");
			XDocument xmlDoc = XDocument.Load(xmlFilePath);
			xmlDoc.Descendants("NewsElement").Where(x => x.Attribute("ID").Value == NE.ID.ToString()).Remove();
			xmlDoc.Save(xmlFilePath);
		}

		public static NewsElement getNodeById(int id)
		{
			string xmlFilePath = HttpContext.Current.Server.MapPath("~/NewsFeed/News.xml");
			XDocument xmlDoc = XDocument.Load(xmlFilePath);
			var element = xmlDoc.Descendants("NewsElement").FirstOrDefault(x => x.Attribute("ID").Value == id.ToString());
			if (element == null)
				return null;
			else
			{
				var temp = new NewsElement();
				temp.ID = int.Parse(element.Attribute("ID").Value);
				temp.Title = element.Element("Title").Value;
				temp.Text = element.Element("Text").Value;
				temp.URL = element.Element("URL").Value;
                temp.DepartementName = element.Element("Dept").Value;
                return temp;
			}
		}
		#endregion
	}
}