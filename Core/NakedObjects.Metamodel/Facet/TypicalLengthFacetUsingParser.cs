// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Capabilities;

namespace NakedObjects.Meta.Facet {
    [Serializable]
    public class TypicalLengthFacetUsingParser<T> : FacetAbstract, ITypicalLengthFacet {
        private readonly IParser<T> parser;

        public TypicalLengthFacetUsingParser(IParser<T> parser, ISpecification holder)
            : base(typeof (ITypicalLengthFacet), holder) {
            this.parser = parser;
        }

        #region ITypicalLengthFacet Members

        public int Value {
            get { return parser.TypicalLength; }
        }

        #endregion

        protected override string ToStringValues() {
            return parser.ToString();
        }
    }
}