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
    public sealed class OneToOneActionParameter : ActionParameterSpec, IOneToOneActionParameterSpec {
        private bool? isFindMenuEnabled;

        public OneToOneActionParameter(IMetamodelManager metamodel, int index, IActionSpec actionImpl, IActionParameterSpecImmutable actionParameterSpecImmutable, INakedObjectManager manager, ISession session, IObjectPersistor persistor)
            : base(metamodel, index, actionImpl, actionParameterSpecImmutable, manager, session, persistor) { }

        #region IOneToOneActionParameterSpec Members

        public bool IsFindMenuEnabled {
            get {
                isFindMenuEnabled ??= ContainsFacet<IFindMenuFacet>();

                return isFindMenuEnabled.Value;
            }
        }

        #endregion
    }
}