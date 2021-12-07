// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Reflect;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.ParallelReflector.FacetFactory;
using NakedFramework.ParallelReflector.TypeFacetFactory;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace NakedObjects.Reflector.Test.FacetFactory;

// Reflector place holder for type marker facet factory tests !!!
[TestClass]
public class TypeMarkerFacetFactoryTest : AbstractFacetFactoryTest {
    private TypeMarkerFacetFactory facetFactory;

    protected override Type[] SupportedTypes => new[] {
        typeof(ITypeIsAbstractFacet),
        typeof(ITypeIsVoidFacet),
        typeof(ITypeIsInterfaceFacet),
        typeof(ITypeIsSealedFacet),
        typeof(ITypeIsStaticFacet)
    };

    protected override IFacetFactory FacetFactory => facetFactory;

    [TestMethod]
    public override void TestFeatureTypes() {
        var featureTypes = facetFactory.FeatureTypes;
        Assert.IsTrue(featureTypes.HasFlag(FeatureType.Objects));
        Assert.IsFalse(featureTypes.HasFlag(FeatureType.Properties));
        Assert.IsFalse(featureTypes.HasFlag(FeatureType.Collections));
        Assert.IsFalse(featureTypes.HasFlag(FeatureType.Actions));
        Assert.IsFalse(featureTypes.HasFlag(FeatureType.ActionParameters));
    }

    private void AssertAbstract(bool isAbstract) {
        var facet = Specification.GetFacet<ITypeIsAbstractFacet>();
        Assert.IsNotNull(facet);
        Assert.AreEqual(isAbstract, facet.Flag);
    }

    private void AssertVoid(bool isVoid) {
        var facet = Specification.GetFacet<ITypeIsVoidFacet>();
        Assert.IsNotNull(facet);
        Assert.AreEqual(isVoid, facet.Flag);
    }

    private void AssertSealed(bool isSealed) {
        var facet = Specification.GetFacet<ITypeIsSealedFacet>();
        Assert.IsNotNull(facet);
        Assert.AreEqual(isSealed, facet.Flag);
    }

    private void AssertInterface(bool isInterface) {
        var facet = Specification.GetFacet<ITypeIsInterfaceFacet>();
        Assert.IsNotNull(facet);
        Assert.AreEqual(isInterface, facet.Flag);
    }

    private void AssertStatic(bool isStatic) {
        var facet = Specification.GetFacet<ITypeIsStaticFacet>();
        Assert.IsNotNull(facet);
        Assert.AreEqual(isStatic, facet.Flag);
    }

    private void Process(Type type) {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();
        facetFactory.Process(Reflector, type, MethodRemover, Specification, metamodel);
    }

    [TestMethod]
    public void TestIsAbstract() {
        Process(typeof(AbstractTestClass));
        AssertAbstract(true);
        AssertVoid(false);
        AssertSealed(false);
        AssertInterface(false);
        AssertStatic(false);
    }

    [TestMethod]
    public void TestIsVoid() {
        Process(typeof(void));
        AssertAbstract(false);
        AssertVoid(true);
        AssertSealed(true);
        AssertInterface(false);
        AssertStatic(false);
    }

    [TestMethod]
    public void TestIsSealed() {
        Process(typeof(SealedTestClass));
        AssertAbstract(false);
        AssertVoid(false);
        AssertSealed(true);
        AssertInterface(false);
        AssertStatic(false);
    }

    [TestMethod]
    public void TestIsInterface() {
        Process(typeof(ITestInterface));
        AssertAbstract(true);
        AssertVoid(false);
        AssertSealed(false);
        AssertInterface(true);
        AssertStatic(false);
    }

    [TestMethod]
    public void TestIsStatic() {
        Process(typeof(StaticTestClass));
        AssertAbstract(true);
        AssertVoid(false);
        AssertSealed(true);
        AssertInterface(false);
        AssertStatic(true);
    }

    private sealed class SealedTestClass { }

    private abstract class AbstractTestClass { }

    private static class StaticTestClass { }

    private interface ITestInterface { }

    #region Setup/Teardown

    [TestInitialize]
    public override void SetUp() {
        base.SetUp();
        facetFactory = new TypeMarkerFacetFactory(GetOrder<TypeMarkerFacetFactory>(), LoggerFactory);
    }

    [TestCleanup]
    public override void TearDown() {
        facetFactory = null;
        base.TearDown();
    }

    #endregion
}

// Copyright (c) Naked Objects Group Ltd.