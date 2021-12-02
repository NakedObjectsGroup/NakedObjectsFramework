// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
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
public class MemberOrderAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
    private MemberOrderAnnotationFacetFactory facetFactory;

    protected override Type[] SupportedTypes => new[] { typeof(IMemberOrderFacet) };

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
    public void TestMemberOrderAnnotationPickedUpOnAction() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var method = FindMethod(typeof(Customer2), "SomeAction");
        metamodel = facetFactory.Process(Reflector, method, MethodRemover, Specification, metamodel);
        var facet = Specification.GetFacet(typeof(IMemberOrderFacet));
        Assert.IsNotNull(facet);
        Assert.IsTrue(facet is MemberOrderFacet);
        var memberOrderFacetAnnotation = (MemberOrderFacet)facet;
        Assert.AreEqual("3", memberOrderFacetAnnotation.Sequence);
        AssertNoMethodsRemoved();
        Assert.IsNotNull(metamodel);
    }

    [TestMethod]
    public void TestMemberOrderAnnotationPickedUpOnCollection() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var property = FindProperty(typeof(Customer1), "Orders");
        metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
        var facet = Specification.GetFacet(typeof(IMemberOrderFacet));
        Assert.IsNotNull(facet);
        Assert.IsTrue(facet is MemberOrderFacet);
        var memberOrderFacetAnnotation = (MemberOrderFacet)facet;
        Assert.AreEqual("2", memberOrderFacetAnnotation.Sequence);
        AssertNoMethodsRemoved();
        Assert.IsNotNull(metamodel);
    }

    [TestMethod]
    public void TestMemberOrderAnnotationPickedUpOnProperty() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var property = FindProperty(typeof(Customer), "FirstName");
        metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
        var facet = Specification.GetFacet(typeof(IMemberOrderFacet));
        Assert.IsNotNull(facet);
        Assert.IsTrue(facet is MemberOrderFacet);
        var memberOrderFacetAnnotation = (MemberOrderFacet)facet;
        Assert.AreEqual("1", memberOrderFacetAnnotation.Sequence);
        AssertNoMethodsRemoved();
        Assert.IsNotNull(metamodel);
    }

    #region Nested type: Customer

    private class Customer {
        [MemberOrder(Sequence = "1")]

        public string FirstName => null;
    }

    #endregion

    #region Nested type: Customer1

    private class Customer1 {
        [MemberOrder(Sequence = "2")]
        public IList Orders => null;

        // ReSharper disable once UnusedParameter.Local
        public void AddToOrders(Order o) { }
    }

    #endregion

    #region Nested type: Customer2

    private class Customer2 {
        [MemberOrder(Sequence = "3")]
        public void SomeAction() { }
    }

    #endregion

    #region Nested type: Order

// ReSharper disable once ClassNeverInstantiated.Local
    private class Order { }

    #endregion

    #region Setup/Teardown

    [TestInitialize]
    public override void SetUp() {
        base.SetUp();
        facetFactory = new MemberOrderAnnotationFacetFactory(GetOrder<MemberOrderAnnotationFacetFactory>(), LoggerFactory);
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