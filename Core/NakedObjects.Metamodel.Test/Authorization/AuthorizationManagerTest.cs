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
using NakedObjects.Meta.Authorization;

namespace NakedObjects.Meta.Test.Authorization {
    [TestClass]
    public class AuthorizationManagerTest {
        #region Setup/Teardown

        [TestInitialize]
        public void SetUp() {}

        #endregion

        [TestMethod]
        public void TestCreateOk() {
            var config = new Mock<IAuthorizationConfiguration>();

            config.Setup(c => c.DefaultAuthorizer).Returns(typeof (object));
            config.Setup(c => c.NamespaceAuthorizers).Returns(new Dictionary<string, Type> {{"1", typeof (object)}});
            config.Setup(c => c.TypeAuthorizers).Returns(new Dictionary<string, Type>());

            // ReSharper disable once UnusedVariable
            var sink = new AuthorizationManager(config.Object);
        }

        [TestMethod]
        public void TestCreateNullAuthorizer() {
            var config = new Mock<IAuthorizationConfiguration>();

            config.Setup(c => c.DefaultAuthorizer).Returns((Type) null);
            config.Setup(c => c.NamespaceAuthorizers).Returns(new Dictionary<string, Type>());

            try {
                // ReSharper disable once UnusedVariable
                var sink = new AuthorizationManager(config.Object);
                Assert.Fail("Expect exception");
            }
            catch (Exception expected) {
                // pass test
                Assert.AreEqual("Default Authorizer cannot be null", expected.Message);
            }
        }
    }
}