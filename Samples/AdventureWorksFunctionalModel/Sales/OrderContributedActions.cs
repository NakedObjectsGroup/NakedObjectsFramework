// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel;
using System.Linq;
using AdventureWorksModel.Sales;
using NakedFunctions;
using NakedObjects;
using static AdventureWorksModel.CommonFactoryAndRepositoryFunctions;

namespace AdventureWorksModel {
    [DisplayName("Orders")]
    public static class OrderContributedActions {
        private const string subMenu = "Orders";

        [MemberOrder(22)]
        [TableView(true, "OrderDate", "Status", "TotalDue")]
        public static IQueryable<SalesOrderHeader> RecentOrders(
            [ContributedAction(subMenu)] Customer customer,
             [Injected] IQueryable<SalesOrderHeader> headers) {
            return from obj in headers
                where obj.Customer.CustomerID == customer.CustomerID
                orderby obj.SalesOrderNumber descending
                select obj;
        }

        [MemberOrder(20)]
        public static (SalesOrderHeader, string) LastOrder(
            [ContributedAction(subMenu)] Customer customer,
            [Injected] IQueryable<SalesOrderHeader> headers) {

            return SingleObjectWarnIfNoMatch(
                headers.Where(x => x.Customer.CustomerID == customer.CustomerID).OrderByDescending(x => x.SalesOrderNumber));
        }

        [MemberOrder(21)]
        [TableView(true, "OrderDate", "TotalDue")]
        public static IQueryable<SalesOrderHeader> OpenOrders(
            [ContributedAction(subMenu)] Customer customer,
            [Injected] IQueryable<SalesOrderHeader> headers)
        {
            var id = customer.CustomerID;
            return headers.Where(x => x.Customer.CustomerID == id && x.Status <= 3).OrderByDescending(x => x.SalesOrderNumber);
        }

        
        [FinderAction]
        [TableView(true, "CurrencyRateDate", "AverageRate", "EndOfDayRate")]
        public static CurrencyRate FindRate(
            string currency, 
            string currency1,
            [Injected] IQueryable<CurrencyRate> rates) {
            return rates.FirstOrDefault(cr => cr.Currency.Name == currency && cr.Currency1.Name == currency1);
        }

        public static string Default0FindRate() {
            return "US Dollar";
        }

        public static string Default1FindRate() {
            return "Euro";
        }

        #region Comments

        public static void AppendComment(
            string commentToAppend, 
            [ContributedAction(subMenu)] IQueryable<SalesOrderHeader> toOrders) {
            foreach (SalesOrderHeader order in toOrders) {
                AppendComment(commentToAppend, order);
            }
        }

        public static string ValidateAppendComment(string commentToAppend, IQueryable<SalesOrderHeader> toOrders) {
            if (commentToAppend == "fail") {
                return "For test purposes the comment 'fail' fails validation";
            }

            return string.IsNullOrEmpty(commentToAppend) ? "Comment required" : null;
        }

        public static void AppendComment(string commentToAppend, [ContributedAction(subMenu)] SalesOrderHeader order) {
            if (order.Comment == null) {
                order.Comment = commentToAppend;
            }
            else {
                order.Comment += "; " + commentToAppend;
            }
        }

        public static string ValidateAppendComment(string commentToAppend, SalesOrderHeader order) {
            return string.IsNullOrEmpty(commentToAppend) ? "Comment required" : null;
        }

        public static void CommentAsUsersUnhappy([ContributedAction(subMenu)] IQueryable<SalesOrderHeader> toOrders) {
            AppendComment("User unhappy", toOrders);
        }

        public static string ValidateCommentAsUsersUnhappy(IQueryable<SalesOrderHeader> toOrders) {
            return toOrders.Any(o => !o.IsShipped()) ? "Not all shipped yet" : null;
        }

        public static void CommentAsUserUnhappy([ContributedAction(subMenu)] SalesOrderHeader order) {
            AppendComment("User unhappy", order);
        }

        public static string ValidateCommentAsUserUnhappy(SalesOrderHeader order) {
            return order.IsShipped() ? null : "Not shipped yet";
        }

        public static void ClearComments([ContributedAction(subMenu)]IQueryable<SalesOrderHeader> toOrders) {
            foreach (SalesOrderHeader order in toOrders) {
                order.Comment = null;
            }
        }

        #endregion

        #region SearchForOrders

        [MemberOrder(12), PageSize(10)]
        [TableView(true, "OrderDate", "Status", "TotalDue")]
        public static IQueryable<SalesOrderHeader> SearchForOrders(
            [ContributedAction(subMenu)] Customer customer,
            [Optionally] [Mask("d")] DateTime? fromDate,
            [Optionally] [Mask("d")] DateTime? toDate,
            [Injected] IQueryable<SalesOrderHeader> query) {

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
        public static SalesOrderHeader CreateNewOrder([ContributedAction(subMenu)] Customer customer,
                                               [Optionally] bool copyHeaderFromLastOrder,
                                               [Injected] IQueryable<BusinessEntityAddress> addresses,
                                               [Injected] IQueryable<SalesOrderHeader> headers)
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
        public static QuickOrderForm QuickOrder([ContributedAction(subMenu)] Customer customer) {
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