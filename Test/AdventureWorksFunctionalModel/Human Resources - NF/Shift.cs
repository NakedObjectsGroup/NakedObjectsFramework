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
    [Bounded]
    public record Shift : IHasModifiedDate {
        #region ID

        [Hidden]
        public virtual byte ShiftID { get; init; }

        #endregion

        #region Name

        [MemberOrder(1)]
        public virtual string Name { get; init; }

        #endregion

        [Mask("T")] [MemberOrder(3)]
        public virtual TimeSpan StartTime { get; init; }

        [Mask("T")] [MemberOrder(4)]
        public virtual TimeSpan EndTime { get; init; }

        [MemberOrder(99), ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; init; }

        public override string ToString() => Name;
    }

    public static class ShiftFunctions {
        public static (Shift, Shift) ChangeTimes(Shift s, TimeSpan startTime, TimeSpan endTime) => throw new NotImplementedException();

        //var s2 = s with { Times.StartTime = startTime } with {Times.EndTime,endTime);
        //return DisplayAndPersist(s2);
        public static TimeSpan Default0ChangeTimes(Shift s) => new(0, 9, 0, 0);

        #region Life Cycle Methods

        public static Shift Updating(this Shift x, [Injected] DateTime now) => x with { ModifiedDate = now };

        #endregion
    }
}