// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects.Architecture.Facets.Naming.Named {
    public abstract class NamedFacetAbstract : SingleStringValueFacetAbstract, INamedFacet {
        protected NamedFacetAbstract(string valueString, IFacetHolder holder)
            : base(Type, holder, valueString) {}

        public static Type Type {
            get { return typeof (INamedFacet); }
        }
    }
}