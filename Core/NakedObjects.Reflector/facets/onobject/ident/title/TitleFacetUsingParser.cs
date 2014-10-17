// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Capabilities;
using NakedObjects.Metamodel.Facet;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.Ident.Title {
    public class TitleFacetUsingParser<T> : FacetAbstract, ITitleFacet {
        private readonly IParser<T> parser;

        public TitleFacetUsingParser(IParser<T> parser, ISpecification holder)
            : base(typeof (ITitleFacet), holder) {
            this.parser = parser;
        }

        #region ITitleFacet Members

        public string GetTitle(INakedObject nakedObject) {
            if (nakedObject == null || nakedObject.Object == null) {
                return null;
            }
            return parser.DisplayTitleOf((T) nakedObject.Object);
        }

        public string GetTitleWithMask(string mask, INakedObject nakedObject, INakedObjectManager manager) {
            if (nakedObject == null || nakedObject.Object == null) {
                return null;
            }
            return parser.TitleWithMaskOf(mask, (T) nakedObject.Object);
        }

        #endregion

        protected override string ToStringValues() {
            return parser.ToString();
        }
    }
}