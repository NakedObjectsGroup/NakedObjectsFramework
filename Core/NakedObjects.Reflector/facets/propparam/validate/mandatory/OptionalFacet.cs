// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Propparam.Validate.Mandatory;

namespace NakedObjects.Reflector.DotNet.Facets.Propparam.Validate.Mandatory {
    /// <summary>
    ///     Derived by presence of an <see cref="OptionallyAttribute" /> annotation.
    /// </summary>
    /// <para>
    ///     This implementation indicates that the <see cref="IFacetHolder" /> is <i>not</i> mandatory.
    /// </para>
    public class OptionalFacet : MandatoryFacetAbstract {
        public OptionalFacet(IFacetHolder holder)
            : base(holder) {}

        /// <summary>
        ///     Always returns <c>false</c>, indicating that the facet holder is in fact optional.
        /// </summary>
        public override bool IsMandatory {
            get { return false; }
        }
    }


    // Copyright (c) Naked Objects Group Ltd.
}