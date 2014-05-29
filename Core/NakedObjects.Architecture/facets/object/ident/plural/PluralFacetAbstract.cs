// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects.Architecture.Facets.Objects.Ident.Plural {
    public abstract class PluralFacetAbstract : SingleStringValueFacetAbstract, IPluralFacet {
        protected PluralFacetAbstract(string stringValue, IFacetHolder holder)
            : base(Type, holder, stringValue) {}

        public static Type Type {
            get { return typeof (IPluralFacet); }
        }
    }
}