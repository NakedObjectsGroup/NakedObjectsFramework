// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;

namespace NakedObjects.Architecture.Facets.Properties.Defaults {
    public class PropertyDefaultFacetNone : PropertyDefaultFacetAbstract {
        public PropertyDefaultFacetNone(IFacetHolder holder)
            : base(holder) {}

        public override bool IsNoOp {
            get { return true; }
        }

        /// <summary>
        ///     Provides a default of <c>null</c>
        /// </summary>
        public override object GetDefault(INakedObject inObject) {
            return null;
        }
    }


    // Copyright (c) Naked Objects Group Ltd.
}