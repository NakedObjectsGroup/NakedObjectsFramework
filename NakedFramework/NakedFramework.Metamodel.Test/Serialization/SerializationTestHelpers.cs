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
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Spec;
using NakedFramework.Menu;

#pragma warning disable CS0618
#pragma warning disable SYSLIB0011

namespace NakedFramework.Metamodel.Test.Serialization;

public static class SerializationTestHelpers {
    public static void AssertIFacet(IFacet facet, IFacet facet1) {
        Assert.AreNotEqual(facet, facet1);
        Assert.AreEqual(facet.GetType(), facet1.GetType());
        Assert.AreEqual(facet.IsNoOp, facet1.IsNoOp);
        Assert.AreEqual(facet.FacetType, facet1.FacetType);
        Assert.AreEqual(facet.CanAlwaysReplace, facet1.CanAlwaysReplace);
        Assert.AreEqual(facet.CanNeverBeReplaced, facet1.CanNeverBeReplaced);
    }

    public static void AssertISpecification(ISpecification spec, ISpecification spec1) {
        Assert.AreNotEqual(spec, spec1);
        Assert.AreEqual(spec.Identifier, spec1.Identifier);
        Assert.AreEqual(spec.FacetTypes, spec1.FacetTypes);
    }

    private static Stream BinarySerialize(object graph) {
        Stream memoryStream = new MemoryStream();
        var serializer = new BinaryFormatter();
        serializer.Serialize(memoryStream, graph);
        memoryStream.Position = 0;
        return memoryStream;
    }

    private static IFacet BinaryDeserialize(Stream stream) {
        var deserializer = new BinaryFormatter();
        return (IFacet)deserializer.Deserialize(stream);
    }

    public static T BinaryRoundTrip<T>(T facet) where T : IFacet {
        using var stream = BinarySerialize(facet);
        return (T)BinaryDeserialize(stream);
    }

    public static T BinaryRoundTripSpec<T>(T spec) where T : ISpecification {
        using var stream = BinarySerialize(spec);
        return (T)BinaryDeserialize(stream);
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