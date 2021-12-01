﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using NakedFramework.Test.Interface;
using NakedObjects.Services;
using NUnit.Framework;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace NakedObjects.SystemTest.ObjectFinderSingleKey; 

[TestFixture]
public class TestObjectFinderWithSingleKeys : AbstractSystemTest<PaymentContext> {
    [SetUp]
    public void SetUp() {
        StartTest();
        payment1 = GetAllInstances<Payment>(0);
        payee1 = payment1.GetPropertyByName("Payee");
        key1 = payment1.GetPropertyByName("Payee Compound Key");
        customer1 = GetAllInstances<Customer>(0);
        customer2 = GetAllInstances<Customer>(1);
        supplier1 = GetAllInstances<Supplier>(0);
        emp1 = GetAllInstances<Employee>(1);
        emp2 = GetAllInstances<Employee>(0); //They seem to be persisted in reverse order!
    }

    [TearDown]
    public void CleanUp() {
        EndTest();
        payment1 = null;
        customer1 = null;
        payee1 = null;
        key1 = null;
        emp1 = null;
        emp2 = null;
    }

    [OneTimeSetUp]
    public void SetupTestFixture() {
        Database.SetInitializer(new DatabaseInitializer());
        InitializeNakedObjectsFramework(this);
    }

    [OneTimeTearDown]
    public void TearDownFixture() {
        CleanupNakedObjectsFramework(this);
        PaymentContext.Delete();
    }

    private ITestObject customer1;
    private ITestObject customer2;
    private ITestObject emp1;
    private ITestObject emp2;
    private ITestProperty key1;
    private ITestProperty payee1;
    private ITestObject payment1;
    private ITestObject supplier1;

    protected override Type[] ObjectTypes =>
        new[] {
            typeof(IPayee),
            typeof(Payment),
            typeof(Customer),
            typeof(Supplier),
            typeof(Employee)
        };

    protected override Type[] Services =>
        new[] {
            typeof(ObjectFinder),
            typeof(SimpleRepository<Payment>),
            typeof(SimpleRepository<Customer>),
            typeof(SimpleRepository<Supplier>),
            typeof(SimpleRepository<Employee>)
        };

    [Test]
    public void ChangeAssociatedObjectType() {
        payee1.SetObject(customer1);
        payee1.ClearObject();
        payee1.SetObject(supplier1);
        Assert.AreEqual(payee1.ContentAsObject, supplier1);

        key1.AssertValueIsEqual("NakedObjects.SystemTest.ObjectFinderSingleKey.Supplier|1");
    }

    [Test]
    public void ClearAssociatedObject() {
        payee1.SetObject(customer1);
        payee1.ClearObject();
        key1.AssertIsEmpty();
    }

    [Test]
    public void GetAssociatedObject() {
        key1.SetValue("NakedObjects.SystemTest.ObjectFinderSingleKey.Customer|1");
        payee1.AssertIsNotEmpty();
        payee1.ContentAsObject.GetPropertyByName("Id").AssertValueIsEqual("1");

        payee1.ClearObject();

        key1.SetValue("NakedObjects.SystemTest.ObjectFinderSingleKey.Customer|2");
        payee1.AssertIsNotEmpty();
        payee1.ContentAsObject.GetPropertyByName("Id").AssertValueIsEqual("2");
    }

    [Test]
    public void GetAssociatedObjectWithAStringKey() {
        key1.SetValue("NakedObjects.SystemTest.ObjectFinderSingleKey.Employee|foo");
        payee1.AssertObjectIsEqual(emp1);

        payee1.ClearObject();

        key1.SetValue("NakedObjects.SystemTest.ObjectFinderSingleKey.Employee|bar");
        payee1.AssertObjectIsEqual(emp2);
    }

    [Test]
    public void NoAssociatedObject() {
        key1.AssertIsEmpty();
    }

    [Test]
    public void SetAssociatedObject() {
        payee1.SetObject(customer1);
        key1.AssertValueIsEqual("NakedObjects.SystemTest.ObjectFinderSingleKey.Customer|1");

        payee1.SetObject(customer2);
        Assert.AreEqual(payee1.ContentAsObject, customer2);

        key1.AssertValueIsEqual("NakedObjects.SystemTest.ObjectFinderSingleKey.Customer|2");
    }

    [Test]
    public void SetAssociatedObjectObjectWithAStringKey() {
        payee1.SetObject(emp1);
        key1.AssertValueIsEqual("NakedObjects.SystemTest.ObjectFinderSingleKey.Employee|foo");

        payee1.SetObject(emp2);
        key1.AssertValueIsEqual("NakedObjects.SystemTest.ObjectFinderSingleKey.Employee|bar");
    }
}

#region Classes used by test

public class PaymentContext : DbContext {
    public const string DatabaseName = "ObjectFinderSingleKey";
    private static readonly string Cs = @$"Data Source={Constants.Server};Initial Catalog={DatabaseName};Integrated Security=True;";

    public PaymentContext() : base(Cs) { }

    public DbSet<Payment> Payments { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<Employee> Employees { get; set; }

    public static void Delete() => Database.Delete(Cs);

    protected override void OnModelCreating(DbModelBuilder modelBuilder) {
        Database.SetInitializer(new DatabaseInitializer());
    }
}

public class DatabaseInitializer : DropCreateDatabaseAlways<PaymentContext> {
    protected override void Seed(PaymentContext context) {
        context.Payments.Add(new Payment());
        context.Customers.Add(new Customer());
        context.Customers.Add(new Customer());
        context.Suppliers.Add(new Supplier());
        context.Employees.Add(new Employee {Id = "foo"});
        context.Employees.Add(new Employee {Id = "bar"});
        context.SaveChanges();
    }
}

public class Payment {
    public IDomainObjectContainer Container { protected get; set; }

    [Disabled]
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

public interface IPayee { }

public class Customer : IPayee {
    [Disabled]
    public virtual int Id { get; set; }
}

public class Supplier : IPayee {
    [Disabled]
    public virtual int Id { get; set; }
}

public class Employee : IPayee {
    [Key]
    public virtual string Id { get; set; }
}

#endregion