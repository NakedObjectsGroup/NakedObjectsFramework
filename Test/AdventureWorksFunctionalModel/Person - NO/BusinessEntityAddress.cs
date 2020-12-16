using NakedObjects;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AdventureWorksModel
{
    [DisplayName("Address")]
    public  class BusinessEntityAddress {
        #region Injected Services
        public IDomainObjectContainer Container { set; protected get; }
        #endregion

        #region LifeCycle methods
        public void Persisting() {
            rowguid = Guid.NewGuid();
            ModifiedDate = DateTime.Now;
        }
        #endregion

        public override string ToString() {
            var t = Container.NewTitleBuilder();
            t.Append(AddressType).Append(":", Address);
            return t.ToString();
        }

        [Disabled]
        public virtual int BusinessEntityID { get; set; }

        [Disabled]
        public virtual int AddressID { get; set; }

        [Disabled]
        public virtual int AddressTypeID { get; set; }
        #region Row Guid and Modified Date

        #region rowguid

        [NakedObjectsIgnore]
        public virtual Guid rowguid { get; set; }

        #endregion

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        [ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion

        #endregion

        [MemberOrder(1)]
        [Optionally]
        public virtual AddressType AddressType { get; set; }

        [MemberOrder(2)]
        [Optionally]
        public virtual Address Address { get; set; }

        [MemberOrder(3)]
        public virtual BusinessEntity BusinessEntity { get; set; }
    }
}
