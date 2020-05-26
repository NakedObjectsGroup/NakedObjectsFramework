// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Meta.Adapter;

namespace NakedObjects.Meta.Test.Adapter {
    [TestClass]
    public class TestIdentifierImpl {
        #region Setup/Teardown

        [TestInitialize]
        public void SetUp() { }

        #endregion

        [TestMethod]
        public void TestCreateOk() {
            var identifier = new IdentifierImpl("testclass");
            Assert.IsNotNull(identifier);
        }

        [TestMethod]
        public void TestToClassString() {
            var identifier = new IdentifierImpl("testclass");
            var s = identifier.ToIdentityString(IdentifierDepth.Class);
            Assert.AreEqual("testclass", s);
        }

        [TestMethod]
        public void TestToClassNameString() {
            var identifier = new IdentifierImpl("testclass", "testfield");
            var s = identifier.ToIdentityString(IdentifierDepth.ClassName);
            Assert.AreEqual("testclass#testfield", s);
        }

        [TestMethod]
        public void TestToClassNameParamsString() {
            var identifier = new IdentifierImpl("testclass", "testmethod", new[] {"testparam1", "testparam2"});
            var s = identifier.ToIdentityString(IdentifierDepth.ClassNameParams);
            Assert.AreEqual("testclass#testmethod(testparam1,testparam2)", s);
        }

        [TestMethod]
        public void TestEquals() {
            var identifier1 = new IdentifierImpl("testclass", "testmethod", new[] {"testparam1", "testparam2"});
            var identifier2 = new IdentifierImpl("testclass", "testmethod", new[] {"testparam1", "testparam2"});
            Assert.AreEqual(identifier1, identifier2);
        }

        [TestMethod]
        public void TestHash() {
            var identifier1 = new IdentifierImpl("testclass", "testmethod", new[] {"testparam1", "testparam2"});
            var identifier2 = new IdentifierImpl("testclass", "testmethod", new[] {"testparam1", "testparam2"});
            var testDict = new Dictionary<IdentifierImpl, string> {{identifier1, "1"}};
            Assert.IsTrue(testDict.ContainsKey(identifier2));
        }

        [TestMethod]
        public void TestToClassNameParamsStringWithActionCheck() {
            var identifier = new IdentifierImpl("testclass", "testmethod", new[] {"testparam1", "testparam2"});
            var s = identifier.ToIdentityStringWithCheckType(IdentifierDepth.ClassNameParams, CheckType.Action);
            Assert.AreEqual("testclass#testmethod(testparam1,testparam2):Action", s);
        }

        [TestMethod]
        public void TestToClassNameParamsStringWithViewCheck() {
            var identifier = new IdentifierImpl("testclass", "testmethod", new[] {"testparam1", "testparam2"});
            var s = identifier.ToIdentityStringWithCheckType(IdentifierDepth.ClassNameParams, CheckType.ViewField);
            Assert.AreEqual("testclass#testmethod(testparam1,testparam2):ViewField", s);
        }

        [TestMethod]
        public void TestToClassNameParamsStringWithEditCheck() {
            var identifier = new IdentifierImpl("testclass", "testmethod", new[] {"testparam1", "testparam2"});
            var s = identifier.ToIdentityStringWithCheckType(IdentifierDepth.ClassNameParams, CheckType.EditField);
            Assert.AreEqual("testclass#testmethod(testparam1,testparam2):EditField", s);
        }

        [TestMethod]
        public void TestFromIdString() {
            var mockMetaModel = new Mock<IMetamodel>();

            IIdentifier id = IdentifierImpl.FromIdentityString(mockMetaModel.Object, "testclass#testmethod(testparam1,testparam2)");

            Assert.AreEqual("testclass", id.ClassName);
            Assert.AreEqual("testmethod", id.MemberName);
            Assert.AreEqual(2, id.MemberParameterTypeNames.Count());
            Assert.AreEqual("testparam1", id.MemberParameterTypeNames.First());
            Assert.AreEqual("testparam2", id.MemberParameterTypeNames.Last());
            Assert.AreEqual(2, id.MemberParameterNames.Count());
            Assert.AreEqual("", id.MemberParameterNames.First());
            Assert.AreEqual("", id.MemberParameterNames.Last());
        }

        [TestMethod]
        public void TestToNameString() {
            var identifier = new IdentifierImpl("testclass", "testfield");
            var s = identifier.ToIdentityString(IdentifierDepth.Name);
            Assert.AreEqual("testfield", s);
        }

        [TestMethod]
        public void TestToParmsString() {
            var identifier = new IdentifierImpl("testclass", "testmethod", new[] {"testparam1", "testparam2"});
            var s = identifier.ToIdentityString(IdentifierDepth.Parms);
            Assert.AreEqual("(testparam1,testparam2)", s);
        }
    }
}