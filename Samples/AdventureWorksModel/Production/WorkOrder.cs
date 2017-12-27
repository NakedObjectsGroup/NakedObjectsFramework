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
using NakedObjects;

namespace AdventureWorksModel {
    [IconName("gear.png")]
    public class WorkOrder  {
        #region Injected Servives
        public IDomainObjectContainer Container { set; protected get; }
        public ProductRepository ProductRepository { set; protected get; }
        #endregion

        #region Life Cycle Methods
        public virtual void Persisting() {
            ModifiedDate = DateTime.Now;
        }

        public virtual void Updating() {
            ModifiedDate = DateTime.Now;
        }
        #endregion

        #region Title

        public override string ToString() {
            var t = Container.NewTitleBuilder();
            t.Append(Product).Append(":",StartDate, "d MMM yyyy", null);
            return t.ToString();
        }

        #endregion

        #region ID

        [NakedObjectsIgnore]
        public virtual int WorkOrderID { get; set; }

        #endregion

        #region StockedQty

        [MemberOrder(22)]
        [Disabled]
        public virtual int StockedQty { get; set; }

        #endregion

        #region ScrappedQty

        [MemberOrder(24)]
        public virtual short ScrappedQty { get; set; }

        #endregion

        #region EndDate

        [Hidden(WhenTo.UntilPersisted)] //Mandatory  -  for testing only
        [MemberOrder(32)]
        [Mask("d")]
        public virtual DateTime? EndDate { get; set; }

        #endregion

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        [ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion

        #region ScrapReason

        [NakedObjectsIgnore]
        public virtual short? ScrapReasonID { get; set; }

        [Optionally]
        [MemberOrder(26)]
        public virtual ScrapReason ScrapReason { get; set; }

        #endregion

        #region OrderQty

        [MemberOrder(20)]
        public virtual int OrderQty { get; set; }

        public virtual string ValidateOrderQty(int qty) {
            var rb = new ReasonBuilder();
            if (qty <= 0) {
                rb.Append("Order Quantity must be > 0");
            }
            return rb.Reason;
        }

        #endregion

        #region StartDate

        [MemberOrder(30)]
        [Mask("d")]
        public virtual DateTime StartDate { get; set; }

        public virtual DateTime DefaultStartDate() {
            return DateTime.Now;
        }

        #endregion

        #region DueDate

        [MemberOrder(34)]
        [Mask("d")]
        public virtual DateTime DueDate { get; set; }

        public virtual DateTime DefaultDueDate() {
            return DateTime.Now.AddMonths(1);
        }

        #endregion

        #region Product
        [NakedObjectsIgnore]
        public virtual int ProductID { get; set; }

        [MemberOrder(10), FindMenu]
        public virtual Product Product { get; set; }

        [PageSize(20)]
        public IQueryable<Product> AutoCompleteProduct([MinLength(2)] string name) {
            return ProductRepository.FindProductByName(name);
        }

        #endregion

        #region WorkOrderRoutings

        private ICollection<WorkOrderRouting> _WorkOrderRouting = new List<WorkOrderRouting>();

        [Disabled]
        [Hidden(WhenTo.UntilPersisted)]
        [Eagerly(EagerlyAttribute.Do.Rendering)]
        [TableView(true, "OperationSequence", "ScheduledStartDate", "ScheduledEndDate", "Location", "PlannedCost")]
        public virtual ICollection<WorkOrderRouting> WorkOrderRoutings {
            get { return _WorkOrderRouting; }
            set { _WorkOrderRouting = value; }
        }

        #region AddNewRouting (Action)

        [Hidden(WhenTo.UntilPersisted)]
        [MemberOrder(1)]
        public WorkOrderRouting AddNewRouting(Location loc) {
            var wor = Container.NewTransientInstance<WorkOrderRouting>();
            wor.WorkOrder = this;
            wor.Location = loc;
            short highestSequence = 0;
            short increment = 1;
            if (WorkOrderRoutings.Count > 0) {
                highestSequence = WorkOrderRoutings.Max(n => n.OperationSequence);
            }
            highestSequence += increment;
            wor.OperationSequence = highestSequence;
            return wor;
        }

        #endregion

        #endregion

        public string Validate(DateTime startDate, DateTime dueDate) {
            return startDate > dueDate ? "StartDate must be before DueDate" : null;
        }

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
}