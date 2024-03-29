﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedFramework.Architecture.Facet;
using NakedFramework.Metamodel.Facet;

namespace NakedFramework.Metamodel.Test.Facet;

[TestClass]
public class MandatoryFacetTest {
    [TestMethod]
    public void TestMandatoryFacet() {
        IMandatoryFacet facet = MandatoryFacet.Instance;
        Assert.IsTrue(facet.IsMandatory);
        Assert.IsFalse(facet.IsOptional);
        Assert.IsTrue(facet.IsRequiredButNull(null));
    }

    [TestMethod]
    public void TestMandatoryFacetDefault() {
        IMandatoryFacet facet = MandatoryFacetDefault.Instance;
        Assert.IsTrue(facet.IsMandatory);
        Assert.IsFalse(facet.IsOptional);
        Assert.IsTrue(facet.IsRequiredButNull(null));
    }

    [TestMethod]
    public void TestOptionalFacet() {
        IMandatoryFacet facet = OptionalFacet.Instance;
        Assert.IsFalse(facet.IsMandatory);
        Assert.IsTrue(facet.IsOptional);
        Assert.IsFalse(facet.IsRequiredButNull(null));
    }

    [TestMethod]
    public void TestOptionsFacetDefault() {
        IMandatoryFacet facet = OptionalFacetDefault.Instance;
        Assert.IsFalse(facet.IsMandatory);
        Assert.IsTrue(facet.IsOptional);
        Assert.IsFalse(facet.IsRequiredButNull(null));
    }
}