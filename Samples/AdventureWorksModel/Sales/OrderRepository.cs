// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.ComponentModel;
using System.Linq;
using NakedObjects;
using NakedObjects.Services;

namespace AdventureWorksModel {
    public enum Ordering {
        Ascending,
        Descending
    };

    [DisplayName("Orders")]
    public class OrderRepository : AbstractFactoryAndRepository {
        [FinderAction]
        [MemberOrder(99)]
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
    }
}