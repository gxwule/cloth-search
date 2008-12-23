using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Zju.Dao;
using Zju.Domain;
using System.IO;


namespace Zju.Util
{
    public sealed class RegisterUtil
    {
        private static IRegisterDao registerDao = new RegisterDao();

        private const string DEFAULT_NAME = "Tony#123";

        private const int SEARCH_TIME_LIMIT = 280;

        private const int LOGIN_TIME_LIMIT = 30;

        private const string KEY_FILE_NAME = "C:/cloth.key";

        private static string MY_KEY = getMyKey();

        private static string getMyKey()
        {
            string s = null;
            try
            {
                if (File.Exists(KEY_FILE_NAME))
                {
                    StreamReader sr = File.OpenText(KEY_FILE_NAME);
                    s = sr.ReadLine();
                    sr.Close();
                }
            }
            catch (Exception)
            {
            	// do nothing
            }
            
            return s;
        }


        public static bool VerifyLogin()
        {
            RegisterInfo registerInfo = registerDao.FindByName(DEFAULT_NAME);
            if (registerInfo == null)
            {
                registerInfo = new RegisterInfo(DEFAULT_NAME, getMd5Hash(DEFAULT_NAME));
                registerDao.Insert(registerInfo);
            }

            bool re = true;
            if (string.IsNullOrEmpty(MY_KEY) || !MY_KEY.Equals(registerInfo.Md5Key))
            {
                if (registerInfo.LoginTime >= LOGIN_TIME_LIMIT)
                {
                    re = false;
                }
                else
                {
                    registerInfo.LoginTime++;
                    registerDao.UpdateNoneIndex(registerInfo);
                }
            }

            return re;
        }

        public static bool VerifySearch()
        {
            RegisterInfo registerInfo = registerDao.FindByName(DEFAULT_NAME);
            if (registerInfo == null)
            {
                return false;
            }

            bool re = true;
            if (string.IsNullOrEmpty(MY_KEY) || !MY_KEY.Equals(registerInfo.Md5Key))
            {
                if (registerInfo.SearchTime >= SEARCH_TIME_LIMIT)
                {
                    re = false;
                }
                else
                {
                    registerInfo.SearchTime++;
                    registerDao.UpdateNoneIndex(registerInfo);
                }
            }

            return re;
        }

        // Hash an input string and return the hash as
        // a 32 character hexadecimal string.
        public static string getMd5Hash(string input)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        // Verify a hash against a string.
        public static bool verifyMd5Hash(string input, string hash)
        {
            // Hash the input.
            string hashOfInput = getMd5Hash(input);

            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
