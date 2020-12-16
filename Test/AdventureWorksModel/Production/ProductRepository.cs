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
using NakedObjects.Services;
using NakedObjects.Util;
using System.Text;

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
    public class ProductRepository : AbstractFactoryAndRepository {
        #region FindProductByName

        [FinderAction]
        [TableView(true, "ProductNumber", "ProductSubcategory", "ListPrice"), MemberOrder(1)]
        public IQueryable<Product> FindProductByName(string searchString) {
            return from obj in Instances<Product>()
                where obj.Name.ToUpper().Contains(searchString.ToUpper())
                orderby obj.Name
                select obj;
        }

        #endregion

        #region FindProductByNumber

        [FinderAction]
        [QueryOnly, MemberOrder(2)]
        public Product FindProductByNumber(string number) {
            IQueryable<Product> query = from obj in Instances<Product>()
                where obj.ProductNumber == number
                select obj;

            return SingleObjectWarnIfNoMatch(query);
        }

        #endregion

        #region RandomProduct

        [FinderAction]
        [QueryOnly]
        [MemberOrder(10)]
        public Product RandomProduct() {
            return Random<Product>();
        }

        #endregion

        #region NewProduct

        [FinderAction]
        [MemberOrder(9)]
        public virtual Product NewProduct() {
            return Container.NewTransientInstance<Product>();
        }

        #endregion

        [FinderAction]
        [QueryOnly]
        [MemberOrder(11)]
        public Product FindProductByKey(string key) {
            return Container.FindByKey<Product>(int.Parse(key));
        }

        #region FindProduct

        [FinderAction]
        [QueryOnly]
        [MemberOrder(7)]
        public Product FindProduct(Product product) {
            return product;
        }

        public Product Default0FindProduct() {
            return Instances<Product>().First();
        }

        public virtual IQueryable<Product> AutoComplete0FindProduct(string name) {
            return from obj in Instances<Product>()
                where obj.Name.ToUpper().Contains(name.ToUpper())
                orderby obj.Name
                select obj;
        }

        #endregion

        #region ListProductsBySubCategory

        [FinderAction]
        [TableView(true, "ProductNumber", "ListPrice"), MemberOrder(3)]
        public IQueryable<Product> ListProductsBySubCategory([ContributedAction("Products")] ProductSubcategory subCategory) {
            return from obj in Instances<Product>()
                where obj.ProductSubcategory.ProductSubcategoryID == subCategory.ProductSubcategoryID
                orderby obj.Name
                select obj;
        }

        public virtual ProductSubcategory Default0ListProductsBySubCategory() {
            return Instances<ProductSubcategory>().First();
        }

        #endregion

        #region ListProducts

        [FinderAction]
        [TableView(true, "ProductNumber", "ListPrice"), MemberOrder(3)]
        public IQueryable<Product> ListProducts(ProductCategory category, ProductSubcategory subCategory)
        {
            return from obj in Instances<Product>()
                   where obj.ProductSubcategory.ProductSubcategoryID == subCategory.ProductSubcategoryID
                   orderby obj.Name
                   select obj;
        }


        public ProductCategory Default0ListProducts()
        {
            return Container.Instances<ProductCategory>().First();
        }


        public IList<ProductSubcategory> Choices1ListProducts(ProductCategory category)
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

        [FinderAction]
        [MemberOrder(4)]
        [QueryOnly]
        public IList<Product> ListProductsBySubCategories(IEnumerable<ProductSubcategory> subCategories) {
            return QueryableOfProductsBySubcat(subCategories).ToList();
        }

        public virtual IList<ProductSubcategory> Default0ListProductsBySubCategories() {
            return new List<ProductSubcategory> {
                Instances<ProductSubcategory>().Single(psc => psc.Name == "Mountain Bikes"),
                Instances<ProductSubcategory>().Single(psc => psc.Name == "Touring Bikes")
            };
        }

        private IQueryable<Product> QueryableOfProductsBySubcat(IEnumerable<ProductSubcategory> subCategories) {
            IEnumerable<int> subCatIds = subCategories.Select(x => x.ProductSubcategoryID);
            IQueryable<Product> q = from p in Instances<Product>()
                from sc in subCatIds
                where p.ProductSubcategory.ProductSubcategoryID == sc
                orderby p.Name
                select p;
            return q;
        }

        public string Validate0ListProductsBySubCategories(IEnumerable<ProductSubcategory> subCategories) {
            var rb = new ReasonBuilder();
            rb.AppendOnCondition(subCategories.Count() > 5, "Max 5 SubCategories may be selected");
            return rb.Reason;
        }

        #endregion

        #region FindProductsByCategory

        [FinderAction]
        [MemberOrder(8)]
        public IQueryable<Product> FindProductsByCategory(IEnumerable<ProductCategory> categories, IEnumerable<ProductSubcategory> subcategories) {
            return QueryableOfProductsBySubcat(subcategories);
        }

        public IQueryable<ProductCategory> Choices0FindProductsByCategory() {
            return Instances<ProductCategory>();
        }

        public IQueryable<ProductSubcategory> Choices1FindProductsByCategory(IEnumerable<ProductCategory> categories) {
            if (categories != null) {
                IEnumerable<int> catIds = categories.Select(c => c.ProductCategoryID);

                return from psc in Instances<ProductSubcategory>()
                    from cid in catIds
                    where psc.ProductCategory.ProductCategoryID == cid
                    select psc;
            }
            return new ProductSubcategory[] {}.AsQueryable();
        }

        public IList<ProductCategory> Default0FindProductsByCategory() {
            return new List<ProductCategory> {Instances<ProductCategory>().First()};
        }

        public List<ProductSubcategory> Default1FindProductsByCategory() {
            IList<ProductCategory> pcs = Default0FindProductsByCategory();

            if (pcs != null) {
                IEnumerable<int> ids = pcs.Select(c => c.ProductCategoryID);

                return (from psc in Instances<ProductSubcategory>()
                    from cid in ids
                    where psc.ProductCategory.ProductCategoryID == cid
                    select psc).OrderBy(psc => psc.ProductSubcategoryID).Take(2).ToList();
            }
            return new List<ProductSubcategory>();
        }

        #endregion

        #region FindByProductLinesAndClasses

        [FinderAction]
        [MemberOrder(6)]
        public IQueryable<Product> FindByProductLinesAndClasses(IEnumerable<ProductLineEnum> productLine, IEnumerable<ProductClassEnum> productClass) {
            IQueryable<Product> products = Container.Instances<Product>();

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

        public virtual IList<ProductLineEnum> Default0FindByProductLinesAndClasses() {
            return new List<ProductLineEnum> {ProductLineEnum.M, ProductLineEnum.S};
        }

        public virtual IList<ProductClassEnum> Default1FindByProductLinesAndClasses() {
            return new List<ProductClassEnum> {ProductClassEnum.H};
        }

        [FinderAction]
        [MemberOrder(7)]
        public IQueryable<Product> FindByOptionalProductLinesAndClasses([Optionally]IEnumerable<ProductLineEnum> productLine, [Optionally]IEnumerable<ProductClassEnum> productClass) {
            IQueryable<Product> products = Container.Instances<Product>();

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

        public virtual IList<ProductLineEnum> Default0FindByOptionalProductLinesAndClasses() {
            return new List<ProductLineEnum> { ProductLineEnum.M, ProductLineEnum.S };
        }

        public virtual IList<ProductClassEnum> Default1FindByOptionalProductLinesAndClasses() {
            return new List<ProductClassEnum> { ProductClassEnum.H };
        }



        #endregion

        #region FindByProductLineAndClass

        [FinderAction]
        [MemberOrder(5)]
        public IQueryable<Product> FindByProductLineAndClass(ProductLineEnum productLine, ProductClassEnum productClass) {
            string pls = Enum.GetName(typeof (ProductLineEnum), productLine);
            string pcs = Enum.GetName(typeof (ProductClassEnum), productClass);

            return Container.Instances<Product>().Where(p => p.ProductLine == pls && p.Class == pcs);
        }

        public virtual ProductLineEnum Default0FindByProductLineAndClass() {
            return ProductLineEnum.M;
        }

        public virtual ProductClassEnum Default1FindByProductLineAndClass() {
            return ProductClassEnum.H;
        }

        #endregion
      
      #region Inventory
        /// <summary>
        /// Action is intended to test the returing of a scalar (large multi-line string).
        /// </summary>
        /// <returns></returns>
      public string StockReport()
      {
          var inventories = Container.Instances<ProductInventory>().Select(pi => new InventoryLine {ProductName = pi.Product.Name, Quantity = pi.Quantity});
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
          public string ProductName { get; set; }
          public int Quantity { get; set; }
      }


        public IQueryable<ProductPhoto> AllProductPhotos()
        {
            return Container.Instances<ProductPhoto>();
        }

    }

}