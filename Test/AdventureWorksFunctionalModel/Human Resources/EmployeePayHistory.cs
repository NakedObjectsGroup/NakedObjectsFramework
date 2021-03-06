// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;

using NakedFunctions;

namespace AW.Types
{
    public record EmployeePayHistory
    {
        [Hidden]
        public virtual int EmployeeID { get; init; }

        [MemberOrder(1), Mask("d")]
        public virtual DateTime RateChangeDate { get; init; }

        [MemberOrder(2), Mask("C")]
        public virtual decimal Rate { get; init; }

        [MemberOrder(3)]
        public virtual byte PayFrequency { get; init; }

        [MemberOrder(4)]
        public virtual Employee Employee { get; init; }

        [MemberOrder(99)]
        [Versioned]
		public virtual DateTime ModifiedDate { get; init; }

        public override string ToString() => $"{Rate.ToString("C")} from {RateChangeDate.ToString("d")}";

		public override int GetHashCode() =>base.GetHashCode();

        public virtual bool Equals(EmployeePayHistory other) => ReferenceEquals(this, other);
    }
}