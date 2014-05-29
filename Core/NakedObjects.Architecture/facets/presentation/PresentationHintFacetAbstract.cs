// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects.Architecture.Facets.Presentation {
    public abstract class PresentationHintFacetAbstract : SingleStringValueFacetAbstract, IPresentationHintFacet {
        protected PresentationHintFacetAbstract(string stringValue, IFacetHolder holder)
            : base(Type, holder, stringValue) {}

        public static Type Type {
            get { return typeof (IPresentationHintFacet); }
        }
    }
}