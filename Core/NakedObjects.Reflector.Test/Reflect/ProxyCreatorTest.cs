// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Data.Entity;
using Microsoft.Practices.Unity;
using NakedObjects.Persistor.Entity.Configuration;
using NakedObjects.Services;
using NakedObjects.Xat;
using NUnit.Framework;

namespace NakedObjects.Reflect.Test {
     public class DatabaseInitializer : DropCreateDatabaseAlways<ProxyTestContext> {}

    public class ProxyTestContext : DbContext {
        public ProxyTestContext()
            : base("ProxyCreatorTest") {}

        public DbSet<HasProperty> HasProperties { get; set; }
        public DbSet<HasCollectionWithVirtualAccessors> HasCollectionWithVirtualAccessors { get; set; }
        public DbSet<HasCollectionWithNonVirtualAccessors> HasCollectionWithNonVirtualAccessors { get; set; }
    }


    public class HasProperty {
        public virtual int Id { get; set; }

        public virtual string Prop { get; set; }
    }

    public class HasCollectionWithVirtualAccessors {
        public virtual int Id { get; set; }

        private ICollection<HasProperty> aCollection = new List<HasProperty>();

        public virtual ICollection<HasProperty> ACollection {
            get { return aCollection; }
            set { aCollection = value; }
        }

        public virtual void AddToACollection(HasProperty val) {
            aCollection.Add(val);
        }

        public virtual void RemoveFromACollection(HasProperty val) {
            aCollection.Remove(val);
        }

        public virtual void ClearACollection() {
            aCollection.Clear();
        }
    }

    public class HasCollectionWithNonVirtualAccessors {
        public virtual int Id { get; set; }

        private ICollection<HasProperty> aCollection = new List<HasProperty>();

        public virtual ICollection<HasProperty> ACollection {
            get { return aCollection; }
            set { aCollection = value; }
        }

        public void AddToACollection(HasProperty val) {
            aCollection.Add(val);
        }

        public void RemoveFromACollection(HasProperty val) {
            aCollection.Remove(val);
        }

        public void ClearACollection() {
            aCollection.Clear();
        }
    }


    [TestFixture] 
    public class ProxyCreatorTest : AcceptanceTestCase {
        #region Setup/Teardown

        [SetUp]
        public void Setup() {
            InitializeNakedObjectsFramework(this);
            StartTest();
        }

        [TearDown]
        public void TearDown() {}

        #endregion

        protected override void RegisterTypes(IUnityContainer container) {
            base.RegisterTypes(container);
            var config = new EntityObjectStoreConfiguration {EnforceProxies = false};
            config.UsingCodeFirstContext(Activator.CreateInstance<ProxyTestContext>);
            container.RegisterInstance<IEntityObjectStoreConfiguration>(config, (new ContainerControlledLifetimeManager()));
        }

        [TestFixtureSetUp]
        public void SetupFixture() {
            Database.SetInitializer(new DatabaseInitializer());
            InitializeNakedObjectsFramework(this);
        }

        [TestFixtureTearDown]
        public void TearDownFixture() {
            CleanupNakedObjectsFramework(this);
        }

        protected override Type[] Types {
            get {
                return new Type[] {
                    typeof (HasProperty), typeof (HasCollectionWithNonVirtualAccessors), typeof (HasCollectionWithVirtualAccessors)
                };
            }
        }

        protected override object[] MenuServices {
            get { return new[] {new SimpleRepository<HasProperty>()}; }
        }

        [Test]
        public void CreateProxyWithNonVirtualAccessors() {
            Type proxyType = ProxyCreator.CreateProxyType(NakedObjectsFramework.Metamodel, NakedObjectsFramework.LifecycleManager, typeof (HasCollectionWithNonVirtualAccessors));
            var proxy = (HasCollectionWithNonVirtualAccessors) Activator.CreateInstance(proxyType);
            NakedObjectsFramework.Injector.InitDomainObject(proxy);

            var hasProperty = new HasProperty();

            proxy.AddToACollection(hasProperty);

            Assert.IsTrue(proxy.ACollection.Contains(hasProperty));
        }

        [Test]
        public void CreateProxyWithVirtualAddTo() {
            Type proxyType = ProxyCreator.CreateProxyType(NakedObjectsFramework.Metamodel, NakedObjectsFramework.LifecycleManager, typeof (HasCollectionWithVirtualAccessors));
            var proxy = (HasCollectionWithVirtualAccessors) Activator.CreateInstance(proxyType);
            NakedObjectsFramework.Injector.InitDomainObject(proxy);

            var hasProperty = new HasProperty();

            proxy.AddToACollection(hasProperty);

            Assert.IsTrue(proxy.ACollection.Contains(hasProperty));
        }

        [Test]
        public void CreateProxyWithVirtualClear() {
            Type proxyType = ProxyCreator.CreateProxyType(NakedObjectsFramework.Metamodel, NakedObjectsFramework.LifecycleManager, typeof (HasCollectionWithVirtualAccessors));
            var proxy = (HasCollectionWithVirtualAccessors) Activator.CreateInstance(proxyType);
            NakedObjectsFramework.Injector.InitDomainObject(proxy);
            var hasProperty = new HasProperty();

            proxy.ACollection.Add(hasProperty);

            Assert.IsTrue(proxy.ACollection.Contains(hasProperty));

            proxy.ClearACollection();

            Assert.IsFalse(proxy.ACollection.Contains(hasProperty));
        }


        [Test]
        public void CreateProxyWithVirtualProperty() {
            Type proxyType = ProxyCreator.CreateProxyType(NakedObjectsFramework.Metamodel, NakedObjectsFramework.LifecycleManager, typeof (HasProperty));
            var proxy = (HasProperty) Activator.CreateInstance(proxyType);
            NakedObjectsFramework.Injector.InitDomainObject(proxy);
            const string testValue = "A Test Value";

            proxy.Prop = testValue;

            Assert.AreEqual(testValue, proxy.Prop);
        }

        [Test]
        public void CreateProxyWithVirtualRemoveFrom() {
            Type proxyType = ProxyCreator.CreateProxyType(NakedObjectsFramework.Metamodel, NakedObjectsFramework.LifecycleManager, typeof (HasCollectionWithVirtualAccessors));
            var proxy = (HasCollectionWithVirtualAccessors) Activator.CreateInstance(proxyType);
            NakedObjectsFramework.Injector.InitDomainObject(proxy);
            var hasProperty = new HasProperty();

            proxy.ACollection.Add(hasProperty);

            Assert.IsTrue(proxy.ACollection.Contains(hasProperty));

            proxy.RemoveFromACollection(hasProperty);

            Assert.IsFalse(proxy.ACollection.Contains(hasProperty));
        }
    }
}