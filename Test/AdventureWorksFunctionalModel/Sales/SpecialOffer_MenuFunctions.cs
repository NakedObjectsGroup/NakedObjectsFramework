// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using AW.Types;
using NakedFunctions;

namespace AW.Functions {
    [Named("Special Offers")]
    public static class SpecialOffer_MenuFunctions {
        [MemberOrder(1)]
        [TableView(false, "Description", "XNoMatchingColumn", "Category", "DiscountPct")]
        public static IQueryable<SpecialOffer> CurrentSpecialOffers(IContext context) {
            var today = context.Today();
            var start = new DateTime(2004, 6, 1);
            return AllSpecialOffers(context).Where(x => x.StartDate <= today &&
                                                        x.EndDate >= start);
        }

        //Returns most recently-modified first
        [MemberOrder(2)]
        public static IQueryable<SpecialOffer> AllSpecialOffers(IContext context) =>
            context.Instances<SpecialOffer>().OrderByDescending(so => so.ModifiedDate);

        [MemberOrder(3)]
        public static IQueryable<SpecialOffer> SpecialOffersWithNoMinimumQty(IContext context) =>
            CurrentSpecialOffers(context).Where(s => s.MinQty <= 1);

        //TODO: Annotations & complementary methods

        [MemberOrder(4)]
        public static (SpecialOffer, IContext) CreateNewSpecialOffer(
            [MaxLength(50)] string description,
            decimal discountPct,
            [DefaultValue("Promotion")] string type,
            [DefaultValue("Customer")] string category,
            [DefaultValue(10)] int minQty,
            [DefaultValue(1)] DateTime startDate,
            DateTime endDate,
            IContext context) {
            var so = new SpecialOffer {
                Description = description,
                DiscountPct = discountPct,
                Type = type,
                Category = category,
                MinQty = minQty,
                StartDate = startDate,
                EndDate = endDate,
                ModifiedDate = context.Now(),
                rowguid = context.NewGuid()
            };
            return (so, context.WithNew(so));
        }

        public static IList<string> Choices3CreateNewSpecialOffer() =>
            SpecialOffer_Functions.Categories;

        public static DateTime Default6CreateNewSpecialOffer(IContext context) =>
            SpecialOffer_Functions.DefaultEndDate(context);

        [MemberOrder(5)] [MultiLine(2)]
        public static (SpecialOffer, IContext) CreateMultipleSpecialOffers(
            string description,
            decimal discountPct,
            [DefaultValue("Promotion")] string type,
            [DefaultValue("Customer")] string category,
            [DefaultValue(10)] int minQty,
            [DefaultValue(1)] DateTime startDate,
            [DefaultValue(90)] DateTime endDate,
            IContext context) =>
            CreateNewSpecialOffer(description, discountPct, type, category, minQty, startDate, endDate, context);

        internal static SpecialOffer NoDiscount(IContext context) => context.Instances<SpecialOffer>().Where(x => x.SpecialOfferID == 1).First();
    }
}