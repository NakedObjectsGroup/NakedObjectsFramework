// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Component;
using NakedFramework.Core.Adapter;
using NakedFramework.Core.Util;
using NakedFramework.Persistor.EFCore.Component;

namespace NakedFramework.Persistor.EFCore.Util {
    public static class EFCoreHelpers {
        private static IEntityType GetEntityType(this DbContext context, Type type) => context.Model.FindEntityType(type.GetProxiedType());

        private static PropertyInfo[] GetKeys(this IEntityType eType, Type type) {
            var keyProperties = eType.GetKeys().SelectMany(k => k.Properties);
            return keyProperties.Select(p => p.PropertyInfo).ToArray();
        }

        public static bool HasEntityType(this DbContext context, Type type) => context.GetEntityType(type) is not null;

        public static PropertyInfo[] SafeGetKeys(this DbContext context, Type type) => context.GetEntityType(type)?.GetKeys(type) ?? Array.Empty<PropertyInfo>();

        public static object[] GetKeyValues(this DbContext context, object obj) {
            var eType = context.GetEntityType(obj.GetType());
            var keyProperties = eType.GetKeys().SelectMany(k => k.Properties);
            return keyProperties.Select(p => p.PropertyInfo.GetValue(obj, null)).ToArray();
        }

        public static PropertyInfo[] GetNonIdMembers(this DbContext context, Type type) {
            var eType = context.GetEntityType(type);
            var keyProperties = eType.GetKeys().SelectMany(k => k.Properties).Select(p => p.PropertyInfo).ToArray();
            var properties = eType.GetProperties().Select(p => p.PropertyInfo).Where(pi => pi is not null).ToArray();
            var nonIdProperties = properties.Except(keyProperties);
            var valueNavigations = eType.GetNavigations().Select(p => p.PropertyInfo).Where(pi => pi is not null).Where(pi => pi.PropertyType.IsValueType);
            return nonIdProperties.Union(valueNavigations).ToArray();
        }

        public static PropertyInfo[] GetReferenceMembers(this DbContext context, Type type) {
            var eType = context.GetEntityType(type);
            var properties = eType.GetNavigations();
            return properties.Select(p => p.PropertyInfo).Where(pi => pi is not null && !pi.PropertyType.IsValueType && !CollectionUtils.IsCollection(pi.PropertyType)).ToArray();
        }

        public static PropertyInfo[] GetCollectionMembers(this DbContext context, Type type) {
            var eType = context.GetEntityType(type);
            var properties = eType.GetNavigations();
            var propertyInfos = properties.Select(p => p.PropertyInfo);
            return propertyInfos.Where(p => CollectionUtils.IsCollection(p.PropertyType)).ToArray();
        }

        public static void Invoke(this object onObject, string name, params object[] parms) => onObject.GetType().GetMethod(name)?.Invoke(onObject, parms);

        public static PropertyInfo[] GetComplexMembers(this DbContext context, Type type) {
            var eType = context.GetEntityType(type);
            if (eType is not null) {
                //var cm = eType.Members.Where(m => m.TypeUsage.EdmType is ComplexType);
                //return type.GetProperties().Join(cm, pi => pi.Name, em => em.Name, (pi, em) => pi).ToArray();
                // todo
            }

            return Array.Empty<PropertyInfo>();
        }

        public static bool IdMembersAreIdentity(this DbContext context, Type type) {
            var eType = context.GetEntityType(type);
            if (eType is not null) {
                //var mp = et.KeyMembers.SelectMany(m => m.MetadataProperties).Where(p => p.Name.Contains("StoreGeneratedPattern")).ToArray();
                //return mp.Any() && mp.All(p => p.Value.Equals("Identity"));
                // todo
            }

            return false;
        }

        //private static readonly IDictionary<DbContext, List<INakedObjectAdapter>> PersistedNakedObjectDictionary = new Dictionary<DbContext, List<INakedObjectAdapter>>();

        //public static List<INakedObjectAdapter> PersistedNakedObjects(this DbContext context) {
        //    lock (PersistedNakedObjectDictionary) {
        //        if (!PersistedNakedObjectDictionary.ContainsKey(context)) {
        //            PersistedNakedObjectDictionary.Add(context, new List<INakedObjectAdapter>());
        //        }
        //        return PersistedNakedObjectDictionary[context];
        //    }
        //}

        //private static readonly IDictionary<DbContext, AdapterHolder> CurrentSaveRootObjectAdapterDictionary = new Dictionary<DbContext, AdapterHolder>();

        //public class AdapterHolder {
        //    public INakedObjectAdapter Adapter {
        //        get;
        //        set;
        //    }
        //}

        //public static AdapterHolder CurrentSaveRootObjectAdapter(this DbContext context) {
        //    lock (CurrentSaveRootObjectAdapterDictionary)
        //    {
        //        if (!CurrentSaveRootObjectAdapterDictionary.ContainsKey(context))
        //        {
        //            CurrentSaveRootObjectAdapterDictionary.Add(context, new AdapterHolder());
        //        }
        //        return CurrentSaveRootObjectAdapterDictionary[context];
        //    }
        //}

     

        public static void UpdateVersion(this INakedObjectAdapter nakedObjectAdapter, ISession session, INakedObjectManager manager)
        {
            var versionObject = nakedObjectAdapter?.GetVersion(manager);
            if (versionObject != null)
            {
                nakedObjectAdapter.OptimisticLock = new ConcurrencyCheckVersion(session.UserName, DateTime.Now, versionObject);
            }
        }

        public static bool IsEFCoreProxy(Type type) => FasterTypeUtils.IsCastleProxy(type.FullName ?? "");

        public static string GetEFCoreProxiedTypeName(object domainObject) => domainObject.GetEFCoreProxiedType().FullName;

        public static Type GetEFCoreProxiedType(this object domainObject) =>
            domainObject.GetType() switch
            {
                { } t when IsEFCoreProxy(t) => t.BaseType,
                { } t => t
            };
    }
}