






using System;
using AW.Types;
using NakedFunctions;

namespace AW.Functions {
    public static class CreditCard_Functions {
        internal static string? ObfuscatedNumber(CreditCard cc) =>
            cc.CardNumber.Length > 4
                ? cc.CardNumber.Substring(cc.CardNumber.Length - 4).PadLeft(cc.CardNumber.Length, '*')
                : null;

        public static string? Validate(this CreditCard cc, byte expMonth, short expYear, IContext context) {
            if (expMonth == 0 || expYear == 0) {
                return null;
            }

            var today = context.Today();
            var expiryDate = new DateTime(expYear, expMonth, 1); //.EndOfMonth();
            if (expiryDate <= today) {
                return "Expiry date must be in the future";
            }

            return null;
        }
    }
}