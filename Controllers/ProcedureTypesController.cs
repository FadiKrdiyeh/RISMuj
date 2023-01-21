using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RIS.Models;

namespace RIS.Controllers
{
    public class ProcedureTypesController : Controller
    {
        // GET: ProcedureTypes
        public ActionResult Index(int num)
        {
            return PartialView("Index",ProcedureTypes.getAllByParent(num));
        }

    }
}
