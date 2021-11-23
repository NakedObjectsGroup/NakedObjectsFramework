using static AW.Helpers;

namespace AW.Functions;

[Named("Products")]
public static class Product_MenuFunctions
{
    [MemberOrder(1)]
    [TableView(true, nameof(Product.ProductNumber), nameof(Product.ProductSubcategory), nameof(Product.ListPrice))]
    public static IQueryable<Product> FindProductByName(string searchString, IContext context) =>
        context.Instances<Product>().Where(x => x.Name.ToUpper().Contains(searchString.ToUpper())).OrderBy(x => x.Name);

    [MemberOrder(2)]
    public static Product RandomProduct(IContext context) => Random<Product>(context);

    [MemberOrder(3)]
    public static Product? FindProductByNumber(string number, IContext context) =>
        context.Instances<Product>().FirstOrDefault(x => x.ProductNumber == number);

    [MemberOrder(4)]
    public static IQueryable<Product> AllProducts(IContext context) => context.Instances<Product>();

    [MemberOrder(5)]
    public static IQueryable<Product> ListProductsByCategory(ProductCategory category, [Optionally] ProductSubcategory? subCategory, IContext context)
    {
        var catId = category.ProductCategoryID;
        var subId = subCategory?.ProductSubcategoryID ?? 0;
        return context.Instances<Product>().Where(p => p.ProductSubcategory != null &&
                                                       p.ProductSubcategory.ProductCategoryID == catId &&
                                                       (subId == 0 || p.ProductSubcategoryID != null && p.ProductSubcategoryID.Value == subId));
    }

    public static List<ProductSubcategory> Choices1ListProductsByCategory(ProductCategory category) =>
        category?.ProductSubcategory.ToList() ?? new List<ProductSubcategory>();

    [MemberOrder(5)]
    public static IQueryable<Product> ListBikes([Disabled] ProductCategory category, [Optionally] ProductSubcategory subCategory, IContext context)
    {
        return ListProductsByCategory(category, subCategory, context);
    }

    public static ProductCategory Default0ListBikes(IContext context) => context.Instances<ProductCategory>().First();
}