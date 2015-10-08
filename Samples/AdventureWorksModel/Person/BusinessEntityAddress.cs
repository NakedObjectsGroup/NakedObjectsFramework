using NakedObjects;
using System;

namespace AdventureWorksModel
{
    public  class BusinessEntityAddress
    {
        [NakedObjectsIgnore]
        public virtual int BusinessEntityID { get; set; }

         [NakedObjectsIgnore]
        public virtual int AddressID { get; set; }

         [NakedObjectsIgnore]
        public virtual int AddressTypeID { get; set; }
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

    
        public virtual Address Address { get; set; }
        public virtual AddressType AddressType { get; set; }
        public virtual BusinessEntity BusinessEntity { get; set; }
    }
}
