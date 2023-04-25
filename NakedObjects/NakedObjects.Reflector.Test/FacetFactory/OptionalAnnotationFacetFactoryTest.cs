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
using NakedFramework.Metamodel.Facet;
using NakedObjects.Reflector.FacetFactory;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace NakedObjects.Reflector.Test.FacetFactory;

[TestClass]
public class OptionalAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
    private OptionalAnnotationFacetFactory facetFactory;

    protected override Type[] SupportedTypes => new[] { typeof(IMandatoryFacet) };

    protected override IFacetFactory FacetFactory => facetFactory;

    [TestMethod]
    public override void TestFeatureTypes() {
        var featureTypes = facetFactory.FeatureTypes;
        Assert.IsFalse(featureTypes.HasFlag(FeatureType.Objects));
        Assert.IsTrue(featureTypes.HasFlag(FeatureType.Properties));
        Assert.IsFalse(featureTypes.HasFlag(FeatureType.Collections));
        Assert.IsFalse(featureTypes.HasFlag(FeatureType.Actions));
        Assert.IsTrue(featureTypes.HasFlag(FeatureType.ActionParameters));
    }

    [TestMethod]
    public void TestOptionalAnnotationIgnoredForPrimitiveOnActionParameter() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var method = FindMethod(typeof(Customer4), nameof(Customer4.SomeAction), new[] { typeof(int) });
        metamodel = facetFactory.ProcessParams(Reflector, method, 0, Specification, metamodel);
        Assert.IsNull(Specification.GetFacet(typeof(IMandatoryFacet)));
        Assert.IsNotNull(metamodel);
    }

    [TestMethod]
    public void TestOptionalAnnotationIgnoredForPrimitiveOnProperty() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var property = FindProperty(typeof(Customer3), nameof(Customer3.NumberOfOrders));
        metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
        Assert.IsNull(Specification.GetFacet(typeof(IMandatoryFacet)));
        Assert.IsNotNull(metamodel);
    }

    [TestMethod]
    public void TestOptionalAnnotationPickedUpOnActionParameter() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var method = FindMethod(typeof(Customer2), nameof(Customer2.SomeAction), new[] { typeof(string) });
        metamodel = facetFactory.ProcessParams(Reflector, method, 0, Specification, metamodel);
        var facet = Specification.GetFacet(typeof(IMandatoryFacet));
        Assert.IsNotNull(facet);
        Assert.IsTrue(facet is OptionalFacet);
        Assert.IsNotNull(metamodel);
    }

    [TestMethod]
    public void TestOptionalAnnotationPickedUpOnProperty() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var property = FindProperty(typeof(Customer1), nameof(Customer1.FirstName));
        metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
        var facet = Specification.GetFacet(typeof(IMandatoryFacet));
        Assert.IsNotNull(facet);
        Assert.IsTrue(facet is OptionalFacet);
        Assert.IsNotNull(metamodel);
    }

    [TestMethod]
    public void TestNullableAnnotationPickedUpOnProperty1() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var property1 = FindProperty(typeof(Customer11), nameof(Customer11.FirstName));
        metamodel = facetFactory.Process(Reflector, property1, MethodRemover, Specification, metamodel);
        var facet = Specification.GetFacet(typeof(IMandatoryFacet));
        Assert.IsNotNull(facet);
        Assert.IsTrue(facet is OptionalFacet);
        Assert.IsNotNull(metamodel);

    }

    [TestMethod]
    public void TestNullableAnnotationPickedUpOnProperty2() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var property1 = FindProperty(typeof(Customer11), nameof(Customer11.SecondName));
        metamodel = facetFactory.Process(Reflector, property1, MethodRemover, Specification, metamodel);
        var facet = Specification.GetFacet(typeof(IMandatoryFacet));
        Assert.IsNull(facet);
        Assert.IsNotNull(metamodel);
    }

    [TestMethod]
    public void TestNullableAnnotationPickedUpOnProperty3() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var property1 = FindProperty(typeof(Customer11), nameof(Customer12.FirstName));
        metamodel = facetFactory.Process(Reflector, property1, MethodRemover, Specification, metamodel);
        var facet = Specification.GetFacet(typeof(IMandatoryFacet));
        Assert.IsNotNull(facet);
        Assert.IsTrue(facet is OptionalFacet);
        Assert.IsNotNull(metamodel);
    }

    [TestMethod]
    public void TestNullableAnnotationPickedUpOnProperty4() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var property2 = FindProperty(typeof(Customer11), nameof(Customer13.SecondName));
        metamodel = facetFactory.Process(Reflector, property2, MethodRemover, Specification, metamodel);
        var facet = Specification.GetFacet(typeof(IMandatoryFacet));
        Assert.IsNull(facet);
        Assert.IsNotNull(metamodel);
    }

    #region Nested type: Customer1

    private class Customer1 {
        [Optionally]

        public string FirstName => null;
    }

    #endregion

    #region Nested type: Customer2

    private class Customer2 {
// ReSharper disable once UnusedParameter.Local
        public void SomeAction([Optionally] string foo) { }
    }

    #endregion

    #region Nested type: Customer3

    private class Customer3 {
        [Optionally]
        public int NumberOfOrders => 0;
    }

    #endregion

    #region Nested type: Customer4

    private class Customer4 {
// ReSharper disable once UnusedParameter.Local
        public void SomeAction([Optionally] int foo) { }
    }

    #endregion


    #region Nullable types
    #nullable enable

    private class Customer11 {
        public string? FirstName => null;
        public string SecondName => "";
    }

    private class Customer12 {
        public string? FirstName => null;
    }

    private class Customer13 {
        public string SecondName => "";
    }


    #nullable restore
    #endregion
   





    #region Setup/Teardown

    [TestInitialize]
    public override void SetUp() {
        base.SetUp();
        facetFactory = new OptionalAnnotationFacetFactory(GetOrder<OptionalAnnotationFacetFactory>(), LoggerFactory);
    }

    [TestCleanup]
    public override void TearDown() {
        facetFactory = null;
        base.TearDown();
    }

    #endregion
}

// Copyright (c) Naked Objects Group Ltd.
// ReSharper restore UnusedMember.Local