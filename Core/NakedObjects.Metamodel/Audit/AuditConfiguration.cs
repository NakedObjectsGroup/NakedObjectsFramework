// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using NakedObjects.Audit;

namespace NakedObjects.Meta.Audit {
    public class AuditConfiguration : IAuditConfiguration {
        #region IAuditConfiguration Members

        public Type DefaultAuditor { get; set; }
        public Dictionary<string, Type> NamespaceAuditors { get; set; }

        public void SetNameSpaceAuditors(params INamespaceAuditor[] namespaceAuditors) {
            NamespaceAuditors = namespaceAuditors.ToDictionary(na => na.NamespaceToAudit, na => na.GetType());
        }

        #endregion
    }
}