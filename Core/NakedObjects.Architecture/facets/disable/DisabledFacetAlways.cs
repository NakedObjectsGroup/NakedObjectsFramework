// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;

namespace NakedObjects.Architecture.Facets.Disable {
    public class DisabledFacetAlways : DisabledFacetAbstract {
        public DisabledFacetAlways(IFacetHolder holder)
            : base(When.Always, holder) {}

        /// <summary>
        ///     Always returns <i>Always disabled</i>
        /// </summary>
        public override string DisabledReason(INakedObject target) {
            return Resources.NakedObjects.AlwaysDisabled;
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}