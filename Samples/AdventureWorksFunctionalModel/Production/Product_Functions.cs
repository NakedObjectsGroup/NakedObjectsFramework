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

namespace AdventureWorksFunctionalModel {
    public static class Product_Functions {

        #region Life Cycle Methods
        public static Product Updating(this Product p, [Injected] DateTime now) => p with { ModifiedDate = now };

        #endregion

        #region Edit properties
        #region Edit Product Line
        public static (Product, Product) EditProductLine(this Product p, string line)
            => DisplayAndPersist(p with { ProductLine = line });
        public static string[] Choices0EditProductLine(this Product p)
        => new[] { "R ", "M ", "T ", "S " };  // nchar(2) in database so pad right with space
        #endregion

        #region Edit class
        public static (Product, Product) EditClass(this Product p, string newClass)
    => DisplayAndPersist(p with { Class = newClass });

        public static string[] Choices0EditClass(this Product p)
        => new[] { "H ", "M ", "L " }; // nchar(2) in database so pad right with space
        #endregion

        #region Edit style
        public static (Product, Product) EditStyle(this Product p, string style)
    => DisplayAndPersist(p with { Style = style });

        public static string[] Choices0EditStyle(this Product p)
        => new[] { "U ", "M ", "W " }; // nchar(2) in database so pad right with space
        #endregion

        #region Edit Product Model

        public static (Product, Product) EditProductModel(this Product p, ProductModel newModel)
        => DisplayAndPersist(p with { ProductModel = newModel });

        public static IQueryable<ProductModel> AutoComplete0EditProductModel(Product p, string match, IQueryable<ProductModel> models)
        {
            return models.Where(pm => pm.Name.ToUpper().Contains(match.ToUpper()));
        }
        #endregion

        #region Edit Categories
        public static (Product, Product) EditCategories(this Product p, ProductCategory category, ProductSubcategory subcategory)
            => DisplayAndPersist(p with { ProductCategory = category, ProductSubcategory = subcategory });



        //TODO: Check - does the contributee count as a parameter?
        public static IList<ProductSubcategory> Choices1Edit(
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

        #region BestSpecialOffer


        [DescribedAs("Determines the best discount offered by current special offers for a specified order quantity")]
        public static SpecialOffer BestSpecialOffer(
            Product p, short quantity, IQueryable<SpecialOfferProduct> sops, IQueryable<SpecialOffer> offers)
           => BestSpecialOfferProduct(p, quantity, sops).SpecialOffer ?? SpecialOfferRepository.NoDiscount(offers);

        public static string ValidateBestSpecialOffer(this Product p, short quantity)
            => quantity <= 0 ? "Quantity must be > 0" : null;

        public static string DisableBestSpecialOffer(this Product p, [Injected] DateTime now)
         => p.IsDiscontinued(now) ? "Product is discontinued" : null;

        private static SpecialOfferProduct BestSpecialOfferProduct(
            Product p,
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
    }
}