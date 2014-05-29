using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using NakedObjects.Services;

namespace RestfulObjects.Test.Data {
    public class TestKeyCodeMapper : IKeyCodeMapper {
        private const string KeySeparator = "-";
        private static readonly RijndaelManaged Provider = new RijndaelManaged();

        // these are constants so that tests are reproduceable
        private static readonly byte[] Iv = new byte[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
        private static readonly byte[] Key = new byte[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};

        public string[] KeyFromCode(string code, Type type) {
            string decryptedCode = string.IsNullOrEmpty(code) ? "" : Decrypt(code);
            return decryptedCode.Split(new[] {KeySeparator}, StringSplitOptions.None);
        }

        public string CodeFromKey(string[] key, Type type) {
            string instanceId = key.Aggregate("", (s, t) => s + (s == "" ? "" : KeySeparator) + t);
            return  string.IsNullOrEmpty(instanceId) ? instanceId : Encrypt(instanceId);
        }

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