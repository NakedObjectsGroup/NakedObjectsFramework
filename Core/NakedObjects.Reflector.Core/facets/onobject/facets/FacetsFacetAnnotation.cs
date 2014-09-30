// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Objects.Facets;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.Value {
    public class FacetsFacetAnnotation : FacetsFacetAbstract {
        public FacetsFacetAnnotation(FacetsAttribute attribute, IFacetHolder holder)
            : base(attribute.FacetFactoryNames, attribute.FacetFactoryClasses, holder) {}
    }
}

// Copyright (c) Naked Objects Group Ltd.