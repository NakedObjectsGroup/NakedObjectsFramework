namespace AW.Types;

[Bounded]
public class StateProvince : IHasRowGuid, IHasModifiedDate
{
    [Hidden]
    public int StateProvinceID { get; init; }

    public string StateProvinceCode { get; init; } = "";

    public bool IsOnlyStateProvinceFlag { get; init; }

    public string Name { get; init; } = "";

    [Hidden]
    public string CountryRegionCode { get; init; } = "";

    public virtual CountryRegion CountryRegion { get; init; }

    [Hidden]
    public int TerritoryID { get; init; }

    public virtual SalesTerritory SalesTerritory { get; init; }

    [MemberOrder(99)]
    [Versioned]
    public DateTime ModifiedDate { get; init; }

    [Hidden]
    public Guid rowguid { get; init; }

    public override string ToString() => Name;
}