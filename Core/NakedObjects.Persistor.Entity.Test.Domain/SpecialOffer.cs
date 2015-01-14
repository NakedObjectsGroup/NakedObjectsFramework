// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NakedObjects;

namespace AdventureWorksModel {
    // ReSharper disable once PartialTypeWithSinglePart
    // ReSharper disable InconsistentNaming

    public partial class SpecialOffer {
        #region Primitive Properties

        #region SpecialOfferID (Int32)

        [MemberOrder(100)]
        public virtual int SpecialOfferID { get; set; }

        #endregion

        #region Description (String)

        [MemberOrder(110), StringLength(255)]
        public virtual string Description { get; set; }

        #endregion

        #region DiscountPct (Decimal)

        [MemberOrder(120)]
        public virtual decimal DiscountPct { get; set; }

        #endregion

        #region Type (String)

        [MemberOrder(130), StringLength(50)]
        public virtual string Type { get; set; }

        #endregion

        #region Category (String)

        [MemberOrder(140), StringLength(50)]
        public virtual string Category { get; set; }

        #endregion

        #region StartDate (DateTime)

        [MemberOrder(150), Mask("d")]
        public virtual DateTime StartDate { get; set; }

        #endregion

        #region EndDate (DateTime)

        [MemberOrder(160), Mask("d")]
        public virtual DateTime EndDate { get; set; }

        #endregion

        #region MinQty (Int32)

        [MemberOrder(170)]
        public virtual int MinQty { get; set; }

        #endregion

        #region MaxQty (Int32)

        [MemberOrder(180), Optionally]
        public virtual int? MaxQty { get; set; }

        #endregion

        #region rowguid (Guid)

        [MemberOrder(190)]
        public virtual Guid rowguid { get; set; }

        #endregion

        #region ModifiedDate (DateTime)

        [MemberOrder(200), Mask("d")]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion

        #endregion

        #region Navigation Properties

        #region SpecialOfferProducts (Collection of SpecialOfferProduct)

        private ICollection<SpecialOfferProduct> _specialOfferProducts = new List<SpecialOfferProduct>();

        [MemberOrder(210), Disabled]
        public virtual ICollection<SpecialOfferProduct> SpecialOfferProducts {
            get { return _specialOfferProducts; }
            set { _specialOfferProducts = value; }
        }

        #endregion

        #endregion
    }
}