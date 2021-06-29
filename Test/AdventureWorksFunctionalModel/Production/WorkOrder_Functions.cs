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
        internal static IContext UpdateWO(
            WorkOrder original, WorkOrder update, IContext context) =>
            context.WithUpdated(original, update with { ModifiedDate = context.Now() });

        /*[Edit]*/
        public static  IContext ChangeScrappedQuantity(this WorkOrder wo, short scrappedQty, IContext context)
        => UpdateWO(wo, wo with { ScrappedQty = scrappedQty }, context);

        /*[Edit]*/
        public static IContext EditDates(this WorkOrder wo, 
            DateTime startDate, DateTime dueDate, IContext context) =>
                UpdateWO(wo, wo with { StartDate = startDate, DueDate = dueDate }, context);

        public static string ValidateEditDates(this WorkOrder wo, DateTime startDate, DateTime dueDate) =>
            startDate > dueDate ? "StartDate must be before DueDate" : null;

        /*[Edit]*/
        public static  IContext EditOrderQty(
            this WorkOrder wo, int orderQty, IContext context) =>
                UpdateWO(wo, wo with { OrderQty = orderQty }, context);

        public static string Validate1EditOrderQty(this WorkOrder wo, int orderQty) =>
             orderQty <= 0 ? "Order Quantity must be > 0" : null;

        [PageSize(20)]
        public static IQueryable<Product> AutoCompleteProduct(
            [MinLength(2)] string name, IContext context) =>
                Product_MenuFunctions.FindProductByName(name, context);

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
            return (wor, context.WithNew(wor));
        }

        #region Edits

        /*[Edit]*/
        public static IContext EditStartDate(this WorkOrder wo,
            [DefaultValue(0)] DateTime startDate, IContext context) =>
            UpdateWO(wo, wo with { StartDate = startDate }, context);

        /*[Edit]*/
        public static IContext EditDueDate(this WorkOrder wo,
            [DefaultValue(7)] DateTime dueDate, IContext context) =>
               UpdateWO(wo, wo with { DueDate = dueDate }, context);

        #endregion
    }
}