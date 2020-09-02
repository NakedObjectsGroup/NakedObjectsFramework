// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using AdventureWorksModel;
using NakedFunctions;
using NakedObjects;

namespace AdventureWorksFunctionalModel.Functions {
    public static class ProductFunctions {
        public static IProduct GetAnotherProduct(this Product product, [Injected] IQueryable<IProduct> allProducts) {
            return allProducts.First(p => p.ProductID != product.ProductID);
        }

        public static string DisableGetAnotherProduct(this Product product, [Injected] IQueryable<IProduct> products) {
            return "";
        }

        public static string ValidateGetAnotherProduct(this Product product, [Injected] IQueryable<IProduct> products) {
            return "";
        }

        public static string HideGetAnotherProduct(this Product product, [Injected] IQueryable<IProduct> products) {
            return "";
        }

        public static (IProduct, string) GetAnotherProductWithWarning(this Product product, [Injected] IQueryable<IProduct> allProducts) {
            return (allProducts.First(p => p.ProductID != product.ProductID), "A warning message");
        }

        public static IQueryable<IProduct> GetProducts(this Product product, [Injected] IQueryable<IProduct> allProducts) {
            return allProducts.Where(p => p.ProductID != product.ProductID).Take(2);
        }

        public static IList<IProduct> GetProductsNotQueryable(this Product product, [Injected] IQueryable<IProduct> allProducts) {
            return allProducts.Where(p => p.ProductID != product.ProductID).Take(2).ToList();
        }

        public static (Product, Product) GetAndPersistProduct(this Product product, [Injected] IQueryable<Product> allProducts) {
            var pp = allProducts.First(p => p.ProductID != product.ProductID);
            pp.Name = $"{pp.Name}:1";
            return Result.DisplayAndPersist(pp);
        }

        public static (Product, Product[]) GetAndPersistProducts(this Product product, [Injected] IQueryable<Product> allProducts) {
            var pp = allProducts.First(p => p.ProductID != product.ProductID);
            pp.Name = $"{pp.Name}:1";
            return (pp, new[] {pp});
        }

        public static (Product, (Product, Product)) GetAndPersistProductsTuple(this Product product, [Injected] IQueryable<Product> allProducts) {
            var pps = allProducts.Where(p => p.ProductID != product.ProductID).Take(2).ToList();
            var pp1 = pps.First();
            var pp2 = pps.Last();
            pp1.Name = $"{pp1.Name}:1";
            pp2.Name = $"{pp2.Name}:2";
            return (pp1, (pp1, pp2));
        }

        public static (Product, (Product, Product, (Product, Product))) GetAndPersistProductsNestedTuple(this Product product, [Injected] IQueryable<Product> allProducts) {
            var pps = allProducts.Where(p => p.ProductID != product.ProductID).Take(4).ToArray();
            var pp1 = pps[0];
            var pp2 = pps[1];
            var pp3 = pps[2];
            var pp4 = pps[3];
            pp1.Name = $"{pp1.Name}:1";
            pp2.Name = $"{pp2.Name}:2";
            pp3.Name = $"{pp3.Name}:3";
            pp4.Name = $"{pp4.Name}:4";
            return (pp1, (pp1, pp2, (pp3, pp4)));
        }

        public static (Product, Product[], string) GetAndPersistProductsWithWarning(this Product product, [Injected] IQueryable<Product> allProducts) {
            var pp = allProducts.First(p => p.ProductID != product.ProductID);
            pp.Name = $"{pp.Name}:1";
            return (pp, new[] {pp}, "A warning message");
        }

        public static (Product, Product, string) GetAndPersistProductWithWarning(this Product product, [Injected] IQueryable<Product> allProducts) {
            var pp = allProducts.First(p => p.ProductID != product.ProductID);
            pp.Name = $"{pp.Name}:1";
            return Result.DisplayAndPersist(pp, "A warning message");
        }

        public static (Product, Product) UpdateProductUsingRemute(this Product product, [Injected] IQueryable<Product> allProducts) {
            //var pp = allProducts.First(p => p.ProductID != product.ProductID);

            var up = product.With(x => x.Name, $"{product.Name}:1");
            return Result.DisplayAndPersist(up);
        }

        public static (IProduct, IProduct) UpdateIProductUsingRemute(this Product product, [Injected] IQueryable<IProduct> allProducts) {
            var pp = allProducts.First(p => p.ProductID != product.ProductID);

            var up = pp.With(x => x.Name, $"{pp.Name}:1");
            return Result.DisplayAndPersist(up);
        }

        public static Product GetAndChangeButNotPersistProduct(this Product product, [Injected] IQueryable<Product> allProducts) {
            var pp = allProducts.First(p => p.ProductID != product.ProductID);
            pp.Name = $"{pp.Name}:2";
            return pp;
        }

        public static IProduct TestInjectedGuid(this Product product, [Injected] Guid guid) {
            var test = guid;
            return product;
        }

        public static IProduct TestInjectedPrincipal(this Product product, [Injected] IPrincipal principal) {
            var test = principal;
            return product;
        }

        public static IProduct TestInjectedDateTime(this Product product, [Injected] DateTime dateTime) {
            var test = dateTime;
            return product;
        }

        public static IProduct TestInjectedRandom(this Product product, [Injected] int random) {
            var test = random;
            return product;
        }

        public static string Title(this Product p) {
            return p.CreateTitle($"{p.Name}");
        }

        public static IProduct Persisting(this Product product, [Injected] IQueryable<Product> allProducts, [Injected] Guid guid) {
            product.rowguid = guid;
            return product;
        }

        public static IProduct Persisted(this Product product, [Injected] IQueryable<Product> allProducts, [Injected] Guid guid) {
            return null;
        }

        public static IProduct Updating(this Product product, [Injected] IQueryable<Product> allProducts, [Injected] Guid guid) {
            product.rowguid = guid;
            return product;
        }

        public static IProduct Updated(this Product product, [Injected] IQueryable<Product> allProducts, [Injected] Guid guid) {
            return null;
        }

        public static Product FindProduct(this Product product,
                                          Product product1) {
            return product1;
        }

        public static Product Default1FindProduct(this Product product, [Injected] IQueryable<Product> products) {
            return products.FirstOrDefault();
        }

        public static IQueryable<Product> AutoComplete1FindProduct(this Product product, string name, [Injected] IQueryable<Product> products) {
            return products.Where(x => x.Name.ToUpper().Contains(name.ToUpper()));
        }

        public static Product SelectProduct(this Product product, Product product1) {
            return product1;
        }

        public static IEnumerable<Product> Choices1SelectProduct(this Product product, [Injected] IQueryable<Product> products) {
            return products.Take(10).ToList();
        }

        [Disabled]
        public static Product TestDisabled(this Product product, Product product1) {
            return product1;
        }
    }
}