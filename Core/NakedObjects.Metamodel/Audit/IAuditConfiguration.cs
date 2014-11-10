// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using NakedObjects.Audit;

namespace NakedObjects.Meta.Audit {
    public interface IAuditConfiguration {
        Type DefaultAuditor { get; set; }

        // either set the NamespaceAuditors directly with the property or use teh old method and pass 
        // in a list of auditor objects that each return their audited namespace. List of objects takes priority.
        Dictionary<string, Type> NamespaceAuditors { get; set; }

        [Obsolete]
        void SetNameSpaceAuditors(params INamespaceAuditor[] namespaceAuditors);
    }
}