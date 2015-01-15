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
    // ReSharper disable once PartialTypeWithSinglePart
    // ReSharper disable InconsistentNaming

    public partial class TransactionHistory {
        #region Primitive Properties

        #region TransactionID (Int32)

        [MemberOrder(100)]
        public virtual int TransactionID { get; set; }

        #endregion

        #region ReferenceOrderID (Int32)

        [MemberOrder(110)]
        public virtual int ReferenceOrderID { get; set; }

        #endregion

        #region ReferenceOrderLineID (Int32)

        [MemberOrder(120)]
        public virtual int ReferenceOrderLineID { get; set; }

        #endregion

        #region TransactionDate (DateTime)

        [MemberOrder(130), Mask("d")]
        public virtual DateTime TransactionDate { get; set; }

        #endregion

        #region TransactionType (String)

        [MemberOrder(140), StringLength(1)]
        public virtual string TransactionType { get; set; }

        #endregion

        #region Quantity (Int32)

        [MemberOrder(150)]
        public virtual int Quantity { get; set; }

        #endregion

        #region ActualCost (Decimal)

        [MemberOrder(160)]
        public virtual decimal ActualCost { get; set; }

        #endregion

        #region ModifiedDate (DateTime)

        [MemberOrder(170), Mask("d")]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion

        #endregion

        #region Navigation Properties

        #region Product (Product)

        [MemberOrder(180)]
        public virtual Product Product { get; set; }

        #endregion

        #endregion
    }
}