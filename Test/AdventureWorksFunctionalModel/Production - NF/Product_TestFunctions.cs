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
using static NakedFunctions.Helpers;

namespace AdventureWorksFunctionalModel {
    public static class Product_TestFunctions {
        public static IProduct GetAnotherProduct(this Product product, IQueryable<IProduct> allProducts) {
            return allProducts.First(p => p.ProductID != product.ProductID);
        }

        public static string DisableGetAnotherProduct(this Product product, IQueryable<IProduct> products) {
            return "";
        }

        public static string ValidateGetAnotherProduct(this Product product, IQueryable<IProduct> products) {
            return "";
        }

        public static string HideGetAnotherProduct(this Product product, IQueryable<IProduct> products) {
            return "";
        }

        public static (IProduct, string) GetAnotherProductWithWarning(this Product product, IQueryable<IProduct> allProducts) {
            return (allProducts.First(p => p.ProductID != product.ProductID), "A warning message");
        }

        public static IQueryable<IProduct> GetProducts(this Product product, IQueryable<IProduct> allProducts) =>  
            allProducts.Where(p => p.ProductID != product.ProductID).Take(2);       

        public static IList<IProduct> GetProductsNotQueryable(this Product product, IQueryable<IProduct> allProducts) =>
            allProducts.Where(p => p.ProductID != product.ProductID).Take(2).ToList();


        public static (Product, Product) GetAndPersistProduct(this Product product, IQueryable<Product> allProducts) {
            var pp = allProducts.First(p => p.ProductID != product.ProductID);
            return DisplayAndPersist(pp with { Name = $"{pp.Name}:1" });
        }

        public static (Product, Product[]) GetAndPersistProducts(this Product product, IQueryable<Product> allProducts) {
            var pp = allProducts.First(p => p.ProductID != product.ProductID);
            var pp1 = pp with { Name = $"{pp.Name}:1" };
            return (pp1, new[] {pp1});
        }

        public static (Product, (Product, Product)) GetAndPersistProductsTuple(this Product product, IQueryable<Product> allProducts) {
            var pps = allProducts.Where(p => p.ProductID != product.ProductID).Take(2).ToList();
            var pp1 = pps.First();
            var pp2 = pps.Last();
            var pp1a = pp1 with { Name = $"{pp1.Name}:1" };
            var pp2a = pp2 with { Name = $"{pp2.Name}:2" };
            return (pp1a, (pp1a, pp2a));
        }

        public static (Product, (Product, Product, (Product, Product))) GetAndPersistProductsNestedTuple(this Product product, IQueryable<Product> allProducts) {
            var pps = allProducts.Where(p => p.ProductID != product.ProductID).Take(4).ToArray();
            var pp1 = pps[0];
            var pp2 = pps[1];
            var pp3 = pps[2];
            var pp4 = pps[3];
            var pp1a = pp1 with { Name = $"{pp1.Name}:1" };
            var pp2a = pp2 with { Name = $"{pp2.Name}:2" };
            var pp3a = pp3 with { Name = $"{pp3.Name}:3" };
            var pp4a = pp4 with { Name = $"{pp4.Name}:4" };
            return (pp1a, (pp1a, pp2a, (pp3a, pp4a)));
        }

        public static (Product, Product[], Action<IAlert>) GetAndPersistProductsWithWarning(this Product product, IQueryable<Product> allProducts) {
            var pp = allProducts.First(p => p.ProductID != product.ProductID);
            var pp1 = pp with { Name = $"{pp.Name}:1" };
            return (pp, new[] {pp}, WarnUser("A warning message"));
        }

        public static (Product, Product, Action<IAlert>) GetAndPersistProductWithWarning(this Product product, IQueryable<Product> allProducts) {
            var pp = allProducts.First(p => p.ProductID != product.ProductID);
            var pp1 = pp with { Name = $"{pp.Name}:1" };
            return (pp, pp, WarnUser("A warning message"));
        }

        public static (Product, Product) UpdateProductUsingRemute(this Product product, IQueryable<Product> allProducts) {
            //var pp = allProducts.First(p => p.ProductID != product.ProductID);

            var up = product with {Name =  $"{product.Name}:1"};
            return (up, up);
        }

        public static (IProduct, IProduct) UpdateIProductUsingRemute(this Product product, IQueryable<IProduct> allProducts) {
            throw new NotImplementedException();
            //Product pp = allProducts.First(p => p.ProductID != product.ProductID);

            //var up = pp with {Name =  $"{pp.Name}:1"};
            //return Result.DisplayAndPersist(up);
        }

        public static Product GetAndChangeButNotPersistProduct(this Product product, IQueryable<Product> allProducts) {
            var pp = allProducts.First(p => p.ProductID != product.ProductID);
            var pp1 = pp with {Name = $"{pp.Name}:2"};
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


        public static IProduct Persisting(this Product product, IQueryable<Product> allProducts, [Injected] Guid guid)
            => product with { rowguid = guid };

        public static IProduct Persisted(this Product product, IQueryable<Product> allProducts, [Injected] Guid guid) {
            return null;
        }

        public static IProduct Updating(this Product product, IQueryable<Product> allProducts, [Injected] Guid guid) => product with { rowguid = guid };

        public static IProduct Updated(this Product product, IQueryable<Product> allProducts, [Injected] Guid guid) => null;

        public static Product FindProduct(this Product product, Product product1) => product1;

        public static Product Default1FindProduct(this Product product, IQueryable<Product> products) => products.FirstOrDefault();
   

        public static IQueryable<Product> AutoComplete1FindProduct(this Product product, string name, IQueryable<Product> products) {
            return products.Where(x => x.Name.ToUpper().Contains(name.ToUpper()));
        }

        public static Product SelectProduct(this Product product, Product product1) {
            return product1;
        }

        public static IEnumerable<Product> Choices1SelectProduct(this Product product, IQueryable<Product> products) {
            return products.Take(10).ToList();
        }
    
        public static Product TestDisabled(this Product product, Product product1) {
            return product1;
        }
    }
}