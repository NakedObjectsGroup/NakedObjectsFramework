using NakedFunctions;
using AW.Types;
using System.Text;
using System.Security.Cryptography;
using System;
using System.Linq;

namespace AW.Functions
{
    public static class Password_Functions
    {

        #region ChangePassword 
        public static (Person, IContext) ChangePassword(this Person p,
            [Password] string oldPassword,
            [Password] string newPassword,
            [Named("New Password (Confirm)"), Password] string confirm,
            IContext context) =>
            (p, context.WithNew(CreateNewPassword(newPassword, p, context)));

        public static string ValidateChangePassword(this Person p,
            string oldPassword, string newPassword, string confirm, IContext context)
        {
            var reason = "";
            if (!MostRecentPassword(p.BusinessEntityID, context).OfferedPasswordIsCorrect(oldPassword))
            {
                reason += "Old Password is incorrect";
            }
            if (newPassword != confirm)
            {
                reason += "New Password and Confirmation don't match";
            }
            if (newPassword.Length < 6)
            {
                reason += "New Password must be at least 6 characters";
            }
            if (newPassword == oldPassword)
            {
                reason += "New Password should be different from Old Password";
            }
            return reason;
        }
        #endregion

        public static (BusinessEntity, IContext) TestPassword(this Person p,
          [Password] string test,
          IContext context)
        {
            var pw = MostRecentPassword(p.BusinessEntityID, context);
            return pw is not null && OfferedPasswordIsCorrect(pw, test) ?
               (p, context.WithInformUser($"Entered password is correct."))
               : (p, context.WithInformUser($"Password is incorrect (or {p} has no password)."));
        }

        internal static Password CreateNewPassword(string newPassword, Person person, IContext context)
        {
            var salt = CreateRandomSalt();
            var pw = new Password()
            {
                Person = person,
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


        internal static Password MostRecentPassword(int forBusinessEntityID, IContext context) =>
            context.Instances<Password>().Where(p => p.BusinessEntityID == forBusinessEntityID).
            OrderByDescending(p => p.ModifiedDate).FirstOrDefault();
    }
}
