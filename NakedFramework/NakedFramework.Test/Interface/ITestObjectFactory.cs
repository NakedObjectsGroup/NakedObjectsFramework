// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Menu;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;

namespace NakedFramework.Test.Interface; 

/// <summary>
///     Creates test objects for XAT
///     This interface is not used by XAT2.
/// </summary>
public interface ITestObjectFactory {
    ITestService CreateTestService(object service);
    ITestMenu CreateTestMenuMain(IMenuImmutable menu);
    ITestMenu CreateTestMenuForObject(IMenuImmutable menu, ITestHasActions owningObject);
    ITestMenuItem CreateTestMenuItem(IMenuItemImmutable item, ITestHasActions owningObject);
    ITestCollection CreateTestCollection(INakedObjectAdapter instances);
    ITestObject CreateTestObject(INakedObjectAdapter nakedObjectAdapter);
    ITestNaked CreateTestNaked(INakedObjectAdapter nakedObjectAdapter);
    ITestAction CreateTestActionOnService(IActionSpecImmutable actionSpecImm);
    ITestAction CreateTestAction(IActionSpecImmutable actionSpec, ITestHasActions owningObject);
    ITestAction CreateTestAction(IActionSpec actionSpec, ITestHasActions owningObject);
    ITestParameter CreateTestParameter(IActionSpec actionSpec, IActionParameterSpec parameterSpec, ITestHasActions owningObject);
    ITestProperty CreateTestProperty(IAssociationSpec field, ITestHasActions owningObject);
}

// Copyright (c) Naked Objects Group Ltd.