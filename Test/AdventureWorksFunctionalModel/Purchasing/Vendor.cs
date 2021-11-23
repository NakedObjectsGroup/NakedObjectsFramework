namespace AW.Types;

public class Vendor : IBusinessEntity
{
    [MemberOrder(10)]
    public string AccountNumber { get; init; } = "";

    [MemberOrder(20)]
    public string Name { get; init; } = "";

    [MemberOrder(30)]
    public byte CreditRating { get; init; }

    [MemberOrder(40)]
    public virtual bool PreferredVendorStatus { get; init; }

    [MemberOrder(50)]
    public bool ActiveFlag { get; init; }

    [MemberOrder(60)]
    public string? PurchasingWebServiceURL { get; init; }

    [Named("Product - Order Info")]
    [AWNotCounted] //To test this capability
    public virtual ICollection<ProductVendor> Products { get; init; } = new List<ProductVendor>();

    [MemberOrder(99), Versioned]
    public DateTime ModifiedDate { get; init; }

    [Hidden]
    public int BusinessEntityID { get; init; }

    public virtual IQueryable<string> AutoCompletePurchasingWebServiceURL([MinLength(2)] string value)
    {
        var matchingNames = new List<string> { "http://www.store1.com", "http://www.store2.com", "http://www.store3.com" };
        return from p in matchingNames.AsQueryable() select p.Trim();
    }

    public override string ToString() => $"{Name}";
}