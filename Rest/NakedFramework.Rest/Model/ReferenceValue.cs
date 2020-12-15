// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects.Facade;
using NakedObjects.Rest.Snapshot.Utility;

namespace NakedObjects.Rest.Model {
    public class ReferenceValue : IValue {
        private readonly string internalValue;

        public ReferenceValue(object value, string name) {
            internalValue = value as string;
            Validate(internalValue, name);
        }

        #region IValue Members

        public object GetValue(IFrameworkFacade facade, UriMtHelper helper, IOidStrategy oidStrategy) => GetObjectByHref(internalValue, facade, helper, oidStrategy);

        #endregion

        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
        private static void Validate(string internalValue, string name) {
            if (string.IsNullOrWhiteSpace(internalValue)) {
                var msg = $"Malformed json name: {name} arguments: href = null or empty";
                throw new ArgumentException($"malformed arguments : {msg}");
            }
        }

        private static object GetObjectByHref(string href, IFrameworkFacade facade, UriMtHelper helper, IOidStrategy oidStrategy) {
            var oids = UriMtHelper.GetObjectId(href);
            if (oids != null) {
                var oid = facade.OidTranslator.GetOidTranslation($"{oids.Value.type}/{oids.Value.key}");
                return facade.GetObject(oid).Target?.Object;
            }

            var typeName = UriMtHelper.GetTypeId(href);
            return facade.GetDomainType(typeName);
        }
    }
}