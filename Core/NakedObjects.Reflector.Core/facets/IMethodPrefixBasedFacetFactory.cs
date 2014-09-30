// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Facets;

namespace NakedObjects.Reflector.DotNet.Facets {
    /// <summary>
    ///     Indicates that the <see cref="IFacetFactory" /> works by recognizing methods
    ///     with a certain prefix (or prefixes).
    /// </summary>
    /// <para>
    ///     Used by <see cref="IFacetFactorySet.Recognizes" />
    /// </para>
    public interface IMethodPrefixBasedFacetFactory : IFacetFactory {
        string[] Prefixes { get; }
    }

    // Copyright (c) Naked Objects Group Ltd.
}