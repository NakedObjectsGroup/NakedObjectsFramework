namespace AW.Types;

public class PurchaseOrderHeader : IHasModifiedDate
{
    public PurchaseOrderHeader() { }

    public PurchaseOrderHeader(PurchaseOrderHeader cloneFrom)
    {
        PurchaseOrderID = cloneFrom.PurchaseOrderID;
        RevisionNumber = cloneFrom.RevisionNumber;
        ShipMethodID = cloneFrom.ShipMethodID;
        ShipMethod = cloneFrom.ShipMethod;
        Details = cloneFrom.Details;
        VendorID = cloneFrom.VendorID;
        Vendor = cloneFrom.Vendor;
        Status = cloneFrom.Status;
        OrderDate = cloneFrom.OrderDate;
        ShipDate = cloneFrom.ShipDate;
        SubTotal = cloneFrom.SubTotal;
        TaxAmt = cloneFrom.TaxAmt;
        Freight = cloneFrom.Freight;
        TotalDue = cloneFrom.TotalDue;
        OrderPlacedByID = cloneFrom.OrderPlacedByID;
        OrderPlacedBy = cloneFrom.OrderPlacedBy;
        ModifiedDate = cloneFrom.ModifiedDate;
    }

    [Hidden]
    public int PurchaseOrderID { get; init; }

    [MemberOrder(90)]
    public byte RevisionNumber { get; init; }

    [Hidden]
    public int ShipMethodID { get; init; }

    [MemberOrder(22)]
    public virtual ShipMethod ShipMethod { get; init; }

    [RenderEagerly, TableView(true, "OrderQty", "Product", "UnitPrice", "LineTotal")]
    public virtual ICollection<PurchaseOrderDetail> Details { get; init; }

    [Hidden]
    public int VendorID { get; init; }

    [MemberOrder(1)]
    public virtual Vendor Vendor { get; init; }

    [Hidden]
    public byte Status { get; init; }

    [Named("Status"), MemberOrder(1)]
    public virtual POStatus StatusAsEnum => (POStatus)Status;

    [MemberOrder(11), Mask("d")]
    public DateTime OrderDate { get; init; }

    [MemberOrder(20), Mask("d")]
    public DateTime? ShipDate { get; init; }

    [MemberOrder(31), Mask("C")]
    public decimal SubTotal { get; init; }

    [MemberOrder(32), Mask("C")]
    public decimal TaxAmt { get; init; }

    [MemberOrder(33), Mask("C")]
    public decimal Freight { get; init; }

    [MemberOrder(34), Mask("C")]
    public decimal TotalDue { get; init; }

    [Hidden]
    public int OrderPlacedByID { get; init; }

    [MemberOrder(12)]
    public virtual Employee OrderPlacedBy { get; init; }

    [MemberOrder(99), Versioned]
    public DateTime ModifiedDate { get; init; }

    public override string ToString() => $"PO from {Vendor}, {OrderDate}";
}

public enum POStatus
{
    Pending = 1,
    Approved = 2,
    Rejected = 3,
    Complete = 4
}