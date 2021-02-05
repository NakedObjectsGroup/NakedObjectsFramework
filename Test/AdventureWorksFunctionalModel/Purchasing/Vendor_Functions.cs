
using NakedFunctions;
using AW.Types;
using System;
using System.Collections.Generic;
using System.Linq;


namespace AW.Functions
{
    public static class Vendor_Functions
    {
        [MemberOrder(1)]
        public static List<Product> ShowAllProducts(this Vendor vendor) =>
            vendor.Products.Select(vp => vp.Product).ToList();

        //Not implemented.  Action is to test disable function only.
        [MemberOrder(2), DescribedAs("Get report from credit agency")]
        public static Vendor CheckCredit(this Vendor v, IContext context) => v;

        public static string DisableCheckCredit(this Vendor v) => "Not yet implemented";

        [MemberOrder("Purchase Orders", 2)]
        public static IQueryable<PurchaseOrderHeader> OpenPurchaseOrders(this Vendor vendor, IContext context) =>
            PurchaseOrder_MenuFunctions.OpenPurchaseOrdersForVendor(vendor, context);

        public static IQueryable<Vendor> AutoComplete0OpenPurchaseOrders([MinLength(2)] string name, IContext context) =>
            PurchaseOrder_MenuFunctions.AutoComplete0OpenPurchaseOrdersForVendor(name, context);


        [MemberOrder("Purchase Orders",3)]
        [TableView(true, "OrderDate", "Status", "TotalDue")]
        public static IQueryable<PurchaseOrderHeader> ListPurchaseOrders(
            this Vendor vendor, DateTime? fromDate, DateTime? toDate, IContext context) =>
                PurchaseOrder_MenuFunctions.ListPurchaseOrders(vendor, fromDate, toDate, context);


        [PageSize(20)]
        public static IQueryable<Vendor> AutoComplete0ListPurchaseOrders(
            this Vendor vendor, [MinLength(2)] string name, IContext context) =>
            PurchaseOrder_MenuFunctions.AutoComplete0ListPurchaseOrders(name, context);


        public static string ValidateListPurchaseOrders(
           this Vendor vendor, DateTime? fromDate, DateTime? toDate) =>
                PurchaseOrder_MenuFunctions.ValidateListPurchaseOrders(vendor, fromDate, toDate);

        [MemberOrder("Purchase Orders", 1)]
        public static (PurchaseOrderHeader, IContext) CreateNewPurchaseOrder(this Vendor vendor,
           ShipMethod shipMethod, IContext context) =>
                PurchaseOrder_MenuFunctions.CreateNewPurchaseOrder(vendor, shipMethod, context);
    }
}
