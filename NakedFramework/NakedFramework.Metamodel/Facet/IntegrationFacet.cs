// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Spec;

namespace NakedFramework.Metamodel.Facet;

public class IntegrationFacet : AbstractIntegrationFacet {
    private Action<IMetamodelBuilder> toExecute;
    public IntegrationFacet(ISpecification holder, Action<IMetamodelBuilder> toExecute) : base(holder) => this.toExecute = toExecute;

    public override void Execute(IMetamodelBuilder metamodelBuilder) => toExecute(metamodelBuilder);

    public override void AddAction(Action<IMetamodelBuilder> action) {
        var oldToExecute = toExecute;

        toExecute = mb => {
            oldToExecute(mb);
            action(mb);
        };
    }
}