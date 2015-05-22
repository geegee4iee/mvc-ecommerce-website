using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QLBH_MVC.Models
{
    public class RegisterModel
    {
        public string UID { get; set; }
        public string PWD { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string DOB { get; set; }
        public string Phone { get; set; }
    }
}