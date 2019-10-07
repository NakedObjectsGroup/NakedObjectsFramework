// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Util;
using NakedObjects.Meta.Utils;

namespace NakedObjects.Reflect {
    public sealed class FacetDecoratorSet : IFacetDecoratorSet {
        private readonly IDictionary<Type, IList<IFacetDecorator>> facetDecorators = new Dictionary<Type, IList<IFacetDecorator>>();

        public FacetDecoratorSet(IFacetDecorator[] decorators) {
            if (decorators != null) {
                decorators.ForEach(Add);
            }
        }

        // for testing
        public IImmutableDictionary<Type, IList<IFacetDecorator>> FacetDecorators {
            get { return facetDecorators.ToImmutableDictionary(); }
        }

        #region IFacetDecoratorSet Members

        public void DecorateAllHoldersFacets(ISpecification holder) {
            if (facetDecorators.Any()) {
                foreach (Type facetType in holder.FacetTypes) {
                    DecorateFacet(facetType, holder);
                }
            }
        }

        #endregion

        private void Add(IFacetDecorator decorator) {
            foreach (Type type in decorator.ForFacetTypes) {
                if (!facetDecorators.ContainsKey(type)) {
                    facetDecorators[type] = new List<IFacetDecorator>();
                }

                facetDecorators[type].Add(decorator);
            }
        }

        private void DecorateFacet(Type facetType, ISpecification holder) {
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