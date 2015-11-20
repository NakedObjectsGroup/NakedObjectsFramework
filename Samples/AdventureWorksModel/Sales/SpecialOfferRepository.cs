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

namespace AdventureWorksModel {
    [DisplayName("Special Offers")]
    public class SpecialOfferRepository : AbstractFactoryAndRepository {
        #region CurrentSpecialOffers

        [FinderAction]
        [MemberOrder(1)]
        [TableView(false, "Description", "Category", "DiscountPct")]
        public IQueryable<SpecialOffer> CurrentSpecialOffers() {
            return from obj in Instances<SpecialOffer>()
                where obj.StartDate <= DateTime.Now &&
                      obj.EndDate >= new DateTime(2004, 6, 1)
                select obj;
        }

        #endregion

        public IQueryable<SpecialOffer> SpecialOffersWithNoMinimumQty()
        {
            return CurrentSpecialOffers().Where(s => s.MinQty <= 1);
        }

        [FinderAction]
        [MemberOrder(3)]
        public SpecialOffer CreateNewSpecialOffer() {
            var obj = NewTransientInstance<SpecialOffer>();
            //set up any parameters
            //MakePersistent();
            return obj;
        }

        #region NoDiscount

        private SpecialOffer _noDiscount;

        [NakedObjectsIgnore]
        public SpecialOffer NoDiscount() {
            if (_noDiscount == null) {
                IQueryable<SpecialOffer> query = from obj in Instances<SpecialOffer>()
                    where obj.SpecialOfferID == 1
                    select obj;

                _noDiscount = query.FirstOrDefault();
            }
            return _noDiscount;
        }

        #endregion

        #region AssociateSpecialOfferWithProduct

        [MemberOrder(2)]
        public SpecialOfferProduct AssociateSpecialOfferWithProduct([ContributedAction("Special Offers")] SpecialOffer offer, [ContributedAction("Special Offers")] Product product) {
            //First check if association already exists
            IQueryable<SpecialOfferProduct> query = from sop in Instances<SpecialOfferProduct>()
                where sop.SpecialOfferID == offer.SpecialOfferID &&
                      sop.ProductID == product.ProductID
                select sop;

            if (query.Count() != 0) {
                var t = Container.NewTitleBuilder();
                t.Append(offer).Append(" is already associated with").Append(product);
                WarnUser(t.ToString());
                return null;
            }
            var newSop = NewTransientInstance<SpecialOfferProduct>();
            newSop.SpecialOffer = offer;
            newSop.Product = product;
            //product.SpecialOfferProduct.Add(newSop);
            Persist(ref newSop);
            return newSop;
        }

        [PageSize(20)]
        public IQueryable<SpecialOffer> AutoComplete0AssociateSpecialOfferWithProduct([MinLength(2)] string name) {
            return Container.Instances<SpecialOffer>().Where(specialOffer => specialOffer.Description.ToUpper().StartsWith(name.ToUpper()));
        }

        [PageSize(20)]
        public IQueryable<Product> AutoComplete1AssociateSpecialOfferWithProduct([MinLength(2)] string name) {
            return Container.Instances<Product>().Where(product => product.Name.ToUpper().StartsWith(name.ToUpper()));
        }

        #endregion
    }
}