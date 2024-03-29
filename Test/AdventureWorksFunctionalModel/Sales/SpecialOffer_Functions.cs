namespace AW.Functions;

public static class SpecialOffer_Functions
{

    public static List<Product> ListAssociatedProducts(this SpecialOffer specialOffer, IContext context)
    {
        var id = specialOffer.SpecialOfferID;
        return context.Instances<SpecialOfferProduct>().Where(x => x.SpecialOfferID == id).Select(x => x.Product).ToList();
    }

    #region Edit

    public static IContext EditDescription(
        this SpecialOffer sp, string description, IContext context) =>
        context.WithUpdated(sp, new(sp)
        {
            Description = description,
            ModifiedDate = context.Now()
        });

    public static IContext EditDiscount(
        this SpecialOffer sp, decimal discountPct, IContext context) =>
        context.WithUpdated(sp, new(sp)
        {
            DiscountPct = discountPct,
            ModifiedDate = context.Now()
        });

    public static string? DisableEditDiscount(
        this SpecialOffer sp, IContext context) =>
        DisableIfStarted(sp, context);

    public static IContext EditType(
        this SpecialOffer sp, string type, IContext context) =>
        context.WithUpdated(sp, new(sp)
        {
            Type = type,
            ModifiedDate = context.Now()
        });

    public static bool HideEditType(
        this SpecialOffer sp, IContext context) =>
        HideIfEnded(sp, context);

    public static IContext EditCategory(
        this SpecialOffer sp, string category, IContext context) =>
        context.WithUpdated(sp, new(sp)
        {
            Category = category,
            ModifiedDate = context.Now()
        });

    public static IList<string> Choices1Category(this SpecialOffer sp) => Categories;

    internal static IList<string> Categories = new[] { "Reseller", "Customer" };

    internal static string? DisableIfStarted(this SpecialOffer so, IContext context) =>
        context.Today() > so.StartDate ? "Offer has started" : null;

    internal static bool HideIfEnded(this SpecialOffer so, IContext context) =>
        context.Today() > so.EndDate;

    public static IContext EditDates(this SpecialOffer sp,
                                     DateTime startDate, DateTime endDate, IContext context) =>
        context.WithUpdated(sp, new(sp)
        {
            StartDate = startDate,
            EndDate = endDate,
            ModifiedDate = context.Now()
        });

    public static DateTime Default1EditDates(this SpecialOffer sp, IContext context) =>
        sp.StartDate;

    public static DateTime Default2EditDates(this SpecialOffer sp, IContext context) =>
        DefaultEndDate(context);

    internal static DateTime DefaultEndDate(IContext context) =>
        context.GetService<IClock>().Today().AddMonths(1);

    public static IContext EditQuantities(this SpecialOffer sp, [DefaultValue(1)] int minQty, [Optionally] int? maxQty, IContext context) =>
        context.WithUpdated(sp, new(sp)
        {
            MinQty = minQty,
            MaxQty = maxQty,
            ModifiedDate = context.Now()
        });

    public static string? Validate1EditQuantities(this SpecialOffer sp, int minQty) =>
        minQty < 1 ? "Must be > 0" : null;

    public static string? ValidateEditQuantities(this SpecialOffer sp, [DefaultValue(1)] int minQty, [Optionally] int? maxQty, IContext context) =>
        ValidateQuantities(minQty, maxQty);

    internal static string? ValidateQuantities(int minQty, int? maxQty) =>
        maxQty != null && maxQty.Value < minQty ? "Max Qty cannot be < Min Qty" : null;

    #endregion

    #region AssociateWithProduct

    public static IContext AssociateWithProduct(
        this SpecialOffer offer, Product product, IContext context)
    {
        var prev = context.Instances<SpecialOfferProduct>().Where(x => x.SpecialOfferID == offer.SpecialOfferID && x.ProductID == product.ProductID).Count() == 0;
        if (prev)
        {
            var newSO = new SpecialOfferProduct { SpecialOffer = offer, Product = product, ModifiedDate = context.Now(), rowguid = context.NewGuid() };
            return context.WithNew(newSO);
        }

        return context.WithInformUser($"{offer} is already associated with {product}");
    }

    [PageSize(20)]
    public static IQueryable<Product> AutoComplete1AssociateWithProduct(this SpecialOffer offer, [MinLength(2)] string name, IContext context)
        => context.Instances<Product>().Where(product => product.Name.ToUpper().StartsWith(name.ToUpper()));

    #endregion

    #region Queryable-contributed

    internal static IContext Change(this IQueryable<SpecialOffer> offers,
                                    Func<SpecialOffer, SpecialOffer> change, IContext context) =>
        offers.Select(x => new { original = x, updated = change(x) })
              .Aggregate(context, (c, of) => c.WithUpdated(of.original, of.updated));

    //TODO: This example shows we must permit returning a List (not a queryable) for display.
    public static IContext ExtendOffers(this IQueryable<SpecialOffer> offers, DateTime toDate, IContext context) =>
        Change(offers, sp => new(sp) { EndDate = toDate, ModifiedDate = context.Now() }, context);

    public static IContext TerminateOffers(
        this IQueryable<SpecialOffer> offers, IContext context)
    {
        var yesterday = context.Today().AddDays(-1);
        return Change(offers, sp => new(sp) { EndDate = yesterday, ModifiedDate = context.Now() }, context);
    }

    #endregion
}