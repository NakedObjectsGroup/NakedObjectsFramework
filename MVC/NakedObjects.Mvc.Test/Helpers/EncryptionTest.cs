// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Specialized;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Expenses.Fixtures;
using Expenses.RecordedActions;
using Expenses.Services;
using Microsoft.Practices.Unity;
using MvcTestApp.Tests.Util;
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.EntityObjectStore;
using NakedObjects.Mvc.Test.Data;
using NakedObjects.Web.Mvc.Helpers;
using NakedObjects.Web.Mvc.Html;
using NakedObjects.Xat;
using NUnit.Framework;

namespace MvcTestApp.Tests.Helpers {
    [TestFixture]
    public class EncryptionTest : AcceptanceTestCase {
        #region Setup/Teardown

        [SetUp]
        public void SetupTest() {
            StartTest();
            controller = new DummyController();
            mocks = new ContextMocks(controller);
            SetUser("sven");
        }

        #endregion

        protected override void RegisterTypes(IUnityContainer container) {
            base.RegisterTypes(container);
            var config = new EntityObjectStoreConfiguration {EnforceProxies = false};
            config.UsingCodeFirstContext(() => new MvcTestContext("EncryptionTest"));
            container.RegisterInstance(config, (new ContainerControlledLifetimeManager()));
        }

        [TestFixtureSetUp]
        public void SetupTestFixture() {
            Database.SetInitializer(new DatabaseInitializer());
            InitializeNakedObjectsFramework(this);
        }

        [TestFixtureTearDown]
        public void TearDownTest() {
            CleanupNakedObjectsFramework(this);
            Database.Delete("EncryptionTest");
        }

        private DummyController controller;
        private ContextMocks mocks;

        protected override IServicesInstaller MenuServices {
            get { return new ServicesInstaller(DemoServicesSet.ServicesSet()); }
        }

        protected override IServicesInstaller ContributedActions {
            get { return new ServicesInstaller(new object[] {new RecordedActionContributedActions()}); }
        }

        protected override IFixturesInstaller Fixtures {
            get { return new FixturesInstaller(DemoFixtureSet.FixtureSet()); }
        }


        private class DummyController : Controller {}


        private CustomHelperTestClass TestClass {
            get { return (CustomHelperTestClass) GetTestService("Custom Helper Test Classes").GetAction("New Instance").InvokeReturnObject().NakedObject.Object; }
        }

        private DescribedCustomHelperTestClass DescribedTestClass {
            get { return (DescribedCustomHelperTestClass) GetTestService("Described Custom Helper Test Classes").GetAction("New Instance").InvokeReturnObject().NakedObject.Object; }
        }


        private static byte[] GetConstantKey(int size) {
            var ba = new byte[size];

            for (int i = 0; i < size; i++) {
                ba[i] = 0;
            }
            return ba;
        }


        [Test]
        public void CustomEncrypted() {
            CustomHelperTestClass tc = TestClass;
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofEncryptDecrypt] = new SimpleEncryptDecrypt();

            // keys to make test reproduceable 
            byte[] key = GetConstantKey(32);
            byte[] iv = GetConstantKey(16);
            var data = new Tuple<byte[], byte[]>(key, iv);

            mocks.HttpContext.Object.Session.Add(SimpleEncryptDecrypt.EncryptFieldData, data);

            string result = mocks.HtmlHelper.CustomEncrypted("name", "value");

            Assert.AreEqual(@"<input id=""name"" name=""-encryptedField-name"" type=""hidden"" value=""+xG+YO3ZY8KuTB6z4pUXjQ=="" />", result);
        }

        [Test]
        public void Decrypt() {
            IEncryptDecrypt encrypter = new SimpleEncryptDecrypt();

            // keys to make test reproduceable 
            byte[] key = GetConstantKey(32);
            byte[] iv = GetConstantKey(16);
            var data = new Tuple<byte[], byte[]>(key, iv);

            mocks.HttpContext.Object.Session.Add(SimpleEncryptDecrypt.EncryptFieldData, data);

            var randomName = Guid.NewGuid().ToString();
            var randomValue = Guid.NewGuid().ToString();
            var encryptValue = encrypter.Encrypt(mocks.HttpContext.Object.Session, randomName, randomValue);

            var collection = new NameValueCollection {{encryptValue.Item1, encryptValue.Item2}};

            Assert.IsFalse(collection.AllKeys.Contains(randomName));
            Assert.IsTrue(collection.AllKeys.Contains(encryptValue.Item1));
            Assert.AreEqual(encryptValue.Item2, collection[encryptValue.Item1]);

            encrypter.Decrypt(mocks.HttpContext.Object.Session, collection);

            Assert.IsTrue(collection.AllKeys.Contains(randomName));
            Assert.AreEqual(randomValue, collection[randomName]);
            Assert.IsTrue(collection.AllKeys.Contains(encryptValue.Item1));
            Assert.AreEqual(encryptValue.Item2, collection[encryptValue.Item1]);
        }

        [Test]
        public void Encrypted() {
            CustomHelperTestClass tc = TestClass;
            mocks.ViewDataContainer.Object.ViewData[IdHelper.NofEncryptDecrypt] = new SimpleEncryptDecrypt();

            // keys to make test reproduceable 
            byte[] key = GetConstantKey(32);
            byte[] iv = GetConstantKey(16);
            var data = new Tuple<byte[], byte[]>(key, iv);

            mocks.HttpContext.Object.Session.Add(SimpleEncryptDecrypt.EncryptFieldData, data);

            string result = mocks.HtmlHelper.Encrypted("name", "value").ToString();

            Assert.AreEqual(@"<input name=""-encryptedField-name"" type=""hidden"" value=""+xG+YO3ZY8KuTB6z4pUXjQ=="" />", result);
        }
    }
}