
using NakedFunctions;
using AW.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using static AW.Helpers;

namespace AW.Functions
{
    public static class Vendor_Functions
    {
        public static List<Product> ShowAllProducts(this Vendor vendor)
        {
            return vendor.Products.Select(vp => vp.Product).ToList();
        }

        public static (Person, IContext) CreateNewContact(this Vendor vendor, IContext context) =>
          DisplayAndSave(new Person() with { ForEntity = vendor }, context);

        [DescribedAs("Get report from credit agency")]
        public static Vendor CheckCredit(this Vendor v)
        {
            throw new NotImplementedException();
            //Not implemented.  Action is to test disable function only.
        }

        public static string DisableCheckCredit(this Vendor v) => "Not yet implemented";


        public static IQueryable<PurchaseOrderHeader> OpenPurchaseOrders(this Vendor vendor, IContext context) =>
            PurchaseOrder_MenuFunctions.OpenPurchaseOrdersForVendor(vendor, context);

        public static IQueryable<Vendor> AutoComplete0OpenPurchaseOrders([MinLength(2)] string name, IContext context) =>
            PurchaseOrder_MenuFunctions.AutoComplete0OpenPurchaseOrdersForVendor(name, context);


        [MemberOrder(4, "Purchase Orders")]
        [TableView(true, "OrderDate", "Status", "TotalDue")]
        public static IQueryable<PurchaseOrderHeader> ListPurchaseOrders(
    this Vendor vendor, DateTime? fromDate, DateTime? toDate, IContext context) =>
            PurchaseOrder_MenuFunctions.ListPurchaseOrders(vendor, fromDate, toDate, context);


        [PageSize(20)]
        public static IQueryable<Vendor> AutoComplete0ListPurchaseOrders(
            this Vendor vendor, [MinLength(2)] string name, IContext context) =>
            PurchaseOrder_MenuFunctions.AutoComplete0ListPurchaseOrders(name, context);

        //TODO: Should match action, ignoring any injected properties, on either action or this
        public static string ValidateListPurchaseOrders(
           this Vendor vendor, DateTime? fromDate, DateTime? toDate) =>
           PurchaseOrder_MenuFunctions.ValidateListPurchaseOrders(vendor, fromDate, toDate);
    }
}
