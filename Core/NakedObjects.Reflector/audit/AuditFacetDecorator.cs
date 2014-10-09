// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Linq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Actions.Invoke;
using NakedObjects.Architecture.Facets.Actions.Potency;
using NakedObjects.Architecture.Facets.Objects.Callbacks;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Security;
using NakedObjects.Architecture.Spec;
using NakedObjects.Reflector.Spec;

namespace NakedObjects.Reflector.Audit {
    public class AuditFacetDecorator : IFacetDecorator {
        private readonly AuditManager manager;
        private readonly IMetamodel metamodel;

        public AuditFacetDecorator(AuditManager manager, IMetamodel metamodel) {
            this.manager = manager;
            this.metamodel = metamodel;
        }

        #region IFacetDecorator Members

        public virtual IFacet Decorate(IFacet facet, IFacetHolder holder) {
            if (facet.FacetType == typeof (IActionInvocationFacet)) {
                return new AuditActionInvocationFacet((IActionInvocationFacet) facet, manager, metamodel);
            }

            if (facet.FacetType == typeof (IUpdatedCallbackFacet)) {
                return new AuditUpdatedFacet((IUpdatedCallbackFacet) facet, manager);
            }

            if (facet.FacetType == typeof (IPersistedCallbackFacet)) {
                return new AuditPersistedFacet((IPersistedCallbackFacet) facet, manager);
            }

            return facet;
        }

        public virtual Type[] ForFacetTypes {
            get { return new[] {typeof (IActionInvocationFacet), typeof (IUpdatedCallbackFacet), typeof (IPersistedCallbackFacet)}; }
        }

        #endregion

        #region Nested type: AuditActionInvocationFacet

        private class AuditActionInvocationFacet : ActionInvocationFacetAbstract {
            private readonly IIdentifier identifier;
            private readonly AuditManager auditManager;
            private readonly IMetamodel metamodel;
            private readonly IActionInvocationFacet underlyingFacet;
            private bool? isQueryOnly;

            public AuditActionInvocationFacet(IActionInvocationFacet underlyingFacet, AuditManager auditManager, IMetamodel metamodel)
                : base(underlyingFacet.FacetHolder) {
                this.underlyingFacet = underlyingFacet;
                this.auditManager = auditManager;
                this.metamodel = metamodel;
                identifier = underlyingFacet.FacetHolder.Identifier;
            }



            private bool IsQueryOnly {
                get {
                    if (!isQueryOnly.HasValue) {
                        //INakedObjectAction action = metamodel.GetSpecification(identifier.ClassName).GetActionLeafNodes().FirstOrDefault(a => a.Id == identifier.MemberName);
                        //isQueryOnly = action.ReturnType.IsQueryable || action.ContainsFacet<IQueryOnlyFacet>();
                        throw new NotImplementedException();
                    }
                    return isQueryOnly.Value;
                }
            }

            public override IIntrospectableSpecification ReturnType {
                get { return underlyingFacet.ReturnType; }
            }

            public override IIntrospectableSpecification OnType {
                get { return underlyingFacet.OnType; }
            }

            public override bool GetIsRemoting(INakedObject target) {
                return underlyingFacet.GetIsRemoting(target);
            }

            public override INakedObject Invoke(INakedObject nakedObject, INakedObject[] parameters, ILifecycleManager persistor, ISession session) {
                auditManager.Invoke(nakedObject, parameters, IsQueryOnly, identifier, session);
                return underlyingFacet.Invoke(nakedObject, parameters, persistor, session);
            }

            public override INakedObject Invoke(INakedObject nakedObject, INakedObject[] parameters, int resultPage, ILifecycleManager persistor, ISession session) {
                auditManager.Invoke(nakedObject, parameters, IsQueryOnly, identifier, session);
                return underlyingFacet.Invoke(nakedObject, parameters, resultPage, persistor, session);
            }
        }

        #endregion

        #region Nested type: AuditUpdatedFacet

        private class AuditUpdatedFacet : UpdatedCallbackFacetAbstract {
            private readonly AuditManager manager;
            private readonly IUpdatedCallbackFacet underlyingFacet;

            public AuditUpdatedFacet(IUpdatedCallbackFacet underlyingFacet, AuditManager auditManager)
                : base(underlyingFacet.FacetHolder) {
                this.underlyingFacet = underlyingFacet;
                manager = auditManager;
            }

            public override void Invoke(INakedObject nakedObject, ISession session) {
                manager.Updated(nakedObject, session);
                underlyingFacet.Invoke(nakedObject, session);
            }
        }

        #endregion

        #region Nested type: AuditPersistedFacet

        private class AuditPersistedFacet : PersistedCallbackFacetAbstract {
            private readonly AuditManager manager;
            private readonly IPersistedCallbackFacet underlyingFacet;

            public AuditPersistedFacet(IPersistedCallbackFacet underlyingFacet, AuditManager auditManager)
                : base(underlyingFacet.FacetHolder) {
                this.underlyingFacet = underlyingFacet;
                manager = auditManager;
            }

            public override void Invoke(INakedObject nakedObject, ISession session) {
                manager.Persisted(nakedObject, session);
                underlyingFacet.Invoke(nakedObject, session);
            }
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}