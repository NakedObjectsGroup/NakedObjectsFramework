// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Interactions;
using NakedFramework.Core.Error;

namespace NakedFramework.Metamodel.Facet;

[Serializable]
public sealed class BoundedFacet : FacetAbstract, IBoundedFacet, IMarkerFacet {
    private BoundedFacet() { }

    public static BoundedFacet Instance { get; } = new();

    public override Type FacetType => typeof(IBoundedFacet);

    private static string DisabledReason(INakedObjectAdapter inObjectAdapter) => NakedObjects.Resources.NakedObjects.Bounded;

    #region IBoundedFacet Members

    public string Disables(IInteractionContext ic) =>
        ic.TypeEquals(InteractionType.ObjectPersist)
            ? DisabledReason(ic.Target)
            : null;

    public Exception CreateExceptionFor(IInteractionContext ic) => new DisabledException(ic, Disables(ic));

    #endregion
}

// Copyright (c) Naked Objects Group Ltd.