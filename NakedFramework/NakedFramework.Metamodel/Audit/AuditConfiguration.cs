// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using NakedObjects.Audit;

namespace NakedFramework.Metamodel.Audit {
    //Add namespace auditors individually via AddNamespaceAuditor, or create the whole dictionary
    //and set the NamespaceAuditors property.
    public class AuditConfiguration<TDefault> : IAuditConfiguration where TDefault : IAuditor {
        public AuditConfiguration() {
            DefaultAuditor = typeof(TDefault);
            NamespaceAuditors = new Dictionary<string, Type>();
        }

        #region IAuditConfiguration Members

        public Type DefaultAuditor { get; }
        public Dictionary<string, Type> NamespaceAuditors { get; }

        public void AddNamespaceAuditor<T>(string namespaceCovered) where T : IAuditor => NamespaceAuditors.Add(namespaceCovered, typeof(T));

        #endregion
    }
}