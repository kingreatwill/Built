using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace System.IO
{
    public static class FileExtension
    {
        /// <summary>
        /// 获取文件的MD5
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns>MD5</returns>
        public static string GetMD5(this string fileName)
        {
            if (!File.Exists(fileName)) return string.Empty;
            byte[] hash;
            using (FileStream file = new FileStream(fileName, FileMode.Open))
            {
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                hash = md5.ComputeHash(file);
            }
            if (hash == null) return string.Empty;
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                result.Append(hash[i].ToString("x2"));
            }
            return result.ToString();
        }
    }
}