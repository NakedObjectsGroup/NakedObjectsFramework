// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Reflection;
using System.Text;
using NakedObjects.Resources;
using NakedObjects.UtilInternal;

namespace NakedObjects.Util {
    /// <summary>
    /// Utility methods for obtaining and making use of domain object keys, whether
    /// explicitly defined, or inferred by convention.  The Naked Objects framework makes extensive
    /// use of these utils, but they are provided within the NakedObjects.Helpers
    /// assembly to permit optional use within domain code.
    /// </summary>
    public static class KeyUtils {
        public static T FindByKey<T>(this IQueryable<T> source, string keyName, object keyValue) {
            string queryString = BuildQueryString(keyName, keyValue);
            return source.Where(queryString).SingleOrDefault();
        }

        public static T FindByKeys<T>(this IQueryable<T> source, IDictionary<string, object> keys) {
            string queryString = BuildQueryString(keys);
            return source.Where(queryString).SingleOrDefault();
        }

        public static PropertyInfo[] GetKeys(this IDomainObjectContainer container, Type type) {
            return ((IInternalAccess) container).GetKeys(type).ToArray();
        }

        public static PropertyInfo GetSingleKey(this IDomainObjectContainer container, Type type) {
            PropertyInfo[] keyProperties = container.GetKeys(type);
            if (keyProperties.Count() > 1) {
                throw new DomainException(string.Format(ProgrammingModel.MultiPartKeyMessage, type));
            }
            if (keyProperties.Count() < 1) {
                throw new DomainException(string.Format(ProgrammingModel.CannotFindKeyMessage, type));
            }
            return keyProperties.Single();
        }

        private static string BuildQueryString(string keyName, object keyValue) {
            string format = (keyValue.GetType().IsPrimitive ? "{0} == {1}" : "{0} == \"{1}\"");
            return string.Format(format, keyName, keyValue);
        }

        private static string BuildQueryString(IDictionary<string, object> objectKeys) {
            var qs = new StringBuilder();
            for (int i = 0; i < objectKeys.Count(); i++) {
                if (i > 0) {
                    qs.Append(" && ");
                }
                string keyName = objectKeys.Keys.ElementAt(i);
                object keyValue = objectKeys.Values.ElementAt(i);
                qs.Append(BuildQueryString(keyName, keyValue));
            }
            return qs.ToString();
        }
    }
}