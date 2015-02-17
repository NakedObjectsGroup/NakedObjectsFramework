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
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Audit;
using NakedObjects.Core;
using NakedObjects.Core.Util;

namespace NakedObjects.Meta.Audit {
    [Serializable]
    public class AuditManager : IFacetDecorator, IAuditManager {
        private readonly Type defaultAuditor;
        private readonly Type[] forFacetTypes = {typeof (IActionInvocationFacet), typeof (IUpdatedCallbackFacet), typeof (IPersistedCallbackFacet)};
        private readonly ImmutableDictionary<string, Type> namespaceAuditors;

        public AuditManager(IAuditConfiguration config) {
            defaultAuditor = config.DefaultAuditor;
            namespaceAuditors = config.NamespaceAuditors.ToImmutableDictionary();

            Validate();
        }

        // validate all the passed in types to fail at reflection time as far as possible

        #region IAuditManager Members

        public void Invoke(INakedObject nakedObject, INakedObject[] parameters, bool queryOnly, IIdentifier identifier, ISession session, ILifecycleManager lifecycleManager, IMetamodelManager manager) {
            IAuditor auditor = GetAuditor(nakedObject, lifecycleManager, manager);

            IPrincipal byPrincipal = session.Principal;
            string memberName = identifier.MemberName;
            if (nakedObject.Spec is IServiceSpec) {
                string serviceName = nakedObject.Spec.GetTitle(nakedObject);
                auditor.ActionInvoked(byPrincipal, memberName, serviceName, queryOnly, parameters.Select(no => no.GetDomainObject()).ToArray());
            }
            else {
                auditor.ActionInvoked(byPrincipal, memberName, nakedObject.GetDomainObject(), queryOnly, parameters.Select(no => no.GetDomainObject()).ToArray());
            }
        }

        public void Updated(INakedObject nakedObject, ISession session, ILifecycleManager lifecycleManager, IMetamodelManager manager) {
            IAuditor auditor = GetAuditor(nakedObject, lifecycleManager, manager);
            auditor.ObjectUpdated(session.Principal, nakedObject.GetDomainObject());
        }

        public void Persisted(INakedObject nakedObject, ISession session, ILifecycleManager lifecycleManager, IMetamodelManager manager) {
            IAuditor auditor = GetAuditor(nakedObject, lifecycleManager, manager);
            auditor.ObjectPersisted(session.Principal, nakedObject.GetDomainObject());
        }

        #endregion

        #region IFacetDecorator Members

        public virtual IFacet Decorate(IFacet facet, ISpecification holder) {
            if (facet.FacetType == typeof (IActionInvocationFacet)) {
                return new AuditActionInvocationFacet((IActionInvocationFacet) facet, this);
            }

            if (facet.FacetType == typeof (IUpdatedCallbackFacet)) {
                return new AuditUpdatedFacet((IUpdatedCallbackFacet) facet, this);
            }

            if (facet.FacetType == typeof (IPersistedCallbackFacet)) {
                return new AuditPersistedFacet((IPersistedCallbackFacet) facet, this);
            }

            return facet;
        }

        public virtual Type[] ForFacetTypes {
            get { return forFacetTypes; }
        }

        #endregion

        private void ValidateType(Type toValidate) {
            if (!typeof (IAuditor).IsAssignableFrom(toValidate)) {
                throw new InitialisationException(toValidate.FullName + " is not an IAuditor");
            }

        }

        private void Validate() {
            ValidateType(defaultAuditor);
            if (namespaceAuditors.Any()) {
                namespaceAuditors.ForEach(kvp => ValidateType(kvp.Value));
            }
        }

        private IAuditor GetAuditor(INakedObject nakedObject, ILifecycleManager lifecycleManager, IMetamodelManager manager) {
            return GetNamespaceAuditorFor(nakedObject, lifecycleManager, manager) ?? GetDefaultAuditor(lifecycleManager, manager);
        }

        private IAuditor GetNamespaceAuditorFor(INakedObject target, ILifecycleManager lifecycleManager, IMetamodelManager manager) {
            Assert.AssertNotNull(target);
            string fullyQualifiedOfTarget = target.Spec.FullName;
            Type auditor = namespaceAuditors.
                Where(x => fullyQualifiedOfTarget.StartsWith(x.Key)).
                OrderByDescending(x => x.Key.Length).
                Select(x => x.Value).
                FirstOrDefault();

            return auditor != null ? CreateAuditor(auditor, lifecycleManager, manager) : null;
        }

        private IAuditor CreateAuditor(Type auditor, ILifecycleManager lifecycleManager, IMetamodelManager manager) {
            return lifecycleManager.CreateInstance((IObjectSpec) manager.GetSpecification(auditor)).GetDomainObject<IAuditor>();
        }

        private IAuditor GetDefaultAuditor(ILifecycleManager lifecycleManager, IMetamodelManager manager) {
            return CreateAuditor(defaultAuditor, lifecycleManager, manager);
        }
    }
}