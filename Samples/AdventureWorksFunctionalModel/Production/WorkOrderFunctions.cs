// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using NakedFunctions;
using static NakedFunctions.Helpers;

namespace AdventureWorksModel
{
    public static class WorkOrderFunctions
    {

        #region LifeCycle functions
        public static WorkOrder Updating(WorkOrder wo, [Injected] DateTime now)
        {
            return wo with { ModifiedDate = now };
        }
        #endregion

        public static (WorkOrder, WorkOrder) ChangeScrappedQuantity(this WorkOrder wo, short newQty)
        {
           return DisplayAndPersist(wo with { ScrappedQty = newQty });
        }
        public static string Validate(WorkOrder wo, DateTime startDate, DateTime dueDate)
        {
            return startDate > dueDate ? "StartDate must be before DueDate" : null;
        }

        public static string ValidateOrderQty(WorkOrder wo, int qty)
        {
            return qty <= 0 ? "Order Quantity must be > 0" : null;
        }

        public static DateTime DefaultStartDate(WorkOrder wo, [Injected] DateTime now)
        {
            return now;
        }

        public static DateTime DefaultDueDate([Injected] DateTime now)
        {
            return now.AddMonths(1).Date;
        }

        [PageSize(20)]
        public static IQueryable<Product> AutoCompleteProduct([Range(2, 0)] string name, IQueryable<Product> products)
        {
            return Product_MenuFunctions.FindProductByName(name, products);
        }

        [MemberOrder(1)]
        public static (WorkOrderRouting, WorkOrderRouting) AddNewRouting(WorkOrder wo, Location loc)
        {
            int highestSequence = wo.WorkOrderRoutings.Count > 0 ? wo.WorkOrderRoutings.Max(n => n.OperationSequence) + 1 : 1;
            var wor = new WorkOrderRouting() with
            {
                WorkOrder = wo,
                Location = loc,
                OperationSequence = (short) highestSequence
            };
            return DisplayAndPersist(wor);
        }
    }
}