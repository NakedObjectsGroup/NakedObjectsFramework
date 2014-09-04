// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Collections.Specialized;
using System.Web.Mvc;
using Expenses.Fixtures;
using Expenses.RecordedActions;
using Expenses.Services;
using MvcTestApp.Tests.Util;
using NakedObjects.Boot;
using NakedObjects.Core.Context;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Persistor.Objectstore.Inmemory;
using NakedObjects.Web.Mvc.Helpers;
using NakedObjects.Web.Mvc.Html;
using NakedObjects.Xat;
using NUnit.Framework;
using System.Linq;

namespace MvcTestApp.Tests.Helpers {
    [TestFixture]
    public class EncryptionTest : AcceptanceTestCase {
        #region Setup/Teardown

        [TestFixtureSetUp]
        public void SetupTest() {
            InitializeNakedObjectsFramework();

        }

        [TestFixtureTearDown]
        public void TearDownTest() {
            CleanupNakedObjectsFramework();
        }

        [SetUp]
        public new void StartTest() {
            SetUser("sven");
            Fixtures.InstallFixtures(NakedObjectsFramework.ObjectPersistor, null);
        }

        [TearDown]
        public void EndTest() {
            MemoryObjectStore.DiscardObjects();
        }

        #endregion

        protected override IServicesInstaller MenuServices {
            get { return new ServicesInstaller(DemoServicesSet.ServicesSet()); }
        }

        protected override IServicesInstaller ContributedActions {
            get { return new ServicesInstaller(new object[] { new RecordedActionContributedActions() }); }
        }

        protected override IFixturesInstaller Fixtures {
            get { return new FixturesInstaller(DemoFixtureSet.FixtureSet()); }
        }


        private class DummyController : Controller {}

        private readonly Controller controller = new DummyController();


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
            var mocks = new ContextMocks(controller);
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
        public void Encrypted() {
            var mocks = new ContextMocks(controller);
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

        [Test]
        public void Decrypt() {
            IEncryptDecrypt encrypter = new SimpleEncryptDecrypt();
            var mocks = new ContextMocks(controller);
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
    }
}