namespace NakedObjects.Persistor.Entity.Test.AdventureWorksCodeOnly
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Production.Product")]
    public partial class Product
    {
        private ICollection<WorkOrder> workOrders =new List<WorkOrder>();
        private ICollection<TransactionHistory> transactionHistories = new List<TransactionHistory>();
        private ICollection<SpecialOfferProduct> specialOfferProducts = new List<SpecialOfferProduct>();
        private ICollection<ShoppingCartItem> shoppingCartItems = new List<ShoppingCartItem>();
        private ICollection<PurchaseOrderDetail> purchaseOrderDetails =new List<PurchaseOrderDetail>();
        private ICollection<ProductVendor> productVendors =new List<ProductVendor>();
        private ICollection<ProductReview> productReviews = new List<ProductReview>();
        private ICollection<ProductProductPhoto> productProductPhotoes =new List<ProductProductPhoto>();
        private ICollection<ProductListPriceHistory> productListPriceHistories = new List<ProductListPriceHistory>();
        private ICollection<ProductInventory> productInventories = new List<ProductInventory>();
        private ICollection<ProductDocument> productDocuments = new List<ProductDocument>();
        private ICollection<ProductCostHistory> productCostHistories =new List<ProductCostHistory>();
        private ICollection<BillOfMaterial> billOfMaterials1 = new List<BillOfMaterial>();
        private ICollection<BillOfMaterial> billOfMaterials =new List<BillOfMaterial>();  

        public int ProductID { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(25)]
        public string ProductNumber { get; set; }

        public bool MakeFlag { get; set; }

        public bool FinishedGoodsFlag { get; set; }

        [StringLength(15)]
        public string Color { get; set; }

        public short SafetyStockLevel { get; set; }

        public short ReorderPoint { get; set; }

        [Column(TypeName = "money")]
        public decimal StandardCost { get; set; }

        [Column(TypeName = "money")]
        public decimal ListPrice { get; set; }

        [StringLength(5)]
        public string Size { get; set; }

        [StringLength(3)]
        public string SizeUnitMeasureCode { get; set; }

        [StringLength(3)]
        public string WeightUnitMeasureCode { get; set; }

        public decimal? Weight { get; set; }

        public int DaysToManufacture { get; set; }

        [StringLength(2)]
        public string ProductLine { get; set; }

        [StringLength(2)]
        public string Class { get; set; }

        [StringLength(2)]
        public string Style { get; set; }

        public int? ProductSubcategoryID { get; set; }

        public int? ProductModelID { get; set; }

        public DateTime SellStartDate { get; set; }

        public DateTime? SellEndDate { get; set; }

        public DateTime? DiscontinuedDate { get; set; }

        public Guid rowguid { get; set; }

        public DateTime ModifiedDate { get; set; }

        public virtual ICollection<BillOfMaterial> BillOfMaterials {
            get { return billOfMaterials; }
            set { billOfMaterials = value; }
        }

        public virtual ICollection<BillOfMaterial> BillOfMaterials1 {
            get { return billOfMaterials1; }
            set { billOfMaterials1 = value; }
        }

        public virtual ProductModel ProductModel { get; set; }

        public virtual ProductSubcategory ProductSubcategory { get; set; }

        public virtual UnitMeasure UnitMeasure { get; set; }

        public virtual UnitMeasure UnitMeasure1 { get; set; }

        public virtual ICollection<ProductCostHistory> ProductCostHistories {
            get { return productCostHistories; }
            set { productCostHistories = value; }
        }

        public virtual ICollection<ProductDocument> ProductDocuments {
            get { return productDocuments; }
            set { productDocuments = value; }
        }

        public virtual ICollection<ProductInventory> ProductInventories {
            get { return productInventories; }
            set { productInventories = value; }
        }

        public virtual ICollection<ProductListPriceHistory> ProductListPriceHistories {
            get { return productListPriceHistories; }
            set { productListPriceHistories = value; }
        }

        public virtual ICollection<ProductProductPhoto> ProductProductPhotoes {
            get { return productProductPhotoes; }
            set { productProductPhotoes = value; }
        }

        public virtual ICollection<ProductReview> ProductReviews {
            get { return productReviews; }
            set { productReviews = value; }
        }

        public virtual ICollection<ProductVendor> ProductVendors {
            get { return productVendors; }
            set { productVendors = value; }
        }

        public virtual ICollection<PurchaseOrderDetail> PurchaseOrderDetails {
            get { return purchaseOrderDetails; }
            set { purchaseOrderDetails = value; }
        }

        public virtual ICollection<ShoppingCartItem> ShoppingCartItems {
            get { return shoppingCartItems; }
            set { shoppingCartItems = value; }
        }

        public virtual ICollection<SpecialOfferProduct> SpecialOfferProducts {
            get { return specialOfferProducts; }
            set { specialOfferProducts = value; }
        }

        public virtual ICollection<TransactionHistory> TransactionHistories {
            get { return transactionHistories; }
            set { transactionHistories = value; }
        }

        public virtual ICollection<WorkOrder> WorkOrders {
            get { return workOrders; }
            set { workOrders = value; }
        }
    }
}
