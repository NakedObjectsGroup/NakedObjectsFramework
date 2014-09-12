// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Data.Entity.SqlServer;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.EntityObjectStore;
using NakedObjects.Services;
using NakedObjects.Xat;
using System.Data.Entity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NakedObjects.SystemTest.PolymorphicLinkService
{
    [TestClass]
    public class TestPolymorphicNavigatorWithOid : AcceptanceTestCase
    {
        //    #region Setup/Teardown

        //    // to get EF SqlServer Dll in memory
        //    public SqlProviderServices instance = SqlProviderServices.Instance;

        //    [TestInitialize]
        //    public void Initialize()
        //    {

        //        InitializeNakedObjectsFramework(this);
        //    }

        //    [TestCleanup]
        //    public void CleanUp()
        //    {
        //        CleanupNakedObjectsFramework(this);
        //    }

        //    #endregion

        //    #region Run configuration

        //    protected override IServicesInstaller MenuServices
        //    {
        //        get
        //        {
        //            return new ServicesInstaller(
        //                new SimpleRepository<PolymorphicPayment>(),
        //                new SimpleRepository<CustomerAsPayee>(),
        //                new SimpleRepository<SupplierAsPayee>(),
        //                new SimpleRepository<InvoiceAsPayableItem>(),
        //                new SimpleRepository<ExpenseClaimAsPayableItem>(),
        //                // new PolymorphicNavigatorWithOid(),
        //                new ObjectFinder());
        //        }
        //    }


        //    protected override IObjectPersistorInstaller Persistor
        //    {
        //        get
        //        {
        //            var p = new EntityPersistorInstaller();
        //            p.UsingCodeFirstContext(() => new MyContext());
        //            return p;
        //        }
        //    }

        //    #endregion

        //    [TestMethod]
        //    public void SetPolymorphicPropertyOnTransientObject()
        //    {
        //        ITestObject payment1 = GetTestService("Polymorphic Payments").GetAction("New Instance").InvokeReturnObject();

        //        ITestObject customer1 = GetTestService("Customer As Payees").GetAction("New Instance").InvokeReturnObject().Save();
        //        string cusId = customer1.GetPropertyByName("Id").Title;

        //        ITestProperty payeeProp = payment1.GetPropertyByName("Payee");
        //        ITestProperty payeeLinkProp = payment1.GetPropertyByName("Payee Link").AssertIsUnmodifiable().AssertIsEmpty();
        //        payeeProp.SetObject(customer1);
        //        payment1.Save();
        //        ITestObject payeeLink = payeeLinkProp.AssertIsNotEmpty().ContentAsObject;
        //        ITestProperty oid = payeeLink.GetPropertyByName("Role Object Oid").AssertIsUnmodifiable();
        //        oid.AssertValueIsEqual("NakedObjects.SystemTest.PolymorphicLinkService.CustomerAsPayee|" + cusId);
        //    }

        //    [TestMethod]
        //    public void AttemptSetPolymorphicPropertyWithATransientAssociatedObject()
        //    {
        //        ITestObject payment1 = GetTestService("Polymorphic Payments").GetAction("New Instance").InvokeReturnObject().Save();

        //        ITestObject customer1 = GetTestService("Customer As Payees").GetAction("New Instance").InvokeReturnObject();
        //        string cusId = customer1.GetPropertyByName("Id").Title;

        //        ITestProperty payeeProp = payment1.GetPropertyByName("Payee");
        //        ITestProperty payeeLinkProp = payment1.GetPropertyByName("Payee Link").AssertIsUnmodifiable().AssertIsEmpty();
        //        try
        //        {
        //            payeeProp.SetObject(customer1);
        //            Assert.Fail("Should not get to here");
        //        }
        //        catch (Exception e)
        //        {
        //            Assert.IsTrue(e.Message.Contains("Can't set field of persistent with a transient reference"));
        //        }
        //    }


        //    [TestMethod]
        //    public void SetPolymorphicPropertyOnPersistentObject()
        //    {
        //        ITestObject payment1 = GetTestService("Polymorphic Payments").GetAction("New Instance").InvokeReturnObject().Save();

        //        ITestObject customer1 = GetTestService("Customer As Payees").GetAction("New Instance").InvokeReturnObject().Save();
        //        string cusId = customer1.GetPropertyByName("Id").Title;

        //        ITestProperty payeeProp = payment1.GetPropertyByName("Payee");
        //        ITestProperty payeeLinkProp = payment1.GetPropertyByName("Payee Link").AssertIsUnmodifiable().AssertIsEmpty();
        //        payeeProp.SetObject(customer1);

        //        ITestObject payeeLink = payeeLinkProp.AssertIsNotEmpty().ContentAsObject;
        //        ITestProperty oid = payeeLink.GetPropertyByName("Role Object Oid").AssertIsUnmodifiable();
        //        oid.AssertValueIsEqual("NakedObjects.SystemTest.PolymorphicLinkService.CustomerAsPayee|" + cusId);
        //    }

        //    [TestMethod]
        //    public void ChangePolymorphicPropertyOnPersistentObject()
        //    {
        //        ITestObject payment1 = GetTestService("Polymorphic Payments").GetAction("New Instance").InvokeReturnObject();

        //        ITestObject customer1 = GetTestService("Customer As Payees").GetAction("New Instance").InvokeReturnObject().Save();
        //        string cusId = customer1.GetPropertyByName("Id").Title;

        //        ITestProperty payeeProp = payment1.GetPropertyByName("Payee");
        //        ITestProperty payeeLinkProp = payment1.GetPropertyByName("Payee Link").AssertIsUnmodifiable().AssertIsEmpty();
        //        payeeProp.SetObject(customer1);
        //        payment1.Save();
        //        ITestObject payeeLink = payeeLinkProp.AssertIsNotEmpty().ContentAsObject;
        //        ITestProperty oid = payeeLink.GetPropertyByName("Role Object Oid").AssertIsUnmodifiable();
        //        oid.AssertValueIsEqual("NakedObjects.SystemTest.PolymorphicLinkService.CustomerAsPayee|" + cusId);

        //        ITestObject sup1 = GetTestService("Supplier As Payees").GetAction("New Instance").InvokeReturnObject().Save();
        //        string supId1 = sup1.GetPropertyByName("Id1").Title;
        //        string supId2 = sup1.GetPropertyByName("Id2").Title;

        //        payeeProp.SetObject(sup1);
        //        oid.AssertValueIsEqual("NakedObjects.SystemTest.PolymorphicLinkService.SupplierAsPayee|" + supId1 + "|" + supId2);

        //        payeeProp.ClearObject();
        //        payeeLinkProp.AssertIsEmpty();
        //        payeeProp.AssertIsEmpty();
        //    }


        //    [TestMethod]
        //    public void ClearPolymorphicProperty()
        //    {
        //        ITestObject payment1 = GetTestService("Polymorphic Payments").GetAction("New Instance").InvokeReturnObject();

        //        ITestObject customer1 = GetTestService("Customer As Payees").GetAction("New Instance").InvokeReturnObject().Save();
        //        ITestProperty payeeProp = payment1.GetPropertyByName("Payee");
        //        ITestProperty payeeLinkProp = payment1.GetPropertyByName("Payee Link");
        //        payeeProp.SetObject(customer1);
        //        payment1.Save();
        //        payeeLinkProp.AssertIsNotEmpty();
        //        payeeProp.AssertIsNotEmpty();

        //        payeeProp.ClearObject();
        //        payeeLinkProp.AssertIsEmpty();
        //        payeeProp.AssertIsEmpty();
        //    }

        //    [TestMethod]
        //    public void PolymorphicCollectionAddMutlipleItemsOfOneType()
        //    {
        //        ITestObject payment = GetTestService("Polymorphic Payments").GetAction("New Instance").InvokeReturnObject().Save();

        //        ITestObject inv = GetTestService("Invoice As Payable Items").GetAction("New Instance").InvokeReturnObject().Save();
        //        string invId = inv.GetPropertyByName("Id").Title;
        //        ITestObject inv2 = GetTestService("Invoice As Payable Items").GetAction("New Instance").InvokeReturnObject().Save();
        //        string inv2Id = inv2.GetPropertyByName("Id").Title;


        //        ITestCollection links = payment.GetPropertyByName("Payable Item Links").ContentAsCollection;
        //        ITestCollection items = payment.GetPropertyByName("Payable Items").ContentAsCollection;

        //        links.AssertCountIs(0);
        //        items.AssertCountIs(0);

        //        //Add an Invoice
        //        payment.GetAction("Add Payable Item").InvokeReturnObject(inv);
        //        links.AssertCountIs(1);
        //        ITestObject link1 = links.ElementAt(0);
        //        link1.GetPropertyByName("Role Object Oid").AssertValueIsEqual("NakedObjects.SystemTest.PolymorphicLinkService.InvoiceAsPayableItem|" + invId);
        //        items = payment.GetPropertyByName("Payable Items").ContentAsCollection;
        //        ITestObject item = items.AssertCountIs(1).ElementAt(0);
        //        item.AssertIsType(typeof(InvoiceAsPayableItem));

        //        //Add an expense claim
        //        payment.GetAction("Add Payable Item").InvokeReturnObject(inv2);
        //        links.AssertCountIs(2);
        //        ITestObject link2 = links.ElementAt(1);
        //        link2.GetPropertyByName("Role Object Oid").AssertValueIsEqual("NakedObjects.SystemTest.PolymorphicLinkService.InvoiceAsPayableItem|" + inv2Id);
        //        items = payment.GetPropertyByName("Payable Items").ContentAsCollection;
        //        item = items.AssertCountIs(2).ElementAt(1);
        //        item.AssertIsType(typeof(InvoiceAsPayableItem));
        //    }

        //    [TestMethod]
        //    public void PolymorphicCollectionAddDifferentItems()
        //    {
        //        ITestObject payment = GetTestService("Polymorphic Payments").GetAction("New Instance").InvokeReturnObject().Save();

        //        ITestObject inv = GetTestService("Invoice As Payable Items").GetAction("New Instance").InvokeReturnObject().Save();
        //        string invId = inv.GetPropertyByName("Id").Title;
        //        ITestObject exp = GetTestService("Expense Claim As Payable Items").GetAction("New Instance").InvokeReturnObject().Save();
        //        string expId1 = exp.GetPropertyByName("Id1").Title;
        //        string expId2 = exp.GetPropertyByName("Id2").Title;

        //        ITestCollection links = payment.GetPropertyByName("Payable Item Links").ContentAsCollection;
        //        ITestCollection items = payment.GetPropertyByName("Payable Items").ContentAsCollection;

        //        links.AssertCountIs(0);
        //        items.AssertCountIs(0);

        //        //Add an Invoice
        //        payment.GetAction("Add Payable Item").InvokeReturnObject(inv);
        //        links.AssertCountIs(1);
        //        ITestObject link1 = links.ElementAt(0);
        //        link1.GetPropertyByName("Role Object Oid").AssertValueIsEqual("NakedObjects.SystemTest.PolymorphicLinkService.InvoiceAsPayableItem|" + invId);
        //        items = payment.GetPropertyByName("Payable Items").ContentAsCollection;
        //        ITestObject item1 = items.AssertCountIs(1).ElementAt(0);
        //        item1.AssertIsType(typeof(InvoiceAsPayableItem));

        //        //Add an expense claim
        //        payment.GetAction("Add Payable Item").InvokeReturnObject(exp);
        //        links.AssertCountIs(2);
        //        ITestObject link2 = links.ElementAt(1);
        //        link2.GetPropertyByName("Role Object Oid").AssertValueIsEqual("NakedObjects.SystemTest.PolymorphicLinkService.ExpenseClaimAsPayableItem|" + expId1 + "|" + expId2);
        //        items = payment.GetPropertyByName("Payable Items").ContentAsCollection.AssertCountIs(2);
        //        item1 = items.ElementAt(0);
        //        item1.AssertIsType(typeof(InvoiceAsPayableItem));
        //        var item2 = items.ElementAt(1);
        //        item2.AssertIsType(typeof(ExpenseClaimAsPayableItem));
        //    }


        //    [TestMethod]
        //    public void AttemptToAddSameItemTwice()
        //    {
        //        ITestObject payment = GetTestService("Polymorphic Payments").GetAction("New Instance").InvokeReturnObject().Save();

        //        ITestObject inv = GetTestService("Invoice As Payable Items").GetAction("New Instance").InvokeReturnObject().Save();
        //        string invId = inv.GetPropertyByName("Id").Title;

        //        ITestCollection links = payment.GetPropertyByName("Payable Item Links").ContentAsCollection;
        //        ITestCollection items = payment.GetPropertyByName("Payable Items").ContentAsCollection;

        //        links.AssertCountIs(0);
        //        items.AssertCountIs(0);

        //        //Add an Invoice
        //        payment.GetAction("Add Payable Item").InvokeReturnObject(inv);
        //        links.AssertCountIs(1);
        //        items = payment.GetPropertyByName("Payable Items").ContentAsCollection;
        //        items.AssertCountIs(1);

        //        //Try adding same expense claim again
        //        payment.GetAction("Add Payable Item").InvokeReturnObject(inv);
        //        links.AssertCountIs(1); //Should still be 1
        //        items = payment.GetPropertyByName("Payable Items").ContentAsCollection;
        //        items.AssertCountIs(1);
        //    }

        //    [TestMethod]
        //    public void RemoveItem()
        //    {
        //        ITestObject payment = GetTestService("Polymorphic Payments").GetAction("New Instance").InvokeReturnObject().Save();

        //        ITestObject inv = GetTestService("Invoice As Payable Items").GetAction("New Instance").InvokeReturnObject().Save();
        //        string invId = inv.GetPropertyByName("Id").Title;

        //        ITestCollection links = payment.GetPropertyByName("Payable Item Links").ContentAsCollection;
        //        ITestCollection items = payment.GetPropertyByName("Payable Items").ContentAsCollection;

        //        links.AssertCountIs(0);
        //        items.AssertCountIs(0);

        //        //Add an Invoice
        //        payment.GetAction("Add Payable Item").InvokeReturnObject(inv);
        //        links.AssertCountIs(1);
        //        items = payment.GetPropertyByName("Payable Items").ContentAsCollection;
        //        ITestCollection item = items.AssertCountIs(1);

        //        //Now remove the invoice
        //        payment.GetAction("Remove Payable Item").InvokeReturnObject(inv);
        //        links.AssertCountIs(0);
        //        items = payment.GetPropertyByName("Payable Items").ContentAsCollection;
        //        items.AssertCountIs(0);
        //    }


        //    [TestMethod]
        //    public void AttemptToRemoveNonExistentItem()
        //    {
        //        ITestObject payment = GetTestService("Polymorphic Payments").GetAction("New Instance").InvokeReturnObject().Save();

        //        ITestObject inv = GetTestService("Invoice As Payable Items").GetAction("New Instance").InvokeReturnObject().Save();
        //        ITestObject inv2 = GetTestService("Invoice As Payable Items").GetAction("New Instance").InvokeReturnObject().Save();
        //        string invId = inv.GetPropertyByName("Id").Title;

        //        ITestCollection links = payment.GetPropertyByName("Payable Item Links").ContentAsCollection;
        //        ITestCollection items = payment.GetPropertyByName("Payable Items").ContentAsCollection;

        //        links.AssertCountIs(0);
        //        items.AssertCountIs(0);

        //        //Add Invoice 1
        //        payment.GetAction("Add Payable Item").InvokeReturnObject(inv);
        //        links.AssertCountIs(1);
        //        items = payment.GetPropertyByName("Payable Items").ContentAsCollection;
        //        ITestObject item = items.AssertCountIs(1).ElementAt(0);
        //        item.AssertIsType(typeof(InvoiceAsPayableItem));

        //        //Now attempt to remove invoice 2
        //        payment.GetAction("Remove Payable Item").InvokeReturnObject(inv2);
        //        links.AssertCountIs(1);
        //        items = payment.GetPropertyByName("Payable Items").ContentAsCollection;
        //        item = items.AssertCountIs(1).ElementAt(0);
        //    }


        //    [TestMethod]
        //    public void FindOwnersForObject()
        //    {
        //        ITestObject payment1 = GetTestService("Polymorphic Payments").GetAction("New Instance").InvokeReturnObject().Save();
        //        ITestObject payment2 = GetTestService("Polymorphic Payments").GetAction("New Instance").InvokeReturnObject().Save();
        //        ITestObject payment3 = GetTestService("Polymorphic Payments").GetAction("New Instance").InvokeReturnObject().Save();


        //        ITestObject cus = GetTestService("Customer As Payees").GetAction("New Instance").InvokeReturnObject().Save();
        //        ITestObject inv = GetTestService("Invoice As Payable Items").GetAction("New Instance").InvokeReturnObject().Save();


        //        ITestProperty payeeProp = payment1.GetPropertyByName("Payee");
        //        payeeProp.SetObject(cus);
        //        payeeProp = payment3.GetPropertyByName("Payee");
        //        payeeProp.SetObject(cus);

        //        ITestCollection results = cus.GetAction("Payments To This Payee").InvokeReturnCollection();
        //        results.AssertCountIs(2);
        //        Assert.AreEqual(payment1, results.ElementAt(0));
        //        Assert.AreEqual(payment3, results.ElementAt(1));


        //        //Add invoice to two payments
        //        payment1.GetAction("Add Payable Item").InvokeReturnObject(inv);
        //        payment2.GetAction("Add Payable Item").InvokeReturnObject(inv);

        //        results = inv.GetAction("Payments Containing This").InvokeReturnCollection();
        //        results.AssertCountIs(2);
        //        Assert.AreEqual(payment1, results.ElementAt(0));
        //        Assert.AreEqual(payment2, results.ElementAt(1));
        //    }
        //}

        //#region Classes used by test

        //public interface IPayee { }

        //public class PolymorphicPayment : IHasIntegerId
        //{
        //    #region Injected Services
        //    public IPolymorphicNavigatorWithOid PolymorphicNavigator { set; protected get; }

        //    public IDomainObjectContainer Container { protected get; set; }
        //    #endregion

        //    #region LifeCycle methods
        //    public void Persisting()
        //    {
        //        PayeeLink = PolymorphicNavigator.NewTransientLink<PolymorphicPaymentPayeeLink, IPayee, PolymorphicPayment>(_Payee, this);
        //    }
        //    #endregion

        //    public virtual int Id { get; set; }

        //    #region Payee Property of type IPayee ('role' interface)

        //    private IPayee _Payee;

        //    [Disabled]
        //    public virtual PolymorphicPaymentPayeeLink PayeeLink { get; set; }

        //    [NotPersisted, Optionally]
        //    public IPayee Payee
        //    {
        //        get
        //        {
        //            return PayeeLink != null ? PayeeLink.AssociatedRoleObject : null;
        //        }
        //        set
        //        {
        //            _Payee = value;
        //            PayeeLink = PolymorphicNavigator.UpdateAddOrDeleteLink(_Payee, PayeeLink, this);
        //        }
        //    }

        //    #endregion

        //    #region PayableItems Collection of type IPayableItem

        //    //TODO:  Create a type 'PolymorphicPaymentPayableItemLink', which can either inherit from PolymorphicLink<IPayableItem, PolymorphicPayment>
        //    //or otherwise implement IPolymorphicLink<IPayableItem, PolymorphicPayment>.

        //    private ICollection<PolymorphicPaymentPayableItemLink> _PayableItem = new List<PolymorphicPaymentPayableItemLink>();

        //    [Hidden]
        //    public virtual ICollection<PolymorphicPaymentPayableItemLink> PayableItemLinks
        //    {
        //        get { return _PayableItem; }
        //        set { _PayableItem = value; }
        //    }

        //    /// <summary>
        //    ///     This is an optional, derrived collection, which shows the associated objects directly.
        //    ///     It is more convenient for the user, but each element is resolved separately, so more
        //    ///     expensive in processing terms.  Use this pattern only on smaller collections.
        //    /// </summary>
        //    [NotPersisted]
        //    public ICollection<IPayableItem> PayableItems
        //    {
        //        get { return PayableItemLinks.Select(x => x.AssociatedRoleObject).ToList(); }
        //    }

        //    public void AddPayableItem(IPayableItem value)
        //    {
        //        PolymorphicNavigator.AddLink<PolymorphicPaymentPayableItemLink, IPayableItem, PolymorphicPayment>(value, this);
        //    }

        //    public void RemovePayableItem(IPayableItem value)
        //    {
        //        PolymorphicNavigator.RemoveLink<PolymorphicPaymentPayableItemLink, IPayableItem, PolymorphicPayment>(value, this);
        //    }

        //    #endregion
        //}

        //public class PolymorphicPaymentPayeeLink : PolymorphicLinkWithOid<IPayee, PolymorphicPayment> { }


        //public class CustomerAsPayee : IPayee
        //{
        //    public IPolymorphicNavigatorWithOid PolymorphicNavigator { set; protected get; }

        //    [Disabled]
        //    public virtual int Id { get; set; }

        //    public IQueryable<PolymorphicPayment> PaymentsToThisPayee()
        //    {
        //        return PolymorphicNavigator.FindOwners<PolymorphicPaymentPayeeLink, IPayee, PolymorphicPayment>(this);
        //    }
        //}


        //public class SupplierAsPayee : IPayee
        //{
        //    [Disabled, Key, Column(Order = 1)]
        //    public virtual int Id1 { get; set; }

        //    [Disabled, Key, Column(Order = 2)]
        //    public virtual int Id2 { get; set; }
        //}

        //public interface IPayableItem { };

        //public class PolymorphicPaymentPayableItemLink : PolymorphicLinkWithOid<IPayableItem, PolymorphicPayment> { }

        //public class InvoiceAsPayableItem : IPayableItem
        //{
        //    public IPolymorphicNavigatorWithOid PolymorphicNavigator { set; protected get; }

        //    [Disabled]
        //    public virtual int Id { get; set; }

        //    public IQueryable<PolymorphicPayment> PaymentsContainingThis()
        //    {
        //        return PolymorphicNavigator.FindOwners<PolymorphicPaymentPayableItemLink, IPayableItem, PolymorphicPayment>(this);
        //    }
        //}


        //public class ExpenseClaimAsPayableItem : IPayableItem
        //{
        //    [Disabled, Key, Column(Order = 1)]
        //    public virtual int Id1 { get; set; }

        //    [Disabled, Key, Column(Order = 2)]
        //    public virtual int Id2 { get; set; }
        //}

        //#endregion

        //#region Code First DBContext

        //public class MyContext : DbContext
        //{
        //    public MyContext(string name) : base(name) { }
        //    public MyContext() { }
        //    public DbSet<PolymorphicPayment> Payments { get; set; }
        //    public DbSet<CustomerAsPayee> Customers { get; set; }
        //    public DbSet<SupplierAsPayee> Suppliers { get; set; }
        //    public DbSet<InvoiceAsPayableItem> Invoices { get; set; }
        //    public DbSet<ExpenseClaimAsPayableItem> ExpenseClaims { get; set; }

        //    protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //    {
        //        //Initialisation
        //        //Use the Naked Objects > DbInitialiser template to add a custom initialiser, then reference thus:
        //        Database.SetInitializer(new DropCreateDatabaseAlways<MyContext>());

        //        //Mappings
        //        //Use the Naked Objects > Mapping template to add mapping classes & reference them thus:
        //        //modelBuilder.Configurations.Add(new Employee_Mapping());
        //    }
        //}

        //#endregion
    }
}