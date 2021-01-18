using NakedFunctions;
using AW.Types;
using System.Text;
using System.Security.Cryptography;
using System;

namespace AW.Functions {
    public static class Password_Functions
    {

        internal static Password CreateNewPassword(string newPassword, int businessEntityId, IContext context)
        {
            var salt = CreateRandomSalt();
            var pw = new Password()
            {
                BusinessEntityID = businessEntityId,
                PasswordSalt = salt,
                PasswordHash = Hashed(newPassword, salt),
                rowguid = context.NewGuid(),
                ModifiedDate = context.Now()
            };
            return pw;
        }

        internal static bool OfferedPasswordIsCorrect(this Password pw, string offered) =>
          Hashed(offered, pw.PasswordSalt) == pw.PasswordHash;

        private static string Hashed(this string password, string salt)
        {
            string saltedPassword = password + salt;
            byte[] data = Encoding.UTF8.GetBytes(saltedPassword);
            byte[] hash = SHA256.Create().ComputeHash(data);
            char[] chars = Encoding.UTF8.GetChars(hash);
            return new string(chars);
        }

        private static string CreateRandomSalt()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var output = new StringBuilder();
            for (int i = 1; i <= 7; i++)
            {
                output.Append(chars.Substring(random.Next(chars.Length - 1), 1));
            }
            output.Append("=");
            return output.ToString();
        }
    }
}
