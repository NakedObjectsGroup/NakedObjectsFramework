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

namespace AdventureWorksModel {
    [DisplayName("Work Orders")]
    public class WorkOrderRepository : AbstractFactoryAndRepository {
        [FinderAction]
        [QueryOnly]
        public WorkOrder RandomWorkOrder() {
            return Random<WorkOrder>();
        }

        [FinderAction]
        public WorkOrder CreateNewWorkOrder([ContributedAction("Work Orders"), FindMenu, Description("product partial name")] Product product) {
            var wo = NewTransientInstance<WorkOrder>();
            wo.Product = product;
            return wo;
        }

        [PageSize(20)]
        public IQueryable<Product> AutoComplete0CreateNewWorkOrder([MinLength(2)] string name) {
            return Container.Instances<Product>().Where(p => p.Name.Contains(name));
        }

        [DescribedAs("Has no auto-complete or FindMenu - to test use of 'auto-auto-complete' from recently viewed")]
        public WorkOrder CreateNewWorkOrder2(Product product) {
            return CreateNewWorkOrder(product);
        }
        [FinderAction]
        public WorkOrder CreateNewWorkOrder3([ContributedAction("Work Orders"), FindMenu, Description("product partial name")] Product product, int orderQty) {
            var wo = CreateNewWorkOrder(product);
            wo.OrderQty = orderQty;
            wo.ScrappedQty = 0;
            Container.Persist(ref wo);
            return wo;
        }

        [PageSize(20)]
        public IQueryable<Product> AutoComplete0CreateNewWorkOrder3([MinLength(2)] string name) {
            return Container.Instances<Product>().Where(p => p.Name.Contains(name));
        }

        [QueryOnly]
        public void GenerateInfoAndWarning() {
            Container.InformUser("Inform User of something");
            Container.WarnUser("Warn User of something else ");
        }


        #region Injected Services

        // This region should contain properties to hold references to any services required by the
        // object.  Use the 'injs' shortcut to add a new service.

        #endregion

        #region CurrentWorkOrders

        [TableView(true, "Product", "OrderQty", "StartDate")]
        public IQueryable<WorkOrder> WorkOrders([ContributedAction("Work Orders")] Product product, bool currentOrdersOnly) {
            return from obj in Instances<WorkOrder>()
                where obj.Product.ProductID == product.ProductID &&
                      (currentOrdersOnly == false || obj.EndDate == null)
                select obj;
        }

        [PageSize(20)]
        public IQueryable<Product> AutoComplete0WorkOrders([MinLength(2)] string name) {
            return Container.Instances<Product>().Where(p => p.Name.Contains(name));
        }

        #endregion

        public IQueryable<Location> AllLocations()
        {
            return Container.Instances<Location>();
        }
    }
}