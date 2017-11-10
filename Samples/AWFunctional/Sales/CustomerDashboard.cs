// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Linq;
using NakedObjects;
using NakedFunctions;

namespace AdventureWorksModel {
    /// <summary>
    /// 
    /// </summary>
    public class CustomerDashboard : IFunctionalViewModel {
       [NakedObjectsIgnore]
        public virtual Customer Root { get; set; }
      

        public string Name {
            get { return Root.IsIndividual() ? Root.Person.ToString() : Root.Store.Name; }
        }

        //TODO: Render Function as Property
        //[AsProperty] ?
        [TableView(true, "OrderDate", "TotalDue", "Status")]
        public IList<SalesOrderHeader> RecentOrders(IFunctionalContainer container)  {
            return OrderContributedActions.RecentOrders(Root, container).ToList();
        }

        //TODO: This (derived) function needs to be rendered as a property?
        //[AsProperty] ?  Ditto for collections (incl. contributed ?)
        public decimal TotalOrderValue(IFunctionalContainer container) { 
                int id = Root.CustomerID;
                return container.Instances<SalesOrderHeader>().Where(x => x.Customer.CustomerID == id).Sum(x => x.TotalDue);
        }

        //Empty field, not - to test that fields are not editable in a VM
        public virtual string Comments { get; set; }

        public override string ToString() {
            var t = Container.NewTitleBuilder();
            t.Append(Name).Append(" - Dashboard");
            return t.ToString();
        }

        public SalesOrderHeader NewOrder(IFunctionalContainer container) {
            var order = OrderContributedActions.CreateNewOrder(Root, true, container);
            Container.Persist(ref order);
            return order;
        }

        public string[] DeriveKeys() {
            return new[] {Root.CustomerID.ToString() };
        }

        public void PopulateUsingKeys(string[] keys, IQueryable<Customer> customers) {
            int customerId = int.Parse(keys[0]);
            Root = customers.Single(c => c.CustomerID == customerId);
        }
    }
}