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
public class RestExtensionAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
    private RestExtensionAnnotationFacetFactory facetFactory;

    protected override Type[] SupportedTypes => new[] { typeof(IRestExtensionFacet) };

    protected override IFacetFactory FacetFactory => facetFactory;

    #region Nested type: Customer

    [RestExtension("aname", "avalue")]
    private class Customer { }

    #endregion

    #region Nested type: Customer1

    private class Customer1 {
        [RestExtension("aname", "avalue")]

        public string FirstName => null;

        [RestExtension("aname", "avalue")]
        public List<Customer3> Customers { get; set; }
    }

    #endregion

    #region Nested type: Customer2

    private class Customer2 {
        [RestExtension("aname", "avalue")]
// ReSharper disable UnusedParameter.Local
        public void SomeAction([RestExtension("aname", "avalue")] string foo) { }
    }

    #endregion

    #region Setup/Teardown

    [TestInitialize]
    public override void SetUp() {
        base.SetUp();
        facetFactory = new RestExtensionAnnotationFacetFactory(GetOrder<RestExtensionAnnotationFacetFactory>(), LoggerFactory);
    }

    [TestCleanup]
    public override void TearDown() {
        facetFactory = null;
        base.TearDown();
    }

    #endregion

    private class Customer3 {
        [RestExtension("aname", "avalue")]
        public int NumberOfOrders => 0;
    }

    private class Customer4 {
        public void SomeAction([RestExtension("aname", "avalue")] int foo) { }
    }

    [TestMethod]
    public override void TestFeatureTypes() {
        var featureTypes = facetFactory.FeatureTypes;
        Assert.IsTrue(featureTypes.HasFlag(FeatureType.Objects));
        Assert.IsTrue(featureTypes.HasFlag(FeatureType.Properties));
        Assert.IsTrue(featureTypes.HasFlag(FeatureType.Collections));
        Assert.IsTrue(featureTypes.HasFlag(FeatureType.Actions));
        Assert.IsTrue(featureTypes.HasFlag(FeatureType.ActionParameters));
    }

    [TestMethod]
    public void TestRestExtensionAnnotationNotIgnoredForNonStringsProperty() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var property = FindProperty(typeof(Customer3), "NumberOfOrders");
        metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
        Assert.IsNotNull(Specification.GetFacet(typeof(IRestExtensionFacet)));
        Assert.IsNotNull(metamodel);
    }

    [TestMethod]
    public void TestRestExtensionAnnotationNotIgnoredForPrimitiveOnActionParameter() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var method = FindMethod(typeof(Customer4), "SomeAction", new[] { typeof(int) });
        metamodel = facetFactory.ProcessParams(Reflector, method, 0, Specification, metamodel);
        Assert.IsNotNull(Specification.GetFacet(typeof(IRestExtensionFacet)));
        Assert.IsNotNull(metamodel);
    }

    [TestMethod]
    public void TestRestExtensionAnnotationPickedUpOnAction() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var method = FindMethod(typeof(Customer2), "SomeAction", new[] { typeof(string) });
        metamodel = facetFactory.Process(Reflector, method, MethodRemover, Specification, metamodel);
        var facet = Specification.GetFacet(typeof(IRestExtensionFacet));
        Assert.IsNotNull(facet);
        Assert.IsTrue(facet is RestExtensionFacet);
        var extFacet = (RestExtensionFacet)facet;
        Assert.AreEqual("aname", extFacet.Name);
        Assert.AreEqual("avalue", extFacet.Value);
        Assert.IsNotNull(metamodel);
    }

    [TestMethod]
    public void TestRestExtensionAnnotationPickedUpOnActionParameter() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var method = FindMethod(typeof(Customer2), "SomeAction", new[] { typeof(string) });
        metamodel = facetFactory.ProcessParams(Reflector, method, 0, Specification, metamodel);
        var facet = Specification.GetFacet(typeof(IRestExtensionFacet));
        Assert.IsNotNull(facet);
        Assert.IsTrue(facet is RestExtensionFacet);
        var extFacet = (RestExtensionFacet)facet;
        Assert.AreEqual("aname", extFacet.Name);
        Assert.AreEqual("avalue", extFacet.Value);
        Assert.IsNotNull(metamodel);
    }

    [TestMethod]
    public void TestRestExtensionAnnotationPickedUpOnClass() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        metamodel = facetFactory.Process(Reflector, typeof(Customer), MethodRemover, Specification, metamodel);
        var facet = Specification.GetFacet(typeof(IRestExtensionFacet));
        Assert.IsNotNull(facet);
        Assert.IsTrue(facet is RestExtensionFacet);
        var extFacet = (RestExtensionFacet)facet;
        Assert.AreEqual("aname", extFacet.Name);
        Assert.AreEqual("avalue", extFacet.Value);
        Assert.IsNotNull(metamodel);
    }

    [TestMethod]
    public void TestRestExtensionAnnotationPickedUpOnCollectionProperty() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var property = FindProperty(typeof(Customer1), "Customers");
        metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
        var facet = Specification.GetFacet(typeof(IRestExtensionFacet));
        Assert.IsNotNull(facet);
        Assert.IsTrue(facet is RestExtensionFacet);
        var extFacet = (RestExtensionFacet)facet;
        Assert.AreEqual("aname", extFacet.Name);
        Assert.AreEqual("avalue", extFacet.Value);
        Assert.IsNotNull(metamodel);
    }

    [TestMethod]
    public void TestRestExtensionAnnotationPickedUpOnProperty() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var property = FindProperty(typeof(Customer1), "FirstName");
        metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
        var facet = Specification.GetFacet(typeof(IRestExtensionFacet));
        Assert.IsNotNull(facet);
        Assert.IsTrue(facet is RestExtensionFacet);
        var extFacet = (RestExtensionFacet)facet;
        Assert.AreEqual("aname", extFacet.Name);
        Assert.AreEqual("avalue", extFacet.Value);
        Assert.IsNotNull(metamodel);
    }
}

// Copyright (c) Naked Objects Group Ltd.
// ReSharper restore UnusedMember.Local
// ReSharper restore UnusedParameter.Local