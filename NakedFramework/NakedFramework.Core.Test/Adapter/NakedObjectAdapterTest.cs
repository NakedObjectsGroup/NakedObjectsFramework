// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using Microsoft.Extensions.Logging;
using Moq;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Framework;
using NakedFramework.Core.Adapter;
using NakedFramework.Core.Error;
using NUnit.Framework;

namespace NakedFramework.Core.Test.Adapter {
    [TestFixture]
    public class NakedObjectAdapterTest {
        private readonly ILifecycleManager lifecycleManager = new Mock<ILifecycleManager>().Object;
        private readonly ILogger<NakedObjectAdapter> logger = new Mock<ILogger<NakedObjectAdapter>>().Object;
        private readonly ILoggerFactory loggerFactory = new Mock<ILoggerFactory>().Object;
        private readonly IMetamodelManager metamodel = new Mock<IMetamodelManager>().Object;
        private readonly INakedObjectManager nakedObjectManager = new Mock<INakedObjectManager>().Object;
        private readonly IOid oid = new Mock<IOid>().Object;
        private readonly IObjectPersistor persistor = new Mock<IObjectPersistor>().Object;
        private readonly object poco = new();
        private readonly ISession session = new Mock<ISession>().Object;
        private readonly ILogger<NullVersion> vLogger = new Mock<ILogger<NullVersion>>().Object;

        private readonly INakedFramework framework = new Mock<INakedFramework>().Object;

        [Test]
        public void TestCheckLockDefaultOk() {
            INakedObjectAdapter testAdapter = new NakedObjectAdapter(poco, oid, framework, loggerFactory, logger);
            var testNullVersion = new NullVersion(vLogger);
            testAdapter.CheckLock(testNullVersion);
        }

        [Test]
        public void TestCheckLockDefaultFail() {
            INakedObjectAdapter testAdapter = new NakedObjectAdapter(poco, oid, framework, loggerFactory, logger);
            var testCcVersionVersion = new ConcurrencyCheckVersion("", DateTime.Now, new object());

            try {
                testAdapter.CheckLock(testCcVersionVersion);
                Assert.Fail("exception expected");
            }
            catch (ConcurrencyException) {
                // expected 
            }
        }

        [Test]
        public void TestCheckLockNewVersionOk() {
            INakedObjectAdapter testAdapter = new NakedObjectAdapter(poco, oid, framework, loggerFactory, logger);
            var testCcVersion = new ConcurrencyCheckVersion("", DateTime.Now, new object());

            testAdapter.OptimisticLock = testCcVersion;

            testAdapter.CheckLock(testCcVersion);
        }

        [Test]
        public void TestCheckLockNewVersionFail() {
            INakedObjectAdapter testAdapter = new NakedObjectAdapter(poco, oid, framework, loggerFactory, logger);
            var testCcVersion = new ConcurrencyCheckVersion("", DateTime.Now, new object());

            testAdapter.OptimisticLock = testCcVersion;

            try {
                testAdapter.CheckLock(new NullVersion(vLogger));
                Assert.Fail("exception expected");
            }
            catch (ConcurrencyException) {
                // expected 
            }
        }

        [Test]
        public void TestRoundTrip() {
            var mockMetamodelManager = new Mock<IMetamodelManager>();
            var mockLoggerFactory = new Mock<ILoggerFactory>();

            var testCcVersion = new ConcurrencyCheckVersion("", DateTime.Now, "avalue");

            var encoded = testCcVersion.ToEncodedStrings();
            var newVersion = new ConcurrencyCheckVersion(mockMetamodelManager.Object, mockLoggerFactory.Object, encoded);

            Assert.AreEqual(testCcVersion, newVersion);
        }
    }
}