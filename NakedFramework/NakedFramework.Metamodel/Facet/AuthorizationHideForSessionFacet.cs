// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Spec;
using NakedObjects.Meta.Utils;

namespace NakedObjects.Meta.Facet {
    [Serializable]
    public sealed class AuthorizationHideForSessionFacet : HideForSessionFacetAbstract {
        private readonly string[] roles;
        private readonly string[] users;

        public AuthorizationHideForSessionFacet(string roles,
                                                string users,
                                                ISpecification holder)
            : base(holder) {
            this.roles = FacetUtils.SplitOnComma(roles);
            this.users = FacetUtils.SplitOnComma(users);
        }

        public override string HiddenReason( INakedObjectAdapter target, INakedObjectsFramework framework) => FacetUtils.IsAllowed(framework.Session, roles, users) ? null : "Not authorized to view";
    }
}