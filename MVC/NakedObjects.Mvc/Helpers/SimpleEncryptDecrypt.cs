// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace NakedObjects.Web.Mvc.Helpers {
    public class SimpleEncryptDecrypt : IEncryptDecrypt {
        public const string EncryptFieldPrefix = "-encryptedField-";
        public const string EncryptFieldData = "_encryptedFieldData";
        private readonly RijndaelManaged provider;
        private byte[] iv;
        private byte[] key;

        public SimpleEncryptDecrypt() {
            provider = new RijndaelManaged();
        }

        #region IEncryptDecrypt Members

        public Tuple<string, string> Encrypt(HttpSessionStateBase session, string name, string value) {
            string newValue = Encrypt(session, value);
            string newName = EncryptFieldPrefix + name;
            return new Tuple<string, string>(newName, newValue);
        }

        public void Decrypt(HttpSessionStateBase session, NameValueCollection collection) {
            if (collection.AllKeys.Any(k => k.StartsWith(EncryptFieldPrefix))) {
                foreach (string index in collection.AllKeys.Where(k => k.StartsWith(EncryptFieldPrefix))) {
                    string value = collection[index];
                    string newValue = Decrypt(session, value);
                    string newKey = index.Substring(EncryptFieldPrefix.Length);

                    collection.Add(newKey, newValue);
                }
            }
        }

        #endregion

        public string Encrypt(HttpSessionStateBase session, string toEncrypt) {
            byte[] valueBytes = Encoding.UTF8.GetBytes(toEncrypt);
            CreateOrUpdateKey(session);
            using (ICryptoTransform encrypter = provider.CreateEncryptor(key, iv)) {
                byte[] encryptedBytes = encrypter.TransformFinalBlock(valueBytes, 0, valueBytes.Length);
                return Convert.ToBase64String(encryptedBytes);
            }
        }

        public string Decrypt(HttpSessionStateBase session, string toDecrypt) {
            byte[] valueBytes = Convert.FromBase64String(toDecrypt);
            GetKey(session);
            using (ICryptoTransform decrypter = provider.CreateDecryptor(key, iv)) {
                byte[] decryptedBytes = decrypter.TransformFinalBlock(valueBytes, 0, valueBytes.Length);
                return Encoding.UTF8.GetString(decryptedBytes);
            }
        }

        private void CreateOrUpdateKey(HttpSessionStateBase session) {
            var data = session[EncryptFieldData] as Tuple<byte[], byte[]>;

            if (data == null) {
                provider.GenerateKey();
                provider.GenerateIV();
                data = new Tuple<byte[], byte[]>(provider.Key, provider.IV);
                session.Add(EncryptFieldData, data);
            }

            key = data.Item1;
            iv = data.Item2;
        }

        private void GetKey(HttpSessionStateBase session) {
            var data = session[EncryptFieldData] as Tuple<byte[], byte[]>;
            if (data == null)
            {
                throw new Exception("unexpected null: ");
            }
            key = data.Item1;
            iv = data.Item2;
        }
    }
}