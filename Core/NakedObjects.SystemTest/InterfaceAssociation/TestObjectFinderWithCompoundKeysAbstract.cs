// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Services;
using NakedObjects.Xat;

namespace NakedObjects.SystemTest.ObjectFinderCompoundKeys {
    public abstract class TestObjectFinderWithCompoundKeysAbstract : AbstractSystemTest<PaymentContext> {
        protected ITestObject customer1;
        protected ITestObject customer2a;
        protected ITestObject customer2b;
        protected ITestObject customer3;
        protected ITestObject customer4;
        protected ITestObject customer4a;
        protected ITestObject emp1;
        protected ITestProperty key1;
        protected ITestProperty payee1;
        protected ITestObject payment1;
        protected ITestObject supplier1;

        public void Initialize() {
            StartTest();
            payment1 = GetAllInstances(typeof(SimpleRepository<Payment>), 0);
            payee1 = payment1.GetPropertyByName("Payee");
            key1 = payment1.GetPropertyByName("Payee Compound Key");

            customer1 = GetAllInstances(typeof(SimpleRepository<CustomerOne>), 0);
            customer2a = GetAllInstances(typeof(SimpleRepository<CustomerTwo>), 0);
            customer2b = GetAllInstances(typeof(SimpleRepository<CustomerTwo>), 1);
            customer3 = GetAllInstances(typeof(SimpleRepository<CustomerThree>), 0);
            customer4 = GetAllInstances(typeof(SimpleRepository<CustomerFour>), 0);
            customer4a = GetAllInstances(typeof(SimpleRepository<CustomerFour>), 1);
            supplier1 = GetAllInstances(typeof(SimpleRepository<Supplier>), 0);
            emp1 = GetAllInstances(typeof(SimpleRepository<Employee>), 0);
        }

        public void CleanUp() {
            EndTest();
            payment1 = null;
            customer1 = null;
            customer2a = null;
            customer2b = null;
            customer3 = null;
            payee1 = null;
            key1 = null;
            emp1 = null;
        }
    }
}