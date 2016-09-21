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

namespace AdventureWorksModel {
    public enum Ordering {
        Ascending,
        Descending
    };

    [DisplayName("Orders")]
    public class OrderRepository : AbstractFactoryAndRepository {

        #region Injected Services
        public CustomerRepository CustomerRepository { set; protected get; }

        public OrderContributedActions OrderContributedActions { set; protected get; }

        #endregion

        [FinderAction]
        [MemberOrder(99)]
        [QueryOnly]
        public SalesOrderHeader RandomOrder() {
            return Random<SalesOrderHeader>();
        }

        #region OrdersInProcess

        [FinderAction]
        [MemberOrder(5)]
        [TableView(true, "OrderDate", "DueDate")]
        public IQueryable<SalesOrderHeader> OrdersInProcess() {
            return from obj in Instances<SalesOrderHeader>()
                where obj.Status == 1
                select obj;
        }

        #endregion

        #region FindOrder

        [FinderAction]
        [MemberOrder(10)]
        public SalesOrderHeader FindOrder([DefaultValue("SO")] string orderNumber) {
            IQueryable<SalesOrderHeader> query = from obj in Instances<SalesOrderHeader>()
                where obj.SalesOrderNumber == orderNumber
                select obj;

            return SingleObjectWarnIfNoMatch(query);
        }

        #region HighestValueOrders

        [FinderAction]
        [MemberOrder(90)]
        [TableView(true, "TotalDue", "Customer", "OrderDate", "SalesPerson", "Comment")]
        public IQueryable<SalesOrderHeader> HighestValueOrders() {
            return OrdersByValue(Ordering.Descending);
        }

        [FinderAction]
        [MemberOrder(91)]
        [TableView(true, "TotalDue", "Customer", "OrderDate", "SalesPerson")]
        public IQueryable<SalesOrderHeader> OrdersByValue(Ordering ordering) {
            return ordering == Ordering.Descending ? Instances<SalesOrderHeader>().OrderByDescending(obj => obj.TotalDue) :
                Instances<SalesOrderHeader>().OrderBy(obj => obj.TotalDue);
        }

        #endregion

        #endregion

        #region OrdersForCustomer
        //Action to demonstrate use of Auto-Complete that returns a single object
        public IQueryable<SalesOrderHeader> OrdersForCustomer([DescribedAs("Enter the Account Number (AW + 8 digits) & select the customer")]Customer customer) {
            return OrderContributedActions.RecentOrders(customer);
        }
     
        [PageSize(10)]
        public Customer AutoComplete0OrdersForCustomer([MinLength(10)] string accountNumber) {
            return CustomerRepository.FindCustomerByAccountNumber(accountNumber);
        }
        #endregion

        [TableView(true,  "OrderDate", "Details")]
        public IQueryable<SalesOrderHeader> OrdersWithMostLines()
        {
            return Instances<SalesOrderHeader>().OrderByDescending(obj => obj.Details.Count);
        }

        public IQueryable<SalesOrderHeader> FindOrders([Optionally] Customer customer, [Optionally] DateTime? orderDate)
        {
            IQueryable<SalesOrderHeader> results;
            if (customer != null)
            {
                results = OrdersForCustomer(customer);
            } else
            {
                results = Container.Instances<SalesOrderHeader>();
            }
            if (orderDate != null)
            {
                results = results.Where(soh => soh.OrderDate == orderDate);
            }
            return results;
        }

        [PageSize(10)]
        public Customer AutoComplete0FindOrders([MinLength(10)] string accountNumber)
        {
            return CustomerRepository.FindCustomerByAccountNumber(accountNumber);
        }
    }
}