// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

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
             context.SaveAndDisplay(new WorkOrder() { Product = product });

        [PageSize(20)]
        public static IQueryable<Product> AutoComplete0CreateNewWorkOrder(
            [MinLength(2)] string name, IContext context) =>
            Product_MenuFunctions.FindProductByName(name, context);

        public static(WorkOrder, IContext) CreateNewWorkOrder2(
             [DescribedAs("product partial name")] this Product product, int orderQty, IContext context) {
            (var wo, var context2 ) = CreateNewWorkOrder(product, context);
            return context2.SaveAndDisplay(wo with { OrderQty = orderQty, ScrappedQty = 0 });
        }

        [PageSize(20)]
        public static IQueryable<Product> AutoComplete0CreateNewWorkOrder2(
            [MinLength(2)] string name, IContext context) =>
            Product_MenuFunctions.FindProductByName(name, context);
    
        public static IQueryable<Location> AllLocations(IContext context) => context.Instances<Location>();

        #region CurrentWorkOrders

        [TableView(true, "Product", "OrderQty", "StartDate")]
        public static IQueryable<WorkOrder> ListWorkOrders(
            Product product, [DefaultValue(true)] bool currentOrdersOnly, IContext context) =>
            context.Instances<WorkOrder>().Where(x => x.Product.ProductID == product.ProductID &&
                      (currentOrdersOnly == false || x.EndDate == null));

        [PageSize(20)]
        public static IQueryable<Product> AutoComplete0ListWorkOrders(
            [MinLength(2)] string name, IContext context) =>
            Product_MenuFunctions.FindProductByName(name, context);

        #endregion
    }
}