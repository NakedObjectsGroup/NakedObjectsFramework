﻿using NakedFramework.Value;

namespace AW.Functions;
public static class Product_Functions
{
    #region CurrentWorkOrders

    [TableView(true, "Product", "OrderQty", "StartDate")]
    public static IQueryable<WorkOrder> CurrentWorkOrders(
        this Product product, IContext context) =>
        WorkOrder_MenuFunctions.ListWorkOrders(product, true, context);

    #endregion

    internal static int NumberInStock(this Product p) =>
        p.ProductInventory.Sum(obj => obj.Quantity);

    public static IContext AddOrChangePhoto(this Product product, Image newImage, IContext context)
    {
        //TODO: code looks wrong. Would be clearer to separate add and change photo? Also, Product should not need updating?
        var productProductPhoto = product.ProductProductPhoto.First();
        var productPhoto = productProductPhoto.ProductPhoto;
        ProductPhoto newProductPhoto = new(productPhoto) { LargePhoto = newImage.GetResourceAsByteArray(), LargePhotoFileName = newImage.Name };
        var newProductProductPhoto = new ProductProductPhoto { ProductPhoto = newProductPhoto, Product = product, ModifiedDate = DateTime.Now };
        Product newProduct = new(product) { ProductProductPhoto = new List<ProductProductPhoto> { newProductProductPhoto } };
        return context.WithUpdated(product, newProduct).WithUpdated(productProductPhoto, newProductProductPhoto).WithUpdated(productPhoto, newProductPhoto);
    }

    [CreateNew]
    public static (WorkOrder, IContext context) CreateNewWorkOrder(
        this Product product,
        int orderQty,
        DateTime startDate,
        IContext context) =>
        WorkOrder_MenuFunctions.CreateNewWorkOrder(product, orderQty, startDate, context);

    public static IContext AddProductReview(this Product p,
                                            [DefaultValue(0)][ValueRange(-30, 0)] DateTime dateOfReview,
                                            [Named("No. of Stars (1-5")] [DefaultValue(5)]
                                                int rating,
                                            [Optionally] string comments,
                                            IContext context) =>
        context.WithNew(CreateReview(
                            p,
                            context.CurrentUser()?.Identity?.Name ?? "",
                            dateOfReview,
                            "[private]",
                            rating,
                            comments,
                            context));

    private static ProductReview CreateReview(Product p, string reviewerName, DateTime date, string emailAddress, int rating, string comments, IContext context) =>
        new()
        {
            Product = p,
            ReviewerName = reviewerName,
            ReviewDate = date,
            EmailAddress = emailAddress,
            Rating = rating,
            Comments = comments,
            ModifiedDate = context.Now()
        };

    public static List<int> Choices2AddProductReview(this Product p) => Ratings();

    private static List<int> Ratings() => new() { 1, 2, 3, 4, 5 };

    public static string? ValidateAddProductReview(this Product p,
                                                   DateTime dateOfReview, int rating, string comments) =>
        LessThan5StarsRequiresComment(rating, comments);

    private static string? LessThan5StarsRequiresComment(int rating, string comments) =>
        rating < 5 && string.IsNullOrEmpty(comments) ? "Must provide comments for rating < 5" : null;

    public static IContext AddAnonReviews(this IQueryable<Product> products,
                                          [Named("No. of Stars (1-5)")] [DefaultValue(5)]
                                              int rating,
                                          [Optionally] string comments,
                                          IContext context) =>
        products.Aggregate(context, (c, p) => c.WithNew(
                               CreateReview(p, "Anon.", context.Today(), "[hidden]", rating, comments, context)));

    public static List<int> Choices1AddAnonReviews(this IQueryable<Product> products) =>
        Ratings();

    public static string? ValidateAddAnonReviews(this IQueryable<Product> products,
                                                 int rating, string comments) =>
        LessThan5StarsRequiresComment(rating, comments);

    [DisplayAsProperty]
    [MemberOrder(11)]
    public static ProductDescription? Description(this Product product) =>
        product.ProductModel is null ? null : ProductModel_Functions.LocalCultureDescription(product.ProductModel);

    [DisplayAsProperty]
    [MemberOrder(110)]
    public static ICollection<SpecialOffer> SpecialOffers(this IProduct product, IContext context)
    {
        // Implementation uses context to check that this works
        var pid = product.ProductID;
        return context.Instances<SpecialOfferProduct>().Where(sop => sop.ProductID == pid).Select(sop => sop.SpecialOffer).ToList();
        // Simpler implementation would be just:
        // product.SpecialOfferProduct.Select(sop => sop.SpecialOffer).ToList()
    }

    internal static Image? Photo(Product product)
    {
        var p = product.ProductProductPhoto.Select(p => p.ProductPhoto).FirstOrDefault();
        return p is null ? null : new Image(p.LargePhoto, p.LargePhotoFileName);
    }

    #region Helpers

    internal static string WeightWithUnit(this Product p) =>
        $"{p.Weight} {p.WeightUnit}";

    internal static ProductCategory? ProductCategory(this Product p) =>
        p.ProductSubcategory is null ? null : p.ProductSubcategory.ProductCategory;

    internal static string SizeWithUnit(this Product p) =>
        $"{p.Size} {p.SizeUnit}";

    #endregion

    #region BestSpecialOffer

    [DescribedAs("Determines the best discount offered by current special offers for a specified order quantity")]
    public static SpecialOffer BestSpecialOffer(
        this Product p, [ValueRange(1, 999)] int quantity, IContext context) =>
        BestSpecialOfferProduct(p, (short)quantity, context).SpecialOffer;

    public static string? DisableBestSpecialOffer(this Product p, IContext context)
        => p.IsDiscontinued(context) ? "Product is discontinued" : null;

    internal static SpecialOfferProduct BestSpecialOfferProduct(
        this Product p, short quantity, IContext context)
    {
        //TODO: Currently this ignores end date, but this is because all special offers in the AW database,
        //including No Discount have ended!
        var pID = p.ProductID;
        var today = context.Today();
        var best = context.Instances<SpecialOfferProduct>().Where(obj => obj.Product.ProductID == p.ProductID &&
                                                                         obj.SpecialOffer.StartDate <= today &&
                                                                         obj.SpecialOffer.MinQty < quantity).OrderByDescending(obj => obj.SpecialOffer.DiscountPct).FirstOrDefault();
        if (best is null)
        {
            throw new Exception($"No Special Offers new(associated) {p}");
        }

        return best;
    }

    private static bool IsDiscontinued(this Product p, IContext context) =>
        p.DiscontinuedDate != null ? p.DiscontinuedDate.Value < context.Now() : false;

    #endregion

    #region Associate with Special Offer

    public static IContext AssociateWithSpecialOffer(
        this Product product, SpecialOffer offer, IContext context) =>
        offer.AssociateWithProduct(product, context);

    [PageSize(20)]
    public static IQueryable<SpecialOffer> AutoComplete1AssociateWithSpecialOffer(
        this Product product,
        [MinLength(2)] string name, IContext context) =>
        context.Instances<SpecialOffer>().Where(specialOffer => specialOffer.Description.ToUpper().StartsWith(name.ToUpper()));

    #endregion

    #region Edits

    [Edit]
    public static IContext EditProductLine(this Product p,
                                           string productLine, IContext context) =>
        context.WithUpdated(p, new(p)
        {
            ProductLine = productLine,
            ModifiedDate = context.Now()
        });

    public static IList<string> Choices1EditProductLine(this Product p)
        => new List<string> { "R ", "M ", "T ", "S " }; // nchar(2) in database so pad right with space

    [Edit]
    public static IContext EditClass(this Product p,
                                     string @class, IContext context) =>
        context.WithUpdated(p, new(p)
        {
            Class = @class,
            ModifiedDate = context.Now()
        });

    public static IList<string> Choices1EditClass(this Product p) =>
        new[] { "H ", "M ", "L " }; // nchar(2) in database so pad right with space

    [Edit]
    public static IContext EditStyle(this Product p,
                                     string style, IContext context) =>
        context.WithUpdated(p, new(p)
        {
            Style = style,
            ModifiedDate = context.Now()
        });

    public static IList<string> Choices1EditStyle(this Product p) =>
        new[] { "U ", "M ", "W " }; // nchar(2) in database so pad right with space

    [Edit]
    public static IContext EditProductModel(this Product p,
                                            ProductModel productModel, IContext context) =>
        context.WithUpdated(p, new(p)
        {
            ProductModel = productModel,
            ModifiedDate = context.Now()
        });

    public static IQueryable<ProductModel> AutoComplete1EditProductModel(this Product p,
                                                                         [MinLength(3)] string match, IContext context)
    {
        return context.Instances<ProductModel>().Where(pm => pm.Name.ToUpper().Contains(match.ToUpper()));
    }

    [Edit]
    public static IContext EditCategories(this Product p, ProductCategory productCategory, ProductSubcategory productSubcategory, IContext context) =>
        context.WithUpdated(p, new(p)
        {
            ProductSubcategory = productSubcategory,
            ModifiedDate = context.Now()
        });

    public static IList<ProductSubcategory> Choices2EditCategories(this Product p,
                                                                   ProductCategory productCategory, IContext context) =>
        productCategory is null
            ? new ProductSubcategory[] { }
            : context.Instances<ProductSubcategory>().Where(psc => psc.ProductCategory.ProductCategoryID == productCategory.ProductCategoryID).ToArray();

    #endregion
}