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
using NakedObjects;
using NakedObjects.Services;

namespace AdventureWorksLegacyModel.Production {
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
        [TableView(true, "ProductNumber", "ProductSubcategory", "ListPrice")] [MemberOrder(1)]
        public IQueryable<Product> FindProductByName(string searchString) =>
            from obj in Instances<Product>()
            where obj.Name.ToUpper().Contains(searchString.ToUpper())
            orderby obj.Name
            select obj;

        #endregion

        #region FindProductByNumber

        [FinderAction]
        [QueryOnly] [MemberOrder(2)]
        public Product FindProductByNumber(string number) {
            var query = from obj in Instances<Product>()
                        where obj.ProductNumber == number
                        select obj;

            return SingleObjectWarnIfNoMatch(query);
        }

        #endregion

        #region RandomProduct

        [FinderAction]
        [QueryOnly]
        [MemberOrder(10)]
        public Product RandomProduct() => Random<Product>();

        #endregion

        #region NewProduct

        [FinderAction]
        [MemberOrder(9)]
        public virtual Product NewProduct() => Container.NewTransientInstance<Product>();

        #endregion

        [FinderAction]
        [QueryOnly]
        [MemberOrder(11)]
        public Product FindProductByKey(string key) => Container.FindByKey<Product>(int.Parse(key));

        #region Inventory

        /// <summary>
        ///     Action is intended to test the returing of a scalar (large multi-line string).
        /// </summary>
        /// <returns></returns>
        public string StockReport() {
            var inventories = Container.Instances<ProductInventory>().Select(pi => new InventoryLine {ProductName = pi.Product.Name, Quantity = pi.Quantity});
            var sb = new StringBuilder();
            sb.AppendLine(@"<h1 id=""report"">Stock Report</h1>");
            sb.AppendLine("<table>");
            sb.AppendLine("<tr><th>Product</th><th>Quantity</th></tr>");
            foreach (var i in inventories) {
                sb.AppendLine("<tr><td>" + i.ProductName + "</td><td>" + i.Quantity + "</td></tr>");
            }

            sb.AppendLine("</table>");
            return sb.ToString();
        }

        #endregion

        public IQueryable<ProductPhoto> AllProductPhotos() => Container.Instances<ProductPhoto>();

        private class InventoryLine {
            public string ProductName { get; set; }
            public int Quantity { get; set; }
        }

        #region FindProduct

        [FinderAction]
        [QueryOnly]
        [MemberOrder(7)]
        public Product FindProduct(Product product) => product;

        public Product Default0FindProduct() => Instances<Product>().First();

        public virtual IQueryable<Product> AutoComplete0FindProduct(string name) =>
            from obj in Instances<Product>()
            where obj.Name.ToUpper().Contains(name.ToUpper())
            orderby obj.Name
            select obj;

        #endregion

        #region ListProductsBySubCategory

        [FinderAction]
        [TableView(true, "ProductNumber", "ListPrice")] [MemberOrder(3)]
        public IQueryable<Product> ListProductsBySubCategory([ContributedAction("Products")] ProductSubcategory subCategory) =>
            from obj in Instances<Product>()
            where obj.ProductSubcategory.ProductSubcategoryID == subCategory.ProductSubcategoryID
            orderby obj.Name
            select obj;

        public virtual ProductSubcategory Default0ListProductsBySubCategory() => Instances<ProductSubcategory>().First();

        #endregion

        #region ListProducts

        [FinderAction]
        [TableView(true, "ProductNumber", "ListPrice")] [MemberOrder(3)]
        public IQueryable<Product> ListProducts(ProductCategory category, ProductSubcategory subCategory) =>
            from obj in Instances<Product>()
            where obj.ProductSubcategory.ProductSubcategoryID == subCategory.ProductSubcategoryID
            orderby obj.Name
            select obj;

        public ProductCategory Default0ListProducts() => Container.Instances<ProductCategory>().First();

        public IList<ProductSubcategory> Choices1ListProducts(ProductCategory category) {
            if (category != null) {
                return category.ProductSubcategory.ToList();
            }

            return null;
        }

        #endregion

        #region ListProductsBySubcategories

        [FinderAction]
        [MemberOrder(4)]
        [QueryOnly]
        public IList<Product> ListProductsBySubCategories(IEnumerable<ProductSubcategory> subCategories) => QueryableOfProductsBySubcat(subCategories).ToList();

        public virtual IList<ProductSubcategory> Default0ListProductsBySubCategories() {
            return new List<ProductSubcategory> {
                Instances<ProductSubcategory>().Single(psc => psc.Name == "Mountain Bikes"),
                Instances<ProductSubcategory>().Single(psc => psc.Name == "Touring Bikes")
            };
        }

        private IQueryable<Product> QueryableOfProductsBySubcat(IEnumerable<ProductSubcategory> subCategories) {
            var subCatIds = subCategories.Select(x => x.ProductSubcategoryID).ToArray();
            IQueryable<Product> q = from p in Instances<Product>()
                                    //from sc in subCatIds
                                    where subCatIds.Contains(p.ProductSubcategory.ProductSubcategoryID)
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
        public IQueryable<Product> FindProductsByCategory(IEnumerable<ProductCategory> categories, IEnumerable<ProductSubcategory> subcategories) => QueryableOfProductsBySubcat(subcategories);

        public IQueryable<ProductCategory> Choices0FindProductsByCategory() => Instances<ProductCategory>();

        public IQueryable<ProductSubcategory> Choices1FindProductsByCategory(IEnumerable<ProductCategory> categories) {
            if (categories != null) {
                var catIds = categories.Select(c => c.ProductCategoryID).ToArray();

                return from psc in Instances<ProductSubcategory>()
                       //from cid in catIds
                       where catIds.Contains(psc.ProductCategory.ProductCategoryID)
                       select psc;
            }

            return new ProductSubcategory[] { }.AsQueryable();
        }

        public IList<ProductCategory> Default0FindProductsByCategory() => new List<ProductCategory> {Instances<ProductCategory>().First()};

        public List<ProductSubcategory> Default1FindProductsByCategory() {
            var pcs = Default0FindProductsByCategory();

            if (pcs != null) {
                var ids = pcs.Select(c => c.ProductCategoryID).ToArray();

                return (from psc in Instances<ProductSubcategory>()
                        //from cid in ids
                        where ids.Contains(psc.ProductCategory.ProductCategoryID)
                        select psc).OrderBy(psc => psc.ProductSubcategoryID).Take(2).ToList();
            }

            return new List<ProductSubcategory>();
        }

        #endregion

        #region FindByProductLinesAndClasses

        [FinderAction]
        [MemberOrder(6)]
        public IQueryable<Product> FindByProductLinesAndClasses(IEnumerable<ProductLineEnum> productLine, IEnumerable<ProductClassEnum> productClass) {
            var products = Container.Instances<Product>();

            foreach (var pl in productLine) {
                var pls = Enum.GetName(typeof(ProductLineEnum), pl);
                products = products.Where(p => p.ProductLine == pls);
            }

            foreach (var pc in productClass) {
                var pcs = Enum.GetName(typeof(ProductClassEnum), pc);
                products = products.Where(p => p.Class == pcs);
            }

            return products;
        }

        public virtual IList<ProductLineEnum> Default0FindByProductLinesAndClasses() => new List<ProductLineEnum> {ProductLineEnum.M, ProductLineEnum.S};

        public virtual IList<ProductClassEnum> Default1FindByProductLinesAndClasses() => new List<ProductClassEnum> {ProductClassEnum.H};

        [FinderAction]
        [MemberOrder(7)]
        public IQueryable<Product> FindByOptionalProductLinesAndClasses([Optionally] IEnumerable<ProductLineEnum> productLine, [Optionally] IEnumerable<ProductClassEnum> productClass) {
            var products = Container.Instances<Product>();

            if (productLine != null) {
                foreach (var pl in productLine) {
                    var pls = Enum.GetName(typeof(ProductLineEnum), pl);
                    products = products.Where(p => p.ProductLine == pls);
                }
            }

            if (productClass != null) {
                foreach (var pc in productClass) {
                    var pcs = Enum.GetName(typeof(ProductClassEnum), pc);
                    products = products.Where(p => p.Class == pcs);
                }
            }

            return products;
        }

        public virtual IList<ProductLineEnum> Default0FindByOptionalProductLinesAndClasses() => new List<ProductLineEnum> {ProductLineEnum.M, ProductLineEnum.S};

        public virtual IList<ProductClassEnum> Default1FindByOptionalProductLinesAndClasses() => new List<ProductClassEnum> {ProductClassEnum.H};

        #endregion

        #region FindByProductLineAndClass

        [FinderAction]
        [MemberOrder(5)]
        public IQueryable<Product> FindByProductLineAndClass(ProductLineEnum productLine, ProductClassEnum productClass) {
            var pls = Enum.GetName(typeof(ProductLineEnum), productLine);
            var pcs = Enum.GetName(typeof(ProductClassEnum), productClass);

            return Container.Instances<Product>().Where(p => p.ProductLine == pls && p.Class == pcs);
        }

        public virtual ProductLineEnum Default0FindByProductLineAndClass() => ProductLineEnum.M;

        public virtual ProductClassEnum Default1FindByProductLineAndClass() => ProductClassEnum.H;

        #endregion
    }
}