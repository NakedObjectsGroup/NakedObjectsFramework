// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Architecture.Adapter.Value;
using NakedObjects.Architecture.Facets.Actcoll.Typeof;
using NakedObjects.Architecture.Facets.Objects.Bounded;
using NakedObjects.Architecture.Facets.Objects.Immutable;
using NakedObjects.Architecture.Facets.Properties.Enums;
using NakedObjects.Reflector.Spec;

namespace NakedObjects.Architecture.Spec {
    public static class SpecificationFacets {
        public static bool IsAlwaysImmutable(this IObjectSpec spec) {
            var immutableFacet = spec.GetFacet<IImmutableFacet>();
            if (immutableFacet == null) {
                return false;
            }
            return immutableFacet.Value == WhenTo.Always;
        }

        public static bool IsImmutableOncePersisted(this IObjectSpec spec) {
            var immutableFacet = spec.GetFacet<IImmutableFacet>();
            if (immutableFacet == null) {
                return false;
            }
            return immutableFacet.Value == WhenTo.OncePersisted;
        }

        public static bool IsBoundedSet(this IObjectSpec spec) {
            return spec.ContainsFacet<IBoundedFacet>() || spec.ContainsFacet<IEnumValueFacet>();
        }

        public static bool IsCollectionOfBoundedSet(this IObjectSpec spec) {
            return spec.IsCollection && spec.GetFacet<ITypeOfFacet>().ValueSpec.IsBoundedSet();
        }

        public static bool IsCollectionOfEnum(this IObjectSpec spec) {
            return spec.IsCollection && spec.GetFacet<ITypeOfFacet>().ValueSpec.ContainsFacet<IEnumFacet>();
        }

        public static bool IsBoundedSet(this IObjectSpecImmutable specification) {
            return specification.ContainsFacet<IBoundedFacet>() || specification.ContainsFacet<IEnumValueFacet>();
        }

        public static bool IsCollectionOfBoundedSet(this IObjectSpecImmutable specification) {
            return specification.IsCollection && specification.GetFacet<ITypeOfFacet>().ValueSpec.IsBoundedSet();
        }

        public static bool IsCollectionOfEnum(this IObjectSpecImmutable specification) {
            return specification.IsCollection && specification.GetFacet<ITypeOfFacet>().ValueSpec.ContainsFacet<IEnumFacet>();
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}