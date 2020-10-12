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

namespace AdventureWorksModel
{
    public record WorkOrder : IHasModifiedDate
    {
        [Hidden]
        public virtual int WorkOrderID { get; init; }

        [MemberOrder(22)]

        public virtual int StockedQty { get; init; }

        [MemberOrder(24)]
        public virtual short ScrappedQty { get; init; }

        [MemberOrder(32)]
        [Mask("d")]
        public virtual DateTime? EndDate { get; init; }

        [MemberOrder(99),ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; init; }

        [Hidden]
        public virtual short? ScrapReasonID { get; init; }


        [MemberOrder(26)]
        public virtual ScrapReason ScrapReason { get; init; }

        [MemberOrder(20)]
        public virtual int OrderQty { get; init; }

        [MemberOrder(30)]
        [Mask("d")]
        public virtual DateTime StartDate { get; init; }

        [MemberOrder(34)]
        [Mask("d")]
        public virtual DateTime DueDate { get; init; }

        [Hidden]
        public virtual int ProductID { get; init; }

        [MemberOrder(10)]
        public virtual Product Product { get; init; }

        [RenderEagerly]
        [TableView(true, "OperationSequence", "ScheduledStartDate", "ScheduledEndDate", "Location", "PlannedCost")]
        public virtual ICollection<WorkOrderRouting> WorkOrderRoutings { get; init; } = new List<WorkOrderRouting>();


        // for testing 

        [Hidden]
        [NotMapped]
        public virtual string AnAlwaysHiddenReadOnlyProperty
        {
            get { return ""; }
        }

      

        public override string ToString() => $"{Product}: {StartDate}";
    }
}