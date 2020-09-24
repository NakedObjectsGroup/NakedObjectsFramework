// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Reflection;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
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

        public override INakedObjectAdapter Invoke(INakedObjectAdapter nakedObjectAdapter, INakedObjectAdapter[] parameters, ILifecycleManager lifecycleManager, IMetamodelManager manager, ISession session, INakedObjectManager nakedObjectManager, IMessageBroker messageBroker, ITransactionManager transactionManager, IServicesManager servicesManager) {
            auditManager.Invoke(nakedObjectAdapter, parameters, IsQueryOnly, identifier, session, lifecycleManager);
            return underlyingFacet.Invoke(nakedObjectAdapter, parameters, lifecycleManager, manager, session, nakedObjectManager, messageBroker, transactionManager, servicesManager);
        }

        public override INakedObjectAdapter Invoke(INakedObjectAdapter nakedObjectAdapter, INakedObjectAdapter[] parameters, int resultPage, ILifecycleManager lifecycleManager, IMetamodelManager manager, ISession session, INakedObjectManager nakedObjectManager, IMessageBroker messageBroker, ITransactionManager transactionManager, IServicesManager servicesManager) {
            auditManager.Invoke(nakedObjectAdapter, parameters, IsQueryOnly, identifier, session, lifecycleManager);
            return underlyingFacet.Invoke(nakedObjectAdapter, parameters, resultPage, lifecycleManager, manager, session, nakedObjectManager, messageBroker, transactionManager, servicesManager);
        }
    }
}