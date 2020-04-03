//// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
//// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
//// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
//// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
//// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//// See the License for the specific language governing permissions and limitations under the License.

//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.IO;
//using System.Linq;
//using System.Security.Principal;
//using AdventureWorksModel;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using NakedObjects.Architecture.Component;
//using NakedObjects.Architecture.Configuration;
//using NakedObjects.Architecture.Facet;
//using NakedObjects.Architecture.Menu;
//using NakedObjects.Architecture.SpecImmutable;
//using NakedObjects.Audit;
//using NakedObjects.Core.Configuration;
//using NakedObjects.Menu;
//using NakedObjects.Meta.Audit;
//using NakedObjects.Meta.Authorization;
//using NakedObjects.Meta.Component;
//using NakedObjects.Meta.I18N;
//using NakedObjects.Reflect;
//using NakedObjects.Reflect.Component;
//using NakedObjects.Security;
//using Microsoft.Practices.Unity;


//namespace NakedObjects.SystemTest.Reflect {
//    [TestClass]
//    public class ReflectorTest {
//        protected IUnityContainer GetContainer() {
//            var c = new UnityContainer();
//            RegisterTypes(c);
//            return c;
//        }

//        protected static void RegisterFacetFactories(IUnityContainer container) {
//            var factoryTypes = FacetFactories.StandardFacetFactories();
//            for (int i = 0; i < factoryTypes.Length; i++) {
//                RegisterFacetFactory(factoryTypes[i], container, i);
//            }
//        }

//        private static int RegisterFacetFactory(Type factory, IUnityContainer container, int order) {
//            container.RegisterType(typeof (IFacetFactory), factory, factory.Name, new ContainerControlledLifetimeManager(), new InjectionConstructor(order));
//            return order;
//        }

//        protected virtual void RegisterTypes(IUnityContainer container) {
//            ReflectorConfiguration.NoValidate = true;
//            RegisterFacetFactories(container);

//            container.RegisterType<IMenuFactory, NullMenuFactory>();
//            container.RegisterType<ISpecificationCache, ImmutableInMemorySpecCache>(
//                new ContainerControlledLifetimeManager(), new InjectionConstructor());
//            container.RegisterType<IClassStrategy, DefaultClassStrategy>();
//            container.RegisterType<IReflector, Reflector>();
//            container.RegisterType<IMetamodel, Metamodel>();
//            container.RegisterType<IMetamodelBuilder, Metamodel>();
//        }

//        [TestMethod]
//        public void ReflectNoTypes() {
//            IUnityContainer container = GetContainer();
//            var rc = new ReflectorConfiguration(new Type[] {}, new Type[] {}, new string[] {});

//            rc.SupportedSystemTypes.Clear();

//            container.RegisterInstance<IReflectorConfiguration>(rc);

//            var reflector = container.Resolve<IReflector>();
//            reflector.Reflect();
//            Assert.IsFalse(reflector.AllObjectSpecImmutables.Any());
//        }

//        [TestMethod]
//        public void ReflectWithDecorators() {
//            IUnityContainer container = GetContainer();
//            var rc = new ReflectorConfiguration(new Type[] {}, new Type[] {}, new string[] {});

//            rc.SupportedSystemTypes.Clear();

//            container.RegisterInstance<IReflectorConfiguration>(rc);

//            container.RegisterInstance<IAuditConfiguration>(new AuditConfiguration<TestAuditor>());
//            container.RegisterInstance<IAuthorizationConfiguration>(new AuthorizationConfiguration<TestAuthorizer>());
//            container.RegisterType<IFacetDecorator, AuditManager>("AuditManager");
//            container.RegisterType<IFacetDecorator, AuthorizationManager>("AuthorizationManager");
//            container.RegisterType<IFacetDecorator, I18NManager>("I18NManager");

//            var reflector = container.Resolve<IReflector>();
//            reflector.Reflect();

//            // todo add test code to validate this
//            //dynamic set = ((Reflector) reflector).FacetDecoratorSet;
//            //Assert.AreEqual(7, set.FacetDecorators.Count());
//        }

//        private static Type[] AdventureWorksTypes() {
//            Type[] allTypes =
//                AppDomain.CurrentDomain.GetAssemblies()
//                    .Single(a => a.GetName().Name == "AdventureWorksModel")
//                    .GetTypes();
//            return allTypes.Where(t => t.IsPublic && !t.IsAbstract && t.FullName.StartsWith("AdventureWorksModel")).ToArray();
//        }

//        [TestMethod]
//        public void ReflectAdventureworks() {
//            // load adventurework

//            AssemblyHook.EnsureAssemblyLoaded();

//            Type[] types = AdventureWorksTypes();
//            IUnityContainer container = GetContainer();
//            var rc = new ReflectorConfiguration(types, new Type[] {}, types.Select(t => t.Namespace).Distinct().ToArray());

//            container.RegisterInstance<IReflectorConfiguration>(rc);

//            var reflector = container.Resolve<IReflector>();
//            var stopwatch = new Stopwatch();
//            stopwatch.Start();

//            reflector.Reflect();
//            stopwatch.Stop();
//            TimeSpan interval = stopwatch.Elapsed;

//            Assert.IsTrue(reflector.AllObjectSpecImmutables.Any());
//            Assert.IsTrue(interval.Milliseconds < 1000);
//        }

//        // Comment this block out - serialization a no go at the moment.
//        // However useful example code if we need to revisit this type of thing later.
//        // this still fails - looks to be one of the facets on the OneToOneSpecImmutable causing a BinaryHeader error
//        // need further investigation
//        // how about wring a test that serialises/deserialises all facets ?
//        //[TestMethod]
//        //public void SerializeAdventureworks() {
//        //    // load adventurework

//        //    AssemblyHook.EnsureAssemblyLoaded();

//        //    Type[] types = AdventureWorksTypes();
//        //    IUnityContainer container = GetContainer();
//        //    var rc = new ReflectorConfiguration(types, new Type[] {}, types.Select(t => t.Namespace).Distinct().ToArray());

//        //    container.RegisterInstance<IReflectorConfiguration>(rc);

//        //    var reflector = container.Resolve<IReflector>();

//        //    var stopwatch = new Stopwatch();
//        //    stopwatch.Start();

//        //    reflector.Reflect();
//        //    stopwatch.Stop();
//        //    TimeSpan reflectInterval = stopwatch.Elapsed;
//        //    Console.WriteLine("reflect {0}", reflectInterval);

//        //    Assert.IsTrue(reflector.AllObjectSpecImmutables.Any());

//        //    var cache = container.Resolve<ISpecificationCache>();

//        //    //var f1 =
//        //    //    cache.AllSpecifications().SelectMany(s => s.Fields)
//        //    //        .Select(s => s.Spec)
//        //    //        .Where(s => s != null)
//        //    //        .OfType<OneToOneAssociationSpecImmutable>()
//        //    //        .SelectMany(s => s.GetFacets())
//        //    //        .Select(f => f.GetType().FullName)
//        //    //        .Distinct();

//        //    //foreach (var f in f1) {
//        //    //    Console.WriteLine(" field facet  {0}", f);
//        //    //}

//        //    Directory.CreateDirectory(@"c:\testmetadata");

//        //    const string file = @"c:\testmetadata\metadataAW.bin";

//        //    SerializeDeserialize(cache, file);
//        //}

//        //private void SerializeDeserialize(ISpecificationCache cache, string file) {
//        //    var stopwatch = new Stopwatch();
//        //    IUnityContainer container = GetContainer();

//        //    stopwatch.Start();

//        //    cache.Serialize(file);

//        //    stopwatch.Stop();
//        //    TimeSpan serializeInterval = stopwatch.Elapsed;
//        //    stopwatch.Reset();

//        //    // and roundtrip 

//        //    container.RegisterType<ISpecificationCache, ImmutableInMemorySpecCache>(
//        //        new PerResolveLifetimeManager(), new InjectionConstructor(file));

//        //    stopwatch.Start();
//        //    var newCache = container.Resolve<ISpecificationCache>();
//        //    stopwatch.Stop();
//        //    TimeSpan deserializeInterval = stopwatch.Elapsed;
//        //    stopwatch.Reset();

//        //    CompareCaches(cache, newCache);

//        //    Console.WriteLine("serialize {0} deserialize {1} ", serializeInterval,
//        //        deserializeInterval);
//        //}

//        //[TestMethod]
//        //public void SerializeAdventureworksByType() {
//        //    // load adventurework

//        //    AssemblyHook.EnsureAssemblyLoaded();

//        //    int count = AdventureWorksTypes().Count();

//        //    Type[] spec51 = AdventureWorksTypes().Skip(50).Take(1).ToArray();
//        //    Type[] types = AdventureWorksTypes().Take(20).ToArray();
//        //    IUnityContainer container = GetContainer();
//        //    var rc = new ReflectorConfiguration(types, new Type[] {}, types.Select(t => t.Namespace).Distinct().ToArray());

//        //    container.RegisterInstance<IReflectorConfiguration>(rc);

//        //    var reflector = container.Resolve<IReflector>();

//        //    var stopwatch = new Stopwatch();
//        //    stopwatch.Start();

//        //    reflector.Reflect();
//        //    stopwatch.Stop();
//        //    TimeSpan reflectInterval = stopwatch.Elapsed;
//        //    stopwatch.Reset();

//        //    Assert.IsTrue(reflector.AllObjectSpecImmutables.Any());

//        //    var cache = container.Resolve<ISpecificationCache>();

//        //    Directory.CreateDirectory(@"c:\testmetadata");

//        //    const string file = @"c:\testmetadata\metadataAWT.bin";

//        //    SerializeDeserialize(cache, file);
//        //}

//        [TestMethod]
//        public void SerializeAdventureworksFacets() {
//            // load adventurework

//            AssemblyHook.EnsureAssemblyLoaded();

//            int count = AdventureWorksTypes().Count();

//            Type[] types50 = AdventureWorksTypes().Take(50).ToArray();
//            Type[] types51 = AdventureWorksTypes().Take(51).ToArray();
//            IUnityContainer container = GetContainer();
//            var rc = new ReflectorConfiguration(types50, new Type[] {}, types50.Select(t => t.Namespace).Distinct().ToArray());

//            container.RegisterInstance<IReflectorConfiguration>(rc);

//            var reflector = container.Resolve<IReflector>();

//            reflector.Reflect();

//            ITypeSpecImmutable[] cache1 = container.Resolve<ISpecificationCache>().AllSpecifications();

//            rc = new ReflectorConfiguration(types51, new Type[] {}, types51.Select(t => t.Namespace).Distinct().ToArray());

//            container.RegisterInstance<IReflectorConfiguration>(rc);

//            reflector = container.Resolve<IReflector>();

//            reflector.Reflect();

//            ITypeSpecImmutable[] cache2 = container.Resolve<ISpecificationCache>().AllSpecifications();

//            int c1 = cache1.Count();
//            int c2 = cache2.Count();

//            foreach (IObjectSpecImmutable objectSpecImmutable in cache2) {
//                if (cache1.Any(s => s.FullName == objectSpecImmutable.FullName)) {
//                    //Console.WriteLine("Found {0}", objectSpecImmutable.FullName);
//                }
//                else {
//                    Console.WriteLine("Not Found object spec {0}", objectSpecImmutable.FullName);
//                }
//            }

//            IEnumerable<IFacet> f1 = cache1.SelectMany(s => s.GetFacets()).Distinct();
//            IEnumerable<IFacet> f2 = cache2.SelectMany(s => s.GetFacets()).Distinct();

//            foreach (IFacet f in f2) {
//                if (f1.Any(s => s.GetType() == f.GetType())) {
//                    //Console.WriteLine("Found {0}", f.GetType().FullName);
//                }
//                else {
//                    Console.WriteLine("Not Found object facet {0}", f.GetType().FullName);
//                }
//            }

//            f1 =
//                cache1.SelectMany(s => s.ObjectActions)
//                    .Where(s => s != null)
//                    .SelectMany(s => s.GetFacets())
//                    .Distinct();
//            f2 =
//                cache2.SelectMany(s => s.ObjectActions)
//                    .Where(s => s != null)
//                    .SelectMany(s => s.GetFacets())
//                    .Distinct();

//            foreach (IFacet f in f2) {
//                if (f1.Any(s => s.GetType() == f.GetType())) {
//                    //Console.WriteLine("Found {0}", f.GetType().FullName);
//                }
//                else {
//                    Console.WriteLine("Not Found action facet  {0}", f.GetType().FullName);
//                }
//            }

//            f1 =
//                cache1.SelectMany(s => s.ObjectActions)
//                    .Where(s => s != null)
//                    .SelectMany(s => s.Parameters)
//                    .SelectMany(s => s.GetFacets())
//                    .Distinct();
//            f2 =
//                cache2.SelectMany(s => s.ObjectActions)
//                    .Where(s => s != null)
//                    .SelectMany(s => s.Parameters)
//                    .SelectMany(s => s.GetFacets())
//                    .Distinct();

//            foreach (IFacet f in f2) {
//                if (f1.Any(s => s.GetType() == f.GetType())) {
//                    //Console.WriteLine("Found {0}", f.GetType().FullName);
//                }
//                else {
//                    Console.WriteLine("Not Found parm facet  {0}", f.GetType().FullName);
//                }
//            }

//            f1 =
//                cache1.SelectMany(s => s.Fields)
//                    .Where(s => s != null)
//                    .SelectMany(s => s.GetFacets())
//                    .Distinct();
//            f2 =
//                cache2.SelectMany(s => s.Fields)
//                    .Where(s => s != null)
//                    .SelectMany(s => s.GetFacets())
//                    .Distinct();

//            foreach (IFacet f in f2) {
//                if (f1.Any(s => s.GetType() == f.GetType())) {
//                    //Console.WriteLine("Found {0}", f.GetType().FullName);
//                }
//                else {
//                    Console.WriteLine("Not Found field facet  {0}", f.GetType().FullName);
//                }
//            }
//        }

//        private static void CompareCaches(ISpecificationCache cache, ISpecificationCache newCache) {
//            Assert.AreEqual(cache.AllSpecifications().Count(), newCache.AllSpecifications().Count());

//            var zipped = cache.AllSpecifications().Zip(newCache.AllSpecifications(), (a, b) => new {a, b});

//            foreach (var item in zipped) {
//                Assert.AreEqual(item.a.FullName, item.b.FullName);

//                Assert.AreEqual(item.a.GetFacets().Count(), item.b.GetFacets().Count());

//                var zipfacets = item.a.GetFacets().Zip(item.b.GetFacets(), (x, y) => new {x, y});

//                foreach (var zipfacet in zipfacets) {
//                    Assert.AreEqual(zipfacet.x.FacetType, zipfacet.y.FacetType);
//                }
//            }
//        }

//        #region Nested type: NullMenuFactory

//        public class NullMenuFactory : IMenuFactory {
//            #region IMenuFactory Members

//            public IMenu NewMenu<T>(bool addAllActions, string name = null) {
//                throw new NotImplementedException();
//            }

//            public IMenu NewMenu(Type type, bool addAllActions = false, string name = null) {
//                throw new NotImplementedException();
//            }

//            #endregion

//            public IMenu NewMenu(string name) {
//                throw new NotImplementedException();
//            }
//        }

//        #endregion

//        #region Nested type: TestAuditor

//        public class TestAuditor : IAuditor {
//            #region IAuditor Members

//            public void ActionInvoked(IPrincipal byPrincipal, string actionName, object onObject, bool queryOnly, object[] withParameters) {}

//            public void ActionInvoked(IPrincipal byPrincipal, string actionName, string serviceName, bool queryOnly, object[] withParameters) {}

//            public void ObjectUpdated(IPrincipal byPrincipal, object updatedObject) {}

//            public void ObjectPersisted(IPrincipal byPrincipal, object updatedObject) {}

//            #endregion
//        }

//        #endregion

//        #region Nested type: TestAuthorizer

//        public class TestAuthorizer : ITypeAuthorizer<object> {
//            #region ITypeAuthorizer<object> Members

//            public bool IsEditable(IPrincipal principal, object target, string memberName) {
//                throw new NotImplementedException();
//            }

//            public bool IsVisible(IPrincipal principal, object target, string memberName) {
//                throw new NotImplementedException();
//            }

//            #endregion

//            public void Init() {
//                throw new NotImplementedException();
//            }

//            public void Shutdown() {
//                throw new NotImplementedException();
//            }
//        }

//        #endregion
//    }
//}