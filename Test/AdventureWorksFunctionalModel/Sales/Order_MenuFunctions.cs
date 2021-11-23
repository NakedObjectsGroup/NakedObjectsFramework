






using System;
using System.Linq;
using AW.Types;
using NakedFunctions;
using static AW.Helpers;

namespace AW.Types {
    public enum Ordering {
        Ascending,
        Descending
    }
}

namespace AW.Functions {
    [Named("Orders")]
    public static class Order_MenuFunctions {
        [MemberOrder(99)]
        public static SalesOrderHeader RandomOrder(IContext context) => Random<SalesOrderHeader>(context);

        [MemberOrder(5)]
        [TableView(true, "OrderDate", "DueDate")]
        public static IQueryable<SalesOrderHeader> OrdersInProcess(IContext context) =>
            context.Instances<SalesOrderHeader>().Where(x => x.StatusByte == 1);

        [MemberOrder(10)]
        public static SalesOrderHeader? FindOrder([DefaultValue("SO")] string orderNumber, IContext context) =>
            context.Instances<SalesOrderHeader>().FirstOrDefault(x => x.SalesOrderNumber == orderNumber);

        [MemberOrder(90)]
        [TableView(true, "TotalDue", "Customer", "OrderDate", "SalesPerson", "Comment")]
        public static IQueryable<SalesOrderHeader> HighestValueOrders(IContext context) =>
            context.Instances<SalesOrderHeader>().OrderByDescending(obj => obj.TotalDue);

        [MemberOrder(91)]
        [TableView(true, "OrderDate", "Customer")]
        public static IQueryable<SalesOrderHeader> OrdersByStatus(IContext context, OrderStatus status) =>
            context.Instances<SalesOrderHeader>().Where(x => x.StatusByte == (byte)status).OrderByDescending(obj => obj.OrderDate);

        [TableView(true, "OrderDate", "Details")]
        public static IQueryable<SalesOrderHeader> OrdersWithMostLines(
            IQueryable<SalesOrderHeader> headers) {
            return headers.OrderByDescending(obj => obj.Details.Count);
        }

        public static IQueryable<SalesOrderHeader> FindOrders(
            [DescribedAs("Enter the Account Number (AW + 8 digits) & select the customer"), Optionally] Customer? customer,
            [Optionally] DateTime? orderDate, IContext context) =>
            customer == null
                ? ByDate(context.Instances<SalesOrderHeader>(), orderDate)
                : ByDate(OrdersForCustomer(customer, context), orderDate);

        private static IQueryable<SalesOrderHeader> ByDate(
            IQueryable<SalesOrderHeader> orders, DateTime? d) =>
            d == null
                ? orders
                : orders.Where(soh => soh.OrderDate == d);

        [PageSize(10)]
        public static Customer? AutoComplete0FindOrders(
            [MinLength(10)] string accountNumber,
            IContext context) =>
            Customer_MenuFunctions.FindCustomerByAccountNumber(accountNumber, context);

        #region OrdersForCustomer

        //Action to demonstrate use of Auto-Complete that returns a single object
        public static IQueryable<SalesOrderHeader> OrdersForCustomer(
            [DescribedAs("Enter the Account Number (AW + 8 digits) & select the customer")]
            Customer customer,
            IContext context) =>
            customer.RecentOrders(context);

        [PageSize(10)]
        public static Customer? AutoComplete0OrdersForCustomer(
            [MinLength(10)] string accountNumber,
            IContext context) =>
            Customer_MenuFunctions.FindCustomerByAccountNumber(accountNumber, context);

        #endregion
    }
}