using NakedObjects;

namespace AdventureWorksModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    
    public class BusinessEntity
    {   
        [NakedObjectsIgnore]
        public virtual int BusinessEntityID { get; set; }

        #region Row Guid and Modified Date

        #region rowguid

        [NakedObjectsIgnore]
        public virtual Guid BusinessEntityRowguid { get; set; }

        #endregion

        #region ModifiedDate

        [NakedObjectsIgnore]
        public virtual DateTime BusinessEntityModifiedDate { get; set; }

        #endregion

        #endregion

        private ICollection<BusinessEntityAddress> _addresses = new List<BusinessEntityAddress>();

        [DisplayName("Addresses")]
        [Eagerly(EagerlyAttribute.Do.Rendering)]
        [TableView(false, "AddressType", "Address")] 
        public virtual ICollection<BusinessEntityAddress> BusinessEntityAddresses
        {
            get { return _addresses; }
            set { _addresses = value; }
        }

        private ICollection<BusinessEntityContact> _contacts = new List<BusinessEntityContact>();

        [DisplayName("Contacts")]
        public virtual ICollection<BusinessEntityContact> BusinessEntityContacts {
            get { return _contacts; }
            set { _contacts = value; }
        }

        public virtual bool HideBusinessEntityContacts() {
            return false;
        }


    }
}
