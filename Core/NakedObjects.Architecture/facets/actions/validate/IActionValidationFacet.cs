// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets.Actions.Invoke;
using NakedObjects.Architecture.Interactions;

namespace NakedObjects.Architecture.Facets.Actions.Validate {
    /// <summary>
    ///     The mechanism by which the set of parameters of the action can be validated
    ///     before the action itself is invoked.
    /// </summary>
    /// <para>
    ///     In the standard Naked Objects Programming Model, corresponds to invoking the
    ///     <c>ValidateXxx</c> support method for an action.
    /// </para>
    /// <para>
    ///     The parameters may be validated independently first (eg a range check on a numeric parameter).
    /// </para>
    /// <seealso cref="IActionInvocationFacet" />
    public interface IActionValidationFacet : IFacet, IValidatingInteractionAdvisor {
        /// <summary>
        ///     Reason why the validation has failed, or <c>null</c> if okay
        /// </summary>
        string InvalidReason(INakedObject target, INakedObject[] arguments);
    }
}