// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Framework;
using NakedFramework.Architecture.Spec;
using NakedFramework.Audit;
using NakedFramework.Core.Error;
using NakedFramework.Core.Util;

namespace NakedFramework.Metamodel.Audit {
    [Serializable]
    public sealed class AuditManager : IFacetDecorator, IAuditManager {
        private readonly Type defaultAuditor;
        private readonly ILogger<AuditManager> logger;
        private readonly ImmutableDictionary<string, Type> namespaceAuditors;

        public AuditManager(IAuditConfiguration config, ILogger<AuditManager> logger) {
            this.logger = logger;
            defaultAuditor = config.DefaultAuditor;
            namespaceAuditors = config.NamespaceAuditors.ToImmutableDictionary();
            Validate();
        }

        private void ValidateType(Type toValidate) {
            if (!typeof(IAuditor).IsAssignableFrom(toValidate)) {
                throw new InitialisationException(logger.LogAndReturn($"{toValidate.FullName} is not an IAuditor"));
            }
        }

        private void Validate() {
            ValidateType(defaultAuditor);
            if (namespaceAuditors.Any()) {
                namespaceAuditors.ForEach(kvp => ValidateType(kvp.Value));
            }
        }

        private IAuditor GetAuditor(INakedObjectAdapter nakedObjectAdapter, ILifecycleManager lifecycleManager) => GetNamespaceAuditorFor(nakedObjectAdapter, lifecycleManager) ?? GetDefaultAuditor(lifecycleManager);

        private IAuditor GetNamespaceAuditorFor(INakedObjectAdapter target, ILifecycleManager lifecycleManager) {
            var fullyQualifiedOfTarget = target.Spec.FullName;
            // order here as ImmutableDictionary not ordered
            var auditor = namespaceAuditors.OrderByDescending(x => x.Key.Length).Where(x => fullyQualifiedOfTarget.StartsWith(x.Key)).Select(x => x.Value).FirstOrDefault();

            return auditor != null ? CreateAuditor(auditor, lifecycleManager) : null;
        }

        private static IAuditor CreateAuditor(Type auditor, ILifecycleManager lifecycleManager) => lifecycleManager.CreateNonAdaptedInjectedObject(auditor) as IAuditor;

        private IAuditor GetDefaultAuditor(ILifecycleManager lifecycleManager) => CreateAuditor(defaultAuditor, lifecycleManager);

        #region IAuditManager Members

        public void Invoke(INakedObjectAdapter nakedObjectAdapter, INakedObjectAdapter[] parameters, bool queryOnly, IIdentifier identifier, INakedObjectsFramework framework) {
            var auditor = GetAuditor(nakedObjectAdapter, framework.LifecycleManager);

            var byPrincipal = framework.Session.Principal;
            var memberName = identifier.MemberName;
            if (nakedObjectAdapter.Spec is IServiceSpec) {
                var serviceName = nakedObjectAdapter.Spec.GetTitle(nakedObjectAdapter);
                auditor.ActionInvoked(byPrincipal, memberName, serviceName, queryOnly, parameters.Select(no => no.GetDomainObject()).ToArray());
            }
            else {
                auditor.ActionInvoked(byPrincipal, memberName, nakedObjectAdapter.GetDomainObject(), queryOnly, parameters.Select(no => no.GetDomainObject()).ToArray());
            }
        }

        public void Updated(INakedObjectAdapter nakedObjectAdapter, INakedObjectsFramework framework) {
            var auditor = GetAuditor(nakedObjectAdapter, framework.LifecycleManager);
            auditor.ObjectUpdated(framework.Session.Principal, nakedObjectAdapter.GetDomainObject());
        }

        public void Persisted(INakedObjectAdapter nakedObjectAdapter, INakedObjectsFramework framework) {
            var auditor = GetAuditor(nakedObjectAdapter, framework.LifecycleManager);
            auditor.ObjectPersisted(framework.Session.Principal, nakedObjectAdapter.GetDomainObject());
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
    }
}