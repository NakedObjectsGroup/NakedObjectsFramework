using System;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace NakedFramework.Persistor.EFCore.Util {
    public static class EFCoreHelpers {
        public static PropertyInfo[] GetNonIdMembers(this DbContext context, Type type) {
            var eType = context.Model.FindEntityType(type);
            var keyProperties = eType.GetKeys().SelectMany(k => k.Properties);
            var properties = eType.GetProperties();

            //return properties.Except(keyProperties).ToArray();
            return Array.Empty<PropertyInfo>();
        }

        public static PropertyInfo[] GetReferenceMembers(this DbContext context, Type type) => Array.Empty<PropertyInfo>();

        public static PropertyInfo[] GetCollectionMembers(this DbContext context, Type type) => Array.Empty<PropertyInfo>();

        public static void Invoke(this object onObject, string name, params object[] parms) => onObject.GetType().GetMethod(name)?.Invoke(onObject, parms);
    }
}