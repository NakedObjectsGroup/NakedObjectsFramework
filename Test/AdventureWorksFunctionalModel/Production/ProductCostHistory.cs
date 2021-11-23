namespace AW.Types;

public class ProductCostHistory : IHasModifiedDate
{

    [Hidden]
    public int ProductID { get; init; }

    public DateTime StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public decimal StandardCost { get; init; }

    [Hidden]
    public virtual Product Product { get; init; }

    [MemberOrder(99), Versioned]
    public DateTime ModifiedDate { get; init; }

    public override string ToString() => $"{StandardCost} {StartDate}~";
}