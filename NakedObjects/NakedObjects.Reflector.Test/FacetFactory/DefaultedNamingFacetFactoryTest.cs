// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Reflect;
using NakedObjects.Reflector.FacetFactory;

namespace NakedObjects.Reflector.Test.FacetFactory;

// Reflector place holder for defaulted naming facet factory tests !!!
[TestClass]
public class DefaultedNamingFacetFactoryTest : AbstractFacetFactoryTest {
    private DefaultNamingFacetFactory facetFactory;

    protected override Type[] SupportedTypes => new[] { typeof(INamedFacet), typeof(IPluralFacet) };

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

    #region Setup/Teardown

    [TestInitialize]
    public override void SetUp() {
        base.SetUp();
        facetFactory = new DefaultNamingFacetFactory(GetOrder<DefaultNamingFacetFactory>(), LoggerFactory);
    }

    [TestCleanup]
    public override void TearDown() {
        facetFactory = null;
        base.TearDown();
    }

    #endregion
}

// Copyright (c) Naked Objects Group Ltd.