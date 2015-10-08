using NakedObjects;
using System;
using System.ComponentModel;

namespace AdventureWorksModel
{
    [DisplayName("Address")]
    public  class BusinessEntityAddress {
        #region Injected Services
        public IDomainObjectContainer Container { set; protected get; }
        #endregion

        public override string ToString() {
            var t = Container.NewTitleBuilder();
            t.Append(AddressType).Append(":", Address);
            return t.ToString();
        }

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

        [MemberOrder(1)]
        public virtual AddressType AddressType { get; set; }

        [MemberOrder(2)]
        public virtual Address Address { get; set; }

        [NakedObjectsIgnore]
        public virtual BusinessEntity BusinessEntity { get; set; }
    }
}
