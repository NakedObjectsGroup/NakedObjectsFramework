// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Reflect;
using NakedFramework.Architecture.Spec;
using NakedFramework.Core.Util;
using NakedFramework.Metamodel.Utils;

namespace NakedObjects.ParallelReflector.Reflect {
    public sealed class FacetDecoratorSet : IFacetDecoratorSet {
        private readonly IDictionary<Type, IList<IFacetDecorator>> facetDecorators = new Dictionary<Type, IList<IFacetDecorator>>();

        public FacetDecoratorSet(IFacetDecorator[] decorators) => decorators?.ForEach(Add);

        #region IFacetDecoratorSet Members

        public void DecorateAllHoldersFacets(ISpecification holder) {
            if (facetDecorators.Any()) {
                foreach (var facetType in holder.FacetTypes) {
                    DecorateFacet(facetType, holder);
                }
            }
        }

        #endregion

        private void Add(IFacetDecorator decorator) {
            foreach (var type in decorator.ForFacetTypes) {
                if (!facetDecorators.ContainsKey(type)) {
                    facetDecorators[type] = new List<IFacetDecorator>();
                }

                facetDecorators[type].Add(decorator);
            }
        }

        private void DecorateFacet(Type facetType, ISpecification holder) {
            if (facetDecorators.ContainsKey(facetType)) {
                foreach (var decorator in facetDecorators[facetType]) {
                    var previousFacet = holder.GetFacet(facetType);
                    var decoratedFacet = decorator.Decorate(previousFacet, holder);
                    if (decoratedFacet != null && decoratedFacet != previousFacet) {
                        FacetUtils.AddFacet(decoratedFacet);
                    }
                }
            }
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}