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
public class UrlLinkAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
    private UrlLinkAnnotationFacetFactory facetFactory;

    protected override Type[] SupportedTypes => new[] { typeof(IUrlLinkFacet) };

    protected override IFacetFactory FacetFactory => facetFactory;

    [TestMethod]
    public override void TestFeatureTypes() {
        var featureTypes = facetFactory.FeatureTypes;
        Assert.IsFalse(featureTypes.HasFlag(FeatureType.Objects));
        Assert.IsTrue(featureTypes.HasFlag(FeatureType.Properties));
        Assert.IsTrue(featureTypes.HasFlag(FeatureType.Collections));
        Assert.IsTrue(featureTypes.HasFlag(FeatureType.Actions));
        Assert.IsFalse(featureTypes.HasFlag(FeatureType.ActionParameters));
    }

    [TestMethod]
    public void TestUrlLinkAnnotationPickedUpOnAction() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var method = FindMethod(typeof(Customer2), nameof(Customer2.SomeAction1), new[] { typeof(int) });
        metamodel = facetFactory.Process(Reflector, method, MethodRemover, Specification, metamodel);
        var facet = Specification.GetFacet(typeof(IUrlLinkFacet));
        Assert.IsNotNull(facet);
        Assert.IsTrue(facet is UrlLinkFacet);
        var hintFacet = (UrlLinkFacet)facet;
        Assert.AreEqual("Action", hintFacet.DisplayAs);
        Assert.AreEqual(true, hintFacet.OpenInNewTab);
        Assert.IsNotNull(metamodel);
    }

    [TestMethod]
    public void TestUrlLinkAnnotationPickedUpOnActionDefault()
    {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var method = FindMethod(typeof(Customer2), nameof(Customer2.SomeAction), new[] { typeof(int) });
        metamodel = facetFactory.Process(Reflector, method, MethodRemover, Specification, metamodel);
        var facet = Specification.GetFacet(typeof(IUrlLinkFacet));
        Assert.IsNotNull(facet);
        Assert.IsTrue(facet is UrlLinkFacet);
        var hintFacet = (UrlLinkFacet)facet;
        Assert.AreEqual(null, hintFacet.DisplayAs);
        Assert.AreEqual(false, hintFacet.OpenInNewTab);
        Assert.IsNotNull(metamodel);
    }

    [TestMethod]
    public void TestUrlLinkAnnotationPickedUpOnCollectionProperty() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var property = FindProperty(typeof(Customer1), nameof(Customer1.Customers1));
        metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
        var facet = Specification.GetFacet(typeof(IUrlLinkFacet));
        Assert.IsNotNull(facet);
        Assert.IsTrue(facet is UrlLinkFacet);
        var hintFacet = (UrlLinkFacet)facet;
        Assert.AreEqual("Customers", hintFacet.DisplayAs);
        Assert.AreEqual(true, hintFacet.OpenInNewTab);
        Assert.IsNotNull(metamodel);
    }

    [TestMethod]
    public void TestUrlLinkAnnotationPickedUpOnCollectionPropertyDefault()
    {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var property = FindProperty(typeof(Customer1), nameof(Customer1.Customers));
        metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
        var facet = Specification.GetFacet(typeof(IUrlLinkFacet));
        Assert.IsNotNull(facet);
        Assert.IsTrue(facet is UrlLinkFacet);
        var hintFacet = (UrlLinkFacet)facet;
        Assert.AreEqual(null, hintFacet.DisplayAs);
        Assert.AreEqual(false, hintFacet.OpenInNewTab);
        Assert.IsNotNull(metamodel);
    }

    [TestMethod]
    public void TestUrlLinkAnnotationPickedUpOnProperty() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var property = FindProperty(typeof(Customer1), nameof(Customer1.SecondName));
        metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
        var facet = Specification.GetFacet(typeof(IUrlLinkFacet));
        Assert.IsNotNull(facet);
        Assert.IsTrue(facet is UrlLinkFacet);
        var hintFacet = (UrlLinkFacet)facet;
        Assert.AreEqual("Name", hintFacet.DisplayAs);
        Assert.AreEqual(true, hintFacet.OpenInNewTab);
        Assert.IsNotNull(metamodel);
    }

    [TestMethod]
    public void TestUrlLinkAnnotationPickedUpOnPropertyDefault()
    {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var property = FindProperty(typeof(Customer1), nameof(Customer1.FirstName));
        metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
        var facet = Specification.GetFacet(typeof(IUrlLinkFacet));
        Assert.IsNotNull(facet);
        Assert.IsTrue(facet is UrlLinkFacet);
        var hintFacet = (UrlLinkFacet)facet;
        Assert.AreEqual(null, hintFacet.DisplayAs);
        Assert.AreEqual(false, hintFacet.OpenInNewTab);
        Assert.IsNotNull(metamodel);
    }

    private class Customer1 {
        [UrlLink]
        public string FirstName => null;

        [UrlLink(true, "Name")]
        public string SecondName => null;

        [UrlLink]
        public List<Customer2> Customers { get; set; }

        [UrlLink(true, "Customers")]
        public List<Customer2> Customers1 { get; set; }
    }

    private class Customer2 {
        [UrlLink]
        public string SomeAction(int foo) => foo.ToString();

        [UrlLink(true, "Action")]
        public string SomeAction1(int foo) => foo.ToString();
    }

    #region Setup/Teardown

    [TestInitialize]
    public override void SetUp() {
        base.SetUp();
        facetFactory = new UrlLinkAnnotationFacetFactory(GetOrder<UrlLinkAnnotationFacetFactory>(), LoggerFactory);
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
// ReSharper restore UnusedParameter.Local