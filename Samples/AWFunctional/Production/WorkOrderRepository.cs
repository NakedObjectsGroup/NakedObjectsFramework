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
        [QueryOnly]
        public WorkOrder RandomWorkOrder() {
            return Random<WorkOrder>();
        }

        public WorkOrder CreateNewWorkOrder([ContributedAction("Work Orders"), Description("product partial name")] Product product) {
            var wo = new WorkOrder();
            wo.Product = product;
            return wo;
        }

        [PageSize(20)]
        public IQueryable<Product> AutoComplete0CreateNewWorkOrder([MinLength(2)] string name, IQueryable<Product> products) {
            return products.Where(p => p.Name.Contains(name));
        }

        [DescribedAs("Has no auto-complete or FindMenu - to test use of 'auto-auto-complete' from recently viewed")]
        public WorkOrder CreateNewWorkOrder2(Product product) {
            return CreateNewWorkOrder(product);
        }

        [QueryOnly]
        public void GenerateInfoAndWarning() {
            Container.InformUser("Inform User of something");
            Container.WarnUser("Warn User of something else ");
        }
        #region CurrentWorkOrders

        [TableView(true, "Product", "OrderQty", "StartDate")]
        public IQueryable<WorkOrder> WorkOrders([ContributedAction("Work Orders")] Product product, bool currentOrdersOnly) {
            return from obj in Instances<WorkOrder>()
                where obj.Product.ProductID == product.ProductID &&
                      (currentOrdersOnly == false || obj.EndDate == null)
                select obj;
        }

        [PageSize(20)]
        public IQueryable<Product> AutoComplete0WorkOrders([MinLength(2)] string name, IQueryable<Product> products) {
            return products.Where(p => p.Name.Contains(name));
        }

        #endregion

        public IQueryable<Location> AllLocations(IQueryable<Location> locations)
        {
            return locations;
        }
    }
}