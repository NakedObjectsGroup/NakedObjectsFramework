// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace NakedObjects.Architecture.Facets {
    public static class IdentifierUtils {
        public static int ComputeMD5HashAsInt(this IIdentifier id, CheckType checkType) {
            return Math.Abs(BitConverter.ToInt32(ComputeMD5HashFromIdentifier(id, checkType), 0));
        }

        private static byte[] ComputeMD5HashFromIdentifier(IIdentifier id, CheckType checkType) {
            return ComputeMD5HashFromString(id.ToIdentityStringWithCheckType(IdentifierDepth.ClassNameParams, checkType));
        }

        public static string ComputeMD5HashAsString(string s) {
            return Math.Abs(BitConverter.ToInt64(ComputeMD5HashFromString(s), 0)).ToString(CultureInfo.InvariantCulture);
        }

        private static byte[] ComputeMD5HashFromString(string s) {
            return new MD5CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(s));
        }
    }
}