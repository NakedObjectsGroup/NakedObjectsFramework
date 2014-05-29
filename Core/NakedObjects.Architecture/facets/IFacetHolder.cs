// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects.Architecture.Facets {
    /// <summary>
    ///     Anything in the metamodel (which also includes peers in the reflector) that can be extended
    /// </summary>
    public interface IFacetHolder {
        /// <summary>
        ///     Get the list of all facet <see cref="Type" />s that are supported by objects of this specification
        /// </summary>
        Type[] FacetTypes { get; }

        /// <summary>
        ///     Identifier of this <see cref="IFacetHolder" /> (feature)
        /// </summary>
        IIdentifier Identifier { get; }

        /// <summary>
        ///     Whether there is a facet registered of the specified type.
        /// </summary>
        /// <para>
        ///     Convenience; saves having to <see cref="GetFacet" /> and then check if <c>null</c>.
        /// </para>
        bool ContainsFacet(Type facetType);

        /// <summary>
        ///     Whether there is a facet registered of the specified type.
        /// </summary>
        /// <para>
        ///     Convenience; saves having to <see cref="GetFacet" /> and then check if <c>null</c>.
        /// </para>
        bool ContainsFacet<T>() where T : IFacet;

        /// <summary>
        ///     Get the facet of the specified type (as per the type it reports from <see cref="IFacet.FacetType" />)
        /// </summary>
        IFacet GetFacet(Type type);

        /// <summary>
        ///     Get the facet of type T (as per the type it reports from <see cref="IFacet.FacetType" />)
        /// </summary>
        T GetFacet<T>() where T : IFacet;

        /// <summary>
        ///     Returns all <see cref="IFacet" />s matching the specified <see cref="IFacetFilter" />
        /// </summary>
        IFacet[] GetFacets(IFacetFilter filter);

        /// <summary>
        ///     Adds the facet, extracting its <see cref="IFacet.FacetType" /> as the key.
        /// </summary>
        /// <para>
        ///     If there are any facet of the same type, they will be overwritten <i>provided</i>
        ///     that either the <see cref="IFacet" /> specifies to <see cref="IFacet.CanAlwaysReplace" />
        ///     or if the existing <see cref="IFacet" /> is an <see cref="IFacet.IsNoOp" />
        /// </para>
        void AddFacet(IFacet facet);

        /// <summary>
        ///     Adds the <see cref="IMultiTypedFacet" />, extracting each of its <see cref="IMultiTypedFacet.FacetTypes" /> as keys.
        /// </summary>
        /// <para>
        ///     If there are any facet of the same type, they will be overwritten <i>provided</i>
        ///     that either the <see cref="IFacet" /> specifies to <see cref="IFacet.CanAlwaysReplace" />
        ///     or if the existing <see cref="IFacet" /> is an <see cref="IFacet.IsNoOp" />
        /// </para>
        void AddFacet(IMultiTypedFacet facet);

        /// <summary>
        ///     Remove the facet whose type is that reported by <see cref="IFacet.FacetType" />
        /// </summary>
        void RemoveFacet(IFacet facet);

        /// <summary>
        ///     Remove the facet of the specified type
        /// </summary>
        void RemoveFacet(Type facetType);
    }
}