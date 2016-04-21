// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using NakedObjects.Services;

namespace RestfulObjects.Test.Data {
    public class TestKeyCodeMapper : IKeyCodeMapper {
        private const string KeySeparator = "--";
        private static readonly RijndaelManaged Provider = new RijndaelManaged();

        // these are constants so that tests are reproduceable
        private static readonly byte[] Iv = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
        private static readonly byte[] Key = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};

        #region IKeyCodeMapper Members

        public string[] KeyFromCode(string code, Type type) {
            string decryptedCode = string.IsNullOrEmpty(code) ? "" : Decrypt(code);
            return decryptedCode.Split(new[] {KeySeparator}, StringSplitOptions.None);
        }

        public string CodeFromKey(string[] key, Type type) {
            string instanceId = key.Aggregate("", (s, t) => s + (s == "" ? "" : KeySeparator) + t);
            return string.IsNullOrEmpty(instanceId) ? instanceId : Encrypt(instanceId);
        }

        #endregion

        public string KeyStringFromCode(string code) {
            return Decrypt(code);
        }

        public string CodeFromKeyString(string key) {
            return Encrypt(key);
        }


        private string Encrypt(string toEncrypt) {
            byte[] valueBytes = Encoding.UTF8.GetBytes(toEncrypt);

            using (ICryptoTransform encrypter = Provider.CreateEncryptor(Key, Iv)) {
                byte[] encryptedBytes = encrypter.TransformFinalBlock(valueBytes, 0, valueBytes.Length);
                return Convert.ToBase64String(encryptedBytes);
            }
        }

        private string Decrypt(string toDecrypt) {
            byte[] valueBytes = Convert.FromBase64String(toDecrypt);

            using (ICryptoTransform decrypter = Provider.CreateDecryptor(Key, Iv)) {
                byte[] decryptedBytes = decrypter.TransformFinalBlock(valueBytes, 0, valueBytes.Length);
                return Encoding.UTF8.GetString(decryptedBytes);
            }
        }
    }
}