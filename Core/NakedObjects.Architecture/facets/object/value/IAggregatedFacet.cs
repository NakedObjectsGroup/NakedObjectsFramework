// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Facets.Objects.Immutable;

namespace NakedObjects.Architecture.Facets.Objects.Value {
    /// <summary>
    ///     Indicates that this class is aggregated, that is, wholly contained within a larger object.
    /// </summary>
    /// <para>
    ///     The object may or may not be immutable <see cref="IImmutableFacet" />, and may
    ///     reference regular entity domain objects or other aggregated objects.
    /// </para>
    /// <para>
    ///     One important subset of aggregated objects are  value types <see cref="IValueFacet" />
    ///     In fact, value types only have one mandatory semantic, that they are aggregated.
    /// </para>
    /// <para>
    ///     In terms of an analogy, aggregated is similar to Hibernate's component types
    ///     (for larger mutable in-line objects) or to Hibernate's user-defined types (for
    ///     smaller immutable values).
    /// </para>
    public interface IAggregatedFacet : IMarkerFacet {}
}