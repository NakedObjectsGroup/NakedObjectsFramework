// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Disable;
using NakedObjects.Architecture.Facets.Hide;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Security;
using NakedObjects.Architecture.Spec;
using NakedObjects.Reflector.Spec;

namespace NakedObjects.Reflector.Security {
    public class SecurityFacetDecorator : IFacetDecorator {
        private readonly IAuthorizationManager manager;

        public SecurityFacetDecorator(IAuthorizationManager manager) {
            this.manager = manager;
        }

        #region IFacetDecorator Members

        public virtual IFacet Decorate(IFacet facet, ISpecification holder) {
            Type facetType = facet.FacetType;
            ISpecification specification = facet.Specification;
            IIdentifier identifier = holder.Identifier;

            if (facetType == typeof (IHideForSessionFacet)) {
                return new SecurityHideForSessionFacet(identifier, this, specification);
            }

            if (facetType == typeof (IDisableForSessionFacet)) {
                return new SecurityDisableForSessionFacet(identifier, this, specification);
            }
            return facet;
        }

        public virtual Type[] ForFacetTypes {
            get { return new[] {typeof (IHideForSessionFacet), typeof (IDisableForSessionFacet)}; }
        }

        #endregion

        #region Nested type: SecurityDisableForSessionFacet

        private class SecurityDisableForSessionFacet : DisableForSessionFacetAbstract {
            private readonly SecurityFacetDecorator decorator;
            private readonly IIdentifier identifier;

            public SecurityDisableForSessionFacet(IIdentifier identifier,
                                                  SecurityFacetDecorator decorator,
                                                  ISpecification holder)
                : base(holder) {
                this.identifier = identifier;
                this.decorator = decorator;
            }

            public override string DisabledReason(ISession session, INakedObject target, ILifecycleManager persistor) {
                return decorator.manager.IsEditable(session, persistor, target, identifier) ? null : "Not authorized to edit";
            }
        }

        #endregion

        #region Nested type: SecurityHideForSessionFacet

        private class SecurityHideForSessionFacet : HideForSessionFacetAbstract {
            private readonly SecurityFacetDecorator decorator;
            private readonly IIdentifier identifier;

            public SecurityHideForSessionFacet(IIdentifier identifier,
                                               SecurityFacetDecorator decorator,
                                               ISpecification holder)
                : base(holder) {
                this.identifier = identifier;
                this.decorator = decorator;
            }

            public override string HiddenReason(ISession session, INakedObject target, ILifecycleManager persistor) {
                return decorator.manager.IsVisible(session, persistor, target, identifier) ? null : "Not authorized to view";
            }
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}