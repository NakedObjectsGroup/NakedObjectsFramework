// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System.Linq;
using NakedObjects.Xat;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NakedObjects.SystemTest {
    public abstract class AbstractSystemTest : AcceptanceTestCase {
        #region Constructors

        public AbstractSystemTest(string name) : base(name) {}

        public AbstractSystemTest() : this(typeof (AbstractSystemTest).Name) {}

        #endregion

        /// <summary>
        /// Assumes that a SimpleRepository for the type T has been registered in Services
        /// </summary>
        protected ITestObject NewTestObject<T>() {
            return GetTestService(typeof (T).Name + "s").GetAction("New Instance").InvokeReturnObject();
        }

        /// <summary>
        /// Assumes that a SimpleRepository for the type T has been registered in Services.
        /// Throws error if more than one match found.
        /// </summary>
        protected ITestObject FindTestObjectByTitle<T>(string match) {
            ITestCollection results = GetTestService(typeof (T).Name + "s").GetAction("Find By Title").InvokeReturnCollection(match);
            Assert.AreEqual(1, results.Count());
            return results.FirstOrDefault();
        }
    }
}