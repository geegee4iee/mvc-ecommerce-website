using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace QLBH_MVC.Utils
{
    public class MD5Encrypt
    {
        public static string Encrypt(string _input)
        {
            MD5 md5 = MD5CryptoServiceProvider.Create();
            byte[] input = Encoding.Default.GetBytes(_input);
            byte[] output = md5.ComputeHash(input);
            string res = BitConverter.ToString(output).Replace("-", "");
            return res;
        }
    }
}