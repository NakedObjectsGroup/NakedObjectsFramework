// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Globalization;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Services;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace NakedObjects.SystemTest.ObjectFinderCompoundKeys;

[TestFixture]
public class TestObjectFinderWithCompoundKeys : TestObjectFinderWithCompoundKeysAbstract {
    [SetUp]
    public void SetUp() => Initialize();

    [TearDown]
    public void TearDown() => CleanUp();

    [OneTimeSetUp]
    public void FixtureSetUp() {
        PaymentContext.Delete();
        var context = Activator.CreateInstance<PaymentContext>();

        context.Database.Create();
        DatabaseInitializer.Seed(context);
        InitializeNakedObjectsFramework(this);
    }

    [OneTimeTearDown]
    public void TearDownTest() {
        CleanupNakedObjectsFramework(this);
        PaymentContext.Delete();
    }

    protected override Type[] ObjectTypes =>
        new[] {
            typeof(IPayee),
            typeof(Payment),
            typeof(CustomerOne),
            typeof(CustomerTwo),
            typeof(CustomerThree),
            typeof(CustomerFour),
            typeof(Supplier),
            typeof(Employee)
        };

    protected override Type[] Services =>
        new[] {
            typeof(ObjectFinder),
            typeof(SimpleRepository<Payment>),
            typeof(SimpleRepository<CustomerOne>),
            typeof(SimpleRepository<CustomerTwo>),
            typeof(SimpleRepository<CustomerThree>),
            typeof(SimpleRepository<CustomerFour>),
            typeof(SimpleRepository<Supplier>),
            typeof(SimpleRepository<Employee>)
        };

    [Test]
    public virtual void ChangeAssociatedObjectType() {
        payee1.SetObject(customer2a);
        key1.AssertValueIsEqual("NakedObjects.SystemTest.ObjectFinderCompoundKeys.CustomerTwo|1|1001");
        payee1.SetObject(supplier1);
        Assert.AreEqual(payee1.ContentAsObject, supplier1);

        key1.AssertValueIsEqual("NakedObjects.SystemTest.ObjectFinderCompoundKeys.Supplier|1|2001");
    }

    [Test]
    public virtual void ClearAssociatedObject() {
        payee1.SetObject(customer2a);
        payee1.ClearObject();
        key1.AssertIsEmpty();
    }

    [Test]
    public virtual void FailsIfTooFewKeysSupplied() {
        key1.SetValue("NakedObjects.SystemTest.ObjectFinderCompoundKeys.CustomerThree|1|1001");
        try {
            payee1.AssertIsNotEmpty();
            throw new AssertFailedException("Exception should have been thrown");
        }
        catch (Exception ex) {
            Assert.AreEqual("Number of keys provided does not match the number of keys specified for type: NakedObjects.SystemTest.ObjectFinderCompoundKeys.CustomerThree", ex.Message);
        }
    }

    [Test]
    public virtual void FailsIfTooManyKeysSupplied() {
        key1.SetValue("NakedObjects.SystemTest.ObjectFinderCompoundKeys.CustomerTwo|1|1001|2001");
        try {
            payee1.AssertIsNotEmpty();
            throw new AssertFailedException("Exception should have been thrown");
        }
        catch (Exception ex) {
            Assert.AreEqual("Number of keys provided does not match the number of keys specified for type: NakedObjects.SystemTest.ObjectFinderCompoundKeys.CustomerTwo", ex.Message);
        }
    }

    [Test]
    public virtual void FailsIfTypeNameIsEmpty() {
        key1.SetValue("|1|1001|2001");
        try {
            payee1.AssertIsNotEmpty();
            throw new AssertFailedException("Exception should have been thrown");
        }
        catch (Exception ex) {
            Assert.AreEqual("Compound key: |1|1001|2001 does not contain an object type", ex.Message);
        }
    }

    [Test]
    public virtual void FailsIfTypeNameIsWrong() {
        key1.SetValue("CustomerThree|1|1001|2001");
        try {
            payee1.AssertIsNotEmpty();
            Assert.Fail("Exception should have been thrown");
        }
        catch (Exception ex) {
            Assert.AreEqual("Type: CustomerThree cannot be found", ex.Message);
        }
    }

    [Test]
    public virtual void GetAssociatedObject() {
        key1.SetValue("NakedObjects.SystemTest.ObjectFinderCompoundKeys.CustomerTwo|1|1001");
        payee1.AssertIsNotEmpty();
        payee1.ContentAsObject.GetPropertyByName("Id").AssertValueIsEqual("1");

        payee1.ClearObject();

        key1.SetValue("NakedObjects.SystemTest.ObjectFinderCompoundKeys.CustomerTwo|2|1002");
        payee1.AssertIsNotEmpty();
        payee1.ContentAsObject.GetPropertyByName("Id").AssertValueIsEqual("2");
    }

    [Test]
    public virtual void NoAssociatedObject() {
        key1.AssertIsEmpty();
    }

    [Test]
    public virtual void SetAssociatedObject() {
        payee1.SetObject(customer2a);
        key1.AssertValueIsEqual("NakedObjects.SystemTest.ObjectFinderCompoundKeys.CustomerTwo|1|1001");

        payee1.SetObject(customer2b);
        Assert.AreEqual(payee1.ContentAsObject, customer2b);

        key1.AssertValueIsEqual("NakedObjects.SystemTest.ObjectFinderCompoundKeys.CustomerTwo|2|1002");
    }

    [Test]
    public virtual void WorksWithADateTimeKey() {
        var culture = Thread.CurrentThread.CurrentCulture;
        try {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            payee1.SetObject(customer4);
            key1.AssertValueIsEqual("NakedObjects.SystemTest.ObjectFinderCompoundKeys.CustomerFour|1|01/16/2015 00:00:00");
            payee1.AssertObjectIsEqual(customer4);
            payee1.ClearObject();

            key1.SetValue("NakedObjects.SystemTest.ObjectFinderCompoundKeys.CustomerFour|1|01/17/2015 00:00:00");
            payee1.AssertIsNotEmpty();
            payee1.AssertObjectIsEqual(customer4a);
        }
        finally {
            Thread.CurrentThread.CurrentCulture = culture;
        }
    }

    [Test]
    public virtual void WorksWithASingleIntegerKey() {
        payee1.SetObject(customer1);
        key1.AssertValueIsEqual("NakedObjects.SystemTest.ObjectFinderCompoundKeys.CustomerOne|1");
        payee1.ClearObject();

        key1.SetValue("NakedObjects.SystemTest.ObjectFinderCompoundKeys.CustomerOne|1");
        payee1.AssertIsNotEmpty();
        payee1.AssertObjectIsEqual(customer1);
    }

    [Test]
    public virtual void WorksWithTripleIntegerKey() {
        payee1.SetObject(customer3);
        key1.AssertValueIsEqual("NakedObjects.SystemTest.ObjectFinderCompoundKeys.CustomerThree|1|1001|2001");
        payee1.ClearObject();

        key1.SetValue("NakedObjects.SystemTest.ObjectFinderCompoundKeys.CustomerThree|1|1001|2001");
        payee1.AssertIsNotEmpty();
        payee1.AssertObjectIsEqual(customer3);
    }
}