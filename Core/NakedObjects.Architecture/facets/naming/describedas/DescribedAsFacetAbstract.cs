// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects.Architecture.Facets.Naming.DescribedAs {
    public abstract class DescribedAsFacetAbstract : SingleStringValueFacetAbstract, IDescribedAsFacet {
        protected DescribedAsFacetAbstract(string valueString, IFacetHolder holder)
            : base(Type, holder, valueString) {}

        public static Type Type {
            get { return typeof (IDescribedAsFacet); }
        }
    }
}