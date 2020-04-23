// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

namespace NakedObjects.SystemTest.PolymorphicAssociations {
    public class FixtureEntities {
        #region Injected Services

        public IDomainObjectContainer Container { set; protected get; }

        #endregion

        public void Install() {
            //Create Payments 1 to 11
            for (var i = 1; i <= 11; i++) {
                NewPersisted<PolymorphicPayment>();
            }

            NewPersisted<CustomerAsPayee>();
            NewPersisted<CustomerAsPayee>();

            NewPersisted<InvoiceAsPayableItem>();
            NewPersisted<InvoiceAsPayableItem>();
            NewPersisted<InvoiceAsPayableItem>();
        }

        private T NewPersisted<T>() where T : class, new() {
            var t = Container.NewTransientInstance<T>();
            Container.Persist(ref t);
            return t;
        }
    }
}