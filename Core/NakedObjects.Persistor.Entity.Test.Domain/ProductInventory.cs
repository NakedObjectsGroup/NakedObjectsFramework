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

    public partial class ProductInventory {
        #region Primitive Properties

        #region ProductID (Int32)

        [MemberOrder(100)]
        public virtual int ProductID { get; set; }

        #endregion

        #region LocationID (Int16)

        [MemberOrder(110)]
        public virtual short LocationID { get; set; }

        #endregion

        #region Shelf (String)

        [MemberOrder(120), StringLength(10)]
        public virtual string Shelf { get; set; }

        #endregion

        #region Bin (Byte)

        [MemberOrder(130)]
        public virtual byte Bin { get; set; }

        #endregion

        #region Quantity (Int16)

        [MemberOrder(140)]
        public virtual short Quantity { get; set; }

        #endregion

        #region rowguid (Guid)

        [MemberOrder(150)]
        public virtual Guid rowguid { get; set; }

        #endregion

        #region ModifiedDate (DateTime)

        [MemberOrder(160), Mask("d")]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion

        #endregion

        #region Navigation Properties

        #region Location (Location)

        [MemberOrder(170)]
        public virtual Location Location { get; set; }

        #endregion

        #region Product (Product)

        [MemberOrder(180)]
        public virtual Product Product { get; set; }

        #endregion

        #endregion
    }
}