// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.ComponentModel.DataAnnotations;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Propparam.Validate.Mandatory;

namespace NakedObjects.Reflector.DotNet.Facets.Propparam.Validate.Mandatory {
    /// <summary>
    ///     Derived by presence of a <see cref="RequiredAttribute" /> annotation.
    /// </summary>
    /// <para>
    ///     This implementation indicates that the <see cref="IFacetHolder" /> is mandatory.
    /// </para>
    public class MandatoryFacet : MandatoryFacetAbstract {
        public MandatoryFacet(IFacetHolder holder)
            : base(holder) {}

        /// <summary>
        ///     Always returns <c>true</c>, indicating that the facet holder is in fact mandatory.
        /// </summary>
        public override bool IsMandatory {
            get { return true; }
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}