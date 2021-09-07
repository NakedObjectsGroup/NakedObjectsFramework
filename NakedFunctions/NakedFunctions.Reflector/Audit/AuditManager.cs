// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Framework;
using NakedFramework.Architecture.Spec;
using NakedFramework.Core.Error;
using NakedFramework.Core.Persist;
using NakedFramework.Core.Util;
using NakedFramework.Metamodel.Audit;
using NakedFunctions.Audit;
using NakedFunctions.Reflector.Component;

namespace NakedFunctions.Reflector.Audit {
    [Serializable]
    public sealed class AuditManager : AbstractAuditManager, IFacetDecorator, IAuditManager {
        private readonly Type MainMenuAuditor;

        public AuditManager(IFunctionalAuditConfiguration config, ILogger<AuditManager> logger) : base(config, logger) {
            MainMenuAuditor = config.MainMenuAuditor;
        }

        protected override void ValidateType(Type toValidate) {
            if (!typeof(ITypeAuditor).IsAssignableFrom(toValidate)) {
                throw new InitialisationException(Logger.LogAndReturn($"{toValidate.FullName} is not an IAuditor"));
            }
        }

        private object GetAuditor(INakedObjectAdapter nakedObjectAdapter, ILifecycleManager lifecycleManager) {
            if (nakedObjectAdapter is null) {
                return CreateAuditor(MainMenuAuditor, lifecycleManager);
            }

            return GetNamespaceAuditorFor(nakedObjectAdapter, lifecycleManager) ?? GetDefaultAuditor(lifecycleManager);
        }

        private object GetNamespaceAuditorFor(INakedObjectAdapter target, ILifecycleManager lifecycleManager) {
            var fullyQualifiedOfTarget = target.Spec.FullName;
            // order here as ImmutableDictionary not ordered
            var auditor = NamespaceAuditors.OrderByDescending(x => x.Key.Length).Where(x => fullyQualifiedOfTarget.StartsWith(x.Key)).Select(x => x.Value).FirstOrDefault();

            return auditor is not null ? CreateAuditor(auditor, lifecycleManager) : null;
        }

        private static object CreateAuditor(Type auditor, ILifecycleManager lifecycleManager) => lifecycleManager.CreateNonAdaptedObject(auditor);

        private object GetDefaultAuditor(ILifecycleManager lifecycleManager) => CreateAuditor(DefaultAuditor, lifecycleManager);

        private static FunctionalContext FunctionalContext(INakedObjectsFramework framework) => new() { Persistor = framework.Persistor, Provider = framework.ServiceProvider };

        #region IAuditManager Members

        private static void PersistResult(ILifecycleManager lifecycleManager, object[] newObjects, object[] deletedObjects, (object proxy, object updated)[] updatedObjects, Func<IDictionary<object, object>, bool> postSaveFunction) =>
            lifecycleManager.Persist(new DetachedObjects(newObjects, deletedObjects, updatedObjects, postSaveFunction));

        private static void HandleContext(FunctionalContext functionalContext, INakedObjectsFramework framework) =>
            PersistResult(framework.LifecycleManager, functionalContext.New, functionalContext.Deleted, functionalContext.Updated, _ => false);

        public void Invoke(INakedObjectAdapter nakedObjectAdapter, INakedObjectAdapter[] parameters, bool queryOnly, IIdentifier identifier, INakedObjectsFramework framework) {
            var auditor = GetAuditor(nakedObjectAdapter, framework.LifecycleManager);

            var memberName = identifier.MemberName;
            IContext context = null;
            if (auditor is IMainMenuAuditor menuAuditor) {
                var menu = identifier.ClassName;
                context = menuAuditor.ActionInvoked(memberName, menu, queryOnly, parameters.Select(no => no.GetDomainObject()).ToArray(), FunctionalContext(framework));
            }
            else if (auditor is ITypeAuditor typeAuditor) {
                context = typeAuditor.ActionInvoked(memberName, nakedObjectAdapter.GetDomainObject(), queryOnly, parameters.Select(no => no.GetDomainObject()).ToArray(), FunctionalContext(framework));
            }

            if (context is FunctionalContext fc) {
                HandleContext(fc, framework);
            }
        }

        public void Updated(INakedObjectAdapter nakedObjectAdapter, INakedObjectsFramework framework) {
            var auditor = GetAuditor(nakedObjectAdapter, framework.LifecycleManager) as ITypeAuditor;
            auditor.ObjectUpdated(nakedObjectAdapter.GetDomainObject(), null);
        }

        public void Persisted(INakedObjectAdapter nakedObjectAdapter, INakedObjectsFramework framework) {
            var auditor = GetAuditor(nakedObjectAdapter, framework.LifecycleManager) as ITypeAuditor;
            auditor.ObjectPersisted(nakedObjectAdapter.GetDomainObject(), null);
        }

        #endregion

        #region IFacetDecorator Members

        public IFacet Decorate(IFacet facet, ISpecification holder) {
            if (facet.FacetType == typeof(IActionInvocationFacet)) {
                return new AuditActionInvocationFacet((IActionInvocationFacet)facet, this);
            }

            if (facet.FacetType == typeof(IUpdatedCallbackFacet)) {
                return new AuditUpdatedFacet((IUpdatedCallbackFacet)facet, this);
            }

            if (facet.FacetType == typeof(IPersistedCallbackFacet)) {
                return new AuditPersistedFacet((IPersistedCallbackFacet)facet, this);
            }

            return facet;
        }

        public Type[] ForFacetTypes { get; } = { typeof(IActionInvocationFacet), typeof(IUpdatedCallbackFacet), typeof(IPersistedCallbackFacet) };

        #endregion
    }
}