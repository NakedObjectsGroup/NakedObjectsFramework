// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Reflect;
using NakedObjects.Boot;
using NakedObjects.Core.Context;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Core.Persist;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using NakedObjects.Reflector.Spec;
using TestClass.Attributes.NotContributed;

namespace NakedObjects.SystemTest.Attributes.NotContributed {

    [TestClass, Ignore]
    public class TestNotContributedActionAttribute : AbstractSystemTest {
        #region Setup/Teardown

        [TestInitialize()]
        public void SetUp() {
            InitializeNakedObjectsFramework(this);
            
        }

        [TestCleanup()]
        public void TearDown() {
            CleanupNakedObjectsFramework(this);
           
        }

        #endregion

        #region "Services & Fixtures"

        protected override IFixturesInstaller Fixtures {
            get { return new FixturesInstaller(new object[] {}); }
        }

        protected override IServicesInstaller MenuServices {
            get {
                return new ServicesInstaller(new object[] {
                                                              new TestService()
                                                          });
            }
        }

        public class TestService {
            public void ContributedAction(Object1 obj) {}

            [NotContributedAction]
            public void NotContributedAction(Object1 obj) {}

            public Object1 NewObject1() {
                return new Object1();
            }
        }

        #endregion

       

        [TestMethod]
        public virtual void Contributed() {

            var service = (TestService) GetTestService("Test Service").NakedObject.Object;
            var obj = service.NewObject1();
            var adapter = NakedObjectsFramework.ObjectPersistor.CreateAdapter(obj, null, null);
            var actions = adapter.Specification.GetObjectActions();

            Assert.AreEqual(1, actions.Count());
            Assert.IsTrue(actions[0] is NakedObjectActionSet);
            Assert.AreEqual(1, actions[0].Actions.Count());
            Assert.IsTrue(actions[0].Actions[0] is NakedObjectActionImpl);
            Assert.AreEqual("Contributed Action", actions[0].Actions[0].GetName(NakedObjectsFramework.ObjectPersistor));



        }

    }

    
}

namespace TestClass.Attributes.NotContributed {
    public class Object1 { }

}

