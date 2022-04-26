// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedFramework.Metamodel.Test.Serialization;
using NakedFunctions.Reflector.Facet;
using static NakedFramework.Metamodel.Test.Serialization.SerializationTestHelpers;

namespace NakedFunctions.Reflector.Test.Serialization;

public static class FacetSerializationTests {
    public static void TestSerializeActionChoicesFacetViaFunction(Func<ActionChoicesFacetViaFunction, ActionChoicesFacetViaFunction> roundTripper) {
        var m = GetChoicesFunction();
        var f = new ActionChoicesFacetViaFunction(m, m.GetParameters().Select(p => (p.Name, p.ParameterType)).ToArray(), null);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.IsMultiple, dsf.IsMultiple);
        Assert.AreEqual(f.ParameterNamesAndTypes[0], dsf.ParameterNamesAndTypes[0]);
        Assert.AreEqual(f.GetMethod(), dsf.GetMethod());
        Assert.AreEqual(f.GetMethodDelegate().GetType(), dsf.GetMethodDelegate().GetType());
    }

    public static void TestSerializeActionDefaultsFacetViaFunction(Func<ActionDefaultsFacetViaFunction, ActionDefaultsFacetViaFunction> roundTripper) {
        var m = GetDefaultFunction();
        var f = new ActionDefaultsFacetViaFunction(m, null);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.GetMethod(), dsf.GetMethod());
        Assert.AreEqual(f.GetMethodDelegate().GetType(), dsf.GetMethodDelegate().GetType());
    }

    public static void TestSerializeActionInvocationFacetViaStaticMethod(Func<ActionInvocationFacetViaStaticMethod, ActionInvocationFacetViaStaticMethod> roundTripper) {
        var m = GetFunction();
        var f = new ActionInvocationFacetViaStaticMethod(m, m.DeclaringType, m.ReturnType, typeof(TestSerializationFunctions), true, null);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.OnType, dsf.OnType);
        Assert.AreEqual(f.ReturnType, dsf.ReturnType);
        Assert.AreEqual(f.ElementType, dsf.ElementType);
        Assert.AreEqual(f.IsQueryOnly, dsf.IsQueryOnly);
        Assert.AreEqual(f.GetMethod(), dsf.GetMethod());
        Assert.AreEqual(f.GetMethodDelegate().GetType(), dsf.GetMethodDelegate().GetType());
    }

    public static void TestSerializeActionParameterValidationViaFunctionFacet(Func<ActionParameterValidationViaFunctionFacet, ActionParameterValidationViaFunctionFacet> roundTripper) {
        var m = GetFunction();
        var f = new ActionParameterValidationViaFunctionFacet(m, null);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.GetMethod(), dsf.GetMethod());
        Assert.AreEqual(f.GetMethodDelegate().GetType(), dsf.GetMethodDelegate().GetType());
    }

    public static void TestSerializeActionValidationViaFunctionFacet(Func<ActionValidationViaFunctionFacet, ActionValidationViaFunctionFacet> roundTripper) {
        var m = GetFunction();
        var f = new ActionValidationViaFunctionFacet(m, null);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.GetMethod(), dsf.GetMethod());
        Assert.AreEqual(f.GetMethodDelegate().GetType(), dsf.GetMethodDelegate().GetType());
    }

    public static void TestSerializeAutoCompleteViaFunctionFacet(Func<AutoCompleteViaFunctionFacet, AutoCompleteViaFunctionFacet> roundTripper) {
        var m = GetFunction();
        var f = new AutoCompleteViaFunctionFacet(m, 10, 20, null);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.PageSize, dsf.PageSize);
        Assert.AreEqual(f.MinLength, dsf.MinLength);
        Assert.AreEqual(f.GetMethod(), dsf.GetMethod());
        Assert.AreEqual(f.GetMethodDelegate().GetType(), dsf.GetMethodDelegate().GetType());
    }
}