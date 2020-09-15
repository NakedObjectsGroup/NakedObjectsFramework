// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using NakedFunctions;
using static NakedFunctions.Helpers;

namespace AdventureWorksModel {


    public static class SpecialOffer_Functions
    {

        #region Life Cycle Methods
        public static SpecialOffer Updating(SpecialOffer x, [Injected] DateTime now) => x with {ModifiedDate =  now};

        public static SpecialOffer Persisting(SpecialOffer x, [Injected] DateTime now, [Injected] Guid guid) => x with { ModifiedDate = now, rowguid = guid };
        #endregion

        #region Edit
        public static SpecialOffer_Edit Edit(this SpecialOffer x) => SpecialOffer_Edit_Functions.CreateFrom(x);
        #endregion

        #region AssociateWithProduct

       //Helper method
        public static (SpecialOfferProduct, SpecialOfferProduct, Action<IAlert>) AssociateWithProduct(
        this SpecialOffer offer,
        Product product,
            IQueryable<SpecialOfferProduct> sops
            )
        {
            //First check if association already exists
            IQueryable<SpecialOfferProduct> query = from sop in sops
                                                    where sop.SpecialOfferID == offer.SpecialOfferID &&
                                                    sop.ProductID == product.ProductID
                                                    select sop;

            if (query.Count() != 0)
            {

                Action<IAlert> msg = InformUser($"{offer} is already associated with { product}");
                return (null, null, msg);
            }
            var newSop = new SpecialOfferProduct() with
            {
                SpecialOffer = offer,
                Product = product
            };
            return (newSop, newSop, null);
        }

        [PageSize(20)]
        public static IQueryable<Product> AutoComplete1AssociateWithProduct([Range(2, 0)] string name, IQueryable<Product> products)
            => products.Where(product => product.Name.ToUpper().StartsWith(name.ToUpper()));

        #endregion
    }
}