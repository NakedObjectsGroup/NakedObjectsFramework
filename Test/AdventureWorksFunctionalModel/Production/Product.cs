// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using AW.Functions;
using NakedFunctions;
using NakedFramework.Value;

namespace AW.Types
{
    public record Product : IProduct, IHasModifiedDate, IHasRowGuid
    {
        [Hidden]
        public virtual int ProductID { get; init; }
 
        [MemberOrder(1)]
        public virtual string Name { get; init; }
   
        [MemberOrder(2)]
        public virtual string ProductNumber { get; init; }

        [MemberOrder(3)]
        public virtual string Color { get; init; }


        private Image cachedPhoto;

        [MemberOrder(4)]
        public virtual Image Photo
        {
            get
            {
                if (cachedPhoto == null)
                {
                    ProductPhoto p = (from obj in ProductProductPhoto
                                      select obj.ProductPhoto).FirstOrDefault();

                    if (p != null)
                    {
                        cachedPhoto = new Image(p.LargePhoto, p.LargePhotoFileName);
                    }
                }
                return cachedPhoto;
            }
        }

        //

        [MemberOrder(12)]
        public ProductCategory ProductCategory => Product_Functions.ProductCategory(this);

        [MemberOrder(20)]
        public virtual bool Make { get; init; }

    [MemberOrder(21)]
        public virtual bool FinishedGoods { get; init; }
        
        [MemberOrder(22)]
        public virtual short SafetyStockLevel { get; init; }
        
        [MemberOrder(23)]
        public virtual short ReorderPoint { get; init; }

      [MemberOrder(90), Mask("C")]
        public virtual decimal StandardCost { get; init; }
       
        [MemberOrder(11), Mask("C")]
        public virtual decimal ListPrice { get; init; }

       [Hidden]
        public virtual string Size { get; init; }

        [Hidden]
        public virtual string SizeUnitMeasureCode { get; init; }

        [Hidden]
        public virtual UnitMeasure SizeUnit { get; init; }

        [Named("Size"), MemberOrder(16)]
        public string SizeWithUnit => Product_Functions.SizeWithUnit(this);

        [Hidden]
        public virtual string WeightUnitMeasureCode { get; init; }

        [Hidden]
        public virtual decimal? Weight { get; init; }

        [Named("Weight"), MemberOrder(17)]
        public string WeightWithUnit => Product_Functions.WeightWithUnit(this);

        [Hidden]
        public virtual UnitMeasure WeightUnit { get; init; }
  
        [MemberOrder(24)] 
        public virtual int DaysToManufacture { get; init; }

        [MemberOrder(14)]
        public virtual string ProductLine { get; init; }

        [MemberOrder(19)]
        public virtual string Class { get; init; }
        
        [MemberOrder(18)]
        public virtual string Style { get; init; }
               
        [MemberOrder(81),Mask("d")]
        public virtual DateTime SellStartDate { get; init; }

        [MemberOrder(82),Mask("d")]
        public virtual DateTime? SellEndDate { get; init; }

        [MemberOrder(83), Mask("d")]
        public virtual DateTime? DiscontinuedDate { get; init; }

        [Hidden]
        public virtual int? ProductModelID { get; init; }

        [MemberOrder(10)]
        public virtual ProductModel ProductModel { get; init; }
        
        [Hidden]
        public virtual int? ProductSubcategoryID { get; init; }

        [MemberOrder(12)]
        public virtual ProductSubcategory ProductSubcategory { get; init; }

        [Hidden]
        public virtual Guid rowguid { get; init; }

        [MemberOrder(99)]
        //[Versioned]
		public virtual DateTime ModifiedDate { get; init; }
   
        [Hidden]
        public virtual ICollection<ProductProductPhoto> ProductProductPhoto { get; init; } = new List<ProductProductPhoto>();
        

        [TableView(true, nameof(ProductReview.Rating), nameof(ProductReview.Comments))]
        public virtual ICollection<ProductReview> ProductReviews { get; init; } = new List<ProductReview>();

        [RenderEagerly, TableView(false, 
            nameof(Types.ProductInventory.Quantity),
            nameof(Types.ProductInventory.Location),
            nameof(Types.ProductInventory.Shelf),
            nameof(Types.ProductInventory.Bin))]
        public virtual ICollection<ProductInventory> ProductInventory { get; init; } = new List<ProductInventory>();


        [Hidden]
        public virtual ICollection<SpecialOfferProduct> SpecialOfferProduct { get; init; } = new List<SpecialOfferProduct>();

        public override string ToString()=> Name;

		public override int GetHashCode() =>base.GetHashCode();

        public virtual bool Equals(Product other) => ReferenceEquals(this, other);
    }
}