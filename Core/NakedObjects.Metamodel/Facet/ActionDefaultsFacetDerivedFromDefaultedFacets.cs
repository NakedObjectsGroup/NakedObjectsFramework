// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Metamodel.Facet;

namespace NakedObjects.Metamodel.Facet {
    public class ActionDefaultsFacetDerivedFromDefaultedFacets : ActionDefaultsFacetAbstract {
        private readonly IDefaultedFacet defaultedFacet;

        public ActionDefaultsFacetDerivedFromDefaultedFacets(IDefaultedFacet defaultedFacet, ISpecification holder)
            : base(holder) {
            this.defaultedFacet = defaultedFacet;
        }

        /// <summary>
        ///     Return the defaults.
        /// </summary>
        /// <para>
        ///     We get the defaults fresh each time in case the defaults might
        ///     conceivably change.
        /// </para>
        public override Tuple<object, TypeOfDefaultValue> GetDefault(INakedObject nakedObject) {
            return new Tuple<object, TypeOfDefaultValue>(defaultedFacet == null ? null : defaultedFacet.Default, TypeOfDefaultValue.Implicit);
        }
    }
}