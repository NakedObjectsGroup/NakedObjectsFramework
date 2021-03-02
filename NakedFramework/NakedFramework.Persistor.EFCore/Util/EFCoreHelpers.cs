// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using NakedFramework.Core.Util;

namespace NakedFramework.Persistor.EFCore.Util {
    public static class EFCoreHelpers {
        private static IEntityType GetEntityType(this DbContext context, Type type) => context.Model.FindEntityType(type.GetProxiedType());

        public static PropertyInfo[] GetKeys(this DbContext context, Type type) {
            var eType = context.GetEntityType(type);
            var keyProperties = eType.GetKeys().SelectMany(k => k.Properties);
            return keyProperties.Select(p => p.PropertyInfo).ToArray();
        }

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
    }
}