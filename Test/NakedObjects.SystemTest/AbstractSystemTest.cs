// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Data.Entity;
using System.Linq;
using Microsoft.Practices.Unity;
using NakedObjects.Architecture.Menu;
using NakedObjects.Reflect.Test;
using NakedObjects.Util;
using NakedObjects.Xat;
using NakedObjects.Persistor.Entity.Configuration;

namespace NakedObjects.SystemTest {
    public abstract class AbstractSystemTest<TContext> : AcceptanceTestCase
        where TContext : DbContext {
        #region Run Configuration

        protected override void RegisterTypes(IUnityContainer container) {
            base.RegisterTypes(container);
            var config = new EntityObjectStoreConfiguration {EnforceProxies = false};
            config.UsingCodeFirstContext(Activator.CreateInstance<TContext>);
            container.RegisterInstance<IEntityObjectStoreConfiguration>(config, (new ContainerControlledLifetimeManager()));
            container.RegisterType<IMainMenuDefinition, ReflectorTest.NullMenuDfinition>();
            container.RegisterType<IMenuFactory, ReflectorTest.NullMenuFactory>();
        }

        #endregion

        private bool initialized;

        protected void InitializeNakedObjectsFrameworkOnce() {
            if (!initialized) {
                InitializeNakedObjectsFramework(this);
                initialized = true;
            }
        }

        /// <summary>
        /// Assumes that a SimpleRepository for the type T has been registered in Services
        /// </summary>
        protected ITestObject NewTestObject<T>() {
            return GetTestService<T>().GetAction("New Instance").InvokeReturnObject();
        }

        private ITestService GetTestService<T>() {
            var name = NameUtils.NaturalName(typeof (T).Name) + "s";
            return GetTestService(name);
        }

        protected ITestObject GetAllInstances<T>(int number) {
            return GetTestService<T>().GetAction("All Instances").InvokeReturnCollection().ElementAt(number);
        }

        protected ITestObject GetAllInstances(string simpleRepositoryName, int number) {
            return GetTestService(simpleRepositoryName).GetAction("All Instances").InvokeReturnCollection().ElementAt(number);
        }

        protected ITestObject GetAllInstances(Type repositoryType, int number) {
            return GetTestService(repositoryType).GetAction("All Instances").InvokeReturnCollection().ElementAt(number);
        }

        protected ITestObject FindById<T>(int id) {
            return GetTestService<T>().GetAction("Find By Key").InvokeReturnObject(id);
        }

        protected ITestObject FindById(string simpleRepositoryName, int id) {
            return GetTestService(simpleRepositoryName).GetAction("Find By Key").InvokeReturnObject(id);
        }
    }
}