// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NakedObjects.Reflector.Facet;

namespace NakedObjects.Reflector.Test.Facet;

[TestClass]
public class ActionInvocationFacetViaMethodTest {
    #region Setup/Teardown

    [TestInitialize]
    public void SetUp() { }

    #endregion

    private static void DelegateActionTest(MethodInfo method) {
        var facet = new ActionInvocationFacetViaMethod(method, null, null, null, false, null);
        var parms = method.GetParameters().Select(p => "astring").Cast<object>().ToArray();
        Assert.IsNotNull(facet.GetMethodDelegate(), method.Name);
        var testObject = new TestDelegateClass();
        facet.GetMethodDelegate().Invoke(testObject, parms);
        Assert.AreEqual(method.Name, testObject.ActionCalled);
    }

    private static void DelegateFuncTest(MethodInfo method) {
        var facet = new ActionInvocationFacetViaMethod(method, null, null, null, false, null);
        Assert.IsNotNull(facet.GetMethodDelegate(), method.Name);
        var parms = method.GetParameters().Select(p => "astring").Cast<object>().ToArray();
        Assert.AreEqual(method.Name, facet.GetMethodDelegate().Invoke(new TestDelegateClass(), parms));
    }

    private static void InvokeActionTest(MethodInfo method) {
        var facet = new ActionInvocationFacetViaMethod(method, null, null, null, false, new Mock<ILogger<ActionInvocationFacetViaMethod>>().Object);
        Assert.IsNull(facet.GetMethodDelegate());
        var parms = method.GetParameters().Select(p => "astring").Cast<object>().ToArray();
        Assert.IsNotNull(facet.GetMethod());
        var testObject = new TestDelegateClass();
        facet.GetMethod().Invoke(testObject, parms);
        Assert.AreEqual(method.Name, testObject.ActionCalled);
    }

    private static void InvokeFuncTest(MethodInfo method) {
        var facet = new ActionInvocationFacetViaMethod(method, null, null, null, false, new Mock<ILogger<ActionInvocationFacetViaMethod>>().Object);
        Assert.IsNull(facet.GetMethodDelegate());
        Assert.IsNotNull(facet.GetMethod());
        var parms = method.GetParameters().Select(p => "astring").Cast<object>().ToArray();
        Assert.AreEqual(method.Name, facet.GetMethod().Invoke(new TestDelegateClass(), parms));
    }

    [TestMethod]
    public void TestDelegateCreation() {
        DelegateActionTest(typeof(TestDelegateClass).GetMethod("Action0"));
        DelegateActionTest(typeof(TestDelegateClass).GetMethod("Action1"));
        DelegateActionTest(typeof(TestDelegateClass).GetMethod("Action2"));
        DelegateActionTest(typeof(TestDelegateClass).GetMethod("Action3"));
        DelegateActionTest(typeof(TestDelegateClass).GetMethod("Action4"));
        DelegateActionTest(typeof(TestDelegateClass).GetMethod("Action5"));
        DelegateActionTest(typeof(TestDelegateClass).GetMethod("Action6"));

        DelegateFuncTest(typeof(TestDelegateClass).GetMethod("Func0"));
        DelegateFuncTest(typeof(TestDelegateClass).GetMethod("Func1"));
        DelegateFuncTest(typeof(TestDelegateClass).GetMethod("Func2"));
        DelegateFuncTest(typeof(TestDelegateClass).GetMethod("Func3"));
        DelegateFuncTest(typeof(TestDelegateClass).GetMethod("Func4"));
        DelegateFuncTest(typeof(TestDelegateClass).GetMethod("Func5"));
        DelegateFuncTest(typeof(TestDelegateClass).GetMethod("Func6"));
    }

    [TestMethod]
    public void TestDelegateNonCreation() {
        InvokeActionTest(typeof(TestDelegateClass).GetMethod("Action7"));
        InvokeFuncTest(typeof(TestDelegateClass).GetMethod("Func7"));
    }

    #region Nested type: TestDelegateClass

    // ReSharper disable UnusedMember.Global
    public class TestDelegateClass {
        public string ActionCalled { get; set; }

        public void Action0() {
            ActionCalled = "Action0";
        }

        public void Action1(string p1) {
            ActionCalled = "Action1";
        }

        public void Action2(string p1, string p2) {
            ActionCalled = "Action2";
        }

        public void Action3(string p1, string p2, string p3) {
            ActionCalled = "Action3";
        }

        public void Action4(string p1, string p2, string p3, string p4) {
            ActionCalled = "Action4";
        }

        public void Action5(string p1, string p2, string p3, string p4, string p5) {
            ActionCalled = "Action5";
        }

        public void Action6(string p1, string p2, string p3, string p4, string p5, string p6) {
            ActionCalled = "Action6";
        }

        public void Action7(string p1, string p2, string p3, string p4, string p5, string p6, string p7) {
            ActionCalled = "Action7";
        }

        public string Func0() => "Func0";

        public string Func1(string p1) => "Func1";

        public string Func2(string p1, string p2) => "Func2";

        public string Func3(string p1, string p2, string p3) => "Func3";

        public string Func4(string p1, string p2, string p3, string p4) => "Func4";

        public string Func5(string p1, string p2, string p3, string p4, string p5) => "Func5";

        public string Func6(string p1, string p2, string p3, string p4, string p5, string p6) => "Func6";

        public string Func7(string p1, string p2, string p3, string p4, string p5, string p6, string p7) => "Func7";
    }
    // ReSharper restore UnusedMember.Global

    #endregion
}