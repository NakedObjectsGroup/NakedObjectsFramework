using NakedFunctions;
using NakedObjects;
using System;
using System.Collections.Generic;
using System.Linq;


namespace AdventureWorksModel.Sales
{
    [ViewModel]
    public class QuickOrderLine 
    {
        public QuickOrderLine(Product product, short number)
        {
            Product = product;
            Number = number;
        }
        [NakedObjectsIgnore]
        public Product Product { get; set; }

        [NakedObjectsIgnore]
        public short Number { get; set; }
    }

    public static class QuickOrderLineFunctions
    {
        public static string[] DeriveKeys(QuickOrderLine vm)
        {
            return new[] { vm.Product.ProductID.ToString(), vm.Number.ToString() };
        }

        public static QuickOrderLine PopulateUsingKeys(
            QuickOrderLine vm,
            string[] keys,
            [Injected] IQueryable<Product> products)
        {
            int p = int.Parse(keys.First());
            short n = short.Parse(keys.Skip(1).First());
            var product = products.Single(c => c.ProductID == p);
            return vm.With(x => x.Product, product).With(x => x.Number, n);
        }

        public static string Title(QuickOrderLine vm)
        {
            return $"{vm.Number} x {vm.Product.Title()}";
        }

        public static (SalesOrderHeader, SalesOrderDetail) AddTo(QuickOrderLine vm, SalesOrderHeader salesOrder, IQueryable<SpecialOfferProduct> sops)
        {
            SalesOrderDetail sod = salesOrder.AddNewDetail(vm.Product, vm.Number, sops);
            return Result.DisplayAndPersistDifferentItems(sod.SalesOrderHeader, sod);
        }
    }

}
