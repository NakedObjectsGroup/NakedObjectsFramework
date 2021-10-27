using NakedObjects;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AdventureWorksModel {
    [DisplayName("Contact")]
    public partial class BusinessEntityContact {

        #region Injected Services
        public IDomainObjectContainer Container { set; protected get; }
        #endregion

        #region Title
        public override string ToString() {
            var t = Container.NewTitleBuilder();
            t.Append(ContactType).Append(":", Person);
            return t.ToString();
        }
        #endregion

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
        [ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion

        #endregion

        [NakedObjectsIgnore]
        public virtual BusinessEntity BusinessEntity { get; set; }
        public virtual ContactType ContactType { get; set; }
        public virtual Person Person { get; set; }
    }
}
