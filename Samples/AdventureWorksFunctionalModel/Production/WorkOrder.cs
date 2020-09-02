// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using NakedFunctions;
using NakedObjects;

namespace AdventureWorksModel {
    [IconName("gear.png")]
    public class WorkOrder : IHasModifiedDate {

        [NakedObjectsIgnore]
        public virtual int WorkOrderID { get; set; }

        [MemberOrder(22)]
        [Disabled]
        public virtual int StockedQty { get; set; }

        [MemberOrder(24)]
        public virtual short ScrappedQty { get; set; }

        [Hidden(WhenTo.UntilPersisted)] //Mandatory  -  for testing only
        [MemberOrder(32)]
        [Mask("d")]
        public virtual DateTime? EndDate { get; set; }

        [MemberOrder(99)]
        [Disabled]
        [ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; set; }

        [NakedObjectsIgnore]
        public virtual short? ScrapReasonID { get; set; }

        [Optionally]
        [MemberOrder(26)]
        public virtual ScrapReason ScrapReason { get; set; }

        [MemberOrder(20)]
        public virtual int OrderQty { get; set; }

        [MemberOrder(30)]
        [Mask("d")]
        public virtual DateTime StartDate { get; set; }

        [MemberOrder(34)]
        [Mask("d")]
        public virtual DateTime DueDate { get; set; }

        [NakedObjectsIgnore]
        public virtual int ProductID { get; set; }

        [MemberOrder(10)]
        public virtual Product Product { get; set; }

        [Disabled]
        [Hidden(WhenTo.UntilPersisted)]
        [Eagerly(EagerlyAttribute.Do.Rendering)]
        [TableView(true, "OperationSequence", "ScheduledStartDate", "ScheduledEndDate", "Location", "PlannedCost")]
        public virtual ICollection<WorkOrderRouting> WorkOrderRoutings { get; set; }


        // for testing 

        [Hidden(WhenTo.Always)]
        [NotMapped]
        public virtual string AnAlwaysHiddenReadOnlyProperty {
            get { return ""; }
        }

        public void ChangeScrappedQuantity(short newQty)
        {
            this.ScrappedQty = newQty;
        }
    }

    public static class WorkOrderFunctions
    {
        public static string Title(this WorkOrder wo)
        {
            return wo.CreateTitle($"{ProductFunctions2.Title(wo.Product)}: {wo.StartDate.ToString("d MMM yyyy")}");
        }
        #region LifeCycle functions
        public static WorkOrder Updating(WorkOrder wo, [Injected] DateTime now)
        {
            return wo.With(x => x.ModifiedDate, now);
        }
        #endregion

        public static string Validate(WorkOrder wo, DateTime startDate, DateTime dueDate)
        {
            return startDate > dueDate ? "StartDate must be before DueDate" : null;
        }

        public static string ValidateOrderQty(WorkOrder wo, int qty)
        {
            return qty <= 0? "Order Quantity must be > 0": null;
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
        public static IQueryable<Product> AutoCompleteProduct([MinLength(2)] string name, [Injected] IQueryable<Product> products)
        {
            return ProductRepository.FindProductByName( name, products);
        }

        [MemberOrder(1)]
        public static (WorkOrderRouting, WorkOrderRouting) AddNewRouting(WorkOrder wo, Location loc)
        {
            var wor = new WorkOrderRouting();  //TODO: Add all required parameters
            wor.WorkOrder = wo;
            wor.Location = loc;
            short highestSequence = 0;
            short increment = 1;
            if (wo.WorkOrderRoutings.Count > 0)
            {
                highestSequence = wo.WorkOrderRoutings.Max(n => n.OperationSequence);
            }
            highestSequence += increment;
            wor.OperationSequence = highestSequence;
            return Result.DisplayAndPersist(wor);
        }

    }
}