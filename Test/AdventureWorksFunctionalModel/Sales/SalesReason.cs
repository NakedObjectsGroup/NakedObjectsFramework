namespace AW.Types;

[Bounded]
public class SalesReason
{
    [Hidden]
    public int SalesReasonID { get; init; }

    public string Name { get; init; } = "";

    public string ReasonType { get; init; } = "";

    [MemberOrder(99)]
    [Versioned]
    public DateTime ModifiedDate { get; init; }

    public override string ToString() => Name;
}