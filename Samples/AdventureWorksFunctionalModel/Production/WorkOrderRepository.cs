// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.



using System;
using System.Linq;
using NakedFunctions;
using static NakedFunctions.Helpers;
using static AdventureWorksModel.CommonFactoryAndRepositoryFunctions;


namespace AdventureWorksModel {
    [Named("Work Orders")]
    public static class WorkOrderRepository {

        public static WorkOrder RandomWorkOrder(
            IQueryable<WorkOrder> workOrders,
            [Injected] int random) {
            return Random(workOrders, random);
        }

        public static (WorkOrder, WorkOrder) CreateNewWorkOrder(
             [DescribedAs("product partial name")] this Product product) {
            //TODO: Need to request all required fields for WO & pass into constructor
            return DisplayAndPersist(new WorkOrder() { Product = product });
        }

        [PageSize(20)]
        public static IQueryable<Product> AutoComplete0CreateNewWorkOrder(
            [Range(2,0)] string name,
            IQueryable<Product> products) {
            return products.Where(p => p.Name.Contains(name));
        }

         public static(WorkOrder, WorkOrder) CreateNewWorkOrder3([DescribedAs("product partial name")] this Product product, int orderQty) {
            (_, var wo) = CreateNewWorkOrder(product);
            return DisplayAndPersist(wo with { OrderQty = orderQty, ScrappedQty = 0 });
        }

        [PageSize(20)]
        public static IQueryable<Product> AutoComplete0CreateNewWorkOrder3(
            [Range(2,0)] string name,
            IQueryable<Product> products) {
            return products.Where(p => p.Name.Contains(name));
        }


        #region CurrentWorkOrders

        [TableView(true, "Product", "OrderQty", "StartDate")]
        public static IQueryable<WorkOrder> WorkOrders(
            this Product product,
            bool currentOrdersOnly,
            IQueryable<WorkOrder> workOrders) =>
            workOrders.Where(x => x.Product.ProductID == product.ProductID &&
                      (currentOrdersOnly == false || x.EndDate == null));

        [PageSize(20)]
        public static IQueryable<Product> AutoComplete0WorkOrders(
            [Range(2,0)] string name,
            IQueryable<Product> products) {
            return products.Where(p => p.Name.Contains(name));
        }

        #endregion

        public static IQueryable<Location> AllLocations(
            IQueryable<Location> locations
            )
        {
            return locations;
        }
    }
}