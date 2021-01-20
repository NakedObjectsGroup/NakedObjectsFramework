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


namespace AW.Functions
{
    public static class WorkOrder_Functions
    {

        public static (WorkOrder, IContext) ChangeScrappedQuantity(this WorkOrder wo, short newQty, IContext context)
        => context.SaveAndDisplay(wo with { ScrappedQty = newQty });

        public static (WorkOrder, IContext) EditDates(this WorkOrder wo, 
            DateTime startDate, DateTime dueDate, IContext context) =>
             context.SaveAndDisplay(wo with { StartDate = startDate, DueDate = dueDate });

        public static string ValidateEditDates(this WorkOrder wo, DateTime startDate, DateTime dueDate) =>
            startDate > dueDate ? "StartDate must be before DueDate" : null;

        public static (WorkOrder, IContext) EditOrderQty(this WorkOrder wo, int orderQty, IContext context) =>
            context.SaveAndDisplay(wo with { OrderQty = orderQty });

        public static string Validate1EditOrderQty(this WorkOrder wo, int orderQty)
        {
            return orderQty <= 0 ? "Order Quantity must be > 0" : null;
        }



        [PageSize(20)]
        public static IQueryable<Product> AutoCompleteProduct([MinLength(2)] string name, IContext context)
        {
            return Product_MenuFunctions.FindProductByName(name, context);
        }

        [MemberOrder(1)]
        public static (WorkOrderRouting, IContext) AddNewRouting(WorkOrder wo, Location loc, IContext context)
        {
            int highestSequence = wo.WorkOrderRoutings.Count > 0 ? wo.WorkOrderRoutings.Max(n => n.OperationSequence) + 1 : 1;
            var wor = new WorkOrderRouting() with
            {
                WorkOrder = wo,
                Location = loc,
                OperationSequence = (short)highestSequence
            };
            return context.SaveAndDisplay(wor);
        }

        #region Edits

        [Edit]
        public static (WorkOrder, IContext) EditStartDate(this WorkOrder wo,
            [DefaultValue(0)] DateTime startDate, IContext context) =>
            context.SaveAndDisplay(wo with { StartDate = startDate });

        [Edit]
        public static (WorkOrder, IContext) EditDueDate(this WorkOrder wo,
            [DefaultValue(7)] DateTime dueDate, IContext context) =>
            context.SaveAndDisplay(wo with { DueDate = dueDate });

        #endregion
    }
}