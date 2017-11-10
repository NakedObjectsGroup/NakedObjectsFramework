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
using NakedObjects;
using NakedObjects.Services;
using NakedFunctions;

namespace AdventureWorksModel
{
    [DisplayName("Special Offers")]
    public class SpecialOfferRepository : AWAbstractFactoryAndRepository
    {
        #region CurrentSpecialOffers

        [MemberOrder(1)]
        [TableView(false, "Description", "XNoMatchingColumn", "Category", "DiscountPct")]
        public static IQueryable<SpecialOffer> CurrentSpecialOffers(IFunctionalContainer container)
        {
            return from obj in container.Instances<SpecialOffer>()
                   where obj.StartDate <= DateTime.Now &&
                         obj.EndDate >= new DateTime(2004, 6, 1)
                   select obj;
        }

        #endregion

        #region All Special Offers
        //Returns most recently-modified first
        [MemberOrder(2)]
        public static IQueryable<SpecialOffer> AllSpecialOffers(IQueryable<SpecialOffer>  offers)
        {
            return offers.OrderByDescending(so => so.ModifiedDate);
        }
        #endregion

        #region Special Offers With No Minimum Qty
        [MemberOrder(3)]
        public static IQueryable<SpecialOffer> SpecialOffersWithNoMinimumQty(IFunctionalContainer container)
        {
            return CurrentSpecialOffers(container).Where(s => s.MinQty <= 1);
        }
        #endregion

        #region Create New Special Offer
        [MemberOrder(4)]
        public static SpecialOffer CreateNewSpecialOffer()
        {
            var obj = new SpecialOffer();
            //set up any parameters
            //MakePersistent();
            return obj;
        }
        #endregion

        #region Create Multiple Special Offers
        [MemberOrder(5)]
        [MultiLine(NumberOfLines = 2)]
        public static void CreateMultipleSpecialOffers(
            string description,
            [Mask("P")] decimal discountPct,
            string type,
            string category,
            int minQty,
            DateTime startDate
            )
        {
            var so = new SpecialOffer();
            so.Description = description;
            so.DiscountPct = discountPct;
            so.Type = type;
            so.Category = category;
            so.MinQty = minQty;
            //Deliberately created non-current so they don't show up
            //in Current Special Offers (but can be viewed via All Special Offers)
            so.StartDate = startDate;
            so.EndDate = new DateTime(2003, 12, 31);
            Container.Persist(ref so);
        }
        public static string[] Choices3CreateMultipleSpecialOffers()
        {
            return new[] { "Reseller", "Customer" };
        }

        public static string Validate5CreateMultipleSpecialOffers(DateTime startDate)
        {
            var rb = new ReasonBuilder();
            rb.AppendOnCondition(startDate > new DateTime(2003, 12, 1), "Start Date must be before 1/12/2003");
            return rb.Reason;
        }

        #endregion

        #region AssociateSpecialOfferWithProduct

        [MemberOrder(6)]
        public static SpecialOfferProduct AssociateSpecialOfferWithProduct
            ([ContributedAction("Special Offers")] SpecialOffer offer, 
            [ContributedAction("Special Offers")] Product product,
            IFunctionalContainer container)
        {
            //First check if association already exists
            IQueryable<SpecialOfferProduct> query = from sop in container.Instances<SpecialOfferProduct>()
                                                    where sop.SpecialOfferID == offer.SpecialOfferID &&
                                                          sop.ProductID == product.ProductID
                                                    select sop;

            if (query.Count() != 0)
            {
                var t = Container.NewTitleBuilder();
                t.Append(offer).Append(" is already associated with").Append(product);
                WarnUser(t.ToString());
                return null;
            }
            var newSop = new SpecialOfferProduct();
            newSop.SpecialOffer = offer;
            newSop.Product = product;
            //product.SpecialOfferProduct.Add(newSop);
            Persist(ref newSop);
            return newSop;
        }

        [PageSize(20)]
        public static IQueryable<SpecialOffer> AutoComplete0AssociateSpecialOfferWithProduct([MinLength(2)] string name, IQueryable<SpecialOffer> offers)
        {
            return offers.Where(specialOffer => specialOffer.Description.ToUpper().StartsWith(name.ToUpper()));
        }

        [PageSize(20)]
        public static IQueryable<Product> AutoComplete1AssociateSpecialOfferWithProduct([MinLength(2)] string name, IFunctionalContainer container)
        {
            return container.Instances<Product>().Where(product => product.Name.ToUpper().StartsWith(name.ToUpper()));
        }

        #endregion

        #region Helper methods

        [NakedObjectsIgnore]
        public static SpecialOffer NoDiscount(IFunctionalContainer container)
        {
            IQueryable<SpecialOffer> query = from obj in container.Instances<SpecialOffer>()
                                             where obj.SpecialOfferID == 1
                                             select obj;

            return query.FirstOrDefault();
        }

        #endregion

    }
}