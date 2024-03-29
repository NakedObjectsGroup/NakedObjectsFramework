﻿using static AW.Helpers;

namespace AW.Functions;
[Named("Purchase Orders")]
public static class PurchaseOrder_MenuFunctions
{
    [MemberOrder(1)]
    public static PurchaseOrderHeader RandomPurchaseOrder(IContext context) => Random<PurchaseOrderHeader>(context);

    //returns most recently-modified first
    [MemberOrder(2)]
    public static IQueryable<PurchaseOrderHeader> AllPurchaseOrders(IContext context) =>
        context.Instances<PurchaseOrderHeader>().OrderByDescending(poh => poh.ModifiedDate);

    [MemberOrder(2)]
    [TableView(true, "Vendor", "OrderDate", "TotalDue")]
    public static IQueryable<PurchaseOrderHeader> AllOpenPurchaseOrders(IContext context) =>
        context.Instances<PurchaseOrderHeader>().Where(poh => poh.Status <= 2);

    #region FindById

    [MemberOrder(7)]
    public static PurchaseOrderHeader? FindById(int id, IContext context) =>
        context.Instances<PurchaseOrderHeader>().FirstOrDefault(x => x.PurchaseOrderID == id);

    #endregion

    #region OpenPurchaseOrdersForVendor

    [MemberOrder(3)]
    [TableView(true, "OrderDate", "TotalDue")]
    public static IQueryable<PurchaseOrderHeader> OpenPurchaseOrdersForVendor(Vendor vendor, IContext context) =>
        AllOpenPurchaseOrders(context).Where(poh => poh.Vendor.BusinessEntityID == vendor.BusinessEntityID);

    [PageSize(20)]
    public static IQueryable<Vendor> AutoComplete0OpenPurchaseOrdersForVendor([MinLength(2)] string name, IContext context) =>
        context.Instances<Vendor>().Where(v => v.Name.ToUpper().StartsWith(name.ToUpper()));

    #endregion

    #region ListPurchaseOrdersForVendor

    [MemberOrder(4)]
    [TableView(true, "OrderDate", "Status", "TotalDue")]
    public static IQueryable<PurchaseOrderHeader> ListPurchaseOrders(
        Vendor vendor, DateTime? fromDate, DateTime? toDate, IContext context) =>
        from obj in context.Instances<PurchaseOrderHeader>()
        where obj.Vendor.BusinessEntityID == vendor.BusinessEntityID &&
              (fromDate == null || obj.OrderDate >= fromDate) &&
              (toDate == null || obj.OrderDate <= toDate)
        orderby obj.OrderDate
        select obj;

    [PageSize(20)]
    public static IQueryable<Vendor> AutoComplete0ListPurchaseOrders(
        [MinLength(2)] string name, IContext context) =>
        context.Instances<Vendor>().Where(v => v.Name.ToUpper().StartsWith(name.ToUpper()));

    public static string? ValidateListPurchaseOrders(
        Vendor vendor,
        DateTime? fromDate,
        DateTime? toDate) =>
        toDate.IsAtLeastOneDayBefore(fromDate) ? "To Date cannot be before From Date" : null;

    #endregion

    #region OpenPurchaseOrdersForProduct

    [MemberOrder("Purchase Orders", 5)]
    [TableView(true, "Vendor", "OrderDate", "Status")]
    public static IQueryable<PurchaseOrderHeader> OpenPurchaseOrdersForProduct(
        Product product, IContext context) =>
        from obj in context.Instances<PurchaseOrderDetail>()
        where obj.Product.ProductID == product.ProductID &&
              obj.PurchaseOrderHeader.Status <= 2
        select obj.PurchaseOrderHeader;

    [PageSize(20)]
    public static IQueryable<Product> AutoComplete0OpenPurchaseOrdersForProduct(
        [MinLength(2)] string name, IContext context)
    {
        return context.Instances<Product>().Where(product => product.Name.ToUpper().StartsWith(name.ToUpper()));
    }

    // for autoautocomplete testing
    public static IQueryable<PurchaseOrderHeader> OpenPurchaseOrdersForVendorAndProduct(
        Vendor vendor, Product product, IContext context) =>
        from obj in context.Instances<PurchaseOrderDetail>()
        where obj.Product.ProductID == product.ProductID &&
              obj.PurchaseOrderHeader.Status <= 2 &&
              obj.PurchaseOrderHeader.Vendor.BusinessEntityID == vendor.BusinessEntityID
        select obj.PurchaseOrderHeader;

    #endregion

    #region Create New Purchase Order

    [MemberOrder(6)]
    [CreateNew]
    public static (PurchaseOrderHeader, IContext) CreateNewPurchaseOrder(
        Vendor vendor,
        ShipMethod shipMethod,
        IContext context)
    {
        var po = new PurchaseOrderHeader
        {
            RevisionNumber = 0,
            Status = (byte)POStatus.Pending,
            OrderPlacedByID = 1, //TODO: using Employee 1 as a default as no logged on user
            VendorID = vendor.BusinessEntityID,
            ShipMethodID = shipMethod.ShipMethodID,
            OrderDate = context.Today(),
            SubTotal = 0,
            TaxAmt = 0,
            Freight = 0,
            ModifiedDate = context.Now()
        };
        return (po, context.WithNew(po));
    }

    [PageSize(20)]
    public static IQueryable<Vendor> AutoComplete0CreateNewPurchaseOrder(
        [MinLength(2)] string name, IContext context) =>
        context.Instances<Vendor>().Where(v => v.Name.ToUpper().StartsWith(name.ToUpper()));

    #endregion
}