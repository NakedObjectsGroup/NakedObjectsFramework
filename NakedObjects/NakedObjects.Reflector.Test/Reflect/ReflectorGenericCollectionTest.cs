// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Metamodel.Facet;
using NakedFramework.ParallelReflector.Component;
using NakedObjects.Reflector.Component;

namespace NakedObjects.Reflector.Test.Reflect;

[TestClass]
public class ReflectorGenericCollectionTest : ObjectReflectorTest {
    protected override (ITypeSpecBuilder, IImmutableDictionary<string, ITypeSpecBuilder>) LoadSpecification(IReflector reflector) {
        var objectReflector = (ObjectReflector)reflector;
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();
        (_, metamodel) = reflector.LoadSpecification(typeof(List<TestPoco>), metamodel);
        return ((AbstractParallelReflector)reflector).IntrospectSpecification(typeof(List<TestPoco>), metamodel);
    }

    [TestMethod]
    public void TestDescriptionFaced() {
        var facet = Specification.GetFacet(typeof(IDescribedAsFacet));
        Assert.IsNotNull(facet);
        AssertIsInstanceOfType<DescribedAsFacetNone>(facet);
    }

    [TestMethod]
    public void TestElementTypeFacet() {
        var facet = (IElementTypeFacet)Specification.GetFacet(typeof(IElementTypeFacet));
        Assert.IsNull(facet);
    }

    [TestMethod]
    public void TestFacets() {
        Assert.AreEqual(20, Specification.FacetTypes.Length);
    }

    [TestMethod]
    public void TestName() {
        Assert.AreEqual("System.Collections.Generic.List`1", Specification.FullName);
    }

    [TestMethod]
    public void TestNamedFaced() {
        var facet = Specification.GetFacet(typeof(INamedFacet));
        Assert.IsNotNull(facet);
        AssertIsInstanceOfType<NamedFacetInferred>(facet);
    }

    [TestMethod]
    public void TestPluralFaced() {
        var facet = Specification.GetFacet(typeof(IPluralFacet));
        Assert.IsNotNull(facet);
        AssertIsInstanceOfType<PluralFacetInferred>(facet);
    }
}

[TestClass]
public class SystemTypeReflectorGenericCollectionTest : SystemTypeReflectorTest {
    protected override (ITypeSpecBuilder, IImmutableDictionary<string, ITypeSpecBuilder>) LoadSpecification(IReflector reflector) {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();
        (_, metamodel) = reflector.LoadSpecification(typeof(List<>), metamodel);
        return ((AbstractParallelReflector)reflector).IntrospectSpecification(typeof(List<>), metamodel);
    }

    [TestMethod]
    public void TestCollectionFacet() {
        var facet = Specification.GetFacet(typeof(ICollectionFacet));
        Assert.IsNotNull(facet);
        AssertIsInstanceOfType<GenericCollectionFacet>(facet);
    }

    [TestMethod]
    public void TestElementTypeFacet() {
        var facet = (IElementTypeFacet)Specification.GetFacet(typeof(IElementTypeFacet));
        Assert.IsNull(facet);
    }

    [TestMethod]
    public void TestFacets() {
        Assert.AreEqual(2, Specification.FacetTypes.Length);
    }

    [TestMethod]
    public void TestType() {
        Assert.IsTrue(Specification.IsCollection);
    }

    [TestMethod]
    public void TestTypeOfFacet() {
        var facet = (ITypeOfFacet)Specification.GetFacet(typeof(ITypeOfFacet));
        Assert.IsNotNull(facet);
        AssertIsInstanceOfType<TypeOfFacetInferredFromGenerics>(facet);
    }
}

// Copyright (c) Naked Objects Group Ltd.