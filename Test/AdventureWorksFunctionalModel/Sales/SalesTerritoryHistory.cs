namespace AW.Types;

public class SalesTerritoryHistory
{
    public SalesTerritoryHistory() { }

    public SalesTerritoryHistory(SalesTerritoryHistory cloneFrom)
    {
        BusinessEntityID = cloneFrom.BusinessEntityID;
        StartDate = cloneFrom.StartDate;
        EndDate = cloneFrom.EndDate;
        SalesPerson = cloneFrom.SalesPerson;
        SalesTerritoryID = cloneFrom.SalesTerritoryID;
        SalesTerritory = cloneFrom.SalesTerritory;
        ModifiedDate = cloneFrom.ModifiedDate;
        rowguid = cloneFrom.rowguid;
    }

    [Hidden]
    public int BusinessEntityID { get; init; }

    [MemberOrder(1), Mask("d")]
    public DateTime StartDate { get; init; }

    [MemberOrder(2), Mask("d")]
    public DateTime? EndDate { get; init; }

    [MemberOrder(3)]
    public virtual SalesPerson SalesPerson { get; init; }

    [Hidden]
    public int SalesTerritoryID { get; init; }

    [MemberOrder(4)]
    public virtual SalesTerritory SalesTerritory { get; init; }

    [MemberOrder(99), Versioned]
    public DateTime ModifiedDate { get; init; }

    [Hidden]
    public Guid rowguid { get; init; }

    public override string ToString() => $"{SalesPerson} {SalesTerritory}";
}