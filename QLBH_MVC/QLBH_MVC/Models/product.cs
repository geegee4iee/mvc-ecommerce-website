//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace QLBH_MVC.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class product
    {
        public product()
        {
            this.orderdetails = new HashSet<orderdetail>();
        }
    
        public int ProId { get; set; }
        public string ProName { get; set; }
        public string ShortDes { get; set; }
        public string LongDes { get; set; }
        public int NewPrice { get; set; }
        public int OldPrice { get; set; }
        public int CatId { get; set; }
        public int Quantity { get; set; }
        public int ViewCount { get; set; }
        public System.DateTime DateAdd { get; set; }
        public int MaId { get; set; }
        public Nullable<int> Sale { get; set; }
    
        public virtual category category { get; set; }
        public virtual ICollection<orderdetail> orderdetails { get; set; }
        public virtual manufacturer manufacturer { get; set; }
    }
}
