// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedFunctions;
using static AW.Utilities;

namespace AW.Types {
        public record SalesOrderDetail {

        #region OrderQty

        
        [MemberOrder(15)]
        public virtual short OrderQty { get; init; }

        #endregion

        #region UnitPrice

        
        [MemberOrder(20)]
        [Mask("C")]
        public virtual decimal UnitPrice { get; init; }

        #endregion

        #region UnitPriceDiscount

        
        [Named("Discount %")]
        [MemberOrder(30)]
        [Mask("P")]
        public virtual decimal UnitPriceDiscount { get; init; }

        #endregion

        #region LineTotal
        
        
        [MemberOrder(40)]
        [Mask("C")]
        public virtual decimal LineTotal { get; init; }

        #endregion

        #region CarrierTrackingNumber

        
        [MemberOrder(50)]
        
        public virtual string CarrierTrackingNumber { get; init; }

        #endregion

        #region SalesOrder

        // [Hidden]
        public virtual SalesOrderHeader SalesOrderHeader { get; init; }

        #endregion

        #region ID

        [Hidden]
        public virtual int SalesOrderID { get; init; }

        [Hidden]
        public virtual int SalesOrderDetailID { get; init; }

        #endregion

        #region Product & Special Offer

        #region SpecialOfferProduct

        [Hidden]
        public virtual int SpecialOfferID { get; init; }

        [Hidden]
        public virtual int ProductID { get; init; }

        [Hidden]
        public virtual SpecialOfferProduct SpecialOfferProduct { get; init; }

        #endregion

        
        
        [MemberOrder(11)]
        public virtual Product Product {
            get { return SpecialOfferProduct == null ? null : SpecialOfferProduct.Product; }
            set {
                //Does nothing -  derived field
            }
        }

        
        [MemberOrder(12)]
        public virtual SpecialOffer SpecialOffer {
            get { return SpecialOfferProduct == null ? null : SpecialOfferProduct.SpecialOffer; }
            set { }
        }

        #endregion

       [MemberOrder(99)]
        //[Versioned]
		public virtual DateTime ModifiedDate { get; init; }

        [Hidden]
        public virtual Guid rowguid { get; init; }

        public override string ToString() => $"{OrderQty} x {Product}";

		public override int GetHashCode() =>base.GetHashCode();

        public virtual bool Equals(SalesOrderDetail other) => ReferenceEquals(this, other);
    }
}