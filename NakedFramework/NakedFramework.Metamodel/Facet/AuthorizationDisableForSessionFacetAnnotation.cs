// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Framework;
using NakedFramework.Metamodel.Utils;

namespace NakedFramework.Metamodel.Facet;

[Serializable]
public sealed class AuthorizationDisableForSessionFacetAnnotation : DisableForSessionFacetAbstract {
    public AuthorizationDisableForSessionFacetAnnotation(string roles,
                                                         string users) {
        Roles = FacetUtils.SplitOnComma(roles);
        Users = FacetUtils.SplitOnComma(users);
    }

    public string[] Roles { get; }
    public string[] Users { get; }

    public override string DisabledReason(INakedObjectAdapter target, INakedFramework framework) => FacetUtils.IsAllowed(framework.Session, Roles, Users) ? null : "Not authorized to edit";
}