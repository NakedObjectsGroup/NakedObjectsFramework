// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
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
            container.RegisterType<IMainMenuDefinition, NullMenuDfinition>();

            container.RegisterType<IMenuFactory, NullMenuBuilder>();
            container.RegisterType<ISpecificationCache, ImmutableInMemorySpecCache>(new ContainerControlledLifetimeManager(), new InjectionConstructor());
            container.RegisterType<IClassStrategy, DefaultClassStrategy>();
            container.RegisterType<IFacetFactorySet, FacetFactorySet>();
            container.RegisterType<IReflector, Reflector>();
            container.RegisterType<IMetamodel, Metamodel>();
            container.RegisterType<IMetamodelBuilder, Metamodel>();
            container.RegisterType<IServicesConfiguration, ServicesConfiguration>();
        }

        public class NullMenuDfinition : IMainMenuDefinition {
            public IMenu[] MainMenus(IMenuFactory factory) {
                return new IMenu[]{};
            }
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
            return allTypes.Where(t => t.BaseType == typeof (AWDomainObject) && !t.IsAbstract).ToArray();
        }

        [TestMethod]
        public void ReflectAdventureworks() {
            // load adventurework

            AssemblyHook.EnsureAssemblyLoaded();

            var types = AdventureWorksTypes();
            var container = GetContainer();
            var rc = new ReflectorConfiguration(types, new Type[] {}, new Type[] {}, new Type[] {});

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


        [TestMethod]
        public void SerializeAdventureworks() {
            // load adventurework

            AssemblyHook.EnsureAssemblyLoaded();

            var types = AdventureWorksTypes();
            var container = GetContainer();
            var rc = new ReflectorConfiguration(types, new Type[] {}, new Type[] {}, new Type[] {});

            container.RegisterInstance<IReflectorConfiguration>(rc);

            var reflector = container.Resolve<IReflector>();

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            reflector.Reflect();
            stopwatch.Stop();
            var reflectInterval = stopwatch.Elapsed;
            stopwatch.Reset();

            Assert.IsTrue(reflector.AllObjectSpecImmutables.Any());

            var cache = container.Resolve<ISpecificationCache>();

            TimeSpan serializeInterval;
            using (var fs = File.Open(@"c:\testmetadata\metadataAW.bin", FileMode.OpenOrCreate)) {
                IFormatter formatter = new BinaryFormatter();

                stopwatch.Start();
                formatter.Serialize(fs, cache);
                stopwatch.Stop();
                serializeInterval = stopwatch.Elapsed;
                stopwatch.Reset();
            }

            // and roundtrip 

            ISpecificationCache newCache;

            TimeSpan deserializeInterval;
            using (var fs = File.Open(@"c:\testmetadata\metadataAW.bin", FileMode.Open)) {
                IFormatter formatter = new BinaryFormatter();

                stopwatch.Start();
                newCache = (ISpecificationCache) formatter.Deserialize(fs);

                stopwatch.Stop();
                deserializeInterval = stopwatch.Elapsed;
                stopwatch.Reset();
            }

            Assert.AreEqual(cache.AllSpecifications().Count(), newCache.AllSpecifications().Count());

            var zipped = cache.AllSpecifications().Zip(newCache.AllSpecifications(), (a, b) => new {a, b});

            foreach (var item in zipped) {
                Assert.AreEqual(item.a.FullName, item.b.FullName);

                Assert.AreEqual(item.a.GetFacets().Count(), item.b.GetFacets().Count());

                var zipfacets = item.a.GetFacets().Zip(item.b.GetFacets(), (x, y) => new {x, y});

                foreach (var zipfacet in zipfacets) {
                    Assert.AreEqual(zipfacet.x.FacetType, zipfacet.y.FacetType);
                }
            }

            Console.WriteLine(string.Format("reflect: {0} serialize {1} deserialize {2} ", reflectInterval, serializeInterval, deserializeInterval));
        }

        #region Nested type: NullMenuBuilder

        public class NullMenuBuilder : IMenuFactory {
            #region IMenuBuilder Members

            public IMenu[] DefineMainMenus() {
                return new IMenu[] {};
            }

            #endregion

            public IMenu NewMenu(string name) {
                throw new NotImplementedException();
            }

            public ITypedMenu<T> NewMenu<T>(bool addAllActions, string name = null) {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}