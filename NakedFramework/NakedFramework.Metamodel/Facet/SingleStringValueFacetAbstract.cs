// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Spec;

namespace NakedFramework.Metamodel.Facet {
    [Serializable]
    public abstract class SingleStringValueFacetAbstract : FacetAbstract, ISingleStringValueFacet {
        private readonly string valueString;

        protected SingleStringValueFacetAbstract(Type facetType, ISpecification holder, string valueString)
            : base(facetType, holder) =>
            this.valueString = valueString;

        #region ISingleStringValueFacet Members

        public virtual string Value => valueString;

        #endregion

        protected override string ToStringValues() => valueString == null ? "null" : $"\"{valueString}\"";
    }
}