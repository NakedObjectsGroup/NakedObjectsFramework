// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;

namespace NakedObjects.Architecture.Facets.Objects.Immutable {
    public class ImmutableFacetNever : ImmutableFacetAbstract {
        public ImmutableFacetNever(IFacetHolder holder)
            : base(When.Never, holder) {}

        /// <summary>
        ///     Always returns <c>null</c>
        /// </summary>
        public override string DisabledReason(INakedObject no) {
            return null;
        }
    }


    // Copyright (c) Naked Objects Group Ltd.
}