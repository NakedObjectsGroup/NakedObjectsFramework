// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using Common.Logging;
using NakedObjects.Facade;
using NakedObjects.Rest.Snapshot.Utility;

namespace NakedObjects.Rest.Model {
    public class ReferenceValue : IValue {
        private static readonly ILog Logger = LogManager.GetLogger<ReferenceValue>();
        private readonly string internalValue;

        public ReferenceValue(object value, string name) {
            internalValue = value as string;

            if (string.IsNullOrWhiteSpace(internalValue)) {
                Logger.ErrorFormat("Malformed json name: {0} arguments: href = null or empty", name);
                throw new ArgumentException("malformed arguments");
            }
        }

        #region IValue Members

        public object GetValue(IFrameworkFacade facade, UriMtHelper helper, IOidStrategy oidStrategy) {
            return GetObjectByHref(internalValue, facade, helper, oidStrategy);
        }

        #endregion

        private object GetObjectByHref(string href, IFrameworkFacade facade, UriMtHelper helper, IOidStrategy oidStrategy) {
            string[] oids = helper.GetObjectId(href);
            if (oids != null) {
                var oid = facade.OidTranslator.GetOidTranslation(oids[0] + "/" + oids[1]);
                return facade.GetObject(oid).Target?.Object;
            }

            string typeName = helper.GetTypeId(href);
            return facade.GetDomainType(typeName);
        }
    }
}