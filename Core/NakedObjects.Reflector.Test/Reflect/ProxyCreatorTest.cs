// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using Microsoft.Practices.Unity;
using NakedObjects.Services;
using NakedObjects.Xat;
using NUnit.Framework;

namespace NakedObjects.Reflect.Test {
    public class HasProperty {
        public virtual string Prop { get; set; }
    }

    public class HasCollectionWithVirtualAccessors {
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


    [TestFixture, Ignore] // fix by changing to codefirst
    public class ProxyCreatorTest : AcceptanceTestCase {
        #region Setup/Teardown

        [SetUp]
        public void Setup() {
            StartTest();
        }

        [TearDown]
        public void TearDown() {}

        #endregion

        protected override void RegisterTypes(IUnityContainer container) {
            base.RegisterTypes(container);
            // replace INakedObjectStore types

            //container.RegisterType<IOidGenerator, SimpleOidGenerator>(new InjectionConstructor(typeof (INakedObjectReflector), 0L));
            //container.RegisterType<IPersistAlgorithm, DefaultPersistAlgorithm>();
            //container.RegisterType<INakedObjectStore, MemoryObjectStore>();
            //container.RegisterType<IIdentityMap, IdentityMapImpl>();
        }

        [TestFixtureSetUp]
        public void SetupFixture() {
            InitializeNakedObjectsFramework(this);
        }

        [TestFixtureTearDown]
        public void TearDownFixture() {
            CleanupNakedObjectsFramework(this);
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