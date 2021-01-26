// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Linq;
using AW.Functions;
using NakedFunctions;

namespace AW.Types {
    /// <summary>
    /// 
    /// </summary>
    [ViewModel(typeof(CustomerDashboard_Functions))]
    public record CustomerDashboard {

        public CustomerDashboard(Customer cust)
        {
            Root = cust;
        }

        [Hidden]
        public virtual Customer Root { get; init; }


        public string Name {
            get { return Customer_Functions.IsIndividual(Root) ? Root.Person.ToString() : Root.Store.Name; }
        }

        [DisplayAsProperty]
        [TableView(true, "OrderDate", "TotalDue", "Status")]
        public IList<SalesOrderHeader> RecentOrders(
            IQueryable<SalesOrderHeader> headers)
        {
            return Order_AdditionalFunctions.RecentOrders(Root, headers).ToList();
        }

        //[DisplayAsProperty]
        public decimal TotalOrderValue(
            IQueryable<SalesOrderHeader> headers)
        {
            int id = Root.CustomerID;
            return headers.Where(x => x.Customer.CustomerID == id).Sum(x => x.TotalDue);
        }

        //Empty field, not - to test that fields are not editable in a VM
        public virtual string Comments { get; init; }

        public override string ToString() {
            return $"{Name} - Dashboard";
        }
    }
}