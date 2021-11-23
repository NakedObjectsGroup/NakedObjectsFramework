namespace AW.Types;

[Bounded]
public class ScrapReason
{

    [Hidden]
    public short ScrapReasonID { get; init; }

    public string Name { get; init; } = "";

    [MemberOrder(99), Versioned]
    public DateTime ModifiedDate { get; init; }

    public override string ToString() => Name;
}