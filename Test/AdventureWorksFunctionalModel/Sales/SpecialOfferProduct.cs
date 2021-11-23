namespace AW.Types;

public class SpecialOfferProduct
{
    [Hidden]
    public int SpecialOfferID { get; init; }

    [MemberOrder(1)]

    public virtual SpecialOffer SpecialOffer { get; init; }


    [Hidden]
    public int ProductID { get; init; }

    [MemberOrder(2)]
    public virtual Product Product { get; init; }


    [Hidden]
    public Guid rowguid { get; init; }

    [MemberOrder(99), Versioned]
    public DateTime ModifiedDate { get; init; }

    public override string ToString() => $"SpecialOfferProduct: {SpecialOfferID}-{ProductID}";
}