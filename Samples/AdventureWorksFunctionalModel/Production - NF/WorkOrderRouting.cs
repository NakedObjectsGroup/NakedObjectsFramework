// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel.DataAnnotations;
using NakedFunctions;

namespace AdventureWorksModel {
    public record WorkOrderRouting  {

        [NakedObjectsIgnore]
        public virtual int WorkOrderID { get; init; }

        [NakedObjectsIgnore]
        public virtual int ProductID { get; init; }

        
        [MemberOrder(1)]
        public virtual short OperationSequence { get; init; }

        [MemberOrder(20)]
        
        public virtual DateTime? ScheduledStartDate { get; init; }

        [MemberOrder(22)]
        public virtual DateTime? ScheduledEndDate { get; init; }

        
        [MemberOrder(21)]
        [Mask("d")]
        public virtual DateTime? ActualStartDate { get; init; }

        
        [MemberOrder(23)]
        [Mask("d")]
        public virtual DateTime? ActualEndDate { get; init; }

        
        [MemberOrder(31)]
        public virtual decimal? ActualResourceHrs { get; init; }

        [Mask("C")]
        [MemberOrder(40)]
        public virtual decimal PlannedCost { get; init; }

        
        [MemberOrder(41)]
        [Mask("C")]
        public virtual decimal? ActualCost { get; init; }

        #region Location
        [NakedObjectsIgnore]
        public virtual short LocationID { get; init; }

        [MemberOrder(10)]
        public virtual Location Location { get; init; }
        #endregion

        [NakedObjectsIgnore]
        public virtual WorkOrder WorkOrder { get; init; }

        [MemberOrder(99), ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; init; }

        public override string ToString() => $"{Location}";
    }
}