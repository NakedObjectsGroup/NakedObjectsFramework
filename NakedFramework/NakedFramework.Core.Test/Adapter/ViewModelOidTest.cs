// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Moq;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Component;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Adapter;
using NUnit.Framework;

namespace NakedObjects.Core.Test.Adapter {
    [TestFixture]
    public class ViewModelOidTest {
        private readonly Mock<IMetamodelManager> mockMetamodel = new Mock<IMetamodelManager>();
        private readonly Mock<IObjectSpec> mockObjectSpec = new Mock<IObjectSpec>();

        [SetUp]
        public void SetUp() {
            mockObjectSpec.Setup(f => f.FullName).Returns("System.Object");
            mockMetamodel.Setup(f => f.GetSpecification(It.IsAny<string>())).Returns(mockObjectSpec.Object);
        }

        [Test]
        public void TestDefaultIsFinal() {
            IViewModelOid testOid = new ViewModelOid(mockMetamodel.Object, mockObjectSpec.Object);
            Assert.IsFalse(testOid.IsFinal);
        }

        [Test]
        public void TestUpdateIsFinal() {
            IViewModelOid testOid = new ViewModelOid(mockMetamodel.Object, mockObjectSpec.Object);
            var testkeys = new[] {"key1", "key2"};

            testOid.UpdateKeys(testkeys, true);

            Assert.AreEqual(testkeys, testOid.Keys);
            Assert.IsTrue(testOid.IsFinal);
        }
    }
}