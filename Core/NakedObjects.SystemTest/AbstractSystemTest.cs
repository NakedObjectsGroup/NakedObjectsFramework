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
using NakedObjects.Persistor.Entity.Configuration;
using NakedObjects.Util;
using NakedObjects.Xat;
using NUnit.Framework;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace NakedObjects.SystemTest {
    public static class Constants {
        public static string AppveyorServer => @"(local)\SQL2017";
        public static string LocalServer => @".\SQLEXPRESS";

#if APPVEYOR
        public static string Server => AppveyorServer;
#else
        public static string Server => LocalServer;
#endif
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

        private ITestService GetSimpleRepositoryTestService<T>() => GetTestService($"{NameUtils.NaturalName(typeof(T).Name)}s");

        protected ITestObject GetAllInstances<T>(int number) => GetSimpleRepositoryTestService<T>().GetAction("All Instances").InvokeReturnCollection().ElementAt(number);

        protected ITestObject GetAllInstances(Type repositoryType, int number) => GetTestService(repositoryType).GetAction("All Instances").InvokeReturnCollection().ElementAt(number);

        protected ITestObject FindById<T>(int id) => GetSimpleRepositoryTestService<T>().GetAction("Find By Key").InvokeReturnObject(id);

        protected static void IsInstanceOfType(object obj, Type typ) {
            Assert.IsTrue(typ.IsInstanceOfType(obj), $"{obj.GetType()} isn't a {typ}");
        }
    }
}