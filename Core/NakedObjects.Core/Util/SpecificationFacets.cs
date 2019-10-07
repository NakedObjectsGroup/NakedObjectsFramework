// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;

namespace NakedObjects.Core.Util {
    public static class SpecificationFacets {

        public static bool IsNeverPersisted(this ITypeSpec spec) {
            return spec.IsViewModel;
        }

        public static bool IsAlwaysImmutable(this ITypeSpec spec) {
            var immutableFacet = spec.GetFacet<IImmutableFacet>();
            if (immutableFacet == null) {
                return false;
            }
            return immutableFacet.Value == WhenTo.Always;
        }

        public static bool IsImmutableOncePersisted(this ITypeSpec spec) {
            var immutableFacet = spec.GetFacet<IImmutableFacet>();
            if (immutableFacet == null) {
                return false;
            }
            return immutableFacet.Value == WhenTo.OncePersisted;
        }

        public static bool IsBoundedSet(this ITypeSpec spec) {
            return spec.ContainsFacet<IBoundedFacet>() || spec.ContainsFacet<IEnumValueFacet>();
        }

        public static bool IsBoundedSet(this ITypeSpecImmutable spec) {
            return spec.ContainsFacet<IBoundedFacet>() || spec.ContainsFacet<IEnumValueFacet>();
        }

        public static bool IsCollectionOfBoundedSet(this ITypeSpec spec, IObjectSpec elementSpec) {
            return spec.IsCollection && elementSpec.IsBoundedSet();
        }

        public static bool IsCollectionOfBoundedSet(this ITypeSpecImmutable spec, IObjectSpecImmutable elementSpec) {
            return spec.IsCollection && elementSpec.IsBoundedSet();
        }

        public static bool IsCollectionOfEnum(this ITypeSpec spec, IObjectSpec elementSpec) {
            return spec.IsCollection && elementSpec.ContainsFacet<IEnumFacet>();
        }

        public static bool IsBoundedSet(this IObjectSpecImmutable specification) {
            return specification.ContainsFacet<IBoundedFacet>() || specification.ContainsFacet<IEnumValueFacet>();
        }

        public static bool IsCollectionOfBoundedSet(this IObjectSpecImmutable specification, IObjectSpecImmutable elementSpec) {
            return specification.IsCollection && elementSpec.IsBoundedSet();
        }

        public static bool IsCollectionOfEnum(this IObjectSpecImmutable specification, IObjectSpecImmutable elementSpec) {
            return specification.IsCollection && elementSpec.ContainsFacet<IEnumFacet>();
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}