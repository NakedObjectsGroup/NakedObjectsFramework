// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Objects.Defaults;
using NakedObjects.Architecture.Facets.Properties.Defaults;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.Defaults {
    public class PropertyDefaultFacetDerivedFromDefaultedFacet : FacetAbstract, IPropertyDefaultFacet {
        private readonly IDefaultedFacet typeFacet;

        public PropertyDefaultFacetDerivedFromDefaultedFacet(IDefaultedFacet typeFacet, ISpecification holder)
            : base(typeof (IPropertyDefaultFacet), holder) {
            this.typeFacet = typeFacet;
        }

        #region IPropertyDefaultFacet Members

        public object GetDefault(INakedObject inObject) {
            return typeFacet.Default;
        }

        #endregion
    }
}