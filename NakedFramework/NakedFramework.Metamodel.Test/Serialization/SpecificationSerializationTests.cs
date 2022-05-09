using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedFramework.Metamodel.Adapter;
using NakedFramework.Metamodel.Menu;
using NakedFramework.Metamodel.SpecImmutable;
using static NakedFramework.Metamodel.Test.Serialization.SerializationTestHelpers;

namespace NakedFramework.Metamodel.Test.Serialization;

public static class SpecificationSerializationTests {
    public static void TestSerializeIdentifierImpl(Func<IdentifierImpl, IdentifierImpl> roundTripper) {
        var i = new IdentifierImpl("name");
        var dsi = roundTripper(i);

        Assert.AreEqual(i.ClassName, dsi.ClassName);
        Assert.AreEqual(i.IsField, dsi.IsField);
        Assert.AreEqual(i.MemberName, dsi.MemberName);
        AssertArrayAreEqual(i.MemberParameterNames, dsi.MemberParameterNames);
        AssertArrayAreEqual(i.MemberParameterTypeNames, dsi.MemberParameterTypeNames);
    }

    public static void TestSerializeActionParameterSpecImmutable(Func<ActionParameterSpecImmutable, ActionParameterSpecImmutable> roundTripper) {
        var s = new ActionParameterSpecImmutable(null, null);
        var dss = roundTripper(s);

        AssertISpecification(s, dss);
        Assert.AreEqual(s.Specification, dss.Specification);
    }

    public static void TestSerializeActionSpecImmutable(Func<ActionSpecImmutable, ActionSpecImmutable> roundTripper) {
        var s = new ActionSpecImmutable(null, null, null);
        var dss = roundTripper(s);

        AssertISpecification(s, dss);
        Assert.AreEqual(s.OwnerSpec, dss.OwnerSpec);
        AssertArrayAreEqual(s.Parameters, dss.Parameters);
    }

    public static void TestSerializeActionToAssociationSpecAdapter(Func<ActionToAssociationSpecAdapter, ActionToAssociationSpecAdapter> roundTripper) {
        var s = new ActionToAssociationSpecAdapter(new ActionSpecImmutable(null, null, null));
        var dss = roundTripper(s);

        AssertISpecification(s, dss);
        Assert.AreEqual(s.OwnerSpec, dss.OwnerSpec);
    }

    public static void TestSerializeActionToCollectionSpecAdapter(Func<ActionToCollectionSpecAdapter, ActionToCollectionSpecAdapter> roundTripper) {
        var s = new ActionToCollectionSpecAdapter(new ActionSpecImmutable(null, null, null));
        var dss = roundTripper(s);

        AssertISpecification(s, dss);
        Assert.AreEqual(s.OwnerSpec, dss.OwnerSpec);
    }

    public static void TestSerializeOneToManyAssociationSpecImmutable(Func<OneToManyAssociationSpecImmutable, OneToManyAssociationSpecImmutable> roundTripper) {
        var s = new OneToManyAssociationSpecImmutable(null, null, null, null);
        var dss = roundTripper(s);

        AssertISpecification(s, dss);
        Assert.AreEqual(s.OwnerSpec, dss.OwnerSpec);
    }

    public static void TestSerializeOneToOneAssociationSpecImmutable(Func<OneToOneAssociationSpecImmutable, OneToOneAssociationSpecImmutable> roundTripper) {
        var s = new OneToOneAssociationSpecImmutable(null, null, null);
        var dss = roundTripper(s);

        AssertISpecification(s, dss);
        Assert.AreEqual(s.OwnerSpec, dss.OwnerSpec);
    }

    public static void TestSerializeServiceSpecImmutable(Func<ServiceSpecImmutable, ServiceSpecImmutable> roundTripper) {
        var s = new ServiceSpecImmutable(typeof(TestSerializationService), false);
        var dss = roundTripper(s);

        AssertISpecification(s, dss);
        //AssertTypeSpecification(dss, dss);
    }

    public static void TestSerializeObjectSpecImmutable(Func<ObjectSpecImmutable, ObjectSpecImmutable> roundTripper) {
        var s = new ObjectSpecImmutable(typeof(TestSerializationClass), false);
        var dss = roundTripper(s);

        AssertISpecification(s, dss);
        //AssertTypeSpecification(dss, dss);
    }

    public static void TestSerializeMenuImmutable(Func<MenuImmutable, MenuImmutable> roundTripper) {
        var m = new MenuImmutable("name", "id", "grouping", null);
        var dsm = roundTripper(m);

        Assert.AreEqual(m.Name, dsm.Name);
        Assert.AreEqual(m.Id, dsm.Id);
        Assert.AreEqual(m.Grouping, dsm.Grouping);
        Assert.AreEqual(m.MenuItems, dsm.MenuItems);
    }

    public static void TestSerializeMenuAction(Func<MenuAction, MenuAction> roundTripper) {
        var m = new MenuAction(new ActionSpecImmutable(null, null, null), "name");
        var dsm = roundTripper(m);

        Assert.AreEqual(m.Name, dsm.Name);
        Assert.AreEqual(m.Id, dsm.Id);
        Assert.AreEqual(m.Grouping, dsm.Grouping);
        AssertISpecification(m.Action, dsm.Action);
    }
}