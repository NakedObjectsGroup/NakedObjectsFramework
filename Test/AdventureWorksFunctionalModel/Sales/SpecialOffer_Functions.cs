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

namespace AW.Functions
{
    public static class SpecialOffer_Functions
    {

        #region Life Cycle Methods
        public static SpecialOffer Updating(SpecialOffer x,  IContext context) => x with { ModifiedDate = context.Now() };

        public static SpecialOffer Persisting(SpecialOffer x, IContext context) =>
            x with { ModifiedDate = context.Now(), rowguid = context.NewGuid() };
        #endregion

        #region Edit
        [Edit]
        public static (SpecialOffer, IContext) EditDescription(this SpecialOffer sp, string description, IContext context)
        => DisplayAndSave(sp with { Description = description }, context);

        [Edit]
        public static (SpecialOffer, IContext) EditDiscount(this SpecialOffer sp, decimal discountPct, IContext context)
        => DisplayAndSave(sp with { DiscountPct = discountPct }, context);

        [Edit]
        public static (SpecialOffer, IContext) EditType(this SpecialOffer sp, string type, IContext context)
        => DisplayAndSave(sp with { Type = type }, context);

        [Edit]
        public static (SpecialOffer, IContext) EditCategory(this SpecialOffer sp, string category, IContext context)
        => DisplayAndSave(sp with { Category = category }, context);

        public static string[] Choices0Category(this SpecialOffer sp) => new[] { "Reseller", "Customer" };

        [Edit]
        public static (SpecialOffer, IContext) EditDates(this SpecialOffer sp, DateTime startDate, DateTime endDate, IContext context)
        => DisplayAndSave(sp with { StartDate = startDate, EndDate = endDate }, context);

        public static DateTime Default0EditDates(this SpecialOffer sp, IContext context) => context.GetService<IClock>().Today();

        public static DateTime Default1EditDates(this SpecialOffer sp, IContext context) => context.GetService<IClock>().Today().AddDays(90);

        [Edit]
        public static (SpecialOffer, IContext) EditQuantities(
            this SpecialOffer sp, [DefaultValue(1)] int minQty, [Optionally] int? maxQty, IContext context) => 
            DisplayAndSave(sp with { MinQty = minQty, MaxQty = maxQty }, context);

        public static string ValidateEditQuantities(
            this SpecialOffer sp, [DefaultValue(1)] int minQty, [Optionally] int? maxQty) => 
            minQty >= 1 && maxQty is null || maxQty.Value >= minQty ? null : "Quanties invalid";
        #endregion

        #region AssociateWithProduct

        public static (SpecialOfferProduct, IContext) AssociateWithProduct(
            this SpecialOffer offer, Product product, IContext context) =>
            context.Instances<SpecialOfferProduct>().Where(x => x.SpecialOfferID == offer.SpecialOfferID && x.ProductID == product.ProductID).Count() == 0 ?
                DisplayAndSave(new SpecialOfferProduct() with { SpecialOffer = offer, Product = product }, context)
                : (null, context.WithInformUser($"{offer} is already associated with { product}"));

        [PageSize(20)]
        public static IQueryable<Product> AutoComplete1AssociateWithProduct([Length(2)] string name, IContext context)
            => context.Instances<Product>().Where(product => product.Name.ToUpper().StartsWith(name.ToUpper()));

        #endregion

        #region Queryable-contributed
        private static (IList<SpecialOffer>, IContext) Change(this IQueryable<SpecialOffer> offers, Func<SpecialOffer, SpecialOffer> change, IContext context)
        => DisplayAndSave(offers.ToList().Select(change).ToList(), context);

        //TODO: This example shows we must permit returning a List (not a queryable) for display.
        public static (IList<SpecialOffer>, IContext) ExtendOffers(this IQueryable<SpecialOffer> offers, DateTime toDate, IContext context)
        => Change(offers, x => x with { EndDate = toDate }, context);


        public static (IList<SpecialOffer>, IContext) TerminateActiveOffers(
            this IQueryable<SpecialOffer> offers, IContext context)
        {
            var yesterday = context.Today().AddDays(-1);
            var list = offers.Where(x => x.EndDate > yesterday).ToList().Select(x => x with { EndDate = yesterday }).ToList();
            return DisplayAndSave(list, context);
        }
        #endregion
    }
}