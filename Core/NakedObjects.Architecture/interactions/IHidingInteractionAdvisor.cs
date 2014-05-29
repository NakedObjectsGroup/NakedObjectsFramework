// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Architecture.Interactions {
    /// <summary>
    ///     Mix-in interface for facets that can advise as to whether a member
    ///     should be hidden.
    /// </summary>
    /// <seealso cref="IDisablingInteractionAdvisor" />
    /// <seealso cref="IValidatingInteractionAdvisor" />
    public interface IHidingInteractionAdvisor {
        /// <summary>
        ///     Whether the rule represented by this facet hides the
        ///     member to which it applies.
        /// </summary>
        /// <para>
        ///     Implementations should use the provided <see cref="InteractionContext" /> to
        ///     determine whether they disable the member from being modified or used.  They must
        ///     however guard against a <c>null</c> target <see cref="InteractionContext.Target" />
        ///     and session <see cref="InteractionContext.Session" /> - neither are guaranteed to be populated.
        /// </para>
        string Hides(InteractionContext ic);


        /// <summary>
        ///     Create (not throw) an exception to indicate that this
        ///     interaction cannot be performed because the target object or member is hidden.
        /// </summary>
        HiddenException CreateExceptionFor(InteractionContext ic);
    }
}