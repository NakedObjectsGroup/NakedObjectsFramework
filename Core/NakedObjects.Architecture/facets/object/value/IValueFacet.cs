// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Architecture.Facets.Objects.Value {
    /// <summary>
    ///     Indicates that this class has value semantics
    /// </summary>
    /// <para>
    ///     In the standard Naked Objects Programming Model, corresponds to
    ///     the <see cref="ValueAttribute" /> annotation.  However, value semantics
    ///     is just a convenient term for a number of mostly optional
    ///     semantics all of which are defined elsewhere.  The only
    ///     semantic that it definitely does imply is aggregation <see cref="IAggregatedFacet" />
    /// </para>
    public interface IValueFacet : IMarkerFacet, IMultiTypedFacet {
        bool IsValid { get; }
    }
}