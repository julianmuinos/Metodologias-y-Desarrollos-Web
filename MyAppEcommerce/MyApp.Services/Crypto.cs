using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Services
{
    public static class Crypto
    {
        static UnicodeEncoding _uCode = new UnicodeEncoding();
        static MD5CryptoServiceProvider _md5 = new MD5CryptoServiceProvider();
        public static string Parse(string pString)
        {
            return Convert.ToBase64String(_md5.ComputeHash(_uCode.GetBytes(pString)));
        }
        public static bool Compare(string pString, string pStringEncrypted)
        {
            return Crypto.Parse(pString) == pStringEncrypted ? true : false;
        }
    }
}
