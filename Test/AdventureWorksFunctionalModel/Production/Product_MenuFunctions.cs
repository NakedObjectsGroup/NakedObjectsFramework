// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Linq;
using NakedFunctions;
using AW.Types;
using static AW.Helpers;

namespace AW.Functions {

    [Named("Products")]
    public static class Product_MenuFunctions
    {

        [MemberOrder(1)]
        [TableView(true, nameof(Product.ProductNumber), nameof(Product.ProductSubcategory), nameof(Product.ListPrice))]
        public static IQueryable<Product> FindProductByName(string searchString, IContext context)
        => context.Instances<Product>().Where(x => x.Name.ToUpper().Contains(searchString.ToUpper())).OrderBy(x => x.Name);

       [MemberOrder(2)]
        public static Product RandomProduct(IContext context) => Random<Product>(context);

        [MemberOrder(3)]
        public static (Product, IContext) FindProductByNumber(string number, IContext context)
 => context.Instances<Product>().Where(x => x.ProductNumber == number).SingleObjectWarnIfNoMatch(context);

        [MemberOrder(4)]
        public static (Product, IContext) FindProductByNumber2(string number, IContext context)
 => (context.Instances<Product>().Where(x => x.ProductNumber == number).FirstOrDefault(), context);

        [MemberOrder(5)]
        public static Product FindProductByNumber3(string number, IContext context)
=> context.Instances<Product>().Where(x => x.ProductNumber == number).FirstOrDefault();

        [MemberOrder(6)]
        public static (Product, IContext) FindProductByNumber4(string number, IContext context)
=> (null, context);

    }
}