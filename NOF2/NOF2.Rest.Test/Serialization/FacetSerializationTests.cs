// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedFunctions.Reflector.Facet;
using NOF2.About;
using NOF2.Menu;
using NOF2.Reflector.Facet;
using NOF2.Reflector.Helpers;
using NOF2.Rest.Test.Data.AppLib;
using NOF2.Title;
using static NakedFramework.Metamodel.Test.Serialization.SerializationTestHelpers;
using ActionInvocationFacetViaStaticMethod = NOF2.Reflector.Facet.ActionInvocationFacetViaStaticMethod;

namespace NOF2.Rest.Test.Serialization;

public class NOF2TestSerializationClass {
    public Logical TestProperty { get; set; }
    public void AboutTest(IAbout about) { }

    public void ActionTest(IAbout about) { }

    public static void ActionStaticTest(IAbout about) { }

    public static IMenu Menu() => null;

    public void ActionSave() { }

    public ITitle Title() => null;
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

    public static void TestSerializeMenuFacetViaMethod(Func<MenuFacetViaMethod, MenuFacetViaMethod> roundTripper) {
        var m = typeof(NOF2TestSerializationClass).GetMethod(nameof(NOF2TestSerializationClass.Menu));
        var f = new MenuFacetViaMethod(m, null);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.GetMethod(), dsf.GetMethod());
        Assert.AreEqual(f.GetMethodDelegate().GetType(), dsf.GetMethodDelegate().GetType());
    }

    public static void TestSerializePropertyChoicesViaAboutMethodFacet(Func<PropertyChoicesViaAboutMethodFacet, PropertyChoicesViaAboutMethodFacet> roundTripper) {
        var m = typeof(NOF2TestSerializationClass).GetMethod(nameof(NOF2TestSerializationClass.AboutTest));
        var f = new PropertyChoicesViaAboutMethodFacet(m, null);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.GetMethod(), dsf.GetMethod());
        Assert.AreEqual(f.GetMethodDelegate().GetType(), dsf.GetMethodDelegate().GetType());
    }

    public static void TestSerializePropertySetterFacetViaValueHolder(Func<PropertySetterFacetViaValueHolder<Logical, bool>, PropertySetterFacetViaValueHolder<Logical, bool>> roundTripper) {
        var p = typeof(NOF2TestSerializationClass).GetProperty(nameof(NOF2TestSerializationClass.TestProperty));
        var f = new PropertySetterFacetViaValueHolder<Logical, bool>(p, null);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.GetMethod(), dsf.GetMethod());
        Assert.AreEqual(f.GetMethodDelegate().GetType(), dsf.GetMethodDelegate().GetType());
    }

    public static void TestSerializePropertyValidateViaAboutMethodFacet(Func<PropertyValidateViaAboutMethodFacet, PropertyValidateViaAboutMethodFacet> roundTripper) {
        var m = typeof(NOF2TestSerializationClass).GetMethod(nameof(NOF2TestSerializationClass.AboutTest));
        var f = new PropertyValidateViaAboutMethodFacet(m, AboutHelpers.AboutType.Action, null);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.GetMethod(), dsf.GetMethod());
        Assert.AreEqual(f.GetMethodDelegate().GetType(), dsf.GetMethodDelegate().GetType());
    }

    public static void TestSerializeSaveNullFacet(Func<SaveNullFacet, SaveNullFacet> roundTripper) {
        var f = SaveNullFacet.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    public static void TestSerializeSaveViaActionSaveFacet(Func<SaveViaActionSaveFacet, SaveViaActionSaveFacet> roundTripper) {
        var m = typeof(NOF2TestSerializationClass).GetMethod(nameof(NOF2TestSerializationClass.ActionSave));
        var f = new SaveViaActionSaveFacet(m, null);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.GetMethod(), dsf.GetMethod());
        Assert.AreEqual(f.GetMethodDelegate().GetType(), dsf.GetMethodDelegate().GetType());
    }

    public static void TestSerializeSaveViaActionSaveWithAboutFacet(Func<SaveViaActionSaveWithAboutFacet, SaveViaActionSaveWithAboutFacet> roundTripper) {
        var m1 = typeof(NOF2TestSerializationClass).GetMethod(nameof(NOF2TestSerializationClass.ActionSave));
        var m2 = typeof(NOF2TestSerializationClass).GetMethod(nameof(NOF2TestSerializationClass.AboutTest));
        var f = new SaveViaActionSaveWithAboutFacet(m1, m2, null);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.GetMethod(0), dsf.GetMethod(0));
        Assert.AreEqual(f.GetMethodDelegate(0).GetType(), dsf.GetMethodDelegate(0).GetType());
        Assert.AreEqual(f.GetMethod(1), dsf.GetMethod(1));
        Assert.AreEqual(f.GetMethodDelegate(1).GetType(), dsf.GetMethodDelegate(1).GetType());
    }

    public static void TestSerializeStaticFunctionFacet(Func<StaticFunctionFacet, StaticFunctionFacet> roundTripper) {
        var f = StaticFunctionFacet.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    public static void TestSerializeStaticMenuMethodFacet(Func<StaticMenuMethodFacet, StaticMenuMethodFacet> roundTripper) {
        var f = StaticMenuMethodFacet.Instance;
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
    }

    public static void TestSerializeTitleFacetViaTitleMethod(Func<TitleFacetViaTitleMethod, TitleFacetViaTitleMethod> roundTripper)
    {
        var m = typeof(NOF2TestSerializationClass).GetMethod(nameof(NOF2TestSerializationClass.Title));
        var f = new TitleFacetViaTitleMethod(m, null);
        var dsf = roundTripper(f);

        AssertIFacet(f, dsf);
        Assert.AreEqual(f.GetMethod(), dsf.GetMethod());
        Assert.AreEqual(f.GetMethodDelegate().GetType(), dsf.GetMethodDelegate().GetType());
    }
}