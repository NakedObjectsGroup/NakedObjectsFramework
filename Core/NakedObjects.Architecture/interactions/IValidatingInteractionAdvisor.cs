// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Facets.Ordering.MemberOrder;
using NakedObjects.Architecture.Facets.Propparam.Validate.MaxLength;

namespace NakedObjects.Architecture.Interactions {
    /// <summary>
    ///     Mix-in interface for facets that can advise as to whether a proposed value is valid
    /// </summary>
    /// <para>
    ///     For example, <see cref="IMaxLengthFacet" /> does constrain the
    ///     length of candidate values, whereas <see cref="IMemberOrderFacet" /> does not - it's basically basically just
    ///     a UI hint
    /// </para>
    /// <seealso cref="IDisablingInteractionAdvisor" />
    /// <seealso cref="IHidingInteractionAdvisor" />
    public interface IValidatingInteractionAdvisor {
        /// <summary>
        ///     Whether the validation represented by this facet passes or fails
        /// </summary>
        /// <para>
        ///     Implementations should use the provided <see cref="InteractionContext" /> to
        ///     determine whether they declare the interaction invalid.  They must
        ///     however guard against a <c>null</c> <see cref="InteractionContext.Target" /> target
        ///     and <see cref="InteractionContext.Session" /> session} - neither are
        ///     guaranteed to be populated
        /// </para>
        string Invalidates(InteractionContext ic);

        /// <summary>
        ///     Create (not throw) an exception of the appropriate subclass
        ///     if the validation has failed
        /// </summary>
        InvalidException CreateExceptionFor(InteractionContext ic);
    }
}