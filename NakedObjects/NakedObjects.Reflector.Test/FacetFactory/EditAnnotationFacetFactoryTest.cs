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
public class EditAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
    private EditAnnotationFacetFactory facetFactory;

    protected override Type[] SupportedTypes => new[] { typeof(IEditPropertiesFacet) };

    protected override IFacetFactory FacetFactory => facetFactory;

    [TestMethod]
    public void TestEditAnnotationPickedUpOnAction() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var actionMethod = FindMethod(typeof(Customer), nameof(Customer.EditTest), new[] { typeof(string), typeof(int) });
        metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification, metamodel);
        var facet = Specification.GetFacet(typeof(IEditPropertiesFacet));
        Assert.IsNotNull(facet);
        Assert.IsTrue(facet is EditPropertiesFacet);
        AssertNoMethodsRemoved();
        Assert.IsNotNull(metamodel);
    }

    [TestMethod]
    public void TestDefaultsPickedUpOnEditAction() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var actionMethod = FindMethod(typeof(Customer), nameof(Customer.EditTest), new[] { typeof(string), typeof(int) });

        for (var i = 0; i < actionMethod.GetParameters().Length; i++) {
            metamodel = facetFactory.ProcessParams(Reflector, actionMethod, i, Specification, metamodel);
            var facet = Specification.GetFacet(typeof(IActionDefaultsFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is ActionDefaultsFacetViaProperty);
            AssertNoMethodsRemoved();
            Assert.IsNotNull(metamodel);
        }
    }

    [TestMethod]
    public void TestEditAnnotationPickedUpUnmatchedProperty() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var actionMethod = FindMethod(typeof(Customer1), nameof(Customer1.EditTest), new[] { typeof(string), typeof(int) });
        metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification, metamodel);
        var facet = Specification.GetFacet(typeof(IEditPropertiesFacet));
        Assert.IsNotNull(facet);
        Assert.IsTrue(facet is EditPropertiesFacet);
        AssertNoMethodsRemoved();
        Assert.IsNotNull(metamodel);
    }

    [TestMethod]
    public void TestDefaultsPickedUpOnSubclass() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var actionMethod = FindMethod(typeof(Customer5), nameof(Customer5.EditTest), new[] { typeof(string), typeof(int) });
        metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification, metamodel);
        var facet = Specification.GetFacet(typeof(IEditPropertiesFacet));
        Assert.IsNotNull(facet);
        Assert.IsTrue(facet is EditPropertiesFacet);
        AssertNoMethodsRemoved();
        Assert.IsNotNull(metamodel);
    }

    [TestMethod]
    public void TestEditAnnotationIgnoredUnmatchedParameter() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var actionMethod = FindMethod(typeof(Customer2), nameof(Customer2.EditTest), new[] { typeof(string), typeof(int), typeof(int) });
        metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification, metamodel);
        var facet = Specification.GetFacet(typeof(IEditPropertiesFacet));
        Assert.IsNull(facet);
    }

    [TestMethod]
    public void TestEditAnnotationIgnoredNoParameter() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var actionMethod = FindMethod(typeof(Customer3), nameof(Customer2.EditTest));
        metamodel = facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification, metamodel);
        var facet = Specification.GetFacet(typeof(IEditPropertiesFacet));
        Assert.IsNull(facet);
    }

    [TestMethod]
    public override void TestFeatureTypes() {
        var featureTypes = facetFactory.FeatureTypes;
        Assert.IsFalse(featureTypes.HasFlag(FeatureType.Objects));
        Assert.IsFalse(featureTypes.HasFlag(FeatureType.Properties));
        Assert.IsFalse(featureTypes.HasFlag(FeatureType.Collections));
        Assert.IsTrue(featureTypes.HasFlag(FeatureType.Actions));
        Assert.IsTrue(featureTypes.HasFlag(FeatureType.ActionParameters));
    }

    #region Nested type: Customer

    private class Customer {
        public string Property1 => "";
        public int Property2 => 1;

        [Edit]
        public Customer EditTest(string property1, int property2) => this;
    }

    private class Customer1 {
        public string Property1 => "";
        public int Property2 => 1;
        public int Property3 => 1;

        [Edit]
        public Customer1 EditTest(string property1, int property2) => this;
    }

    private class Customer2 {
        public string Property1 => "";
        public int Property2 => 1;

        [Edit]
        public Customer2 EditTest(string property1, int property2, int property3) => this;
    }

    private class Customer3 {
        public string Property1 => "";
        public int Property2 => 1;

        [Edit]
        public Customer3 EditTest() => this;
    }

    private class Customer4 {
        public string Property1 => "";
        public int Property2 => 1;
    }

    private class Customer5 : Customer4 {
        [Edit]
        public Customer4 EditTest(string property1, int property2) => this;
    }

    #endregion

    #region Setup/Teardown

    [TestInitialize]
    public override void SetUp() {
        base.SetUp();
        facetFactory = new EditAnnotationFacetFactory(GetOrder<EditAnnotationFacetFactory>(), LoggerFactory);
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