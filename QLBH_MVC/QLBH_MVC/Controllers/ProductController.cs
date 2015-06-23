using QLBH_MVC.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QLBH_MVC.Controllers
{
    public class ProductController : Controller
    {
        //
        // GET: /Product/
        public ActionResult PartialViewTopSale()
        {
            using (QLBHEntities ctx = new QLBHEntities())
            {
                List<product> list = ctx.products.OrderByDescending(c => c.Sale).Take(10).ToList();

                return PartialView(list);
            }
        }

        public ActionResult PartialViewTopNew()
        {
            using (QLBHEntities ctx = new QLBHEntities())
            {
                List<product> list = ctx.products.OrderByDescending(c => c.DateAdd).Take(10).ToList();

                return PartialView(list);
            }
        }

        public ActionResult PartialViewTopView()
        {
            using (QLBHEntities ctx = new QLBHEntities())
            {
                List<product> list = ctx.products.OrderByDescending(c => c.ViewCount).Take(10).ToList();

                return PartialView(list);
            }
        }

        public ActionResult PartialViewSameCat(int? id)
        {
            using (QLBHEntities ctx = new QLBHEntities())
            {
                List<product> list = ctx.products.Include("category").Where(c => c.CatId == id).Take(5).ToList();

                return PartialView(list);
            }
        }

        public ActionResult PartialViewSameMa(int? id)
        {
            using (QLBHEntities ctx = new QLBHEntities())
            {
                List<product> list = ctx.products.Include("manufacturer").Where(c => c.MaId == id).Take(5).ToList();

                return PartialView(list);
            }
        }


        int PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);

        public ActionResult ByCat(int? id, int page = 1)
        {
            if (id.HasValue == false)
            {
                return RedirectToAction("Index", "Home");
            }


            using (QLBHEntities ctx = new QLBHEntities())
            {


                var query = ctx.products.Include("manufacturer").Include("category").Where(c => c.CatId == id);
                int count = query.Count();
                int pages = count / PageSize + (count % PageSize > 0 ? 1 : 0);

                if (page < 1 || page > pages)
                {
                    page = 1;
                }

                List<product> list = query.OrderBy(c => c.ProId).Skip((page - 1) * PageSize).Take(PageSize).ToList();
                category cat = ctx.categories.Where(c => c.CatId == id).FirstOrDefault();

                ViewBag.Count = count;
                ViewBag.PageCount = pages;
                ViewBag.CurPage = page;
                ViewBag.CatName = cat.CatName;
                ViewBag.CatId = cat.CatId;
                return View(list);
            }
        }

        public ActionResult ByMa(int? id, int page = 1)
        {
            if (id.HasValue == false)
            {
                return RedirectToAction("Index", "Home");
            }

            using (QLBHEntities ctx = new QLBHEntities())
            {

                var query = ctx.products.Include("manufacturer").Include("category").Where(c => c.MaId == id);
                int count = query.Count();
                int pages = count / PageSize + (count % PageSize > 0 ? 1 : 0);

                if (page < 1 || page > pages)
                {
                    page = 1;
                }

                List<product> list = query.OrderBy(c => c.ProId).Skip((page - 1) * PageSize).Take(PageSize).ToList();
                manufacturer manu = ctx.manufacturers.Where(c => c.MaId == id).FirstOrDefault();

                ViewBag.Count = count;
                ViewBag.PageCount = pages;
                ViewBag.CurPage = page;
                ViewBag.MaName = manu.MaName;
                ViewBag.MaId = manu.MaId;
                return View(list);
            }
        }

        public ActionResult Detail(int? id)
        {
            if (id.HasValue == false)
            {
                return RedirectToAction("Index", "Home");
            }

            using (QLBHEntities ctx = new QLBHEntities())
            {
                product proc = ctx.products.Include("manufacturer").Include("category").Where(c => c.ProId == id).FirstOrDefault();
                proc.ViewCount++;
                ctx.SaveChanges();

                return View(proc);
            }
        }

        public ActionResult Search()
        {
            return View();
        }

        public ActionResult EnhanceSearch()
        {
            using (QLBHEntities ctx = new QLBHEntities())
            {
                ViewBag.Categories = ctx.categories.ToList();
                ViewBag.Manufacturers = ctx.manufacturers.ToList();
            }

            return View();
        }

        [HttpGet]
        public ActionResult Search(int? selection, string id, int page = 1)
        {
            string content;
            int digit;
            bool isDigit;

            if (id == null || selection.HasValue == false)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                content = id.ToLower();
            }



            if (!int.TryParse(content, out digit))
            {
                isDigit = false;
            }
            else
            {
                isDigit = true;
            }

            List<product> list = new List<product>();

            using (QLBHEntities ctx = new QLBHEntities())
            {

                IQueryable<product> unionlist = ctx.products.Include("category").Include("manufacturer").Where(_ => false);
                var proclist = ctx.products.Include("category").Include("manufacturer");

                switch (selection)
                {
                    //Find all
                    case 0:
                        {
                            if (isDigit == true)
                            {
                                var inProduct = from p in proclist
                                                where p.ProName.Contains(content) ||
                                                p.Quantity.Equals(digit) ||
                                                p.ShortDes.ToLower().Contains(content) ||
                                                p.LongDes.ToLower().Contains(content) ||
                                                p.NewPrice.Equals(digit)
                                                select p;

                                unionlist = unionlist.Union(inProduct);
                                list = unionlist.ToList();

                            }
                            else
                            {
                                var inProduct = from p in ctx.products
                                                where p.ProName.ToLower().Contains(content) ||
                                                p.ShortDes.ToLower().Contains(content) ||
                                                p.LongDes.ToLower().Contains(content)
                                                select p;

                                unionlist = unionlist.Union(inProduct);
                            }

                            var inCategory = from p in proclist
                                             join c in ctx.categories
                                             on p.CatId equals c.CatId
                                             where c.CatName.ToLower().Contains(content)
                                             select p;

                            var inManufacturer = from p in proclist
                                                 join m in ctx.manufacturers
                                                 on p.MaId equals m.MaId
                                                 where m.MaName.ToLower().Contains(content)
                                                 select p;

                            unionlist = unionlist.Union(inCategory);
                            unionlist = unionlist.Union(inManufacturer);
                        }
                        break;
                    //Find in categories
                    case 1:
                        {
                            var inCategory = from p in proclist
                                             join c in ctx.categories
                                             on p.CatId equals c.CatId
                                             where c.CatName.ToLower().Contains(content)
                                             select p;

                            unionlist = unionlist.Union(inCategory);
                        }
                        break;
                    //Find in manufacturers
                    case 2:
                        {
                            var inManufacturer = from p in proclist
                                                 join m in ctx.manufacturers
                                                 on p.MaId equals m.MaId
                                                 where m.MaName.ToLower().Contains(content)
                                                 select p;

                            unionlist = unionlist.Union(inManufacturer);
                        }
                        break;
                    //Find in product's name
                    case 3:
                        {
                            var inProductName = from p in proclist
                                                where p.ProName.ToLower().Contains(content)
                                                select p;

                            unionlist = unionlist.Union(inProductName);
                        }
                        break;
                    //Find in product's description
                    case 4:
                        {
                            var inDescription = from p in proclist
                                                where p.ShortDes.ToLower().Contains(content) ||
                                                p.LongDes.ToLower().Contains(content)
                                                select p;

                            unionlist = unionlist.Union(inDescription);
                        }
                        break;
                    //Find on greater or equal product's price
                    case 5:
                        {
                            if (isDigit == true)
                            {
                                var inPrice = from p in proclist
                                              where p.NewPrice >= digit
                                              select p;

                                unionlist = unionlist.Union(inPrice);
                            }
                        }
                        break;
                    //Find on less than or equal product's price
                    case 6:
                        {
                            if (isDigit == true)
                            {
                                var inPrice = from p in proclist
                                              where p.NewPrice <= digit
                                              select p;

                                unionlist = unionlist.Union(inPrice);
                            }
                        }
                        break;
                    //Find on greater or erual product's quantity
                    case 7:
                        {
                            if (isDigit == true)
                            {
                                var inQuantity = from p in proclist
                                                 where p.Quantity >= digit
                                                 select p;

                                unionlist = unionlist.Union(inQuantity);
                            }
                        }
                        break;
                    //Find on less than or equal product's quantity
                    case 8:
                        {
                            if (isDigit == true)
                            {
                                var inQuantity = from p in proclist
                                                 where p.Quantity <= digit
                                                 select p;

                                unionlist = unionlist.Union(inQuantity);
                            }
                        }
                        break;
                }





                int count = unionlist.Count();
                int pages = count / PageSize + (count % PageSize > 0 ? 1 : 0);

                if (page < 1 || page > pages)
                {
                    page = 1;
                }

                list = unionlist.OrderBy(c => c.ProId).Skip((page - 1) * PageSize).Take(PageSize).ToList();

                ViewBag.Opt = selection;
                ViewBag.Count = count;
                ViewBag.PageCount = pages;
                ViewBag.CurPage = page;
                ViewBag.SearchContent = content;
            }

            return View(list);
        }

        [HttpGet]
        public ActionResult EnhanceSearch(SearchingModel search, int page = 1)
        {
            string content = null;

            if (search.Product != null)
            {
                content = search.Product.ToLower();
            }


            List<product> list = new List<product>();

            using (QLBHEntities ctx = new QLBHEntities())
            {
                ViewBag.Categories = ctx.categories.ToList();
                ViewBag.Manufacturers = ctx.manufacturers.ToList();

                var proclist = ctx.products.Include("category").Include("manufacturer").Where(c => c.MaId == search.MaId && c.CatId == search.CatId);

                if (content != null)
                {
                    proclist = proclist.Where(c => c.ProName.ToLower().Contains(content)
                         || c.ShortDes.ToLower().Contains(content)
                         || c.LongDes.ToLower().Contains(content));
                }

                if (search.OptinalPrice == 1)
                {
                    proclist = proclist.Where(c => c.NewPrice <= search.Price);
                }
                else
                {
                    proclist = proclist.Where(c => c.NewPrice >= search.Price);
                }





                int count = proclist.Count();
                int pages = count / PageSize + (count % PageSize > 0 ? 1 : 0);

                if (page < 1 || page > pages)
                {
                    page = 1;
                }

                list = proclist.OrderBy(c => c.ProId).Skip((page - 1) * PageSize).Take(PageSize).ToList();

                ViewBag.Count = count;
                ViewBag.PageCount = pages;
                ViewBag.CurPage = page;
                ViewBag.Search = search;
            }

            return View(list);
        }


    }
}