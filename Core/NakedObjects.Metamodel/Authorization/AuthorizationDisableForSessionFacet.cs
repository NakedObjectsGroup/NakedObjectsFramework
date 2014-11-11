using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Spec;
using NakedObjects.Meta.Facet;

namespace NakedObjects.Meta.Authorization {
    [Serializable]
    public class AuthorizationDisableForSessionFacet : DisableForSessionFacetAbstract {
        private readonly IAuthorizationManager authorizationManager;
        private readonly IIdentifier identifier;

        public AuthorizationDisableForSessionFacet(IIdentifier identifier,
            IAuthorizationManager authorizationManager,
            ISpecification holder)
            : base(holder) {
            this.authorizationManager = authorizationManager;
            this.identifier = identifier;
        }

        public override string DisabledReason(ISession session, INakedObject target, ILifecycleManager persistor, IMetamodelManager manager) {
            return authorizationManager.IsEditable(session, persistor, manager, target, identifier)
                ? null
                : "Not authorized to edit";
        }
    }
}