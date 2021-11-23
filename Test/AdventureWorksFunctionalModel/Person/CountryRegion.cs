namespace AW.Types;

[Bounded]
public class CountryRegion : IHasModifiedDate
{
    [MemberOrder(1)]
    public string Name { get; init; } = "";

    [MemberOrder(2)]
    public string CountryRegionCode { get; init; } = "";

    [MemberOrder(99),Versioned]
    public DateTime ModifiedDate { get; init; }

    public override string ToString() => Name;
}