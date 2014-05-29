// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using NakedObjects;
using NakedObjects.Services;

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

        [TableView(true, "ProductNumber", "ProductSubcategory", "ListPrice"), MemberOrder(1)]
        public IQueryable<Product> FindProductByName(string searchString) {
            return from obj in Instances<Product>()
                   where obj.Name.ToUpper().Contains(searchString.ToUpper())
                   orderby obj.Name
                   select obj;
        }

        #endregion

        #region FindProduct
        [QueryOnly]
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

        [TableView(true, "ProductNumber", "ListPrice"), MemberOrder(3)]
        public IQueryable<Product> ListProductsBySubCategory(ProductSubcategory subCategory) {
            return from obj in Instances<Product>()
                   where obj.ProductSubcategory.ProductSubcategoryID == subCategory.ProductSubcategoryID
                   orderby obj.Name
                   select obj;
        }

        public virtual ProductSubcategory Default0ListProductsBySubCategory() {
            return Instances<ProductSubcategory>().First();
        }

        #endregion

        #region ListProductsBySubcategories
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

        #region FindProductByNumber

        [QueryOnly, MemberOrder(2)]
        public Product FindProductByNumber(string number) {
            IQueryable<Product> query = from obj in Instances<Product>()
                                        where obj.ProductNumber == number
                                        select obj;

            return SingleObjectWarnIfNoMatch(query);
        }

        #endregion

        #region FindProductsByCategory
        public IQueryable<Product> FindProductsByCategory(IEnumerable<ProductCategory> categories, IEnumerable<ProductSubcategory> subcategories) {
            return QueryableOfProductsBySubcat(subcategories);
        }

        public IQueryable<ProductCategory> Choices0FindProductsByCategory() {
            return Instances<ProductCategory>();
        }

        public IQueryable<ProductSubcategory> Choices1FindProductsByCategory(IEnumerable<ProductCategory> categories) {
            if (categories != null) {
                var catIds = categories.Select(c => c.ProductCategoryID);

                return from psc in Instances<ProductSubcategory>()
                       from cid in catIds
                       where psc.ProductCategory.ProductCategoryID == cid
                       select psc;
            }
            return new ProductSubcategory[] { }.AsQueryable();
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
  
        #region RandomProduct
        [QueryOnly]
        public Product RandomProduct() {
            return Random<Product>();
        } 
        #endregion

        #region QueryProducts
        public IQueryable<Product> QueryProducts([Optionally, TypicalLength(40)] string whereClause,
                                                 [Optionally, TypicalLength(40)] string orderByClause,
                                                 bool descending) {
            IQueryable<Product> q = DynamicQuery<Product>(whereClause, orderByClause, descending);
            return q;
        }

        public virtual string ValidateQueryProducts(string whereClause, string orderByClause, bool descending) {
            return ValidateDynamicQuery<Product>(whereClause, orderByClause, descending);
        } 
        #endregion

        #region NewProduct
        public virtual Product NewProduct() {
            return Container.NewTransientInstance<Product>();
        } 
        #endregion

        #region FindByProductLinesAndClasses
        public IQueryable<Product> FindByProductLinesAndClasses(IEnumerable<ProductLineEnum> productLine, IEnumerable<ProductClassEnum> productClass) {
            IQueryable<Product> products = Container.Instances<Product>();

            foreach (ProductLineEnum pl in productLine) {
                string pls = Enum.GetName(typeof(ProductLineEnum), pl);
                products = products.Where(p => p.ProductLine == pls);
            }

            foreach (ProductClassEnum pc in productClass) {
                string pcs = Enum.GetName(typeof(ProductClassEnum), pc);
                products = products.Where(p => p.Class == pcs);
            }

            return products;
        }


        public virtual IList<ProductLineEnum> Default0FindByProductLinesAndClasses() {
            return new List<ProductLineEnum> { ProductLineEnum.M, ProductLineEnum.S };
        }

        public virtual IList<ProductClassEnum> Default1FindByProductLinesAndClasses() {
            return new List<ProductClassEnum>() { ProductClassEnum.H };
        } 
        #endregion

        #region FindByProductLineAndClass
        public IQueryable<Product> FindByProductLineAndClass(ProductLineEnum productLine, ProductClassEnum productClass) {
            string pls = Enum.GetName(typeof(ProductLineEnum), productLine);
            string pcs = Enum.GetName(typeof(ProductClassEnum), productLine);

            return Container.Instances<Product>().Where(p => p.ProductLine == pls && p.Class == pcs);
        }

        public virtual ProductLineEnum Default0FindByProductLineAndClass() {
            return ProductLineEnum.M;
        }

        public virtual ProductClassEnum Default1FindByProductLineAndClass() {
            return ProductClassEnum.H;
        } 
        #endregion
    }
}