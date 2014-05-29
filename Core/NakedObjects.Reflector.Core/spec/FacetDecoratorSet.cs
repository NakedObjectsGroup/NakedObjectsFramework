// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using NakedObjects.Architecture;
using NakedObjects.Architecture.Facets;

namespace NakedObjects.Reflector.Spec {
    public class FacetDecoratorSet : IRequiresSetup {
        private readonly IDictionary<Type, IList<IFacetDecorator>> facetDecorators = new Dictionary<Type, IList<IFacetDecorator>>();

        public virtual bool IsEmpty {
            get { return (facetDecorators.Count == 0); }
        }

        #region IRequiresSetup Members

        public virtual void Init() {
            foreach (var decoratorlist in facetDecorators.Values) {
                foreach (IFacetDecorator decorator in decoratorlist) {
                    if (decorator is IRequiresSetup) {
                        ((IRequiresSetup) decorator).Init();
                    }
                }
            }
        }

        public virtual void Shutdown() {
            foreach (var decoratorlist in facetDecorators.Values) {
                foreach (IFacetDecorator decorator in decoratorlist) {
                    if (decorator is IRequiresSetup) {
                        ((IRequiresSetup) decorator).Shutdown();
                    }
                }
            }
        }

        #endregion

        public virtual void Add(IFacetDecorator decorator) {
            foreach (Type type in decorator.ForFacetTypes) {
                if (!facetDecorators.ContainsKey(type)) {
                    facetDecorators[type] = new List<IFacetDecorator>();
                }
                facetDecorators[type].Add(decorator);
            }
        }

        public virtual void DecorateAllHoldersFacets(IFacetHolder holder) {
            if (!IsEmpty) {
                foreach (Type facetType in holder.FacetTypes) {
                    DecoratedFacet(facetType, holder);
                }
            }
        }

        private void DecoratedFacet(Type facetType, IFacetHolder holder) {
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