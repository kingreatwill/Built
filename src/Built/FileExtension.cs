using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Built
{
    public class FileExtension
    {
        public static string GetMD5(string fileName)
        {
            if (!File.Exists(fileName)) return string.Empty;
            using (FileStream file = new FileStream(fileName, FileMode.Open))
            {
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] hash = md5.ComputeHash(file);
                StringBuilder result = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    result.Append(hash[i].ToString("x2"));
                }
                return result.ToString();
            }
        }
    }
}