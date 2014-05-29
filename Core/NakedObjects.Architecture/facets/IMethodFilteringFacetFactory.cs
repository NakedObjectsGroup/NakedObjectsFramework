// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Reflection;

namespace NakedObjects.Architecture.Facets {
    /// <summary>
    ///     A <see cref="IFacetFactory" /> which filters out arbitrary <see cref="MethodInfo" /> methods.
    /// </summary>
    /// <para>
    ///     Used by <see cref="IFacetFactorySet.Filters" />
    /// </para>
    public interface IMethodFilteringFacetFactory : IFacetFactory {
        bool Filters(MethodInfo method);
    }

    // Copyright (c) Naked Objects Group Ltd.
}