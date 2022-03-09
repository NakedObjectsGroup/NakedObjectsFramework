// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Spec;

namespace NakedFramework.Metamodel.Facet;

public abstract class AbstractIntegrationFacet : FacetAbstract, IIntegrationFacet {
    protected internal AbstractIntegrationFacet() { }

    // for testing so not on interface  
    public int ActionCount { get; protected set; } = 1;

    // also for testing to check ActionCount
    public static bool AllowRemove { get; set; } = true;

    public override Type FacetType => typeof(IIntegrationFacet);

    public abstract void Execute(IMetamodelBuilder metamodelBuilder);
    public abstract void AddAction(Action<IMetamodelBuilder> action);

    public override bool CanNeverBeReplaced => true;

    public void Remove(ISpecificationBuilder specification) {
        if (AllowRemove) {
            specification.RemoveFacet(this);
        }
    }
}