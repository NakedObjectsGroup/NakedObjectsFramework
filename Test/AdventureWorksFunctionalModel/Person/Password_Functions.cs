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
            reason += ValidateNewPassword(newPassword, confirm);
            if (newPassword == oldPassword)
            {
                reason += "New Password should be different from Old Password";
            }
            return reason;
        }

        public static bool HideChangePassword(this Person p, IContext context) =>
            MostRecentPassword(p.BusinessEntityID, context) is null;
        #endregion

        #region Initial Password 
        public static (Person, IContext) CreateInitialPassword(this Person p,
            [Password] string newPassword,
            [Named("New Password (Confirm)"), Password] string confirm,
            IContext context) =>
                (p, context.WithNew(CreateNewPassword(newPassword, p, context)));

        public static string ValidateCreateInitialPassword(this Person p,
             string newPassword, string confirm, IContext context) =>
                ValidateNewPassword(newPassword, confirm);


        internal static string ValidateNewPassword(string newPassword, string confirm)
        {
            var reason = "";
            if (newPassword != confirm)
            {
                reason += "New Password and Confirmation don't match";
            }
            if (newPassword.Length < 6)
            {
                reason += "New Password must be at least 6 characters";
            }
            return reason;
        }

        public static bool HideCreateInitialPassword(this Person p, IContext context) =>
            MostRecentPassword(p.BusinessEntityID, context) is not null;

        #endregion


        internal static Password CreateNewPassword(string newPassword, Person person, IContext context)
        {
            var salt = CreateRandomSalt();
            var pw = new Password()
            {
                BusinessEntityID = person.BusinessEntityID,
                //Person = person,
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
