using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace AppAnunciosMVCWeb.Models
{
    public class Util
    {
        public static int ReturnInteger(Object o)
        {
            try
            {
                return Convert.ToInt32(o.ToString());
            }
            catch
            {
                return 0;
            }
        }
        public static string ReturnString(Object o)
        {
            try
            {
                return o.ToString();
            }
            catch
            {
                return null;
            }
        }
        public static decimal ReturnDecimal(Object o)
        {
            try
            {
                return Convert.ToDecimal(o.ToString());
            }
            catch
            {
                return 0;
            }
        }
        public static byte[] ReadFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }

        }
    }
}