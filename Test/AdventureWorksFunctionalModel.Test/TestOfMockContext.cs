using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using NakedFunctions.Test;

namespace AdventureWorksFunctionalModel.Test
{
    [TestClass]
    public class TestOfMockContext
    {
        [TestMethod]
       public void TestServices()
        {
            var a = new MockAlert();
            var cl = new MockClock();
            var r = new MockRandomSeedGenerator(1);
            var con = new MockContext()
                .WithService<IAlert, MockAlert>(a)
                .WithService<IClock, MockClock>(cl)
                .WithService(r);//No interface specified

            Assert.AreEqual(a, con.GetService<IAlert>());
            Assert.AreEqual(cl, con.GetService<IClock>());
            Assert.AreEqual(r, con.GetService<MockRandomSeedGenerator>());
            try
            {
                Assert.AreEqual(null, con.GetService<MockAlert>());
                Assert.Fail("Should not get to here");
            }
            catch (KeyNotFoundException)
            {
            }
            try
            {
                Assert.AreEqual(null, con.GetService<MockClock>());
                Assert.Fail("Should not get to here");
            }
            catch (KeyNotFoundException)
            {
            }
            try
            {
                Assert.AreEqual(null, con.GetService<IRandomSeedGenerator>());
                Assert.Fail("Should not get to here");
            }
            catch (KeyNotFoundException)
            {
            }
        }

        [TestMethod]
        public void TestInstances()
        {
            var instances = new object[] { new A(1), new B(1), new A(2), new A(3), new B(2)};
            var con = new MockContext().WithInstances(instances).WithInstances(instances);
            var a_s = con.Instances<A>();
            Assert.AreEqual(6, a_s.Count());
            var b_s = con.Instances<B>();
            Assert.AreEqual(4, b_s.Count());
            Assert.AreEqual(10, con.AllInstances.Count());
        }

        [TestMethod]
        public void TestWithNew()
        {
            var a1 = new A(1);
            var con = new MockContext().WithNew(a1);
            Assert.AreEqual(0, con.Instances<A>().Count());
            var a2 = new A(2);
            con = con.WithSavedNew(a1, a2);
            Assert.AreEqual(1, con.Instances<A>().Count());
            Assert.AreEqual(a2, con.Instances<A>().First());
        }


        [TestMethod]
        public void TestReloadOnNew()
        {
            var a1 = new A(1);
            var con = new MockContext().WithNew(a1);
            var a2 = new A(2);
            con = con.WithSavedNew(a1, a2);
            Assert.AreEqual(a2, con.Reload(a1));
        }

        [TestMethod]
        public void TestWithUpdated()
        {
            var a1 = new A(1);
            var a2 = new A(2);
            var con = new MockContext().WithInstances(a1).WithUpdated(a1, a2);
           
            var a_s = con.Instances<A>();
            Assert.AreEqual(1, a_s.Count());
            Assert.AreEqual(a2, a_s.First());
        }

        [TestMethod]
        public void TestReloadOnUpdated()
        {
            var a1 = new A(1);
            var a2 = new A(2);
            var con = new MockContext().WithInstances(a1).WithUpdated(a1, a2);
            var a3 = new A(3);
            con = con.WithSavedUpdated(a1, a2, a3);
            var a_s = con.Instances<A>();
            Assert.AreEqual(1, a_s.Count());
            Assert.AreEqual(a3, a_s.First());

            Assert.AreEqual(a3, con.Reload(a1));
            Assert.AreEqual(a3, con.Reload(a2));
        }

        [TestMethod]
        public void TestWithReplacementForAnAssociatedObject()
        {
            var a1 = new A(1);
            var a2 = new A(2);
            var con = new MockContext().WithInstances(a1, a2);
            var a3 = new A(3);
            con = con.WithReplacementForAnAssociatedObject(a2, a3);
            var a_s = con.Instances<A>();
            Assert.AreEqual(2, a_s.Count());
            Assert.AreEqual(a3, a_s.Last());


        }

        [TestMethod]
        public void TestWithDeleted()
        {
            var instances = new object[] { new A(1), new B(1), new A(2), new A(3), new B(2) };
            var con = new MockContext().WithInstances(instances).WithInstances(instances);
            var a_s = con.Instances<A>();
            Assert.AreEqual(6, a_s.Count());
            var b_s = con.Instances<B>();
            Assert.AreEqual(4, b_s.Count());
            var con2 = con.WithDeleted(a_s.First());
            Assert.AreEqual(5, con2.Instances<A>().Count());
        }

       // [TestMethod]



    }

    internal class A { public A(int i) => this.i = i; int i; public override string ToString() => i.ToString(); }
    internal class B { public B(int i) => this.i = i; int i; public override string ToString() => i.ToString(); }

}
