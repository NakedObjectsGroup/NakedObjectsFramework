






using System;
using System.Linq;
using AW.Functions;
using NakedFunctions;

namespace AW.Types {

    public class Store : BusinessEntity, IBusinessEntityWithContacts, IHasModifiedDate {
        public Store() { }
       
        public Store(Store cloneFrom) : base(cloneFrom)
        {
            Name = cloneFrom.Name;
            Demographics = cloneFrom.Demographics;
            SalesPersonID = cloneFrom.SalesPersonID;
            SalesPerson = cloneFrom.SalesPerson;
            ModifiedDate = cloneFrom.ModifiedDate;
            rowguid = cloneFrom.rowguid;
        }

        #region Properties

        [Named("Store Name")] [MemberOrder(20)]
        public string Name { get; init; } = "";

        #region Demographics

        [Hidden]
        public string? Demographics { get; init; }

        [Named("Demographics")] [MemberOrder(30)] [MultiLine(10)]
        public string FormattedDemographics => Utilities.FormatXML(Demographics);

        #endregion

        #region SalesPerson

        [Hidden]
        public int? SalesPersonID { get; init; }

        [MemberOrder(40)]
        public virtual SalesPerson? SalesPerson { get; init; }

        [PageSize(20)]
        public IQueryable<SalesPerson> AutoCompleteSalesPerson(
            [MinLength(2)] string name, IContext context) =>
            Sales_MenuFunctions.FindSalesPersonByName(null, name, context);

        #endregion

        #region ModifiedDate and rowguid

        #region ModifiedDate

        [MemberOrder(99)]
        [Versioned]
        public DateTime ModifiedDate { get; init; }

        #endregion

        #region rowguid

        [Hidden]
        public Guid rowguid { get; init; }

        #endregion

        #endregion

        #endregion

        public override string ToString() => Name;

    }
}