// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NakedObjects.Resources;
using NakedObjects.Util;

namespace NakedObjects.Services {
    /// <summary>
    ///     An implementation of IObjectFinder. Works with multiple keys, of type Integer, String, Short, or Char
    /// </summary>
    public class ObjectFinder : IObjectFinder {
        #region Injected Services

        public IDomainObjectContainer Container { set; protected get; }

        #endregion

        protected object FindByKeyGeneric<TActual>(object id) where TActual : class {
            return Container.FindByKey<TActual>(id);
        }

        private T FindByKey<T>(Type type, object id) {
            if (type != null && id != null) {
                MethodInfo m = GetType().GetMethod("FindByKeyGeneric", BindingFlags.Instance | BindingFlags.NonPublic);
                MethodInfo gm = m.MakeGenericMethod(new[] {type});
                return (T) gm.Invoke(this, new[] {id});
            }

            return default(T);
        }

        private object GetSingleKey(object obj) {
            object key = Container.GetSingleKey(obj.GetType()).GetValue(obj, null);
            return key;
        }

        protected object FindByKeysGeneric<TActual>(Dictionary<string, object> keyDict) where TActual : class {
            return Container.FindByKeys<TActual>(keyDict.Values.ToArray());
        }

        private T FindByKeys<T>(Type type, Dictionary<string, object> keyDict) {
            if (type != null && keyDict != null) {
                MethodInfo m = GetType().GetMethod("FindByKeysGeneric", BindingFlags.Instance | BindingFlags.NonPublic);
                MethodInfo gm = m.MakeGenericMethod(new[] {type});
                return (T) gm.Invoke(this, new[] {keyDict});
            }

            return default(T);
        }

        protected Type GetAssociatedObjectType(string compoundKey) {
            string typeAsString = compoundKey.Split('|').ElementAt(0);
            if (string.IsNullOrEmpty(typeAsString)) {
                throw new DomainException(string.Format(ProgrammingModel.CompoundKeyDoesNotContainType, compoundKey));
            }

            Type type = TypeFromCode(typeAsString);
            if (type == null) {
                throw new DomainException(string.Format(ProgrammingModel.TypeCannotBeFound, typeAsString));
            }

            return type;
        }

        // Creates a dictionary of the keys of the form <keyPropertyName, keyPropertyValue) where the type of
        // the keyPropertyValue is the correct type based on key properties info extracted from the input type.
        private Dictionary<string, object> CreateKeyDictionary(Type type, string compoundKey) {
            PropertyInfo[] keyProperties = Container.GetKeys(type);
            string[] valuesAsStrings = ExtractKeyValuesAsStrings(compoundKey);

            //test that the number of key values is correct for the type
            if (valuesAsStrings.Count() != keyProperties.Count()) {
                throw new DomainException(string.Format(ProgrammingModel.NumberOfkeysMismatch, type));
            }

            //Create the dictionary
            var keyDict = new Dictionary<string, object>();
            for (int i = 0; i < keyProperties.Count(); i++) {
                string stringValue = valuesAsStrings[i];
                object value = null;
                Type propType = keyProperties[i].PropertyType;
                try {
                    if (propType == typeof(string)) {
                        value = stringValue;
                    }
                    else if (propType == typeof(int)) {
                        value = int.Parse(stringValue);
                    }
                    else if (propType == typeof(short)) {
                        value = short.Parse(stringValue);
                    }
                    else if (propType == typeof(char)) {
                        value = char.Parse(stringValue);
                    }
                    else if (propType == typeof(DateTime)) {
                        value = DateTime.Parse(stringValue);
                    }
                    else {
                        throw new DomainException(string.Format(ProgrammingModel.InvalidKeyType, propType));
                    }
                }
                catch (FormatException) {
                    throw new DomainException(string.Format(ProgrammingModel.KeyTypeMismatch, stringValue, propType));
                }

                keyDict.Add(keyProperties[i].Name, value);
            }

            return keyDict;
        }

        private string[] ExtractKeyValuesAsStrings(string compoundKey) {
            string[] keyStrings = compoundKey.Split('|');

            var keyValues = new List<string>();
            //Skip 1 as the first item is the type rather than a key
            foreach (string keyString in keyStrings.Skip(1)) {
                keyValues.Add(keyString);
            }

            return keyValues.ToArray();
        }

        #region Implementation of IObjectFinder

        public virtual T FindObject<T>(string compoundKey) {
            Type type = GetAssociatedObjectType(compoundKey);

            if (typeof(IHasGuid).IsAssignableFrom(type)) {
                MethodInfo m = GetType().GetMethod("FindObjectByGuid", BindingFlags.Instance | BindingFlags.NonPublic);
                MethodInfo gm = m.MakeGenericMethod(new[] {type});
                return (T) gm.Invoke(this, new[] {compoundKey});
            }

            Dictionary<string, object> keyDict = CreateKeyDictionary(type, compoundKey);
            if (keyDict.Any()) {
                return FindByKeys<T>(type, keyDict);
            }

            throw new DomainException(string.Format(ProgrammingModel.CompoundKeyHasNoKeyValues, compoundKey));
        }

        public virtual string GetCompoundKey<T>(T obj) {
            var compoundKey = new StringBuilder();
            //In all cases, the key starts with the fully-qualified type name
            string typeAsString = CodeFromType(obj);
            compoundKey.Append((obj == null) ? null : typeAsString);

            if (typeof(IHasGuid).IsAssignableFrom(typeof(T))) {
                compoundKey.Append("|");
                var objWithGuid = (IHasGuid) obj;
                compoundKey.Append(objWithGuid.Guid.ToString()); //but must generate this dynamically
            }
            else {
                PropertyInfo[] keyProperties = Container.GetKeys(obj.GetType());
                if (!keyProperties.Any()) {
                    throw new DomainException(string.Format(ProgrammingModel.NoKeysDefined, obj));
                }

                foreach (PropertyInfo key in keyProperties) {
                    compoundKey.Append("|");
                    compoundKey.Append((obj == null) ? null : key.GetValue(obj, null).ToString());
                }
            }

            return compoundKey.ToString();
        }

        public object FindBySingleIntegerKey(Type type, int key) {
            return FindByKey<object>(type, key);
        }

        public T FindBySingleIntegerKey<T>(int key) where T : class {
            return (T) FindByKeyGeneric<T>(key);
        }

        private TActual FindObjectByGuid<TActual>(string compoundKey) where TActual : class, IHasGuid {
            var guidFromKey = new Guid(compoundKey.Split('|').ElementAt(1));
            IQueryable<TActual> q = from t in Container.Instances<TActual>()
                where t.Guid == guidFromKey
                select t;
            return q.FirstOrDefault();
        }

        #endregion

        #region Convert between Type and string representation (code) for Type

        protected virtual Type TypeFromCode(string code) {
            return TypeUtils.GetType(code);
        }

        protected virtual string CodeFromType(object obj) {
            Type type = obj.GetType().GetProxiedType();
            return type.FullName;
        }

        #endregion

        #region Instances

        protected IQueryable<T> InstancesGeneric<T>() where T : class {
            return Container.Instances<T>();
        }

        public IQueryable<T> Instances<T>(Type type) {
            if (!typeof(T).IsAssignableFrom(type)) throw new DomainException(type + " does not implement " + typeof(T));
            MethodInfo m = GetType().GetMethod("InstancesGeneric", BindingFlags.Instance | BindingFlags.NonPublic);
            MethodInfo gm = m.MakeGenericMethod(new[] {type});
            return gm.Invoke(this, new object[] { }) as IQueryable<T>;
        }

        #endregion
    }
}