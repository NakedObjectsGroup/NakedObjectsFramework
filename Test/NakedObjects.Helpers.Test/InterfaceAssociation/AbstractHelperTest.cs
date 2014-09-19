using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.EntityObjectStore;
using NakedObjects.Xat;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NakedObjects.Helpers.Test
{

    public abstract class AbstractHelperTest<TContext> : AcceptanceTestCase
        where TContext : DbContext
    {
        #region Run Configuration
        protected override void RegisterTypes(IUnityContainer container)
        {
            base.RegisterTypes(container);
            var config = new EntityObjectStoreConfiguration { EnforceProxies = false };
            config.UsingCodeFirstContext(() => Activator.CreateInstance<TContext>() );
            container.RegisterInstance(config, (new ContainerControlledLifetimeManager()));
        }
        #endregion

        #region Helper methods
        protected ITestObject GetAllInstances(string simpleRepositoryName, int number)
        {
            return GetTestService(simpleRepositoryName).GetAction("All Instances").InvokeReturnCollection().ElementAt(number);
        }

        protected ITestObject FindById(string simpleRepositoryName, int id)
        {
            return GetTestService(simpleRepositoryName).GetAction("Find By Key").InvokeReturnObject(id);
        }
        #endregion
    }
}
