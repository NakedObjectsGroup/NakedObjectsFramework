namespace AW.Types;

[Bounded]
public class ProductCategory : IHasRowGuid, IHasModifiedDate
{

    [Hidden]
    public int ProductCategoryID { get; init; }

    public string Name { get; init; } = "";

    [Named("Subcategories")]
    [TableView(true)] //TableView == ListView ?
    public virtual ICollection<ProductSubcategory> ProductSubcategory { get; init; } = new List<ProductSubcategory>();

    [MemberOrder(99), Versioned]
    public DateTime ModifiedDate { get; init; }

    [Hidden]
    public Guid rowguid { get; init; }

    public override string ToString() => Name;
}