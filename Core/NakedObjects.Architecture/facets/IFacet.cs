// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Facets.Actions.Invoke;

namespace NakedObjects.Architecture.Facets {
    public interface IFacet {
        /// <summary>
        ///     The <see cref="IFacetHolder" /> of this facet. Set allows reparenting of Facet.
        /// </summary>
        /// <para>
        ///     Used by Facet decorators.
        /// </para>
        IFacetHolder FacetHolder { get; set; }

        /// <summary>
        ///     Whether this facet implementation is a no-op
        /// </summary>
        bool IsNoOp { get; }

        /// <summary>
        ///     Determines the type of this facet to be stored under.
        /// </summary>
        /// <para>
        ///     The framework looks for <see cref="IFacet" />s of certain well-known facet types.
        ///     Each facet implementation must specify which type of facet it corresponds to.
        ///     This therefore allows the (rules of the) programming model to be varied
        ///     without impacting the rest of the framework.
        /// </para>
        /// <para>
        ///     For example, the <see cref="IActionInvocationFacet" /> specifies the facet to
        ///     invoke an action.  The typical implementation of this wraps a <c>public</c>
        ///     method.  However, a different facet factory could be installed that creates
        ///     facet also of type <see cref="IActionInvocationFacet" /> but that have some other
        ///     rule, such as requiring an <i>action</i> prefix, or that decorate the
        ///     interaction by logging it, for example.
        /// </para>
        Type FacetType { get; }


        /// <summary>
        ///     Whether this facet implementation should replace existing(none-noop) implementations.
        /// </summary>
        bool CanAlwaysReplace { get; }
    }
}