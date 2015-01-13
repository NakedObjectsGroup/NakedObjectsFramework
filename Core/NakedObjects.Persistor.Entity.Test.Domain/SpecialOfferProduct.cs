// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using NakedObjects;

namespace AdventureWorksModel {
    // ReSharper disable once PartialTypeWithSinglePart

    public partial class SpecialOfferProduct {
        #region Primitive Properties

        #region SpecialOfferID (Int32)

        [MemberOrder(100)]
        public virtual int SpecialOfferID { get; set; }

        #endregion

        #region ProductID (Int32)

        [MemberOrder(110)]
        public virtual int ProductID { get; set; }

        #endregion

        #region rowguid (Guid)

        [MemberOrder(120)]
        public virtual Guid rowguid { get; set; }

        #endregion

        #region ModifiedDate (DateTime)

        [MemberOrder(130), Mask("d")]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion

        #endregion

        #region Navigation Properties

        #region Product (Product)

        [MemberOrder(140)]
        public virtual Product Product { get; set; }

        #endregion

        #region SalesOrderDetails (Collection of SalesOrderDetail)

        private ICollection<SalesOrderDetail> _salesOrderDetails = new List<SalesOrderDetail>();

        [MemberOrder(150), Disabled]
        public virtual ICollection<SalesOrderDetail> SalesOrderDetails {
            get { return _salesOrderDetails; }
            set { _salesOrderDetails = value; }
        }

        #endregion

        #region SpecialOffer (SpecialOffer)

        [MemberOrder(160)]
        public virtual SpecialOffer SpecialOffer { get; set; }

        #endregion

        #endregion
    }
}