// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Reflection;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Framework;
using NakedFramework.Metamodel.Facet;
using NakedFramework.Metamodel.Serialization;
using NakedFunctions.Reflector.Utils;

namespace NakedFunctions.Reflector.Facet;

[Serializable]
public sealed class ViewModelFacetViaFunctionsConvention : ViewModelFacetAbstract, IMultipleImperativeFacet {
    private readonly MethodSerializationWrapper deriveWrapper;
    private readonly MethodSerializationWrapper populateWrapper;

    public ViewModelFacetViaFunctionsConvention(MethodInfo deriveFunction,
                                                MethodInfo populateFunction,
                                                ILogger<ViewModelFacetViaFunctionsConvention> logger) {
        deriveWrapper = MethodSerializationWrapper.Wrap(deriveFunction, logger);
        populateWrapper = MethodSerializationWrapper.Wrap(populateFunction, logger);
    }

    public int Count => 2;

    public MethodInfo GetMethod(int index) => index switch {
        0 => deriveWrapper.GetMethod(),
        1 => populateWrapper.GetMethod(),
        _ => null
    };

    public Func<object, object[], object> GetMethodDelegate(int index) => index switch {
        0 => deriveWrapper.GetMethodDelegate(),
        1 => populateWrapper.GetMethodDelegate(),
        _ => null
    };

    public override string[] Derive(INakedObjectAdapter nakedObjectAdapter,
                                    INakedFramework framework) =>
        deriveWrapper.Invoke<string[]>(deriveWrapper.GetMethod().GetParameterValues(nakedObjectAdapter, framework));

    public override void Populate(string[] keys,
                                  INakedObjectAdapter nakedObjectAdapter,
                                  INakedFramework framework) {
        var newVm = populateWrapper.Invoke<object>(populateWrapper.GetMethod().GetParameterValues(nakedObjectAdapter, keys, framework));
        nakedObjectAdapter.ReplacePoco(newVm);
    }
}