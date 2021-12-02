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

namespace NakedFramework.Core.Util;

public static class IdentifierUtils {
    public static string ComputeMD5HashAsString(string s) => Math.Abs(BitConverter.ToInt64(ComputeMD5HashFromString(s), 0)).ToString(CultureInfo.InvariantCulture);

#pragma warning disable SYSLIB0021 // Type or member is obsolete
    private static byte[] ComputeMD5HashFromString(string s) => new MD5CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(s));
#pragma warning restore SYSLIB0021 // Type or member is obsolete
}