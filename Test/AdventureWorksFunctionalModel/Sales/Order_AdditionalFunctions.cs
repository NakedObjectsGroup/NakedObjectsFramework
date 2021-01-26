// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
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
            this Customer customer, IContext context) {
            return from obj in context.Instances<SalesOrderHeader>()
                where obj.Customer.CustomerID == customer.CustomerID
                orderby obj.SalesOrderNumber descending
                select obj;
        }

        [MemberOrder(20)]
        public static (SalesOrderHeader, IContext) LastOrder(this Customer customer, IContext context) {
            var order = context.Instances<SalesOrderHeader>().Where(x => x.Customer.CustomerID == customer.CustomerID).OrderByDescending(x => x.SalesOrderNumber).FirstOrDefault();
            return (order, context.WithInformUser("No Previous Orders"));
        }

        [MemberOrder(21)]
        [TableView(true, "OrderDate", "TotalDue")]
        public static IQueryable<SalesOrderHeader> OpenOrders(
            this Customer customer,
            IQueryable<SalesOrderHeader> headers)
        {
            var id = customer.CustomerID;
            return headers.Where(x => x.Customer.CustomerID == id && x.StatusByte <= 3).OrderByDescending(x => x.SalesOrderNumber);
        }


        #region Comments

        public static void AppendComment(this IQueryable<SalesOrderHeader> toOrders, string commentToAppend, IContext context) {
            foreach (SalesOrderHeader order in toOrders) {
                AppendComment(order, commentToAppend, context);
            }
        }

        // temp comment out
        //public static string Validate1AppendComment(this IQueryable<SalesOrderHeader> toOrder, string commentToAppend) {
        //    if (commentToAppend == "fail") {
        //        return "For test purposes the comment 'fail' fails validation";
        //    }

        //    return string.IsNullOrEmpty(commentToAppend) ? "Comment required" : null;
        //}

        public static (SalesOrderHeader, IContext) AppendComment(this SalesOrderHeader order, string commentToAppend, IContext context) {
            string newComments = order.Comment == null? commentToAppend: order.Comment + "; " + commentToAppend;
            return context.SaveAndDisplay(order with {Comment = newComments });
        }

        public static string Validate1AppendComment(this SalesOrderHeader order, string commentToAppend) {
            return string.IsNullOrEmpty(commentToAppend) ? "Comment required" : null;
        }

        public static void CommentAsUsersUnhappy(this IQueryable<SalesOrderHeader> toOrders, IContext context) => 
            AppendComment(toOrders, "User unhappy", context);
        

        public static string DisableCommentAsUsersUnhappy(this IQueryable<SalesOrderHeader> toOrders) {
            return toOrders.Any(o => !o.IsShipped()) ? "Not all shipped yet" : null;
        }

        public static void CommentAsUserUnhappy(this SalesOrderHeader order, IContext context) {
            AppendComment(order, "User unhappy", context);
        }

        public static string DisableCommentAsUserUnhappy(this SalesOrderHeader order) {
            return order.IsShipped() ? null : "Not shipped yet";
        }

        public static void ClearComments(this IQueryable<SalesOrderHeader> toOrders) {
            foreach (SalesOrderHeader order in toOrders) {
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

        #region CreateNewOrder

        //Automatically copies common header info from previous order
        //Disabled if the customer has no previous orders
        [MemberOrder(1)]
        public static (SalesOrderHeader, IContext) CreateAnotherOrder(
            this Customer customer, IContext context)
        {
             SalesOrderHeader last = GetLastOrder(customer, context);
            var newOrder = new SalesOrderHeader() 
            {
                RevisionNumber = (byte)1,
                OrderDate = context.Today(),
                DueDate = context.Today().AddDays(7),
                StatusByte = (byte)OrderStatus.InProcess,
                OnlineOrder = false,
                CustomerID = last.CustomerID,
                BillingAddressID = last.BillingAddressID,
                ShippingAddressID = last.ShippingAddressID,
                ShipMethodID = last.ShipMethodID,
                CreditCardID = last.CreditCardID,
                AccountNumber = last.AccountNumber,
                rowguid = context.NewGuid(),
                ModifiedDate = context.Now()
            };
            return (newOrder, context.WithPendingSave(newOrder));
        }

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

        [MemberOrder(1)]
        public static QuickOrderForm QuickOrder(this Customer customer) =>
            new QuickOrderForm { Customer = customer };
            
        #endregion
    }
}