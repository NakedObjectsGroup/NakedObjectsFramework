// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using NakedObjects.Architecture.Adapter;

namespace NakedObjects.Core.Util {
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