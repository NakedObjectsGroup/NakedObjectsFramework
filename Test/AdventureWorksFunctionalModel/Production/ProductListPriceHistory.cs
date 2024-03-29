namespace AW.Types;

public class ProductListPriceHistory : IHasModifiedDate
{
    public int ProductID { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public decimal ListPrice { get; init; }

    public virtual Product Product { get; init; }

    [MemberOrder(99), Versioned]
    public DateTime ModifiedDate { get; init; }

    public override string ToString() => $"ProductListPriceHistory: {ProductID}";
}