using System;

namespace NakedObjects.Architecture.Facets.Properties.Set {
    public class IsASetFacet : MarkerFacetAbstract, IIsASetFacet {
        public IsASetFacet(IFacetHolder holder) : base(Type, holder) {}

        public static Type Type {
            get { return typeof (IIsASetFacet); }
        }
    }
}