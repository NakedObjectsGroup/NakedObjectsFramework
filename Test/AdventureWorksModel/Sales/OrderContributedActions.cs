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
using NakedObjects;
using NakedObjects.Services;

namespace AdventureWorksModel {
    [DisplayName("Orders")]
    public class OrderContributedActions : AbstractFactoryAndRepository {
        private const string subMenu = "Orders";

        #region Injected Services
        public PersonRepository PersonRepository { set; protected get; }
        #endregion

        #region RecentOrders

        [MemberOrder(22)]
        [TableView(true, "OrderDate", "Status", "TotalDue")]
        public IQueryable<SalesOrderHeader> RecentOrders([ContributedAction(subMenu)] Customer customer) {
            return from obj in Instances<SalesOrderHeader>()
                where obj.Customer.CustomerID == customer.CustomerID
                orderby obj.SalesOrderNumber descending
                select obj;
        }
        #endregion

        #region LastOrder
        [MemberOrder(20), QueryOnly]
        public SalesOrderHeader LastOrder([ContributedAction(subMenu)] Customer customer) {
            var query = from obj in Container.Instances<SalesOrderHeader>()
                where obj.Customer.CustomerID == customer.CustomerID
                orderby obj.SalesOrderNumber descending
                select obj;

            return SingleObjectWarnIfNoMatch(query);
        }

       #endregion

        #region OpenOrders
        [MemberOrder(21)]
        [TableView(true, "OrderDate", "TotalDue")]
        public IQueryable<SalesOrderHeader> OpenOrders([ContributedAction(subMenu)] Customer customer) {
            return from obj in Container.Instances<SalesOrderHeader>()
                where obj.Customer.CustomerID == customer.CustomerID &&
                      obj.Status <= 3
                orderby obj.SalesOrderNumber descending
                select obj;
        }
        #endregion

        [QueryOnly]
        [FinderAction]
        [TableView(true, "CurrencyRateDate", "AverageRate", "EndOfDayRate")]
        public CurrencyRate FindRate(string currency, string currency1) {
            return Container.Instances<CurrencyRate>().FirstOrDefault(cr => cr.Currency.Name == currency && cr.Currency1.Name == currency1);
        }

        public string Default0FindRate() {
            return "US Dollar";
        }

        public string Default1FindRate() {
            return "Euro";
        }

        #region Comments

        public void AppendComment(string commentToAppend, [ContributedAction(subMenu)] IQueryable<SalesOrderHeader> toOrders) {
            foreach (SalesOrderHeader order in toOrders) {
                AppendComment(commentToAppend, order);
            }
        }

        public string ValidateAppendComment(string commentToAppend, IQueryable<SalesOrderHeader> toOrders) {
            if (commentToAppend == "fail") {
                return "For test purposes the comment 'fail' fails validation";
            }

            return string.IsNullOrEmpty(commentToAppend) ? "Comment required" : null;
        }

        public void AppendComment(string commentToAppend, [ContributedAction(subMenu)] SalesOrderHeader order) {
            if (order.Comment == null) {
                order.Comment = commentToAppend;
            }
            else {
                order.Comment += "; " + commentToAppend;
            }
        }

        public string ValidateAppendComment(string commentToAppend, SalesOrderHeader order) {
            return string.IsNullOrEmpty(commentToAppend) ? "Comment required" : null;
        }

        public void CommentAsUsersUnhappy([ContributedAction(subMenu)] IQueryable<SalesOrderHeader> toOrders) {
            AppendComment("User unhappy", toOrders);
        }

        public string ValidateCommentAsUsersUnhappy(IQueryable<SalesOrderHeader> toOrders) {
            return toOrders.Any(o => !o.IsShipped()) ? "Not all shipped yet" : null;
        }

        public void CommentAsUserUnhappy([ContributedAction(subMenu)] SalesOrderHeader order) {
            AppendComment("User unhappy", order);
        }

        public string ValidateCommentAsUserUnhappy(SalesOrderHeader order) {
            return order.IsShipped() ? null : "Not shipped yet";
        }

        public void ClearComments([ContributedAction(subMenu)]IQueryable<SalesOrderHeader> toOrders) {
            foreach (SalesOrderHeader order in toOrders) {
                order.Comment = null;
            }
        }

        #endregion

        #region SearchForOrders

        [MemberOrder(12), PageSize(10)]
        [TableView(true, "OrderDate", "Status", "TotalDue")]
        public IQueryable<SalesOrderHeader> SearchForOrders(
            [ContributedAction(subMenu)] Customer customer,
            [Optionally] [Mask("d")] DateTime? fromDate,
            [Optionally] [Mask("d")] DateTime? toDate) {
            IQueryable<SalesOrderHeader> query = Instances<SalesOrderHeader>();

            if (customer != null) {
                query = from obj in query
                    where obj.Customer.CustomerID == customer.CustomerID
                    select obj;
            }

            return from obj in query
                where ((fromDate == null) || obj.OrderDate >= fromDate) &&
                      ((toDate == null) || obj.OrderDate <= toDate)
                orderby obj.OrderDate
                select obj;
        }

        public DateTime Default1SearchForOrders() {
            return new DateTime(2000, 1, 1);
        }

        public string ValidateSearchForOrders(Customer customer, DateTime? fromDate, DateTime? toDate) {
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
        public SalesOrderHeader CreateNewOrder([ContributedAction(subMenu)] Customer customer,
                                               [Optionally] bool copyHeaderFromLastOrder) {
            var newOrder = Container.NewTransientInstance<SalesOrderHeader>();
            newOrder.Customer = customer;

            if (copyHeaderFromLastOrder) {
                SalesOrderHeader last = LastOrder(customer);
                if (last != null) {
                    newOrder.BillingAddress = last.BillingAddress;
                    newOrder.ShippingAddress = last.ShippingAddress;
                    newOrder.CreditCard = last.CreditCard;
                    newOrder.ShipMethod = last.ShipMethod;
                    newOrder.AccountNumber = last.AccountNumber;
                }
            }
            else {
                newOrder.BillingAddress = PersonRepository.AddressesFor(customer.BusinessEntity(), "Billing").FirstOrDefault();
                newOrder.ShippingAddress = PersonRepository.AddressesFor(customer.BusinessEntity(), "Shipping").FirstOrDefault();
            }
            return newOrder;
        }

        public string ValidateCreateNewOrder(Customer customer)
        {
            if (customer.SalesTerritoryID == 6 )
                return "Customers in Canada may not place orders directly.";
            return null;
        }

        [MemberOrder(1)]
        public QuickOrderForm QuickOrder([ContributedAction(subMenu)] Customer customer) {
            var qo = Container.NewViewModel<QuickOrderForm>();
            qo.Customer = customer;
            return qo;
        }

        public virtual bool Default1CreateNewOrder() {
            return true;
        }

        #endregion
    }
}