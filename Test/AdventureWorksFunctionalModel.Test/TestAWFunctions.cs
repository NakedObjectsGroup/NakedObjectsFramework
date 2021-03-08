using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using NakedFunctions.Test;
using AW.Types;
using AW.Functions;

namespace AdventureWorksFunctionalModel.Test
{
    [TestClass]
    public class TestAWFunctions
    {

        [TestMethod]
        public void MenuFunctionQueryOnly()
        {
            var a1 = new Address();
            var a2 = new Address();
            var a3 = new Address();
            var c = new MockContext().WithInstances(a1,a2,a3);
            var result = Address_MenuFunctions.RecentAddresses(c).ToList();
            CollectionAssert.AreEqual(new List<Address> { a1, a2, a3 }, result);
        }

        [TestMethod]
        public void MenuFunctionCreateNew()
        {
            var now = DateTime.Now;
            var g = new Guid();
            var c = new MockContext()
                .WithService<IClock>(new MockClock(now))
                .WithService<IGuidGenerator>(new MockGuidGenerator(g));

            var sp = new StateProvince { Name = "UK" };
            //c = c.WithNewlySaved<Address>(a => a with { /* update Id, add to  */})
            var (a, c2) = Address_MenuFunctions.CreateNewAddress(null, "l1", "l2", "ct", "pc", sp, c);
            Assert.IsInstanceOfType(a, typeof(Address));
            Assert.AreEqual("l1", a.AddressLine1);
            Assert.AreEqual("l2", a.AddressLine2);
            Assert.AreEqual("ct", a.City);
            Assert.AreEqual("pc", a.PostalCode);
            Assert.AreEqual("UK", a.StateProvince.ToString());
            Assert.AreEqual("l1...", a.ToString());
            Assert.AreEqual(a, c2.Instances<Address>().First());
            Assert.AreEqual(now, c2.Instances<Address>().First().ModifiedDate);
        }

        [TestMethod]
        public void MenuFunctionCreateNewWithSimulatedId()
        {
            var now = DateTime.Now;
            var g = new Guid();
            var c = new MockContext()
                .WithService<IClock>(new MockClock(now))
                .WithService<IGuidGenerator>(new MockGuidGenerator(g));

            var sp = new StateProvince { Name = "UK" };
            c = c.WithOnSavingNew<Address>(a => a with { AddressID = 1});
            var (a, c2) = Address_MenuFunctions.CreateNewAddress(null, "l1", "l2", "ct", "pc", sp, c);
            Assert.AreEqual(0, a.AddressID);
            a = c2.Reload(a);
            Assert.AreEqual(1, a.AddressID);
        }

        [TestMethod]
        public void ObjectEditFunction()
        {
            var e = new Employee { NationalIDNumber = "1234" };
            var now = DateTime.Now;
            var c = new MockContext()
                .WithInstances(e)
                .WithService<IClock>(new MockClock(now));

           var c2 = e.UpdateNationalIDNumber("2345", c);
            var e2 = c2.Reload(e);
            Assert.AreEqual("2345", e2.NationalIDNumber);
        }

        [TestMethod, Ignore] //Not yet working
        public void WithDeferredFunction()
        {
            var now = DateTime.Now;
            var g = new Guid();
            var c = new MockContext()
                .WithService<IClock>(new MockClock(now))
                .WithService<IGuidGenerator>(new MockGuidGenerator(g));

            var cust = new Customer();
            var prod = new Product { ListPrice = 9.99m };


            var (soh, c2)  = Order_AdditionalFunctions.QuickOrder(cust, prod, 3, c);
            var det = c2.Instances<SalesOrderDetail>().First();
            c.WithReplacement(soh, soh with { Details = soh.Details.WithAdded(det) });
            c.ExecuteDeferred();
            Assert.AreEqual(0, soh.TotalDue);
            var soh2 = c.Instances<SalesOrderHeader>().First();
            Assert.AreEqual(29.97, soh2.TotalDue);
        }

        
    }

}
