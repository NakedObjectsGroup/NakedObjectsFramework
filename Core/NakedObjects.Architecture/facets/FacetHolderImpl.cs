// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;

namespace NakedObjects.Architecture.Facets {
    /// <summary>
    ///     For base subclasses or, more likely, to help write tests
    /// </summary>
    public class FacetHolderImpl : IFacetHolder {
        private readonly Dictionary<Type, IFacet> facetsByClass = new Dictionary<Type, IFacet>();

        #region IFacetHolder Members

        public virtual Type[] FacetTypes {
            get { return FacetUtils.GetFacetTypes(facetsByClass); }
        }

        public virtual IIdentifier Identifier {
            get { return null; }
        }

        public bool ContainsFacet(Type facetType) {
            return GetFacet(facetType) != null;
        }

        public bool ContainsFacet<T>() where T : IFacet {
            return GetFacet<T>() != null;
        }

        public void AddFacet(IFacet facet) {
            AddFacet(facet.FacetType, facet);
        }

        public void AddFacet(IMultiTypedFacet facet) {
            foreach (Type facetType in facet.FacetTypes) {
                AddFacet(facetType, facet.GetFacet(facetType));
            }
        }

        public void RemoveFacet(IFacet facet) {
            FacetUtils.RemoveFacet(facetsByClass, facet);
        }

        public void RemoveFacet(Type facetType) {
            FacetUtils.RemoveFacet(facetsByClass, facetType);
        }

        public virtual IFacet GetFacet(Type facetType) {
            return FacetUtils.GetFacet(facetsByClass, facetType);
        }

        public T GetFacet<T>() where T : IFacet {
            return (T) GetFacet(typeof (T));
        }

        public IFacet[] GetFacets(IFacetFilter filter) {
            return FacetUtils.GetFacets(facetsByClass, filter);
        }

        #endregion

        private void AddFacet(Type facetType, IFacet facet) {
            IFacet existingFacet = GetFacet(facetType);
            if (existingFacet == null || existingFacet.IsNoOp || facet.CanAlwaysReplace) {
                facetsByClass[facetType] = facet;
            }
        }
    }
}