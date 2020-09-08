// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Principal;
using AdventureWorksModel;
using NakedFunctions;
using static NakedFunctions.Helpers;

namespace AdventureWorksFunctionalModel
{
    public static class ProductEVMFunctions
    {

        #region Life cycle
        public static string[] DeriveKeys(this ProductEVM vm)
        => new string[]
        {
            vm.ProductID.ToString(),
            vm.Color,
        };
        public static ProductEVM PopulateUsingKeys(string[] keys)
    => new ProductEVM
    {
        ProductID = Convert.ToInt32(keys[0]),
        Color = keys[1]
    };

        public static ProductEVM CreateFrom(Product p)
   => new ProductEVM { 
       ProductID = p.ProductID, 
       Color = p.Color 
   }; //TODO: all relevant props

        public static (Product, Product) Save(this ProductEVM vm)
              => DisplayAndPersist(new Product()
              {
                  ProductID = vm.ProductID,
                  Color = vm.Color

              });


        #endregion

        #region Property functions
        #region Product Line
        public static string[] ChoicesProductLine(this Product p)
        => new[] { "R ", "M ", "T ", "S " };  // nchar(2) in database so pad right with space
    #endregion

    #region Class
    public static string[] ChoicesClass(this Product p)
    => new[] { "H ", "M ", "L " }; // nchar(2) in database so pad right with space
    #endregion

    #region Style
    public static string[] ChoicesStyle(this Product p)
    => new[] { "U ", "M ", "W " }; // nchar(2) in database so pad right with space
    #endregion

    #region Product Model
    public static IQueryable<ProductModel> AutoCompleteProductModel(Product p, string match, IQueryable<ProductModel> models)
    {
        return models.Where(pm => pm.Name.ToUpper().Contains(match.ToUpper()));
    }
    #endregion

    #region Categories
    public static IList<ProductSubcategory> ChoicesCategories(
        Product p,
        ProductCategory productCategory,
        IQueryable<ProductSubcategory> subCats)
    {
        if (productCategory != null)
        {
            return subCats.Where(psc => psc.ProductCategory.ProductCategoryID == productCategory.ProductCategoryID).ToList();
        }
        return new ProductSubcategory[] { }.ToList();
    }
        #endregion
        #endregion
    }
}