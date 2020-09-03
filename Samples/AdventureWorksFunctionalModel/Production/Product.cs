// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using NakedFunctions;

namespace AdventureWorksModel
{

    public interface IProduct
    {
        string Name { get; }
        int ProductID { get; }
    }

        public record Product : IProduct, IHasModifiedDate, IHasRowGuid
    { 
        #region Properties

        #region ProductID

        [Hidden]
        public virtual int ProductID { get; init; }

        #endregion

        #region Name

       [MemberOrder(1)]
        public virtual string Name { get; init; }

        #endregion

        #region ProductNumber

        [MemberOrder(2)]
        public virtual string ProductNumber { get; init; }

        #endregion

        #region Color

        
        [MemberOrder(3)]
        public virtual string Color { get; init; }

        #endregion

        //#region Photo

        //private Image cachedPhoto;

        //[MemberOrder(4)]
        //public virtual Image Photo
        //{
        //    get
        //    {
        //        if (cachedPhoto == null)
        //        {
        //            ProductPhoto p = (from obj in ProductProductPhoto
        //                              select obj.ProductPhoto).FirstOrDefault();

        //            if (p != null)
        //            {
        //                cachedPhoto = new Image(p.LargePhoto, p.LargePhotoFileName);
        //            }
        //        }
        //        return cachedPhoto;
        //    }
        //}

        //public void AddOrChangePhoto(Image newImage)
        //{
        //    ProductPhoto p = (from obj in ProductProductPhoto
        //                      select obj.ProductPhoto).FirstOrDefault();

        //    p.LargePhoto = newImage.GetResourceAsByteArray();
        //    p.LargePhotoFileName = newImage.Name;
        //}

        //#endregion

        #region Make

        [MemberOrder(20)]
        public virtual bool Make { get; init; }

        #endregion

        #region FinishedGoods

        [MemberOrder(21)]
        public virtual bool FinishedGoods { get; init; }

        #endregion

        #region SafetyStockLevel

        [MemberOrder(22)]
        public virtual short SafetyStockLevel { get; init; }

        #endregion

        #region ReorderPoint

        [MemberOrder(23)]
        public virtual short ReorderPoint { get; init; }

        #endregion

        #region StandardCost

        [MemberOrder(90)]
        [Mask("C")]
        public virtual decimal StandardCost { get; init; }

        #endregion

        #region ListPrice

        [MemberOrder(11)]
        [Mask("C")]
        public virtual decimal ListPrice { get; init; }

        #endregion

        #region Size & Weight

        [Hidden]
        public virtual string Size { get; init; }

        [Hidden]
        public virtual string SizeUnitMeasureCode { get; init; }

        [Hidden]
        public virtual UnitMeasure SizeUnit { get; init; }

        [Named("Size")]
        [MemberOrder(16)]
        public virtual string SizeWithUnit
        {
            get
            {
                return $"{Size} {SizeUnit}";
            }
        }

        [Hidden]
        public virtual string WeightUnitMeasureCode { get; init; }

        [Hidden]
        public virtual decimal? Weight { get; init; }

        [Hidden]
        public virtual UnitMeasure WeightUnit { get; init; }

        [MemberOrder(17)]
        [Named("Weight")]
        public virtual string WeightWithUnit
        {
            get
            {
                return $"{Weight} {WeightUnit}";
            }
        }

        #endregion

        #region DaysToManufacture

        [MemberOrder(24)] //TODO Range(1, 90)]
        public virtual int DaysToManufacture { get; init; }

        #endregion

        #region ProductLine

        
        [MemberOrder(14)]
        public virtual string ProductLine { get; init; }

        #endregion

        #region Class

        
        [MemberOrder(19)]
        public virtual string Class { get; init; }
        #endregion

        #region Style

        
        [MemberOrder(18)]
        public virtual string Style { get; init; }
        #endregion

        #region SellStartDate

        [MemberOrder(81)]
        [Mask("d")]
        public virtual DateTime SellStartDate { get; init; }

        #endregion

        #region SellEndDate

        [MemberOrder(82)]
        
        [Mask("d")]
        public virtual DateTime? SellEndDate { get; init; }

        #endregion

        #region Discontinued

        
        [MemberOrder(83)]
        [Mask("d")]
        //[Range(0, 10)] TODO
        public virtual DateTime? DiscontinuedDate { get; init; }

        #endregion

        #region ProductModel
        [Hidden]
        public virtual int? ProductModelID { get; init; }

        
        [MemberOrder(10)]
        public virtual ProductModel ProductModel { get; init; }
        #endregion

        #region ProductSubcategory
        private ProductCategory productCategory;

        
        
        [MemberOrder(12)]
        public virtual ProductCategory ProductCategory  //TODO: How to handle derived properties?
        {
            get
            {
                if (productCategory == null)
                {
                    return ProductSubcategory == null ? null : ProductSubcategory.ProductCategory;
                }
                return productCategory;
            }
            set { productCategory = value; }
        }

        #region ProductSubcategory
        [Hidden]
        public virtual int? ProductSubcategoryID { get; init; }

        
        [MemberOrder(12)]
        public virtual ProductSubcategory ProductSubcategory { get; init; }


        #endregion

        #endregion

        #region ModifiedDate and rowguid

        #region ModifiedDate

        [MemberOrder(99)]
        
        [ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; init; }
        #endregion

        #region rowguid

        [Hidden]
        public virtual Guid rowguid { get; init; }

        #endregion

        #endregion

        #endregion

        #region Collections

        #region Photos

        private ICollection<ProductProductPhoto> _ProductProductPhoto = new List<ProductProductPhoto>();

        [Hidden]
        public virtual ICollection<ProductProductPhoto> ProductProductPhoto
        {
            get { return _ProductProductPhoto; }
            set { _ProductProductPhoto = value; }
        }

        #endregion

        #region Reviews

        private ICollection<ProductReview> _ProductReviews = new List<ProductReview>();

        [TableView(true, nameof(ProductReview.Rating), nameof(ProductReview.Comments))]
        public virtual ICollection<ProductReview> ProductReviews
        {
            get { return _ProductReviews; } //deliberately returned as array to test Bug #13269
            set { _ProductReviews = value; }
        }

        #endregion

        #region Inventory

        private ICollection<ProductInventory> _ProductInventory = new List<ProductInventory>();

        [RenderEagerly]
        [TableView(false, nameof(AdventureWorksModel.ProductInventory.Quantity),
                nameof(AdventureWorksModel.ProductInventory.Location),
                    nameof(AdventureWorksModel.ProductInventory.Shelf),
                        nameof(AdventureWorksModel.ProductInventory.Bin))]
        public virtual ICollection<ProductInventory> ProductInventory
        {
            get { return _ProductInventory; }
            set { _ProductInventory = value; }
        }

        [Hidden]
        public virtual int NumberInStock()
        {
            return (from obj in ProductInventory
                    select obj).Sum(obj => obj.Quantity);
        }

        #endregion

        #region Special Offers

        [Hidden]
        public virtual ICollection<SpecialOfferProduct> SpecialOfferProduct { get; init; }


        // needs to be initialised for moment
        [NotMapped]
        [RenderEagerly]
        [TableView(true, "MinQty", "DiscountPct", "StartDate", "EndDate")]
        public virtual IList<SpecialOffer> SpecialOffers { get; private set; } = new List<SpecialOffer>();


        #endregion

        #endregion

        public override string ToString()
        {
            return Name;
        }
    }
}