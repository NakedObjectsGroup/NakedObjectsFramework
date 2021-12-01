// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Security.Principal;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Spec;
using NakedFramework.Metamodel.Authorization;
using NakedFramework.Security;
using NakedObjects.Reflector.Authorization;

namespace NakedObjects.Reflector.Test.Authorization; 

[TestClass]
public class AuthorizationManagerTest {
    private readonly ILogger<AuthorizationManager> mockLogger = new Mock<ILogger<AuthorizationManager>>().Object;

    #region Setup/Teardown

    [TestInitialize]
    public void SetUp() { }

    #endregion

    [TestMethod]
    public void TestCreateOk() {
        var config = new AuthorizationConfiguration<ITypeAuthorizer<object>>();

        config.AddNamespaceAuthorizer<INamespaceAuthorizer>("1");
        config.AddTypeAuthorizer<TestClass, ITypeAuthorizer<TestClass>>();

        // ReSharper disable once UnusedVariable
        var sink = new AuthorizationManager(config, mockLogger);
    }

    [TestMethod]
    public void TestCreateNullAuthorizer() {
        var config = new Mock<IAuthorizationConfiguration>();

        config.Setup(c => c.DefaultAuthorizer).Returns((Type) null);
        config.Setup(c => c.NamespaceAuthorizers).Returns(new Dictionary<string, Type>());

        try {
            // ReSharper disable once UnusedVariable
            var sink = new AuthorizationManager(config.Object, mockLogger);
            Assert.Fail("Expect exception");
        }
        catch (Exception expected) {
            // pass test
            Assert.AreEqual("Default Authorizer cannot be null", expected.Message);
        }
    }

    [TestMethod]
    public void TestDecorateHideForSessionFacet() {
        var config = new Mock<IAuthorizationConfiguration>();

        config.Setup(c => c.DefaultAuthorizer).Returns(typeof(TestDefaultAuthorizer));
        config.Setup(c => c.NamespaceAuthorizers).Returns(new Dictionary<string, Type> {{"1", typeof(TestNamespaceAuthorizer)}});
        config.Setup(c => c.TypeAuthorizers).Returns(new Dictionary<string, Type>());

        var manager = new AuthorizationManager(config.Object, mockLogger);

        var testSpec = new Mock<ISpecification>();
        var testHolder = new Mock<ISpecification>();
        var identifier = new Mock<IIdentifier>();
        var testFacet = new Mock<IHideForSessionFacet>();

        testHolder.Setup(h => h.Identifier).Returns(identifier.Object);

        testSpec.Setup(s => s.Identifier).Returns(identifier.Object);

        testFacet.Setup(n => n.FacetType).Returns(typeof(IHideForSessionFacet));

        testFacet.Setup(n => n.Specification).Returns(testSpec.Object);

        var facet = manager.Decorate(testFacet.Object, testHolder.Object);

        Assert.IsInstanceOfType(facet, typeof(AuthorizationHideForSessionFacet));
    }

    [TestMethod]
    public void TestDecorateDisableForSessionFacet() {
        var config = new Mock<IAuthorizationConfiguration>();

        config.Setup(c => c.DefaultAuthorizer).Returns(typeof(TestDefaultAuthorizer));
        config.Setup(c => c.NamespaceAuthorizers).Returns(new Dictionary<string, Type> {{"1", typeof(TestNamespaceAuthorizer)}});
        config.Setup(c => c.TypeAuthorizers).Returns(new Dictionary<string, Type>());

        var manager = new AuthorizationManager(config.Object, mockLogger);

        var testSpec = new Mock<ISpecification>();
        var testHolder = new Mock<ISpecification>();
        var identifier = new Mock<IIdentifier>();
        var testFacet = new Mock<IDisableForSessionFacet>();

        testHolder.Setup(h => h.Identifier).Returns(identifier.Object);

        testSpec.Setup(s => s.Identifier).Returns(identifier.Object);

        testFacet.Setup(n => n.FacetType).Returns(typeof(IDisableForSessionFacet));

        testFacet.Setup(n => n.Specification).Returns(testSpec.Object);

        var facet = manager.Decorate(testFacet.Object, testHolder.Object);

        Assert.IsInstanceOfType(facet, typeof(AuthorizationDisableForSessionFacet));
    }

    #region Nested type: TestClass

    public class TestClass { }

    #endregion

    #region Nested type: TestDefaultAuthorizer

    public class TestDefaultAuthorizer : ITypeAuthorizer<object> {
        #region ITypeAuthorizer<object> Members

        public bool IsEditable(IPrincipal principal, object target, string memberName) => true;

        public bool IsVisible(IPrincipal principal, object target, string memberName) => true;

        #endregion
    }

    #endregion

    #region Nested type: TestNamespaceAuthorizer

    public class TestNamespaceAuthorizer : INamespaceAuthorizer {
        #region INamespaceAuthorizer Members

        public bool IsEditable(IPrincipal principal, object target, string memberName) => true;

        public bool IsVisible(IPrincipal principal, object target, string memberName) => true;

        #endregion
    }

    #endregion
}