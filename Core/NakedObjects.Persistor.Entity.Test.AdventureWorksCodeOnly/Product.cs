// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NakedObjects.Persistor.Entity.Test.AdventureWorksCodeOnly {
    [Table("Production.Product")]
    public partial class Product {
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

        public virtual ICollection<BillOfMaterial> BillOfMaterials { get; set; } = new List<BillOfMaterial>();

        public virtual ICollection<BillOfMaterial> BillOfMaterials1 { get; set; } = new List<BillOfMaterial>();

        public virtual ProductModel ProductModel { get; set; }

        public virtual ProductSubcategory ProductSubcategory { get; set; }

        public virtual UnitMeasure UnitMeasure { get; set; }

        public virtual UnitMeasure UnitMeasure1 { get; set; }

        public virtual ICollection<ProductCostHistory> ProductCostHistories { get; set; } = new List<ProductCostHistory>();

        public virtual ICollection<ProductDocument> ProductDocuments { get; set; } = new List<ProductDocument>();

        public virtual ICollection<ProductInventory> ProductInventories { get; set; } = new List<ProductInventory>();

        public virtual ICollection<ProductListPriceHistory> ProductListPriceHistories { get; set; } = new List<ProductListPriceHistory>();

        public virtual ICollection<ProductProductPhoto> ProductProductPhotoes { get; set; } = new List<ProductProductPhoto>();

        public virtual ICollection<ProductReview> ProductReviews { get; set; } = new List<ProductReview>();

        public virtual ICollection<ProductVendor> ProductVendors { get; set; } = new List<ProductVendor>();

        public virtual ICollection<PurchaseOrderDetail> PurchaseOrderDetails { get; set; } = new List<PurchaseOrderDetail>();

        public virtual ICollection<ShoppingCartItem> ShoppingCartItems { get; set; } = new List<ShoppingCartItem>();

        public virtual ICollection<SpecialOfferProduct> SpecialOfferProducts { get; set; } = new List<SpecialOfferProduct>();

        public virtual ICollection<TransactionHistory> TransactionHistories { get; set; } = new List<TransactionHistory>();

        public virtual ICollection<WorkOrder> WorkOrders { get; set; } = new List<WorkOrder>();
    }
}