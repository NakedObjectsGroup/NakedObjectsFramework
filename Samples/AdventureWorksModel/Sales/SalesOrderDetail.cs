// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.ComponentModel;
using NakedObjects;
using System.ComponentModel.DataAnnotations;

namespace AdventureWorksModel {
    [IconName("trolley_item.png")]
    public class SalesOrderDetail : AWDomainObject {

        public override string ToString() {
            var t = new TitleBuilder();
            t.Append(OrderQty.ToString()).Append(" x", Product);
            return t.ToString();
        }


        #region Life Cycle methods

        public void Persisted() {
            SalesOrderHeader.Details.Add(this);
            SalesOrderHeader.Recalculate();
        }

        #endregion

        #region ID

        [Hidden]
        public virtual int SalesOrderID { get; set; }

        [Hidden]
        public virtual int SalesOrderDetailID { get; set; }

        #endregion

        #region Product & Special Offer

        #region SpecialOfferProduct

        [Hidden]
        public virtual SpecialOfferProduct SpecialOfferProduct { get; set; }

        #endregion

        [Disabled]
        [NotPersisted]
        [MemberOrder(11)]
        public virtual Product Product {
            get { return SpecialOfferProduct == null ? null : SpecialOfferProduct.Product; }
            set {
                //Does nothing -  derived field
            }
        }

        [Disabled]
        [MemberOrder(12)]
        public virtual SpecialOffer SpecialOffer {
            get { return SpecialOfferProduct == null ? null : SpecialOfferProduct.SpecialOffer; }
            set { }
        }

        #endregion

        #region OrderQty

        [Disabled]
        [MemberOrder(15)]
        public virtual short OrderQty { get; set; }

        #endregion

        #region UnitPrice

        [Disabled]
        [MemberOrder(20)]
        [Mask("C")]
        public virtual decimal UnitPrice { get; set; }

        #endregion

        #region UnitPriceDiscount

        [Disabled]
        [DisplayName("Discount")]
        [MemberOrder(30)]
        [Mask("C")]
        public virtual decimal UnitPriceDiscount { get; set; }

        #endregion

        #region LineTotal

        [Disabled]
        [MemberOrder(40)]
        [Mask("C")]
        public virtual decimal LineTotal { get; set; }

        #endregion

        #region CarrierTrackingNumber

        [Optionally]
        [MemberOrder(50)]
        [StringLength(25)]
        public virtual string CarrierTrackingNumber { get; set; }

        #endregion

        #region ModifiedDate and rowguid

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        public  override DateTime ModifiedDate { get; set; }

        #endregion

        #region rowguid

        [Hidden]
        public override Guid rowguid { get; set; }

        #endregion

        #endregion

        #region SalesOrder

       // [Hidden]
        private SalesOrderHeader _salesOrderHeader;
        public virtual SalesOrderHeader SalesOrderHeader {
            get { return _salesOrderHeader; }
            set { _salesOrderHeader = value; }
        }

        #endregion

        public void Recalculate() {
            UnitPrice = SpecialOfferProduct.Product.ListPrice;
            UnitPriceDiscount = (SpecialOfferProduct.SpecialOffer.DiscountPct*UnitPrice);
            LineTotal = (UnitPrice - UnitPriceDiscount) * OrderQty;
            if (Container.IsPersistent(this)) {
                SalesOrderHeader.Recalculate();
            }
        }

        public void ChangeQuantity(short newQuantity) {
            OrderQty = newQuantity;
            SpecialOfferProduct = SpecialOfferProduct.Product.BestSpecialOfferProduct(newQuantity);
            Recalculate();
        }

        public virtual string DisableChangeQuantity() {
            return SalesOrderHeader.DisableAddNewDetail();
        }
    }
}