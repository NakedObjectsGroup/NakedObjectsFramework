namespace AW.Types;

public class SalesPersonQuotaHistory
{
    [Hidden]
    public int BusinessEntityID { get; init; }

    [MemberOrder(1), Mask("d")]
    public DateTime QuotaDate { get; init; }

    [MemberOrder(2), Mask("C")]
    public decimal SalesQuota { get; init; }

    [MemberOrder(3)]
    public virtual SalesPerson SalesPerson { get; init; }

    public override string ToString() => $"{QuotaDate.ToString("d")} {SalesQuota.ToString("C")}";

    [MemberOrder(99), Versioned]
    public DateTime ModifiedDate { get; init; }

    [Hidden]
    public Guid rowguid { get; init; }
}