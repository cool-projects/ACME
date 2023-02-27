using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Helpers
{
    public class PasswordHelper
    {
        public static byte[] GetSecureSalt()
        {
            // Starting .NET 6, the Class RNGCryptoServiceProvider is obsolete,
            // so now we have to use the RandomNumberGenerator Class to generate a secure random number bytes

            return RandomNumberGenerator.GetBytes(32);
        }
        public static string HashUsingPbkdf2(string password, byte[] salt)
        {
            byte[] derivedKey = KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA256, iterationCount: 100000, 32);

            return Convert.ToBase64String(derivedKey);
        }
        public static byte[] StringToByteArray(string str)
        {
            Dictionary<string, byte> hexindex = new Dictionary<string, byte>();
            for (int i = 0; i <= 255; i++)
                hexindex.Add(i.ToString("X2"), (byte)i);

            List<byte> hexres = new List<byte>();
            for (int i = 0; i < str.Length; i += 2)
                hexres.Add(hexindex[str.Substring(i, 2)]);

            return hexres.ToArray();
        }
        public static bool ValidatePassword(string passwordInput, string hashedPassword, string passwordSalt)
        {
            var salt = StringToByteArray(passwordSalt);
            string newHashedPin = HashUsingPbkdf2(passwordInput, salt);
            return newHashedPin.Equals(hashedPassword);
        }
    }
}
