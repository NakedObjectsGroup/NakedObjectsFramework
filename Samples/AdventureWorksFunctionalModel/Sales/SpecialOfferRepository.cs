// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using AdventureWorksFunctionalModel.Functions;
using NakedFunctions;
using NakedObjects;
using NakedObjects.Services;

namespace AdventureWorksModel {
    [DisplayName("Special Offers")]
    public static class SpecialOfferRepository {
        #region CurrentSpecialOffers

        [MemberOrder(1)]
        [TableView(false, "Description", "XNoMatchingColumn", "Category", "DiscountPct")] 
        public static IQueryable<SpecialOffer> CurrentSpecialOffers( [Injected] IQueryable<SpecialOffer> specialOffers) {
            return from obj in specialOffers
                where obj.StartDate <= DateTime.Now &&
                      obj.EndDate >= new DateTime(2004, 6, 1)
                select obj;
        }

        #endregion

        #region All Special Offers
        //Returns most recently-modified first
        [MemberOrder(2)]
        public static IQueryable<SpecialOffer> AllSpecialOffers( [Injected] IQueryable<SpecialOffer> specialOffers)
        {
            return specialOffers.OrderByDescending(so => so.ModifiedDate);
        }
        #endregion

        #region Special Offers With No Minimum Qty
        [MemberOrder(3)]
        public static IQueryable<SpecialOffer> SpecialOffersWithNoMinimumQty( [Injected] IQueryable<SpecialOffer> specialOffers)
        {
            return CurrentSpecialOffers(specialOffers).Where(s => s.MinQty <= 1);
        }
        #endregion

        #region Create New Special Offer
        [MemberOrder(4)]
        public static (SpecialOffer, SpecialOffer) CreateNewSpecialOffer(
            string description,
            decimal discountPct,
            string type,
            string category,
            DateTime startDate,
            DateTime endDate,
            [DefaultValue(1)] int minQty,
            [Optionally] int? maxQty,
            [Injected] DateTime now,
            [Injected] Guid guid
            ) {

            return Result.DisplayAndPersist(new SpecialOffer(0, description, discountPct, type, category, startDate, endDate, minQty, maxQty, now, guid));
        }

        public static DateTime Default4CreateNewSpecialOffer([Injected] DateTime now)
        {
            return now.Date;
        }

        public static DateTime Default5CreateNewSpecialOffer([Injected] DateTime now)
        {
            return now.Date.AddMonths(1);
        }

        #endregion

        #region Create Multiple Special Offers
        [MemberOrder(5)]
        [MultiLine(NumberOfLines=2)]
        public static (SpecialOffer, SpecialOffer) CreateMultipleSpecialOffers(
            
            string description,
            [Mask("P")] decimal discountPct,
            string type,
            string category,
            int minQty,
            DateTime startDate
            )
        {
            var so = new SpecialOffer();  //TODO -  use full constructor
            so.Description = description;
            so.DiscountPct = discountPct;
            so.Type = type;
            so.Category = category;
            so.MinQty = minQty;
            //Deliberately created non-current so they don't show up
            //in Current Special Offers (but can be viewed via All Special Offers)
            so.StartDate = startDate;
            so.EndDate = new DateTime(2003, 12, 31);
            return Result.DisplayAndPersist(so);
        }

        public static string[] Choices3CreateMultipleSpecialOffers()
        {
            return new[] { "Reseller", "Customer" };
        }

        public static string Validate5CreateMultipleSpecialOffers(DateTime startDate)
        {
            return startDate > new DateTime(2003,12,1)? "Start Date must be before 1/12/2003": null;
        }

        #endregion

        #region AssociateSpecialOfferWithProduct

        [MemberOrder(6)]
        public static (SpecialOfferProduct, SpecialOfferProduct, string) AssociateSpecialOfferWithProduct(
            
            [ContributedAction("Special Offers")] SpecialOffer offer, 
            [ContributedAction("Special Offers")] Product product,
            [Injected] IQueryable<SpecialOfferProduct> sops
            ) {
            //First check if association already exists
            IQueryable<SpecialOfferProduct> query = from sop in sops
                      where sop.SpecialOfferID == offer.SpecialOfferID &&
                      sop.ProductID == product.ProductID
                select sop;

            if (query.Count() != 0) {

                string msg = $"{offer} is already associated with { product}"; //TODO: sort titles
                return Result.DisplayAndPersist<SpecialOfferProduct>(null, msg);
            }
            var newSop = new SpecialOfferProduct();  //TODO use proper constructor
            newSop.SpecialOffer = offer;
            newSop.Product = product;
            return Result.DisplayAndPersist(newSop, null);
        }

        [PageSize(20)]
        public static IQueryable<SpecialOffer> AutoComplete0AssociateSpecialOfferWithProduct(
            [MinLength(2)] string name,
            [Injected] IQueryable<SpecialOffer> offers) {
            return offers.Where(specialOffer => specialOffer.Description.ToUpper().StartsWith(name.ToUpper()));
        }

        [PageSize(20)]
        public static IQueryable<Product> AutoComplete1AssociateSpecialOfferWithProduct(
            [MinLength(2)] string name,
            [Injected] IQueryable<Product> products
            ) {
            return products.Where(product => product.Name.ToUpper().StartsWith(name.ToUpper()));
        }

        #endregion

        #region Helper methods

        [NakedObjectsIgnore]
        public static SpecialOffer NoDiscount(IQueryable<SpecialOffer> offers)
        {
            return offers.Where(x => x.SpecialOfferID == 1).First();
        }

        #endregion

    }
}