namespace AW.Types;

[Bounded]
public class ShipMethod
{

    [Hidden]
    public int ShipMethodID { get; init; }

    [MemberOrder(1)]
    public string Name { get; init; } = "";

    [MemberOrder(2)]
    public decimal ShipBase { get; init; }

    [MemberOrder(3)]
    public decimal ShipRate { get; init; }

    [MemberOrder(99), Versioned]
    public DateTime ModifiedDate { get; init; }

    [Hidden]
    public Guid rowguid { get; init; }

    public override string ToString() => Name;
}