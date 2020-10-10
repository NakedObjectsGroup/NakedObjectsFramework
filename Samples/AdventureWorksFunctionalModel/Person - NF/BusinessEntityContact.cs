using NakedFunctions;
using System;
using System.ComponentModel.DataAnnotations;

namespace AdventureWorksModel {
    [Named("Contact")]
    public record BusinessEntityContact: IHasRowGuid, IHasModifiedDate {

            //BusinessEntityID = businessEntityId;
            //BusinessEntity = businessEntity;
            //PersonID = personID;
            //Person = person;
            //ContactTypeID = contactTypeID;
            //ContactType = contactType;
            //this.rowguid = rowguid;
            //this.ModifiedDate = ModifiedDate;


        [Hidden]
        public virtual int BusinessEntityID { get; init; }

        [Hidden]
        public virtual BusinessEntity BusinessEntity { get; init; }

        [Hidden]
        public virtual int PersonID { get; init; }

        public virtual Person Person { get; init; }

        [Hidden]
        public virtual int ContactTypeID { get; init; }
        public virtual ContactType ContactType { get; init; }

        [Hidden]
        public virtual Guid rowguid { get; init; }

        [MemberOrder(99),ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; init; }
    }
}
