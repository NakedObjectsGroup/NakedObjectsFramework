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
using static AdventureWorksModel.Helpers;

namespace AdventureWorksModel
{


    public static class SpecialOffer_Functions
    {

        #region Life Cycle Methods
        public static SpecialOffer Updating(SpecialOffer x,  DateTime now) => x with { ModifiedDate = now };

        public static SpecialOffer Persisting(SpecialOffer x,  DateTime now,  Guid guid) => x with { ModifiedDate = now, rowguid = guid };
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
        this SpecialOffer offer,
        Product product,
        IContext context
            )
        {
            //First check if association already exists
            IQueryable<SpecialOfferProduct> query = from sop in context.Instances<SpecialOfferProduct>()
                                                    where sop.SpecialOfferID == offer.SpecialOfferID &&
                                                    sop.ProductID == product.ProductID
                                                    select sop;

            if (query.Count() != 0)
            {

                Action<IAlert> msg = InformUser($"{offer} is already associated with { product}");
                return (null, context.WithAction(msg));
            }
            var newSop = new SpecialOfferProduct() with
            {
                SpecialOffer = offer,
                Product = product
            };
            return (newSop, context.WithPendingSave(newSop));
        }

        [PageSize(20)]
        public static IQueryable<Product> AutoComplete1AssociateWithProduct([Range(2, 0)] string name, IContext context)
            => context.Instances<Product>().Where(product => product.Name.ToUpper().StartsWith(name.ToUpper()));

        #endregion

        #region Queryable-contributed
        private static (IList<SpecialOffer>, IList<SpecialOffer>) Change(this IQueryable<SpecialOffer> offers, Func<SpecialOffer, SpecialOffer> change)
=> DisplayAndPersist(offers.ToList().Select(change).ToList());

        //TODO: This example shows we must permit returning a List (not a queryable) for display.
        public static (IList<SpecialOffer>, IList<SpecialOffer>) ExtendOffers(this IQueryable<SpecialOffer> offers, DateTime toDate)
        => Change(offers, x => x with { EndDate = toDate });


        public static (IList<SpecialOffer>, IList<SpecialOffer>) TerminateActiveOffers(
            this IQueryable<SpecialOffer> offers,
             DateTime now)
        {
            var yesterday = now.Date.AddDays(-1);
            var list = offers.Where(x => x.EndDate > yesterday).ToList().Select(x => x with { EndDate = yesterday }).ToList();
            return DisplayAndPersist(list);
        }
        #endregion
    }
}