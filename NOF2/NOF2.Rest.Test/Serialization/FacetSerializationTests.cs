// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NOF2.About;
using NOF2.Reflector.Facet;
using NOF2.Reflector.Helpers;
using static NakedFramework.Metamodel.Test.Serialization.SerializationTestHelpers;

namespace NOF2.Rest.Test.Serialization;

public class NOF2TestSerializationClass {
    public void AboutTest(IAbout about) { }

    public void ActionTest(IAbout about) { }

    public static void ActionStaticTest(IAbout about) { }
}

public static class FacetSerializationTests {
    public static void TestSerializeActionChoicesViaAboutMethodFacet(Func<ActionChoicesViaAboutMethodFacet, ActionChoicesViaAboutMethodFacet> roundTripper) {
        var m = typeof(NOF2TestSerializationClass).GetMethod(nameof(NOF2TestSerializationClass.AboutTest));
        var f = new ActionChoicesViaAboutMethodFacet(m, 0, null);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.IsMultiple, dsf.IsMultiple);
        Assert.AreEqual(f.GetMethod(), dsf.GetMethod());
        Assert.AreEqual(f.GetMethodDelegate().GetType(), dsf.GetMethodDelegate().GetType());
    }

    public static void TestSerializeActionDefaultsViaAboutMethodFacet(Func<ActionDefaultsViaAboutMethodFacet, ActionDefaultsViaAboutMethodFacet> roundTripper) {
        var m = typeof(NOF2TestSerializationClass).GetMethod(nameof(NOF2TestSerializationClass.AboutTest));
        var f = new ActionDefaultsViaAboutMethodFacet(m, 0, null);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.GetMethod(), dsf.GetMethod());
        Assert.AreEqual(f.GetMethodDelegate().GetType(), dsf.GetMethodDelegate().GetType());
    }

    public static void TestSerializeActionInvocationFacetViaMethod(Func<ActionInvocationFacetViaMethod, ActionInvocationFacetViaMethod> roundTripper) {
        var m = typeof(NOF2TestSerializationClass).GetMethod(nameof(NOF2TestSerializationClass.ActionTest));
        var f = new ActionInvocationFacetViaMethod(m, m.DeclaringType, m.ReturnType, typeof(NOF2TestSerializationClass), true, null);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.OnType, dsf.OnType);
        Assert.AreEqual(f.ReturnType, dsf.ReturnType);
        Assert.AreEqual(f.ElementType, dsf.ElementType);
        Assert.AreEqual(f.IsQueryOnly, dsf.IsQueryOnly);
        Assert.AreEqual(f.GetMethod(), dsf.GetMethod());
        Assert.AreEqual(f.GetMethodDelegate().GetType(), dsf.GetMethodDelegate().GetType());
    }

    public static void TestSerializeActionInvocationFacetViaStaticMethod(Func<ActionInvocationFacetViaStaticMethod, ActionInvocationFacetViaStaticMethod> roundTripper) {
        var m = typeof(NOF2TestSerializationClass).GetMethod(nameof(NOF2TestSerializationClass.ActionStaticTest));
        var f = new ActionInvocationFacetViaStaticMethod(m, m.DeclaringType, m.ReturnType, typeof(NOF2TestSerializationClass), true, null);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.OnType, dsf.OnType);
        Assert.AreEqual(f.ReturnType, dsf.ReturnType);
        Assert.AreEqual(f.ElementType, dsf.ElementType);
        Assert.AreEqual(f.IsQueryOnly, dsf.IsQueryOnly);
        Assert.AreEqual(f.GetMethod(), dsf.GetMethod());
        Assert.AreEqual(f.GetMethodDelegate().GetType(), dsf.GetMethodDelegate().GetType());
    }

    public static void TestSerializeActionValidateViaAboutMethodFacet(Func<ActionValidateViaAboutMethodFacet, ActionValidateViaAboutMethodFacet> roundTripper) {
        var m = typeof(NOF2TestSerializationClass).GetMethod(nameof(NOF2TestSerializationClass.AboutTest));
        var f = new ActionValidateViaAboutMethodFacet(m, AboutHelpers.AboutType.Action, null);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.GetMethod(), dsf.GetMethod());
        Assert.AreEqual(f.GetMethodDelegate().GetType(), dsf.GetMethodDelegate().GetType());
    }

    public static void TestSerializeDescribedAsViaAboutMethodFacet(Func<DescribedAsViaAboutMethodFacet, DescribedAsViaAboutMethodFacet> roundTripper) {
        var m = typeof(NOF2TestSerializationClass).GetMethod(nameof(NOF2TestSerializationClass.AboutTest));
        var f = new DescribedAsViaAboutMethodFacet(m, AboutHelpers.AboutType.Action, null);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.GetMethod(), dsf.GetMethod());
        Assert.AreEqual(f.GetMethodDelegate().GetType(), dsf.GetMethodDelegate().GetType());
    }

    public static void TestSerializeDisableForContextViaAboutMethodFacet(Func<DisableForContextViaAboutMethodFacet, DisableForContextViaAboutMethodFacet> roundTripper) {
        var m = typeof(NOF2TestSerializationClass).GetMethod(nameof(NOF2TestSerializationClass.AboutTest));
        var f = new DisableForContextViaAboutMethodFacet(m, AboutHelpers.AboutType.Action, null);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.GetMethod(), dsf.GetMethod());
        Assert.AreEqual(f.GetMethodDelegate().GetType(), dsf.GetMethodDelegate().GetType());
    }

    public static void TestSerializeHideForContextViaAboutMethodFacet(Func<HideForContextViaAboutMethodFacet, HideForContextViaAboutMethodFacet> roundTripper) {
        var m = typeof(NOF2TestSerializationClass).GetMethod(nameof(NOF2TestSerializationClass.AboutTest));
        var f = new HideForContextViaAboutMethodFacet(m, AboutHelpers.AboutType.Action, null);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.GetMethod(), dsf.GetMethod());
        Assert.AreEqual(f.GetMethodDelegate().GetType(), dsf.GetMethodDelegate().GetType());
    }

    public static void TestSerializeMemberNamedViaAboutMethodFacet(Func<MemberNamedViaAboutMethodFacet, MemberNamedViaAboutMethodFacet> roundTripper) {
        var m = typeof(NOF2TestSerializationClass).GetMethod(nameof(NOF2TestSerializationClass.AboutTest));
        var f = new MemberNamedViaAboutMethodFacet(m, AboutHelpers.AboutType.Action, "name", null);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.GetMethod(), dsf.GetMethod());
        Assert.AreEqual(f.GetMethodDelegate().GetType(), dsf.GetMethodDelegate().GetType());
    }
}