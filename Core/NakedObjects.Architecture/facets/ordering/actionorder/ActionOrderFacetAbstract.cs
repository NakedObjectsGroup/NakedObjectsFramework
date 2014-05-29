// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects.Architecture.Facets.Ordering.MemberOrder {
    public abstract class ActionOrderFacetAbstract : SingleStringValueFacetAbstract, IActionOrderFacet {
        protected ActionOrderFacetAbstract(string stringValue, IFacetHolder holder)
            : base(Type, holder, stringValue) {}

        public static Type Type {
            get { return typeof (IActionOrderFacet); }
        }
    }
}