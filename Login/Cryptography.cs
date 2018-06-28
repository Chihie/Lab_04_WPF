using System;
using System.Security.Cryptography;

namespace Login
{
    //TODO: Test stronger cryptography algorithms, secure db, pass passwords inside secure strings
    public class Cryptography
    {
        public string SecurePassword(string pass)
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
            byte[] hash = new Rfc2898DeriveBytes(pass,salt,10000).GetBytes(20);
            byte[] hashBytes = new byte[36];
            Array.Copy(salt,0,hashBytes,0,16);
            Array.Copy(hash,0,hashBytes,16,20);
            return Convert.ToBase64String(hashBytes);
        }

        public Boolean RetrivePassword(string passLogin, string passHashed)
        {
            byte[] hashBytes = Convert.FromBase64String(passHashed);
            byte[] salt = new byte[16];
            Array.Copy(hashBytes,0,salt,0,16);
            byte[] hash = new Rfc2898DeriveBytes(passLogin,salt,10000).GetBytes(20);
            Boolean isMatch = true;
            for(UInt16 i=0;i<20;i++)
                if (hashBytes[i + 16] != hash[i]) isMatch = false;
            if (isMatch) return true;
            return false;
        }
    }
}