// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using AdventureWorksModel;
using NakedFunctions;
using static NakedFunctions.Helpers;

namespace AdventureWorksModel {
    public static class Product_Functions {

        #region Life Cycle Methods
        public static Product Updating(this Product p, [Injected] DateTime now) => p with { ModifiedDate = now };

        #endregion

        #region BestSpecialOffer


        [DescribedAs("Determines the best discount offered by current special offers for a specified order quantity")]
        public static SpecialOffer BestSpecialOffer(
            Product p, short quantity, IQueryable<SpecialOfferProduct> sops, IQueryable<SpecialOffer> offers)
           => BestSpecialOfferProduct(p, quantity, sops).SpecialOffer ?? SpecialOffer_MenuFunctions.NoDiscount(offers);

        public static string ValidateBestSpecialOffer(this Product p, short quantity)
            => quantity <= 0 ? "Quantity must be > 0" : null;

        public static string DisableBestSpecialOffer(this Product p, [Injected] DateTime now)
         => p.IsDiscontinued(now) ? "Product is discontinued" : null;

        public static SpecialOfferProduct BestSpecialOfferProduct(
            this Product p,
            short quantity,
            IQueryable<SpecialOfferProduct> sops)
        => sops.Where(obj => obj.Product.ProductID == p.ProductID &&
                              obj.SpecialOffer.StartDate <= DateTime.Now &&
                              obj.SpecialOffer.EndDate >= new DateTime(2004, 6, 1) &&
                              obj.SpecialOffer.MinQty < quantity).
                        OrderByDescending(obj => obj.SpecialOffer.DiscountPct)
                        .FirstOrDefault();

        private static bool IsDiscontinued(this Product p, DateTime now)
        {
            return p.DiscontinuedDate != null ? p.DiscontinuedDate.Value < now : false;
        }

        #endregion


        #region Associate with Special Offer
        public static (SpecialOfferProduct, IContainer) AssociateWithSpecialOffer(
            this Product product, SpecialOffer offer,IContainer container)  => 
            SpecialOffer_Functions.AssociateWithProduct(offer, product, container);

        [PageSize(20)]
        public static IQueryable<SpecialOffer> AutoComplete1AssociateWithSpecialOffer(
            [Range(2, 0)] string name, IContainer container) => 
            container.Instances<SpecialOffer>().Where(specialOffer => specialOffer.Description.ToUpper().StartsWith(name.ToUpper()));
        #endregion

        #region Property functions
        #region Product Line
        public static string[] ChoicesProductLine(this Product p)
        => new[] { "R ", "M ", "T ", "S " };  // nchar(2) in database so pad right with space
        #endregion

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
        #endregion
    }
}