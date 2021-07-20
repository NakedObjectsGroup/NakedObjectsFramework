// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using NakedFunctions;
using AW.Types;
using NakedFramework.Value;

namespace AW.Functions
{
    public static class Product_Functions
    {
        #region Helpers
        internal static string WeightWithUnit(this Product p) =>
            $"{p.Weight} {p.WeightUnit}";

        internal static ProductCategory ProductCategory(this Product p) =>
              p.ProductSubcategory is null ? null : p.ProductSubcategory.ProductCategory;

        internal static string SizeWithUnit(this Product p) =>
            $"{p.Size} {p.SizeUnit}";
        #endregion

        #region BestSpecialOffer
        [DescribedAs("Determines the best discount offered by current special offers for a specified order quantity")]
        public static SpecialOffer BestSpecialOffer(
            this Product p, [ValueRange(1, 999)] int quantity, IContext context) =>
               BestSpecialOfferProduct(p, (short)quantity, context).SpecialOffer;

        public static string DisableBestSpecialOffer(this Product p, IContext context)
         => p.IsDiscontinued(context) ? "Product is discontinued" : null;

        internal static SpecialOfferProduct BestSpecialOfferProduct(
            this Product p, short quantity, IContext context)
        {
            //TODO: Currently this ignores end date, but this is because all special offers in the AW database,
            //including No Discount have ended!
            var pID = p.ProductID;
            var today = context.Today();
            var best = context.Instances<SpecialOfferProduct>().Where(obj => obj.Product.ProductID == p.ProductID &&
                              obj.SpecialOffer.StartDate <= today &&
                              obj.SpecialOffer.MinQty < quantity).OrderByDescending(obj => obj.SpecialOffer.DiscountPct).FirstOrDefault();
            if (best is null) throw new Exception($"No Special Offers associated with {p}");
            return best;
        }

        private static bool IsDiscontinued(this Product p, IContext context) =>
            p.DiscontinuedDate != null ? p.DiscontinuedDate.Value < context.Now() : false;

        #endregion

        #region Associate with Special Offer
        public static IContext AssociateWithSpecialOffer(
            this Product product, SpecialOffer offer, IContext context) =>
            SpecialOffer_Functions.AssociateWithProduct(offer, product, context);

        [PageSize(20)]
        public static IQueryable<SpecialOffer> AutoComplete1AssociateWithSpecialOffer(
            this Product product,
            [MinLength(2)] string name, IContext context) =>
            context.Instances<SpecialOffer>().Where(specialOffer => specialOffer.Description.ToUpper().StartsWith(name.ToUpper()));
        #endregion

        #region CurrentWorkOrders

        [TableView(true, "Product", "OrderQty", "StartDate")]
        public static IQueryable<WorkOrder> CurrentWorkOrders(
            this Product product, IContext context) =>
            WorkOrder_MenuFunctions.ListWorkOrders(product, true, context);

        #endregion


        internal static int NumberInStock(this Product p) =>
            p.ProductInventory.Sum(obj => obj.Quantity);

        #region Edits
        internal static IContext UpdateProduct(
            Product original, Product updated, IContext context) =>
                 context.WithUpdated(original, updated with { ModifiedDate = context.Now() });

        [Edit]
        public static IContext EditProductLine(this Product p,
            string productLine, IContext context) =>
            UpdateProduct(p, p with { ProductLine = productLine }, context);

        public static IList<string> Choices1EditProductLine(this Product p)
        => new List<string> { "R ", "M ", "T ", "S " };  // nchar(2) in database so pad right with space

        [Edit]
        public static IContext EditClass(this Product p,
          string @class, IContext context) =>
                UpdateProduct(p, p with { Class = @class }, context);

        public static IList<string> Choices1EditClass(this Product p) =>
            new[] { "H ", "M ", "L " }; // nchar(2) in database so pad right with space

        [Edit]
        public static IContext EditStyle(this Product p,
            string style, IContext context) =>
                UpdateProduct(p, p with { Style = style }, context);

        public static IList<string> Choices1EditStyle(this Product p) =>
            new[] { "U ", "M ", "W " }; // nchar(2) in database so pad right with space

        [Edit]
        public static IContext EditProductModel(this Product p,
            ProductModel productModel, IContext context) =>
                UpdateProduct(p, p with { ProductModel = productModel }, context);


        public static IQueryable<ProductModel> AutoComplete1EditProductModel(this Product p,
            [MinLength(3)] string match, IContext context)
        {
            return context.Instances<ProductModel>().Where(pm => pm.Name.ToUpper().Contains(match.ToUpper()));
        }

        [Edit]
        public static IContext EditCategories(this Product p, ProductCategory productCategory, ProductSubcategory productSubcategory, IContext context) =>
              UpdateProduct(p, p with { ProductSubcategory = productSubcategory }, context);

        public static IList<ProductSubcategory> Choices2EditCategories(this Product p,
            ProductCategory productCategory, IContext context) =>
            productCategory is null ? new ProductSubcategory[] { }
            : context.Instances<ProductSubcategory>().Where(psc => psc.ProductCategory.ProductCategoryID == productCategory.ProductCategoryID).ToArray();

        #endregion

        public static IContext AddOrChangePhoto(this Product product, Image newImage, IContext context)
        {
            var productProductPhoto = product.ProductProductPhoto.FirstOrDefault();
            var productPhoto = productProductPhoto.ProductPhoto;
            var newProductPhoto = productPhoto with { LargePhoto = newImage.GetResourceAsByteArray(), LargePhotoFileName = newImage.Name };
            var newProductProductPhoto = new ProductProductPhoto { ProductPhoto = newProductPhoto, Product = product, ModifiedDate = DateTime.Now };
            var newProduct = product with { ProductProductPhoto = new List<ProductProductPhoto> { newProductProductPhoto } };
            return context.WithUpdated(product, newProduct).WithUpdated(productProductPhoto, newProductProductPhoto).WithUpdated(productPhoto, newProductPhoto);
        }


        //public static (Product, IContext) AddOrChangePhoto(this Product product, Image newImage, IContext context)
        //{
        //    ProductPhoto pp = (from obj in product.ProductProductPhoto
        //                      select obj.ProductPhoto).FirstOrDefault();
        //    var pp2 = pp with
        //    {
        //        LargePhoto = newImage.GetResourceAsByteArray(),
        //        LargePhotoFileName = newImage.Name
        //    };
        //    return (product, context.WithUpdated(pp, pp2));
        //}

        [CreateNew]
        public static (WorkOrder, IContext context) CreateNewWorkOrder(
             this Product product,
             int orderQty,
             DateTime startDate,
             IContext context) =>
                WorkOrder_MenuFunctions.CreateNewWorkOrder(product, orderQty, startDate, context);

        public static IContext AddProductReview(this Product p,
            [DefaultValue(0), ValueRange(-30, 0)] DateTime dateOfReview,
            [Named("No. of Stars (1-5"), DefaultValue(5)] int rating,
            [Optionally] string comments,
            IContext context) =>
             context.WithNew(CreateReview(
                    p,
                    context.CurrentUser().Identity.Name,
                    dateOfReview,
                    "[private]",
                    rating,
                    comments,
                    context));


        private static ProductReview CreateReview(Product p, string reviewerName, DateTime date, string emailAddress, int rating, string comments, IContext context)
        {
            return new ProductReview
            {
                Product = p,
                ReviewerName = reviewerName,
                ReviewDate = date,
                EmailAddress = emailAddress,
                Rating = rating,
                Comments = comments,
                ModifiedDate = context.Now()
            };
        }

        public static List<int> Choices2AddProductReview(this Product p) => Ratings();

        private static List<int> Ratings() => new List<int> { 1, 2, 3, 4, 5 };

        public static string ValidateAddProductReview(this Product p,
             DateTime dateOfReview, int rating, string comments) =>
            LessThan5StarsRequiresComment(rating, comments);

        private static string LessThan5StarsRequiresComment(int rating, string comments) =>
            rating < 5 && string.IsNullOrEmpty(comments) ? "Must provide comments for rating < 5" : null;

        public static IContext AddAnonReviews(this IQueryable<Product> products,
            [Named("No. of Stars (1-5)"), DefaultValue(5)] int rating,
            [Optionally] string comments,
            IContext context) =>
            products.Aggregate(context, (c, p) => c.WithNew(
                CreateReview(p, "Anon.", context.Today(), "[hidden]", rating, comments, context)));

        public static List<int> Choices1AddAnonReviews(this IQueryable<Product> products) =>
            Ratings();

        public static string ValidateAddAnonReviews(this IQueryable<Product> products,
            int rating, string comments) =>
         LessThan5StarsRequiresComment(rating, comments);

        [DisplayAsProperty, MemberOrder(11)]
        public static ProductDescription Description(this Product product) =>
           product.ProductModel is null ? null : ProductModel_Functions.LocalCultureDescription(product.ProductModel);

        [DisplayAsProperty, MemberOrder(110)]
        public static ICollection<SpecialOffer> SpecialOffers(this IProduct product, IContext context)
        {
            //Implementation uses context to check that this works
            int pid = product.ProductID;
            return context.Instances<SpecialOfferProduct>().Where(sop => sop.ProductID == pid).Select(sop => sop.SpecialOffer).ToList();
            //Simpler implementation would be just:
            //product.SpecialOfferProduct.Select(sop => sop.SpecialOffer).ToList();
        }

        internal static Image Photo(Product product)
        {
            ProductPhoto p = product.ProductProductPhoto.Select(p => p.ProductPhoto).FirstOrDefault();
            return p is null ? null : new Image(p.LargePhoto, p.LargePhotoFileName);
        }
    }
}