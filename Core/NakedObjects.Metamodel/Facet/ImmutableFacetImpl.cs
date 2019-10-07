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
    public abstract class ImmutableFacetImpl : ImmutableFacetAbstract {
        protected ImmutableFacetImpl(WhenTo when, ISpecification holder)
            : base(when, holder) { }

        public override string DisabledReason(INakedObjectAdapter target) {
            if (Value == WhenTo.Always) {
                return string.Format(Resources.NakedObjects.ImmutableMessage, target.Spec.SingularName);
            }

            if (Value == WhenTo.Never) {
                return null;
            }

            // remaining tests depend on target in question.
            if (target == null) {
                return null;
            }

            if (Value == WhenTo.UntilPersisted) {
                return target.ResolveState.IsTransient() ? string.Format(Resources.NakedObjects.ImmutableUntilPersistedMessage, target.Spec.SingularName) : null;
            }

            if (Value == WhenTo.OncePersisted) {
                return target.ResolveState.IsPersistent() ? string.Format(Resources.NakedObjects.ImmutableOncePersistedMessage, target.Spec.SingularName) : null;
            }

            return null;
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}