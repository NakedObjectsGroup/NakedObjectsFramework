﻿using NakedFunctions;
using static AW.Functions.SalesOrderHeader_Functions;
using System.Linq;


namespace AW.Types
{
    [ViewModel]
    public record QuickOrderLine 
    {
        public QuickOrderLine(Product product, short number)
        {
            Product = product;
            Number = number;
        }
        [Hidden]
        public Product Product { get; init; }

        [Hidden]
        public short Number { get; init; }

        public override string ToString() =>  $"{Number} x {Product}";
    }

    public static class QuickOrderLineFunctions
    {
        public static string[] DeriveKeys(QuickOrderLine vm)
        {
            return new[] { vm.Product.ProductID.ToString(), vm.Number.ToString() };
        }

        public static QuickOrderLine PopulateUsingKeys(
            QuickOrderLine vm, string[] keys,IContext context)
        {
            int p = int.Parse(keys.First());
            short n = short.Parse(keys.Skip(1).First());
            var product = context.Instances<Product>().Single(c => c.ProductID == p);
            return vm with {Product =  product, Number= n};
        }


        public static (SalesOrderHeader, IContext) AddTo(QuickOrderLine vm, SalesOrderHeader salesOrder, IContext context) =>
            salesOrder.AddNewDetail(vm.Product, vm.Number, context);
        
    }

}