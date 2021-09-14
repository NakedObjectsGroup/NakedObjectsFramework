// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using NakedFunctions;

namespace AW.Types {
    public record WorkOrder : IHasModifiedDate {
        [Hidden]
        public virtual int WorkOrderID { get; init; }

        [MemberOrder(22)]

        public virtual int StockedQty { get; init; }

        [MemberOrder(24)]
        public virtual short ScrappedQty { get; init; }

        [MemberOrder(32)]
        [Mask("d")]
        public virtual DateTime? EndDate { get; init; }

        [Hidden]
        public virtual short? ScrapReasonID { get; init; }

        [MemberOrder(26)]
#pragma warning disable 8618
        public virtual ScrapReason ScrapReason { get; init; }
#pragma warning restore 8618

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
#pragma warning disable 8618
        public virtual Product Product { get; init; }
#pragma warning restore 8618

        [RenderEagerly]
        [TableView(true, "OperationSequence", "ScheduledStartDate", "ScheduledEndDate", "Location", "PlannedCost")]
        public virtual ICollection<WorkOrderRouting> WorkOrderRoutings { get; init; } = new List<WorkOrderRouting>();

        // for testing 

        [Hidden]
        public virtual string AnAlwaysHiddenReadOnlyProperty => "";

        public virtual bool Equals(WorkOrder? other) => ReferenceEquals(this, other);

        [MemberOrder(99)]
        [Versioned]
        public virtual DateTime ModifiedDate { get; init; }

        public override string ToString() => $"{Product}: {StartDate}";

        public override int GetHashCode() => base.GetHashCode();
    }
}