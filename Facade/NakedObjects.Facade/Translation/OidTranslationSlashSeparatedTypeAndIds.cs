// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Linq;

namespace NakedObjects.Facade.Translation {
    public class OidTranslationSlashSeparatedTypeAndIds : IOidTranslation {
        static OidTranslationSlashSeparatedTypeAndIds() {
            // default 
            KeySeparator = "--";
        }

        // when using this ctor be aware of encoded values that might include a "/"
        public OidTranslationSlashSeparatedTypeAndIds(string id) {
            var split = id.Split('/');
            DomainType = split.First();
            InstanceId = split.Skip(1).FirstOrDefault();
        }

        public OidTranslationSlashSeparatedTypeAndIds(string domainType, string instanceId) {
            DomainType = domainType;
            InstanceId = instanceId;
        }

        public static string KeySeparator { get; private set; }

        #region IOidTranslation Members

        public string DomainType { get; set; }
        public string InstanceId { get; set; }

        public IOidFacade GetOid(IOidStrategy oidStrategy) {
            return oidStrategy.RestoreOid(this);
        }

        public IOidFacade GetSid(IOidStrategy oidStrategy) {
            return oidStrategy.RestoreSid(this);
        }

        public string Encode() {
            return DomainType + (string.IsNullOrEmpty(InstanceId) ? "" : "/" + InstanceId);
        }

        #endregion

        public override string ToString() {
            return DomainType + (string.IsNullOrEmpty(InstanceId) ? "" : "-" + InstanceId);
        }
    }
}