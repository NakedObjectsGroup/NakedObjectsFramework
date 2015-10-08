using NakedObjects;
using System;

namespace AdventureWorksModel
{
    public partial class BusinessEntityContact
    {
        [NakedObjectsIgnore]
        public virtual int BusinessEntityID { get; set; }
                [NakedObjectsIgnore]
        public virtual int PersonID { get; set; }
                [NakedObjectsIgnore]
        public virtual int ContactTypeID { get; set; }
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

        public virtual BusinessEntity BusinessEntity { get; set; }
        public virtual ContactType ContactType { get; set; }
        public virtual Contact Person { get; set; }
    }
}
