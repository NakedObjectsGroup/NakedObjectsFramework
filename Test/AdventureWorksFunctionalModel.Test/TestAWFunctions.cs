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
            var c = new MockContext();
            var sp = new StateProvince { Name = "UK" };
            var (a, c2) = Address_MenuFunctions.CreateNewAddress(null, "l1", "l2", "ct", "pc", sp, c);
            Assert.IsInstanceOfType(a, typeof(Address));
            Assert.AreEqual("l1", a.AddressLine1);
            Assert.AreEqual("l2", a.AddressLine2);
            Assert.AreEqual("ct", a.City);
            Assert.AreEqual("pc", a.PostalCode);
            Assert.AreEqual("UK", a.StateProvince);
            Assert.AreEqual("l1...", a.ToString());
            Assert.IsFalse(c2.Instances<Address>().Contains(a));
            var a2 = a with { AddressID = 1 };
            var c3 = ((MockContext)c2).WithSavedNew(a, a2);

            Assert.AreEqual(1, c3.Instances<Address>().First().AddressID);
        }

    }

}
