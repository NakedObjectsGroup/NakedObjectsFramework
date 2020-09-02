// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;

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
        #region Life Cycle Methods
        public virtual void Persisting() {
            ModifiedDate = DateTime.Now;
        }

        public virtual void Updating() {
            ModifiedDate = DateTime.Now;
        }
        #endregion

        #region EmployeeID

        [Hidden]
        public virtual int EmployeeID { get; set; }

        #endregion

        #region RateChangeDate

        [MemberOrder(1)]
        [Mask("d")]
        public virtual DateTime RateChangeDate { get; set; }

        #endregion

        #region Rate

        [Mask("C")]
        [MemberOrder(2)]
        public virtual decimal Rate { get; set; }

        #endregion

        #region Employee

        
        [MemberOrder(4)]
        public virtual Employee Employee { get; set; }

        #endregion

        #region ModifiedDate

        [MemberOrder(99)]
        
        [ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion

        #region Title

        public override string ToString() {
            var t = Container.NewTitleBuilder();
            t.Append(Rate, "C", null).Append(" from", RateChangeDate, "d", null);
            return t.ToString();
        }

        #endregion

        #region Life Cycle methods

        public void Persisted() {
            Employee.PayHistory.Add(this);
        }

        #endregion

        #region PayFrequency

        public virtual byte PayFrequency { get; set; }

        #endregion
    }
}