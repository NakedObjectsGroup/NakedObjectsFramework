// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Reflection;
using Moq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Spec;
using NUnit.Framework;
using NakedObjects.Metamodel.Facet;

namespace NakedObjects.Reflector.DotNet.Facets.Actions.Validate {
    [TestFixture]
    public class ActionParameterValidationFacetViaMethodTest {
        #region Setup/Teardown

        [SetUp]
        public void SetUp() {
            ISpecification holder = null;
            var customer = new Customer17();

            var mock = new Mock<INakedObject>();
            target = mock.Object;

            mock.Setup(no => no.Object).Returns(customer);

            MethodInfo method = typeof (Customer17).GetMethod("Validate0SomeAction");
            facet = new ActionParameterValidation(method, 0, holder);
        }

        [TearDown]
        public void TearDown() {}

        #endregion

        private INakedObject target;
        private ActionParameterValidation facet;

        [Test]
        public void Test1() {
            var mock = new Mock<INakedObject>();
            INakedObject value = mock.Object;
            mock.Setup(no => no.Object).Returns(10);
            Assert.That(facet.InvalidReason(target, value), Is.Null);
        }


        [Test]
        public void Test2() {
            var mock = new Mock<INakedObject>();
            INakedObject value = mock.Object;
            mock.Setup(no => no.Object).Returns(-7);
            Assert.That(facet.InvalidReason(target, value), Is.EqualTo("must be positive"));
        }
    }


    internal class Customer17 {
        public void SomeAction(int x, long y, long z) {}

        public string Validate0SomeAction(int x) {
            return x > 0 ? null : "must be positive";
        }

        public string Validate1SomeAction(long x) {
            return null;
        }
    }
}