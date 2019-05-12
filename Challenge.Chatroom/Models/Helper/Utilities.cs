using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Challenge.Chatroom.Models.Helper
{
    public static class Utilities
    {
        public static string EncryptMd5(this string input)
        {
            try
            {
                string output = string.Empty;

                if (!string.IsNullOrEmpty(input))
                {
                    MD5 md5 = MD5.Create();
                    byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                    byte[] hash = md5.ComputeHash(inputBytes);

                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < hash.Length; i++)
                    {
                        sb.Append(hash[i].ToString("X2"));
                    }

                    output = sb.ToString();
                }

                return output;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
