// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter.Value;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Actcoll.Typeof;
using NakedObjects.Architecture.Facets.Objects.Bounded;
using NakedObjects.Architecture.Facets.Objects.Immutable;
using NakedObjects.Architecture.Facets.Properties.Enums;

namespace NakedObjects.Architecture.Spec {
    public static class SpecificationFacets {
        public static bool IsAlwaysImmutable(this INakedObjectSpecification specification) {
            var immutableFacet = specification.GetFacet<IImmutableFacet>();
            if (immutableFacet == null) {
                return false;
            }
            return immutableFacet.Value == When.Always;
        }

        public static bool IsImmutableOncePersisted(this INakedObjectSpecification specification) {
            var immutableFacet = specification.GetFacet<IImmutableFacet>();
            if (immutableFacet == null) {
                return false;
            }
            return immutableFacet.Value == When.OncePersisted;
        }

        public static bool IsBoundedSet(this INakedObjectSpecification specification) {
            return specification.ContainsFacet<IBoundedFacet>() || specification.ContainsFacet<IEnumValueFacet>();
        }

        public static bool IsCollectionOfBoundedSet(this INakedObjectSpecification specification) {
            return specification.IsCollection && specification.GetFacet<ITypeOfFacet>().ValueSpec.IsBoundedSet();
        }

        public static bool IsCollectionOfEnum(this INakedObjectSpecification specification) {
            return specification.IsCollection && specification.GetFacet<ITypeOfFacet>().ValueSpec.ContainsFacet<IEnumFacet>();
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}