using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QLBH_MVC.Models
{
    public class SessionUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Permission { get; set; }
    }
}