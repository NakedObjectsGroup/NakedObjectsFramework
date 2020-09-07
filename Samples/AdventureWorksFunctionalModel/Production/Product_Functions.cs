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

        #region Methods copied from NOF AW
        #region Life Cycle Methods
        public static Product Updating(Product p, [Injected] DateTime now) => p with { ModifiedDate = now };
 
        #endregion

        public static string[] ChoicesProductLine(Product p)
        {
            // nchar(2) in database so pad right with space
            return new[] { "R ", "M ", "T ", "S " };
        }

        public static string[] ChoicesClass(Product p)
        {
            // nchar(2) in database so pad right with space
            return new[] { "H ", "M ", "L " };
        }

        public static string[] ChoicesStyle(Product p)
        {
            // nchar(2) in database so pad right with space
            return new[] { "U ", "M ", "W " };
        }

        [NakedObjectsIgnore]
        public static bool IsDiscontinued(this Product p, DateTime now)
        {
            return p.DiscontinuedDate != null ? p.DiscontinuedDate.Value < now : false;
        }

        public static IQueryable<ProductModel> AutoCompleteProductModel(Product p, string match, IQueryable<ProductModel> models)
        {
            return models.Where(pm => pm.Name.ToUpper().Contains(match.ToUpper()));
        }


        #region BestSpecialOffer


        [DescribedAs("Determines the best discount offered by current special offers for a specified order quantity")]
        public static (SpecialOffer, SpecialOfferProduct) BestSpecialOffer(

            Product p,
            short quantity,
            IQueryable<SpecialOfferProduct> sops,
            IQueryable<SpecialOffer> offers
            )
        {
            var best = BestSpecialOfferProduct(p, quantity, sops);
            if (best != null)
            {
                return (best.SpecialOffer, (SpecialOfferProduct)null);
            }
            var none = SpecialOfferRepository.AssociateSpecialOfferWithProduct(SpecialOfferRepository.NoDiscount(offers), p, sops).Item2;
            return (none.SpecialOffer, none);
        }

        public static string ValidateBestSpecialOffer(Product p, short quantity)
        {
            return quantity <= 0 ? "Quantity must be > 0" : null;
        }

        public static string DisableBestSpecialOffer(Product p, [Injected] DateTime now)
        {
            if (p.IsDiscontinued(now))
            {
                return "Product is discontinued";
            }
            return null;
        }

        public static IList<ProductSubcategory> ChoicesProductSubcategory(
            Product p,
            ProductCategory productCategory,
            IQueryable<ProductSubcategory> subCats)
        {
            if (productCategory != null)
            {
                return (from psc in subCats
                        where psc.ProductCategory.ProductCategoryID == productCategory.ProductCategoryID
                        select psc).ToList();
            }
            return new ProductSubcategory[] { }.ToList();
        }
        [NakedObjectsIgnore]
        public static SpecialOfferProduct BestSpecialOfferProduct(
            Product p,
            short quantity,
            IQueryable<SpecialOfferProduct> sops)
        {
            //reason for testing end date against 1/6/2004 is that in AW database, all offers terminate by 30/6/04
            return sops.Where(obj => obj.Product.ProductID == p.ProductID &&
                              obj.SpecialOffer.StartDate <= DateTime.Now &&
                              obj.SpecialOffer.EndDate >= new DateTime(2004, 6, 1) &&
                              obj.SpecialOffer.MinQty < quantity).
                        OrderByDescending(obj => obj.SpecialOffer.DiscountPct)
                        .FirstOrDefault();

        }

        #endregion


        #endregion
    }
}