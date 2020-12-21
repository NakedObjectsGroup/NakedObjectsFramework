// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Linq;
using NakedFunctions;

namespace AdventureWorksModel {
    /// <summary>
    /// 
    /// </summary>
    //TODO: ViewModel
    public record CustomerDashboard {

        public CustomerDashboard(Customer cust)
        {
            Root = cust;
        }

        [Hidden]
        public virtual Customer Root { get; set; }
      

        public string Name {
            get { return CustomerFunctions.IsIndividual(Root) ? Root.Person.ToString() : Root.Store.Name; }
        }

        [DisplayAsProperty]
        [TableView(true, "OrderDate", "TotalDue", "Status")]
        public IList<SalesOrderHeader> RecentOrders(
            IQueryable<SalesOrderHeader> headers)
            {
            return OrderContributedActions.RecentOrders(Root, headers).ToList();
        }

        [DisplayAsProperty]
        public decimal TotalOrderValue(
            IQueryable<SalesOrderHeader> headers)
        {
                int id = Root.CustomerID;
            return headers.Where(x => x.Customer.CustomerID == id).Sum(x => x.TotalDue);
        }

        //Empty field, not - to test that fields are not editable in a VM
        public virtual string Comments { get; set; }

        public override string ToString() {
            return $"{Name} - Dashboard";
        }

        //TODO: not clear what was intended functionality?
        //public (SalesOrderHeader, IContainer) NewOrder(
        //    IQueryable<BusinessEntityAddress> addresses,
        //    IQueryable<SalesOrderHeader> headers) {
        //    var order = OrderContributedActions.CreateNewOrder(Root, true, addresses, headers);
        //    return DisplayAndSave(order, context);
        //}

        public string[] DeriveKeys() {
            return new[] {Root.CustomerID.ToString() };
        }

        //TODO:
        //public void PopulateUsingKeys(string[] keys) {
        //    int customerId = int.Parse(keys[0]);
        //    Root = Container.Instances<Customer>().Single(c => c.CustomerID == customerId);
        //}
    }
}