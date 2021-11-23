namespace AW.Functions;
public static class PurchaseOrderDetail_Functions
{

    [MemberOrder(1)]
    public static IContext ReceiveGoods(
        this PurchaseOrderDetail pod, int qtyReceived, int qtyRejected, int qtyIntoStock, IContext context) =>
        context.WithUpdated(pod, new(pod)
        {
            ReceivedQty = qtyReceived,
            RejectedQty = qtyRejected,
            StockedQty = qtyIntoStock,
            ModifiedDate = context.Now()
        });

    public static int Default1ReceiveGoods(this PurchaseOrderDetail pod) => pod.OrderQty;

    public static int Default2ReceiveGoods(this PurchaseOrderDetail pod) => pod.OrderQty;

    public static string? ValidateReceiveGoods(this PurchaseOrderDetail pod, int qtyReceived, int qtyRejected, int qtyIntoStock) =>
        qtyRejected + qtyIntoStock != qtyReceived ? "Qty Into Stock + Qty Rejected must add up to Qty Received" : null;
}