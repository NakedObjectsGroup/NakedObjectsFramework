// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects;

namespace AdventureWorksModel {
    public partial class EmployeePayHistory {
        #region Primitive Properties

        #region EmployeeID (Int32)

        [MemberOrder(100)]
        public virtual int EmployeeID { get; set; }

        #endregion

        #region RateChangeDate (DateTime)

        [MemberOrder(110), Mask("d")]
        public virtual DateTime RateChangeDate { get; set; }

        #endregion

        #region Rate (Decimal)

        [MemberOrder(120)]
        public virtual decimal Rate { get; set; }

        #endregion

        #region PayFrequency (Byte)

        [MemberOrder(130)]
        public virtual byte PayFrequency { get; set; }

        #endregion

        #region ModifiedDate (DateTime)

        [MemberOrder(140), Mask("d")]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion

        #endregion

        #region Navigation Properties

        #region Employee (Employee)

        [MemberOrder(150)]
        public virtual Employee Employee { get; set; }

        #endregion

        #endregion
    }
}