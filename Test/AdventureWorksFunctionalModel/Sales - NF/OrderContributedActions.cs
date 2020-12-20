// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using AdventureWorksModel.Sales;
using NakedFunctions;
using static AdventureWorksModel.Helpers;

namespace AdventureWorksModel {
    [Named("Orders")]
    public static class OrderContributedActions {
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
        public static (SalesOrderHeader, IContainer) LastOrder(this Customer customer, IContainer container) =>
            Helpers.SingleObjectWarnIfNoMatch(container.Instances<SalesOrderHeader>().Where(x => x.Customer.CustomerID == customer.CustomerID).OrderByDescending(x => x.SalesOrderNumber), container);
        

        [MemberOrder(21)]
        [TableView(true, "OrderDate", "TotalDue")]
        public static IQueryable<SalesOrderHeader> OpenOrders(
            this Customer customer,
            IQueryable<SalesOrderHeader> headers)
        {
            var id = customer.CustomerID;
            return headers.Where(x => x.Customer.CustomerID == id && x.Status <= 3).OrderByDescending(x => x.SalesOrderNumber);
        }

        
        
        [TableView(true, "CurrencyRateDate", "AverageRate", "EndOfDayRate")]
        public static CurrencyRate FindRate(
            string currency, 
            string currency1,
            IQueryable<CurrencyRate> rates) {
            return rates.FirstOrDefault(cr => cr.Currency.Name == currency && cr.Currency1.Name == currency1);
        }

        public static string Default0FindRate() {
            return "US Dollar";
        }

        public static string Default1FindRate() {
            return "Euro";
        }

        #region Comments

        public static void AppendComment(this IQueryable<SalesOrderHeader> toOrders, string commentToAppend, IContainer container) {
            foreach (SalesOrderHeader order in toOrders) {
                AppendComment(order, commentToAppend, container);
            }
        }

        public static string ValidateAppendComment(string commentToAppend, IQueryable<SalesOrderHeader> toOrders) {
            if (commentToAppend == "fail") {
                return "For test purposes the comment 'fail' fails validation";
            }

            return string.IsNullOrEmpty(commentToAppend) ? "Comment required" : null;
        }

        public static (SalesOrderHeader, IContainer) AppendComment(this SalesOrderHeader order, string commentToAppend, IContainer container) {
            string newComments = order.Comment == null? commentToAppend: order.Comment + "; " + commentToAppend;
            return DisplayAndSave(order with {Comment = newComments }, container);
        }

        public static string ValidateAppendComment(string commentToAppend, SalesOrderHeader order) {
            return string.IsNullOrEmpty(commentToAppend) ? "Comment required" : null;
        }

        public static void CommentAsUsersUnhappy(this IQueryable<SalesOrderHeader> toOrders, IContainer container) => 
            AppendComment(toOrders, "User unhappy", container);
        

        public static string ValidateCommentAsUsersUnhappy(IQueryable<SalesOrderHeader> toOrders) {
            return toOrders.Any(o => !o.IsShipped()) ? "Not all shipped yet" : null;
        }

        public static void CommentAsUserUnhappy(this SalesOrderHeader order, IContainer container) {
            AppendComment(order, "User unhappy", container);
        }

        public static string ValidateCommentAsUserUnhappy(SalesOrderHeader order) {
            return order.IsShipped() ? null : "Not shipped yet";
        }

        public static void ClearComments(this IQueryable<SalesOrderHeader> toOrders) {
            foreach (SalesOrderHeader order in toOrders) {
                order.Comment = null;
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

        public static DateTime Default1SearchForOrders() {
            return new DateTime(2000, 1, 1);
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
        public static SalesOrderHeader CreateNewOrder(this Customer customer,
                                               [Optionally] bool copyHeaderFromLastOrder,
                                               IQueryable<BusinessEntityAddress> addresses,
                                               IQueryable<SalesOrderHeader> headers)
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

        public static string ValidateCreateNewOrder(Customer customer)
        {
            if (customer.SalesTerritoryID == 6 )
                return "Customers in Canada may not place orders directly.";
            return null;
        }

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