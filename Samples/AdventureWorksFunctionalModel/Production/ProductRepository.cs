// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using NakedFunctions;
using NakedObjects;
using static AdventureWorksModel.CommonFactoryAndRepositoryFunctions;

namespace AdventureWorksModel {
    public enum ProductLineEnum {
        R,
        M,
        T,
        S
    }

    public enum ProductClassEnum {
        H,
        M,
        L
    }

    [DisplayName("Products")]
    public static class ProductRepository  { 

        [TableView(true, "ProductNumber", "ProductSubcategory", "ListPrice"), MemberOrder(1)]
        public static IQueryable<Product> FindProductByName(
             
            string searchString, 
            [Injected] IQueryable<Product> products)
        {
            return products.Where(x => x.Name.ToUpper().Contains(searchString.ToUpper())).OrderBy(x => x.Name);
        }

        [FinderAction]
        [MemberOrder(2)]
        public static (Product,string) FindProductByNumber(
            
            string number, 
            [Injected] IQueryable<Product> products)
        {
            return SingleObjectWarnIfNoMatch(products.Where(x => x.ProductNumber == number));
        }

        [FinderAction]
        
        [MemberOrder(10)]
        public static Product RandomProduct(
            
            [Injected] IQueryable<Product> products, 
            [Injected] int random)
        {
            return Random(products, random);
        }

        [MemberOrder(9)]
        public static (Product, Product) NewProduct()
        {
            //TODO: Must add parameters for minimum property set and call full constructor with null for others
            var p = new Product();
            return Result.DisplayAndPersist(p);
        }

        #region FindProduct

        
        [MemberOrder(7)]
        public static Product FindProduct(
             
            Product product)
        {
            return product;
        }

        public static Product Default0FindProduct([Injected] IQueryable<Product> products) {
            return products.FirstOrDefault();
        }

        public static IQueryable<Product> AutoComplete0FindProduct(string name, [Injected] IQueryable<Product> products) {
            return products.Where(x => x.Name.ToUpper().Contains(name.ToUpper()));
        }

        #endregion

       #region ListProductsBySubCategory

      //TODO: This action is both a menu action AND a contributed action.  Should that be permitted? How to specify it?
        [TableView(true, "ProductNumber", "ListPrice"), MemberOrder(3)]
        public static IQueryable<Product> ListProductsBySubCategory(
            
            [ContributedAction("Products")] ProductSubcategory subCategory, 
            [Injected] IQueryable<Product> products) {
            return products.Where(x => x.ProductSubcategory.ProductSubcategoryID == subCategory.ProductSubcategoryID).OrderBy(x => x.Name);
        }

        public static ProductSubcategory Default0ListProductsBySubCategory([Injected] IQueryable<ProductSubcategory> subCats) {
            return subCats.FirstOrDefault();
        }

        #endregion

        #region ListProducts

        [TableView(true, "ProductNumber", "ListPrice"), MemberOrder(3)]
        public static IQueryable<Product> ListProducts(
             
            ProductCategory category, 
            ProductSubcategory subCategory, 
            [Injected] IQueryable<Product> products)
        {
            return products.Where(x => x.ProductSubcategory.ProductSubcategoryID == subCategory.ProductSubcategoryID).OrderBy(x => x.Name);
        }

        public static ProductCategory Default0ListProducts([Injected] IQueryable<ProductCategory> cats)
        {
            return cats.FirstOrDefault();
        }


        public static IList<ProductSubcategory> Choices1ListProducts(ProductCategory category)
        {
                return category is null ? null : category.ProductSubcategory.ToList();
        }

        #endregion

        #region ListProductsBySubcategories

        [MemberOrder(4)]
        
        public static IList<Product> ListProductsBySubCategories(
            
            IEnumerable<ProductSubcategory> subCategories,
            [Injected] IQueryable<Product> products) {
            return QueryableOfProductsBySubcat(subCategories, products).ToList();
        }

        public static IList<ProductSubcategory> Default0ListProductsBySubCategories([Injected] IQueryable<ProductSubcategory> subCats) {
            return new List<ProductSubcategory> {
                subCats.Single(psc => psc.Name == "Mountain Bikes"),
                subCats.Single(psc => psc.Name == "Touring Bikes")
            };
        }

        private static IQueryable<Product> QueryableOfProductsBySubcat(
            
            IEnumerable<ProductSubcategory> subCategories,
            [Injected] IQueryable<Product> products) {

            IEnumerable<int> subCatIds = subCategories.Select(x => x.ProductSubcategoryID);
            return from p in products
                from sc in subCatIds
                where p.ProductSubcategory.ProductSubcategoryID == sc
                orderby p.Name
                select p;
        }

        public static string Validate0ListProductsBySubCategories(IEnumerable<ProductSubcategory> subCategories) {
            return subCategories.Count() > 5? "Max 5 SubCategories may be selected": null;
        }
        #endregion

        #region FindProductsByCategory

        [FinderAction]
        [MemberOrder(8)]
        public static IQueryable<Product> FindProductsByCategory(
            
            IEnumerable<ProductCategory> categories, 
            IEnumerable<ProductSubcategory> subcategories,
            [Injected] IQueryable<Product> products) {
            return QueryableOfProductsBySubcat( subcategories, products);
        }

        public static IQueryable<ProductCategory> Choices0FindProductsByCategory([Injected] IQueryable<ProductCategory> cats) {
            return cats;
        }

        public static IQueryable<ProductSubcategory> Choices1FindProductsByCategory(
            IEnumerable<ProductCategory> categories, 
            [Injected] IQueryable<ProductSubcategory> subCats) 
        {
            if (categories != null) {
                IEnumerable<int> catIds = categories.Select(c => c.ProductCategoryID);

                return from psc in subCats
                    from cid in catIds
                    where psc.ProductCategory.ProductCategoryID == cid
                    select psc;
            }
            return new ProductSubcategory[] {}.AsQueryable();
        }

        public static IList<ProductCategory> Default0FindProductsByCategory(
            [Injected] IQueryable<ProductCategory> cats)
        {
            return new List<ProductCategory> {cats.FirstOrDefault()};
        }

        public static List<ProductSubcategory> Default1FindProductsByCategory(
            [Injected] IQueryable<ProductCategory> cats,
             [Injected] IQueryable<ProductSubcategory> subCats)
        {
            IList<ProductCategory> pcs = Default0FindProductsByCategory(cats);

            if (pcs != null) {
                IEnumerable<int> ids = pcs.Select(c => c.ProductCategoryID);

                return (from psc in subCats
                    from cid in ids
                    where psc.ProductCategory.ProductCategoryID == cid
                    select psc).OrderBy(psc => psc.ProductSubcategoryID).Take(2).ToList();
            }
            return new List<ProductSubcategory>();
        }

        #endregion

        #region FindByProductLinesAndClasses

        [MemberOrder(6)]
        public static IQueryable<Product> FindByProductLinesAndClasses(
            
            IEnumerable<ProductLineEnum> productLine, 
            IEnumerable<ProductClassEnum> productClass,
            [Injected] IQueryable<Product> products)
        {
            foreach (ProductLineEnum pl in productLine) {
                string pls = Enum.GetName(typeof (ProductLineEnum), pl);
                products = products.Where(p => p.ProductLine == pls);
            }

            foreach (ProductClassEnum pc in productClass) {
                string pcs = Enum.GetName(typeof (ProductClassEnum), pc);
                products = products.Where(p => p.Class == pcs);
            }

            return products;
        }

        public static  IList<ProductLineEnum> Default0FindByProductLinesAndClasses() {
            return new List<ProductLineEnum> {ProductLineEnum.M, ProductLineEnum.S};
        }

        public static  IList<ProductClassEnum> Default1FindByProductLinesAndClasses() {
            return new List<ProductClassEnum> {ProductClassEnum.H};
        }

        [MemberOrder(7)]
        public static IQueryable<Product> FindByOptionalProductLinesAndClasses(
            
            [Optionally]IEnumerable<ProductLineEnum> productLine, 
            [Optionally]IEnumerable<ProductClassEnum> productClass,
            [Injected] IQueryable<Product> products)
        {
            if (productLine != null) {
                foreach (ProductLineEnum pl in productLine) {
                    string pls = Enum.GetName(typeof(ProductLineEnum), pl);
                    products = products.Where(p => p.ProductLine == pls);
                }
            }

            if (productClass != null) {
                foreach (ProductClassEnum pc in productClass) {
                    string pcs = Enum.GetName(typeof(ProductClassEnum), pc);
                    products = products.Where(p => p.Class == pcs);
                }
            }
            return products;
        }

        public static  IList<ProductLineEnum> Default0FindByOptionalProductLinesAndClasses() {
            return new List<ProductLineEnum> { ProductLineEnum.M, ProductLineEnum.S };
        }

        public static  IList<ProductClassEnum> Default1FindByOptionalProductLinesAndClasses() {
            return new List<ProductClassEnum> { ProductClassEnum.H };
        }
        #endregion

        #region FindByProductLineAndClass

        [FinderAction]
        [MemberOrder(5)]
        public static IQueryable<Product> FindByProductLineAndClass(
            
            ProductLineEnum productLine, 
            ProductClassEnum productClass,
            [Injected] IQueryable<Product> products)
        {
            string pls = Enum.GetName(typeof (ProductLineEnum), productLine);
            string pcs = Enum.GetName(typeof (ProductClassEnum), productClass);
            return products.Where(p => p.ProductLine == pls && p.Class == pcs);
        }

        public static  ProductLineEnum Default0FindByProductLineAndClass() {
            return ProductLineEnum.M;
        }

        public static  ProductClassEnum Default1FindByProductLineAndClass() {
            return ProductClassEnum.H;
        }

        #endregion
      
      #region Inventory
        /// <summary>
        /// Action is intended to test the returing of a scalar (large multi-line string).
        /// </summary>
        /// <returns></returns>
      public static string StockReport(
            
          [Injected] IQueryable<ProductInventory> inv)
      {
          var inventories = inv.Select(pi => new InventoryLine {ProductName = pi.Product.Name, Quantity = pi.Quantity});
          var sb = new StringBuilder();
          sb.AppendLine(@"<h1 id=""report"">Stock Report</h1>");
          sb.AppendLine("<table>");
          sb.AppendLine("<tr><th>Product</th><th>Quantity</th></tr>");
          foreach (var i in inventories) {
              sb.AppendLine("<tr><td>"+i.ProductName+"</td><td>"+i.Quantity+"</td></tr>");
          }
          sb.AppendLine("</table>");
          return sb.ToString();

      }
      #endregion

      private class InventoryLine {
          public  string ProductName { get; set; }
          public  int Quantity { get; set; }
      }

        public static IQueryable<ProductPhoto> AllProductPhotos(
            [Injected] IQueryable<ProductPhoto> photos)
        {
            return photos;
        }
    }

}