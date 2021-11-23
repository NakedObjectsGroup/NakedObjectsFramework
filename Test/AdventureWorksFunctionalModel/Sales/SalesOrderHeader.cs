namespace AW.Types;

[Named("Sales Order")]
public class SalesOrderHeader : ICreditCardCreator
{
    public SalesOrderHeader() { }

    public SalesOrderHeader(SalesOrderHeader cloneFrom)
    {
        SalesOrderID = cloneFrom.SalesOrderID;
        SalesOrderNumber = cloneFrom.SalesOrderNumber;
        AddItemsFromCart = cloneFrom.AddItemsFromCart;
        StatusByte = cloneFrom.StatusByte;
        CustomerID = cloneFrom.CustomerID;
        Customer = cloneFrom.Customer;
        BillingAddressID = cloneFrom.BillingAddressID;
        BillingAddress = cloneFrom.BillingAddress;
        PurchaseOrderNumber = cloneFrom.PurchaseOrderNumber;
        ShippingAddressID = cloneFrom.ShippingAddressID;
        ShippingAddress = cloneFrom.ShippingAddress;
        ShipMethodID = cloneFrom.ShipMethodID;
        ShipMethod = cloneFrom.ShipMethod;
        AccountNumber = cloneFrom.AccountNumber;
        OrderDate = cloneFrom.OrderDate;
        ShipDate = cloneFrom.ShipDate;
        DueDate = cloneFrom.DueDate;
        SubTotal = cloneFrom.SubTotal;
        TaxAmt = cloneFrom.TaxAmt;
        Freight = cloneFrom.Freight;
        TotalDue = cloneFrom.TotalDue;
        CurrencyRateID = cloneFrom.CurrencyRateID;
        CurrencyRate = cloneFrom.CurrencyRate;
        OnlineOrder = cloneFrom.OnlineOrder;
        CreditCardID = cloneFrom.CreditCardID;
        CreditCard = cloneFrom.CreditCard;
        CreditCardApprovalCode = cloneFrom.CreditCardApprovalCode;
        RevisionNumber = cloneFrom.RevisionNumber;
        Comment = cloneFrom.Comment;
        SalesPersonID = cloneFrom.SalesPersonID;
        SalesPerson = cloneFrom.SalesPerson;
        SalesTerritoryID = cloneFrom.SalesTerritoryID;
        SalesTerritory = cloneFrom.SalesTerritory;
        Details = cloneFrom.Details;
        SalesOrderHeaderSalesReason = cloneFrom.SalesOrderHeaderSalesReason;
        ModifiedDate = cloneFrom.ModifiedDate;
        rowguid = cloneFrom.rowguid;
    }

    [Hidden]
    public int SalesOrderID { get; init; }

    [MemberOrder(1)]
    public string SalesOrderNumber { get; init; } = "";

    public bool AddItemsFromCart { get; init; }

    [Hidden]
    public byte StatusByte { get; init; }

    [MemberOrder(1)]
    public virtual OrderStatus Status => (OrderStatus)StatusByte;

    [Hidden]
    public int CustomerID { get; init; }

    [MemberOrder(2)]
    public virtual Customer Customer { get; init; }

    [Hidden]
    public int BillingAddressID { get; init; }

    [MemberOrder(4)]
    public virtual Address BillingAddress { get; init; }

    [MemberOrder(5)]
    public string? PurchaseOrderNumber { get; init; }

    [Hidden]
    public int ShippingAddressID { get; init; }

    [MemberOrder(10)]
    public virtual Address ShippingAddress { get; init; }

    [Hidden]
    public int ShipMethodID { get; init; }

    [MemberOrder(11)]
    public virtual ShipMethod ShipMethod { get; init; }

    [MemberOrder(12)]
    public string? AccountNumber { get; init; }

    [MemberOrder(20)]
    public DateTime OrderDate { get; init; }

    [MemberOrder(21)]
    public DateTime DueDate { get; init; }

    [MemberOrder(22), Mask("d")]
    public DateTime? ShipDate { get; init; }

    [MemberOrder(31), Mask("C")]
    public decimal SubTotal { get; init; }

    [MemberOrder(32), Mask("C")]
    public decimal TaxAmt { get; init; }

    [MemberOrder(33), Mask("C")]
    public decimal Freight { get; init; }

    [MemberOrder(34)]
    [Mask("C")]
    public decimal TotalDue { get; init; }

    [Hidden]
    public int? CurrencyRateID { get; init; }

    [MemberOrder(35)]
    public virtual CurrencyRate? CurrencyRate { get; init; }

    [DescribedAs("Order has been placed via the web")]
    [MemberOrder(41), Named("Online Order")]
    public bool OnlineOrder { get; init; }

    [Hidden]
    public int? CreditCardID { get; init; }

    [MemberOrder(42)]
    public virtual CreditCard? CreditCard { get; init; }

    [MemberOrder(43)]
    public string? CreditCardApprovalCode { get; init; }

    [MemberOrder(51)]
    public byte RevisionNumber { get; init; }

    [MultiLine(NumberOfLines = 3, Width = 50), MemberOrder(52)]
    public string? Comment { get; init; }

    [Hidden]
    public int? SalesPersonID { get; init; }

    [MemberOrder(61)]
    public virtual SalesPerson? SalesPerson { get; init; }

    [Hidden]
    public int? SalesTerritoryID { get; init; }

    [MemberOrder(62)]
    public virtual SalesTerritory? SalesTerritory { get; init; }

    public virtual ICollection<SalesOrderDetail> Details { get; init; } = new List<SalesOrderDetail>();

    [Named("Reasons")]
    public virtual ICollection<SalesOrderHeaderSalesReason> SalesOrderHeaderSalesReason { get; init; } = new List<SalesOrderHeaderSalesReason>();

    [MemberOrder(99), Versioned]
    public DateTime ModifiedDate { get; init; }

    [Hidden]
    public Guid rowguid { get; init; }

    public override string ToString() => $"{SalesOrderNumber}";
}

public enum OrderStatus : byte
{
    InProcess = 1,
    Approved = 2,
    BackOrdered = 3,
    Rejected = 4,
    Shipped = 5,
    Cancelled = 6
}