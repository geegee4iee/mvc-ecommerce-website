using QLBH_MVC.Models;
using QLBH_MVC.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QLBH_MVC.Controllers
{
    public class CartController : Controller
    {
        //
        // GET: /Cart/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ViewCart()
        {
            if (CurrentContext.IsLogged() == false)
            {
                ViewBag.Required = "Bạn phải đăng nhập trước đã";
                return RedirectToAction("Login", "Account", new { require="cart" });
            }
            else return View();
        }

        public ActionResult PartialListTable()
        {
            var cart = CurrentContext.GetSessionCart();

            ViewBag.TotalPrice = cart.TotalPrice;
            ViewBag.Total = cart.Total;

            return PartialView(cart.cart.OrderByDescending(c=>c.Amount).ToList());
        }

        public ActionResult PartialList()
        {
            var cart = CurrentContext.GetSessionCart();

            return PartialView(cart.cart.OrderByDescending(c=>c.Amount).ToList());
        }

        [HttpPost]
        public ActionResult AddProduct(int id, int? amount = 1)
        {
            var cart = CurrentContext.GetSessionCart();
            bool status = false;

            using (QLBHEntities ctx = new QLBHEntities())
            {
                var dbproc = ctx.products.Where(c => c.ProId == id).FirstOrDefault();

                if (dbproc != null)
                {
                    SessionCartProduct proc = new SessionCartProduct
                    {
                        Id = id,
                        Amount = (int)amount,
                        Price = dbproc.NewPrice,
                        Quantity = dbproc.Quantity,
                        Name = dbproc.ProName
                    };

                    status = cart.AddProduct(proc);
                }
            }

            return Json(new { Cart = cart, Status = status });
        }

        [HttpPost]
        public ActionResult UpdateProduct(int id,int amount)
        {
            var cart = CurrentContext.GetSessionCart();
            bool status = false;

            using (QLBHEntities ctx = new QLBHEntities())
            {
                var dbproc = ctx.products.Where(c => c.ProId == id).FirstOrDefault();

                if (dbproc != null)
                {
                    SessionCartProduct proc = new SessionCartProduct
                    {
                        Id = id,
                        Amount = (int)amount,
                        Price = dbproc.NewPrice,
                        Quantity = dbproc.Quantity,
                        Name = dbproc.ProName
                    };

                   status = cart.UpdateProduct(proc);
                }
            }

            return Json(new {Cart = cart, Status = status});
        }

        [HttpPost]
        public ActionResult RemoveProduct(int id)
        {
            var cart = CurrentContext.GetSessionCart();

            cart.RemoveProduct(id);

            return Json(new { Cart = cart, Status = true });
        }

        [HttpPost]
        public ActionResult EmptyProduct()
        {
            var cart = CurrentContext.GetSessionCart();

            if (cart.Total == 0)
            {
                return Json(new { Status = false });
            }

            CurrentContext.EmptyCart();

            return Json(new { Cart = cart, Status = true });
        }

        [HttpPost]
        public ActionResult AddOrder()
        {
            var cart = CurrentContext.GetSessionCart();
            var sessUser = CurrentContext.GetSessionUser();

            using (QLBHEntities ctx = new QLBHEntities())
            {
                order ord = new order
                {
                    UserId = sessUser.Id,
                    OrderDate = DateTime.Now,
                    OrderStatus = 0,
                    Total = (long)cart.TotalPrice
                };

                foreach (SessionCartProduct detail in cart.cart)
                {
                    ord.orderdetails.Add(new orderdetail
                    {
                        ProId = detail.Id,
                        Amount = detail.Amount,
                        OrderId = ord.OrderId
                    });

                    var proc = ctx.products.Where(c => c.ProId == detail.Id).FirstOrDefault();
                    proc.Quantity -= detail.Amount;
                    proc.Sale += detail.Amount;

                    ctx.SaveChanges();
                }

                ctx.orders.Add(ord);
                ctx.SaveChanges();
            }

            CurrentContext.EmptyCart();

            return Json(new { Status = true });  
        }
    }
}