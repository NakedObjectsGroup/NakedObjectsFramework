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
using NakedFramework.Facade.Utility;

namespace NakedFramework.Facade.Impl.Utility; 

public class SimpleStringHasher : IStringHasher {
    #region IStringHasher Members

    public string GetHash(string toHash) => ComputeSha256HashAsString(toHash);

    #endregion

    private static string ComputeSha256HashAsString(string s) => Math.Abs(BitConverter.ToInt64(ComputeSha256HashFromString(s), 0)).ToString(CultureInfo.InvariantCulture);

    private static byte[] ComputeSha256HashFromString(string s) {
        var idAsBytes = Encoding.UTF8.GetBytes(s);
#pragma warning disable SYSLIB0021 // Type or member is obsolete
        return new SHA256Managed().ComputeHash(idAsBytes);
#pragma warning restore SYSLIB0021 // Type or member is obsolete
    }
}