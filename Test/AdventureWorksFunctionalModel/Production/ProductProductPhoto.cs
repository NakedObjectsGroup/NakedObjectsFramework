namespace AW.Types;

public class ProductProductPhoto
{
    public int ProductID { get; init; }

    public int ProductPhotoID { get; init; }

    public bool Primary { get; init; }

    public virtual Product Product { get; init; }

    public virtual ProductPhoto ProductPhoto { get; init; }

    [MemberOrder(99), Versioned]
    public DateTime ModifiedDate { get; init; }

    public override string ToString() => $"ProductProductPhoto: {ProductID}-{ProductPhotoID}";
}