// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Reflection;
using System.Text;
using NakedObjects.Services;
using NakedObjects.Util;

namespace NakedObjects.EntityObjectStore {
    [Obsolete("This is an experimental class, not yet fully tested.  Use for experimentation only, not deployment.")]
    public class EntitySqlObjectFinder : IObjectFinder {
        #region Injected Services

        public IDomainObjectContainer Container { set; protected get; }

        #endregion

        #region Implementation of IObjectFinder

        public virtual T FindObject<T>(string compoundKey) {
            Type type = GetAssociatedObjectType(compoundKey);

            Dictionary<string, object> keyDict;
            if (typeof (IHasGuid).IsAssignableFrom(typeof (T))) {
                keyDict = new Dictionary<string, object> {{"Guid", new Guid(ExtractKeyValuesAsStrings(compoundKey)[0])}};
            }
            else {
                keyDict = CreateKeyDictionary(type, compoundKey);
            }

            return FindByKeys<T>(type, keyDict);
        }


        public virtual string GetCompoundKey<T>(T obj) {
            var compoundKey = new StringBuilder();
            compoundKey.Append((obj == null) ? null : obj.GetType().GetProxiedType());

            PropertyInfo[] keyProperties;

            if (obj is IHasGuid) {
                keyProperties = new[] {obj.GetType().GetProperty("Guid")};
            }
            else {
                keyProperties = Container.GetKeys(obj.GetType());
            }

            //new stuff
            foreach (PropertyInfo key in keyProperties) {
                compoundKey.Append("|");
                compoundKey.Append((obj == null) ? null : key.GetValue(obj, null).ToString());
            }
            return compoundKey.ToString();
        }

        public object FindBySingleIntegerKey(Type type, int key) {
            return FindByKey<object>(type, key);
        }

        public T FindBySingleIntegerKey<T>(int key) where T : class {
            return (T) FindByKeyGeneric<T>(key);
        }

        #endregion

        protected object FindByKeyGeneric<TActual>(object id) where TActual : class {
            string keyName = Container.GetSingleKey(typeof (TActual)).Name;
            return Container.Instances<TActual>().FindByKey(keyName, id);
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
            return FindByKeys(Container.Instances<TActual>(), keyDict);
        }

        private static T FindByKeys<T>(IQueryable<T> source, Dictionary<string, object> keys) {
            ObjectContext context = ((ObjectQuery) source).Context;

            EntityContainer container = context.MetadataWorkspace.GetEntityContainer(context.DefaultContainerName, DataSpace.CSpace);

            EntitySetBase entitySet = container.BaseEntitySets.FirstOrDefault(item => item.ElementType.Name.Equals(typeof (T).Name));

            // create a entity sql query 

            ObjectQuery<T> oq = context.CreateQuery<T>(entitySet.Name);

            foreach (var kvp in keys) {
                string query = string.Format("it.{0}=@{0}", kvp.Key);

                oq = oq.Where(query, new ObjectParameter(kvp.Key, kvp.Value));
            }


            return oq.SingleOrDefault();
        }


        private T FindByKeys<T>(Type type, Dictionary<string, object> keyDict) {
            if (type != null && keyDict != null) {
                MethodInfo m = GetType().GetMethod("FindByKeysGeneric", BindingFlags.Instance | BindingFlags.NonPublic);
                MethodInfo gm = m.MakeGenericMethod(new[] {type});
                return (T) gm.Invoke(this, new[] {keyDict});
            }
            return default(T);
        }

        private Type GetAssociatedObjectType(string interfaceKey) {
            string typeAsString = interfaceKey.Split('|').ElementAt(0);
            return string.IsNullOrEmpty(typeAsString) ? null : TypeUtils.GetType(typeAsString);
        }

        // Creates a dictionary of the keys of the form <keyPropertyName, keyPropertyValue) where the type of
        // the keyPropertyValue is the correct type based on key properties info extracted from the input type.
        private Dictionary<string, object> CreateKeyDictionary(Type type, string compoundKey) {
            PropertyInfo[] keyProperties = Container.GetKeys(type);
            string[] valuesAsStrings = ExtractKeyValuesAsStrings(compoundKey);

            //test that the number of key values is correct for the type
            if (valuesAsStrings.Count() != keyProperties.Count()) {
                throw new DomainException(string.Format(Resources.NakedObjects.NumberOfkeysMismatch, type));
            }

            //Create the dictionary
            var keyDict = new Dictionary<string, object>();
            for (int i = 0; i < keyProperties.Count(); i++) {
                string stringValue = valuesAsStrings[i];
                object value = null;
                Type propType = keyProperties[i].PropertyType;
                try {
                    if (propType == typeof (string)) {
                        value = stringValue;
                    }
                    else if (propType == typeof (int)) {
                        value = int.Parse(stringValue);
                    }
                    else if (propType == typeof (short)) {
                        value = short.Parse(stringValue);
                    }
                    else if (propType == typeof (char)) {
                        value = char.Parse(stringValue);
                    }
                    else {
                        throw new DomainException(string.Format(Resources.NakedObjects.InvalidKeyType, propType));
                    }
                }
                catch (FormatException) {
                    throw new DomainException(string.Format(Resources.NakedObjects.KeyTypeMismatch, stringValue, propType));
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
    }
}