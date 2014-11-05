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
using System.Runtime.Serialization;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Meta.Spec {
    /// <summary>
    ///     For base subclasses or, more likely, to help write tests
    /// </summary>
    [Serializable]
    public class Specification : ISpecification, ISpecificationBuilder, ISerializable {
        private ImmutableDictionary<Type, IFacet> facetsByClass = ImmutableDictionary<Type, IFacet>.Empty;

        public Specification() {}

        #region ISpecification Members

        public virtual Type[] FacetTypes {
            get { return facetsByClass.Keys.ToArray(); }
        }

        public virtual IIdentifier Identifier {
            get { return null; }
        }

        public bool ContainsFacet(Type facetType) {
            return GetFacet(facetType) != null;
        }

        public bool ContainsFacet<T>() where T : IFacet {
            return GetFacet(typeof (T)) != null;
        }

        public virtual IFacet GetFacet(Type facetType) {
            return facetsByClass.ContainsKey(facetType) ? facetsByClass[facetType] : null;
        }

        public T GetFacet<T>() where T : IFacet {
            return (T) GetFacet(typeof (T));
        }

        public IEnumerable<IFacet> GetFacets() {
            return facetsByClass.Values;
        }

        #endregion

        #region ISpecificationBuilder Members

        public void AddFacet(IFacet facet) {
            AddFacet(facet.FacetType, facet);
        }

        public void AddFacet(IMultiTypedFacet facet) {
            foreach (Type facetType in facet.FacetTypes) {
                AddFacet(facetType, facet.GetFacet(facetType));
            }
        }

        #endregion

        #region ISerializable

        // The special constructor is used to deserialize values. 
        public Specification(SerializationInfo info, StreamingContext context) {
            var dict = (Dictionary<Type, IFacet>) info.GetValue("facetsByClass", typeof (Dictionary<Type, IFacet>));
            dict.OnDeserialization(this);
            facetsByClass = dict.ToImmutableDictionary();
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context) {
            Dictionary<Type, IFacet> dict = facetsByClass.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            info.AddValue("facetsByClass", dict);
        }

        #endregion

        private void AddFacet(Type facetType, IFacet facet) {
            IFacet existingFacet = GetFacet(facetType);
            if (existingFacet == null || existingFacet.IsNoOp || facet.CanAlwaysReplace) {
                facetsByClass = facetsByClass.SetItem(facetType, facet);
            }
        }
    }
}