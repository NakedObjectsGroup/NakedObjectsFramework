// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using AW.Functions;
using NakedFramework.Value;
using NakedFunctions;

namespace AW.Types {
    public record Product : IProduct, IHasModifiedDate, IHasRowGuid {
        public virtual bool Equals(Product? other) => ReferenceEquals(this, other);

        public override string ToString() => Name;

        public override int GetHashCode() => base.GetHashCode();

        #region Visible properties

        [MemberOrder(1)]
        public string Name { get; init; } = "";

        [MemberOrder(2)]
        public string ProductNumber { get; init; } = "";

        [MemberOrder(3)]
        public string? Color { get; init; }

        [MemberOrder(4)]
        public virtual Image? Photo => Product_Functions.Photo(this);

        [MemberOrder(10)]
        public virtual ProductModel? ProductModel { get; init; }

        //MemberOrder 11 -  See Product_Functions.Description

        [MemberOrder(12)] [Mask("C")]
        public decimal ListPrice { get; init; }

        [MemberOrder(13)]
        public virtual ProductCategory? ProductCategory => this.ProductCategory();

        [MemberOrder(14)]
        public virtual ProductSubcategory? ProductSubcategory { get; init; }

        [MemberOrder(15)]
        public string? ProductLine { get; init; }

        [Named("Size")] [MemberOrder(16)]
        public string SizeWithUnit => this.SizeWithUnit();

        [Named("Weight")] [MemberOrder(17)]
        public string WeightWithUnit => this.WeightWithUnit();

        [MemberOrder(18)]
        public string? Style { get; init; }

        [MemberOrder(19)]
        public string? Class { get; init; }

        [MemberOrder(20)]
        public bool Make { get; init; }

        [MemberOrder(21)]
        public virtual bool FinishedGoods { get; init; }

        [MemberOrder(22)]
        public short SafetyStockLevel { get; init; }

        [MemberOrder(23)]
        public short ReorderPoint { get; init; }

        [MemberOrder(24)]
        public int DaysToManufacture { get; init; }

        [MemberOrder(81)] [Mask("d")]
        public DateTime SellStartDate { get; init; }

        [MemberOrder(82)] [Mask("d")]
        public DateTime? SellEndDate { get; init; }

        [MemberOrder(83)] [Mask("d")]
        public DateTime? DiscontinuedDate { get; init; }

        [MemberOrder(90)] [Mask("C")]
        public decimal StandardCost { get; init; }

        [MemberOrder(99)] [Versioned]
        public DateTime ModifiedDate { get; init; }

        #endregion

        #region Visible Collections

        [MemberOrder(100)]
        [TableView(true, nameof(ProductReview.Rating), nameof(ProductReview.Comments))]
        public virtual ICollection<ProductReview> ProductReviews { get; init; } = new List<ProductReview>();

        [MemberOrder(120)] [RenderEagerly] [TableView(false,
                                                      nameof(Types.ProductInventory.Quantity),
                                                      nameof(Types.ProductInventory.Location),
                                                      nameof(Types.ProductInventory.Shelf),
                                                      nameof(Types.ProductInventory.Bin))]
        public virtual ICollection<ProductInventory> ProductInventory { get; init; } = new List<ProductInventory>();

        #endregion

        #region Hidden Properties & Collections

        [Hidden]
        public int ProductID { get; init; }

        [Hidden]
        public string? Size { get; init; }

        [Hidden]
        public string? SizeUnitMeasureCode { get; init; }

        [Hidden]
        public virtual UnitMeasure? SizeUnit { get; init; }

        [Hidden]
        public string? WeightUnitMeasureCode { get; init; }

        [Hidden]
        public decimal? Weight { get; init; }

        [Hidden]
        public virtual UnitMeasure? WeightUnit { get; init; }

        [Hidden]
        public int? ProductModelID { get; init; }

        [Hidden]
        public int? ProductSubcategoryID { get; init; }

        [Hidden]
        public Guid rowguid { get; init; }

        public virtual ICollection<ProductProductPhoto> ProductProductPhoto { get; init; } = new List<ProductProductPhoto>();

        [Hidden]
        public virtual ICollection<SpecialOfferProduct> SpecialOfferProduct { get; init; } = new List<SpecialOfferProduct>();

        #endregion
    }
}