// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Framework;
using NakedFramework.Architecture.Menu;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Test.Interface;

namespace NakedFramework.Test.TestObjects; 

public class TestObjectFactory : ITestObjectFactory {
    private readonly INakedFramework framework;

    public TestObjectFactory(INakedFramework framework) => this.framework = framework;

    private ITestService CreateTestService(INakedObjectAdapter service) => new TestService(service, this);

    private static ITestValue CreateTestValue(INakedObjectAdapter nakedObjectAdapter) => new TestValue(nakedObjectAdapter);

    #region ITestObjectFactory Members

    public ITestService CreateTestService(object service) {
        var no = framework.NakedObjectManager.GetServiceAdapter(service);
        Assert.IsNotNull(no);
        return CreateTestService(no);
    }

    public ITestMenu CreateTestMenuMain(IMenuImmutable menu) => new TestMenu(menu, this, null);

    public ITestMenu CreateTestMenuForObject(IMenuImmutable menu, ITestHasActions owningObject) => new TestMenu(menu, this, owningObject);

    public ITestMenuItem CreateTestMenuItem(IMenuItemImmutable item, ITestHasActions owningObject) => new TestMenuItem(item, this, owningObject);

    public ITestCollection CreateTestCollection(INakedObjectAdapter instances) => new TestCollection(instances, this, framework.NakedObjectManager);

    public ITestObject CreateTestObject(INakedObjectAdapter nakedObjectAdapter) => new TestObject(framework.LifecycleManager, framework.Persistor, nakedObjectAdapter, this, framework.TransactionManager);

    public ITestNaked CreateTestNaked(INakedObjectAdapter nakedObjectAdapter) {
        if (nakedObjectAdapter is null) {
            return null;
        }

        if (nakedObjectAdapter.Spec.IsParseable) {
            return CreateTestValue(nakedObjectAdapter);
        }

        if (nakedObjectAdapter.Spec.IsObject) {
            return CreateTestObject(nakedObjectAdapter);
        }

        return nakedObjectAdapter.Spec.IsCollection ? CreateTestCollection(nakedObjectAdapter) : null;
    }

    public ITestAction CreateTestAction(IActionSpec actionSpec, ITestHasActions owningObject) => new TestAction(framework, actionSpec, owningObject, this);

    public ITestAction CreateTestAction(IActionSpecImmutable actionSpecImm, ITestHasActions owningObject) {
        var actionSpec = framework.MetamodelManager.GetActionSpec(actionSpecImm);
        return CreateTestAction(actionSpec, owningObject);
    }

    public ITestAction CreateTestActionOnService(IActionSpecImmutable actionSpecImm) {
        var objectIm = actionSpecImm.OwnerSpec; //This is the spec for the service

        if (objectIm is not IServiceSpecImmutable) {
            throw new Exception("Action is not on a known service");
        }

        var serviceSpec = (IServiceSpec) framework.MetamodelManager.GetSpecification(objectIm);
        var service = framework.ServicesManager.GetService(serviceSpec);
        var testService = CreateTestService(service);
        return CreateTestAction(actionSpecImm, testService);
    }

    public ITestProperty CreateTestProperty(IAssociationSpec field, ITestHasActions owningObject) => new TestProperty(field, owningObject, this, framework);

    public ITestParameter CreateTestParameter(IActionSpec actionSpec, IActionParameterSpec parameterSpec, ITestHasActions owningObject) => new TestParameter(parameterSpec, owningObject, this);

    #endregion
}

// Copyright (c) Naked Objects Group Ltd.