using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Interactions;

namespace NakedObjects.Architecture.Facets.Propparam.Validate.Range {
    /// <summary>
    /// Whether the value of a property or paramter is outside a specified range 
    /// </summary>
    /// <para>
    /// In the standard Naked Objects Programming Model, corresponds to
    /// the <see cref="System.ComponentModel.DataAnnotations.RangeAttribute"/> annotation
    /// </para>
  
    public interface IRangeFacet : IValidatingInteractionAdvisor, IFacet {
        /// <summary>
        /// Whether the provided value is out of range
        /// </summary>
        int OutOfRange(INakedObject nakedObject);

        IConvertible Min { get; }
        IConvertible Max { get; }
    }
}