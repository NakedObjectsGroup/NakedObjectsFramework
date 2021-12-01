// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Reflection;
using System.Runtime.Serialization;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Framework;
using NakedFramework.Architecture.Spec;
using NakedFramework.Core.Util;
using NakedFramework.Metamodel.Facet;
using NakedFunctions.Reflector.Utils;

namespace NakedFunctions.Reflector.Facet; 

[Serializable]
public sealed class ActionDefaultsFacetViaFunction : ActionDefaultsFacetAbstract, IImperativeFacet {
    private readonly MethodInfo method;
    private readonly Func<object, object[], object> methodDelegate;

    public ActionDefaultsFacetViaFunction(MethodInfo method, ISpecification holder, ILogger<ActionDefaultsFacetViaFunction> logger)
        : base(holder) {
        this.method = method;
        methodDelegate = LogNull(DelegateUtils.CreateDelegate(method), logger);
    }

    public override (object, TypeOfDefaultValue) GetDefault(INakedObjectAdapter nakedObjectAdapter, INakedFramework framework) {
        // type safety is given by the reflector only identifying methods that match the 
        // parameter type
        var defaultValue = methodDelegate.Invoke<object>(method, method.GetParameterValues(nakedObjectAdapter, framework));
        return (defaultValue, TypeOfDefaultValue.Explicit);
    }

    protected override string ToStringValues() => $"method={method}";

    [OnDeserialized]
    private static void OnDeserialized(StreamingContext context) { }

    #region IImperativeFacet Members

    public MethodInfo GetMethod() => method;

    public Func<object, object[], object> GetMethodDelegate() => methodDelegate;

    #endregion
}

// Copyright (c) Naked Objects Group Ltd.