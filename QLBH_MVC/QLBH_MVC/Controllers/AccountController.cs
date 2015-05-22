using BotDetect.Web;
using QLBH_MVC.Models;
using QLBH_MVC.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace QLBH_MVC.Controllers
{
    public class AccountController : Controller
    {
        //
        // GET: /Account/Login
        public ActionResult Login()
        {
            using (QLBHEntities ctx = new QLBHEntities())
            {
                ViewBag.Users = ctx.users.Count();
                ViewBag.NewRegister = ctx.users.OrderByDescending(c => c.Id).FirstOrDefault().Name;
            }

            return View();
        }

        public ActionResult Logout()
        {
            CurrentContext.Logout();

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult Login(LoginModel model,string require)
        {
            using (QLBHEntities ctx = new QLBHEntities())
            {
                ViewBag.Users = ctx.users.Count();
                ViewBag.NewRegister = ctx.users.OrderByDescending(c => c.Id).FirstOrDefault().Name;

                int n = ctx.users.Where(c => c.Username == model.UID).Count();

                if (n > 0)
                {
                    user loginUser = ctx.users.Where(c => c.Username == model.UID).FirstOrDefault();

                    if (MD5Encrypt.Encrypt(model.PWD) == loginUser.Password)
                    {
                        if (model.Cookie != null)
                        {
                            if (model.Cookie == "on")
                            {
                                HttpCookie cookie = new HttpCookie("UserId");
                                cookie.Value = loginUser.Id.ToString();

                                cookie.Expires = DateTime.Now.AddDays(30);
                                Response.SetCookie(cookie);
                            }
                        }

                        Session["IsLogged"] = 1;
                        Session["User"] = new SessionUser
                        {
                            Id = loginUser.Id,
                            Name = loginUser.Name,
                            Permission = loginUser.Permission
                        };

                        if (require!=null)
                        {
                            if (require == "cart")
                            {
                                return Json(new { Success = "Đăng nhập thành công", Address = "/Cart/ViewCart" });
                            }
                        }

                        return Json(new { Success = "Đăng nhập thành công",Address="/Home/Index" });
                    }
                    else
                    {
                        return Json(new { Error = "Mật khẩu không đúng" });
                    }
                }
                else
                {
                    return Json(new { Error = "Tên tài khoản không tồn tại" });
                }
            }
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            using (QLBHEntities ctx = new QLBHEntities())
            {
                int n = ctx.users.Where(c => c.Username == model.UID).Count();

                if (n > 0)
                {
                    return Json(false);
                }

                user ur = new user
                {
                    Username = model.UID,
                    Password = MD5Encrypt.Encrypt(model.PWD),
                    Name = model.FullName,
                    DOB = DateTime.ParseExact(model.DOB, "d/M/yyyy", null),
                    Address = "none",
                    Email = model.Email,
                    Phone = model.Phone,
                    Permission = 0
                };

                ctx.users.Add(ur);
                ctx.SaveChanges();

                return Json(true);


            }
        }


        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public JsonResult CheckCaptcha(string captchaId, string instanceId, string userInput)
        {
            bool ajaxValidationResult = CaptchaControl.AjaxValidate(captchaId, userInput, instanceId);
            return Json(ajaxValidationResult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PartialViewUserOrders()
        {
            using (QLBHEntities ctx = new QLBHEntities())
            {
                var currentUser = CurrentContext.GetSessionUser();

                var userOrders = ctx.orders.Include("orderdetails").Where(c => c.UserId == currentUser.Id);

                return PartialView(userOrders.OrderByDescending(c=>c.OrderDate).ToList());
            }
        }

        public ActionResult UserProfile()
        {
            if (CurrentContext.IsLogged() == true)
            {
                var currentUser = CurrentContext.GetSessionUser();
                UserInfoModel userInfo = null;

                using (QLBHEntities ctx = new QLBHEntities())
                {
                    user dbUser = ctx.users.Where(c => c.Id == currentUser.Id).FirstOrDefault();

                    userInfo = new UserInfoModel
                    {
                        UID = dbUser.Username,
                        FullName = dbUser.Name,
                        DOB = dbUser.DOB.ToString("d/M/yyyy"),
                        Email = dbUser.Email,
                        Phone = dbUser.Phone
                    };
                }
                return View(userInfo);
            }
            else return RedirectToAction("Login", "Account");
        }
        const int WrongPass = 1;
        const int SamePass = 2;
        const int PwdChanged = 1;
        const int InfoChanged = 2;

        [HttpPost]
        public ActionResult UserProfile(UserInfoModel info,int? opt)
        {


            using (QLBHEntities ctx = new QLBHEntities())
            {
                user dbUser = ctx.users.Where(c => c.Username == info.UID).FirstOrDefault();

                if (opt == 1)
                {
                    if (dbUser.Password != MD5Encrypt.Encrypt(info.PWD))
                    {
                        return Json(new { Error = WrongPass });
                    }
                    else
                    {
                        if (dbUser.Password == MD5Encrypt.Encrypt(info.NewPWD))
                        {
                            return Json(new { Error = SamePass });
                        }
                        else
                        {
                            dbUser.Password = MD5Encrypt.Encrypt(info.NewPWD);
                            ctx.SaveChanges();

                            return Json(new { Success = PwdChanged });
                        }
                    }
                }

                if (opt == 0)
                {
                    dbUser.Name = info.FullName;
                    dbUser.DOB = DateTime.ParseExact(info.DOB, "d/M/yyyy", null);
                    dbUser.Email = info.Email;
                    dbUser.Phone = info.Phone;

                    ctx.SaveChanges();

                    ((SessionUser)Session["User"]).Name = info.FullName;

                    return Json(new { Success = InfoChanged });
                }

               
            }

            return RedirectToAction("UserProfile","Account");
        }
    }
}