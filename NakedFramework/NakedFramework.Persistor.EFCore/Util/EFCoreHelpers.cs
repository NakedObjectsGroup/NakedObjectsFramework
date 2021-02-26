using System;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using NakedFramework.Core.Util;

namespace NakedFramework.Persistor.EFCore.Util {
    public static class EFCoreHelpers {
        public static PropertyInfo[] GetKeys(this DbContext context, Type type) {
            var eType = context.Model.FindEntityType(type);
            var keyProperties = eType.GetKeys().SelectMany(k => k.Properties);
            return keyProperties.Select(p => p.PropertyInfo).ToArray();
        }

        public static PropertyInfo[] GetNonIdMembers(this DbContext context, Type type) {
            var eType = context.Model.FindEntityType(type);
            var keyProperties = eType.GetKeys().SelectMany(k => k.Properties);
            var properties = eType.GetProperties();
            return properties.Except(keyProperties).Select(p => p.PropertyInfo).ToArray();
        }

        public static PropertyInfo[] GetReferenceMembers(this DbContext context, Type type) {
            var eType = context.Model.FindEntityType(type);
            var properties = eType.GetNavigations();
            return properties.Select(p => p.PropertyInfo).ToArray();
        }

        public static PropertyInfo[] GetCollectionMembers(this DbContext context, Type type) {
            var eType = context.Model.FindEntityType(type);
            var properties = eType.GetNavigations();
            var propertyInfos = properties.Select(p => p.PropertyInfo);
            return propertyInfos.Where(p => CollectionUtils.IsCollection(p.PropertyType)).ToArray();
        }

        public static void Invoke(this object onObject, string name, params object[] parms) => onObject.GetType().GetMethod(name)?.Invoke(onObject, parms);
    }
}