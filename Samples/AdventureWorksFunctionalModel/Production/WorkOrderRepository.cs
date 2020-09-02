// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NakedFunctions;
using NakedObjects;
using static AdventureWorksModel.CommonFactoryAndRepositoryFunctions;
using static NakedFunctions.Result;

namespace AdventureWorksModel {
    [DisplayName("Work Orders")]
    public static class WorkOrderRepository {

        
        public static WorkOrder RandomWorkOrder(
            [Injected] IQueryable<WorkOrder> workOrders,
            [Injected] int random) {
            return Random(workOrders, random);
        }

        public static (WorkOrder, WorkOrder) CreateNewWorkOrder([ContributedAction("Work Orders"), Description("product partial name")] Product product) {
            //TODO: Need to request all required fields for WO & pass into constructor
            var wo = new WorkOrder();
            wo.Product = product;
            return Result.DisplayAndPersist(wo);
        }

        [PageSize(20)]
        public static IQueryable<Product> AutoComplete0CreateNewWorkOrder(
            [MinLength(2)] string name,
            IQueryable<Product> products) {
            return products.Where(p => p.Name.Contains(name));
        }

        //CreateNewWorkOrder2 deleted (no longer relevant for testing)

        public static(WorkOrder, WorkOrder) CreateNewWorkOrder3([ContributedAction("Work Orders"), FindMenu, Description("product partial name")] Product product, int orderQty) {
            var wo = CreateNewWorkOrder(product).Item2;
            wo.OrderQty = orderQty;
            wo.ScrappedQty = 0;
            return Result.DisplayAndPersist(wo);
        }

        [PageSize(20)]
        public static IQueryable<Product> AutoComplete0CreateNewWorkOrder3(
            [MinLength(2)] string name,
            [Injected] IQueryable<Product> products) {
            return products.Where(p => p.Name.Contains(name));
        }

        
        public static string GenerateInfoAndWarning() {
            //TODO: How should we ... or should we not ... distinguish these two? Maybe just an optional attribute in the string
            return Display("Inform User of something [Warn] Warn User of something else ");
        }

        #region CurrentWorkOrders

        [TableView(true, "Product", "OrderQty", "StartDate")]
        public static IQueryable<WorkOrder> WorkOrders(
            [ContributedAction("Work Orders")] Product product, 
            bool currentOrdersOnly,
            [Injected] IQueryable<WorkOrder> workOrders) {
            return from obj in workOrders
                   where obj.Product.ProductID == product.ProductID &&
                      (currentOrdersOnly == false || obj.EndDate == null)
                select obj;
        }

        [PageSize(20)]
        public static IQueryable<Product> AutoComplete0WorkOrders(
            [MinLength(2)] string name,
            [Injected] IQueryable<Product> products) {
            return products.Where(p => p.Name.Contains(name));
        }

        #endregion

        public static IQueryable<Location> AllLocations(
            [Injected] IQueryable<Location> locations
            )
        {
            return locations;
        }
    }
}