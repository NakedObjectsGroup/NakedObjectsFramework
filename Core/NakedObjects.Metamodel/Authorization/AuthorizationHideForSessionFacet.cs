using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Spec;
using NakedObjects.Meta.Facet;

namespace NakedObjects.Meta.Authorization {

    [Serializable]
    public class AuthorizationHideForSessionFacet : HideForSessionFacetAbstract {
        private readonly IAuthorizationManager authorizationManager;
        private readonly IIdentifier identifier;

        public AuthorizationHideForSessionFacet(IIdentifier identifier,
            IAuthorizationManager authorizationManager,
            ISpecification holder)
            : base(holder) {
            this.identifier = identifier;
            this.authorizationManager = authorizationManager;
        }

        public override string HiddenReason(ISession session, INakedObject target, ILifecycleManager persistor, IMetamodelManager manager) {
            return authorizationManager.IsVisible(session, persistor, manager, target, identifier)
                ? null
                : "Not authorized to view";
        }
    }
}