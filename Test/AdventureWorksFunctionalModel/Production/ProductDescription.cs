namespace AW.Types;

public class ProductDescription : IHasRowGuid, IHasModifiedDate
{
    [Hidden]
    public int ProductDescriptionID { get; init; }

    [MultiLine(10)]
    [MemberOrder(2)]
    public string Description { get; init; } = "";

    [MemberOrder(99), Versioned]
    public DateTime ModifiedDate { get; init; }

    [Hidden]
    public Guid rowguid { get; init; }

    public override string ToString() => Description;
}