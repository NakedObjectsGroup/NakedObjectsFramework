// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using System.Security.Principal;
using NakedFunctions;
using AW.Types;

namespace AW.Functions {
    /// <summary>
    /// 
    /// </summary>
    /// 
    [Named("Cart")]
    public static class ShoppingCat_MenuFunctions {

        //TODO: The Cart should probably be a view model
        [Named("Show Cart")]
        public static IQueryable<ShoppingCartItem> Cart(IContext context) {
            string id = GetShoppingCartIDForUser(context);
            return context.Instances<ShoppingCartItem>().Where(x => x.ShoppingCartID == id);
        }

        public static string DisableCart(IContext context) {
            return DisableIfNoCustomerForUser(context);
        }

        private static string GetShoppingCartIDForUser(IContext context) {
            return GetCustomerForUser(context).CustomerID.ToString();
        }

        public static (Product, IContext) AddToShoppingCart(Product product, IContext context) {
            string id = GetShoppingCartIDForUser(context);
            var item = new ShoppingCartItem() with { ShoppingCartID = id, Product = product, Quantity = 1, DateCreated = context.Now()};
            return (product, context.WithPendingSave(item).WithInformUser($"1 x {product} added to Cart"));
        }

        public static  (SalesOrderHeader, IContext) CheckOut(IContext context) {
            var cust = GetCustomerForUser(context);
            var (order, context2) = Order_AdditionalFunctions.CreateNewOrder(cust, true, context);
            //TODO: Need to check idea of modifying an instance that is pending save from another method
            order.AddItemsFromCart = true;
            return (order, context2);
        }

        public static string DisableCheckOut(IContext context) {
            return DisableIfNoCustomerForUser(context);
        }

        private static Customer GetCustomerForUser(IContext context) {
            throw new NotImplementedException();
            //Person c = GetContactFromUserNameAsEmail(context);
            //if (c == null) return null;

            //var individuals = context.Instances<Customer>();
            //var qi = from i in individuals
            //         where i.Contact.BusinessEntityID == c.BusinessEntityID
            //         select i;
            //if (qi.Count() == 1)
            //{
            //    return qi.First();
            //}

            //var stores = context.Instances<Store>();
            //var storeContacts = context.Instances<StoreContact>();

            //var qs = from s in storeContacts
            //         where s.Contact.BusinessEntityID == c.BusinessEntityID
            //         select s;
            //if (qs.Count() == 1)
            //{
            //    return qs.First().Store;
            //}
            //WarnUser("No Customer found with a Contact email address of: " + UserName());
            //return null;
        }

        private static Person GetContactFromUserNameAsEmail(IContext context) {
            string username = UserName(context.CurrentUser()).Trim().ToUpper();
            var q = from e in  context.Instances<EmailAddress>()
                    where e.EmailAddress1.Trim().ToUpper() == username
                    select e.Person;
            return q.FirstOrDefault();
        }

        private static string UserName(IPrincipal principal) {
            return principal.Identity.Name;
        }

        public static (SalesOrderHeader, IContext) AddAllItemsInCartToOrder(
            SalesOrderHeader order, IContext context) {

            var items = Cart(context);
            var details = items.Select(item => order.AddNewDetail(item.Product, (short) item.Quantity, context));
            EmptyCart(context);
            return (order, context.WithPendingSave(details));
        }

        public static void RemoveItems(IQueryable<ShoppingCartItem> items) {
            foreach (ShoppingCartItem item in items) {

                //TODO: how to handle this>
                //Container.DisposeInstance(item);
            }
        }

        public static void EmptyCart(IContext context) {
            RemoveItems(Cart(context));
        }

        public static string DisableEmptyCart(IContext context) =>  DisableIfNoCustomerForUser(context);

        public static string DisableIfNoCustomerForUser(IContext context) =>  GetCustomerForUser(context) == null? "User is not a recognised Customer": null;
    }
}