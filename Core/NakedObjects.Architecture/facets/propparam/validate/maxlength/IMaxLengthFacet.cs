// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Interactions;

namespace NakedObjects.Architecture.Facets.Propparam.Validate.MaxLength {
    /// <summary>
    ///     Whether the (string) property or a parameter's length must not exceed a certain length
    /// </summary>
    public interface IMaxLengthFacet : ISingleIntValueFacet, IValidatingInteractionAdvisor {
        /// <summary>
        ///     Whether the provided string exceeds the maximum length
        /// </summary>
        bool Exceeds(INakedObject nakedObject);
    }
}