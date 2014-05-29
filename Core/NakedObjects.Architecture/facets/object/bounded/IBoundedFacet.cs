// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Interactions;

namespace NakedObjects.Architecture.Facets.Objects.Bounded {
    /// <summary>
    ///     Whether the number of instances of this class is bounded.
    /// </summary>
    /// <para>
    ///     Typically viewers will interpret this information by displaying
    ///     all instances of the class in a drop-down list box or similar widget.
    /// </para>
    /// <para>
    ///     In the standard Naked Objects Programming Model, corresponds to
    ///     annotating the member with <see cref="BoundedAttribute" />
    /// </para>
    public interface IBoundedFacet : IMarkerFacet, IDisablingInteractionAdvisor {}
}