// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Architecture.Facet;
using NakedObjects.Capabilities;
using NakedObjects.Metamodel.Utils;

namespace NakedObjects.Metamodel.Facet {
    public class ValueFacetUsingSemanticsProvider<T> : ValueFacetAbstract<T> {
        public ValueFacetUsingSemanticsProvider(IValueSemanticsProvider<T> adapter, IFacet underlyingValueTypeFacet)
            : base(adapter, true, underlyingValueTypeFacet.Specification) {
            // add the adapter in as its own facet (eg StringFacet).
            // This facet is almost certainly superfluous; there is nothing in the
            // viewers that needs to get hold of such a facet, for example.
            FacetUtils.AddFacet(underlyingValueTypeFacet);
        }
    }
}