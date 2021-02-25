// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedFramework.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Meta.Facet;

namespace NakedObjects.Meta.Audit {
    [Serializable]
    public sealed class AuditUpdatedFacet : UpdatedCallbackFacetAbstract {
        private readonly IAuditManager manager;
        private readonly IUpdatedCallbackFacet underlyingFacet;

        public AuditUpdatedFacet(IUpdatedCallbackFacet underlyingFacet, IAuditManager auditManager)
            : base(underlyingFacet.Specification) {
            this.underlyingFacet = underlyingFacet;
            manager = auditManager;
        }

        public override void Invoke(INakedObjectAdapter nakedObjectAdapter, INakedObjectsFramework framework) {
            manager.Updated(nakedObjectAdapter, framework);
            underlyingFacet.Invoke(nakedObjectAdapter, framework);
        }
    }
}