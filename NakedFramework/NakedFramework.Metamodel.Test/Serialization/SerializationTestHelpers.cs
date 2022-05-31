// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Menu;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Menu;
using NakedFramework.Metamodel.Adapter;
using NakedFramework.Metamodel.Facet;
using NakedFramework.Metamodel.Menu;
using NakedFramework.Metamodel.SemanticsProvider;
using NakedFramework.Metamodel.SpecImmutable;
using NakedObjects.Reflector.Facet;

#pragma warning disable CS0618
#pragma warning disable SYSLIB0011

namespace NakedFramework.Metamodel.Test.Serialization;

public static class SerializationTestHelpers {
    public static HashSet<ISpecification> RecurseCheck = new();

    private static Type[] KnownTypes => new[] {
        typeof(IdentifierImpl),
        typeof(BooleanValueSemanticsProvider),
        typeof(ImageValueSemanticsProvider),
        typeof(ActionInvocationFacetViaMethod),
        typeof(PersistedCallbackFacetNull),
        typeof(UpdatedCallbackFacetNull),
        typeof(UpdatedCallbackFacetViaMethod),
        typeof(PropertySetterFacetViaSetterMethod),
        typeof(ActionSpecImmutable)
    };

    public static void AssertIFacet(IFacet facet, IFacet facet1) {
        Assert.AreNotEqual(facet, facet1);
        Assert.AreEqual(facet.GetType(), facet1.GetType());
        Assert.AreEqual(facet.IsNoOp, facet1.IsNoOp);
        Assert.AreEqual(facet.FacetType, facet1.FacetType);
        Assert.AreEqual(facet.CanAlwaysReplace, facet1.CanAlwaysReplace);
        Assert.AreEqual(facet.CanNeverBeReplaced, facet1.CanNeverBeReplaced);
    }

    public static void AssertArrayAreEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual) {
        if (expected is null && actual is null) {
            return;
        }

        Assert.AreEqual(expected.Count(), actual.Count());
        var zipped = expected.Zip(actual);

        foreach (var (first, second) in zipped) {
            switch (first) {
                case ISpecification s1 when second is ISpecification s2:
                    AssertSpecification(s1, s2);
                    break;
                case IMenuItemImmutable m1 when second is IMenuItemImmutable m2:
                    AssertIMenuItem(m1, m2);
                    break;
                default:
                    Assert.AreEqual(first, second);
                    break;
            }
        }
    }

    public static void AssertSpecification(ISpecification s1, ISpecification s2) {
        if (s1 is null && s2 is null) {
            return;
        }

        if (RecurseCheck.Contains(s1) || RecurseCheck.Contains(s2)) {
            return;
        }

        RecurseCheck.Add(s1);
        RecurseCheck.Add(s2);

        Assert.AreEqual(s1.GetType(), s2.GetType());
        AssertISpecification(s1, s2);

        switch (s1) {
            case ITypeSpecImmutable t1 when s2 is ITypeSpecImmutable t2:
                AssertTypeSpecification(t1, t2);
                break;
            case ActionParameterSpecImmutable ap1 when s2 is ActionParameterSpecImmutable ap2:
                AssertActionParameterSpec(ap1, ap2);
                break;
            case ActionSpecImmutable a1 when s2 is ActionSpecImmutable a2:
                AssertActionSpec(a1, a2);
                break;
            case AbstractSpecAdapter ad1 when s2 is AbstractSpecAdapter ad2:
                AssertActionSpecAdapter(ad1, ad2);
                break;
            case AssociationSpecImmutable as1 when s2 is AssociationSpecImmutable as2:
                AssertAssociationSpec(as1, as2);
                break;
        }
    }

    public static void AssertIMenuItem(IMenuItemImmutable s1, IMenuItemImmutable s2) {
        if (s1 is null && s2 is null) {
            return;
        }

        Assert.AreEqual(s1.GetType(), s2.GetType());

        switch (s1) {
            case MenuImmutable t1 when s2 is MenuImmutable t2:
                AssertMenu(t1, t2);
                break;
            case MenuAction ap1 when s2 is MenuAction ap2:
                AssertMenuAction(ap1, ap2);
                break;
        }
    }

    public static void AssertISpecification(ISpecification spec, ISpecification spec1) {
        Assert.AreNotEqual(spec, spec1);
        Assert.AreEqual(spec.Identifier, spec1.Identifier);
        AssertArrayAreEqual(spec.FacetTypes, spec1.FacetTypes);
    }

    public static void AssertTypeSpecification(ITypeSpecImmutable spec, ITypeSpecImmutable spec1) {
        Assert.AreEqual(spec.Type, spec1.Type);
        Assert.AreEqual(spec.FullName, spec1.FullName);
        Assert.AreEqual(spec.ShortName, spec1.ShortName);
        AssertIMenuItem(spec.ObjectMenu, spec1.ObjectMenu);
        AssertArrayAreEqual(spec.OrderedObjectActions, spec1.OrderedObjectActions);
        AssertArrayAreEqual(spec.OrderedContributedActions, spec1.OrderedContributedActions);
        AssertArrayAreEqual(spec.OrderedCollectionContributedActions, spec1.OrderedCollectionContributedActions);
        AssertArrayAreEqual(spec.OrderedFinderActions, spec1.OrderedFinderActions);
        AssertArrayAreEqual(spec.Interfaces, spec1.Interfaces);
        AssertArrayAreEqual(spec.Subclasses, spec1.Subclasses);
        Assert.AreEqual(spec.IsObject, spec1.IsObject);
        Assert.AreEqual(spec.IsCollection, spec1.IsCollection);
        Assert.AreEqual(spec.IsQueryable, spec1.IsQueryable);
        Assert.AreEqual(spec.IsParseable, spec1.IsParseable);
    }

    public static void AssertActionParameterSpec(ActionParameterSpecImmutable s, ActionParameterSpecImmutable dss) {
        AssertISpecification(s, dss);
        AssertSpecification(s.Specification, dss.Specification);
    }

    public static void AssertActionSpec(ActionSpecImmutable s, ActionSpecImmutable dss) {
        AssertISpecification(s, dss);
        AssertSpecification(s.OwnerSpec, dss.OwnerSpec);
        AssertArrayAreEqual(s.Parameters, dss.Parameters);
    }

    public static void AssertActionSpecAdapter(AbstractSpecAdapter s, AbstractSpecAdapter dss) {
        AssertISpecification(s, dss);
        AssertSpecification(s.OwnerSpec, dss.OwnerSpec);
    }

    public static void AssertAssociationSpec(AssociationSpecImmutable s, AssociationSpecImmutable dss) {
        AssertISpecification(s, dss);
        AssertSpecification(s.OwnerSpec, dss.OwnerSpec);
    }

    private static void AssertMenuItem(IMenuItemImmutable m, IMenuItemImmutable dsm) {
        Assert.AreEqual(m.Name, dsm.Name);
        Assert.AreEqual(m.Id, dsm.Id);
        Assert.AreEqual(m.Grouping, dsm.Grouping);
    }

    public static void AssertMenu(IMenuImmutable m, IMenuImmutable dsm) {
        AssertMenuItem(m, dsm);
        AssertArrayAreEqual(m.MenuItems, dsm.MenuItems);
    }

    public static void AssertMenuAction(MenuAction m, MenuAction dsm) {
        AssertMenuItem(m, dsm);
        AssertSpecification(m.Action, dsm.Action);
    }

    private static Stream BinarySerialize(object graph) {
        Stream memoryStream = new MemoryStream();
        var serializer = new BinaryFormatter();
        serializer.Serialize(memoryStream, graph);
        memoryStream.Position = 0;
        return memoryStream;
    }

    private static object BinaryDeserialize(Stream stream) {
        var deserializer = new BinaryFormatter();
        return deserializer.Deserialize(stream);
    }

    public static T BinaryRoundTrip<T>(T facet) where T : IFacet {
        using var stream = BinarySerialize(facet);
        return (T)BinaryDeserialize(stream);
    }

    public static T BinaryRoundTripSpec<T>(T spec) where T : ISpecification {
        using var stream = BinarySerialize(spec);
        return (T)BinaryDeserialize(stream);
    }

    public static T BinaryRoundTripId<T>(T id) where T : IIdentifier {
        using var stream = BinarySerialize(id);
        return (T)BinaryDeserialize(stream);
    }

    public static T BinaryRoundTripMenu<T>(T menu) where T : IMenuItemImmutable {
        using var stream = BinarySerialize(menu);
        return (T)BinaryDeserialize(stream);
    }

    public static T BinaryRoundTripCache<T>(T cache) where T : ISpecificationCache {
        using var stream = BinarySerialize(cache);
        return (T)BinaryDeserialize(stream);
    }

    private static Stream XmlSerialize(object graph) {
        Stream memoryStream = new MemoryStream();
        var serializer = new DataContractSerializer(graph.GetType(), KnownTypes);
        serializer.WriteObject(memoryStream, graph);
        memoryStream.Position = 0;
        return memoryStream;
    }

    private static T XmlDeserialize<T>(Stream stream) {
        var deserializer = new DataContractSerializer(typeof(T), KnownTypes);
        var reader = XmlDictionaryReader.CreateTextReader(stream, new XmlDictionaryReaderQuotas());
        return (T)deserializer.ReadObject(reader, true);
    }

    public static T XmlRoundTrip<T>(T facet) where T : IFacet {
        using var stream = XmlSerialize(facet);
        return XmlDeserialize<T>(stream);
    }

    public static T XmlRoundTripSpec<T>(T spec) where T : ISpecification {
        using var stream = XmlSerialize(spec);
        return XmlDeserialize<T>(stream);
    }

    public static T XmlRoundTripId<T>(T id) where T : IIdentifier {
        using var stream = XmlSerialize(id);
        return XmlDeserialize<T>(stream);
    }

    public static T XmlRoundTripMenu<T>(T menu) where T : IMenuItemImmutable {
        using var stream = XmlSerialize(menu);
        return XmlDeserialize<T>(stream);
    }

    public static T XmlRoundTripCache<T>(T cache) where T : ISpecificationCache {
        using var stream = XmlSerialize(cache);
        return XmlDeserialize<T>(stream);
    }

    public static PropertyInfo GetProperty() => typeof(TestSerializationClass).GetProperty(nameof(TestSerializationClass.TestProperty));

    public static MethodInfo GetMenuMethod() => typeof(TestMenuClass).GetMethod(nameof(TestMenuClass.Menu));

    public static MethodInfo GetChoicesMethod() => typeof(TestSerializationClass).GetMethod(nameof(TestSerializationClass.ChoicesTest));

    public static MethodInfo GetDefaultMethod() => typeof(TestSerializationClass).GetMethod(nameof(TestSerializationClass.DefaultTest));

    public static MethodInfo GetMethod() => typeof(TestSerializationClass).GetMethod(nameof(TestSerializationClass.InvokeTest));

    public static MethodInfo GetCallbackMethod() => typeof(TestSerializationClass).GetMethod(nameof(TestSerializationClass.CallbackMethod));

    public static MethodInfo GetChoicesFunction() => typeof(TestSerializationFunctions).GetMethod(nameof(TestSerializationFunctions.ChoicesTest));

    public static MethodInfo GetDefaultFunction() => typeof(TestSerializationFunctions).GetMethod(nameof(TestSerializationFunctions.DefaultTest));

    public static MethodInfo GetFunction() => typeof(TestSerializationFunctions).GetMethod(nameof(TestSerializationFunctions.InvokeTest));

    public static MethodInfo GetDerive() => typeof(TestSerializationFunctions).GetMethod(nameof(TestSerializationFunctions.Derive));

    public static MethodInfo GetPopulate() => typeof(TestSerializationFunctions).GetMethod(nameof(TestSerializationFunctions.Populate));
}

public class TestSerializationService { }

public class TestSerializationClass {
    public int TestProperty { get; set; } = 1;

    public string ToString(string mask) => ToString();

    public string ValidateTest(string arg) => null;

    public IList<object> ChoicesTest(string arg, string arg1) => null;

    public object DefaultTest() => null;

    public IQueryable<TestSerializationClass> InvokeTest(string arg) => null;

    public void CallbackMethod() { }
}

public static class TestSerializationFunctions {
    public static string ValidateTest(string arg) => null;

    public static IList<object> ChoicesTest(string arg, string arg1) => null;

    public static object DefaultTest() => null;

    public static IQueryable<TestSerializationClass> InvokeTest(string arg) => null;

    public static string[] Derive() => Array.Empty<string>();

    public static object Populate(string[] keys) => null;
}

public class TestMenuClass {
    public static void Menu(IMenu menu) { }
}

public enum TestEnum {
    EOne,
    ETwo
}