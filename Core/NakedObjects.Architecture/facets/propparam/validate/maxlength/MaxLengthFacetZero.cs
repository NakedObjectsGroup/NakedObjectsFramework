// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Interactions;

namespace NakedObjects.Architecture.Facets.Propparam.Validate.MaxLength {
    public class MaxLengthFacetZero : MaxLengthFacetAbstract {
        private const int NoLimit = 0;

        public MaxLengthFacetZero(IFacetHolder holder)
            : base(NoLimit, holder) {}

        public override bool IsNoOp {
            get { return true; }
        }

        /// <summary>
        ///     No limit to maximum length
        /// </summary>
        public override string Invalidates(InteractionContext interactionContext) {
            return null;
        }
    }


    // Copyright (c) Naked Objects Group Ltd.
}