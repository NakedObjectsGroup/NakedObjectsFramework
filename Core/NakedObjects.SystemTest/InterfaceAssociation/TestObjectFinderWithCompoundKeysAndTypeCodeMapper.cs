// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects.Services;
using NakedObjects.SystemTest.ObjectFinderCompoundKeys;
using NUnit.Framework;

namespace NakedObjects.SystemTest.TestObjectFinderWithCompoundKeysAndTypeCodeMapper {
    [TestFixture]
    public class TestObjectFinderWithCompoundKeysAndTypeCodeMapper : TestObjectFinderWithCompoundKeysAbstract {
        protected override string[] Namespaces => new[] {typeof(Payment).Namespace};

        protected override Type[] Types => new[] {
            typeof(IPayee),
            typeof(Payment),
            typeof(CustomerOne),
            typeof(CustomerTwo), 
            typeof(CustomerThree),
            typeof(CustomerFour),
            typeof(Supplier),
            typeof(Employee)
        };

        protected override Type[] Services =>
            new[] {
                typeof(ObjectFinderWithTypeCodeMapper),
                typeof(SimpleRepository<Payment>),
                typeof(SimpleRepository<CustomerOne>),
                typeof(SimpleRepository<CustomerTwo>),
                typeof(SimpleRepository<CustomerThree>),
                typeof(SimpleRepository<CustomerFour>),
                typeof(SimpleRepository<Supplier>),
                typeof(SimpleRepository<Employee>),
                typeof(SimpleTypeCodeMapper)
            };

        [SetUp]
        public void SetUp() => Initialize();

        [TearDown]
        public void TearDown() => CleanUp();

        [OneTimeSetUp]
        public void FixtureSetUp() {
            PaymentContext.Delete();
            var context = Activator.CreateInstance<PaymentContext>();

            context.Database.Create();
            DatabaseInitializer.Seed(context);
            InitializeNakedObjectsFramework(this);
        }

        [OneTimeTearDown]
        public void TearDownTest() {
            CleanupNakedObjectsFramework(this);
            PaymentContext.Delete();
        }

        [Test]
        public void ChangeAssociatedObjectType() {
            payee1.SetObject(customer2a);
            key1.AssertValueIsEqual("CU2|1|1001");
            payee1.SetObject(supplier1);
            Assert.AreEqual(payee1.ContentAsObject, supplier1);

            key1.AssertValueIsEqual("SUP|1|2001");
        }

        [Test]
        public void ClearAssociatedObject() {
            payee1.SetObject(customer2a);
            payee1.ClearObject();
            key1.AssertIsEmpty();
        }

        [Test]
        public void FailsIfCodeNotRecognised() {
            key1.SetValue("EMP|1");
            try {
                payee1.AssertIsNotEmpty();
                Assert.Fail("Exception should have been thrown");
            }
            catch (Exception ex) {
                Assert.AreEqual("Code not recognised: EMP", ex.Message);
            }
        }

        [Test]
        public void FailsIfTooFewKeysSupplied() {
            key1.SetValue("CU3|1|1001");
            try {
                payee1.AssertIsNotEmpty();
                Assert.Fail("Exception should have been thrown");
            }
            catch (Exception ex) {
                Assert.AreEqual("Number of keys provided does not match the number of keys specified for type: NakedObjects.SystemTest.ObjectFinderCompoundKeys.CustomerThree", ex.Message);
            }
        }

        [Test]
        public void FailsIfTooManyKeysSupplied() {
            key1.SetValue("CU2|1|1001|2001");
            try {
                payee1.AssertIsNotEmpty();
                Assert.Fail("Exception should have been thrown");
            }
            catch (Exception ex) {
                Assert.AreEqual("Number of keys provided does not match the number of keys specified for type: NakedObjects.SystemTest.ObjectFinderCompoundKeys.CustomerTwo", ex.Message);
            }
        }

        [Test]
        public void FailsIfTypeNameIsEmpty() {
            key1.SetValue("|1|1001|2001");
            try {
                payee1.AssertIsNotEmpty();
                Assert.Fail("Exception should have been thrown");
            }
            catch (Exception ex) {
                Assert.AreEqual("Compound key: |1|1001|2001 does not contain an object type", ex.Message);
            }
        }

        [Test]
        public void FailsIfTypeNotRecognisedByEncodingService() {
            try {
                payee1.SetObject(emp1);
                Assert.Fail("Exception should have been thrown");
            }
            catch (Exception ex) {
                Assert.AreEqual("Type not recognised: NakedObjects.SystemTest.ObjectFinderCompoundKeys.Employee", ex.Message);
            }
        }

        [Test]
        public void GetAssociatedObject() {
            key1.SetValue("CU2|1|1001");
            payee1.AssertIsNotEmpty();
            payee1.ContentAsObject.GetPropertyByName("Id").AssertValueIsEqual("1");

            payee1.ClearObject();

            key1.SetValue("CU2|2|1002");
            payee1.AssertIsNotEmpty();
            payee1.ContentAsObject.GetPropertyByName("Id").AssertValueIsEqual("2");
        }

        [Test]
        public void NoAssociatedObject() {
            key1.AssertIsEmpty();
        }

        [Test]
        public void SetAssociatedObject() {
            payee1.SetObject(customer2a);
            key1.AssertValueIsEqual("CU2|1|1001");

            payee1.SetObject(customer2b);
            Assert.AreEqual(payee1.ContentAsObject, customer2b);

            key1.AssertValueIsEqual("CU2|2|1002");
        }

        [Test]
        public void WorksWithASingleIntegerKey() {
            payee1.SetObject(customer1);
            key1.AssertValueIsEqual("CU1|1");
            payee1.ClearObject();

            key1.SetValue("CU1|1");
            payee1.AssertIsNotEmpty();
            payee1.AssertObjectIsEqual(customer1);
        }

        [Test]
        public void WorksWithTripleIntegerKey() {
            payee1.SetObject(customer3);
            key1.AssertValueIsEqual("CU3|1|1001|2001");
            payee1.ClearObject();

            key1.SetValue("CU3|1|1001|2001");
            payee1.AssertIsNotEmpty();
            payee1.AssertObjectIsEqual(customer3);
        }
    }

    #region Classes used by test

    public class SimpleTypeCodeMapper : ITypeCodeMapper {
        #region ITypeCodeMapper Members

        public Type TypeFromCode(string code) =>
            code switch {
                "CU1" => typeof(CustomerOne),
                "CU2" => typeof(CustomerTwo),
                "CU3" => typeof(CustomerThree),
                "SUP" => typeof(Supplier),
                _ => throw new DomainException("Code not recognised: " + code)
            };

        public string CodeFromType(Type type) {
            if (type == typeof(CustomerOne)) { return "CU1"; }

            if (type == typeof(CustomerTwo)) { return "CU2"; }

            if (type == typeof(CustomerThree)) { return "CU3"; }

            if (type == typeof(Supplier)) { return "SUP"; }

            throw new DomainException("Type not recognised: " + type);
        }

        #endregion
    }

    #endregion
}