// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System.Collections.Generic;
using System.Linq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Boot;
using NakedObjects.Core.Context;
using NakedObjects.Core.NakedObjectsSystem;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Reflector.Spec;
using TestClass.Attributes.Contributed;

namespace NakedObjects.SystemTest.Attributes {
    [TestClass]
    public class TestExcludeFromFindMenuAttribute : AbstractSystemTest {
        #region Setup/Teardown

        [TestInitialize()]
        public void SetUp() {
            InitializeNakedObjectsFramework();
        }

        [TestCleanup()]
        public void TearDown() {
            CleanupNakedObjectsFramework();
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
            public Object1 FinderAction1() {
                return null;
            }

            public ICollection<Object1> FinderAction2() {
                return null;
            }

            [ExcludeFromFindMenu]
            public Object1 NotFinderAction1() {
                return null;
            }

            [ExcludeFromFindMenu]
            public ICollection<Object1> NotFinderAction2() {
                return null;
            }

            [ExcludeFromFindMenu]
            public Object1 NewObject1() {
                return new Object1();
            }
        }

        #endregion

        [TestMethod]
        public virtual void Contributed() {
            var service = (TestService) GetTestService("Test Service").NakedObject.Object;
            Object1 obj = service.NewObject1();
            INakedObject adapter = NakedObjectsContext.ObjectPersistor.CreateAdapter(obj, null, null);
            INakedObjectAction[] actions = adapter.Specification.GetRelatedServiceActions();

            Assert.AreEqual(1, actions.Count());
            Assert.IsTrue(actions[0] is NakedObjectActionSet);
            Assert.AreEqual(2, actions[0].Actions.Count());
            Assert.IsTrue(actions[0].Actions[0] is NakedObjectActionImpl);
            Assert.IsTrue(actions[0].Actions[1] is NakedObjectActionImpl);
            Assert.AreEqual("Finder Action1", actions[0].Actions[0].Name);
            Assert.AreEqual("Finder Action2", actions[0].Actions[1].Name);
        }

    }

    
}

namespace TestClass.Attributes.Contributed {
    public class Object1 {}
}