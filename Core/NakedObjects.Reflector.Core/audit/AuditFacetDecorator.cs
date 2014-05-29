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
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Context;
using NakedObjects.Reflector.Spec;

namespace NakedObjects.Reflector.Audit {
    public class AuditFacetDecorator : IFacetDecorator {
        private readonly AuditManager manager;

        public AuditFacetDecorator(AuditManager manager) {
            this.manager = manager;
        }

        #region IFacetDecorator Members

        public virtual IFacet Decorate(IFacet facet, IFacetHolder holder) {
            if (facet.FacetType == typeof (IActionInvocationFacet)) {
                return new AuditActionInvocationFacet((IActionInvocationFacet) facet, manager);
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
            private readonly AuditManager manager;
            private readonly IActionInvocationFacet underlyingFacet;
            private bool? isQueryOnly;

            public AuditActionInvocationFacet(IActionInvocationFacet underlyingFacet, AuditManager auditManager)
                : base(underlyingFacet.FacetHolder) {
                this.underlyingFacet = underlyingFacet;
                manager = auditManager;
                identifier = underlyingFacet.FacetHolder.Identifier;
            }

            private bool IsQueryOnly {
                get {
                    if (!isQueryOnly.HasValue) {
                        INakedObjectAction action = NakedObjectsContext.Reflector.LoadSpecification(identifier.ClassName).GetActionLeafNodes().FirstOrDefault(a => a.Id == identifier.MemberName);
                        isQueryOnly = action.ReturnType.IsQueryable || action.ContainsFacet<IQueryOnlyFacet>();
                    }
                    return isQueryOnly.Value;
                }
            }

            public override INakedObjectSpecification ReturnType {
                get { return underlyingFacet.ReturnType; }
            }

            public override INakedObjectSpecification OnType {
                get { return underlyingFacet.OnType; }
            }

            public override bool GetIsRemoting(INakedObject target) {
                return underlyingFacet.GetIsRemoting(target);
            }

            public override INakedObject Invoke(INakedObject nakedObject, INakedObject[] parameters) {
                manager.Invoke(nakedObject, parameters, IsQueryOnly, identifier);
                return underlyingFacet.Invoke(nakedObject, parameters);
            }

            public override INakedObject Invoke(INakedObject nakedObject, INakedObject[] parameters, int resultPage) {
                manager.Invoke(nakedObject, parameters, IsQueryOnly, identifier);
                return underlyingFacet.Invoke(nakedObject, parameters, resultPage);
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

            public override void Invoke(INakedObject nakedObject) {
                manager.Updated(nakedObject);
                underlyingFacet.Invoke(nakedObject);
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

            public override void Invoke(INakedObject nakedObject) {
                manager.Persisted(nakedObject);
                underlyingFacet.Invoke(nakedObject);
            }
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}