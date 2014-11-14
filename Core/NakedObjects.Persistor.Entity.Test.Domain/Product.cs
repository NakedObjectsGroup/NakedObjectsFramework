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
    public partial class Product {
        #region Primitive Properties

        #region ProductID (Int32)

        [MemberOrder(100)]
        public virtual int ProductID { get; set; }

        #endregion

        #region Name (String)

        [MemberOrder(110), StringLength(50)]
        public virtual string Name { get; set; }

        #endregion

        #region ProductNumber (String)

        [MemberOrder(120), StringLength(25)]
        public virtual string ProductNumber { get; set; }

        #endregion

        #region MakeFlag (Boolean)

        [MemberOrder(130)]
        public virtual bool MakeFlag { get; set; }

        #endregion

        #region FinishedGoodsFlag (Boolean)

        [MemberOrder(140)]
        public virtual bool FinishedGoodsFlag { get; set; }

        #endregion

        #region Color (String)

        [MemberOrder(150), Optionally, StringLength(15)]
        public virtual string Color { get; set; }

        #endregion

        #region SafetyStockLevel (Int16)

        [MemberOrder(160)]
        public virtual short SafetyStockLevel { get; set; }

        #endregion

        #region ReorderPoint (Int16)

        [MemberOrder(170)]
        public virtual short ReorderPoint { get; set; }

        #endregion

        #region StandardCost (Decimal)

        [MemberOrder(180)]
        public virtual decimal StandardCost { get; set; }

        #endregion

        #region ListPrice (Decimal)

        [MemberOrder(190)]
        public virtual decimal ListPrice { get; set; }

        #endregion

        #region Size (String)

        [MemberOrder(200), Optionally, StringLength(5)]
        public virtual string Size { get; set; }

        #endregion

        #region Weight (Decimal)

        [MemberOrder(210), Optionally]
        public virtual Nullable<decimal> Weight { get; set; }

        #endregion

        #region DaysToManufacture (Int32)

        [MemberOrder(220)]
        public virtual int DaysToManufacture { get; set; }

        #endregion

        #region ProductLine (String)

        [MemberOrder(230), Optionally, StringLength(2)]
        public virtual string ProductLine { get; set; }

        #endregion

        #region Class (String)

        [MemberOrder(240), Optionally, StringLength(2)]
        public virtual string Class { get; set; }

        #endregion

        #region Style (String)

        [MemberOrder(250), Optionally, StringLength(2)]
        public virtual string Style { get; set; }

        #endregion

        #region SellStartDate (DateTime)

        [MemberOrder(260), Mask("d")]
        public virtual DateTime SellStartDate { get; set; }

        #endregion

        #region SellEndDate (DateTime)

        [MemberOrder(270), Optionally, Mask("d")]
        public virtual Nullable<DateTime> SellEndDate { get; set; }

        #endregion

        #region DiscontinuedDate (DateTime)

        [MemberOrder(280), Optionally, Mask("d")]
        public virtual Nullable<DateTime> DiscontinuedDate { get; set; }

        #endregion

        #region rowguid (Guid)

        [MemberOrder(290)]
        public virtual Guid rowguid { get; set; }

        #endregion

        #region ModifiedDate (DateTime)

        [MemberOrder(300), Mask("d")]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion

        #endregion

        #region Navigation Properties

        #region BillOfMaterials (Collection of BillOfMaterial)

        private ICollection<BillOfMaterial> _billOfMaterials = new List<BillOfMaterial>();

        [MemberOrder(310), Disabled]
        public virtual ICollection<BillOfMaterial> BillOfMaterials {
            get { return _billOfMaterials; }
            set { _billOfMaterials = value; }
        }

        #endregion

        #region BillOfMaterials1 (Collection of BillOfMaterial)

        private ICollection<BillOfMaterial> _billOfMaterials1 = new List<BillOfMaterial>();

        [MemberOrder(320), Disabled]
        public virtual ICollection<BillOfMaterial> BillOfMaterials1 {
            get { return _billOfMaterials1; }
            set { _billOfMaterials1 = value; }
        }

        #endregion

        #region ProductModel (ProductModel)

        [MemberOrder(330)]
        public virtual ProductModel ProductModel { get; set; }

        #endregion

        #region ProductSubcategory (ProductSubcategory)

        [MemberOrder(340)]
        public virtual ProductSubcategory ProductSubcategory { get; set; }

        #endregion

        #region UnitMeasure (UnitMeasure)

        [MemberOrder(350)]
        public virtual UnitMeasure UnitMeasure { get; set; }

        #endregion

        #region UnitMeasure1 (UnitMeasure)

        [MemberOrder(360)]
        public virtual UnitMeasure UnitMeasure1 { get; set; }

        #endregion

        #region ProductCostHistories (Collection of ProductCostHistory)

        private ICollection<ProductCostHistory> _productCostHistories = new List<ProductCostHistory>();

        [MemberOrder(370), Disabled]
        public virtual ICollection<ProductCostHistory> ProductCostHistories {
            get { return _productCostHistories; }
            set { _productCostHistories = value; }
        }

        #endregion

        #region ProductDocuments (Collection of ProductDocument)

        private ICollection<ProductDocument> _productDocuments = new List<ProductDocument>();

        [MemberOrder(380), Disabled]
        public virtual ICollection<ProductDocument> ProductDocuments {
            get { return _productDocuments; }
            set { _productDocuments = value; }
        }

        #endregion

        #region ProductInventories (Collection of ProductInventory)

        private ICollection<ProductInventory> _productInventories = new List<ProductInventory>();

        [MemberOrder(390), Disabled]
        public virtual ICollection<ProductInventory> ProductInventories {
            get { return _productInventories; }
            set { _productInventories = value; }
        }

        #endregion

        #region ProductListPriceHistories (Collection of ProductListPriceHistory)

        private ICollection<ProductListPriceHistory> _productListPriceHistories = new List<ProductListPriceHistory>();

        [MemberOrder(400), Disabled]
        public virtual ICollection<ProductListPriceHistory> ProductListPriceHistories {
            get { return _productListPriceHistories; }
            set { _productListPriceHistories = value; }
        }

        #endregion

        #region ProductProductPhotoes (Collection of ProductProductPhoto)

        private ICollection<ProductProductPhoto> _productProductPhotoes = new List<ProductProductPhoto>();

        [MemberOrder(410), Disabled]
        public virtual ICollection<ProductProductPhoto> ProductProductPhotoes {
            get { return _productProductPhotoes; }
            set { _productProductPhotoes = value; }
        }

        #endregion

        #region ProductReviews (Collection of ProductReview)

        private ICollection<ProductReview> _productReviews = new List<ProductReview>();

        [MemberOrder(420), Disabled]
        public virtual ICollection<ProductReview> ProductReviews {
            get { return _productReviews; }
            set { _productReviews = value; }
        }

        #endregion

        #region ProductVendors (Collection of ProductVendor)

        private ICollection<ProductVendor> _productVendors = new List<ProductVendor>();

        [MemberOrder(430), Disabled]
        public virtual ICollection<ProductVendor> ProductVendors {
            get { return _productVendors; }
            set { _productVendors = value; }
        }

        #endregion

        #region PurchaseOrderDetails (Collection of PurchaseOrderDetail)

        private ICollection<PurchaseOrderDetail> _purchaseOrderDetails = new List<PurchaseOrderDetail>();

        [MemberOrder(440), Disabled]
        public virtual ICollection<PurchaseOrderDetail> PurchaseOrderDetails {
            get { return _purchaseOrderDetails; }
            set { _purchaseOrderDetails = value; }
        }

        #endregion

        #region ShoppingCartItems (Collection of ShoppingCartItem)

        private ICollection<ShoppingCartItem> _shoppingCartItems = new List<ShoppingCartItem>();

        [MemberOrder(450), Disabled]
        public virtual ICollection<ShoppingCartItem> ShoppingCartItems {
            get { return _shoppingCartItems; }
            set { _shoppingCartItems = value; }
        }

        #endregion

        #region SpecialOfferProducts (Collection of SpecialOfferProduct)

        private ICollection<SpecialOfferProduct> _specialOfferProducts = new List<SpecialOfferProduct>();

        [MemberOrder(460), Disabled]
        public virtual ICollection<SpecialOfferProduct> SpecialOfferProducts {
            get { return _specialOfferProducts; }
            set { _specialOfferProducts = value; }
        }

        #endregion

        #region TransactionHistories (Collection of TransactionHistory)

        private ICollection<TransactionHistory> _transactionHistories = new List<TransactionHistory>();

        [MemberOrder(470), Disabled]
        public virtual ICollection<TransactionHistory> TransactionHistories {
            get { return _transactionHistories; }
            set { _transactionHistories = value; }
        }

        #endregion

        #region WorkOrders (Collection of WorkOrder)

        private ICollection<WorkOrder> _workOrders = new List<WorkOrder>();

        [MemberOrder(480), Disabled]
        public virtual ICollection<WorkOrder> WorkOrders {
            get { return _workOrders; }
            set { _workOrders = value; }
        }

        #endregion

        #endregion
    }
}