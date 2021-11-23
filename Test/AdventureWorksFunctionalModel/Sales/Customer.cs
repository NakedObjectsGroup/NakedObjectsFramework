using AW.Functions;

namespace AW.Types;

public class Customer
{
    [Hidden]
    public int CustomerID { get; init; }

    [MemberOrder(15)]
    public string CustomerType => this.IsIndividual() ? "Individual" : "Store";

    [DescribedAs("xxx")]
    [MemberOrder(10)]
    public string AccountNumber { get; init; } = "";

    [Hidden]
    public DateTime CustomerModifiedDate { get; init; }

    [Hidden]
    public Guid CustomerRowguid { get; init; }

    public override string ToString() => $"{AccountNumber} {(Store is null ? Person : Store)}";

    #region Store & Personal customers

    [Hidden]
    public int? StoreID { get; init; }

    [MemberOrder(20)]
    public virtual Store? Store { get; init; }

    [Hidden]
    public int? PersonID { get; init; }

    [MemberOrder(20)]
    public virtual Person? Person { get; init; }

    #endregion

    #region Sales Territory

    [Hidden]
    public int? SalesTerritoryID { get; init; }

    [MemberOrder(30)]
    public virtual SalesTerritory? SalesTerritory { get; init; }

    #endregion
}