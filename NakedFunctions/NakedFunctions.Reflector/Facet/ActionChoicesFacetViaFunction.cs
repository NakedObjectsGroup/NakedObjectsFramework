// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Framework;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Core.Error;
using NakedFramework.Core.Util;
using NakedFramework.Metamodel.Facet;
using NakedFunctions.Reflector.Utils;

namespace NakedFunctions.Reflector.Facet; 

[Serializable]
public sealed class ActionChoicesFacetViaFunction : ActionChoicesFacetAbstract, IImperativeFacet {
    private readonly Func<object, object[], object> choicesDelegate;
    private readonly MethodInfo choicesMethod;
    private readonly Type choicesType;

    public ActionChoicesFacetViaFunction(MethodInfo choicesMethod,
                                         (string, IObjectSpecImmutable)[] parameterNamesAndTypes,
                                         Type choicesType,
                                         ISpecification holder,
                                         ILogger<ActionChoicesFacetViaFunction> logger,
                                         bool isMultiple = false)
        : base(holder) {
        this.choicesMethod = choicesMethod;
        this.choicesType = choicesType;
        IsMultiple = isMultiple;
        ParameterNamesAndTypes = parameterNamesAndTypes;
        choicesDelegate = LogNull(DelegateUtils.CreateDelegate(choicesMethod), logger);
    }

    public override (string, IObjectSpecImmutable)[] ParameterNamesAndTypes { get; }

    public override bool IsMultiple { get; }

    public override object[] GetChoices(INakedObjectAdapter nakedObjectAdapter,
                                        IDictionary<string, INakedObjectAdapter> parameterNameValues,
                                        INakedFramework framework) {
        try {
            var parms = choicesMethod.GetParameterValues(nakedObjectAdapter, parameterNameValues, framework);
            return choicesDelegate.Invoke<IEnumerable>(choicesMethod, parms).Cast<object>().ToArray();
        }
        catch (ArgumentException ae) {
            throw new InvokeException($"Choices exception: {choicesMethod.Name} has mismatched (ie type of choices parameter does not match type of action parameter) parameter types", ae);
        }
    }

    protected override string ToStringValues() => $"method={choicesMethod},Type={choicesType}";

    [OnDeserialized]
    private static void OnDeserialized(StreamingContext context) { }

    #region IImperativeFacet Members

    public MethodInfo GetMethod() => choicesMethod;

    public Func<object, object[], object> GetMethodDelegate() => choicesDelegate;

    #endregion
}

// Copyright (c) Naked Objects Group Ltd.