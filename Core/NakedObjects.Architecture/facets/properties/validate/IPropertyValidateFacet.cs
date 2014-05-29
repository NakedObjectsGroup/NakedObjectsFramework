// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets.Properties.Modify;
using NakedObjects.Architecture.Interactions;

namespace NakedObjects.Architecture.Facets.Properties.Validate {
    /// <summary>
    ///     The mechanism by which the proposed value of a property can be validated,
    ///     called immediately before <see cref="IPropertySetterFacet" /> setting the value.
    /// </summary>
    /// <para>
    ///     In the standard Naked Objects Programming Model, corresponds to
    ///     invoking the <c>ValidateXxx</c> method for a property<c>Xxx</c>.
    /// </para>
    /// <seealso cref="IPropertySetterFacet" />
    public interface IPropertyValidateFacet : IFacet, IValidatingInteractionAdvisor {
        /// <summary>
        ///     The reason why the proposed value is invalid.
        /// </summary>
        /// <para>
        ///     Should return <c>null</c> if the value is in fact valid.
        /// </para>
        string InvalidReason(INakedObject targetObject, INakedObject proposedValue);
    }
}