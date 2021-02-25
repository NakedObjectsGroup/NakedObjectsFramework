// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Reflection;
using NakedFramework.Architecture.Adapter;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Meta.Facet;

namespace NakedObjects.Meta.Audit {
    [Serializable]
    public sealed class AuditActionInvocationFacet : ActionInvocationFacetAbstract {
        private readonly IAuditManager auditManager;
        private readonly IIdentifier identifier;
        private readonly IActionInvocationFacet underlyingFacet;

        public AuditActionInvocationFacet(IActionInvocationFacet underlyingFacet, IAuditManager auditManager)
            : base(underlyingFacet.Specification) {
            this.underlyingFacet = underlyingFacet;
            this.auditManager = auditManager;
            identifier = underlyingFacet.Specification.Identifier;
        }

        public override bool IsQueryOnly => underlyingFacet.IsQueryOnly;

        public override MethodInfo ActionMethod => underlyingFacet.ActionMethod;

        public override IObjectSpecImmutable ReturnType => underlyingFacet.ReturnType;

        public override IObjectSpecImmutable ElementType => underlyingFacet.ElementType;

        public override ITypeSpecImmutable OnType => underlyingFacet.OnType;

        public override INakedObjectAdapter Invoke(INakedObjectAdapter nakedObjectAdapter, INakedObjectAdapter[] parameters, INakedObjectsFramework framework) {
            auditManager.Invoke(nakedObjectAdapter, parameters, IsQueryOnly, identifier, framework);
            return underlyingFacet.Invoke(nakedObjectAdapter, parameters, framework);
        }

        public override INakedObjectAdapter Invoke(INakedObjectAdapter nakedObjectAdapter, INakedObjectAdapter[] parameters, int resultPage, INakedObjectsFramework framework) {
            auditManager.Invoke(nakedObjectAdapter, parameters, IsQueryOnly, identifier, framework);
            return underlyingFacet.Invoke(nakedObjectAdapter, parameters, resultPage, framework);
        }
    }
}