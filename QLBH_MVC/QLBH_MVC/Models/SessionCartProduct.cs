using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QLBH_MVC.Models
{
    public class SessionCartProduct
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Amount { get; set; }
        public int Quantity { get; set; }
        public int Price { get; set; }
    }
}