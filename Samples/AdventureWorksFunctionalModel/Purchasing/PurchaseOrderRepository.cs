// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedFunctions;
using NakedObjects;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using static AdventureWorksModel.CommonFactoryAndRepositoryFunctions;
using static NakedFunctions.Result;

namespace AdventureWorksModel
{
    [DisplayName("Purchase Orders")]
    public static class PurchaseOrderRepository
    {
        [Description("For demonstration purposes only")]
        
        [MemberOrder(1)]
        public static PurchaseOrderHeader RandomPurchaseOrder(
            [Injected] IQueryable<PurchaseOrderHeader> pohs, 
            [Injected] int random)
        {
            return Random(pohs, random);
        }

        //returns most recently-modified first
        [MemberOrder(2)]
        public static IQueryable<PurchaseOrderHeader> AllPurchaseOrders(
            [Injected] IQueryable<PurchaseOrderHeader> pohs)
        {
            return pohs.OrderByDescending(poh => poh.ModifiedDate);
        }

        [MemberOrder(2)]
        [TableView(true, "Vendor", "OrderDate", "TotalDue")]
        public static IQueryable<PurchaseOrderHeader> AllOpenPurchaseOrders(
             [Injected] IQueryable<PurchaseOrderHeader> pohs)
        {
            return pohs.Where(poh => poh.Status <= 2);
        }

        #region OpenPurchaseOrdersForVendor

        [MemberOrder(3)]
        [TableView(true, "OrderDate", "TotalDue")]
        public static IQueryable<PurchaseOrderHeader> OpenPurchaseOrdersForVendor(
            [ContributedAction("Purchase Orders")] Vendor vendor,
             [Injected] IQueryable<PurchaseOrderHeader> pohs)
        {
            return AllOpenPurchaseOrders(pohs).Where(poh => poh.Vendor.BusinessEntityID == vendor.BusinessEntityID);
        }

        [PageSize(20)]
        public static IQueryable<Vendor> AutoComplete0OpenPurchaseOrdersForVendor(
            [MinLength(2)] string name,
            [Injected] IQueryable<Vendor> vendors)
        {
            return vendors.Where(v => v.Name.ToUpper().StartsWith(name.ToUpper()));
        }

        #endregion

        #region ListPurchaseOrdersForVendor

        [MemberOrder(4)]
        [TableView(true, "OrderDate", "Status", "TotalDue")]
        public static IQueryable<PurchaseOrderHeader> ListPurchaseOrders(
            [ContributedAction("Purchase Orders")] Vendor vendor,
            DateTime? fromDate, 
            DateTime? toDate,
            [Injected] IQueryable<PurchaseOrderHeader> pohs)
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
            [MinLength(2)] string name,
            [Injected] IQueryable<Vendor> vendors)
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

        [MemberOrder(5)]
        [TableView(true, "Vendor", "OrderDate", "Status")]
        public static IQueryable<PurchaseOrderHeader> OpenPurchaseOrdersForProduct(
            [ContributedAction("Purchase Orders")] Product product,
            [Injected] IQueryable<PurchaseOrderDetail> details)
        {
            return from obj in details
                   where obj.Product.ProductID == product.ProductID &&
                         obj.PurchaseOrderHeader.Status <= 2
                   select obj.PurchaseOrderHeader;
        }

        [PageSize(20)]
        public static IQueryable<Product> AutoComplete0OpenPurchaseOrdersForProduct(
            [MinLength(2)] string name,
            [Injected] IQueryable<Product> products)
        {
            return products.Where(product => product.Name.ToUpper().StartsWith(name.ToUpper()));
        }

        // for autoautocomplete testing
        public static IQueryable<PurchaseOrderHeader> OpenPurchaseOrdersForVendorAndProduct(
            Vendor vendor, 
            Product product,
            [Injected] IQueryable<PurchaseOrderDetail> details)
        {
            return from obj in details
                   where obj.Product.ProductID == product.ProductID &&
                         obj.PurchaseOrderHeader.Status <= 2 &&
                         obj.PurchaseOrderHeader.Vendor.BusinessEntityID == vendor.BusinessEntityID
                   select obj.PurchaseOrderHeader;
        }


        #endregion

        #region Create New Purchase Order
        [MemberOrder(6)]
        public static (PurchaseOrderHeader, PurchaseOrderHeader) CreateNewPurchaseOrder(
            [ContributedAction("Purchase Orders")] Vendor vendor)
        {
            //TODO: set up with all required fields
            var poh = new PurchaseOrderHeader();
            poh.Vendor = vendor;
            poh.ShipDate = DateTime.Today;
            return DisplayAndPersist(poh);
        }

        [PageSize(20)]
        public static IQueryable<Vendor> AutoComplete0CreateNewPurchaseOrder(
            [MinLength(2)] string name,
            [Injected] IQueryable<Vendor> vendors)
        {
            return vendors.Where(v => v.Name.ToUpper().StartsWith(name.ToUpper()));
        }

        [MemberOrder(7)]
        public static (PurchaseOrderHeader, PurchaseOrderHeader) CreateNewPurchaseOrder2()
        {
            //TODO: required properties
            var poh =  new PurchaseOrderHeader();
            poh.OrderPlacedBy = null;
            return DisplayAndPersist(poh);
        }

        #endregion

        #region FindById
        [MemberOrder(7)]
        public static PurchaseOrderHeader FindById(
            int id,
            [Injected] IQueryable<PurchaseOrderHeader> headers)
        {
            return headers.Where(x => x.PurchaseOrderID == id).FirstOrDefault();
        }
        #endregion
    }
}