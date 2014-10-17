// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Reflect;

namespace NakedObjects.Reflector.Spec {
    public class FacetDecoratorSet : IFacetDecoratorSet {
        private readonly IDictionary<Type, IList<IFacetDecorator>> facetDecorators = new Dictionary<Type, IList<IFacetDecorator>>();

        public virtual bool IsEmpty {
            get { return (facetDecorators.Count == 0); }
        }

        public virtual void Add(IFacetDecorator decorator) {
            foreach (Type type in decorator.ForFacetTypes) {
                if (!facetDecorators.ContainsKey(type)) {
                    facetDecorators[type] = new List<IFacetDecorator>();
                }
                facetDecorators[type].Add(decorator);
            }
        }

        public virtual void DecorateAllHoldersFacets(ISpecification holder) {
            if (!IsEmpty) {
                foreach (Type facetType in holder.FacetTypes) {
                    DecoratedFacet(facetType, holder);
                }
            }
        }

        private void DecoratedFacet(Type facetType, ISpecification holder) {
            if (facetDecorators.ContainsKey(facetType)) {
                foreach (IFacetDecorator decorator in facetDecorators[facetType]) {
                    IFacet previousFacet = holder.GetFacet(facetType);
                    IFacet decoratedFacet = decorator.Decorate(previousFacet, holder);
                    if (decoratedFacet != null && decoratedFacet != previousFacet) {
                        holder.AddFacet(decoratedFacet);
                    }
                }
            }
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}