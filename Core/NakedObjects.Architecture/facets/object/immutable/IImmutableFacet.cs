// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Facets.Objects.EqualByContent;
using NakedObjects.Architecture.Facets.Objects.Value;
using NakedObjects.Architecture.Interactions;

namespace NakedObjects.Architecture.Facets.Objects.Immutable {
    /// <summary>
    ///     Indicates that the instances of this class are immutable and so may not be modified
    ///     either through the viewer or indeed programmatically
    /// </summary>
    /// <para>
    ///     In the standard Naked Objects Programming Model, typically corresponds to applying the
    ///     <see cref="ImmutableAttribute" /> annotation at the class level
    /// </para>
    /// <seealso cref="IEqualByContentFacet" />
    /// <seealso cref="IValueFacet" />
    public interface IImmutableFacet : ISingleWhenValueFacet, IDisablingInteractionAdvisor {}
}