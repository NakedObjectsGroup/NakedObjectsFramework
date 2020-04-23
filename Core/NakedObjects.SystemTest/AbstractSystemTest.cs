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
using NakedObjects.Menu;
using NakedObjects.Persistor.Entity.Configuration;
using NakedObjects.Util;
using NakedObjects.Xat;
using NUnit.Framework;

namespace NakedObjects.SystemTest {
    //#region Nested type: NullMenuFactory

    //public class NullMenuFactory : IMenuFactory {
    //    public IMenu NewMenu(string name) => throw new NotImplementedException();

    //    #region IMenuFactory Members

    //    public IMenu NewMenu<T>(bool addAllActions, string name = null) => throw new NotImplementedException();

    //    public IMenu NewMenu(Type type, bool addAllActions = false, string name = null) => throw new NotImplementedException();

    //    #endregion
    //}

    //#endregion

    public static class Constants {
        public static string AppveyorServer => @"(local)\SQL2017";
        public static string LocalServer => @".\SQLEXPRESS";
        public static string Server => AppveyorServer;
    }

    public abstract class AbstractSystemTest<TContext> : AcceptanceTestCase
        where TContext : DbContext {
        protected override Type[] Types {
            get {
                return new[] {
                    typeof(List<object>),
                    typeof(EntityCollection<object>),
                    typeof(ObjectQuery<object>)
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

        /// <summary>
        ///     Assumes that a SimpleRepository for the type T has been registered in Services
        /// </summary>
        protected ITestObject NewTestObject<T>() => GetSimpleRepositoryTestService<T>().GetAction("New Instance").InvokeReturnObject();

        private ITestService GetSimpleRepositoryTestService<T>() {
            var name = NameUtils.NaturalName(typeof(T).Name) + "s";
            return GetTestService(name);
        }

        protected ITestObject GetAllInstances<T>(int number) => GetSimpleRepositoryTestService<T>().GetAction("All Instances").InvokeReturnCollection().ElementAt(number);

        //protected ITestObject GetAllInstances(string simpleRepositoryName, int number) => GetTestService(simpleRepositoryName).GetAction("All Instances").InvokeReturnCollection().ElementAt(number);

        protected ITestObject GetAllInstances(Type repositoryType, int number) => GetTestService(repositoryType).GetAction("All Instances").InvokeReturnCollection().ElementAt(number);

        protected ITestObject FindById<T>(int id) => GetSimpleRepositoryTestService<T>().GetAction("Find By Key").InvokeReturnObject(id);

        //protected ITestObject FindById(string simpleRepositoryName, int id) => GetTestService(simpleRepositoryName).GetAction("Find By Key").InvokeReturnObject(id);

        protected static void IsInstanceOfType(object obj, Type typ) {
            Assert.IsTrue(typ.IsInstanceOfType(obj), $"{obj.GetType()} isn't a {typ}");
        }
    }
}