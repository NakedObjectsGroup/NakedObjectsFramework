// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Framework;
using NakedFramework.Architecture.Spec;
using NakedFramework.Metamodel.Utils;

namespace NakedFramework.Metamodel.Facet; 

[Serializable]
public sealed class AuthorizationDisableForSessionFacet : DisableForSessionFacetAbstract {
    private readonly string[] roles;
    private readonly string[] users;

    public AuthorizationDisableForSessionFacet(string roles,
                                               string users,
                                               ISpecification holder)
        : base(holder) {
        this.roles = FacetUtils.SplitOnComma(roles);
        this.users = FacetUtils.SplitOnComma(users);
    }

    public override string DisabledReason(INakedObjectAdapter target, INakedFramework framework) => FacetUtils.IsAllowed(framework.Session, roles, users) ? null : "Not authorized to edit";
}