// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Menu;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;

namespace NakedObjects.Xat {
    /// <summary>
    ///     Creates test objects for XAT
    ///     This interface is not used by XAT2.
    /// </summary>
    public interface ITestObjectFactory {
        ISession Session { get; set; }
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
        ITestAction CreateTestAction(string contributor, IActionSpec actionSpec, ITestHasActions owningObject);
        ITestProperty CreateTestProperty(IAssociationSpec field, ITestHasActions owningObject);
    }

    // Copyright (c) Naked Objects Group Ltd.
}