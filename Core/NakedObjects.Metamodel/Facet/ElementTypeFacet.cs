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
using NakedObjects.Architecture.SpecImmutable;

namespace NakedObjects.Metamodel.Facet {
    public class ElementTypeFacet : FacetAbstract, IElementTypeFacet {
        private readonly IObjectSpecImmutable spec;
        private readonly Type type;

        public ElementTypeFacet(ISpecification holder, Type type, IObjectSpecImmutable spec) : base(Type, holder) {
            this.type = type;
            this.spec = spec;
        }

        public static Type Type {
            get { return typeof (IElementTypeFacet); }
        }

        #region IElementTypeFacet Members

        public Type Value {
            get { return type; }
        }

        public IObjectSpecImmutable ValueSpec {
            get { return spec; }
        }

        #endregion
    }
}