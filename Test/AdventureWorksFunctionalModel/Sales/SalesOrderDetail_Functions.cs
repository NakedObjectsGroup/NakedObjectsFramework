// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedFunctions;
using AW.Types;

namespace AW.Functions {

    public static class SalesOrderDetail_Functions
    {
        //Call this from any function that updates a SalesOrderDetail
        internal static SalesOrderDetail WithRecalculatedFields(this SalesOrderDetail sod, IContext context)
        {
            var unitPrice = sod.SpecialOfferProduct.Product.ListPrice;
            var discount = sod.SpecialOfferProduct.SpecialOffer.DiscountPct * unitPrice;
            var lineTotal = (unitPrice - discount) * sod.OrderQty;

            return sod with
            {
                UnitPrice = unitPrice,
                UnitPriceDiscount = discount,
                LineTotal = lineTotal,
                ModifiedDate = context.Now()
            };
        }

        public static  IContext ChangeQuantity(this SalesOrderDetail detail, short newQuantity, IContext context)
        {
            var detail2 = (detail with
            {
                OrderQty = newQuantity,
                SpecialOfferProduct = Product_Functions.BestSpecialOfferProduct(detail.Product, newQuantity, context)
            }).WithRecalculatedFields(context);

            return context.WithUpdated(detail, detail2).WithPostSaveFunction(detail.SalesOrderHeader.Recalculate());
        }

        public static string DisableChangeQuantity(this SalesOrderDetail detail) =>
            detail.SalesOrderHeader.DisableAddNewDetail();


    }
}