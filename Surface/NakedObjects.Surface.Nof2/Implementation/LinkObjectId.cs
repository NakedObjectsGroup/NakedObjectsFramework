// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;

namespace NakedObjects.Surface.Nof2.Implementation {
    public class LinkObjectId : ILinkObjectId {
        static LinkObjectId() {
            // default 
            KeySeparator = "-";
        }

        public LinkObjectId(string domainType, string instanceId) {
            DomainType = domainType;
            InstanceId = instanceId;
        }

        public static string KeySeparator { get; set; }

        public string DomainType { get; set; }
        public string InstanceId { get; set; }

        public override string ToString() {
            return DomainType + (String.IsNullOrEmpty(InstanceId) ? "" : KeySeparator + InstanceId);
        }
    }
}