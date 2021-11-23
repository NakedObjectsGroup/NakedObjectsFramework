namespace AW.Types;

public class SpecialOffer : IHasModifiedDate, IHasRowGuid
{

    public SpecialOffer() { }

    public SpecialOffer(SpecialOffer cloneFrom)
    {
        SpecialOfferID = cloneFrom.SpecialOfferID;
        Description = cloneFrom.Description;
        DiscountPct = cloneFrom.DiscountPct;
        Type = cloneFrom.Type;
        Category = cloneFrom.Category;
        StartDate = cloneFrom.StartDate;
        EndDate = cloneFrom.EndDate;
        MinQty = cloneFrom.MinQty;
        MaxQty = cloneFrom.MaxQty;
        ModifiedDate = cloneFrom.ModifiedDate;
        rowguid = cloneFrom.rowguid;
    }
    [Hidden]
    public int SpecialOfferID { get; init; }

    [MemberOrder(10)]
    public string Description { get; init; } = "";

    [MemberOrder(20)]
    [Mask("P")]
    public decimal DiscountPct { get; init; }

    [MemberOrder(30)]
    public string Type { get; init; } = "";

    [MemberOrder(40)]
    public string Category { get; init; } = "";

    [MemberOrder(51)]
    [Mask("d")]
    public DateTime StartDate { get; init; }

    [MemberOrder(52)]
    [Mask("d")]
    public DateTime EndDate { get; init; }

    [MemberOrder(61)]
    public int MinQty { get; init; }

    [MemberOrder(62)]
    public int? MaxQty { get; init; }

    [MemberOrder(99)]
    [Versioned]
    public DateTime ModifiedDate { get; init; }

    [Hidden]
    public Guid rowguid { get; init; }

    public override string ToString() => Description;
}