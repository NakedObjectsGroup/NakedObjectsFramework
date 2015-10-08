using NakedObjects;

namespace AdventureWorksModel
{
    using System;
    using System.Collections.Generic;
    
    public class BusinessEntity
    {   
        [NakedObjectsIgnore]
        public virtual int BusinessEntityID { get; set; }

        #region Row Guid and Modified Date

        #region rowguid

        [NakedObjectsIgnore]
        public virtual Guid rowguid { get; set; }

        #endregion

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion

        #endregion

        private ICollection<BusinessEntityAddress> _addresses = new List<BusinessEntityAddress>();

        public virtual ICollection<BusinessEntityAddress> BusinessEntityAddresses
        {
            get { return _addresses; }
            set { _addresses = value; }
        }

        private ICollection<BusinessEntityContact> _contacts = new List<BusinessEntityContact>();

        public virtual ICollection<BusinessEntityContact> BusinessEntityContacts {
            get { return _contacts; }
            set { _contacts = value; }
        }
    }
}
