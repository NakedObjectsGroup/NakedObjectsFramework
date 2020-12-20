// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedFunctions;
using System;
using System.Linq;
using static AdventureWorksModel.Helpers;


namespace AdventureWorksModel
{

    public static class PurchaseOrder_MenuFunctions
    {
        [DescribedAs("For demonstration purposes only")]
        
        [MemberOrder(1)]
        public static PurchaseOrderHeader RandomPurchaseOrder(
            IQueryable<PurchaseOrderHeader> pohs, 
            [Injected] int random)
        {
            return Random(pohs, random);
        }

        //returns most recently-modified first
        [MemberOrder(2)]
        public static IQueryable<PurchaseOrderHeader> AllPurchaseOrders(
            IQueryable<PurchaseOrderHeader> pohs)
        {
            return pohs.OrderByDescending(poh => poh.ModifiedDate);
        }

        [MemberOrder(2)]
        [TableView(true, "Vendor", "OrderDate", "TotalDue")]
        public static IQueryable<PurchaseOrderHeader> AllOpenPurchaseOrders(
             IQueryable<PurchaseOrderHeader> pohs)
        {
            return pohs.Where(poh => poh.Status <= 2);
        }

        #region OpenPurchaseOrdersForVendor

        [MemberOrder(3)]
        [TableView(true, "OrderDate", "TotalDue")]
        public static IQueryable<PurchaseOrderHeader> OpenPurchaseOrdersForVendor(
            this Vendor vendor,
             IQueryable<PurchaseOrderHeader> pohs)
        {
            return AllOpenPurchaseOrders(pohs).Where(poh => poh.Vendor.BusinessEntityID == vendor.BusinessEntityID);
        }

        [PageSize(20)]
        public static IQueryable<Vendor> AutoComplete0OpenPurchaseOrdersForVendor(
            [Range(2,0)] string name,
            IQueryable<Vendor> vendors)
        {
            return vendors.Where(v => v.Name.ToUpper().StartsWith(name.ToUpper()));
        }

        #endregion

        #region ListPurchaseOrdersForVendor

        [MemberOrder(4,"Purchase Orders")]
        [TableView(true, "OrderDate", "Status", "TotalDue")]
        public static IQueryable<PurchaseOrderHeader> ListPurchaseOrders(
            Vendor vendor,
            DateTime? fromDate, 
            DateTime? toDate,
            IQueryable<PurchaseOrderHeader> pohs)
        {
            return from obj in pohs
                   where obj.Vendor.BusinessEntityID == vendor.BusinessEntityID &&
                   (fromDate == null || obj.OrderDate >= fromDate) &&
                   (toDate == null || obj.OrderDate <= toDate)
                   orderby obj.OrderDate
                   select obj;
        }

        [PageSize(20)]
        public static IQueryable<Vendor> AutoComplete0ListPurchaseOrders(
            [Range(2,0)] string name,
            IQueryable<Vendor> vendors)
        {
            return vendors.Where(v => v.Name.ToUpper().StartsWith(name.ToUpper()));
        }

        //TODO: Should match action, ignoring any injected properties, on either action or this
        public static string ValidateListPurchaseOrders(
            Vendor vendor, 
            DateTime? fromDate, 
            DateTime? toDate)
        {
            return toDate.IsAtLeastOneDayBefore(fromDate) ? "To Date cannot be before From Date" : null;
        }
        #endregion

        #region OpenPurchaseOrdersForProduct

        [MemberOrder(5, "Purchase Orders")]
        [TableView(true, "Vendor", "OrderDate", "Status")]
        public static IQueryable<PurchaseOrderHeader> OpenPurchaseOrdersForProduct(
             this Product product, IContainer container) => from obj in container.Instances<PurchaseOrderDetail>()
                   where obj.Product.ProductID == product.ProductID &&
                         obj.PurchaseOrderHeader.Status <= 2
                   select obj.PurchaseOrderHeader;
   

        [PageSize(20)]
        public static IQueryable<Product> AutoComplete0OpenPurchaseOrdersForProduct(
            [Range(2,0)] string name,
            IQueryable<Product> products)
        {
            return products.Where(product => product.Name.ToUpper().StartsWith(name.ToUpper()));
        }

        // for autoautocomplete testing
        public static IQueryable<PurchaseOrderHeader> OpenPurchaseOrdersForVendorAndProduct(
            Vendor vendor, Product product, IContainer container) =>
                    from obj in container.Instances<PurchaseOrderDetail>()
                   where obj.Product.ProductID == product.ProductID &&
                         obj.PurchaseOrderHeader.Status <= 2 &&
                         obj.PurchaseOrderHeader.Vendor.BusinessEntityID == vendor.BusinessEntityID
                   select obj.PurchaseOrderHeader;


        #endregion

        #region Create New Purchase Order
        [MemberOrder(6, "Purchase Orders")]
        public static (PurchaseOrderHeader, IContainer) CreateNewPurchaseOrder(this Vendor vendor, IContainer container) =>
            DisplayAndSave(new PurchaseOrderHeader() { Vendor = vendor, ShipDate = container.Today() }, container);


        [PageSize(20)]
        public static IQueryable<Vendor> AutoComplete0CreateNewPurchaseOrder(
            [Range(2,0)] string name, IContainer container) =>
            container.Instances<Vendor>().Where(v => v.Name.ToUpper().StartsWith(name.ToUpper()));

        [MemberOrder(7)]
        public static (PurchaseOrderHeader, IContainer) CreateNewPurchaseOrder2(IContainer container) =>
             DisplayAndSave(new PurchaseOrderHeader() { OrderPlacedBy = null }, container);

        #endregion

        #region FindById
        [MemberOrder(7)]
        public static PurchaseOrderHeader FindById(
            int id,
            IQueryable<PurchaseOrderHeader> headers)
        {
            return headers.Where(x => x.PurchaseOrderID == id).FirstOrDefault();
        }
        #endregion
    }
}