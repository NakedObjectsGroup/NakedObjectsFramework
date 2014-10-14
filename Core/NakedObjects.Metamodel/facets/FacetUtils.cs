// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using NakedObjects.Architecture.Adapter;

namespace NakedObjects.Architecture.Facets {
    public static class FacetUtils {
        /// <summary>
        ///     Attaches the <see cref="IFacet" /> to its <see cref="IFacet.FacetHolder" />
        /// </summary>
        /// <returns>
        ///     <c>true</c> if a non-<c>null</c> facet was added, <c>false</c> otherwise.
        /// </returns>
        public static bool AddFacet(IFacet facet) {
            if (facet != null) {
                facet.FacetHolder.AddFacet(facet);
                return true;
            }
            return false;
        }

        public static bool AddFacet(IMultiTypedFacet facet) {
            if (facet != null) {
                facet.FacetHolder.AddFacet(facet);
                return true;
            }
            return false;
        }

        /// <summary>
        ///     Attaches each <see cref="IFacet" /> to its <see cref="IFacet.FacetHolder" />
        /// </summary>
        /// <returns>
        ///     <c>true</c> if any facets were added, <c>false</c> otherwise.
        /// </returns>
        public static bool AddFacets(IFacet[] facets) {
            bool addedFacets = false;
            foreach (IFacet facet in facets) {
                addedFacets |= AddFacet(facet);
            }
            return addedFacets;
        }

        /// <summary>
        ///     Attaches each <see cref="IFacet" /> to its <see cref="IFacet.FacetHolder" />
        /// </summary>
        /// <returns>
        ///     <c>true</c> if any facets were added, <c>false</c> otherwise.
        /// </returns>
        public static bool AddFacets(IList<IFacet> facetList) {
            bool addedFacets = false;
            foreach (IFacet facet in facetList) {
                addedFacets |= AddFacet(facet);
            }
            return addedFacets;
        }

        /// <summary>
        ///     Bit nasty, for use only by <see cref="IFacetHolder" />s that index their <see cref="IFacet" />s
        ///     in a <see cref="Dictionary{TKey,TValue}" />.
        /// </summary>
        public static Type[] GetFacetTypes(Dictionary<Type, IFacet> facetsByClass) {
            return new List<Type>(facetsByClass.Keys).ToArray();
        }

     

        public static IFacet[] GetFacets(IFacet facet, IFacetFilter filter) {
            if (filter.Accept(facet)) {
                return new[] {facet};
            }
            return new IFacet[] {};
        }

        public static void RemoveFacet(IDictionary<Type, IFacet> facetsByClass, IFacet facet) {
            RemoveFacet(facetsByClass, facet.FacetType);
        }

        public static IFacet GetFacet(IDictionary<Type, IFacet> facetsByClass, Type facetType) {
            if (facetsByClass.ContainsKey(facetType)) {
                return facetsByClass[facetType];
            }
            return null;
        }

        public static void RemoveFacet(IDictionary<Type, IFacet> facetsByClass, Type facetType) {
            if (facetsByClass.ContainsKey(facetType)) {
                IFacet facet = facetsByClass[facetType];
                facetsByClass.Remove(facetType);
                facet.FacetHolder = null;
            }
        }

        public static void AddFacet(IDictionary<Type, IFacet> facetsByClass, IFacet facet) {
            facetsByClass[facet.FacetType] = facet;
        }

        public static IFacet[] ToArray(IList<IFacet> facetList) {
            if (facetList == null) {
                return new IFacet[0];
            }
            return new List<IFacet>(facetList).ToArray();
        }

        public static INakedObject[] MatchParameters(string[] parameterNames, IDictionary<string, INakedObject> parameterNameValues) {
            var parmValues = new List<INakedObject>();

            foreach (string name in parameterNames) {
                if (parameterNameValues != null && parameterNameValues.ContainsKey(name)) {
                    parmValues.Add(parameterNameValues[name]);
                }
                else {
                    parmValues.Add(null);
                }
            }

            return parmValues.ToArray();
        }

        public static bool IsNotANoopFacet(IFacet facet) {
            return facet != null && !facet.IsNoOp;
        }
    }
}