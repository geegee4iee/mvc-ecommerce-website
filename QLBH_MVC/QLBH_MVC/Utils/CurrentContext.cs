using QLBH_MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QLBH_MVC.Utils
{
    public class CurrentContext
    {
        public static bool IsLogged()
        {
            //Session["IsLogged"] == 1 means true
            if (HttpContext.Current.Session["IsLogged"] == null)
            {
                HttpCookie cookie = HttpContext.Current.Request.Cookies["UserId"];

                if (cookie == null)
                {
                    return false;
                }
                else
                {
                    int cookieId = Convert.ToInt32(cookie.Value);

                    using (QLBHEntities ctx = new QLBHEntities())
                    {
                        user currentUser = ctx.users.Where(c => c.Id == cookieId).FirstOrDefault();

                        SessionUser sessUser = new SessionUser
                        {
                            Id = currentUser.Id,
                            Name = currentUser.Name,
                            Permission = currentUser.Permission
                        };

                        HttpContext.Current.Session["User"] = sessUser;
                        HttpContext.Current.Session["IsLogged"] = 1;

                        return true;
                    }
                }
            }
            else
            {
                if ((int)HttpContext.Current.Session["IsLogged"] == 1)
                {
                    return true;
                }
            }

            return false;
        }

        public static SessionUser GetSessionUser()
        {
            return (SessionUser)HttpContext.Current.Session["User"];
        }

        public static SessionCart GetSessionCart()
        {
            if ((SessionCart)HttpContext.Current.Session["Cart"] == null)
            {
                HttpContext.Current.Session["Cart"] = new SessionCart();   
            }

            return ((SessionCart)HttpContext.Current.Session["Cart"]);
        }

        public static void EmptyCart()
        {
            ((SessionCart)HttpContext.Current.Session["Cart"]).EmptyCart();
        }

        public static void Logout()
        {
            HttpContext.Current.Session.Abandon();
            HttpContext.Current.Response.Cookies["UserId"].Expires = DateTime.Now.AddDays(-1);
        }
    }
}