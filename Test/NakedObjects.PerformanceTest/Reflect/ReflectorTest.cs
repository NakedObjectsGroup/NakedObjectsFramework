// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Web.UI;
using System.Xml.Serialization;
using AdventureWorksModel;
using AdventureWorksModel.Sales;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Configuration;
using NakedObjects.Architecture.Menu;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core.Configuration;
using NakedObjects.Menu;
using NakedObjects.Meta;
using NakedObjects.Meta.Component;
using NakedObjects.Meta.SpecImmutable;
using NakedObjects.Meta.Utils;
using NakedObjects.Reflect;
using NakedObjects.Reflect.Component;
using NakedObjects.Unity;
using NakedObjects.Value;
using NakedObjectsTest.SystemTest.Reflect;
using Newtonsoft.Json;
using Sdm.App.App_Start;
using Sdm.Test.Fixtures.Clusters.Means.MeansAssessment.Actions;

namespace NakedObjects.SystemTest.Reflect {



    [TestClass]
    public class ReflectorTest {
        protected IUnityContainer GetContainer() {
            ImmutableSpecFactory.ClearCache();
            var c = new UnityContainer();
            RegisterTypes(c);
            return c;
        }

        protected IUnityContainer GetParallelContainer() {
            ImmutableSpecFactory.ClearCache();
            var c = new UnityContainer();
            RegisterParallelTypes(c);
            return c;
        }

        protected static void RegisterFacetFactories(IUnityContainer container) {
            var factoryTypes = FacetFactories.StandardFacetFactories();
            for (int i = 0; i < factoryTypes.Length; i++) {
                RegisterFacetFactory(factoryTypes[i], container, i);
            }
        }

        private static int RegisterFacetFactory(Type factory, IUnityContainer container, int order) {
            container.RegisterType(typeof (IFacetFactory), factory, factory.Name, new ContainerControlledLifetimeManager(), new InjectionConstructor(order));
            return order;
        }

        protected virtual void RegisterTypes(IUnityContainer container) {
            //ReflectorConfiguration.NoValidate = true;
            //RegisterFacetFactories(container);

            //container.RegisterType<IMenuFactory, NullMenuFactory>();
            //container.RegisterType<ISpecificationCache, ImmutableInMemorySpecCache>(
            //    new ContainerControlledLifetimeManager(), new InjectionConstructor());
            //container.RegisterType<IClassStrategy, DefaultClassStrategy>();
            //container.RegisterType<IReflector, Reflector>();
            //container.RegisterType<IMetamodel, Metamodel>();
            //container.RegisterType<IMetamodelBuilder, Metamodel>();

            StandardUnityConfig.RegisterStandardFacetFactories(container);
            StandardUnityConfig.RegisterCoreContainerControlledTypes(container);
            StandardUnityConfig.RegisterCorePerTransactionTypes<HierarchicalLifetimeManager>(container);
        }

        protected virtual void RegisterParallelTypes(IUnityContainer container) {
            //ReflectorConfiguration.NoValidate = true;
            //RegisterFacetFactories(container);

            //container.RegisterType<IMenuFactory, NullMenuFactory>();
            //container.RegisterType<ISpecificationCache, ImmutableInMemorySpecCache>(
            //    new ContainerControlledLifetimeManager(), new InjectionConstructor());
            //container.RegisterType<IClassStrategy, DefaultClassStrategy>();
            //container.RegisterType<IReflector, Reflector>();
            //container.RegisterType<IMetamodel, Metamodel>();
            //container.RegisterType<IMetamodelBuilder, Metamodel>();

            ParallelUnityConfig.RegisterStandardFacetFactories(container);
            ParallelUnityConfig.RegisterCoreContainerControlledTypes(container);
            ParallelUnityConfig.RegisterCorePerTransactionTypes<HierarchicalLifetimeManager>(container);
        }


        private static string[] ModelNamespaces
        {
            get
            {
                return new string[] { "AdventureWorksModel" };
            }
        }

        private static Type[] Types
        {
            get
            {
                return new[] {
                    typeof (EntityCollection<object>),
                    typeof (ObjectQuery<object>),
                    typeof (CustomerCollectionViewModel),
                    typeof (OrderLine),
                    typeof (OrderStatus),
                    typeof (QuickOrderForm),
                    typeof (ProductProductPhoto),
                    typeof (ProductModelProductDescriptionCulture)
                };
            }
        }

        private static Type[] Services
        {
            get
            {
                return new[] {
                    typeof (CustomerRepository),
                    typeof (OrderRepository),
                    typeof (ProductRepository),
                    typeof (EmployeeRepository),
                    typeof (SalesRepository),
                    typeof (SpecialOfferRepository),
                
                    typeof (VendorRepository),
                    typeof (PurchaseOrderRepository),
                    typeof (WorkOrderRepository),
                    typeof (OrderContributedActions),
                    typeof (CustomerContributedActions),
              
                };
            }
        }

        public static IMenu[] MainMenus(IMenuFactory factory) {
            var customerMenu = factory.NewMenu<CustomerRepository>(false);
            CustomerRepository.Menu(customerMenu);
            var salesMenu = factory.NewMenu<SalesRepository>(false);
         
            return new[] {
                customerMenu,
                factory.NewMenu<OrderRepository>(true),
                factory.NewMenu<ProductRepository>(true),
                factory.NewMenu<EmployeeRepository>(true),
                salesMenu,
                factory.NewMenu<SpecialOfferRepository>(true),
               
                factory.NewMenu<VendorRepository>(true),
                factory.NewMenu<PurchaseOrderRepository>(true),
                factory.NewMenu<WorkOrderRepository>(true),
               
            };
        }

        //private static string awFile = "E:\\Users\\scasc_000\\Documents\\GitHub\\NakedObjectsFramework\\Test\\NakedObjects.PerformanceTest\\Reflect\\awnames.txt";

        [TestMethod]
        public void ReflectAdventureworksOldTest() {
            ReflectAdventureworksOld();
        }

        public TimeSpan ReflectAdventureworksOld() {
            // load adventurework

            IUnityContainer container = GetContainer();
            var rc = new ReflectorConfiguration(Types, Services, ModelNamespaces, MainMenus);

            container.RegisterInstance<IReflectorConfiguration>(rc);

            var reflector = container.Resolve<IReflector>();
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            reflector.Reflect();
            stopwatch.Stop();
            TimeSpan interval = stopwatch.Elapsed;

            //Assert.AreEqual(135, reflector.AllObjectSpecImmutables.Length);
            Assert.IsTrue(reflector.AllObjectSpecImmutables.Any());
            //Assert.IsTrue(interval.TotalMilliseconds < 1000);
            Console.WriteLine(interval.TotalMilliseconds);
            // 971

            var cache = container.Resolve<ISpecificationCache>();
            serialAwSpecs = cache.AllSpecifications();

            //string[] names = reflector.AllObjectSpecImmutables.Select(i => i.FullName).ToArray();

            //File.AppendAllLines(awFile, names);
            return interval;
        }


        private ITypeSpecImmutable[] DeSerialize(string file, IFormatter formatter) {

            ITypeSpecImmutable[] data;
            using (FileStream fs = File.Open(file, FileMode.Open)) {
                data = (ITypeSpecImmutable[])formatter.Deserialize(fs);
            }

            return data;
        }

        private void Serialize(string file, IFormatter formatter, ITypeSpecImmutable[] specs) {
            using (FileStream fs = File.Open(file, FileMode.OpenOrCreate)) {
                formatter.Serialize(fs, specs);
            }
        }


        [TestMethod]
        public void SerializeAdventureworks() {
            // load adventurework

            AssemblyHook.EnsureAssemblyLoaded();

            IUnityContainer container = GetContainer();
            var rc = new ReflectorConfiguration(Types, Services, ModelNamespaces, MainMenus);

            container.RegisterInstance<IReflectorConfiguration>(rc);

            var reflector = container.Resolve<IReflector>();

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            reflector.Reflect();
            stopwatch.Stop();
            var reflectInterval = stopwatch.Elapsed.TotalMilliseconds;
            Console.WriteLine("reflect {0}", reflectInterval);

            Assert.IsTrue(reflector.AllObjectSpecImmutables.Any());

            //var cache = container.Resolve<ISpecificationCache>();

            //var f1 =
            //    cache.AllSpecifications().SelectMany(s => s.Fields)
            //        .Select(s => s.Spec)
            //        .Where(s => s != null)
            //        .OfType<OneToOneAssociationSpecImmutable>()
            //        .SelectMany(s => s.GetFacets())
            //        .Select(f => f.GetType().FullName)
            //        .Distinct();

            //foreach (var f in f1) {
            //    Console.WriteLine(" field facet  {0}", f);
            //}
            var cache = container.Resolve<ISpecificationCache>();
            serialAwSpecs = cache.AllSpecifications();

            //Directory.CreateDirectory(@"c:\testmetadata");

            //const string file = @"c:\testmetadata\metadataAW.bin";

            //SerializeDeserializeLocal(cache, file);
        }

        [TestMethod]
        public void SerializeDSP() {
            // load adventurework

            IUnityContainer container = GetContainer();
            var rc = DSPReflectorConfiguration(container);

            container.RegisterInstance<IReflectorConfiguration>(rc);

            var reflector = container.Resolve<IReflector>();

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            reflector.Reflect();
            stopwatch.Stop();
            var reflectInterval = stopwatch.Elapsed.TotalMilliseconds;
            Console.WriteLine("reflect {0}", reflectInterval);

            Assert.IsTrue(reflector.AllObjectSpecImmutables.Any());

            var cache = container.Resolve<ISpecificationCache>();


            Directory.CreateDirectory(@"c:\testmetadata");

            const string file = @"c:\testmetadata\metadataDSP.bin";

            SerializeDeserializeLocalEach(cache, file);
        }


        private void SerializeDeserialize(ISpecificationCache cache, string file) {
            var stopwatch = new Stopwatch();
            IUnityContainer container = GetContainer();

            stopwatch.Start();

            IFormatter formatter = new NetDataContractSerializer();
            //IFormatter formatter = new BinaryFormatter();
            //IFormatter formatter = new SoapFormatter();

            //cache.Serialize(file, formatter);

            stopwatch.Stop();
            TimeSpan serializeInterval = stopwatch.Elapsed;
            stopwatch.Reset();

            // and roundtrip 

            container.RegisterType<ISpecificationCache, ImmutableInMemorySpecCache>(
                new PerResolveLifetimeManager(), new InjectionConstructor(file, formatter));

            stopwatch.Start();
            var newCache = container.Resolve<ISpecificationCache>();
            stopwatch.Stop();
            TimeSpan deserializeInterval = stopwatch.Elapsed;
            stopwatch.Reset();

            CompareCaches(cache, newCache);

            Console.WriteLine("serialize {0} deserialize {1} ", serializeInterval.TotalMilliseconds,
                deserializeInterval.TotalMilliseconds);
        }

        private void SerializeDeserializeLocal(ISpecificationCache cache, string file) {
            var stopwatch = new Stopwatch();
            IUnityContainer container = GetContainer();

            stopwatch.Start();

            IFormatter formatter = new NetDataContractSerializer();
            //IFormatter formatter = new BinaryFormatter();
            //IFormatter formatter = new SoapFormatter();

            var specs = cache.AllSpecifications();

            Serialize(file, formatter, specs);

            stopwatch.Stop();
            TimeSpan serializeInterval = stopwatch.Elapsed;
            stopwatch.Reset();

            // and roundtrip 

            stopwatch.Start();
            var newSpecs = DeSerialize(file, formatter);
            stopwatch.Stop();
            TimeSpan deserializeInterval = stopwatch.Elapsed;
            stopwatch.Reset();

            Assert.AreEqual(specs.Length, newSpecs.Length);

            //CompareCaches(cache, newCache);

            Console.WriteLine("serialize {0} deserialize {1} ", serializeInterval.TotalMilliseconds,
                deserializeInterval.TotalMilliseconds);
        }

        private void SerializeDeserializeLocalEach(ISpecificationCache cache, string file) {
            var stopwatch = new Stopwatch();
            //IUnityContainer container = GetContainer();

            //stopwatch.Start();

            IFormatter formatter = new NetDataContractSerializer();
            //IFormatter formatter = new BinaryFormatter();
            //IFormatter formatter = new SoapFormatter();

            var specs = cache.AllSpecifications();
            var index = 0;

            foreach (var spec in specs) {

                try {
                    ITypeSpecImmutable[] singleSpec = {spec};

                    stopwatch.Start();

                    string fileName = file + index++;

                    Serialize(fileName, formatter, singleSpec);

                    stopwatch.Stop();
                    TimeSpan serializeInterval = stopwatch.Elapsed;
                    stopwatch.Reset();

                    // and roundtrip 

                    stopwatch.Start();
                    var newSpecs = DeSerialize(fileName, formatter);
                    stopwatch.Stop();
                    TimeSpan deserializeInterval = stopwatch.Elapsed;
                    stopwatch.Reset();

                    Assert.AreEqual(1, newSpecs.Length);

                    //CompareCaches(cache, newCache);

                    Console.WriteLine("serialize {0} deserialize {1} ", serializeInterval.TotalMilliseconds,
                        deserializeInterval.TotalMilliseconds);
                }
                catch (Exception e) {
                    Console.WriteLine("failed "+ index + " " + spec.FullName + " " + e.Message);
                }
            }
        }




        private static void CompareCaches(ISpecificationCache cache, ISpecificationCache newCache) {
            Assert.AreEqual(cache.AllSpecifications().Count(), newCache.AllSpecifications().Count());

            var zipped = cache.AllSpecifications().Zip(newCache.AllSpecifications(), (a, b) => new { a, b });

            foreach (var item in zipped) {
                Assert.AreEqual(item.a.FullName, item.b.FullName);

                Assert.AreEqual(item.a.GetFacets().Count(), item.b.GetFacets().Count());

                var zipfacets = item.a.GetFacets().Zip(item.b.GetFacets(), (x, y) => new { x, y });

                foreach (var zipfacet in zipfacets) {
                    Assert.AreEqual(zipfacet.x.FacetType, zipfacet.y.FacetType);
                }
            }
        }

        private ITypeSpecImmutable[] parallelAwSpecs;
        private ITypeSpecImmutable[] serialAwSpecs;
        private ITypeSpecImmutable[] parallelDspSpecs;
        private ITypeSpecImmutable[] serialDspSpecs;

        [TestMethod]
        public void ReflectAdventureworksParallelTest() {
            ReflectAdventureworksParallel();
        }

        public TimeSpan ReflectAdventureworksParallel() {
            // load adventurework

            IUnityContainer container = GetParallelContainer();
            var rc = new ReflectorConfiguration(Types, Services, ModelNamespaces, MainMenus);

            container.RegisterInstance<IReflectorConfiguration>(rc);

            var reflector = container.Resolve<IReflector>();
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            reflector.Reflect();
            stopwatch.Stop();
            TimeSpan interval = stopwatch.Elapsed;

            //Assert.AreEqual(135, reflector.AllObjectSpecImmutables.Length);
            Assert.IsTrue(reflector.AllObjectSpecImmutables.Any());
            //Assert.IsTrue(interval.TotalMilliseconds < 1000);
            Console.WriteLine(interval.TotalMilliseconds);
            // 971

            var cache = container.Resolve<ISpecificationCache>();
            parallelAwSpecs = cache.AllSpecifications();
            return interval;

            //var names = File.ReadAllLines(awFile);

            //var newNames = reflector.AllObjectSpecImmutables.Select(i => i.FullName).ToArray();

            //foreach (var name in names) {
            //    if (!newNames.Contains(name)) {
            //        Console.WriteLine("missing name in new metamodel: " + name);
            //    }
            //}

            //foreach (var name in newNames) {
            //    if (!names.Contains(name)) {
            //        Console.WriteLine("name not present in old metamodel: " + name);
            //    }
            //}
        }




        [TestMethod]
        public void CompareAWSpecs() {
            var oldTime = ReflectAdventureworksOld().TotalMilliseconds;
            var newTime = ReflectAdventureworksParallel().TotalMilliseconds;
            CompareFunctions.Compare(parallelAwSpecs, serialAwSpecs);

            WriteResult(newTime, oldTime, 32);
        }

        [TestMethod]
        public void CompareDSPSpecs() {
            var oldTime = ReflectDSPOld().TotalMilliseconds;
            var newTime = ReflectDSPParallel().TotalMilliseconds;
            CompareFunctions.Compare(parallelDspSpecs, serialDspSpecs);

            WriteResult(newTime, oldTime, 40);
        }

        private static void WriteResult(double newTime, double oldTime, int expect) {
            var percent = Math.Round((newTime / oldTime) * 100);

            Console.WriteLine("Parallel is " + percent + "%");
            Console.WriteLine("Expect ~" + expect + "%");
        }

        private static ReflectorConfiguration DSPReflectorConfiguration(IUnityContainer container) {
            var appSpec = new TweakedSdmAppMainSpecMvc(container);

            var types = (new List<Type> {
                typeof(sdm.systems.application.control.State),
                typeof(sdm.systems.application.control.State[])
            });

            types.AddRange(appSpec.TypesForApp);
            types = types.Distinct().ToList();


            return new ReflectorConfiguration(types.ToArray(), appSpec.AllServicesForApp, appSpec.NamespacesForApp, appSpec.MainMenusForApp);
        }

        //private static string dspFile = "E:\\Users\\scasc_000\\Documents\\GitHub\\NakedObjectsFramework\\Test\\NakedObjects.PerformanceTest\\Reflect\\dspnames.txt";

        [TestMethod]
        public void ReflectDSPOldTest() {
            var interval = ReflectDSPOld();
            Console.WriteLine(interval.TotalMilliseconds);
        }

        public TimeSpan ReflectDSPOld() {
         

            IUnityContainer container = GetContainer();
            var rc = DSPReflectorConfiguration(container);

            container.RegisterInstance<IReflectorConfiguration>(rc);

            var reflector = container.Resolve<IReflector>();
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            reflector.Reflect();
            stopwatch.Stop();
            TimeSpan interval = stopwatch.Elapsed;

            //Assert.AreEqual(7114, reflector.AllObjectSpecImmutables.Length);
            Assert.IsTrue(reflector.AllObjectSpecImmutables.Any());
            //Assert.IsTrue(interval.Milliseconds < 1000);
            Console.WriteLine(interval.TotalMilliseconds);
            // 223881
            // 228833 = 3m 48s

            var cache = container.Resolve<ISpecificationCache>();
            serialDspSpecs = cache.AllSpecifications();
            return interval;
        }

        [TestMethod]
        public void ReflectDSPParallelTest() {
            var interval = ReflectDSPParallel();
            Console.WriteLine("Expect 132894.8429");
            Console.WriteLine(interval.TotalMilliseconds);
        }

        public TimeSpan ReflectDSPParallel() {
            // load adventurework

            IUnityContainer container = GetParallelContainer();
            var rc = DSPReflectorConfiguration(container);

            container.RegisterInstance<IReflectorConfiguration>(rc);

            var reflector = container.Resolve<IReflector>();
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            reflector.Reflect();
            stopwatch.Stop();
            TimeSpan interval = stopwatch.Elapsed;

            //var names = File.ReadAllLines(dspFile);

            //var newNames = reflector.AllObjectSpecImmutables.Select(i => i.FullName).ToArray();

            //foreach (var name in names) {
            //    if (!newNames.Contains(name)) {
            //        Console.WriteLine("missing name in new metamodel: " + name);
            //    }
            //}

            //foreach (var name in newNames) {
            //    if (!names.Contains(name)) {
            //        Console.WriteLine("name not present in old metamodel: " + name);
            //    }
            //}

     
            //Assert.AreEqual(7114, reflector.AllObjectSpecImmutables.Length);
            Assert.IsTrue(reflector.AllObjectSpecImmutables.Any());
            //Assert.IsTrue(interval.Milliseconds < 1000);
            Console.WriteLine(interval.TotalMilliseconds);
            // 144840 = 2m 24s
            // 95249 = 1m 35s


            var cache = container.Resolve<ISpecificationCache>();
            parallelDspSpecs = cache.AllSpecifications();
            return interval;
        }

        [TestMethod]
        public void TestOrder() {
            var testType = typeof(sdm.common.bom.communications.impl.forms.RecordedFormCommunication);

            var methods = testType.GetMethods();

            foreach (var m in methods) {
                Console.WriteLine($"{m.Name}/{m.DeclaringType}");
            }
        }



        #region Nested type: NullMenuFactory

        public class NullMenuFactory : IMenuFactory {
            #region IMenuFactory Members

            public IMenu NewMenu<T>(bool addAllActions, string name = null) {
                throw new NotImplementedException();
            }

            public IMenu NewMenu(Type type, bool addAllActions = false, string name = null) {
                throw new NotImplementedException();
            }

            #endregion

            public IMenu NewMenu(string name) {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}