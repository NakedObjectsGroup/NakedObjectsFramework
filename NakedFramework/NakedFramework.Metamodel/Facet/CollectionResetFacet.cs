// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.Reflection;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Spec;
using NakedFramework.Core.Exception;
using NakedFramework.Core.Util;

namespace NakedFramework.Metamodel.Facet {
    [Serializable]
    public sealed class CollectionResetFacet : FacetAbstract, ICollectionResetFacet {
        private readonly PropertyInfo property;

        public CollectionResetFacet(PropertyInfo property, ISpecification holder)
            : base(Type, holder) =>
            this.property = property;

        public static Type Type => typeof(ICollectionResetFacet);

        #region ICollectionResetFacet Members

        public void Reset(INakedObjectAdapter inObjectAdapter) {
            try {
                var collection = (IList) property.GetValue(inObjectAdapter.GetDomainObject(), null);
                collection.Clear();
                property.SetValue(inObjectAdapter.GetDomainObject(), collection, null);
            }
            catch (System.Exception e) {
                throw new ReflectionException($"Failed to get/set property {property.Name} in {inObjectAdapter.Spec.FullName}", e);
            }
        }

        #endregion

        protected override string ToStringValues() => $"property={property}";
    }

    // Copyright (c) Naked Objects Group Ltd.
}