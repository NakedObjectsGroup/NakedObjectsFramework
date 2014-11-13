// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.ComponentModel.DataAnnotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Services;
using NakedObjects.Xat;

namespace NakedObjects.SystemTest.Other {
    /// <summary>
    /// 
    /// </summary>
    [TestClass, Ignore] //Ignore pending deletion of KeyFacet #5168
    public class TestKeyAttribute : AcceptanceTestCase {
        /// <summary>
        /// Assumes that a SimpleRepository for the type T has been registered in Services
        /// </summary>
        protected ITestObject NewTestObject<T>() {
            return GetTestService(typeof (T).Name + "s").GetAction("New Instance").InvokeReturnObject();
        }

        [TestMethod]
        public void InMemoryPersistorSetsKeyValuesAutomatically() {
            var foo1 = GetTestService("Foos").GetAction("New Instance").InvokeReturnObject();
            var id1 = foo1.GetPropertyByName("Id1").AssertValueIsEqual("0");
            var id2 = foo1.GetPropertyByName("Id2").AssertValueIsEqual("0");

            foo1.Save();
            id1.AssertTitleIsEqual("1");
            id2.AssertValueIsEqual("0");

            var foo2 = GetTestService("Foos").GetAction("New Instance").InvokeReturnObject();
            foo2.Save();
            foo2.GetPropertyByName("Id1").AssertValueIsEqual("2");
        }


        [TestMethod]
        public void SimpleRepositoryFindByKey() {
            var foos = GetTestService("Foos");
            var foo1 = foos.GetAction("New Instance").InvokeReturnObject();
            foo1.GetPropertyByName("Name").SetValue("Foo 1");
            foo1.Save();
            var id1 = foo1.GetPropertyByName("Id1").AssertValueIsEqual("1");

            var foo2 = foos.GetAction("New Instance").InvokeReturnObject();
            foo2.GetPropertyByName("Name").SetValue("Foo 2");
            foo2.Save();

            var foo3 = foos.GetAction("New Instance").InvokeReturnObject();
            foo3.GetPropertyByName("Name").SetValue("Foo 3");
            foo3.Save();

            foos.GetAction("Find By Key").InvokeReturnObject(1).AssertTitleEquals("Foo 1");
            foos.GetAction("Find By Key").InvokeReturnObject(3).AssertTitleEquals("Foo 3");
        }

        #region Setup/Teardown

        [TestInitialize()]
        public void SetupTest() {
            InitializeNakedObjectsFramework(this);
        }

        [TestCleanup()]
        public void TearDownTest() {
            CleanupNakedObjectsFramework(this);
        }

        #endregion

        #region "Services & Fixtures"

        protected override object[] Fixtures {
            get { return (new object[] {}); }
        }

        protected override object[] MenuServices {
            get { return (new object[] {new SimpleRepository<Foo>()}); }
        }

        #endregion
    }

    #region Objects used in tests

    public class Foo {
        [Key]
        public virtual int Id1 { get; set; }

        public virtual int Id2 { get; set; }

        [Optionally, Title]
        public virtual string Name { get; set; }
    }

    #endregion
}