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
        #region Life Cycle methods

        public static void Persisted(this SalesOrderDetail sod)
        {
            //TODO:
            //SalesOrderHeader.Details.Add(this);
            //SalesOrderHeader.Recalculate();
        }

        #endregion

        public static (SalesOrderDetail, IContext) Recalculate(this SalesOrderDetail sod)
        {
            throw new NotImplementedException();
            //UnitPrice = SpecialOfferProduct.Product.ListPrice;
            //UnitPriceDiscount = (SpecialOfferProduct.SpecialOffer.DiscountPct * UnitPrice);
            //LineTotal = (UnitPrice - UnitPriceDiscount) * OrderQty;
            //SalesOrderHeader.Recalculate();
        }

        public static (SalesOrderDetail, IContext) ChangeQuantity(this SalesOrderDetail sod, short newQuantity, IContext context)
        {
            throw new NotImplementedException();
            //OrderQty = newQuantity;
            //            IQueryable<SpecialOfferProduct> sops
            //SpecialOfferProduct = ProductFunctions2.BestSpecialOfferProduct(Product, newQuantity, sops);
            //Recalculate();
        }

        public static string DisableChangeQuantity()
        {
            throw new NotImplementedException();
            //return SalesOrderHeader.DisableAddNewDetail();
        }
    }
}