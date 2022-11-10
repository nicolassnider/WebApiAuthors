using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using WebApiAuthors.DTOs;

namespace WebApiAuthors.Services
{
    public class HashService
    {
        public HashResult Hash(string plainText)
        {
            var salt = new byte[16];
            using(var random= RandomNumberGenerator.Create())
            {
                random.GetBytes(salt);
                return Hash(plainText, salt);
            }
        }
        public HashResult Hash(string plainText, byte[] salt)
        {
            var derivatedKey = KeyDerivation.Pbkdf2(
                plainText,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 32);
            var hash = Convert.ToBase64String(derivatedKey);

            return new HashResult()
            {
                Hash = hash,
                Salt = salt
            };
        }
    }
}
