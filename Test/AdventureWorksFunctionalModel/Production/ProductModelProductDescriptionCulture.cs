namespace AW.Types;

public class ProductModelProductDescriptionCulture : IHasModifiedDate
{
    [Hidden]
    public int ProductModelID { get; init; }

    [Hidden]
    public int ProductDescriptionID { get; init; }

    [Hidden]
    public string CultureID { get; init; } = "";

    public virtual Culture Culture { get; init; }

    public virtual ProductDescription ProductDescription { get; init; }

    [Hidden]
    public virtual ProductModel ProductModel { get; init; }

    [MemberOrder(99), Versioned]
    public DateTime ModifiedDate { get; init; }

    public override string ToString() => $"ProductModelProductDescriptionCulture: {ProductModelID}-{ProductDescriptionID}-{CultureID}";
}