using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QLBH_MVC.Models
{
    public class SearchingModel
    {
        public String Product { get; set; }
        public int CatId { get; set; }
        public int MaId { get; set; }
        public int Price { get; set; }
        public int OptinalPrice { get; set; }
    }
}