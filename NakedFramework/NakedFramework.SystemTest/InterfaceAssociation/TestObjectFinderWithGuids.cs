﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using NakedFramework.Test.Interface;
using NakedObjects.Services;
using NakedObjects.SystemTest.ObjectFinderCompoundKeys;
using NUnit.Framework;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace NakedObjects.SystemTest.ObjectFinderGuid;

[TestFixture]
public class TestObjectFinderWithGuids : AbstractSystemTest<PaymentContext> {
    [SetUp]
    public void Initialize() {
        StartTest();

        payment1 = GetTestService("Payments").GetAction("All Instances").InvokeReturnCollection().ElementAt(0);
        payee1 = payment1.GetPropertyByName("Payee");
        key1 = payment1.GetPropertyByName("Payee Compound Key");

        var customers = GetTestService("Customers").GetAction("All Instances").InvokeReturnCollection();
        customer1 = customers.ElementAt(0);
        customer2 = customers.ElementAt(1);
        supplier1 = GetTestService("Suppliers").GetAction("All Instances").InvokeReturnCollection().ElementAt(0);
    }

    [TearDown]
    public void TearDown() => EndTest();

    [OneTimeSetUp]
    public void FixtureSetUp() {
        PaymentContext.Delete();
        var context = Activator.CreateInstance<PaymentContext>();

        context.Database.Create();
        DatabaseInitializer.Seed(context);
        InitializeNakedObjectsFramework(this);
    }

    [OneTimeTearDown]
    public void TearDownFixture() {
        CleanupNakedObjectsFramework(this);
        PaymentContext.Delete();
    }

    private ITestObject customer1;
    private ITestObject customer2;
    private ITestProperty key1;
    private ITestProperty payee1;
    private ITestObject payment1;
    private ITestObject supplier1;

    protected override Type[] ObjectTypes =>
        new[] {
            typeof(IPayee),
            typeof(ObjectFinder),
            typeof(Payment),
            typeof(Customer),
            typeof(Supplier)
        };

    protected override Type[] Services =>
        new[] {
            typeof(ObjectFinder),
            typeof(SimpleRepository<Payment>),
            typeof(SimpleRepository<Customer>),
            typeof(SimpleRepository<Supplier>)
        };

    [Test]
    public void ChangeAssociatedObjectType() {
        payee1.SetObject(customer1);
        payee1.ClearObject();
        payee1.SetObject(supplier1);
        Assert.AreEqual(payee1.ContentAsObject, supplier1);

        key1.AssertValueIsEqual("NakedObjects.SystemTest.ObjectFinderGuid.Supplier|89bc90ec-7017-11e0-a08c-57564824019b");
    }

    [Test]
    public void ClearAssociatedObject() {
        payee1.SetObject(customer1);
        payee1.ClearObject();
        key1.AssertIsEmpty();
    }

    [Test]
    public void GetAssociatedObject() {
        key1.SetValue("NakedObjects.SystemTest.ObjectFinderGuid.Customer|0c1ced04-7016-11e0-9c44-78544824019b");
        payee1.AssertIsNotEmpty();
        payee1.ContentAsObject.GetPropertyByName("Guid").AssertValueIsEqual("0c1ced04-7016-11e0-9c44-78544824019b");

        payee1.ClearObject();

        key1.SetValue("NakedObjects.SystemTest.ObjectFinderGuid.Customer|3d9d6ca0-7016-11e0-b12a-9e544824019b");
        payee1.AssertIsNotEmpty();
        payee1.ContentAsObject.GetPropertyByName("Guid").AssertValueIsEqual("3d9d6ca0-7016-11e0-b12a-9e544824019b");
    }

    [Test]
    public void NoAssociatedObject() {
        key1.AssertIsEmpty();
    }

    [Test]
    public void SetAssociatedObject() {
        payee1.SetObject(customer1);
        key1.AssertValueIsEqual("NakedObjects.SystemTest.ObjectFinderGuid.Customer|0c1ced04-7016-11e0-9c44-78544824019b");

        payee1.SetObject(customer2);
        Assert.AreEqual(payee1.ContentAsObject, customer2);

        key1.AssertValueIsEqual("NakedObjects.SystemTest.ObjectFinderGuid.Customer|3d9d6ca0-7016-11e0-b12a-9e544824019b");
    }
}

#region Classes used by test

public class PaymentContext : DbContext {
    public const string DatabaseName = "ObjectFinderGuid";
    private static readonly string Cs = @$"Data Source={Constants.Server};Initial Catalog={DatabaseName};Integrated Security=True;";
    public PaymentContext() : base(Cs) { }

    public DbSet<Payment> Payments { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<Employee> Employees { get; set; }

    public static void Delete() => Database.Delete(Cs);
}

public class DatabaseInitializer {
    public static void Seed(PaymentContext context) {
        context.Payments.Add(new Payment());
        context.Customers.Add(new Customer { Guid = new Guid("0c1ced04-7016-11e0-9c44-78544824019b") });
        context.Customers.Add(new Customer { Guid = new Guid("3d9d6ca0-7016-11e0-b12a-9e544824019b") });
        context.Suppliers.Add(new Supplier { Guid = new Guid("89bc90ec-7017-11e0-a08c-57564824019b") });
        context.SaveChanges();
    }
}

public class Payment {
    public IDomainObjectContainer Container { protected get; set; }
    public virtual int Id { get; set; }

    #region Payee Property (Interface Association)

    //IMPORTANT:  Register an implementation of IObjectFinder
    //Suggestion: Move this property into an 'Injected Services' region
    private IPayee myPayee;
    public IObjectFinder ObjectFinder { set; protected get; }

    //Holds a compound key that represents both the
    //actual type and the identity of the associated object.
    //NOTE: If working Model First, an equivalent property should be added to the
    //Entity, and this line of code moved into the 'buddy class'.
    [Optionally]
    public virtual string PayeeCompoundKey { get; set; }

    [NotPersisted]
    [Optionally]
    public IPayee Payee {
        get {
            if ((myPayee == null) & !string.IsNullOrEmpty(PayeeCompoundKey)) {
                myPayee = ObjectFinder.FindObject<IPayee>(PayeeCompoundKey);
            }

            return myPayee;
        }
        set {
            myPayee = value;
            PayeeCompoundKey = value == null ? null : ObjectFinder.GetCompoundKey(value);
        }
    }

    #endregion
}

public interface IPayee : IHasGuid { }

public class Customer : IPayee {
    #region IPayee Members

    [Key]
    public virtual Guid Guid { get; set; }

    #endregion
}

public class Supplier : IPayee {
    #region IPayee Members

    [Key]
    public virtual Guid Guid { get; set; }

    #endregion
}

#endregion