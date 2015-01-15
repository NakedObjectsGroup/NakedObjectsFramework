// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NakedObjects.Audit;
using NakedObjects.Meta.Audit;

namespace NakedObjects.Meta.Test.Audit {
    [TestClass]
    public class AuditManagerTest {
        #region Setup/Teardown

        [TestInitialize]
        public void SetUp() {}

        #endregion

        [TestMethod]
        public void TestCreateOk() {
            var config = new Mock<IAuditConfiguration>();
            var auditor = new Mock<IAuditor>();

            config.Setup(c => c.DefaultAuditor).Returns(auditor.Object.GetType());
            config.Setup(c => c.NamespaceAuditors).Returns(new Dictionary<string, Type>() {{"", auditor.Object.GetType()}});

            // ReSharper disable once UnusedVariable
            var sink = new AuditManager(config.Object);
        }

        [TestMethod]
        public void TestCreateWrongDefaultAuditorType() {
            var config = new Mock<IAuditConfiguration>();

            config.Setup(c => c.DefaultAuditor).Returns(typeof (object));
            config.Setup(c => c.NamespaceAuditors).Returns(new Dictionary<string, Type>());

            try {
                // ReSharper disable once UnusedVariable
                var sink = new AuditManager(config.Object);
                Assert.Fail("Expect exception");
            }
            catch (Exception expected) {
                // pass test
                Assert.AreEqual("System.Object is not an IAuditor", expected.Message);
            }
        }

        [TestMethod]
        public void TestCreateWrongNamespaceAuditorType() {
            var config = new Mock<IAuditConfiguration>();
            var auditor = new Mock<IAuditor>();

            config.Setup(c => c.DefaultAuditor).Returns(auditor.Object.GetType());
            config.Setup(c => c.NamespaceAuditors).Returns(new Dictionary<string, Type> {{"", typeof (object)}});

            try {
                // ReSharper disable once UnusedVariable
                var sink = new AuditManager(config.Object);
                Assert.Fail("Expect exception");
            }
            catch (Exception expected) {
                // pass test
                Assert.AreEqual("System.Object is not an IAuditor", expected.Message);
            }
        }
    }
}