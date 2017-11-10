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
using NakedObjects;
using System.Text;
using NakedFunctions;

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
    public class ProductRepository : AWAbstractFactoryAndRepository {
        #region FindProductByName
        [TableView(true, "ProductNumber", "ProductSubcategory", "ListPrice"), MemberOrder(1)]
        public static QueryResultList FindProductByName(string searchString, IFunctionalContainer container) {
            return new QueryResultList(QueryProductByName(searchString, container));
        }

        internal static IQueryable<Product> QueryProductByName(string searchString, IFunctionalContainer container)
        {
            return container.Instances<Product>().
                Where(obj => obj.Name.ToUpper().Contains(searchString.ToUpper())).
                OrderBy(obj => obj.Name);
        }
        #endregion

        #region FindProductByNumber

        [QueryOnly, MemberOrder(2)]
        public static QueryResultSingle FindProductByNumber(string number, IFunctionalContainer container) {
            IQueryable<Product> query = from obj in container.Instances<Product>()
                where obj.ProductNumber == number
                select obj;

            return SingleObjectWarnIfNoMatch(query);
        }

        #endregion

        #region RandomProduct

        
        [QueryOnly]
        [MemberOrder(10)]
        public static QueryResultSingle RandomProduct(IFunctionalContainer container) {
            return Random<Product>(container);
        }

        #endregion

        #region NewProduct   
        [MemberOrder(9)]
        public static  PotentResultSingle NewProduct(string name) {
            return new PotentResultSingle(new Product(name), null);
        }

        #endregion

        [MemberOrder(11)]
        public static QueryResultSingle FindProductByKey(string key) {
            return new QueryResultSingle(FindByKey<Product>(int.Parse(key)));
        }

        #region FindProduct

        [MemberOrder(7)]
        public static QueryResultSingle FindProduct(Product product) {
            return new QueryResultSingle(product);
        }

        public static Product Default0FindProduct(IFunctionalContainer container) {
            return container.Instances<Product>().First();
        }

        public static  IQueryable<Product> AutoComplete0FindProduct(string name, IFunctionalContainer container) {
            return container.Instances<Product>().
                Where(obj => obj.Name.ToUpper().Contains(name.ToUpper())).
                OrderBy(obj => obj.Name);
        }

        #endregion

        #region ListProductsBySubCategory
        [TableView(true, "ProductNumber", "ListPrice"), MemberOrder(3)]
        public static QueryResultList ListProductsBySubCategory([ContributedAction("Products")] ProductSubcategory subCategory, IFunctionalContainer container) {
            return new QueryResultList(container.Instances<Product>().
                Where(obj => obj.ProductSubcategory.ProductSubcategoryID == subCategory.ProductSubcategoryID).
                OrderBy(obj => obj.Name));
        }

        public static  ProductSubcategory Default0ListProductsBySubCategory(IFunctionalContainer container) {
            return container.Instances<ProductSubcategory>().First();
        }
        #endregion

        #region ListProducts   
        [TableView(true, "ProductNumber", "ListPrice"), MemberOrder(3)]
        public static QueryResultList ListProducts(ProductCategory category, ProductSubcategory subCategory, IFunctionalContainer container)
        {
            return new QueryResultList(container.Instances<Product>().
                   Where(p => p.ProductSubcategory.ProductSubcategoryID == subCategory.ProductSubcategoryID).
                   OrderBy(p => p.Name));
        }

        public static ProductCategory Default0ListProducts(IQueryable<ProductCategory> pcs)
        {
            return pcs.First();
        }

        public static IList<ProductSubcategory> Choices1ListProducts(ProductCategory category)
        {
            if (category != null)
            {
                return category.ProductSubcategory.ToList();
            } else
            {
                return null;
            }
        }

        #endregion

        #region ListProductsBySubcategories
        [MemberOrder(4)]
        [QueryOnly]
        public static QueryResultList ListProductsBySubCategories(IEnumerable<ProductSubcategory> subCategories, IFunctionalContainer container) {
            return new QueryResultList(QueryableOfProductsBySubcat(subCategories, container).ToList());
        }

        public static  IList<ProductSubcategory> Default0ListProductsBySubCategories(IFunctionalContainer container) {
            return new List<ProductSubcategory> {
                container.Instances<ProductSubcategory>().Single(psc => psc.Name == "Mountain Bikes"),
                container.Instances<ProductSubcategory>().Single(psc => psc.Name == "Touring Bikes")
            };
        }

        private static IQueryable<Product> QueryableOfProductsBySubcat(IEnumerable<ProductSubcategory> subCategories, IFunctionalContainer container) {
            IEnumerable<int> subCatIds = subCategories.Select(x => x.ProductSubcategoryID);
            IQueryable<Product> q = from p in container.Instances<Product>()
                from sc in subCatIds
                where p.ProductSubcategory.ProductSubcategoryID == sc
                orderby p.Name
                select p;
            return q;
        }

        public static string Validate0ListProductsBySubCategories(IEnumerable<ProductSubcategory> subCategories) {
            var rb = new ReasonBuilder();
            rb.AppendOnCondition(subCategories.Count() > 5, "Max 5 SubCategories may be selected");
            return rb.Reason;
        }

        #endregion

        #region FindProductsByCategory
 
        [MemberOrder(8)]
        public static QueryResultList FindProductsByCategory(IEnumerable<ProductCategory> categories, IEnumerable<ProductSubcategory> subcategories, IFunctionalContainer container) {
            return new QueryResultList(QueryableOfProductsBySubcat(subcategories, container));
        }

        public static IQueryable<ProductCategory> Choices0FindProductsByCategory(IFunctionalContainer container) {
            return container.Instances<ProductCategory>();
        }

        public static IQueryable<ProductSubcategory> Choices1FindProductsByCategory(IEnumerable<ProductCategory> categories, IFunctionalContainer container) {
            if (categories != null) {
                IEnumerable<int> catIds = categories.Select(c => c.ProductCategoryID);

                return from psc in container.Instances<ProductSubcategory>()
                    from cid in catIds
                    where psc.ProductCategory.ProductCategoryID == cid
                    select psc;
            }
            return new ProductSubcategory[] {}.AsQueryable();
        }

        public static IList<ProductCategory> Default0FindProductsByCategory(IFunctionalContainer container) {
            return new List<ProductCategory> {container.Instances<ProductCategory>().First()};
        }

        public static List<ProductSubcategory> Default1FindProductsByCategory(IFunctionalContainer container) {
            IList<ProductCategory> pcs = Default0FindProductsByCategory(container);

            if (pcs != null) {
                IEnumerable<int> ids = pcs.Select(c => c.ProductCategoryID);

                return (from psc in container.Instances<ProductSubcategory>()
                    from cid in ids
                    where psc.ProductCategory.ProductCategoryID == cid
                    select psc).OrderBy(psc => psc.ProductSubcategoryID).Take(2).ToList();
            }
            return new List<ProductSubcategory>();
        }

        #endregion

        #region FindByProductLinesAndClasses

        
        [MemberOrder(6)]
        public static QueryResultList FindByProductLinesAndClasses(IEnumerable<ProductLineEnum> productLine, IEnumerable<ProductClassEnum> productClass, IFunctionalContainer container) {
            var products = container.Instances<Product>();
            //TODO: This code does not look correct to me!
            //Anyway, need to convert to inline syntax for a proper function
            foreach (ProductLineEnum pl in productLine) {
                string pls = Enum.GetName(typeof (ProductLineEnum), pl);
                products = products.Where(p => p.ProductLine == pls);
            }

            foreach (ProductClassEnum pc in productClass) {
                string pcs = Enum.GetName(typeof (ProductClassEnum), pc);
                products = products.Where(p => p.Class == pcs);
            }

            return new QueryResultList(products);
        }

        public static  IList<ProductLineEnum> Default0FindByProductLinesAndClasses() {
            return new List<ProductLineEnum> {ProductLineEnum.M, ProductLineEnum.S};
        }

        public static  IList<ProductClassEnum> Default1FindByProductLinesAndClasses() {
            return new List<ProductClassEnum> {ProductClassEnum.H};
        }

        [MemberOrder(7)]
        public static IQueryable<Product> FindByOptionalProductLinesAndClasses([Optionally]IEnumerable<ProductLineEnum> productLine, [Optionally]IEnumerable<ProductClassEnum> productClass, IQueryable<Product> products) {
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

        
        [MemberOrder(5)]
        public static IQueryable<Product> FindByProductLineAndClass(ProductLineEnum productLine, ProductClassEnum productClass, IQueryable<Product> products) {
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
      public static string StockReport(IQueryable<ProductInventory> pis)
      {
          var inventories = pis.Select(pi => new InventoryLine(pi.Product.Name,pi.Quantity));
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

            public InventoryLine(string name, int qty)
            {
                ProductName = name;
                Quantity = qty;
            }
          public string ProductName { get; private set; }
          public int Quantity { get; private set; }
      }


        public static IQueryable<ProductPhoto> AllProductPhotos(IQueryable<ProductPhoto> photos)
        {
            return photos;
        }

    }

}