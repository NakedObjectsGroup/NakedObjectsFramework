// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;

namespace NakedFramework.Core.Util {
    public static class SpecificationFacets {
        public static bool IsNeverPersisted(this ITypeSpec spec) => spec.IsViewModel;

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

        public static bool IsBoundedSet(this ITypeSpec spec) => spec.ContainsFacet<IBoundedFacet>() || spec.ContainsFacet<IEnumValueFacet>();

        public static bool IsCollectionOfBoundedSet(this ITypeSpec spec, IObjectSpec elementSpec) => spec.IsCollection && elementSpec.IsBoundedSet();

        public static bool IsCollectionOfEnum(this ITypeSpec spec, IObjectSpec elementSpec) => spec.IsCollection && elementSpec.ContainsFacet<IEnumFacet>();

        public static bool IsBoundedSet(this IObjectSpecImmutable specification) => specification.ContainsFacet<IBoundedFacet>() || specification.ContainsFacet<IEnumValueFacet>();

        public static IFacet GetOpFacet<T>(this ISpecification s) where T : class, IFacet {
            var facet = s.GetFacet<T>();
            return facet == null
                ? null
                : facet.IsNoOp
                    ? null
                    : facet;
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}