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
using static AW.Helpers;

namespace AW.Functions {
    [Named("Orders")]
    public static class Order_AdditionalFunctions {
        private const string subMenu = "Orders";

        [MemberOrder(22)]
        [TableView(true, "OrderDate", "Status", "TotalDue")]
        public static IQueryable<SalesOrderHeader> RecentOrders(
            this Customer customer,
             IQueryable<SalesOrderHeader> headers) {
            return from obj in headers
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

        public static string ValidateAppendComment(string commentToAppend, IQueryable<SalesOrderHeader> toOrders) {
            if (commentToAppend == "fail") {
                return "For test purposes the comment 'fail' fails validation";
            }

            return string.IsNullOrEmpty(commentToAppend) ? "Comment required" : null;
        }

        public static (SalesOrderHeader, IContext) AppendComment(this SalesOrderHeader order, string commentToAppend, IContext context) {
            string newComments = order.Comment == null? commentToAppend: order.Comment + "; " + commentToAppend;
            return DisplayAndSave(order with {Comment = newComments }, context);
        }

        public static string ValidateAppendComment(string commentToAppend, SalesOrderHeader order) {
            return string.IsNullOrEmpty(commentToAppend) ? "Comment required" : null;
        }

        public static void CommentAsUsersUnhappy(this IQueryable<SalesOrderHeader> toOrders, IContext context) => 
            AppendComment(toOrders, "User unhappy", context);
        

        public static string ValidateCommentAsUsersUnhappy(IQueryable<SalesOrderHeader> toOrders) {
            return toOrders.Any(o => !o.IsShipped()) ? "Not all shipped yet" : null;
        }

        public static void CommentAsUserUnhappy(this SalesOrderHeader order, IContext context) {
            AppendComment(order, "User unhappy", context);
        }

        public static string ValidateCommentAsUserUnhappy(SalesOrderHeader order) {
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
            IQueryable<SalesOrderHeader> query) {

            int customerID = customer.CustomerID;

            var headers =  from obj in query
                   where ((fromDate == null) || obj.OrderDate >= fromDate) &&
                         ((toDate == null) || obj.OrderDate <= toDate)
                   orderby obj.OrderDate
                   select obj;

            return customer == null ?
                 headers
                 : headers.Where(x => x.Customer.CustomerID == customerID);
        }

        public static string ValidateSearchForOrders(Customer customer, DateTime? fromDate, DateTime? toDate) {
            if (fromDate.HasValue && toDate.HasValue) {
                if (fromDate >= toDate) {
                    return "'From Date' must be before 'To Date'";
                }
            }
            return "";
        }

        #endregion

        #region CreateNewOrder

        [MemberOrder(1)]
        public static (SalesOrderHeader, IContext) CreateNewOrder(
            this Customer customer, [Optionally] bool copyHeaderFromLastOrder, IContext context)
        {

            throw new NotImplementedException(); //TODO
            //            SalesOrderHeader newOrder = null;//Container.NewTransientInstance<SalesOrderHeader>();
            //newOrder.Customer = customer;

            //if (copyHeaderFromLastOrder) {
            //    SalesOrderHeader last = LastOrder(customer, headers);
            //    if (last != null) {
            //        newOrder.BillingAddress = last.BillingAddress;
            //        newOrder.ShippingAddress = last.ShippingAddress;
            //        newOrder.CreditCard = last.CreditCard;
            //        newOrder.ShipMethod = last.ShipMethod;
            //        newOrder.AccountNumber = last.AccountNumber;
            //    }
            //}
            //else {
            //    newOrder.BillingAddress = PersonRepository.AddressesFor(customer.BusinessEntity(), addresses,  "Billing").FirstOrDefault();
            //    newOrder.ShippingAddress = PersonRepository.AddressesFor(customer.BusinessEntity(), addresses, "Shipping").FirstOrDefault();
            //}
            //return newOrder;
        }

        public static string ValidateCreateNewOrder(this Customer customer) =>
            customer.SalesTerritoryID == 6 ? "Customers in Canada may not place orders directly." : null;

        [MemberOrder(1)]
        public static QuickOrderForm QuickOrder(this Customer customer) {
            throw new NotImplementedException();
            //var qo = Container.NewViewModel<QuickOrderForm>();
            //qo.Customer = customer;
            //return qo;
        }

        public static bool Default1CreateNewOrder() {
            return true;
        }

        #endregion
    }
}