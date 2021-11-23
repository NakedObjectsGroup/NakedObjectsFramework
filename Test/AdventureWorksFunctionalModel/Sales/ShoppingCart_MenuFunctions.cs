using System.Security.Principal;

namespace AW.Functions;

[Named("Cart")]
public static class ShoppingCart_MenuFunctions
{
    //TODO: The Cart should probably be a view model
    [Named("Show Cart")]
    public static IQueryable<ShoppingCartItem> Cart(IContext context)
    {
        var id = GetShoppingCartIDForUser(context);
        return context.Instances<ShoppingCartItem>().Where(x => x.ShoppingCartID == id);
    }

    public static string? DisableCart(IContext context) => DisableIfNoCustomerForUser(context);

    private static string GetShoppingCartIDForUser(IContext context) => GetCustomerForUser(context).CustomerID.ToString();

    public static IContext AddToShoppingCart(Product product, IContext context)
    {
        var id = GetShoppingCartIDForUser(context);
        var newItem = new ShoppingCartItem { ShoppingCartID = id, Product = product, Quantity = 1, DateCreated = context.Now() };
        return context.WithNew(newItem).WithInformUser($"1 x {product} added to Cart");
    }

    public static (SalesOrderHeader, IContext) CheckOut(IContext context)
    {
        var cust = GetCustomerForUser(context);
        throw new NotImplementedException();
    }

    public static string? DisableCheckOut(IContext context) => DisableIfNoCustomerForUser(context);

    private static Customer GetCustomerForUser(IContext context) => throw new NotImplementedException();

    private static Person GetContactFromUserNameAsEmail(IContext context) => throw new NotImplementedException();

    private static string UserName(IPrincipal principal) => principal.Identity?.Name ?? "";

    public static IContext AddAllItemsInCartToOrder(
        SalesOrderHeader order, IContext context)
    {
        var items = Cart(context);
        var details = items.Select(item => order.CreateNewDetail(item.Product, (short)item.Quantity, context));
        var context2 = details.Aggregate(context, (c, d) => c.WithNew(d));
        var context3 = EmptyCart(context);
        return context3;
    }

    public static IContext RemoveItems(IQueryable<ShoppingCartItem> items, IContext context) => throw new NotImplementedException();

    public static IContext EmptyCart(IContext context) => throw new NotImplementedException();

    public static string? DisableEmptyCart(IContext context) => DisableIfNoCustomerForUser(context);

    public static string? DisableIfNoCustomerForUser(IContext context) => GetCustomerForUser(context) == null ? "User is not a recognised Customer" : null;
}