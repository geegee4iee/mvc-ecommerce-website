using QLBH_MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QLBH_MVC.Controllers
{
    public class ManufacturerController : Controller
    {
        //
        // GET: /Manufacturer/
        public ActionResult PartialList()
        {
            using (QLBHEntities ctx = new QLBHEntities())
            {
                List<manufacturer> list = ctx.manufacturers.Include("products").ToList();
                return PartialView(list);
            }
        }
	}
}