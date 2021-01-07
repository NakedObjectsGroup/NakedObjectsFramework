// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.



using System;
using System.Linq;
using NakedFunctions;
using AW.Types;
using static AW.Helpers;

namespace AW.Functions {
    [Named("Work Orders")]
    public static class WorkOrder_MenuFunctions {

        public static WorkOrder RandomWorkOrder(IContext context) => Random<WorkOrder>(context);

        public static (WorkOrder, IContext context) CreateNewWorkOrder(
             [DescribedAs("product partial name")] this Product product, IContext context) =>
            //TODO: Need to request all required fields for WO & pass into constructor
             DisplayAndSave(new WorkOrder() { Product = product }, context);

        [PageSize(20)]
        public static IQueryable<Product> AutoComplete0CreateNewWorkOrder(
            [Range(2,0)] string name,
            IQueryable<Product> products) {
            return products.Where(p => p.Name.Contains(name));
        }

         public static(WorkOrder, IContext) CreateNewWorkOrder3(
             [DescribedAs("product partial name")] this Product product, int orderQty, IContext context) {
            (var wo, var context2 ) = CreateNewWorkOrder(product, context);
            return DisplayAndSave(wo with { OrderQty = orderQty, ScrappedQty = 0 }, context2);
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