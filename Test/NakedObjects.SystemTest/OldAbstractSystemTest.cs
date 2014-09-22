// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System.Linq;
using NakedObjects.Xat;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Practices.Unity;
using NakedObjects.EntityObjectStore;
using System;
using System.Data.Entity;

namespace NakedObjects.SystemTest {
    public abstract class OldAbstractSystemTest : AcceptanceTestCase 
    {



        /// <summary>
        /// Assumes that a SimpleRepository for the type T has been registered in Services
        /// </summary>
        protected ITestObject NewTestObject<T>() {
            return GetTestService(typeof (T).Name + "s").GetAction("New Instance").InvokeReturnObject();
        }

    }
}