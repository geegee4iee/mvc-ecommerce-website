using QLBH_MVC.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QLBH_MVC.Filters
{
    public class LoginRequiredAttribute : ActionFilterAttribute
    {
        public int Permission { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (CurrentContext.IsLogged() == false)
            {
                string controller = filterContext.RouteData.Values["controller"].ToString();
                string action = filterContext.RouteData.Values["action"].ToString();

                filterContext.Result = new RedirectResult(string.Format("~/Account/Login?require=/{0}/{1}",
                    controller,
                    action
                    ));
            }

            if (Permission >= 1)
            {
                if (CurrentContext.GetSessionUser() != null)
                {
                    if (CurrentContext.GetSessionUser().Permission < 1)
                    {
                        filterContext.Result = new RedirectResult("~/Home/Index");
                    }
                    else
                    {
                    }
                }
                else
                {
                }

            }



            base.OnActionExecuting(filterContext);
        }
    }
}