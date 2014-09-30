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
using NakedObjects.Util;

namespace NakedObjects.SystemTest {
    public abstract class AbstractSystemTest<TContext> : AcceptanceTestCase 
        where TContext : DbContext
    {

        #region Run Configuration
        protected override void RegisterTypes(IUnityContainer container)
        {
            base.RegisterTypes(container);
            var config = new EntityObjectStoreConfiguration { EnforceProxies = false };
            config.UsingCodeFirstContext(() => Activator.CreateInstance<TContext>());
            container.RegisterInstance<IEntityObjectStoreConfiguration>(config, (new ContainerControlledLifetimeManager()));
        }
        #endregion

        /// <summary>
        /// Assumes that a SimpleRepository for the type T has been registered in Services
        /// </summary>
        protected ITestObject NewTestObject<T>() {
            return GetTestService<T>().GetAction("New Instance").InvokeReturnObject();
        }

        private ITestService GetTestService<T>()
        {
            var name = NameUtils.NaturalName(typeof(T).Name) + "s";
            return GetTestService(name);
        }

        protected ITestObject GetAllInstances<T>(int number)
        {
            return GetTestService<T>().GetAction("All Instances").InvokeReturnCollection().ElementAt(number);
        }

        protected ITestObject GetAllInstances(string simpleRepositoryName, int number)
        {
            return GetTestService(simpleRepositoryName).GetAction("All Instances").InvokeReturnCollection().ElementAt(number);
        }

        protected ITestObject FindById<T>(int id)
        {
            return GetTestService<T>().GetAction("Find By Key").InvokeReturnObject(id);
        }

        protected ITestObject FindById(string simpleRepositoryName, int id)
        {
            return GetTestService(simpleRepositoryName).GetAction("Find By Key").InvokeReturnObject(id);
        }
    }
}