// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Framework;
using NakedFramework.Architecture.Spec;

namespace NakedFramework.Metamodel.Facet;

[Serializable]
public sealed class CreateNewFacet : FacetAbstract, ICreateNewFacet {
    private readonly Type toCreate;

    public CreateNewFacet(Type toCreate) : base() => this.toCreate = toCreate;

    public static Type Type => typeof(ICreateNewFacet);

    public override Type FacetType => Type;

    public string[] OrderedProperties(INakedObjectAdapter adapter, INakedFramework framework) {
        if (framework.MetamodelManager.GetSpecification(toCreate) is IObjectSpec spec) {
            return spec.Properties.Where(IsNotHidden).Select(f => f.Name(adapter)).ToArray();
        }

        return Array.Empty<string>();
    }

    private static bool IsNotHidden(IAssociationSpec spec) => string.IsNullOrWhiteSpace(spec.GetFacet<IHiddenFacet>()?.HidesForState(false));
}