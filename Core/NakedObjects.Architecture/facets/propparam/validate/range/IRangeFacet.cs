// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Interactions;

namespace NakedObjects.Architecture.Facets.Propparam.Validate.Range {
    /// <summary>
    ///     Whether the value of a property or paramter is outside a specified range
    /// </summary>
    /// <para>
    ///     In the standard Naked Objects Programming Model, corresponds to
    ///     the <see cref="System.ComponentModel.DataAnnotations.RangeAttribute" /> annotation
    /// </para>
    public interface IRangeFacet : IValidatingInteractionAdvisor, IFacet {
        IConvertible Min { get; }
        IConvertible Max { get; }

        /// <summary>
        ///     Returns true if the facet is a range applied to a Date
        /// </summary>
        bool IsDateRange { get; }

        /// <summary>
        ///     Whether the provided value is out of range
        /// </summary>
        int OutOfRange(INakedObject nakedObject);
    }
}