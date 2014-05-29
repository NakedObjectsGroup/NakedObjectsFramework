// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects.Architecture.Facets {
    /// <summary>
    ///     A Class that provides multiple facet implementations, either directly or through a delegate
    /// </summary>
    /// <para>
    ///     The client of this interface should use <see cref="GetFacet" /> to
    ///     obtain the facet implementation for each of the <see cref="FacetTypes" /> facets types}
    /// </para>
    public interface IMultiTypedFacet : IFacet {
        Type[] FacetTypes { get; }

        IFacet GetFacet(Type facet);

        T GetFacet<T>() where T : IFacet;
    }


    // Copyright (c) Naked Objects Group Ltd.
}