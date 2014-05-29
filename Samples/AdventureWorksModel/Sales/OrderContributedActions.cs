// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using AdventureWorksModel.Sales;
using NakedObjects;
using NakedObjects.Services;
using System;

namespace AdventureWorksModel {
    [DisplayName("Orders")]
    public class OrderContributedActions : AbstractFactoryAndRepository  {
       
        #region Comments

        public void AppendComment(string commentToAppend, IQueryable<SalesOrderHeader> toOrders) {
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


        public void AppendComment(string commentToAppend, SalesOrderHeader order) {
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

        public void CommentAsUsersUnhappy(IQueryable<SalesOrderHeader> toOrders) {
            AppendComment("User unhappy", toOrders);
        }

        public string ValidateCommentAsUsersUnhappy(IQueryable<SalesOrderHeader> toOrders) {
            return toOrders.Any(o => !o.IsShipped()) ? "Not all shipped yet" : null;
        }


        public void CommentAsUserUnhappy(SalesOrderHeader order) {
            AppendComment("User unhappy", order);
        }

        public string ValidateCommentAsUserUnhappy(SalesOrderHeader order) {
            return order.IsShipped() ? null : "Not shipped yet";
        }


        public void ClearComments(IQueryable<SalesOrderHeader> toOrders) {
            foreach (SalesOrderHeader order in toOrders) {
                order.Comment = null;
            }
        }
        #endregion

        #region RecentOrders

        [MemberOrder(22)]
        [TableView(true,"OrderDate","Status","TotalDue")]
        public IQueryable<SalesOrderHeader> RecentOrders(Customer customer)
        {
            return from obj in Instances<SalesOrderHeader>()
                        where obj.Customer.Id == customer.Id
                        orderby obj.SalesOrderNumber descending
                        select obj;
        }

        #endregion

        #region LastOrder

        [MemberOrder(20), QueryOnly]
        public SalesOrderHeader LastOrder(Customer customer)
        {
            var query = from obj in Container.Instances<SalesOrderHeader>()
                        where obj.Customer.Id == customer.Id
                        orderby obj.SalesOrderNumber descending
                        select obj;

            return SingleObjectWarnIfNoMatch(query);
        }

        #endregion

        #region OpenOrders

        [MemberOrder(21)]
        [TableView(true, "OrderDate", "TotalDue")]
        public IQueryable<SalesOrderHeader> OpenOrders(Customer customer)
        {
            return from obj in Container.Instances<SalesOrderHeader>()
                        where obj.Customer.Id == customer.Id &&
                              obj.Status <= 3
                        orderby obj.SalesOrderNumber descending
                        select obj;
        }

        #endregion

        #region SearchForOrders

        [MemberOrder(12), PageSize(10)]
        [TableView(true, "OrderDate", "Status", "TotalDue")]
        public IQueryable<SalesOrderHeader> SearchForOrders(
            [Optionally]  Customer customer,
            [Optionally] [Mask("d")] DateTime? fromDate,
            [Optionally] [Mask("d")] DateTime? toDate)
        {
            IQueryable<SalesOrderHeader> query = Instances<SalesOrderHeader>();

            if (customer != null)
            {
                query = from obj in query
                        where obj.Customer.Id == customer.Id
                        select obj;
            }

            return from obj in query
                    where ((fromDate == null) || obj.OrderDate >= fromDate) &&
                          ((toDate == null) || obj.OrderDate <= toDate)
                    orderby obj.OrderDate
                    select obj;
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
        public SalesOrderHeader CreateNewOrder(Customer customer,
                                               [Optionally] bool copyHeaderFromLastOrder)
        {
            var newOrder = Container.NewTransientInstance<SalesOrderHeader>();
            newOrder.Customer = customer;

            if (copyHeaderFromLastOrder)
            {
                SalesOrderHeader last = LastOrder(customer);
                if (last != null)
                {
                    newOrder.BillingAddress = last.BillingAddress;
                    newOrder.ShippingAddress = last.ShippingAddress;
                    newOrder.SetUpContact(last.Contact);
                    newOrder.CreditCard = last.CreditCard;
                    newOrder.ShipMethod = last.ShipMethod;
                    newOrder.AccountNumber = last.AccountNumber;
                }
            }
            else
            {
                newOrder.BillingAddress = newOrder.DefaultBillingAddress();
                newOrder.ShippingAddress = newOrder.DefaultShippingAddress();
            }
            return newOrder;
        }

        [MemberOrder(1)]
        public QuickOrderForm QuickOrder(Customer customer) {
            var qo = Container.NewViewModel<QuickOrderForm>();
            qo.Customer = customer;
            return qo;
        }

        public virtual bool Default1CreateNewOrder()
        {
            return true;
        }

        #endregion

        [QueryOnly]
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
    }
}