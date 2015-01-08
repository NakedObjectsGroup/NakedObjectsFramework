// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using NakedObjects.Services;
using NakedObjects;

namespace AdventureWorksModel {
    [DisplayName("Purchase Orders")]
    public class PurchaseOrderRepository : AbstractFactoryAndRepository {
        #region Injected Services

        // This region should contain properties to hold references to any services required by the
        // object.  Use the 'injs' shortcut to add a new service.

        #endregion

        #region OpenPurchaseOrdersForVendor

        [TableView(true, "OrderDate", "TotalDue")]
        public IQueryable<PurchaseOrderHeader> OpenPurchaseOrdersForVendor([ContributedAction("Purchase Orders")] Vendor vendor)
        {
            return from obj in Instances<PurchaseOrderHeader>()
                                                    where obj.Vendor.VendorID == vendor.VendorID && obj.Status <= 2
                                                    select obj;
        }

        [PageSize(20)]
        public IQueryable<Vendor> AutoComplete0OpenPurchaseOrdersForVendor([MinLength(2)] string name) {
            return Container.Instances<Vendor>().Where(v => v.Name.ToUpper().StartsWith(name.ToUpper()));
        }


        #endregion

        #region ListPurchaseOrdersForVendor

        [TableView(true, "OrderDate", "Status", "TotalDue")]
        public IQueryable<PurchaseOrderHeader> ListPurchaseOrders([ContributedAction("Purchase Orders")]Vendor vendor, DateTime? fromDate, DateTime? toDate)
        {
            IQueryable<PurchaseOrderHeader> query = from obj in Instances<PurchaseOrderHeader>()
                                                    where obj.Vendor.VendorID == vendor.VendorID &&
                                                          (fromDate == null || obj.OrderDate >= fromDate) &&
                                                          (toDate == null || obj.OrderDate <= toDate)
                                                    orderby obj.OrderDate
                                                    select obj;

            return query;
        }

        [PageSize(20)]
        public IQueryable<Vendor> AutoComplete0ListPurchaseOrders([MinLength(2)] string name) {
            return Container.Instances<Vendor>().Where(v => v.Name.ToUpper().StartsWith(name.ToUpper()));
        }

        #endregion

        #region OpenPurchaseOrdersForProduct

         [TableView(true, "Vendor", "OrderDate", "Status")]
        public IQueryable<PurchaseOrderHeader> OpenPurchaseOrdersForProduct([ContributedAction("Purchase Orders")]Product product)
        {
            return from obj in Instances<PurchaseOrderDetail>()
                                                    where obj.Product.ProductID == product.ProductID &&
                                                          obj.PurchaseOrderHeader.Status <= 2
                                                    select obj.PurchaseOrderHeader;
        }

         [PageSize(20)]
         public IQueryable<Product> AutoComplete0OpenPurchaseOrdersForProduct([MinLength(2)] string name) {
             return Container.Instances<Product>().Where(product => product.Name.ToUpper().StartsWith(name.ToUpper()));
         }

        #endregion

        #region RandomPurchaseOrder
        [FinderAction]
        [QueryOnly]
        public PurchaseOrderHeader RandomPurchaseOrder() {
            return Random<PurchaseOrderHeader>();
        }

        #endregion

        [FinderAction]
        public PurchaseOrderHeader CreateNewPurchaseOrder([ContributedAction("Purchase Orders")]Vendor vendor) {
            var purchaseOrderHeader = NewTransientInstance<PurchaseOrderHeader>();
            purchaseOrderHeader.Vendor = vendor;
            //MakePersistent(_PurchaseOrderHeader);
            return purchaseOrderHeader;
        }

        [PageSize(20)]
        public IQueryable<Vendor> AutoComplete0CreateNewPurchaseOrder([MinLength(2)] string name) {
            return Container.Instances<Vendor>().Where(v => v.Name.ToUpper().StartsWith(name.ToUpper()));
        }
    }
}