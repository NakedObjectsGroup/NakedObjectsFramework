namespace AW.Types;

[Bounded]
public class SalesTerritory
{
    [Hidden]
    public int TerritoryID { get; init; }

    [MemberOrder(10)]
    public string Name { get; init; } = "";

    [MemberOrder(20)]
    public string CountryRegionCode { get; init; } = "";

    [MemberOrder(30)]
    public string Group { get; init; } = "";

    [MemberOrder(40)]
    [Mask("C")]
    public decimal SalesYTD { get; init; }

    [MemberOrder(41)]
    [Mask("C")]
    public decimal SalesLastYear { get; init; }

    [MemberOrder(42)]
    [Mask("C")]
    public decimal CostYTD { get; init; }

    [MemberOrder(43)]
    [Mask("C")]
    public decimal CostLastYear { get; init; }

    [Named("States/Provinces covered")]
    [TableView(true)] //Table View == List View
    public virtual ICollection<StateProvince> StateProvince { get; init; } = new List<StateProvince>();

    [Hidden]
    public Guid rowguid { get; init; }

    [MemberOrder(99), Versioned]
    public DateTime ModifiedDate { get; init; }

    public override string ToString() => Name;
}