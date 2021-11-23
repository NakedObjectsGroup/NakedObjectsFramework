






using AW.Types;
using NakedFunctions;

namespace AW.Functions {
    public static class SalesOrderDetail_Functions {
        public static IContext ChangeQuantity(this SalesOrderDetail detail, short newQuantity, IContext context) {
            var sop = detail.Product.BestSpecialOfferProduct(newQuantity, context);
            SalesOrderDetail detail2 = new(detail) {
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

        public static string? DisableChangeQuantity(this SalesOrderDetail detail) =>
            detail.SalesOrderHeader.DisableAddNewDetail();
    }
}