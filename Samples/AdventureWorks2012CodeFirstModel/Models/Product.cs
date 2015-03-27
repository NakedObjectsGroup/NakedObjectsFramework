using System;
using System.Collections.Generic;

namespace AdventureWorks2012CodeFirstModel.Models
{
    public partial class Product
    {
        public Product()
        {
            this.BillOfMaterials = new List<BillOfMaterial>();
            this.BillOfMaterials1 = new List<BillOfMaterial>();
            this.ProductCostHistories = new List<ProductCostHistory>();
            this.ProductDocuments = new List<ProductDocument>();
            this.ProductInventories = new List<ProductInventory>();
            this.ProductListPriceHistories = new List<ProductListPriceHistory>();
            this.ProductProductPhotoes = new List<ProductProductPhoto>();
            this.ProductReviews = new List<ProductReview>();
            this.ProductVendors = new List<ProductVendor>();
            this.PurchaseOrderDetails = new List<PurchaseOrderDetail>();
            this.ShoppingCartItems = new List<ShoppingCartItem>();
            this.SpecialOfferProducts = new List<SpecialOfferProduct>();
            this.TransactionHistories = new List<TransactionHistory>();
            this.WorkOrders = new List<WorkOrder>();
        }

        public int ProductID { get; set; }
        public string Name { get; set; }
        public string ProductNumber { get; set; }
        public bool MakeFlag { get; set; }
        public bool FinishedGoodsFlag { get; set; }
        public string Color { get; set; }
        public short SafetyStockLevel { get; set; }
        public short ReorderPoint { get; set; }
        public decimal StandardCost { get; set; }
        public decimal ListPrice { get; set; }
        public string Size { get; set; }
        public string SizeUnitMeasureCode { get; set; }
        public string WeightUnitMeasureCode { get; set; }
        public Nullable<decimal> Weight { get; set; }
        public int DaysToManufacture { get; set; }
        public string ProductLine { get; set; }
        public string Class { get; set; }
        public string Style { get; set; }
        public Nullable<int> ProductSubcategoryID { get; set; }
        public Nullable<int> ProductModelID { get; set; }
        public System.DateTime SellStartDate { get; set; }
        public Nullable<System.DateTime> SellEndDate { get; set; }
        public Nullable<System.DateTime> DiscontinuedDate { get; set; }
        public System.Guid rowguid { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public virtual ICollection<BillOfMaterial> BillOfMaterials { get; set; }
        public virtual ICollection<BillOfMaterial> BillOfMaterials1 { get; set; }
        public virtual ProductModel ProductModel { get; set; }
        public virtual ProductSubcategory ProductSubcategory { get; set; }
        public virtual UnitMeasure UnitMeasure { get; set; }
        public virtual UnitMeasure UnitMeasure1 { get; set; }
        public virtual ICollection<ProductCostHistory> ProductCostHistories { get; set; }
        public virtual ICollection<ProductDocument> ProductDocuments { get; set; }
        public virtual ICollection<ProductInventory> ProductInventories { get; set; }
        public virtual ICollection<ProductListPriceHistory> ProductListPriceHistories { get; set; }
        public virtual ICollection<ProductProductPhoto> ProductProductPhotoes { get; set; }
        public virtual ICollection<ProductReview> ProductReviews { get; set; }
        public virtual ICollection<ProductVendor> ProductVendors { get; set; }
        public virtual ICollection<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }
        public virtual ICollection<ShoppingCartItem> ShoppingCartItems { get; set; }
        public virtual ICollection<SpecialOfferProduct> SpecialOfferProducts { get; set; }
        public virtual ICollection<TransactionHistory> TransactionHistories { get; set; }
        public virtual ICollection<WorkOrder> WorkOrders { get; set; }
    }
}
