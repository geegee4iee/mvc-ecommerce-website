using QLBH_MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QLBH_MVC.Controllers
{
    public class CategoryController : Controller
    {
        //
        // GET: /Category/
        public ActionResult PartialList()
        {
            using (QLBHEntities ctx = new QLBHEntities())
            {
                List<category> list = ctx.categories.Include("products").ToList();
                return PartialView(list);
            }
        }
	}
}