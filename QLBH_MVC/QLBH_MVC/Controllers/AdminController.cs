using QLBH_MVC.Models;
using QLBH_MVC.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QLBH_MVC.Controllers
{
    public class AdminController : Controller
    {
        //
        // GET: /Admin/
        public ActionResult Index()
        {
            using(QLBHEntities ctx = new QLBHEntities()){

                if (CurrentContext.IsLogged() == false)
                {
                    return RedirectToAction("Index", "Home");
                }
                SessionUser userSess = CurrentContext.GetSessionUser();

                user ur = ctx.users.Where(c => c.Id == userSess.Id).FirstOrDefault();

                if (ur.Permission == 0)
                {
                    return RedirectToAction("Index", "Home");
                }

                ViewBag.TotalProduct = ctx.products.Count();
                ViewBag.TotalCategory = ctx.categories.Count();
                ViewBag.TotalManufacturer = ctx.manufacturers.Count();
                ViewBag.TotalOrdered = ctx.orders.Where(c => c.OrderStatus == 1).Count();
                ViewBag.TotalOrdering = ctx.orders.Where(c => c.OrderStatus == 0).Count();
            }
           
            return View();
        }

        public ActionResult Category()
        {
            using (QLBHEntities ctx = new QLBHEntities())
            {

                if (CurrentContext.IsLogged() == false)
                {
                    return RedirectToAction("Index", "Home");
                }
                SessionUser userSess = CurrentContext.GetSessionUser();

                user ur = ctx.users.Where(c => c.Id == userSess.Id).FirstOrDefault();

                if (ur.Permission == 0)
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            return View();
        }

        public ActionResult Manufacturer()
        {
            using (QLBHEntities ctx = new QLBHEntities())
            {

                if (CurrentContext.IsLogged() == false)
                {
                    return RedirectToAction("Index", "Home");
                }
                SessionUser userSess = CurrentContext.GetSessionUser();

                user ur = ctx.users.Where(c => c.Id == userSess.Id).FirstOrDefault();

                if (ur.Permission == 0)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return View();
        }

        public ActionResult Product()
        {
            using (QLBHEntities ctx = new QLBHEntities())
            {

                if (CurrentContext.IsLogged() == false)
                {
                    return RedirectToAction("Index", "Home");
                }
                SessionUser userSess = CurrentContext.GetSessionUser();

                user ur = ctx.users.Where(c => c.Id == userSess.Id).FirstOrDefault();

                if (ur.Permission == 0)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            using (QLBHEntities ctx = new QLBHEntities())
            {
                ViewBag.Categories = ctx.categories.ToList();
                ViewBag.Manufacturers = ctx.manufacturers.ToList();
            }
            return View();
        }

        public ActionResult Order()
        {
            using (QLBHEntities ctx = new QLBHEntities())
            {

                if (CurrentContext.IsLogged() == false)
                {
                    return RedirectToAction("Index", "Home");
                }
                SessionUser userSess = CurrentContext.GetSessionUser();

                user ur = ctx.users.Where(c => c.Id == userSess.Id).FirstOrDefault();

                if (ur.Permission == 0)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return View();
        }



        public ActionResult TableOrder(int page = 1)
        {
            using (QLBHEntities ctx = new QLBHEntities())
            {
                var ord = ctx.orders.Include("orderdetails").Include("user");


                int count = ord.Count();

                int pages = count / PageSize + (count % PageSize > 0 ? 1 : 0);

                if (page < 1 || page > pages)
                {
                    page = 1;
                }

                List<order> list = ord.OrderByDescending(c => c.OrderDate).Skip((page - 1) * PageSize).Take(PageSize).ToList();
                ViewBag.Count = count;
                ViewBag.PageCount = pages;
                ViewBag.CurPage = page;

                return PartialView(list);
            }
        }

        public ActionResult TableOrderDetail(int id)
        {
            using (QLBHEntities ctx = new QLBHEntities())
            {
                var ordd = ctx.orderdetails.Include("products").ToList();

                return PartialView(ordd);
            }
        }

        public ActionResult TableCategory()
        {
            using (QLBHEntities ctx = new QLBHEntities())
            {
                var cat = ctx.categories.ToList();

                return PartialView(cat);
            }
        }

        public ActionResult TableManufacturer()
        {
            using (QLBHEntities ctx = new QLBHEntities())
            {
                var manu = ctx.manufacturers.ToList();

                return PartialView(manu);
            }
        }

        int PageSize = 10;

        public ActionResult TableProduct(int page = 1)
       {

            using (QLBHEntities ctx = new QLBHEntities())
            {
                var query = ctx.products.Include("category").Include("manufacturer");
                int count = query.Count();

                int pages = count / PageSize + (count % PageSize > 0 ? 1 : 0);

                if (page < 1 || page > pages)
                {
                    page = 1;
                }

                List<product> list = query.OrderBy(c => c.ProId).Skip((page - 1) * PageSize).Take(PageSize).ToList();
                ViewBag.Count = count;
                ViewBag.PageCount = pages;
                ViewBag.CurPage = page;


                ViewBag.Categories = ctx.categories.ToList();
                ViewBag.Manufacturers = ctx.manufacturers.ToList();

                return PartialView(list);
            }
        }

        const string addCatSucc = "Thêm thành công";
        const string updCatSucc = "Cập nhập thành công";
        const string remvCatSucc = "Xóa thành công";
        const string exstCatProc = "Danh mục này còn chứa sản phẩm";

        const string addMaSucc = "Thêm thành công";
        const string updMaSucc = "Cập nhập thành công";
        const string remvMaSucc = "Xóa thành công";
        const string exstMaProc = "Nhà sản xuất này còn chứa sản phẩm";

        [HttpPost]
        public ActionResult UpdateOrder(int id)
        {
            using (QLBHEntities ctx = new QLBHEntities())
            {
                var ord = ctx.orders.Where(c => c.OrderId == id).FirstOrDefault();

                if (ord.OrderStatus == 0)
                {
                    ord.OrderStatus = 1;
                }

                ctx.SaveChanges();

                return Json(new { Success = "đã giao sản phẩm" });
            }
        }

        [HttpPost]
        public ActionResult RemoveOrder(int id)
        {
            using (QLBHEntities ctx = new QLBHEntities())
            {
                var ord = ctx.orders.Where(c => c.OrderId == id).FirstOrDefault();

                if (ord.OrderStatus == 1)
                {
                    var lstOrdDetails = ord.orderdetails.ToList();

                    foreach (var d in lstOrdDetails)
                    {
                        ctx.orderdetails.Remove(d);
                    }

                    ctx.orders.Remove(ord);
                }

                if (ord.OrderStatus == 0)
                {
                    var lstOrdDetails = ord.orderdetails.ToList();

                    foreach (orderdetail d in lstOrdDetails)
                    {
                        d.product.Quantity += d.Amount;
                        d.product.Sale -= d.Amount;

                        ctx.orderdetails.Remove(d);
                    }

                    ctx.orders.Remove(ord);
                }

                ctx.SaveChanges();
            }

            return Json(new { Success = "đã xóa đơn hàng" });
        }

        [HttpPost]
        public ActionResult UpdateCategory(AdminCategoryModel model)
        {
            using (QLBHEntities ctx = new QLBHEntities())
            {


                var cat = ctx.categories.Where(c => c.CatId == model.CatId).FirstOrDefault();

                cat.CatName = model.CatName;

                ctx.SaveChanges();
            }

            return Json(new { Success = updCatSucc });
        }

        [HttpPost]
        public ActionResult UpdateManufacturer(AdminManufacturerModel model)
        {
            using (QLBHEntities ctx = new QLBHEntities())
            {
                var cat = ctx.manufacturers.Where(c => c.MaId == model.MaId).FirstOrDefault();

                cat.MaName = model.MaName;

                ctx.SaveChanges();
            }

            return Json(new { Success = updMaSucc });
        }

        [HttpPost]
        public ActionResult AddCategory(string name)
        {
            using (QLBHEntities ctx = new QLBHEntities())
            {
                category cat = new category { CatName = name };

                ctx.categories.Add(cat);

                ctx.SaveChanges();
            }

            return Json(new { Success = addCatSucc });
        }

        [HttpPost]
        public ActionResult AddManufacturer(string name)
        {
            using (QLBHEntities ctx = new QLBHEntities())
            {
                manufacturer cat = new manufacturer { MaName = name };

                ctx.manufacturers.Add(cat);

                ctx.SaveChanges();
            }

            return Json(new { Success = addMaSucc });
        }

        [HttpPost]
        public ActionResult LoadProduct(int id)
        {
            using (QLBHEntities ctx = new QLBHEntities())
            {
                product proc = ctx.products.Where(c => c.ProId == id).FirstOrDefault();

                ViewBag.Categories = ctx.categories.ToList();
                ViewBag.Manufacturers = ctx.manufacturers.ToList();

                return Json(new
                {
                    proid = proc.ProId,
                    proname = proc.ProName,
                    newprice = proc.NewPrice,
                    catid = proc.CatId,
                    maid = proc.MaId,
                    quantity = proc.Quantity,
                    shortdes = proc.ShortDes,
                    longdes = proc.LongDes
                });
            }


        }

        [HttpPost]
        public ActionResult UpdateProduct(AdminProductModel model)
        {
            using (QLBHEntities ctx = new QLBHEntities())
            {
                product proc = ctx.products.Where(c => c.ProId == model.ProId).FirstOrDefault();

                proc.ProName = model.ProName == null?"":model.ProName;
                proc.NewPrice = model.NewPrice;
                proc.MaId = model.MaId;
                proc.CatId = model.CatId;
                proc.Quantity = model.Quantity;
                proc.ShortDes = model.ShortDes == null ? "" : model.ShortDes;
                proc.LongDes = model.LongDes == null ? "" : model.LongDes;

                ctx.SaveChanges();

                return Json(new { Success = "cập nhập sản phẩm", Id = proc.ProId }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult RemoveProduct(int id)
        {
            using (QLBHEntities ctx = new QLBHEntities())
            {
                product proc = ctx.products.Where(c => c.ProId == id).FirstOrDefault();

                if (proc.orderdetails.Count > 0)
                {
                    return Json(new { Error = "không thể xóa do có đơn hàng" });
                }

                var direc = Server.MapPath("~/images/product/" + proc.ProId);
                var path_image = Server.MapPath("~/images/product/" + proc.ProId + "/img.jpg");

                if (System.IO.File.Exists(path_image))
                {
                    System.IO.File.Delete(path_image);
                    System.IO.Directory.Delete(direc);
                }

                ctx.products.Remove(proc);
                ctx.SaveChanges();

                

                return Json(new { Success = "xóa thành công" });
            }
        }

        [HttpPost]
        public ActionResult AddProduct(AdminProductModel model)
        {
            int procId;

            using (QLBHEntities ctx = new QLBHEntities())
            {
                product proc = new product
                {
                    ProName = model.ProName == null ?"":model.ProName,
                    NewPrice = model.NewPrice,
                    OldPrice = 400000,
                    MaId = model.MaId,
                    CatId = model.CatId,
                    Quantity = model.Quantity,
                    Sale = 0,
                    ViewCount = 0,
                    DateAdd = DateTime.Now,
                    ShortDes = model.ShortDes == null ? "" : model.ShortDes,
                    LongDes = model.LongDes == null ? "" : model.LongDes
                };

                ctx.products.Add(proc);
                ctx.SaveChanges();
                procId = proc.ProId;
            }

            return Json(new { Success = "thêm sản phẩm", Id = procId }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult RemoveCategory(int id)
        {
            using (QLBHEntities ctx = new QLBHEntities())
            {
                var cat = ctx.categories.Where(c => c.CatId == id).FirstOrDefault();

                if (cat.products.Count > 0)
                {
                    return Json(new { Error = exstCatProc });
                }
                else
                {
                    ctx.categories.Remove(cat);

                    ctx.SaveChanges();
                }
            }

            return Json(new { Success = remvCatSucc });
        }

        [HttpPost]
        public ActionResult RemoveManufacturer(int id)
        {
            using (QLBHEntities ctx = new QLBHEntities())
            {
                var cat = ctx.manufacturers.Where(c => c.MaId == id).FirstOrDefault();

                if (cat.products.Count > 0)
                {
                    return Json(new { Error = exstMaProc });
                }
                else
                {
                    ctx.manufacturers.Remove(cat);

                    ctx.SaveChanges();
                }
            }

            return Json(new { Success = remvMaSucc });
        }
    }
}