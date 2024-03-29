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
using NakedFramework.Core.Resolve;

namespace NakedFramework.Metamodel.Facet;

[Serializable]
public abstract class ImmutableFacetAbstract : SingleWhenValueFacetAbstract, IImmutableFacet {
    protected ImmutableFacetAbstract(WhenTo when)
        : base(when) { }

    public override Type FacetType => typeof(IImmutableFacet);

    public virtual string DisabledReason(INakedObjectAdapter target) =>
        Value switch {
            WhenTo.Always => string.Format(NakedObjects.Resources.NakedObjects.ImmutableMessage, target.Spec.SingularName),
            WhenTo.Never => null,
            WhenTo.UntilPersisted when target != null && target.ResolveState.IsTransient() => string.Format(NakedObjects.Resources.NakedObjects.ImmutableUntilPersistedMessage, target.Spec.SingularName),
            WhenTo.OncePersisted when target != null && target.ResolveState.IsPersistent() => string.Format(NakedObjects.Resources.NakedObjects.ImmutableOncePersistedMessage, target.Spec.SingularName),
            _ => null
        };

    #region IImmutableFacet Members

    public virtual string Disables(IInteractionContext ic) => DisabledReason(ic.Target);

    public virtual Exception CreateExceptionFor(IInteractionContext ic) => new DisabledException(ic, Disables(ic));

    #endregion
}