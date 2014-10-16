// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects.Reflector.Spec;

namespace NakedObjects.Architecture.Facets.Actcoll.Typeof {
    public abstract class TypeOfFacetAbstract : SingleClassValueFacetAbstract, ITypeOfFacet {
        private readonly bool inferred;

        protected TypeOfFacetAbstract(Type valueType, bool inferred, ISpecification holder, IObjectSpecImmutable spec)
            : base(Type, holder, valueType, spec) {
            this.inferred = inferred;
        }

        public static Type Type {
            get { return typeof (ITypeOfFacet); }
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

        public override string ToString() {
            return "TypeOf [value=" + Value + ",inferred=" + inferred + "]";
        }
    }
}