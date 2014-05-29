// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects.Architecture.Facets.Objects.Facets {
    /// <summary>
    ///     Indicates that this class has additional arbitrary facets, to be processed.
    /// </summary>
    /// <para>
    ///     Corresponds to the <see cref="FacetsAttribute" /> annotation in the applib.
    /// </para>
    /// <para>
    ///     This <see cref="IFacet" /> allows the <see cref="IFacetFactory" />(s) that will create those
    ///     <see cref="IFacet" />s to be accessed.  Which, admittedly, is rather confusing.
    /// </para>
    public interface IFacetsFacet : IMultipleValueFacet {
        /// <summary>
        ///     Returns the fully qualified class of the facet factory, which
        ///     should be "<see cref="Type.IsAssignableFrom" />"  <see cref="IFacetFactory" />
        /// </summary>
        /// <para>
        ///     Includes both the named facet factories and those identified directly
        ///     by class.  However, all are guaranteed to implement <see cref="IFacetFactory" />
        /// </para>
        Type[] FacetFactories { get; }
    }
}