// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Meta.Facet;
using NakedObjects.Reflect.Spec;

namespace NakedObjects.Reflect.Authorization {
    public class AuthorizationFacetDecorator : IFacetDecorator {
        private readonly IAuthorizationManager manager;

        public AuthorizationFacetDecorator(IAuthorizationManager manager) {
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
            private readonly AuthorizationFacetDecorator decorator;
            private readonly IIdentifier identifier;

            public SecurityDisableForSessionFacet(IIdentifier identifier,
                                                  AuthorizationFacetDecorator decorator,
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
            private readonly AuthorizationFacetDecorator decorator;
            private readonly IIdentifier identifier;

            public SecurityHideForSessionFacet(IIdentifier identifier,
                                               AuthorizationFacetDecorator decorator,
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