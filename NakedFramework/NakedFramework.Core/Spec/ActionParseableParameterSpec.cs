// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;

namespace NakedObjects.Core.Spec {
    public sealed class ActionParseableParameterSpec : ActionParameterSpec, IActionParseableParameterSpec {
        // cached values 
        private int? maximumLength;
        private int? noLines;
        private int? typicalLineLength;

        public ActionParseableParameterSpec(int index, IActionSpec actionSpec, IActionParameterSpecImmutable actionParameterSpecImmutable, INakedObjectsFramework framework)
            : base(index, actionSpec, actionParameterSpecImmutable, framework) { }

        #region IActionParseableParameterSpec Members

        public int NoLines {
            get {
                noLines ??= GetFacet<IMultiLineFacet>().NumberOfLines;
                return noLines.Value;
            }
        }

        public int MaximumLength {
            get {
                maximumLength ??= GetFacet<IMaxLengthFacet>().Value;
                return maximumLength.Value;
            }
        }

        public int TypicalLineLength {
            get {
                typicalLineLength ??= GetFacet<ITypicalLengthFacet>().Value;
                return typicalLineLength.Value;
            }
        }

        public bool IsFindMenuEnabled => false;

        #endregion
    }
}