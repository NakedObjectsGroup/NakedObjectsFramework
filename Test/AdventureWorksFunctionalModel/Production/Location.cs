namespace AW.Types;

[Bounded, PresentationHint("Topaz")]
public class Location
{
    [Hidden]
    public short LocationID { get; init; }

    public string Name { get; init; } = "";

    [Mask("C")]
    public decimal CostRate { get; init; }

    [Mask("########.##")]
    public decimal Availability { get; init; }

    [MemberOrder(99), Versioned]
    public DateTime ModifiedDate { get; init; }

    public override string ToString() => Name;
}