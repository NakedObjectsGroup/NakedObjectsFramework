// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Interactions;

namespace NakedObjects.Architecture.Facets.Propparam.Validate.Mandatory {
    /// <summary>
    ///     Whether a property or a parameter is mandatory or optional.
    /// </summary>
    /// <para>
    ///     For a mandatory property, the object cannot be saved/updated without
    ///     the value being provided.  For a mandatory parameter, the action cannot
    ///     be invoked without the value being provided.
    /// </para>
    /// <para>
    ///     In the standard Naked Objects Programming Model, a property or parameter is mandatory
    ///     <i>unless</i> it is annotated with the <see cref="OptionallyAttribute" /> annotation.
    /// </para>
    public interface IMandatoryFacet : IMarkerFacet, IValidatingInteractionAdvisor {
        /// <summary>
        ///     Indicates that it is mandatory, that it is <i>not</i> optional.
        /// </summary>
        bool IsMandatory { get; }

        /// <summary>
        ///     Indicates that it is optional, that it is <i>not</i> mandatory.
        /// </summary>
        bool IsOptional { get; }

        /// <summary>
        ///     Whether this value is required but has not been provided (and is therefore invalid).
        /// </summary>
        /// <para>
        ///     If the value has been provided, <i>or</i> if the property or parameter is not required,
        ///     then will return <c>false</c>.
        /// </para>
        bool IsRequiredButNull(INakedObject nakedObject);
    }
}