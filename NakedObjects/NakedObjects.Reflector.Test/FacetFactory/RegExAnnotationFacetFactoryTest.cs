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
public class RegExAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
    private RegExAnnotationFacetFactory facetFactory;

    protected override Type[] SupportedTypes => new[] { typeof(IRegExFacet) };

    protected override IFacetFactory FacetFactory => facetFactory;

    #region Nested type: Customer

    [RegEx(Validation = "^A.*", Message = "Class message", CaseSensitive = false)]
    private class Customer { }

    #endregion

    #region Nested type: Customer1

    private class Customer1 {
        [RegEx(Validation = "^A.*", Message = "Property message", CaseSensitive = false)]

        public string FirstName => null;
    }

    #endregion

    #region Nested type: Customer2

    private class Customer2 {
        // ReSharper disable UnusedParameter.Local
        public void SomeAction([RegEx(Validation = "^A.*", Message = "Parameter message", CaseSensitive = false)] string foo) { }
    }

    #endregion

    #region Setup/Teardown

    [TestInitialize]
    public override void SetUp() {
        base.SetUp();
        facetFactory = new RegExAnnotationFacetFactory(GetOrder<RegExAnnotationFacetFactory>(), LoggerFactory);
    }

    [TestCleanup]
    public override void TearDown() {
        facetFactory = null;
        base.TearDown();
    }

    #endregion

    private class Customer3 {
        [RegEx(Validation = "^A.*", CaseSensitive = false)]
        public int NumberOfOrders => 0;
    }

    private class Customer4 {
        public void SomeAction([RegEx(Validation = "^A.*", CaseSensitive = false)] int foo) { }
    }

    private class Customer5 {
        [RegEx(Validation = "^A.*", CaseSensitive = false)]
        public string FirstName => null;
    }

    private class Customer7 {
        [RegularExpression("^A.*", ErrorMessage = "Property message")]
        public string FirstName => null;
    }

    private class Customer8 {
        public void SomeAction([RegularExpression("^A.*", ErrorMessage = "Parameter message")] string foo) { }
    }

    private class Customer9 {
        [RegularExpression("^A.*")]
        public int NumberOfOrders => 0;
    }

    private class Customer10 {
        public void SomeAction([RegularExpression("^A.*")] int foo) { }
    }

    private class Customer11 {
        [RegularExpression("^A.*")]
        public string FirstName => null;
    }

    [TestMethod]
    public override void TestFeatureTypes() {
        var featureTypes = facetFactory.FeatureTypes;
        Assert.IsTrue(featureTypes.HasFlag(FeatureType.Objects));
        Assert.IsTrue(featureTypes.HasFlag(FeatureType.Properties));
        Assert.IsFalse(featureTypes.HasFlag(FeatureType.Collections));
        Assert.IsFalse(featureTypes.HasFlag(FeatureType.Actions));
        Assert.IsTrue(featureTypes.HasFlag(FeatureType.ActionParameters));
    }

    [TestMethod]
    public void TestRegExAnnotationIgnoredForNonStringsProperty() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var property = FindProperty(typeof(Customer3), "NumberOfOrders");
        metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
        Assert.IsNull(Specification.GetFacet(typeof(IRegExFacet)));
        Assert.IsNotNull(metamodel);
    }

    [TestMethod]
    public void TestRegExAnnotationIgnoredForPrimitiveOnActionParameter() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var method = FindMethod(typeof(Customer4), "SomeAction", new[] { typeof(int) });
        metamodel = facetFactory.ProcessParams(Reflector, method, 0, Specification, metamodel);
        Assert.IsNull(Specification.GetFacet(typeof(IRegExFacet)));
        Assert.IsNotNull(metamodel);
    }

    [TestMethod]
    public void TestRegExAnnotationMessageNullWhenNotSpecified() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var property = FindProperty(typeof(Customer5), "FirstName");
        metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
        var facet = Specification.GetFacet(typeof(IRegExFacet));
        var regExFacet = (RegExFacet)facet;
        Assert.AreEqual(null, regExFacet.FailureMessage);
        Assert.IsNotNull(metamodel);
    }

    [TestMethod]
    public void TestRegExAnnotationPickedUpOnActionParameter() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var method = FindMethod(typeof(Customer2), "SomeAction", new[] { typeof(string) });
        metamodel = facetFactory.ProcessParams(Reflector, method, 0, Specification, metamodel);
        var facet = Specification.GetFacet(typeof(IRegExFacet));
        Assert.IsNotNull(facet);
        Assert.IsTrue(facet is RegExFacet);
        var regExFacet = (RegExFacet)facet;
        Assert.AreEqual("^A.*", regExFacet.ValidationPattern);
        Assert.AreEqual("Parameter message", regExFacet.FailureMessage);
        Assert.AreEqual(false, regExFacet.IsCaseSensitive);
        Assert.IsNotNull(metamodel);
    }

    [TestMethod]
    public void TestRegExAnnotationPickedUpOnClass() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        metamodel = facetFactory.Process(Reflector, typeof(Customer), MethodRemover, Specification, metamodel);
        var facet = Specification.GetFacet(typeof(IRegExFacet));
        Assert.IsNotNull(facet);
        Assert.IsTrue(facet is RegExFacet);
        var regExFacet = (RegExFacet)facet;
        Assert.AreEqual("^A.*", regExFacet.ValidationPattern);
        Assert.AreEqual("Class message", regExFacet.FailureMessage);
        Assert.AreEqual(false, regExFacet.IsCaseSensitive);
        Assert.IsNotNull(metamodel);
    }

    [TestMethod]
    public void TestRegExAnnotationPickedUpOnProperty() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var property = FindProperty(typeof(Customer1), "FirstName");
        metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
        var facet = Specification.GetFacet(typeof(IRegExFacet));
        Assert.IsNotNull(facet);
        Assert.IsTrue(facet is RegExFacet);
        var regExFacet = (RegExFacet)facet;
        Assert.AreEqual("^A.*", regExFacet.ValidationPattern);
        Assert.AreEqual("Property message", regExFacet.FailureMessage);
        Assert.AreEqual(false, regExFacet.IsCaseSensitive);
        Assert.IsNotNull(metamodel);
    }

    [TestMethod]
    public void TestRegularExpressionAnnotationIgnoredForNonStringsProperty() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var property = FindProperty(typeof(Customer9), "NumberOfOrders");
        metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
        Assert.IsNull(Specification.GetFacet(typeof(IRegExFacet)));
        Assert.IsNotNull(metamodel);
    }

    [TestMethod]
    public void TestRegularExpressionAnnotationIgnoredForPrimitiveOnActionParameter() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var method = FindMethod(typeof(Customer10), "SomeAction", new[] { typeof(int) });
        metamodel = facetFactory.ProcessParams(Reflector, method, 0, Specification, metamodel);
        Assert.IsNull(Specification.GetFacet(typeof(IRegExFacet)));
        Assert.IsNotNull(metamodel);
    }

    [TestMethod]
    public void TestRegularExpressionAnnotationMessageNullWhenNotSpecified() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var property = FindProperty(typeof(Customer11), "FirstName");
        metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
        var facet = Specification.GetFacet(typeof(IRegExFacet));
        var regExFacet = (RegExFacet)facet;
        Assert.AreEqual(null, regExFacet.FailureMessage);
        Assert.IsNotNull(metamodel);
    }

    [TestMethod]
    public void TestRegularExpressionAnnotationPickedUpOnActionParameter() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var method = FindMethod(typeof(Customer8), "SomeAction", new[] { typeof(string) });
        metamodel = facetFactory.ProcessParams(Reflector, method, 0, Specification, metamodel);
        var facet = Specification.GetFacet(typeof(IRegExFacet));
        Assert.IsNotNull(facet);
        Assert.IsTrue(facet is RegExFacet);
        var regExFacet = (RegExFacet)facet;
        Assert.AreEqual("^A.*", regExFacet.ValidationPattern);
        Assert.AreEqual("Parameter message", regExFacet.FailureMessage);
        Assert.AreEqual(true, regExFacet.IsCaseSensitive);
        Assert.IsNotNull(metamodel);
    }

    [TestMethod]
    public void TestRegularExpressionAnnotationPickedUpOnProperty() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var property = FindProperty(typeof(Customer7), "FirstName");
        metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
        var facet = Specification.GetFacet(typeof(IRegExFacet));
        Assert.IsNotNull(facet);
        Assert.IsTrue(facet is RegExFacet);
        var regExFacet = (RegExFacet)facet;
        Assert.AreEqual("^A.*", regExFacet.ValidationPattern);
        Assert.AreEqual("Property message", regExFacet.FailureMessage);
        Assert.AreEqual(true, regExFacet.IsCaseSensitive);
        Assert.IsNotNull(metamodel);
    }
}

// Copyright (c) Naked Objects Group Ltd.
// ReSharper restore UnusedMember.Local
// ReSharper restore UnusedParameter.Local