using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QLBH_MVC.Models
{
    public class SessionCart
    {
        public List<SessionCartProduct> cart;

        public decimal TotalPrice
        {
            get
            {
                return cart.Sum(c => c.Amount * c.Price);
            }
        }

        public void EmptyCart()
        {
            cart = new List<SessionCartProduct>();
        }

        public int Total
        {
            get
            {
                return TotalProduct();
            }
        }

        public void RemoveProduct(int id)
        {
            cart.Remove(cart.Where(c => c.Id == id).FirstOrDefault());
        }

        public bool AddProduct(SessionCartProduct proc)
        {
            var query = cart.Where(c => c.Id == proc.Id);

            if (query.Count() == 0)
            {
                if (proc.Amount > proc.Quantity)
                {
                    return false;
                }
                else
                {
                    cart.Add(proc);
                }
            }
            else
            {
                SessionCartProduct existProc = query.FirstOrDefault();
                int tempAmount = existProc.Amount + proc.Amount;

                if (tempAmount > existProc.Quantity)
                {
                    return false;
                }
                else
                {
                    existProc.Amount += proc.Amount;
                }
            }

            return true;
        }

        public bool UpdateProduct(SessionCartProduct proc)
        {
            var query = cart.Where(c => c.Id == proc.Id);


            SessionCartProduct existProc = query.FirstOrDefault();

            if (proc.Amount > existProc.Quantity)
            {
                return false;
            }
            else
            {
                existProc.Amount = proc.Amount;
            }

            return true;
        }

        public int TotalProduct()
        {
            return cart.Sum(c => c.Amount);
        }

        public SessionCart()
        {
            if (cart == null)
            {
                cart = new List<SessionCartProduct>();
            }
        }
    }
}