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
using static NakedFunctions.Helpers;

namespace AdventureWorksModel
{


    public static class SpecialOffer_Functions
    {

        #region Life Cycle Methods
        public static SpecialOffer Updating(SpecialOffer x, [Injected] DateTime now) => x with { ModifiedDate = now };

        public static SpecialOffer Persisting(SpecialOffer x, [Injected] DateTime now, [Injected] Guid guid) => x with { ModifiedDate = now, rowguid = guid };
        #endregion

        #region Edit
        [Edit]
        public static (SpecialOffer, IContainer) EditDescription(this SpecialOffer sp, string description, IContainer container)
        => DisplayAndPersist(sp with { Description = description }, container);

        [Edit]
        public static (SpecialOffer, IContainer) EditDiscount(this SpecialOffer sp, decimal discountPct, IContainer container)
        => DisplayAndPersist(sp with { DiscountPct = discountPct }, container);

        [Edit]
        public static (SpecialOffer, IContainer) EditType(this SpecialOffer sp, string type, IContainer container)
        => DisplayAndPersist(sp with { Type = type }, container);

        [Edit]
        public static (SpecialOffer, IContainer) EditCategory(this SpecialOffer sp, string category, IContainer container)
        => DisplayAndPersist(sp with { Category = category }, container);

        public static string[] Choices0Category(this SpecialOffer sp) => new[] { "Reseller", "Customer" };

        [Edit]
        public static (SpecialOffer, IContainer) EditDates(this SpecialOffer sp, DateTime startDate, DateTime endDate, IContainer container)
        => DisplayAndPersist(sp with { StartDate = startDate, EndDate = endDate }, container);

        public static DateTime Default0EditDates(this SpecialOffer sp, IContainer container) => container.GetService<IClock>().Today();

        public static DateTime Default1EditDates(this SpecialOffer sp, IContainer container) => container.GetService<IClock>().Today().AddDays(90);

        [Edit]
        public static (SpecialOffer, IContainer) EditQuantities(this SpecialOffer sp, [DefaultValue(1)] int minQty, [Optionally] int? maxQty, IContainer container)
=> DisplayAndPersist(sp with { MinQty = minQty, MaxQty = maxQty }, container);

        public static string ValidateEditQuantities(this SpecialOffer sp, [DefaultValue(1)] int minQty, [Optionally] int? maxQty)
=> minQty >= 1 && maxQty is null || maxQty.Value >= minQty ? null : "Quanties invalid";
        #endregion

        #region AssociateWithProduct

        public static (SpecialOfferProduct, IContainer) AssociateWithProduct(
        this SpecialOffer offer,
        Product product,
        IContainer container
            )
        {
            //First check if association already exists
            IQueryable<SpecialOfferProduct> query = from sop in container.Instances<SpecialOfferProduct>()
                                                    where sop.SpecialOfferID == offer.SpecialOfferID &&
                                                    sop.ProductID == product.ProductID
                                                    select sop;

            if (query.Count() != 0)
            {

                Action<IAlert> msg = InformUser($"{offer} is already associated with { product}");
                return (null, container.WithOutput(msg));
            }
            var newSop = new SpecialOfferProduct() with
            {
                SpecialOffer = offer,
                Product = product
            };
            return (newSop, container.WithPendingSave(newSop));
        }

        [PageSize(20)]
        public static IQueryable<Product> AutoComplete1AssociateWithProduct([Range(2, 0)] string name, IContainer container)
            => container.Instances<Product>().Where(product => product.Name.ToUpper().StartsWith(name.ToUpper()));

        #endregion

        #region Queryable-contributed
        private static (IList<SpecialOffer>, IList<SpecialOffer>) Change(this IQueryable<SpecialOffer> offers, Func<SpecialOffer, SpecialOffer> change)
=> DisplayAndPersist(offers.ToList().Select(change).ToList());

        //TODO: This example shows we must permit returning a List (not a queryable) for display.
        public static (IList<SpecialOffer>, IList<SpecialOffer>) ExtendOffers(this IQueryable<SpecialOffer> offers, DateTime toDate)
        => Change(offers, x => x with { EndDate = toDate });


        public static (IList<SpecialOffer>, IList<SpecialOffer>) TerminateActiveOffers(
            this IQueryable<SpecialOffer> offers,
            [Injected] DateTime now)
        {
            var yesterday = now.Date.AddDays(-1);
            var list = offers.Where(x => x.EndDate > yesterday).ToList().Select(x => x with { EndDate = yesterday }).ToList();
            return DisplayAndPersist(list);
        }
        #endregion
    }
}