// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Facets.Objects.Immutable;
using NakedObjects.Architecture.Facets.Objects.Value;

namespace NakedObjects.Architecture.Facets.Objects.EqualByContent {
    /// <summary>
    ///     Indicates that the instances of this class are equal-by-content
    /// </summary>
    /// <seealso cref="IImmutableFacet" />
    /// <seealso cref="IValueFacet" />
    public interface IEqualByContentFacet : IMarkerFacet {}
}