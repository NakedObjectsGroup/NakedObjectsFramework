// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Data.Entity;
using System.Linq;
using Microsoft.Extensions.Configuration;
using NakedFramework.Error;
using NakedObjects.Services;
using NakedObjects.SystemTest.PolymorphicAssociations;
using NUnit.Framework;

namespace NakedObjects.SystemTest.PolymorphicNavigator; 

[TestFixture]
public class TestPolymorphicNavigatorWithTypeCodeMapper : TestPolymorphicNavigatorAbstract {
    [SetUp]
    public void SetUp() => StartTest();

    [TearDown]
    public void TearDown() => EndTest();

    [OneTimeSetUp]
    public void FixtureSetUp() {
        InitializeNakedObjectsFramework(this);
        RunFixtures();
    }

    [OneTimeTearDown]
    public void FixtureTearDown() {
        CleanupNakedObjectsFramework(this);
    }

    private const string DatabaseName = "TestPolymorphicNavigatorWithTypeCodeMapper";

    protected override bool EnforceProxies => false;

    protected override Func<IConfiguration, DbContext>[] ContextCreators =>
        new Func<IConfiguration, DbContext>[] {config => new PolymorphicNavigationContext(DatabaseName)};

    protected override object[] Fixtures => new object[] {new FixtureEntities(), new FixtureLinksUsingTypeCode()};

    protected override Type[] Services => base.Services.Union(new[] {typeof(Services.PolymorphicNavigator), typeof(SimpleTypeCodeMapper)}).ToArray();

    [Test]
    public override void AttemptSetPolymorphicPropertyWithATransientAssociatedObject() {
        base.AttemptSetPolymorphicPropertyWithATransientAssociatedObject();
    }

    [Test]
    public override void AttemptToAddSameItemTwice() {
        base.AttemptToAddSameItemTwice();
    }

    [Test]
    public override void AttemptToRemoveNonExistentItem() {
        base.AttemptToRemoveNonExistentItem();
    }

    [Test]
    public void ChangePolymorphicPropertyOnPersistentObject() {
        ChangePolymorphicPropertyOnPersistentObject("CUS", "SUP");
    }

    [Test]
    public override void ClearPolymorphicProperty() {
        base.ClearPolymorphicProperty();
    }

    [Test]
    public override void FindOwnersForObject() {
        base.FindOwnersForObject();
    }

    [Test]
    public void PolymorphicCollectionAddDifferentItems() {
        PolymorphicCollectionAddDifferentItems("INV", "EXP");
    }

    [Test]
    public void PolymorphicCollectionAddMutlipleItemsOfOneType() {
        PolymorphicCollectionAddMutlipleItemsOfOneType("INV");
    }

    [Test]
    public override void RemoveItem() {
        base.RemoveItem();
    }

    [Test]
    public void SetPolymorphicPropertyOnPersistentObject() {
        SetPolymorphicPropertyOnPersistentObject("CUS");
    }

    [Test]
    public void SetPolymorphicPropertyOnTransientObject() {
        SetPolymorphicPropertyOnTransientObject("CUS");
    }
}

public class SimpleTypeCodeMapper : ITypeCodeMapper {
    #region ITypeCodeMapper Members

    public Type TypeFromCode(string code) =>
        code switch {
            "CUS" => typeof(CustomerAsPayee),
            "SUP" => typeof(SupplierAsPayee),
            "INV" => typeof(InvoiceAsPayableItem),
            "EXP" => typeof(ExpenseClaimAsPayableItem),
            _ => throw new DomainException("Code not recognised: " + code)
        };

    public string CodeFromType(Type type) {
        if (type == typeof(CustomerAsPayee)) {
            return "CUS";
        }

        if (type == typeof(SupplierAsPayee)) {
            return "SUP";
        }

        if (type == typeof(InvoiceAsPayableItem)) {
            return "INV";
        }

        if (type == typeof(ExpenseClaimAsPayableItem)) {
            return "EXP";
        }

        throw new DomainException("Type not recognised: " + type);
    }

    #endregion
}