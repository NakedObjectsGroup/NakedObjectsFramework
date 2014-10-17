// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Metamodel.Facet;
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

        public virtual IFacet Decorate(IFacet facet, ISpecification holder) {
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
                : base(underlyingFacet.Specification) {
                this.underlyingFacet = underlyingFacet;
                this.auditManager = auditManager;
                this.metamodel = metamodel;
                identifier = underlyingFacet.Specification.Identifier;
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

            public override IObjectSpecImmutable ReturnType {
                get { return underlyingFacet.ReturnType; }
            }

            public override IObjectSpecImmutable OnType {
                get { return underlyingFacet.OnType; }
            }

            public override bool GetIsRemoting(INakedObject target) {
                return underlyingFacet.GetIsRemoting(target);
            }

            public override INakedObject Invoke(INakedObject nakedObject, INakedObject[] parameters, INakedObjectManager manager, ISession session, ITransactionManager transactionManager) {
                auditManager.Invoke(nakedObject, parameters, IsQueryOnly, identifier, session);
                return underlyingFacet.Invoke(nakedObject, parameters, manager, session, transactionManager);
            }

            public override INakedObject Invoke(INakedObject nakedObject, INakedObject[] parameters, int resultPage, INakedObjectManager manager, ISession session, ITransactionManager transactionManager) {
                auditManager.Invoke(nakedObject, parameters, IsQueryOnly, identifier, session);
                return underlyingFacet.Invoke(nakedObject, parameters, resultPage, manager, session, transactionManager);
            }
        }

        #endregion

        #region Nested type: AuditUpdatedFacet

        private class AuditUpdatedFacet : UpdatedCallbackFacetAbstract {
            private readonly AuditManager manager;
            private readonly IUpdatedCallbackFacet underlyingFacet;

            public AuditUpdatedFacet(IUpdatedCallbackFacet underlyingFacet, AuditManager auditManager)
                : base(underlyingFacet.Specification) {
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
                : base(underlyingFacet.Specification) {
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