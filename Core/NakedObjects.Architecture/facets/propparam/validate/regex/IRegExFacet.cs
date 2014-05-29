// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Text.RegularExpressions;
using NakedObjects.Architecture.Facets.Propparam.Validate.Mask;
using NakedObjects.Architecture.Interactions;

namespace NakedObjects.Architecture.Facets.Propparam.Validate.RegEx {
    /// <summary>
    ///     Whether the (string) property or a parameter must correspond to a specific regular expression
    /// </summary>
    /// <para>
    ///     In the standard Naked Objects Programming Model, corresponds to the <see cref="RegExAttribute" /> annotation
    /// </para>
    /// <seealso cref="IMaskFacet" />
    public interface IRegExFacet : IMultipleValueFacet, IValidatingInteractionAdvisor {
        Regex Pattern { get; }
        string FailureMessage { get; }
        bool DoesNotMatch(string proposed);
        string Format(string text);
    }
}