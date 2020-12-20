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
            public record EmployeePayHistory {
        private Employee e;
        private DateTime now;

        public EmployeePayHistory(Employee e, DateTime now, byte payFrequency)
        {
            this.e = e;
            this.now = now;
            PayFrequency = payFrequency;
        }
        public EmployeePayHistory()
        {

        }
        #region Injected Services
        
        #endregion

        #region EmployeeID

        [Hidden]
        public virtual int EmployeeID { get; init; }

        #endregion

        [MemberOrder(1)]
        [Mask("d")]
        public virtual DateTime RateChangeDate { get; init; }

        #region Rate

        [Mask("C")]
        [MemberOrder(2)]
        public virtual decimal Rate { get; init; }

        #endregion

        #region Employee

        
        [MemberOrder(4)]
        public virtual Employee Employee { get; init; }

        #endregion

        [MemberOrder(99), ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; init; }

        public override string ToString() => $"{Rate.ToString("C")} from {RateChangeDate.ToString("d")}";

        #region Life Cycle methods

        public void Persisted() {
            Employee.PayHistory.Add(this);
        }

        #endregion

        #region PayFrequency

        public virtual byte PayFrequency { get; init; }

        #endregion
    }

    public static class EmployeePayHistoryFunctions
    {
        #region Life Cycle Methods
        public static EmployeePayHistory Updating(this EmployeePayHistory x, [Injected] DateTime now) => x with { ModifiedDate = now };

        public static EmployeePayHistory Persisting(this EmployeePayHistory x, [Injected] DateTime now) => x with { ModifiedDate = now };
        #endregion
    }
}