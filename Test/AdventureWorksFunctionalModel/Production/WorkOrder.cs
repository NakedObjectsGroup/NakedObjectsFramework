namespace AW.Types;

public class WorkOrder : IHasModifiedDate
{

    public WorkOrder() { }

    public WorkOrder(WorkOrder cloneFrom)
    {
        WorkOrderID = cloneFrom.WorkOrderID;
        StockedQty = cloneFrom.StockedQty;
        ScrappedQty = cloneFrom.ScrappedQty;
        EndDate = cloneFrom.EndDate;
        ScrapReasonID = cloneFrom.ScrapReasonID;
        ScrapReason = cloneFrom.ScrapReason;
        OrderQty = cloneFrom.OrderQty;
        StartDate = cloneFrom.StartDate;
        DueDate = cloneFrom.DueDate;
        ProductID = cloneFrom.ProductID;
        Product = cloneFrom.Product;
        WorkOrderRoutings = cloneFrom.WorkOrderRoutings;
        ModifiedDate = cloneFrom.ModifiedDate;
    }

    [Hidden]
    public int WorkOrderID { get; init; }

    [MemberOrder(22)]
    public int StockedQty { get; init; }

    [MemberOrder(24)]
    public short ScrappedQty { get; init; }

    [MemberOrder(32), Mask("d")]
    public DateTime? EndDate { get; init; }

    [Hidden]
    public short? ScrapReasonID { get; init; }

    [MemberOrder(26)]
    public virtual ScrapReason? ScrapReason { get; init; }

    [MemberOrder(20)]
    public int OrderQty { get; init; }

    [MemberOrder(30), Mask("d")]
    public DateTime StartDate { get; init; }

    [MemberOrder(34), Mask("d")]
    public DateTime DueDate { get; init; }

    [Hidden]
    public int ProductID { get; init; }

    [MemberOrder(10)]
    public virtual Product Product { get; init; }

    [RenderEagerly, TableView(true, "OperationSequence", "ScheduledStartDate", "ScheduledEndDate", "Location", "PlannedCost")]
    public virtual ICollection<WorkOrderRouting> WorkOrderRoutings { get; init; } = new List<WorkOrderRouting>();

    [Hidden]
    public string AnAlwaysHiddenReadOnlyProperty => "";

    [MemberOrder(99), Versioned]
    public DateTime ModifiedDate { get; init; }

    public override string ToString() => $"{Product}: {StartDate}";
}