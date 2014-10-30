// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Meta.Utils;

namespace NakedObjects.Reflect.Spec {
    public class FacetDecoratorSet : IFacetDecoratorSet {
        private readonly IDictionary<Type, IList<IFacetDecorator>> facetDecorators = new Dictionary<Type, IList<IFacetDecorator>>();

        #region IFacetDecoratorSet Members

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

        #endregion

        private void DecoratedFacet(Type facetType, ISpecification holder) {
            if (facetDecorators.ContainsKey(facetType)) {
                foreach (IFacetDecorator decorator in facetDecorators[facetType]) {
                    IFacet previousFacet = holder.GetFacet(facetType);
                    IFacet decoratedFacet = decorator.Decorate(previousFacet, holder);
                    if (decoratedFacet != null && decoratedFacet != previousFacet) {
                        FacetUtils.AddFacet(decoratedFacet);
                    }
                }
            }
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}