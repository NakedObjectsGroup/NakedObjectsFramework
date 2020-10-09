// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel.DataAnnotations;
using NakedObjects;

namespace AdventureWorksModel {
    [IconName("clipboard.png")]
    public class EmployeeDepartmentHistory {
        #region Injected Services
        public IDomainObjectContainer Container { set; protected get; }
        #endregion

        #region Life Cycle Methods
        public virtual void Persisting() {
            ModifiedDate = DateTime.Now;
        }

        public virtual void Updating() {
            ModifiedDate = DateTime.Now;
        }
        #endregion

        [NakedObjectsIgnore]
        public virtual int EmployeeID { get; set; }

        [NakedObjectsIgnore]
        public virtual short DepartmentID { get; set; }

        [NakedObjectsIgnore]
        public virtual byte ShiftID { get; set; }

        [Mask("d")]
        [MemberOrder(4)]
        public virtual DateTime StartDate { get; set; }

        [Mask("d")]
        [MemberOrder(5)]
        public virtual DateTime? EndDate { get; set; }

        [MemberOrder(2)]
        public virtual Department Department { get; set; }

        [MemberOrder(1)]
        public virtual Employee Employee { get; set; }

        [MemberOrder(3)]
        public virtual Shift Shift { get; set; }

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        [ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion

        #region Title

        public override string ToString() {
            var t = Container.NewTitleBuilder();
            t.Append(Department).Append(StartDate, "d", null);
            return t.ToString();
        }

        #endregion
    }
}