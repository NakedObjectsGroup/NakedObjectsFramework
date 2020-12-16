using NakedObjects;

namespace AdventureWorksModel {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class BusinessEntity : IBusinessEntity {
        #region Injected Services 
        public IDomainObjectContainer Container { set; protected get; }
        #endregion

        #region Life Cycle Methods
        public virtual void Persisting() {
            BusinessEntityRowguid = Guid.NewGuid();
            BusinessEntityModifiedDate = DateTime.Now;
        }

        public virtual void Updating() {
            BusinessEntityModifiedDate = DateTime.Now;
        }
        #endregion

        [NakedObjectsIgnore]
        public virtual int BusinessEntityID { get; set; }

        #region Row Guid and Modified Date

        #region rowguid

        [NakedObjectsIgnore]
        public virtual Guid BusinessEntityRowguid { get; set; }

        #endregion

        #region ModifiedDate

        [Hidden(WhenTo.Always)]
        [ConcurrencyCheck]
        public virtual DateTime BusinessEntityModifiedDate { get; set; }

        #endregion

        #endregion

        #region Addresses
        private ICollection<BusinessEntityAddress> _addresses = new List<BusinessEntityAddress>();

        [Eagerly(EagerlyAttribute.Do.Rendering)]
        [TableView(false,
            nameof(BusinessEntityAddress.AddressType),
            nameof(BusinessEntityAddress.Address))] 
        public virtual ICollection<BusinessEntityAddress> Addresses
        {
            get { return _addresses; }
            set { _addresses = value; }
        }

        public Address CreateNewAddress() {
            var _Address = Container.NewTransientInstance<Address>();
            _Address.AddressFor = this;
            return _Address;
        }
        #endregion

        private ICollection<BusinessEntityContact> _contacts = new List<BusinessEntityContact>();

        [Eagerly(EagerlyAttribute.Do.Rendering)]
        [TableView(false, "ContactType", "Person")] 
        public virtual ICollection<BusinessEntityContact> Contacts {
            get { return _contacts; }
            set { _contacts = value; }
        }

        public virtual bool HideContacts() {
            return false;
        }
    }
}
