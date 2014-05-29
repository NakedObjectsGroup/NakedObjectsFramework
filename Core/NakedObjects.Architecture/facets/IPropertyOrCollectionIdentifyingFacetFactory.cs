// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Reflection;

namespace NakedObjects.Architecture.Facets {
    /// <summary>
    ///     A <see cref="IFacetFactory" /> implementation that is able to identify a property or collection.
    /// </summary>
    /// <para>
    ///     For example, property  is  used to represent either a property (value or reference) or a collection,
    ///     with the return type indicating which.
    /// </para>
    /// <para>
    ///     Used by <see cref="IFacetFactorySet" /> to determine which facet factories to ask
    ///     whether a <see cref="PropertyInfo" /> represents a property or a collection.
    /// </para>
    public interface IPropertyOrCollectionIdentifyingFacetFactory : IFacetFactory {}
}