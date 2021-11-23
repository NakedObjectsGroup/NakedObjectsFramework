using System.Security.Cryptography;

namespace AW.Functions;
public static class Password_Functions
{
    [MemberOrder("Passwords", 1)]
    public static IContext CheckPassword(
        this Person p, [Password] string offered, IContext context) =>
        context.WithInformUser(p.Password?.OfferedPasswordIsCorrect(offered) == true ? "CORRECT" : "INCORRECT");

    public static bool HideCheckPassword(this Person p) => p.Password is null;

    internal static bool OfferedPasswordIsCorrect(this Password pw, string offered) =>
        Hashed(offered, pw.PasswordSalt).Trim() == pw.PasswordHash.Trim();

    private static string Hashed(string password, string salt)
    {
        var saltedPassword = password + salt;
        var data = Encoding.ASCII.GetBytes(saltedPassword);
        var hash = SHA256.Create().ComputeHash(data);
        var chars = hash.Select(b => Convert.ToChar(b % 128)).ToArray();
        return new string(chars);
    }

    private static string CreateRandomSalt()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        var output = new StringBuilder();
        for (var i = 1; i <= 7; i++)
        {
            output.Append(chars.Substring(random.Next(chars.Length - 1), 1));
        }

        output.Append("=");
        return output.ToString();
    }

    #region ChangePassword

    public static IContext ChangePassword(this Person p,
                                          [Password] string oldPassword,
                                          [Password] string newPassword,
                                          [Named("New Password (Confirm)")] [Password]
                                              string confirm,
                                          IContext context)
    {
        var oldP = p.Password!;
        var salt = CreateRandomSalt();
        Password newP = new(oldP)
        {
            PasswordSalt = salt,
            PasswordHash = Hashed(newPassword, salt),
            ModifiedDate = context.Now()
        };
        return context.WithUpdated(oldP, newP);
    }

    public static string ValidateChangePassword(this Person p,
                                                string oldPassword, string newPassword, string confirm, IContext context)
    {
        var reason = "";
        if (p.Password?.OfferedPasswordIsCorrect(oldPassword) == true)
        {
            reason += "Old Password is incorrect";
        }

        if (newPassword == oldPassword)
        {
            reason += "New Password should be different from Old Password";
        }

        reason += ValidateNewPassword(newPassword, confirm);
        return reason;
    }

    public static bool HideChangePassword(this Person p) => p.Password is null;

    #endregion

    #region Initial Password

    public static IContext CreateInitialPassword(this Person p,
                                                 [Password] string newPassword,
                                                 [Named("New Password (Confirm)")] [Password]
                                                     string confirm,
                                                 IContext context) =>
        context.WithNew(CreateNewPassword(newPassword, p, context));

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

    public static bool HideCreateInitialPassword(this Person p) => p.Password is not null;

    internal static Password CreateNewPassword(string newPassword, Person person, IContext context)
    {
        var salt = CreateRandomSalt();
        return new Password
        {
            Person = person,
            PasswordSalt = salt,
            PasswordHash = Hashed(newPassword, salt),
            rowguid = context.NewGuid(),
            ModifiedDate = context.Now()
        };
    }

    #endregion
}