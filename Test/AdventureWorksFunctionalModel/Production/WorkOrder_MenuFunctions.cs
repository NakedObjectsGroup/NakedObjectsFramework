






using System;
using System.Linq;
using AW.Types;
using NakedFunctions;
using static AW.Helpers;

namespace AW.Functions {
    [Named("Work Orders")]
    public static class WorkOrder_MenuFunctions {
        public static WorkOrder RandomWorkOrder(IContext context) =>
            Random<WorkOrder>(context);

        [CreateNew]
        public static (WorkOrder, IContext context) CreateNewWorkOrder(
            [DescribedAs("product partial name")] Product product,
            int orderQty,
            DateTime startDate,
            IContext context) {
            var wo = new WorkOrder {
                ProductID = product.ProductID,
                OrderQty = orderQty,
                ScrappedQty = 0,
                StartDate = startDate,
                DueDate = startDate.AddDays(7),
                ModifiedDate = context.Now()
            };
            return (wo, context.WithNew(wo));
        }

        [PageSize(20)]
        public static IQueryable<Product> AutoComplete0CreateNewWorkOrder(
            [MinLength(2)] string name, IContext context) =>
            Product_MenuFunctions.FindProductByName(name, context);

        public static IQueryable<Location> AllLocations(IContext context) => context.Instances<Location>();

        #region ListWorkOrders

        [TableView(true, "Product", "OrderQty", "StartDate")]
        public static IQueryable<WorkOrder> ListWorkOrders(
            Product product, [DefaultValue(true)] bool currentOrdersOnly, IContext context) =>
            context.Instances<WorkOrder>().Where(x => x.Product.ProductID == product.ProductID &&
                                                      (currentOrdersOnly == false || x.EndDate == null));

        [PageSize(20)]
        public static IQueryable<Product> AutoComplete0ListWorkOrders(
            [MinLength(2)] string name, IContext context) =>
            Product_MenuFunctions.FindProductByName(name, context);

        #endregion
    }
}