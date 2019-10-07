// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Principal;
using Common.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Audit;
using NakedObjects.Core;
using NakedObjects.Core.Util;

namespace NakedObjects.Meta.Audit {
    [Serializable]
    public sealed class AuditManager : IFacetDecorator, IAuditManager {
        private static readonly ILog Log = LogManager.GetLogger(typeof(AuditManager));

        private readonly Type defaultAuditor;
        private readonly ImmutableDictionary<string, Type> namespaceAuditors;

        public AuditManager(IAuditConfiguration config) {
            defaultAuditor = config.DefaultAuditor;
            namespaceAuditors = config.NamespaceAuditors.OrderByDescending(x => x.Key.Length).ToImmutableDictionary();
            Validate();
        }

        #region IAuditManager Members

        public void Invoke(INakedObjectAdapter nakedObjectAdapter, INakedObjectAdapter[] parameters, bool queryOnly, IIdentifier identifier, ISession session, ILifecycleManager lifecycleManager) {
            IAuditor auditor = GetAuditor(nakedObjectAdapter, lifecycleManager);

            IPrincipal byPrincipal = session.Principal;
            string memberName = identifier.MemberName;
            if (nakedObjectAdapter.Spec is IServiceSpec) {
                string serviceName = nakedObjectAdapter.Spec.GetTitle(nakedObjectAdapter);
                auditor.ActionInvoked(byPrincipal, memberName, serviceName, queryOnly, parameters.Select(no => no.GetDomainObject()).ToArray());
            }
            else {
                auditor.ActionInvoked(byPrincipal, memberName, nakedObjectAdapter.GetDomainObject(), queryOnly, parameters.Select(no => no.GetDomainObject()).ToArray());
            }
        }

        public void Updated(INakedObjectAdapter nakedObjectAdapter, ISession session, ILifecycleManager lifecycleManager) {
            IAuditor auditor = GetAuditor(nakedObjectAdapter, lifecycleManager);
            auditor.ObjectUpdated(session.Principal, nakedObjectAdapter.GetDomainObject());
        }

        public void Persisted(INakedObjectAdapter nakedObjectAdapter, ISession session, ILifecycleManager lifecycleManager) {
            IAuditor auditor = GetAuditor(nakedObjectAdapter, lifecycleManager);
            auditor.ObjectPersisted(session.Principal, nakedObjectAdapter.GetDomainObject());
        }

        #endregion

        #region IFacetDecorator Members

        public IFacet Decorate(IFacet facet, ISpecification holder) {
            if (facet.FacetType == typeof(IActionInvocationFacet)) {
                return new AuditActionInvocationFacet((IActionInvocationFacet) facet, this);
            }

            if (facet.FacetType == typeof(IUpdatedCallbackFacet)) {
                return new AuditUpdatedFacet((IUpdatedCallbackFacet) facet, this);
            }

            if (facet.FacetType == typeof(IPersistedCallbackFacet)) {
                return new AuditPersistedFacet((IPersistedCallbackFacet) facet, this);
            }

            return facet;
        }

        public Type[] ForFacetTypes { get; } = {typeof(IActionInvocationFacet), typeof(IUpdatedCallbackFacet), typeof(IPersistedCallbackFacet)};

        #endregion

        private void ValidateType(Type toValidate) {
            if (!typeof(IAuditor).IsAssignableFrom(toValidate)) {
                throw new InitialisationException(Log.LogAndReturn($"{toValidate.FullName} is not an IAuditor"));
            }
        }

        private void Validate() {
            ValidateType(defaultAuditor);
            if (namespaceAuditors.Any()) {
                namespaceAuditors.ForEach(kvp => ValidateType(kvp.Value));
            }
        }

        private IAuditor GetAuditor(INakedObjectAdapter nakedObjectAdapter, ILifecycleManager lifecycleManager) {
            return GetNamespaceAuditorFor(nakedObjectAdapter, lifecycleManager) ?? GetDefaultAuditor(lifecycleManager);
        }

        private IAuditor GetNamespaceAuditorFor(INakedObjectAdapter target, ILifecycleManager lifecycleManager) {
            Assert.AssertNotNull(target);
            string fullyQualifiedOfTarget = target.Spec.FullName;

            // already ordered OrderByDescending(x => x.Key.Length).
            Type auditor = namespaceAuditors.Where(x => fullyQualifiedOfTarget.StartsWith(x.Key)).Select(x => x.Value).FirstOrDefault();

            return auditor != null ? CreateAuditor(auditor, lifecycleManager) : null;
        }

        private IAuditor CreateAuditor(Type auditor, ILifecycleManager lifecycleManager) {
            return lifecycleManager.CreateNonAdaptedInjectedObject(auditor) as IAuditor;
        }

        private IAuditor GetDefaultAuditor(ILifecycleManager lifecycleManager) {
            return CreateAuditor(defaultAuditor, lifecycleManager);
        }
    }
}