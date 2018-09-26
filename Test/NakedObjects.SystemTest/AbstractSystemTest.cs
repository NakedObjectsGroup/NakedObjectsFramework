// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Linq;
using NakedObjects.Architecture.Menu;
using NakedObjects.Persistor.Entity.Configuration;
using NakedObjects.SystemTest.Reflect;
using NakedObjects.Util;
using NakedObjects.Xat;
using Microsoft.Practices.Unity;


namespace NakedObjects.SystemTest {
    public abstract class AbstractSystemTest<TContext> : AcceptanceTestCase
        where TContext : DbContext {
        private bool initialized;

        protected override Type[] Types {
            get {
                return new[] {
                    typeof (List<object>),
                    typeof (EntityCollection<object>),
                    typeof (ObjectQuery<object>)
                };
            }
        }

        protected override EntityObjectStoreConfiguration Persistor {
            get {
                var config = new EntityObjectStoreConfiguration {EnforceProxies = false};
                config.UsingCodeFirstContext(Activator.CreateInstance<TContext>);
                return config;
            }
        }

        #region Run Configuration

        protected override void RegisterTypes(IUnityContainer container) {
            base.RegisterTypes(container);
            container.RegisterInstance<IEntityObjectStoreConfiguration>(Persistor, (new ContainerControlledLifetimeManager()));
            container.RegisterType<IMenuFactory, ReflectorTest.NullMenuFactory>();
        }

        #endregion

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
            return GetSimpleRepositoryTestService<T>().GetAction("New Instance").InvokeReturnObject();
        }

        private ITestService GetSimpleRepositoryTestService<T>() {
            var name = NameUtils.NaturalName(typeof (T).Name) + "s";
            return GetTestService(name);
        }

        protected ITestObject GetAllInstances<T>(int number) {
            return GetSimpleRepositoryTestService<T>().GetAction("All Instances").InvokeReturnCollection().ElementAt(number);
        }

        protected ITestObject GetAllInstances(string simpleRepositoryName, int number) {
            return GetTestService(simpleRepositoryName).GetAction("All Instances").InvokeReturnCollection().ElementAt(number);
        }

        protected ITestObject GetAllInstances(Type repositoryType, int number) {
            return GetTestService(repositoryType).GetAction("All Instances").InvokeReturnCollection().ElementAt(number);
        }

        protected ITestObject FindById<T>(int id) {
            return GetSimpleRepositoryTestService<T>().GetAction("Find By Key").InvokeReturnObject(id);
        }

        protected ITestObject FindById(string simpleRepositoryName, int id) {
            return GetTestService(simpleRepositoryName).GetAction("Find By Key").InvokeReturnObject(id);
        }
    }
}