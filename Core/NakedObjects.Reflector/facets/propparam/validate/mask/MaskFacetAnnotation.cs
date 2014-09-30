// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Propparam.Validate.Mask;

namespace NakedObjects.Reflector.DotNet.Facets.Propparam.Validate.Mask {
    public class MaskFacetAnnotation : MaskFacetAbstract {
        public MaskFacetAnnotation(string value, IFacetHolder holder)
            : base(value, holder) {}

        /// <summary>
        ///     Not yet implemented, so always returns <c>false</c>.
        /// </summary>
        public override bool DoesNotMatch(INakedObject nakedObject) {
            return false;
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}