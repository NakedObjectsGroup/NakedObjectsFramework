﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Spec;
using NakedFramework.Test.Interface;

namespace NakedFramework.Test.TestObjects;

internal class TestParameter : ITestParameter {
    private readonly ITestObjectFactory factory;
    private readonly ITestHasActions owningObject;
    private readonly IActionParameterSpec parameterSpec;

    public TestParameter(IActionParameterSpec parameterSpec, ITestHasActions owningObject, ITestObjectFactory factory) {
        this.parameterSpec = parameterSpec;
        this.owningObject = owningObject;
        this.factory = factory;
    }

    #region ITestParameter Members

    public string Name => parameterSpec.Name(null);

    public INakedObjectAdapter NakedObject => owningObject.NakedObject;

    public string Title => "";

    public ITestNaked[] GetChoices() {
        return parameterSpec.GetChoices(NakedObject, null).Select(x => factory.CreateTestNaked(x)).ToArray();
    }

    public ITestNaked[] GetCompletions(string autoCompleteParm) {
        return parameterSpec.GetCompletions(NakedObject, autoCompleteParm).Select(x => factory.CreateTestNaked(x)).ToArray();
    }

    public ITestNaked GetDefault() {
        var defaultValue = parameterSpec.GetDefault(NakedObject);
        var defaultType = parameterSpec.GetDefaultType(NakedObject);

        if (defaultValue != null && defaultType == TypeOfDefaultValue.Implicit && defaultValue.Object is Enum) {
            defaultValue = null;
        }

        return factory.CreateTestNaked(defaultValue);
    }

    public ITestParameter AssertIsDescribedAs(string description) {
        Assert.IsTrue(parameterSpec.Description(null) == description, $"Parameter: {Name} description: {parameterSpec.Description(null)} expected: {description}");
        return this;
    }

    #endregion
}