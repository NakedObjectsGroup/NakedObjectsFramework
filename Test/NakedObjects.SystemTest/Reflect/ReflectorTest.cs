// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
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
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Menu;
using NakedObjects.Architecture.SpecImmutable;
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
            container.RegisterType<ISpecificationCache, ImmutableInMemorySpecCache>(
                new ContainerControlledLifetimeManager(), new InjectionConstructor());
            container.RegisterType<IClassStrategy, DefaultClassStrategy>();
            container.RegisterType<IFacetFactorySet, FacetFactorySet>();
            container.RegisterType<IReflector, Reflector>();
            container.RegisterType<IMetamodel, Metamodel>();
            container.RegisterType<IMetamodelBuilder, Metamodel>();
            container.RegisterType<IServicesConfiguration, ServicesConfiguration>();
        }

        [TestMethod]
        public void ReflectNoTypes() {
            IUnityContainer container = GetContainer();
            var rc = new ReflectorConfiguration(new Type[] {}, new Type[] {}, new Type[] {}, new Type[] {});

            container.RegisterInstance<IReflectorConfiguration>(rc);

            var reflector = container.Resolve<IReflector>();
            reflector.Reflect();
            Assert.IsFalse(reflector.AllObjectSpecImmutables.Any());
        }

        private static Type[] AdventureWorksTypes() {
            Type[] allTypes =
                AppDomain.CurrentDomain.GetAssemblies()
                    .Single(a => a.GetName().Name == "AdventureWorksModel")
                    .GetTypes();
            return allTypes.Where(t => t.BaseType == typeof (AWDomainObject) && !t.IsAbstract).ToArray();
        }

        [TestMethod]
        public void ReflectAdventureworks() {
            // load adventurework

            AssemblyHook.EnsureAssemblyLoaded();

            Type[] types = AdventureWorksTypes();
            IUnityContainer container = GetContainer();
            var rc = new ReflectorConfiguration(types, new Type[] {}, new Type[] {}, new Type[] {});

            container.RegisterInstance<IReflectorConfiguration>(rc);

            var reflector = container.Resolve<IReflector>();
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            reflector.Reflect();
            stopwatch.Stop();
            TimeSpan interval = stopwatch.Elapsed;

            Assert.IsTrue(reflector.AllObjectSpecImmutables.Any());
            Assert.IsTrue(interval.Milliseconds < 1000);
        }


        [TestMethod]
        public void SerializeAdventureworks() {
            // load adventurework

            AssemblyHook.EnsureAssemblyLoaded();

            Type[] types = AdventureWorksTypes();
            IUnityContainer container = GetContainer();
            var rc = new ReflectorConfiguration(types, new Type[] {}, new Type[] {}, new Type[] {});

            container.RegisterInstance<IReflectorConfiguration>(rc);

            var reflector = container.Resolve<IReflector>();

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            reflector.Reflect();
            stopwatch.Stop();
            TimeSpan reflectInterval = stopwatch.Elapsed;
            stopwatch.Reset();

            Assert.IsTrue(reflector.AllObjectSpecImmutables.Any());

            var cache = container.Resolve<ISpecificationCache>();

            TimeSpan serializeInterval;
            using (FileStream fs = File.Open(@"c:\testmetadata\metadataAW.bin", FileMode.OpenOrCreate)) {
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
            using (FileStream fs = File.Open(@"c:\testmetadata\metadataAW.bin", FileMode.Open)) {
                IFormatter formatter = new BinaryFormatter();

                stopwatch.Start();
                newCache = (ISpecificationCache) formatter.Deserialize(fs);

                stopwatch.Stop();
                deserializeInterval = stopwatch.Elapsed;
                stopwatch.Reset();
            }

            CompareCaches(cache, newCache);

            Console.WriteLine("reflect: {0} serialize {1} deserialize {2} ", reflectInterval, serializeInterval,
                deserializeInterval);
        }

        [TestMethod]
        public void SerializeAdventureworksByType() {
            // load adventurework

            AssemblyHook.EnsureAssemblyLoaded();

            int count = AdventureWorksTypes().Count();

            Type[] spec51 = AdventureWorksTypes().Skip(50).Take(1).ToArray();
            Type[] types = AdventureWorksTypes().Take(51).ToArray();
            IUnityContainer container = GetContainer();
            var rc = new ReflectorConfiguration(spec51, new Type[] {}, new Type[] {}, new Type[] {});

            container.RegisterInstance<IReflectorConfiguration>(rc);

            var reflector = container.Resolve<IReflector>();

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            reflector.Reflect();
            stopwatch.Stop();
            TimeSpan reflectInterval = stopwatch.Elapsed;
            stopwatch.Reset();

            Assert.IsTrue(reflector.AllObjectSpecImmutables.Any());

            var cache = container.Resolve<ISpecificationCache>();

            TimeSpan serializeInterval;
            using (FileStream fs = File.Open(@"c:\testmetadata\metadataAW.bin", FileMode.OpenOrCreate)) {
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
            using (FileStream fs = File.Open(@"c:\testmetadata\metadataAW.bin", FileMode.Open)) {
                IFormatter formatter = new BinaryFormatter();

                stopwatch.Start();
                newCache = (ISpecificationCache) formatter.Deserialize(fs);

                stopwatch.Stop();
                deserializeInterval = stopwatch.Elapsed;
                stopwatch.Reset();
            }

            CompareCaches(cache, newCache);

            Console.WriteLine("reflect: {0} serialize {1} deserialize {2} ", reflectInterval, serializeInterval,
                deserializeInterval);
        }

        [TestMethod]
        public void SerializeAdventureworksFacets() {
            // load adventurework

            AssemblyHook.EnsureAssemblyLoaded();

            int count = AdventureWorksTypes().Count();


            Type[] types50 = AdventureWorksTypes().Take(50).ToArray();
            Type[] types51 = AdventureWorksTypes().Take(51).ToArray();
            IUnityContainer container = GetContainer();
            var rc = new ReflectorConfiguration(types50, new Type[] {}, new Type[] {}, new Type[] {});

            container.RegisterInstance<IReflectorConfiguration>(rc);

            var reflector = container.Resolve<IReflector>();


            reflector.Reflect();

            IObjectSpecImmutable[] cache1 = container.Resolve<ISpecificationCache>().AllSpecifications();


            rc = new ReflectorConfiguration(types51, new Type[] {}, new Type[] {}, new Type[] {});

            container.RegisterInstance<IReflectorConfiguration>(rc);

            reflector = container.Resolve<IReflector>();

            reflector.Reflect();

            IObjectSpecImmutable[] cache2 = container.Resolve<ISpecificationCache>().AllSpecifications();

            int c1 = cache1.Count();
            int c2 = cache2.Count();

            foreach (IObjectSpecImmutable objectSpecImmutable in cache2) {
                if (cache1.Any(s => s.FullName == objectSpecImmutable.FullName)) {
                    //Console.WriteLine("Found {0}", objectSpecImmutable.FullName);
                }
                else {
                    Console.WriteLine("Not Found object spec {0}", objectSpecImmutable.FullName);
                }
            }

            IEnumerable<IFacet> f1 = cache1.SelectMany(s => s.GetFacets()).Distinct();
            IEnumerable<IFacet> f2 = cache2.SelectMany(s => s.GetFacets()).Distinct();

            foreach (IFacet f in f2) {
                if (f1.Any(s => s.GetType() == f.GetType())) {
                    //Console.WriteLine("Found {0}", f.GetType().FullName);
                }
                else {
                    Console.WriteLine("Not Found object facet {0}", f.GetType().FullName);
                }
            }

            f1 =
                cache1.SelectMany(s => s.ObjectActions)
                    .Select(s => s.Spec)
                    .Where(s => s != null)
                    .SelectMany(s => s.GetFacets())
                    .Distinct();
            f2 =
                cache2.SelectMany(s => s.ObjectActions)
                    .Select(s => s.Spec)
                    .Where(s => s != null)
                    .SelectMany(s => s.GetFacets())
                    .Distinct();

            foreach (IFacet f in f2) {
                if (f1.Any(s => s.GetType() == f.GetType())) {
                    //Console.WriteLine("Found {0}", f.GetType().FullName);
                }
                else {
                    Console.WriteLine("Not Found action facet  {0}", f.GetType().FullName);
                }
            }

            f1 =
              cache1.SelectMany(s => s.ObjectActions)
                  .Select(s => s.Spec)
                  .Where(s => s != null)
                  .SelectMany(s => s.Parameters)
                  .SelectMany(s => s.GetFacets())
                  .Distinct();
            f2 =
                cache2.SelectMany(s => s.ObjectActions)
                    .Select(s => s.Spec)
                    .Where(s => s != null)
                     .SelectMany(s => s.Parameters)
                    .SelectMany(s => s.GetFacets())
                    .Distinct();

            foreach (IFacet f in f2) {
                if (f1.Any(s => s.GetType() == f.GetType())) {
                    //Console.WriteLine("Found {0}", f.GetType().FullName);
                }
                else {
                    Console.WriteLine("Not Found parm facet  {0}", f.GetType().FullName);
                }
            }

            f1 =
              cache1.SelectMany(s => s.Fields)
                  .Select(s => s.Spec)
                  .Where(s => s != null)
                  .SelectMany(s => s.GetFacets())
                  .Distinct();
            f2 =
                cache2.SelectMany(s => s.Fields)
                    .Select(s => s.Spec)
                    .Where(s => s != null)
                    .SelectMany(s => s.GetFacets())
                    .Distinct();

            foreach (IFacet f in f2) {
                if (f1.Any(s => s.GetType() == f.GetType())) {
                    //Console.WriteLine("Found {0}", f.GetType().FullName);
                }
                else {
                    Console.WriteLine("Not Found field facet  {0}", f.GetType().FullName);
                }
            }
        }


        private static void CompareCaches(ISpecificationCache cache, ISpecificationCache newCache) {
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

        public class NullMenuDfinition : IMainMenuDefinition {
            public IMenu[] MainMenus(IMenuFactory factory) {
                return new IMenu[] {};
            }
        }
    }
}