// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Linq;
using NakedFunctions;
using AW.Types;
using static AW.Helpers;
using System;

namespace AW.Functions {
    public enum Ordering {
        Ascending,
        Descending
    };

    [Named("Orders")]
    public static class Order_MenuFunctions  {

        [ MemberOrder(99)]
        public static SalesOrderHeader RandomOrder(IContext context) => Random<SalesOrderHeader>(context);

        [ MemberOrder(5)]
        [TableView(true, "OrderDate", "DueDate")]
        public static IQueryable<SalesOrderHeader> OrdersInProcess(
             IQueryable<SalesOrderHeader> headers) {
            return headers.Where(x => x.StatusByte == 1);
        }

        
        [MemberOrder(10)]
        public static (SalesOrderHeader, IContext) FindOrder(
            [DefaultValue("SO")] string orderNumber, IContext context)
        {
            return Helpers.SingleObjectWarnIfNoMatch(context.Instances<SalesOrderHeader>().Where(x => x.SalesOrderNumber == orderNumber), context);
        }

        [ MemberOrder(90)]
        [TableView(true, "TotalDue", "Customer", "OrderDate", "SalesPerson", "Comment")]
        public static IQueryable<SalesOrderHeader> HighestValueOrders(
            IQueryable<SalesOrderHeader> headers)
        {
            return OrdersByValue(Ordering.Descending, headers);
        }

        
        [MemberOrder(91)]
        [TableView(true, "TotalDue", "Customer", "OrderDate", "SalesPerson")]
        public static IQueryable<SalesOrderHeader> OrdersByValue(
            Ordering ordering,
            IQueryable<SalesOrderHeader> headers)
        {
            return ordering == Ordering.Descending ? headers.OrderByDescending(obj => obj.TotalDue) :
                headers.OrderBy(obj => obj.TotalDue);
        }


        #region OrdersForCustomer
        //Action to demonstrate use of Auto-Complete that returns a single object
        public static IQueryable<SalesOrderHeader> OrdersForCustomer(
            [DescribedAs("Enter the Account Number (AW + 8 digits) & select the customer")]Customer customer,
            IQueryable<SalesOrderHeader> headers
            ) {
            return Order_AdditionalFunctions.RecentOrders(customer, headers);
        }
     
        [PageSize(10)]
        public static Customer AutoComplete0OrdersForCustomer(
            [Range(10,0)] string accountNumber,
            IContext context) {
            return Customer_MenuFunctions.FindCustomerByAccountNumber(accountNumber, context).Item1;
        }
        #endregion

        [TableView(true,  "OrderDate", "Details")]
        public static IQueryable<SalesOrderHeader> OrdersWithMostLines(
            IQueryable<SalesOrderHeader> headers)
        {
            return headers.OrderByDescending(obj => obj.Details.Count);
        }

        public static IQueryable<SalesOrderHeader> FindOrders(
            [Optionally] Customer customer, 
            [Optionally] DateTime? orderDate,
            IQueryable<SalesOrderHeader> headers)
        {
            return customer == null ?
                ByDate(headers, orderDate)
                : ByDate(OrdersForCustomer(customer, headers), orderDate);
        }

        private static IQueryable<SalesOrderHeader> ByDate(IQueryable<SalesOrderHeader> headers,  DateTime? d)
        {
            return d == null ?
                headers
                : headers.Where(soh => soh.OrderDate == d);
        }

        [PageSize(10)]
        public static Customer AutoComplete0FindOrders(
            [Range(10,0)] string accountNumber,
            IContext context)
        {
            return Customer_MenuFunctions.FindCustomerByAccountNumber(accountNumber, context).Item1;
        }
    }
}