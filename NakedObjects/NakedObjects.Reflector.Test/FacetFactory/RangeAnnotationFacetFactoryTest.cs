// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
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
public class RangeAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
    private RangeAnnotationFacetFactory facetFactory;

    protected override Type[] SupportedTypes => new[] {typeof(IRangeFacet)};

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
    public void TestRangeAnnotationPickedUpOnActionParameter() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var method = FindMethod(typeof(Customer2), "SomeAction", new[] {typeof(int)});
        metamodel = facetFactory.ProcessParams(Reflector, method, 0, Specification, metamodel);
        var facet = Specification.GetFacet(typeof(IRangeFacet));
        Assert.IsNotNull(facet);
        Assert.IsTrue(facet is RangeFacet);
        var rangeFacetAnnotation = (RangeFacet) facet;
        Assert.AreEqual(1, rangeFacetAnnotation.Min);
        Assert.AreEqual(10, rangeFacetAnnotation.Max);
        Assert.IsNotNull(metamodel);
    }

    [TestMethod]
    public void TestRangeAnnotationPickedUpOnProperty() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var property = FindProperty(typeof(Customer1), "Prop");
        metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
        var facet = Specification.GetFacet(typeof(IRangeFacet));
        Assert.IsNotNull(facet);
        Assert.IsTrue(facet is RangeFacet);
        var rangeFacetAnnotation = (RangeFacet) facet;
        Assert.AreEqual(1, rangeFacetAnnotation.Min);
        Assert.AreEqual(10, rangeFacetAnnotation.Max);
        Assert.IsNotNull(metamodel);
    }

    #region Nested type: Customer1

    private class Customer1 {
        [Range(1, 10)]
// ReSharper disable once UnusedMember.Local
        public int Prop { get; set; }
    }

    #endregion

    #region Nested type: Customer2

    private class Customer2 {
// ReSharper disable once UnusedMember.Local
// ReSharper disable once UnusedParameter.Local
        public void SomeAction([Range(1, 10)] int foo) { }
    }

    #endregion

    #region Setup/Teardown

    [TestInitialize]
    public override void SetUp() {
        base.SetUp();
        facetFactory = new RangeAnnotationFacetFactory(GetOrder<RangeAnnotationFacetFactory>(), LoggerFactory);
    }

    [TestCleanup]
    public override void TearDown() {
        facetFactory = null;
        base.TearDown();
    }

    #endregion
}

// Copyright (c) Naked Objects Group Ltd.