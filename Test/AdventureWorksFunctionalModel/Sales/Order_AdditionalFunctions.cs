namespace AW.Functions;
[Named("Orders")]
public static class Order_AdditionalFunctions
{
    private const string subMenu = "Orders";

    [MemberOrder(22)]
    [TableView(true, "OrderDate", "Status", "TotalDue")]
    public static IQueryable<SalesOrderHeader> RecentOrders(
        this Customer customer, IContext context) =>
        from obj in context.Instances<SalesOrderHeader>()
        where obj.Customer.CustomerID == customer.CustomerID
        orderby obj.SalesOrderNumber descending
        select obj;

    [MemberOrder(21)]
    [TableView(true, "OrderDate", "TotalDue")]
    public static IQueryable<SalesOrderHeader> OpenOrders(
        this Customer customer, IQueryable<SalesOrderHeader> headers)
    {
        var id = customer.CustomerID;
        return headers.Where(x => x.Customer.CustomerID == id && x.StatusByte <= 3)
                      .OrderByDescending(x => x.SalesOrderNumber);
    }

    #region Comments

    [Named("Append Comment")]
    public static IContext AppendCommentToOrders(this IQueryable<SalesOrderHeader> toOrders, string comment, IContext context)
    {
        var updates = toOrders.Select(x => new { original = x, updated = x.WithAppendedComment(comment, context) });
        return updates.Aggregate(context, (c, of) => c.WithUpdated(of.original, of.updated));
    }

    public static IContext AppendComment(
        this SalesOrderHeader order, string commentToAppend, IContext context)
    {
        var updated = WithAppendedComment(order, commentToAppend, context);
        return context.WithUpdated(order, updated);
    }

    internal static SalesOrderHeader WithAppendedComment(this SalesOrderHeader order, string commentToAppend, IContext context)
    {
        var newComments = order.Comment == null ? commentToAppend : order.Comment + "; " + commentToAppend;
        return new(order) { Comment = newComments, ModifiedDate = context.Now() };
    }

    public static string? Validate1AppendComment(this SalesOrderHeader order, string commentToAppend) => string.IsNullOrEmpty(commentToAppend) ? "Comment required" : null;

    public static IContext CommentAsUsersUnhappy(this IQueryable<SalesOrderHeader> toOrders, IContext context) =>
        AppendCommentToOrders(toOrders, "User unhappy", context);

    public static IContext CommentAsUserUnhappy(this SalesOrderHeader order, IContext context) =>
        AppendComment(order, "User unhappy", context);

    public static string? DisableCommentAsUserUnhappy(this SalesOrderHeader order) => order.IsShipped() ? null : "Not shipped yet";

    [Named("Clear Comments")]
    public static IContext ClearCommentsFromOrders(this IQueryable<SalesOrderHeader> toOrders, IContext context)
    {
        var updates = toOrders.Select(x => new { original = x, updated = WithClearedComments(x, context) });
        return updates.Aggregate(context, (c, of) => c.WithUpdated(of.original, of.updated));
    }

    public static IContext ClearComments(this SalesOrderHeader order, IContext context) =>
        context.WithUpdated(order, WithClearedComments(order, context));

    internal static SalesOrderHeader WithClearedComments(SalesOrderHeader order, IContext context) =>
        new(order) { Comment = "", ModifiedDate = context.Now() };

    #endregion

    #region SearchForOrders

    [MemberOrder(12)]
    [PageSize(10)]
    [TableView(true, "OrderDate", "Status", "TotalDue")]
    public static IQueryable<SalesOrderHeader> SearchForOrders(
        this Customer customer,
        [Optionally] DateTime? fromDate,
        [Optionally] DateTime? toDate,
        IContext context)
    {
        var customerID = customer.CustomerID;

        var headers = from obj in context.Instances<SalesOrderHeader>()
                      where (fromDate == null || obj.OrderDate >= fromDate) &&
                            (toDate == null || obj.OrderDate <= toDate)
                      orderby obj.OrderDate
                      select obj;

        return customer == null
            ? headers
            : headers.Where(x => x.Customer.CustomerID == customerID);
    }

    public static string ValidateSearchForOrders(this Customer customer, DateTime? fromDate, DateTime? toDate)
    {
        if (fromDate.HasValue && toDate.HasValue)
        {
            if (fromDate >= toDate)
            {
                return "'From Date' must be before 'To Date'";
            }
        }

        return "";
    }

    #endregion

    #region QuickOrder

    public static (SalesOrderHeader, IContext) QuickOrder(this Customer customer,
                                                          Product product, short quantity, IContext context)
    {
        var order = NewOrderFrom(context, GetLastOrder(customer, context)!);
        var detail = order.CreateNewDetail(product, quantity, context);
        return (order, context.WithNew(order).WithNew(detail).WithDeferred(
                    c =>
                    {
                        var o = c.Reload(order);
                        return c.WithUpdated(o, o.Recalculated(c));
                    }));
    }

    [PageSize(20)]
    public static IQueryable<Product> AutoComplete1QuickOrder(this Customer customer,
                                                              [MinLength(2)] string name, IContext context) =>
        Product_MenuFunctions.FindProductByName(name, context);

    public static string? DisableQuickOrder(this Customer customer, IContext context) =>
        customer.DisableCreateAnotherOrder(context);

    #endregion

    #region CreateAnotherOrder

    //Automatically copies common header info from previous order
    //Disabled if the customer has no previous orders
    [MemberOrder(1)]
    public static (SalesOrderHeader, IContext) CreateAnotherOrder(
        this Customer customer, IContext context)
    {
        var newOrder = NewOrderFrom(context, GetLastOrder(customer, context)!);
        return (newOrder, context.WithNew(newOrder));
    }

    private static SalesOrderHeader NewOrderFrom(IContext context, SalesOrderHeader previous) =>
        new()
        {
            RevisionNumber = 1,
            OrderDate = context.Today(),
            DueDate = context.Today().AddDays(7),
            StatusByte = (byte)OrderStatus.InProcess,
            OnlineOrder = false,
            Customer = previous.Customer,
            BillingAddress = previous.BillingAddress,
            ShippingAddress = previous.ShippingAddress,
            ShipMethod = previous.ShipMethod,
            CreditCard = previous.CreditCard,
            AccountNumber = previous.AccountNumber,
            rowguid = context.NewGuid(),
            ModifiedDate = context.Now()
        };

    public static string? DisableCreateAnotherOrder(this Customer customer, IContext context) =>
        GetLastOrder(customer, context) is null ? "Customer has no previous orders. Use Create First Order." : null;

    public static SalesOrderHeader? GetLastOrder(this Customer c, IContext context)
    {
        var cid = c.CustomerID;
        return context.Instances<SalesOrderHeader>().Where(o => o.CustomerID == cid).OrderByDescending(o => o.OrderDate).FirstOrDefault();
    }

    #endregion

    #region Create First Order

    public static string CreateFirstOrder(this Customer customer) =>
        throw new NotImplementedException();

    public static string? DisableCreateFirstOrder(this Customer customer) =>
        customer.SalesTerritoryID == 6 ? "Customers in Canada may not place orders directly." : null;

    #endregion
}