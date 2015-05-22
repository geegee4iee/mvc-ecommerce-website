using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QLBH_MVC.Models
{
    public class AdminProductModel
    {
        public int ProId { get; set; }
        public string ProName { get; set; }
        public string ShortDes { get; set; }
        [AllowHtml]
        public string LongDes { get; set; }
        public int NewPrice { get; set; }
        public int CatId { get; set; }
        public int MaId { get; set; }
        public int Quantity { get; set; }
    }
}