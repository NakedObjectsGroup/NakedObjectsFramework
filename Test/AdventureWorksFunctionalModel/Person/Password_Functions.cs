using NakedFunctions;
using AW.Types;
using System;


namespace AW.Functions {
    public static class PasswordFunctions
    {
        public static Password Updating(Password pw, IContext context) => pw with { ModifiedDate = context.Now() };
    }
}