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
using static AW.Helpers;

namespace AW.Functions {
    public static class Product_Functions {

        #region BestSpecialOffer
        [DescribedAs("Determines the best discount offered by current special offers for a specified order quantity")]
        public static SpecialOffer BestSpecialOffer(
            this Product p,[ValueRange(1,999)] int quantity, IContext context)
           => BestSpecialOfferProduct(p, (short) quantity, context).SpecialOffer ?? SpecialOffer_MenuFunctions.NoDiscount(context);


        public static string DisableBestSpecialOffer(this Product p,  IContext context)
         => p.IsDiscontinued(context) ? "Product is discontinued" : null;

        internal static SpecialOfferProduct BestSpecialOfferProduct(
            this Product p, short quantity, IContext context) => 
            context.Instances<SpecialOfferProduct>().Where(obj => obj.Product.ProductID == p.ProductID &&
                              obj.SpecialOffer.StartDate <= DateTime.Now &&
                              obj.SpecialOffer.EndDate >= new DateTime(2004, 6, 1) &&
                              obj.SpecialOffer.MinQty < quantity).
                        OrderByDescending(obj => obj.SpecialOffer.DiscountPct)
                        .FirstOrDefault();

        private static bool IsDiscontinued(this Product p, IContext context) =>
            p.DiscontinuedDate != null ? p.DiscontinuedDate.Value < context.Now() : false;
        
        #endregion

        #region Associate with Special Offer
        public static (SpecialOfferProduct, IContext) AssociateWithSpecialOffer(
            this Product product, SpecialOffer offer,IContext context)  => 
            SpecialOffer_Functions.AssociateWithProduct(offer, product, context);

        [PageSize(20)]
        public static IQueryable<SpecialOffer> AutoComplete1AssociateWithSpecialOffer(
            [MinLength(2)] string name, IContext context) => 
            context.Instances<SpecialOffer>().Where(specialOffer => specialOffer.Description.ToUpper().StartsWith(name.ToUpper()));
        #endregion

        #region CurrentWorkOrders

        [TableView(true, "Product", "OrderQty", "StartDate")]
        public static IQueryable<WorkOrder> CurrentWorkOrders(
            this Product product, IContext context) =>
            WorkOrder_MenuFunctions.ListWorkOrders(product, true, context);

        #endregion

        public static (Product, IContext) EditProductLine(this Product p,
            string productLine, IContext context) =>
            DisplayAndSave(p with { ProductLine = productLine }, context);

        public static string[] ChoicesProductLine(this Product p)
        => new[] { "R ", "M ", "T ", "S " };  // nchar(2) in database so pad right with space


        #region Class
        public static string[] ChoicesClass(this Product p)
        => new[] { "H ", "M ", "L " }; // nchar(2) in database so pad right with space
        #endregion

        #region Style
        public static string[] ChoicesStyle(this Product p)
        => new[] { "U ", "M ", "W " }; // nchar(2) in database so pad right with space
        #endregion

        #region Product Model
        public static IQueryable<ProductModel> AutoCompleteProductModel(Product p, string match, IQueryable<ProductModel> models)
        {
            return models.Where(pm => pm.Name.ToUpper().Contains(match.ToUpper()));
        }
        #endregion

        #region Categories
        public static IList<ProductSubcategory> ChoicesCategories(
            Product p,
            ProductCategory productCategory,
            IQueryable<ProductSubcategory> subCats)
        {
            if (productCategory != null)
            {
                return subCats.Where(psc => psc.ProductCategory.ProductCategoryID == productCategory.ProductCategoryID).ToList();
            }
            return new ProductSubcategory[] { }.ToList();
        }
        #endregion
 


    }
}