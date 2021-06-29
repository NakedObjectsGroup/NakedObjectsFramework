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


namespace AW.Functions
{
    public static class SpecialOffer_Functions
    {
        internal static  IContext UpdateSpecialOffer(
            SpecialOffer original, SpecialOffer updated, IContext context) =>
                context.WithUpdated(original, updated with { ModifiedDate = context.Now() });

        #region Edit
        /*[Edit]*/
        public static  IContext EditDescription(
            this SpecialOffer sp, string description, IContext context) => 
                UpdateSpecialOffer(sp, sp with { Description = description}, context);


        /*[Edit]*/
        public static IContext EditDiscount(
            this SpecialOffer sp, decimal discountPct, IContext context) => 
            UpdateSpecialOffer(sp, sp with { DiscountPct = discountPct }, context);

        public static string DisableEditDiscount(
            this SpecialOffer sp, IContext context) =>
                DisableIfStarted(sp, context);

        /*[Edit]*/
        public static IContext EditType(
            this SpecialOffer sp, string type, IContext context) => 
                UpdateSpecialOffer(sp, sp with { Type = type }, context);

        public static bool HideEditType(
            this SpecialOffer sp, IContext context) =>
                HideIfEnded(sp, context);

        /*[Edit]*/
        public static IContext EditCategory(
            this SpecialOffer sp, string category, IContext context) => 
                UpdateSpecialOffer(sp, sp with { Category = category }, context);

        public static IList<string> Choices1Category(this SpecialOffer sp) => Categories;

        internal static IList<string> Categories = new[] { "Reseller", "Customer" };


        internal static string DisableIfStarted(this SpecialOffer so, IContext context) =>
            context.Today() > so.StartDate? "Offer has started": null;

        internal static bool HideIfEnded(this SpecialOffer so, IContext context) =>
            context.Today() > so.EndDate;

        /*[Edit]*/
        public static IContext EditDates( this SpecialOffer sp,
            DateTime startDate, DateTime endDate, IContext context) => 
                UpdateSpecialOffer(sp, sp with { StartDate = startDate, EndDate = endDate }, context);

        public static DateTime Default1EditDates(this SpecialOffer sp, IContext context) =>
            sp.StartDate;

        public static DateTime Default2EditDates(this SpecialOffer sp, IContext context) =>
            DefaultEndDate(context);

        internal static DateTime DefaultEndDate(IContext context) =>
            context.GetService<IClock>().Today().AddMonths(1);

        /*[Edit]*/
        public static  IContext EditQuantities(this SpecialOffer sp, 
            [DefaultValue(1)] int minQty, [Optionally] int? maxQty, IContext context) =>
                UpdateSpecialOffer(sp, sp with { MinQty = minQty, MaxQty = maxQty }, context);

        public static string Validate1EditQuantities(this SpecialOffer sp, int minQty) =>
            minQty < 1 ? "Must be > 0" : null;

        public static string ValidateEditQuantities(
            this SpecialOffer sp, [DefaultValue(1)] int minQty, [Optionally] int? maxQty, IContext context) =>
               ValidateQuantities(minQty, maxQty);

        internal static string ValidateQuantities(int minQty, int? maxQty) =>
             maxQty != null && maxQty.Value < minQty ? "Max Qty cannot be < Min Qty" : null;
        #endregion


        #region AssociateWithProduct

        public static IContext AssociateWithProduct(
            this SpecialOffer offer, Product product, IContext context)
        {
            var prev = context.Instances<SpecialOfferProduct>().Where(x => x.SpecialOfferID == offer.SpecialOfferID && x.ProductID == product.ProductID).Count() == 0;
            if (prev)
            {
                var newSO = new SpecialOfferProduct() { SpecialOffer = offer, Product = product, ModifiedDate = context.Now(), rowguid = context.NewGuid() };
                return context.WithNew(newSO);
            }
            else
            {
                return context.WithInformUser($"{offer} is already associated with { product}");
            }
        }

        [PageSize(20)]
        public static IQueryable<Product> AutoComplete1AssociateWithProduct([MinLength(2)] string name, IContext context)
            => context.Instances<Product>().Where(product => product.Name.ToUpper().StartsWith(name.ToUpper()));

        #endregion

        public static List<Product> ListAssociatedProducts(this SpecialOffer specialOffer, IContext context)
        {
            int id = specialOffer.SpecialOfferID;
            return context.Instances<SpecialOfferProduct>().Where(x => x.SpecialOfferID == id).Select(x => x.Product).ToList();
        }

        #region Queryable-contributed
        internal static IContext Change(this IQueryable<SpecialOffer> offers,
            Func<SpecialOffer, SpecialOffer> change, IContext context) =>
                offers.Select(x => new { original = x, updated = change(x) })
                    .Aggregate(context, (c, of) => c.WithUpdated(of.original, of.updated));

        //TODO: This example shows we must permit returning a List (not a queryable) for display.
        public static IContext ExtendOffers(this IQueryable<SpecialOffer> offers, DateTime toDate, IContext context) =>
            Change(offers, x => x with { EndDate = toDate, ModifiedDate = context.Now() }, context);


        public static IContext TerminateOffers(
            this IQueryable<SpecialOffer> offers, IContext context)
        {
           var yesterday = context.Today().AddDays(-1);
           return Change(offers, x => x with { EndDate = yesterday, ModifiedDate = context.Now() }, context);
        }
        #endregion
    }
}