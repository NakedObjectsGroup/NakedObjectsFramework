// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel;
using NakedObjects;

namespace AdventureWorksModel {
    [IconName("default.png")]
    public class Customer  {
        #region Injected Services
        public PersonRepository ContactRepository { set; protected get; }
        public IDomainObjectContainer Container { set; protected get; }
        #endregion

        #region Life Cycle Methods
        public virtual void Persisting() {
            CustomerRowguid = Guid.NewGuid();
            CustomerModifiedDate = DateTime.Now;
        }

        public virtual void Updating() {
            CustomerModifiedDate = DateTime.Now;
        }
        #endregion

        #region Title
        public override string ToString() {
            var t = Container.NewTitleBuilder();
            if (IsStore()) {
                t.Append(Store);
            } else {
                t.Append(Person);
            }
            t.Append(",", AccountNumber);
            return t.ToString();
        }
        #endregion

        [NakedObjectsIgnore]
        public virtual int CustomerID { get; set; }

        [Disabled, Description("xxx"), MemberOrder(10)]
        public virtual string AccountNumber { get; set; }

        #region Sales Territory
        [NakedObjectsIgnore]
        public virtual int? SalesTerritoryID { get; set; }

        [Disabled, MemberOrder(20)]
        public virtual SalesTerritory SalesTerritory { get; set; }
        #endregion

        #region Store & Personal customers

        internal BusinessEntity BusinessEntity() {
            if (IsStore()) return Store;
            if (IsIndividual()) return Person;
            throw new DomainException("Customer is neithe Store nor Person!");
        }

        [NakedObjectsIgnore]
        public virtual int? StoreID { get; set; }

        [Disabled, MemberOrder(20)]
        public virtual Store Store { get; set; }

        public bool HideStore() {
            return !IsStore();
        }

        [NakedObjectsIgnore]
        public virtual int? PersonID { get; set; }

        [Disabled, MemberOrder(20)]
        public virtual Person Person { get; set; }

        public bool HidePerson() {
            return !IsIndividual();
        }
        #endregion

        [MemberOrder(15)]
        public virtual string CustomerType {
            get {
                return IsIndividual() ? "Individual" : "Store";
            }
        }

        [NakedObjectsIgnore]
        public virtual bool IsIndividual() {
            return !IsStore();
        }

        [NakedObjectsIgnore]
        public virtual bool IsStore() {
            return Store != null;
        }

        #region ModifiedDate and rowguid

        #region ModifiedDate

        [Hidden(WhenTo.Always)]
        //[ConcurrencyCheck]
        public virtual DateTime CustomerModifiedDate { get; set; }

        #endregion

        #region rowguid

        [NakedObjectsIgnore]
        public virtual Guid CustomerRowguid { get; set; }

        #endregion

        #endregion

        #region Action to test switchable view model
        public StoreSalesInfo ReviewSalesResponsibility()
        {
            var ssi = Container.NewViewModel<StoreSalesInfo>();
            ssi.PopulateUsingKeys(new string[] { AccountNumber, false.ToString() });
            return ssi;
        }


        public bool HideReviewSalesResponsibility()
        {
            return !this.IsStore();
        }

        #endregion
    }
}