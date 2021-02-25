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
using NakedFramework.Architecture.Spec;
using NakedFramework.Core.Exception;
using NakedFramework.Core.Resolve;

namespace NakedFramework.Metamodel.Facet {
    [Serializable]
    public sealed class HiddenFacet : SingleWhenValueFacetAbstract, IHiddenFacet {
        public HiddenFacet(WhenTo when, ISpecification holder)
            : base(typeof(IHiddenFacet), holder, when) { }

        #region IHiddenFacet Members

        public string HiddenReason(INakedObjectAdapter target) =>
            Value switch {
                WhenTo.Always => NakedObjects.Resources.NakedObjects.AlwaysHidden,
                WhenTo.Never => null,
                WhenTo.UntilPersisted when target != null && target.ResolveState.IsTransient() => NakedObjects.Resources.NakedObjects.HiddenUntilPersisted,
                WhenTo.OncePersisted when target != null && target.ResolveState.IsPersistent() => NakedObjects.Resources.NakedObjects.HiddenOncePersisted,
                _ => null
            };

        private string HiddenReason(bool persisted) =>
            Value switch {
                WhenTo.Always => NakedObjects.Resources.NakedObjects.AlwaysHidden,
                WhenTo.Never => null,
                WhenTo.UntilPersisted when !persisted => NakedObjects.Resources.NakedObjects.HiddenUntilPersisted,
                WhenTo.OncePersisted when persisted => NakedObjects.Resources.NakedObjects.HiddenOncePersisted,
                _ => null
            };

        public string Hides(IInteractionContext ic) => HiddenReason(ic.Target);

        public string HidesForState(bool persisted) => HiddenReason(persisted);

        public System.Exception CreateExceptionFor(IInteractionContext ic) => new HiddenException(ic, Hides(ic));

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}