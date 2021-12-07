// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Framework;
using NakedFramework.Core.Adapter;
using NakedFramework.Metamodel.Facet;
using NakedFramework.Metamodel.SemanticsProvider;

namespace NakedObjects.Meta.Test.SemanticsProvider;

public abstract class ValueSemanticsProviderAbstractTestCase<T> {
    private readonly ILogger<NakedObjectAdapter> logger = new Mock<ILogger<NakedObjectAdapter>>().Object;
    private readonly ILoggerFactory loggerFactory = new Mock<ILoggerFactory>().Object;

    protected INakedFramework Framework = new Mock<INakedFramework>().Object;
    protected ILifecycleManager LifecycleManager = new Mock<ILifecycleManager>().Object;
    protected INakedObjectManager Manager = new Mock<INakedObjectManager>().Object;
    protected IMetamodelManager Metamodel = new Mock<IMetamodelManager>().Object;
    protected IObjectPersistor Persistor = new Mock<IObjectPersistor>().Object;
    protected IReflector Reflector = new Mock<IReflector>().Object;
    private IValueSemanticsProvider<T> value;

    protected void SetValue(IValueSemanticsProvider<T> newValue) {
        value = newValue;
    }

    protected IValueSemanticsProvider<T> GetValue() => value;

    public virtual void SetUp() { }

    public virtual void TearDown() {
        value = null;
    }

    protected INakedObjectAdapter CreateAdapter(object obj) {
        var session = new Mock<ISession>().Object;
        return new NakedObjectAdapter(obj, null, Framework, loggerFactory, logger);
    }

    public virtual void TestParseNull() {
        try {
            value.ParseTextEntry(null);
            Assert.Fail();
        }
        catch (ArgumentException /*expected*/) { }
    }

    public virtual void TestParseEmptyString() {
        var newValue = value.ParseTextEntry("");
        Assert.IsNull(newValue);
    }


    protected static INakedObjectAdapter MockAdapter(object obj) {
        var mockParm = new Mock<INakedObjectAdapter>();
        mockParm.Setup(p => p.Object).Returns(obj);
        return mockParm.Object;
    }

    protected static Mock<INakedObjectManager> MockNakedObjectManager() {
        var mgr = new Mock<INakedObjectManager>();
        mgr.Setup(mm => mm.CreateAdapter(It.IsAny<object>(), null, null)).Returns<object, IOid, IVersion>((obj, oid, ver) => MockAdapter(obj));
        return mgr;
    }
}