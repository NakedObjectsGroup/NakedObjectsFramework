// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedFunctions;
using AW.Types;

namespace AW.Functions {

    public static class SalesOrderDetail_Functions
    {
        public static IContext ChangeQuantity(this SalesOrderDetail detail, short newQuantity, IContext context)
        {
            var sop = Product_Functions.BestSpecialOfferProduct(detail.Product, newQuantity, context);
            var detail2 = detail with
            {
                OrderQty = newQuantity,
                SpecialOfferProduct = sop,
                UnitPrice = detail.Product.ListPrice,
                UnitPriceDiscount = sop.SpecialOffer.DiscountPct,
                ModifiedDate = context.Now()
            };
            return context.WithUpdated(detail, detail2).WithDeferred(
                c => {
                    var soh = c.Resolve(detail.SalesOrderHeader);
                    return c.WithUpdated(soh, soh.Recalculated(c));
                    }
                );
        }

        public static string DisableChangeQuantity(this SalesOrderDetail detail) =>
            detail.SalesOrderHeader.DisableAddNewDetail();
    }
}