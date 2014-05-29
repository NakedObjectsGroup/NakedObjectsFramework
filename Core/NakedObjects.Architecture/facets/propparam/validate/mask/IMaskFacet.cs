// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets.Propparam.Validate.RegEx;
using NakedObjects.Architecture.Interactions;

namespace NakedObjects.Architecture.Facets.Propparam.Validate.Mask {
    /// <summary>
    ///     Whether the (string) property or a parameter must correspond to a specific mask.
    /// </summary>
    /// <para>
    ///     In the standard Naked Objects Programming Model, corresponds to the <see cref="MaskAttribute" /> annotation.
    /// </para>
    /// <para>
    ///     Not yet implemented by the framework or any viewer.
    /// </para>
    /// <seealso cref="IRegExFacet" />
    public interface IMaskFacet : ISingleStringValueFacet, IValidatingInteractionAdvisor {
        /// <summary>
        ///     Whether the provided string matches the mask
        /// </summary>
        bool DoesNotMatch(INakedObject nakedObject);
    }
}