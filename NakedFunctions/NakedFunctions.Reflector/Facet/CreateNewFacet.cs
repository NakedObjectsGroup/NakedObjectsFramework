// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using NakedObjects;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Meta.Facet;

namespace NakedFunctions.Reflector.Facet {
    [Serializable]
    public sealed class CreateNewFacet : FacetAbstract, ICreateNewFacet {
        private readonly Type toCreate;

        public CreateNewFacet(Type toCreate, ISpecification holder) : base(Type, holder) => this.toCreate = toCreate;

        public static Type Type => typeof(ICreateNewFacet);

        public string[] OrderedProperties(INakedObjectsFramework framework) {
            if (framework.MetamodelManager.GetSpecification(toCreate) is IObjectSpec spec) {
                return spec.Properties.Where(IsNotHidden).Select(f => f.Name).ToArray();
            }

            return Array.Empty<string>();
        }

        private static bool IsNotHidden(IAssociationSpec spec) => string.IsNullOrWhiteSpace(spec.GetFacet<IHiddenFacet>()?.HidesForState(false));
    }
}