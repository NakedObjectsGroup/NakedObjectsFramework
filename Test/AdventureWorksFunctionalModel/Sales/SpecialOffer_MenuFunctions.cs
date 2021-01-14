// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using NakedFunctions;
using AW.Types;
using static AW.Helpers;

namespace AW.Functions
{
    [Named("Special Offers")]
    public static class SpecialOffer_MenuFunctions
    {
        [MemberOrder(1)]
        [TableView(false, "Description", "XNoMatchingColumn", "Category", "DiscountPct")]
        public static IQueryable<SpecialOffer> CurrentSpecialOffers(IContext context)
        {
            var today = context.Today();
           return AllSpecialOffers(context).Where(x => x.StartDate <= today &&
                         x.EndDate >= new DateTime(2004, 6, 1));
        }

        //Returns most recently-modified first
        [MemberOrder(2)]
        public static IQueryable<SpecialOffer> AllSpecialOffers(IContext context) =>
         context.Instances<SpecialOffer>().OrderByDescending(so => so.ModifiedDate);

        [MemberOrder(3)]
        public static IQueryable<SpecialOffer> SpecialOffersWithNoMinimumQty(IContext context) =>
            CurrentSpecialOffers(context).Where(s => s.MinQty <= 1);


        [MemberOrder(4), CreateNew]
        public static (SpecialOffer, IContext) CreateNewSpecialOffer(string description, IContext context)
       => DisplayAndSave(new SpecialOffer { Description = description, ModifiedDate = context.Now(), rowguid = context.NewGuid() }, context);

        #region Create Multiple Special Offers
        [MemberOrder(5), MultiLine(2)]
        public static (SpecialOffer, IContext) CreateMultipleSpecialOffers(
            string description,
            [Mask("P")] decimal discountPct,
            string type,
            string category,
            int minQty,
            DateTime startDate,
            IContext context) =>
                DisplayAndSave(new SpecialOffer() with
                {
                    Description = description,
                    DiscountPct = discountPct,
                    Type = type,
                    Category = category,
                    MinQty = minQty,
                    //Deliberately created non-current so they don't show up
                    //in Current Special Offers (but can be viewed via All Special Offers)
                    StartDate = startDate,
                    EndDate = new DateTime(2003, 12, 31),
                    ModifiedDate = context.Now(),
                    rowguid = context.NewGuid()
                }, context);


        public static string[] Choices3CreateMultipleSpecialOffers() => new[] { "Reseller", "Customer" };

        public static string Validate5CreateMultipleSpecialOffers(DateTime startDate) 
            => startDate > new DateTime(2003, 12, 1) ? "Start Date must be before 1/12/2003" : null;
        #endregion

        internal static SpecialOffer NoDiscount(IContext context) => context.Instances<SpecialOffer>().Where(x => x.SpecialOfferID == 1).First();
    }
}