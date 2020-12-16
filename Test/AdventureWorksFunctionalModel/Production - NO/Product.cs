// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NakedObjects;
using NakedObjects.Value;

namespace AdventureWorksModel {
    [IconName("carton.png")]
    public class Product { //: IRedirected {
        #region Injected Services
        public IDomainObjectContainer Container { set; protected get; }
        public SpecialOfferRepository SpecialOfferRepository { set; protected get; }
        public ShoppingCartRepository ShoppingCartRepository { set; protected get; }
        #endregion

        #region Life Cycle Methods
        public virtual void Persisting() {
            rowguid = Guid.NewGuid();
            ModifiedDate = DateTime.Now;
        }

        public virtual void Updating() {
            ModifiedDate = DateTime.Now;
        }
        #endregion

        #region Properties

        #region ProductID

        [NakedObjectsIgnore]
        public virtual int ProductID { get; set; }

        #endregion

        #region Name

        [Title]
        [MemberOrder(1)]
        public virtual string Name { get; set; }

        #endregion

        #region ProductNumber

        [MemberOrder(2)]
        public virtual string ProductNumber { get; set; }

        #endregion

        #region Color

        [Optionally]
        [MemberOrder(3)]
        public virtual string Color { get; set; }

        #endregion

        #region Photo

        private Image cachedPhoto;

        [MemberOrder(4)]
        public virtual Image Photo {
            get {
                if (cachedPhoto == null) {
                    ProductPhoto p = (from obj in ProductProductPhoto
                        select obj.ProductPhoto).FirstOrDefault();

                    if (p != null) {
                        cachedPhoto = new Image(p.LargePhoto, p.LargePhotoFileName);
                    }
                }
                return cachedPhoto;
            }
        }

        public void AddOrChangePhoto(Image newImage) {
            ProductPhoto p = (from obj in ProductProductPhoto
                select obj.ProductPhoto).FirstOrDefault();

            p.LargePhoto = newImage.GetResourceAsByteArray();
            p.LargePhotoFileName = newImage.Name;
        }

        #endregion

        #region Make

        [MemberOrder(20)]
        public virtual bool Make { get; set; }

        #endregion

        #region FinishedGoods

        [MemberOrder(21)]
        public virtual bool FinishedGoods { get; set; }

        #endregion

        #region SafetyStockLevel

        [MemberOrder(22)]
        public virtual short SafetyStockLevel { get; set; }

        #endregion

        #region ReorderPoint

        [MemberOrder(23)]
        public virtual short ReorderPoint { get; set; }

        #endregion

        #region StandardCost

        [MemberOrder(90)]
        [Mask("C")]
        public virtual decimal StandardCost { get; set; }

        #endregion

        #region ListPrice

        [MemberOrder(11)]
        [Mask("C")]
        public virtual decimal ListPrice { get; set; }

        #endregion

        #region Size & Weight

        [NakedObjectsIgnore]
        public virtual string Size { get; set; }

        [NakedObjectsIgnore]
        public virtual string SizeUnitMeasureCode { get; set; }

        [NakedObjectsIgnore]
        public virtual UnitMeasure SizeUnit { get; set; }

        [DisplayName("Size")]
        [MemberOrder(16)]
        public virtual string SizeWithUnit {
            get {
                var t = Container.NewTitleBuilder();
                t.Append(Size).Append(SizeUnit);
                return t.ToString();
            }
        }

        [NakedObjectsIgnore]
        public virtual string WeightUnitMeasureCode { get; set; }

        [NakedObjectsIgnore]
        public virtual decimal? Weight { get; set; }

        [NakedObjectsIgnore]
        public virtual UnitMeasure WeightUnit { get; set; }

        [MemberOrder(17)]
        [DisplayName("Weight")]
        public virtual string WeightWithUnit {
            get {
                var t = Container.NewTitleBuilder();
                t.Append(Weight).Append(WeightUnit);
                return t.ToString();
            }
        }

        #endregion

        #region DaysToManufacture

        [MemberOrder(24), Range(1, 90)]
        public virtual int DaysToManufacture { get; set; }

        #endregion

        #region ProductLine

        [Optionally]
        [MemberOrder(14)]
        public virtual string ProductLine { get; set; }

        public virtual string[] ChoicesProductLine() {
            // nchar(2) in database so pad right with space
            return new[] {"R ", "M ", "T ", "S "};
        }

        #endregion

        #region Class

        [Optionally]
        [MemberOrder(19)]
        public virtual string Class { get; set; }

        public virtual string[] ChoicesClass() {
            // nchar(2) in database so pad right with space
            return new[] {"H ", "M ", "L "};
        }

        #endregion

        #region Style

        [Optionally]
        [MemberOrder(18)]
        public virtual string Style { get; set; }

        public virtual string[] ChoicesStyle() {
            // nchar(2) in database so pad right with space
            return new[] {"U ", "M ", "W "};
        }

        #endregion

        #region SellStartDate

        [MemberOrder(81)]
        [Mask("d")]
        public virtual DateTime SellStartDate { get; set; }

        #endregion

        #region SellEndDate

        [MemberOrder(82)]
        [Optionally]
        [Mask("d")]
        public virtual DateTime? SellEndDate { get; set; }

        #endregion

        #region Discontinued

        [Optionally]
        [MemberOrder(83)]
        [Mask("d")]
        [Range(0, 10)]
        public virtual DateTime? DiscontinuedDate { get; set; }

        [NakedObjectsIgnore]
        public virtual bool IsDiscontinued() {
            return DiscontinuedDate != null ? DiscontinuedDate.Value < DateTime.Now : false;
        }

        #endregion

        #region ProductModel
        [NakedObjectsIgnore]
        public virtual int? ProductModelID { get; set; }

        [Optionally]
        [MemberOrder(10)]
        [FindMenu]
        public virtual ProductModel ProductModel { get; set; }

        public virtual IQueryable<ProductModel> AutoCompleteProductModel(string match) {
            return Container.Instances<ProductModel>().Where(pm => pm.Name.ToUpper().Contains(match.ToUpper()));
        }

        #endregion

        #region ProductSubcategory
        private ProductCategory productCategory;

        [NotPersisted]
        [Optionally]
        [MemberOrder(12)]
        public virtual ProductCategory ProductCategory {
            get {
                if (productCategory == null) {
                    return ProductSubcategory == null ? null : ProductSubcategory.ProductCategory;
                }
                return productCategory;
            }
            set { productCategory = value; }
        }

        #region ProductSubcategory
        [NakedObjectsIgnore]
        public virtual int? ProductSubcategoryID { get; set; }

        [Optionally]
        [MemberOrder(12)]
        public virtual ProductSubcategory ProductSubcategory { get; set; }

        public IList<ProductSubcategory> ChoicesProductSubcategory(ProductCategory productCategory) {
            if (productCategory != null) {
                return (from psc in Container.Instances<ProductSubcategory>()
                    where psc.ProductCategory.ProductCategoryID == productCategory.ProductCategoryID
                    select psc).ToList();
            }
            return new ProductSubcategory[] {}.ToList();
        }
        #endregion

        #endregion

        #region ModifiedDate and rowguid

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        [ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; set; }
        #endregion

        #region rowguid

        [NakedObjectsIgnore]
        public virtual Guid rowguid { get; set; }

        #endregion

        #endregion

        #endregion

        #region Collections

        #region Photos

        private ICollection<ProductProductPhoto> _ProductProductPhoto = new List<ProductProductPhoto>();

        [NakedObjectsIgnore]
        public virtual ICollection<ProductProductPhoto> ProductProductPhoto {
            get { return _ProductProductPhoto; }
            set { _ProductProductPhoto = value; }
        }

        #endregion

        #region Reviews

        private ICollection<ProductReview> _ProductReviews = new List<ProductReview>();

        [TableView(true, nameof(ProductReview.Rating), nameof(ProductReview.Comments))]
        public virtual ICollection<ProductReview> ProductReviews {
            get { return _ProductReviews.ToArray(); } //deliberately returned as array to test Bug #13269
            set { _ProductReviews = value; }
        }

        #endregion

        #region Inventory

        private ICollection<ProductInventory> _ProductInventory = new List<ProductInventory>();

        [Eagerly(EagerlyAttribute.Do.Rendering)]
        [TableView(false, nameof(AdventureWorksModel.ProductInventory.Quantity), 
                nameof(AdventureWorksModel.ProductInventory.Location),
                    nameof(AdventureWorksModel.ProductInventory.Shelf),
                        nameof(AdventureWorksModel.ProductInventory.Bin))]
        public virtual ICollection<ProductInventory> ProductInventory {
            get { return _ProductInventory; }
            set { _ProductInventory = value; }
        }

        [NakedObjectsIgnore]
        public virtual int NumberInStock() {
            return (from obj in ProductInventory
                select obj).Sum(obj => obj.Quantity);
        }

        #endregion

        #region Special Offers

        private ICollection<SpecialOfferProduct> _SpecialOfferProduct = new List<SpecialOfferProduct>();

        [NakedObjectsIgnore]
        public virtual ICollection<SpecialOfferProduct> SpecialOfferProduct {
            get { return _SpecialOfferProduct; }
            set { _SpecialOfferProduct = value; }
        }

        [Eagerly(EagerlyAttribute.Do.Rendering)]
        [TableView(true, "MinQty", "DiscountPct", "StartDate", "EndDate")]
        public virtual IList<SpecialOffer> SpecialOffers {
            get { return SpecialOfferProduct.Select(n => n.SpecialOffer).Where(so => so != null).ToList(); }
        }

        #endregion

        #endregion

        #region BestSpecialOffer

        [QueryOnly]
        [Description("Determines the best discount offered by current special offers for a specified order quantity")]
        public virtual SpecialOffer BestSpecialOffer(short quantity) {
            return BestSpecialOfferProduct(quantity).SpecialOffer;
        }

        public virtual string ValidateBestSpecialOffer(short quantity) {
            return quantity <= 0 ? "Quantity must be > 0" : null;
        }

        public virtual string DisableBestSpecialOffer() {
            if (IsDiscontinued()) {
                return "Product is discontinued";
            }
            return null;
        }

        [NakedObjectsIgnore]
        public virtual SpecialOfferProduct BestSpecialOfferProduct(short quantity) {
            //reason for testing end date against 1/6/2004 is that in AW database, all offers terminate by 30/6/04
            var query = from obj in Container.Instances<SpecialOfferProduct>()
                where obj.Product.ProductID == ProductID &&
                      obj.SpecialOffer.StartDate <= DateTime.Now &&
                      obj.SpecialOffer.EndDate >= new DateTime(2004, 6, 1) &&
                      obj.SpecialOffer.MinQty < quantity
                orderby obj.SpecialOffer.DiscountPct descending
                select obj;

            SpecialOfferProduct best = query.FirstOrDefault();
            if (best != null) {
                return best;
            }
            SpecialOffer none = SpecialOfferRepository.NoDiscount();
            return SpecialOfferRepository.AssociateSpecialOfferWithProduct(none, this);
        }

        #endregion

        private static string redirectUrl;

        // just for testing
        [NakedObjectsIgnore]
        public static void SetRedirectUrl(string newUrl) {
            redirectUrl = newUrl;
        }

        [Hidden(WhenTo.Always)]
        public string GetUrl() {
            return redirectUrl;
        }
    }
}