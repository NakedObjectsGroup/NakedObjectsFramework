// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using AW.Types;
using NakedFunctions;


namespace AW.Functions {
    [Named("Orders")]
    public static class Order_AdditionalFunctions {
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

        public static (IList<SalesOrderHeader>, IContext) AppendCommentToOrders(this IQueryable<SalesOrderHeader> toOrders, string comment, IContext context)
        {
            var updates = toOrders.Select(x => new { original = x, updated = x.WithAppendedComment(comment, context) });
            var context2 = updates.Aggregate(context, (c, of) => c.WithUpdated(of.original, of.updated));
            return (updates.Select(x => x.updated).ToList(), context2);
        }

        //public static string ValidateAppendComment(this IQueryable<SalesOrderHeader> toOrder, string commentToAppend) =>
        //       toOrder.Count() > 5 ? "You may not apply the same comment to more than 5 orders at one time." : null;

        public static IContext AppendComment(
            this SalesOrderHeader order, string commentToAppend, IContext context)
        {
            SalesOrderHeader updated = WithAppendedComment(order, commentToAppend, context);
            return context.WithUpdated(order, updated);
        }

        internal static SalesOrderHeader WithAppendedComment(this SalesOrderHeader order, string commentToAppend, IContext context)
        {
            string newComments = order.Comment == null ? commentToAppend : order.Comment + "; " + commentToAppend;
           return order with { Comment = newComments, ModifiedDate = context.Now() };
        }

        public static string Validate1AppendComment(this SalesOrderHeader order, string commentToAppend) {
            return string.IsNullOrEmpty(commentToAppend) ? "Comment required" : null;
        }

        public static void CommentAsUsersUnhappy(this IQueryable<SalesOrderHeader> toOrders, IContext context) =>
            AppendCommentToOrders(toOrders, "User unhappy", context);


        public static void CommentAsUserUnhappy(this SalesOrderHeader order, IContext context) {
            AppendComment(order, "User unhappy", context);
        }

        public static string DisableCommentAsUserUnhappy(this SalesOrderHeader order) {
            return order.IsShipped() ? null : "Not shipped yet";
        }

        public static void ClearComments(this IQueryable<SalesOrderHeader> toOrders)
        {
            foreach (SalesOrderHeader order in toOrders)
            {
                throw new NotImplementedException();
                //TODO: 
                //order.Comment = null;
            }
        }

        #endregion

        #region SearchForOrders

        [MemberOrder(12), PageSize(10)]
        [TableView(true, "OrderDate", "Status", "TotalDue")]
        public static IQueryable<SalesOrderHeader> SearchForOrders(
            this Customer customer,
            [Optionally] [Mask("d")] DateTime? fromDate,
            [Optionally] [Mask("d")] DateTime? toDate,
            IContext context) {

            int customerID = customer.CustomerID;

            var headers =  from obj in context.Instances<SalesOrderHeader>()
                   where ((fromDate == null) || obj.OrderDate >= fromDate) &&
                         ((toDate == null) || obj.OrderDate <= toDate)
                   orderby obj.OrderDate
                   select obj;

            return customer == null ?
                 headers
                 : headers.Where(x => x.Customer.CustomerID == customerID);
        }

        public static string ValidateSearchForOrders(this Customer customer, DateTime? fromDate, DateTime? toDate) {
            if (fromDate.HasValue && toDate.HasValue) {
                if (fromDate >= toDate) {
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
            SalesOrderHeader order = NewOrderFrom(context, GetLastOrder(customer, context));
            SalesOrderDetail detail = order.CreateNewDetail(product, quantity, context);
            return (order, context.WithNew(order).WithNew(detail).WithDeferred(
                    c => { var o = c.Reload(order); return c.WithUpdated(o, o.Recalculated(c));
                    }));
        }

        [PageSize(20)]
        public static IQueryable<Product> AutoComplete1QuickOrder(this SalesOrderHeader soh,
               [MinLength(2)] string name, IContext context) =>
                    Product_MenuFunctions.FindProductByName(name, context);

        public static string DisableQuickOrder(this Customer customer, IContext context) =>
            customer.DisableCreateAnotherOrder(context);

        #endregion 

        #region CreateAnotherOrder

        //Automatically copies common header info from previous order
        //Disabled if the customer has no previous orders
        [MemberOrder(1)]
        public static (SalesOrderHeader, IContext) CreateAnotherOrder(
            this Customer customer, IContext context)
        {
            SalesOrderHeader newOrder = NewOrderFrom(context, GetLastOrder(customer, context));
            return (newOrder, context.WithNew(newOrder));
        }

        private static SalesOrderHeader NewOrderFrom(IContext context, SalesOrderHeader previous) =>
            new SalesOrderHeader()
            {
                RevisionNumber = (byte)1,
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


        public static string DisableCreateAnotherOrder(this Customer customer, IContext context) =>
          GetLastOrder(customer, context) is null ?
                "Customer has no previous orders. Use Create First Order.":
                null;

        public static SalesOrderHeader GetLastOrder(this Customer c, IContext context) {
            int cid = c.CustomerID;
            return context.Instances<SalesOrderHeader>().Where(o => o.CustomerID == cid).
                OrderByDescending(o => o.OrderDate).FirstOrDefault();
            }
        #endregion

        #region Create First Order
        public static string CreateFirstOrder(this Customer customer) =>
            throw new NotImplementedException();

        public static string DisableCreateFirstOrder(this Customer customer) =>
            customer.SalesTerritoryID == 6 ? "Customers in Canada may not place orders directly." : null;

        #endregion
    }
}