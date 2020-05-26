// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using Moq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Core.Adapter;
using NUnit.Framework;

namespace NakedObjects.Core.Test.Adapter {
    [TestFixture]
    public class NakedObjectAdapterTest {
        private readonly ILifecycleManager lifecycleManager = new Mock<ILifecycleManager>().Object;
        private readonly IMetamodelManager metamodel = new Mock<IMetamodelManager>().Object;
        private readonly INakedObjectManager nakedObjectManager = new Mock<INakedObjectManager>().Object;
        private readonly IOid oid = new Mock<IOid>().Object;
        private readonly IObjectPersistor persistor = new Mock<IObjectPersistor>().Object;
        private readonly object poco = new object();
        private readonly ISession session = new Mock<ISession>().Object;

        [Test]
        public void TestCheckLockDefaultOk() {
            INakedObjectAdapter testAdapter = new NakedObjectAdapter(metamodel, session, persistor, lifecycleManager, nakedObjectManager, poco, oid);
            var testNullVersion = new NullVersion();
            testAdapter.CheckLock(testNullVersion);
        }

        [Test]
        public void TestCheckLockDefaultFail() {
            INakedObjectAdapter testAdapter = new NakedObjectAdapter(metamodel, session, persistor, lifecycleManager, nakedObjectManager, poco, oid);
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
            INakedObjectAdapter testAdapter = new NakedObjectAdapter(metamodel, session, persistor, lifecycleManager, nakedObjectManager, poco, oid);
            var testCcVersion = new ConcurrencyCheckVersion("", DateTime.Now, new object());

            testAdapter.OptimisticLock = testCcVersion;

            testAdapter.CheckLock(testCcVersion);
        }

        [Test]
        public void TestCheckLockNewVersionFail() {
            INakedObjectAdapter testAdapter = new NakedObjectAdapter(metamodel, session, persistor, lifecycleManager, nakedObjectManager, poco, oid);
            var testCcVersion = new ConcurrencyCheckVersion("", DateTime.Now, new object());

            testAdapter.OptimisticLock = testCcVersion;

            try {
                testAdapter.CheckLock(new NullVersion());
                Assert.Fail("exception expected");
            }
            catch (ConcurrencyException) {
                // expected 
            }
        }
    }
}