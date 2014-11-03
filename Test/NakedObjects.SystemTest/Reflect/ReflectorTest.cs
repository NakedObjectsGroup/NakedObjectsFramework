// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventureWorksModel;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Configuration;
using NakedObjects.Architecture.Menu;
using NakedObjects.Core.Configuration;
using NakedObjects.Meta;


namespace NakedObjects.Reflect.Test {

    [TestClass]
    public class ReflectorTest {
        protected IUnityContainer GetContainer() {
            var c = new UnityContainer();
            RegisterTypes(c);
            return c;
        }

        protected virtual void RegisterTypes(IUnityContainer container) {
            container.RegisterType<IMenuBuilder, NullMenuBuilder>();
            container.RegisterType<ISpecificationCache, ImmutableInMemorySpecCache>();
            container.RegisterType<IClassStrategy, DefaultClassStrategy>();
            container.RegisterType<IFacetFactorySet, FacetFactorySet>();
            container.RegisterType<IReflector, Reflector>();
            container.RegisterType<IMetamodel, Metamodel>();
            container.RegisterType<IMetamodelBuilder, Metamodel>();
            container.RegisterType<IServicesConfiguration, ServicesConfiguration>();
        }

        [TestMethod]
        public void ReflectNoTypes() {
            var container = GetContainer();
            var rc = new ReflectorConfiguration(new Type[] {}, new Type[] {}, new Type[] {}, new Type[] {});

            container.RegisterInstance<IReflectorConfiguration>(rc);

            var reflector = container.Resolve<IReflector>();
            reflector.Reflect();
            Assert.IsFalse(reflector.AllObjectSpecImmutables.Any());
        }

        private static Type[] AdventureWorksTypes() {
            var allTypes = AppDomain.CurrentDomain.GetAssemblies().Single(a => a.GetName().Name == "AdventureWorksModel").GetTypes();
            return allTypes.Where(t => t.BaseType == typeof(AWDomainObject) && !t.IsAbstract).ToArray();
        }

        [TestMethod]
        public void ReflectAdventureworks() {
            // load adventurework

            AssemblyHook.EnsureAssemblyLoaded();

            var types = AdventureWorksTypes();
            var container = GetContainer();
            var rc = new ReflectorConfiguration(types, new Type[] { }, new Type[] { }, new Type[] { });

            container.RegisterInstance<IReflectorConfiguration>(rc);

            var reflector = container.Resolve<IReflector>();
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            reflector.Reflect();
            stopwatch.Stop();
            var interval = stopwatch.Elapsed;

            Assert.IsTrue(reflector.AllObjectSpecImmutables.Any());
            Assert.IsTrue(interval.Milliseconds < 1000);
        }   

        #region Nested type: NullMenuBuilder

        public class NullMenuBuilder : IMenuBuilder {
            #region IMenuBuilder Members

            public IMenu[] DefineMainMenus() {
                return new IMenu[] {};
            }

            #endregion
        }

        #endregion

    }
}