// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Metamodel.Facet {
    public abstract class TypeOfFacetAbstract : FacetAbstract, ITypeOfFacet {
        private readonly bool inferred;

        protected TypeOfFacetAbstract(Type valueType, bool inferred, ISpecification holder, IObjectSpecImmutable spec)
            : base(typeof(ITypeOfFacet), holder) {
            this.inferred = inferred;
            this.valueType = valueType;
            this.valueSpec = spec;
        }

        #region ITypeOfFacet Members

        /// <summary>
        ///     Does <b>not</b> correspond to a member in the <see cref="TypeOfAttribute" />
        ///     annotation (or equiv), but indicates that the information provided
        ///     has been inferred rather than explicitly specified.
        /// </summary>
        public virtual bool IsInferred {
            get { return inferred; }
        }

        #endregion

        #region ISingleClassValueFacet Members

        private readonly Type valueType;

        public virtual Type Value {
            get { return valueType; }
        }

        private readonly IObjectSpecImmutable valueSpec;
        /// <summary>
        ///     The <see cref="IObjectSpec" /> of the <see cref="Value" />
        /// </summary>
        public virtual IObjectSpecImmutable ValueSpec {
            get { return valueSpec; }
        }

        #endregion

        public override string ToString() {
            return "TypeOf [value=" + Value + ",inferred=" + inferred + "]";
        }
    }
}