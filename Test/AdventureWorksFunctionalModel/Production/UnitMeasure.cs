namespace AW.Types;

[Bounded]
public class UnitMeasure : IHasModifiedDate
{
    [MemberOrder(10)]
    public string UnitMeasureCode { get; init; } = "";

    [MemberOrder(20)]
    public string Name { get; init; } = "";

    [MemberOrder(99), Versioned]
    public DateTime ModifiedDate { get; init; }

    public override string ToString() => Name;
}