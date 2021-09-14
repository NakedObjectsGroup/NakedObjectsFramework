// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using System.Security.Principal;
using AW.Types;
using NakedFunctions;

namespace AW.Functions {
    /// <summary>
    /// </summary>
    [Named("Cart")]
    public static class ShoppingCart_MenuFunctions {
        //TODO: The Cart should probably be a view model
        [Named("Show Cart")]
        public static IQueryable<ShoppingCartItem> Cart(IContext context) {
            var id = GetShoppingCartIDForUser(context);
            return context.Instances<ShoppingCartItem>().Where(x => x.ShoppingCartID == id);
        }

        public static string DisableCart(IContext context) => DisableIfNoCustomerForUser(context);

        private static string GetShoppingCartIDForUser(IContext context) => GetCustomerForUser(context).CustomerID.ToString();

        public static IContext AddToShoppingCart(Product product, IContext context) {
            var id = GetShoppingCartIDForUser(context);
            var newItem = new ShoppingCartItem() with { ShoppingCartID = id, Product = product, Quantity = 1, DateCreated = context.Now() };
            return context.WithNew(newItem).WithInformUser($"1 x {product} added to Cart");
        }

        public static (SalesOrderHeader, IContext) CheckOut(IContext context) {
            var cust = GetCustomerForUser(context);
            throw new NotImplementedException();
        }

        public static string DisableCheckOut(IContext context) => DisableIfNoCustomerForUser(context);

        private static Customer GetCustomerForUser(IContext context) => throw new NotImplementedException();

        private static Person GetContactFromUserNameAsEmail(IContext context) => throw new NotImplementedException();

        private static string UserName(IPrincipal principal) => principal.Identity.Name;

        public static IContext AddAllItemsInCartToOrder(
            SalesOrderHeader order, IContext context) {
            var items = Cart(context);
            var details = items.Select(item => order.CreateNewDetail(item.Product, (short)item.Quantity, context));
            var context2 = details.Aggregate(context, (c, d) => c.WithNew(d));
            var context3 = EmptyCart(context);
            return context3;
        }

        public static IContext RemoveItems(IQueryable<ShoppingCartItem> items, IContext context) => throw new NotImplementedException();

        public static IContext EmptyCart(IContext context) => throw new NotImplementedException();

        public static string DisableEmptyCart(IContext context) => DisableIfNoCustomerForUser(context);

        public static string DisableIfNoCustomerForUser(IContext context) => GetCustomerForUser(context) == null ? "User is not a recognised Customer" : null;
    }
}