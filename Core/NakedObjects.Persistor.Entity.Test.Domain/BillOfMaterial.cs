// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects;

namespace AdventureWorksModel {
    public partial class BillOfMaterial {
        #region Primitive Properties

        #region BillOfMaterialsID (Int32)

        [MemberOrder(100)]
        public virtual int BillOfMaterialsID { get; set; }

        #endregion

        #region StartDate (DateTime)

        [MemberOrder(110), Mask("d")]
        public virtual DateTime StartDate { get; set; }

        #endregion

        #region EndDate (DateTime)

        [MemberOrder(120), Optionally, Mask("d")]
        public virtual Nullable<DateTime> EndDate { get; set; }

        #endregion

        #region BOMLevel (Int16)

        [MemberOrder(130)]
        public virtual short BOMLevel { get; set; }

        #endregion

        #region PerAssemblyQty (Decimal)

        [MemberOrder(140)]
        public virtual decimal PerAssemblyQty { get; set; }

        #endregion

        #region ModifiedDate (DateTime)

        [MemberOrder(150), Mask("d")]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion

        #endregion

        #region Navigation Properties

        #region Product (Product)

        [MemberOrder(160)]
        public virtual Product Product { get; set; }

        #endregion

        #region Product1 (Product)

        [MemberOrder(170)]
        public virtual Product Product1 { get; set; }

        #endregion

        #region UnitMeasure (UnitMeasure)

        [MemberOrder(180)]
        public virtual UnitMeasure UnitMeasure { get; set; }

        #endregion

        #endregion
    }
}