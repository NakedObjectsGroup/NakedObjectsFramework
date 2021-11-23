namespace AW.Functions;
public static class WorkOrder_Functions
{

    [Edit]
    public static IContext ChangeScrappedQuantity(this WorkOrder wo, short scrappedQty, IContext context)
        => context.WithUpdated(wo, new(wo)
        {
            ScrappedQty = scrappedQty,
            ModifiedDate = context.Now()
        });

    [Edit]
    public static IContext EditDates(this WorkOrder wo,
                                     [DefaultValue(0)] DateTime startDate, [DefaultValue(7)] DateTime dueDate, IContext context) =>
        context.WithUpdated(wo, new(wo)
        {
            StartDate = startDate,
            DueDate = dueDate,
            ModifiedDate = context.Now()
        });

    public static string? ValidateEditDates(this WorkOrder wo, DateTime startDate, DateTime dueDate) =>
        startDate > dueDate ? "StartDate must be before DueDate" : null;

    [Edit]
    public static IContext EditOrderQty(
        this WorkOrder wo, int orderQty, IContext context) =>
        context.WithUpdated(wo, new(wo)
        {
            OrderQty = orderQty,
            ModifiedDate = context.Now()
        });

    public static string? Validate1EditOrderQty(this WorkOrder wo, int orderQty) =>
        orderQty <= 0 ? "Order Quantity must be > 0" : null;

    [PageSize(20)]
    public static IQueryable<Product> AutoCompleteProduct(
        [MinLength(2)] string name, IContext context) =>
        Product_MenuFunctions.FindProductByName(name, context);

    [MemberOrder(1)]
    public static (WorkOrderRouting, IContext) AddNewRouting(WorkOrder wo, Location loc, IContext context)
    {
        var highestSequence = wo.WorkOrderRoutings.Count > 0 ? wo.WorkOrderRoutings.Max(n => n.OperationSequence) + 1 : 1;
        var wor = new WorkOrderRouting
        {
            WorkOrder = wo,
            Location = loc,
            OperationSequence = (short)highestSequence
        };
        return (wor, context.WithNew(wor));
    }
}