namespace AW.Types;

public class ProductInventory : IHasRowGuid, IHasModifiedDate
{
    [Hidden]
    public int ProductID { get; init; }

    [Hidden]
    public short LocationID { get; init; }

    [MemberOrder(40)]
    public string Shelf { get; init; } = "";

    [MemberOrder(50)]
    public byte Bin { get; init; }

    [MemberOrder(10)]
    public short Quantity { get; init; }

    [MemberOrder(30)]
    public virtual Location Location { get; init; }

    [MemberOrder(20)]
    public virtual Product Product { get; init; }

    [MemberOrder(99), Versioned]
    public DateTime ModifiedDate { get; init; }

    [Hidden]
    public Guid rowguid { get; init; }

    public override string ToString() => $"{Quantity} in {Location} - {Shelf}";
}