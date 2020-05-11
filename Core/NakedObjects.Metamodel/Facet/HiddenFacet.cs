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
using NakedObjects.Architecture.Interactions;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core;
using NakedObjects.Core.Resolve;

namespace NakedObjects.Meta.Facet {
    [Serializable]
    public sealed class HiddenFacet : SingleWhenValueFacetAbstract, IHiddenFacet {
        public HiddenFacet(WhenTo when, ISpecification holder)
            : base(typeof(IHiddenFacet), holder, when) { }

        #region IHiddenFacet Members

        public string HiddenReason(INakedObjectAdapter target) =>
            Value switch {
                WhenTo.Always => Resources.NakedObjects.AlwaysHidden,
                WhenTo.Never => null,
                WhenTo.UntilPersisted when target != null && target.ResolveState.IsTransient() => Resources.NakedObjects.HiddenUntilPersisted,
                WhenTo.OncePersisted when target != null && target.ResolveState.IsPersistent() => Resources.NakedObjects.HiddenOncePersisted,
                _ => null
            };

        public string Hides(IInteractionContext ic, ILifecycleManager lifecycleManager, IMetamodelManager manager) => HiddenReason(ic.Target);

        public Exception CreateExceptionFor(IInteractionContext ic, ILifecycleManager lifecycleManager, IMetamodelManager manager) => new HiddenException(ic, Hides(ic, lifecycleManager, manager));

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}