// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NakedObjects;
using NakedObjects.Services;
using System;
using NakedFunctions;

namespace AdventureWorksModel {
    public enum Ordering {
        Ascending,
        Descending
    };

    [DisplayName("Orders")]
    public  class OrderRepository : AWAbstractFactoryAndRepository {

        
        [MemberOrder(99)]
        [QueryOnly]
        public static QueryResultSingle RandomOrder(IFunctionalContainer container) {
            return Random<SalesOrderHeader>(container);
        }

        #region OrdersInProcess

        
        [MemberOrder(5)]
        [TableView(true, "OrderDate", "DueDate")]
        public static IQueryable<SalesOrderHeader> OrdersInProcess(IFunctionalContainer container) {
            return from obj in container.Instances<SalesOrderHeader>()
                where obj.Status == 1
                select obj;
        }

        #endregion

        #region FindOrder

        
        [MemberOrder(10)]
        public static QueryResultSingle FindOrder([DefaultValue("SO")] string orderNumber, IFunctionalContainer container) {
            IQueryable<SalesOrderHeader> query = from obj in container.Instances<SalesOrderHeader>()
                where obj.SalesOrderNumber == orderNumber
                select obj;

            return SingleObjectWarnIfNoMatch(query);
        }

        #region HighestValueOrders

        
        [MemberOrder(90)]
        [TableView(true, "TotalDue", "Customer", "OrderDate", "SalesPerson", "Comment")]
        public static IQueryable<SalesOrderHeader> HighestValueOrders(IFunctionalContainer container) {
            return OrdersByValue(Ordering.Descending, container);
        }

        
        [MemberOrder(91)]
        [TableView(true, "TotalDue", "Customer", "OrderDate", "SalesPerson")]
        public static IQueryable<SalesOrderHeader> OrdersByValue(Ordering ordering, IFunctionalContainer container) {
            return ordering == Ordering.Descending ? container.Instances<SalesOrderHeader>().OrderByDescending(obj => obj.TotalDue) :
                container.Instances<SalesOrderHeader>().OrderBy(obj => obj.TotalDue);
        }

        #endregion

        #endregion

        #region OrdersForCustomer
        //Action to demonstrate use of Auto-Complete that returns a single object
        public static IQueryable<SalesOrderHeader> OrdersForCustomer([DescribedAs("Enter the Account Number (AW + 8 digits) & select the customer")]Customer customer, IFunctionalContainer container) {
            return OrderContributedActions.RecentOrders(customer, container);
        }
     
        [PageSize(10)]
        public static Customer AutoComplete0OrdersForCustomer([MinLength(10)] string accountNumber, IFunctionalContainer container) {
            return CustomerRepository.QueryCustomerByAccountNumber(accountNumber, container).FirstOrDefault();
        }
        #endregion

        [TableView(true,  "OrderDate", "Details")]
        public static IQueryable<SalesOrderHeader> OrdersWithMostLines(IQueryable<SalesOrderHeader> headers)
        {
            return headers.OrderByDescending(obj => obj.Details.Count);
        }

        public static IQueryable<SalesOrderHeader> FindOrders([Optionally] Customer customer, [Optionally] DateTime? orderDate, IFunctionalContainer container)
        {
            IQueryable<SalesOrderHeader> results;
            if (customer != null)
            {
                results = OrdersForCustomer(customer, container);
            } else
            {
                results = container.Instances<SalesOrderHeader>();
            }
            if (orderDate != null)
            {
                results = results.Where(soh => soh.OrderDate == orderDate);
            }
            return results;
        }

        [PageSize(10)]
        public static IQueryable<Customer> AutoComplete0FindOrders([MinLength(10)] string accountNumber, IFunctionalContainer container)
        {
            return CustomerRepository.QueryCustomerByAccountNumber(accountNumber, container);
        }
    }
}