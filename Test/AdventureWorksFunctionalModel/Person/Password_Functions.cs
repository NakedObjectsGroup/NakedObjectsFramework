using NakedFunctions;
using System;
using System.ComponentModel.DataAnnotations;

namespace AdventureWorksModel {
    public static class PasswordFunctions
    {
        public static Password Updating(Password pw, IContext context) => pw with { ModifiedDate = context.Now() };
    }
}
