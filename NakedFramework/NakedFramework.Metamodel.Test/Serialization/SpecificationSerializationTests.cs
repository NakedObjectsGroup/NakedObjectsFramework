using System;
using NakedFramework.Metamodel.SpecImmutable;
using static NakedFramework.Metamodel.Test.Serialization.SerializationTestHelpers;

namespace NakedFramework.Metamodel.Test.Serialization;

public static class SpecificationSerializationTests {
    public static void TestSerializeServiceSpecImmutable(Func<ServiceSpecImmutable, ServiceSpecImmutable> roundTripper) {
        var s = new ServiceSpecImmutable(typeof(TestSerializationService), false);
        var dss = roundTripper(s);

        AssertISpecification(s, dss);
    }
}