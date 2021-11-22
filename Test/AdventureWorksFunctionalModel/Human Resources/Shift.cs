// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedFunctions;

namespace AW.Types {
    [Bounded]
    public class Shift : IHasModifiedDate {

        public Shift() { }

        public Shift(Shift cloneFrom) {
          ShiftID = cloneFrom.ShiftID;
            Name = cloneFrom.Name;
            StartTime = cloneFrom.StartTime;
            EndTime = cloneFrom.EndTime;
            ModifiedDate = cloneFrom.ModifiedDate;
        }

        [Hidden]
        public virtual byte ShiftID { get; init; }

        [MemberOrder(1)]
        public virtual string Name { get; init; } = "";

        [MemberOrder(3)] [Mask("T")]
        public virtual TimeSpan StartTime { get; init; }

        [MemberOrder(4)] [Mask("T")]
        public virtual TimeSpan EndTime { get; init; }

        public virtual bool Equals(Shift? other) => ReferenceEquals(this, other);

        [MemberOrder(99)]
        [Versioned]
        public virtual DateTime ModifiedDate { get; init; }

        public override string ToString() => Name;

        public override int GetHashCode() => base.GetHashCode();
    }
}