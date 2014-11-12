using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Spec;
using NakedObjects.Meta.Utils;

namespace NakedObjects.Meta.Facet {
    [Serializable]
    public class AuthorizationHideForSessionFacet : HideForSessionFacetAbstract {
        private readonly string[] roles;
        private readonly string[] users;

        public AuthorizationHideForSessionFacet(string roles,
            string users,
            ISpecification holder)
            : base(holder) {
            this.roles = FacetUtils.SplitOnComma(roles);
            this.users = FacetUtils.SplitOnComma(users);
        }

        public override string HiddenReason(ISession session, INakedObject target, ILifecycleManager persistor, IMetamodelManager manager) {
            return FacetUtils.IsAllowed(session, roles, users) ? null : "Not authorized to view";
        }
    }
}