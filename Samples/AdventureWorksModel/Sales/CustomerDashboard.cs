// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Linq;
using NakedObjects;

namespace AdventureWorksModel {
    /// <summary>
    /// 
    /// </summary>
    public class CustomerDashboard : IViewModel {
        #region Injected Services
        public IDomainObjectContainer Container { set; protected get; }

        public OrderContributedActions OrderContributedActions { set; protected get; }
        #endregion

       [NakedObjectsIgnore]
        public virtual Customer Root { get; set; }
      

        public string Name {
            get { return Root.IsIndividual() ? Root.Person.ToString() : Root.Store.Name; }
        }

        [TableView(true, "OrderDate", "TotalDue", "Status")]
        public IList<SalesOrderHeader> RecentOrders {
            get { return OrderContributedActions.RecentOrders(Root).ToList(); }
        }

        public decimal TotalOrderValue {
            get {
                int id = Root.CustomerID;
                return Container.Instances<SalesOrderHeader>().Where(x => x.Customer.CustomerID == id).Sum(x => x.TotalDue);
            }
        }

        //Empty field, not - to test that fields are not editable in a VM
        public virtual string Comments { get; set; }

        public override string ToString() {
            var t = Container.NewTitleBuilder();
            t.Append(Name).Append(" - Dashboard");
            return t.ToString();
        }

        public SalesOrderHeader NewOrder() {
            var order = OrderContributedActions.CreateNewOrder(Root, true);
            Container.Persist(ref order);
            return order;
        }

        public string[] DeriveKeys() {
            return new[] {Root.CustomerID.ToString() };
        }

        public void PopulateUsingKeys(string[] keys) {
            int customerId = int.Parse(keys[0]);
            Root = Container.Instances<Customer>().Single(c => c.CustomerID == customerId);
        }
    }
}