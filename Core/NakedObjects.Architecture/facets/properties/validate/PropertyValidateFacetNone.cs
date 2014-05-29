// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;

namespace NakedObjects.Architecture.Facets.Properties.Validate {
    public class PropertyValidateFacetNone : PropertyValidateFacetAbstract {
        public PropertyValidateFacetNone(IFacetHolder holder)
            : base(holder) {}

        public override bool IsNoOp {
            get { return true; }
        }

        /// <summary>
        ///     Returns <c>null</c>, ie property is valid.
        /// </summary>
        /// <para>
        ///     Subclasses should override as required.
        /// </para>
        public override string InvalidReason(INakedObject inObject, INakedObject nakedParm) {
            return null;
        }
    }


    // Copyright (c) Naked Objects Group Ltd.
}