// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using System.Reflection;
using NakedObjects.Resources;
using NakedObjects.UtilInternal;

namespace NakedObjects {
    /// <summary>
    ///     Utility methods for obtaining and making use of domain object keys, whether
    ///     explicitly defined, or inferred by convention.  The Naked Objects framework makes extensive
    ///     use of these utils, but they are provided within the NakedObjects.Helpers
    ///     assembly to permit optional use within domain code.
    /// </summary>
    public static class KeyUtils {
        //public static T FindByKey<T>(this IQueryable<T> source, string keyName, object keyValue) {
        //    string queryString = BuildQueryString(keyName, keyValue);
        //    return source.Where(queryString).SingleOrDefault();
        //}

        //public static T FindByKeys<T>(this IQueryable<T> source, IDictionary<string, object> keys) {
        //    string queryString = BuildQueryString(keys);
        //    return source.Where(queryString).SingleOrDefault();
        //}

        public static T FindByKey<T>(this IDomainObjectContainer container, object keyValue) {
            return (T) ((IInternalAccess) container).FindByKeys(typeof(T), new[] {keyValue});
        }

        public static T FindByKeys<T>(this IDomainObjectContainer container, object[] keys) => (T) ((IInternalAccess) container).FindByKeys(typeof(T), keys);

        public static PropertyInfo[] GetKeys(this IDomainObjectContainer container, Type type) => ((IInternalAccess) container).GetKeys(type).ToArray();

        public static PropertyInfo GetSingleKey(this IDomainObjectContainer container, Type type) {
            var keyProperties = container.GetKeys(type);
            if (keyProperties.Count() > 1) {
                throw new DomainException(string.Format(ProgrammingModel.MultiPartKeyMessage, type));
            }

            if (!keyProperties.Any()) {
                throw new DomainException(string.Format(ProgrammingModel.CannotFindKeyMessage, type));
            }

            return keyProperties.Single();
        }
    }
}