// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NakedObjects;
using System.ComponentModel;


namespace AdventureWorksModel {
    [IconName("default.png")]
    public abstract class Customer : AWDomainObject, IHasIntegerId {

        #region Life Cycle Methods

        public override void Persisting()
        {
            base.Persisting();
            this.CustomerModifiedDate = DateTime.Now;
            this.CustomerRowguid = Guid.NewGuid();
        }


        #endregion

        public ContactRepository ContactRepository { set; protected get; }


        private ICollection<CustomerAddress> _CustomerAddress = new List<CustomerAddress>();

        #region ID

        [Hidden]
        public virtual int Id { get; set; }

        #endregion

        [Disabled, Description("xxx")]
        public virtual string AccountNumber { get; set; }

        [Hidden]
        public virtual string CustomerType { get; set; }

        [Optionally]
        public virtual SalesTerritory SalesTerritory { get; set; }

        #region ModifiedDate and rowguid

        #region ModifiedDate

        [Hidden]
        public virtual DateTime CustomerModifiedDate { get; set; }

        #endregion

        #region rowguid

        [Hidden]
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


        public Address CreateNewAddress()
        {
            var _Address = Container.NewTransientInstance<Address>();
            _Address.ForCustomer = this;
            return _Address;
        }

        #endregion

        #endregion


        [Hidden]
        public virtual string Type() {
            return IsIndividual() ? "Individual" : "Store";
        }

        [Hidden]
        public virtual bool IsIndividual() {
            return CustomerType == "I";
        }

        [Hidden]
        public virtual bool IsStore() {
            return CustomerType == "S";
        }
    }
}