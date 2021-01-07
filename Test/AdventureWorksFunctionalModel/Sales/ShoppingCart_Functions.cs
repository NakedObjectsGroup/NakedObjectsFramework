// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.


using NakedFunctions;
using System.Linq;
using AW.Types;

namespace AW.Functions {
    /// <summary>
    /// 
    /// </summary>
    /// 
    [Named("Cart")]
    public record ShoppingCart_Functions {

        public IQueryable<ShoppingCartItem> RemoveItems(
            IQueryable<ShoppingCartItem> items,
            IContext context) {
            ShoppingCat_MenuFunctions.RemoveItems(items);
            return ShoppingCat_MenuFunctions.Cart(context);
        }

        public (Product, IContext) AddToCart(Product product, IContext context) =>  ShoppingCat_MenuFunctions.AddToShoppingCart(product, context);

        public string DisableAddToCart(IContext context) {
            return ShoppingCat_MenuFunctions.DisableIfNoCustomerForUser(context);
        }
    }
}