// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Resolve;

namespace NakedObjects.Meta.Facet {
    [Serializable]
    public sealed class DisabledFacetAnnotation : DisabledFacetAbstract {
        public DisabledFacetAnnotation(WhenTo value, ISpecification holder)
            : base(value, holder) { }

        public override string DisabledReason(INakedObjectAdapter target) {
            if (Value == WhenTo.Always) {
                return Resources.NakedObjects.AlwaysDisabled;
            }

            if (Value == WhenTo.Never) {
                return null;
            }

            // remaining tests depend upon the actual target in question
            if (target == null) {
                return null;
            }

            if (Value == WhenTo.UntilPersisted) {
                return target.ResolveState.IsTransient() ? Resources.NakedObjects.DisabledUntilPersisted : null;
            }

            if (Value == WhenTo.OncePersisted) {
                return target.ResolveState.IsPersistent() ? Resources.NakedObjects.DisabledOncePersisted : null;
            }

            return null;
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}