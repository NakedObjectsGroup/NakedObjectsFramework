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
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Spec;
using NakedObjects.Meta.Utils;

namespace NakedObjects.Meta.Spec {
    /// <summary>
    ///     For base subclasses or, more likely, to help write tests
    /// </summary>
    [Serializable]
    public abstract class Specification : ISpecificationBuilder, ISerializable, IDeserializationCallback {
        private IImmutableDictionary<Type, IFacet> facetsByClass = ImmutableDictionary<Type, IFacet>.Empty;
        protected Specification() { }

        #region IDeserializationCallback Members

        public virtual void OnDeserialization(object sender) {
            tempDict.OnDeserialization(sender);
            facetsByClass = tempDict.ToImmutableDictionary();
        }

        #endregion

        #region ISpecificationBuilder Members

        public virtual Type[] FacetTypes => facetsByClass.Keys.ToArray();

        public virtual IIdentifier Identifier => null;

        public bool ContainsFacet(Type facetType) => GetFacet(facetType) != null;

        public bool ContainsFacet<T>() where T : IFacet => GetFacet(typeof(T)) != null;

        public virtual IFacet GetFacet(Type facetType) => facetsByClass.ContainsKey(facetType) ? facetsByClass[facetType] : null;

        public T GetFacet<T>() where T : IFacet => (T) GetFacet(typeof(T));

        public virtual IEnumerable<IFacet> GetFacets() => facetsByClass.Values;

        public virtual void AddFacet(IFacet facet) => AddFacet(facet.FacetType, facet);

        #endregion

        private void AddFacet(Type facetType, IFacet facet) {
            var existingFacet = GetFacet(facetType);

            if (facet.IsNoOp && !existingFacet?.IsNoOp == true) { 
                return;
            }

            if (existingFacet == null || existingFacet.IsNoOp || facet.CanAlwaysReplace) {
                facetsByClass = facetsByClass.SetItem(facetType, facet);
            }
        }

        #region ISerializable

        // temp variables for deserialization 
        private readonly Dictionary<Type, IFacet> tempDict;

        // The special constructor is used to deserialize values. 
        protected Specification(SerializationInfo info, StreamingContext context) => tempDict = info.GetValue<Type, IFacet>("facetsByClass");

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context) => info.AddValue<Type, IFacet>("facetsByClass", facetsByClass);

        #endregion
    }
}