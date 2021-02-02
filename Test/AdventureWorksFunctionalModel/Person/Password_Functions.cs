using NakedFunctions;
using AW.Types;
using System.Text;
using System.Security.Cryptography;
using System;
using System.Linq;

namespace AW.Functions {
    public static class Password_Functions
    {

        #region ChangePassword 
        public static (BusinessEntity, IContext) ChangePassword(this BusinessEntity be,
            [Password] string oldPassword, 
            [Password] string newPassword, 
            [Named("New Password (Confirm)"), Password] string confirm,
            IContext context) =>
            (be, context.WithNew(CreateNewPassword(newPassword, be.BusinessEntityID, context)));

        public static string ValidateChangePassword(this BusinessEntity be, 
            string oldPassword, string newPassword, string confirm, IContext context)
        {
            var reason = "";
            if (!MostRecentPassword(be.BusinessEntityID, context).OfferedPasswordIsCorrect(oldPassword))
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


        internal static Password MostRecentPassword(int forBusinessEntityID, IContext context) =>
            context.Instances<Password>().Where(p => p.BusinessEntityID == forBusinessEntityID).
            OrderByDescending(p => p.ModifiedDate).FirstOrDefault();
    }
}
