// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using NakedObjects;

namespace AdventureWorksModel {
    [IconName("default.png")]
    public abstract class Customer : AWDomainObject, IHasIntegerId {
        private ICollection<CustomerAddress> _CustomerAddress = new List<CustomerAddress>();
        public ContactRepository ContactRepository { set; protected get; }

        [Disabled, Description("xxx")]
        public virtual string AccountNumber { get; set; }

        [NakedObjectsIgnore]
        public virtual string CustomerType { get; set; }

        [NakedObjectsIgnore]
        public virtual int? SalesTerritoryID { get; set; }

        [Optionally]
        public virtual SalesTerritory SalesTerritory { get; set; }

        #region IHasIntegerId Members

        #region ID

        [NakedObjectsIgnore]
        public virtual int Id { get; set; }

        #endregion

        #endregion

        #region Life Cycle Methods

        public override void Persisting() {
            base.Persisting();
            this.CustomerModifiedDate = DateTime.Now;
            this.CustomerRowguid = Guid.NewGuid();
        }

        #endregion

        [NakedObjectsIgnore]
        public virtual string Type() {
            return IsIndividual() ? "Individual" : "Store";
        }

        [NakedObjectsIgnore]
        public virtual bool IsIndividual() {
            return CustomerType == "I";
        }

        [NakedObjectsIgnore]
        public virtual bool IsStore() {
            return CustomerType == "S";
        }

        #region ModifiedDate and rowguid

        #region ModifiedDate

        [NakedObjectsIgnore]
        public virtual DateTime CustomerModifiedDate { get; set; }

        #endregion

        #region rowguid

        [NakedObjectsIgnore]
        public virtual Guid CustomerRowguid { get; set; }

        #endregion

        #endregion

        #region Addresses

        [Disabled, Description("Use Actions")]
        [Eagerly(EagerlyAttribute.Do.Rendering)]
        [TableView(true)] // Table view == List View
        public virtual ICollection<CustomerAddress> Addresses {
            get { return _CustomerAddress; }
            set { _CustomerAddress = value; }
        }

        #region Creating Addresses

        public Address CreateNewAddress() {
            var _Address = Container.NewTransientInstance<Address>();
            _Address.ForCustomer = this;
            return _Address;
        }

        #endregion

        #endregion
    }
}