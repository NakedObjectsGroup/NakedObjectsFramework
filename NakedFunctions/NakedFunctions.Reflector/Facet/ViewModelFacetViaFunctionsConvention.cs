// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Reflection;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Framework;
using NakedFramework.Metamodel.Facet;
using NakedFunctions.Reflector.Utils;

namespace NakedFunctions.Reflector.Facet;

[Serializable]
public sealed class ViewModelFacetViaFunctionsConvention : ViewModelFacetAbstract {
    private readonly MethodInfo deriveFunction;
    private readonly MethodInfo populateFunction;

    public ViewModelFacetViaFunctionsConvention(MethodInfo deriveFunction,
                                                MethodInfo populateFunction) {
        this.deriveFunction = deriveFunction;
        this.populateFunction = populateFunction;
    }

    public override Type FacetType => typeof(IViewModelFacet);

    public override string[] Derive(INakedObjectAdapter nakedObjectAdapter,
                                    INakedFramework framework) =>
        deriveFunction.Invoke(null, deriveFunction.GetParameterValues(nakedObjectAdapter, framework)) as string[];

    public override void Populate(string[] keys,
                                  INakedObjectAdapter nakedObjectAdapter,
                                  INakedFramework framework) {
        var newVm = populateFunction.Invoke(null, populateFunction.GetParameterValues(nakedObjectAdapter, keys, framework));
        nakedObjectAdapter.ReplacePoco(newVm);
    }
}