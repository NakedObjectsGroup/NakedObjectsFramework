﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using NakedFunctions;

namespace AdventureWorksModel {
    /// <summary>
    /// 
    /// </summary>
    /// 
    [Named("Cart")]
    public static class ShoppingCartRepository {

        //TODO: The Cart should probably be a view model
        [Named("Show Cart")]
        public static IQueryable<ShoppingCartItem> Cart(IContext context) {
            string id = GetShoppingCartIDForUser();
            return context.Instances<ShoppingCartItem>().Where(x => x.ShoppingCartID == id);
        }

        public static string DisableCart() {
            return DisableIfNoCustomerForUser();
        }

        private static string GetShoppingCartIDForUser() {
            return GetCustomerForUser().CustomerID.ToString();
        }

        public static IQueryable<ShoppingCartItem> AddToShoppingCart(Product product) {
            //TODO: Transient object
            throw new NotImplementedException();
            //string id = GetShoppingCartIDForUser();
            //var item = NewTransientInstance<ShoppingCartItem>();
            //item.ShoppingCartID = id;
            //item.Product = product;
            //item.Quantity = 1;
            //item.DateCreated = DateTime.Now;
            //Persist(ref item);
            //InformUser("1 x " + product.Name + " added to Cart");
            //return Cart();
        }

        public static  SalesOrderHeader CheckOut(
            IQueryable<BusinessEntityAddress> addresses,
            IQueryable<SalesOrderHeader> headers) {
            var cust = GetCustomerForUser();
            var order = OrderContributedActions.CreateNewOrder(cust, true, addresses, headers);
            order.AddItemsFromCart = true;
            return order;
        }

        public static string DisableCheckOut() {
            return DisableIfNoCustomerForUser();
        }

        private static Customer GetCustomerForUser() {
            throw new NotImplementedException();
            //Contact c = GetContactFromUserNameAsEmail();
            //if (c == null) return null;

            //var individuals = Instances<Individual>();
            //var qi = from i in individuals
            //    where i.Contact.BusinessEntityID == c.BusinessEntityID
            //    select i;
            //if (qi.Count() == 1) {
            //    return qi.First();
            //}

            //var stores = Instances<Store>();
            //var storeContacts = Instances<StoreContact>();

            //var qs = from s in storeContacts
            //         where s.Contact.BusinessEntityID == c.BusinessEntityID
            //    select s;
            //if (qs.Count() == 1) {
            //    return qs.First().Store;
            //}
            //WarnUser("No Customer found with a Contact email address of: " + UserName());
            //return null;
        }

        private static Person GetContactFromUserNameAsEmail(IPrincipal principal) {
            string username = UserName(principal);

            //var q = from c in Container.Instances<Person>()
            //    where c.EmailAddress.Trim().ToUpper() == username.Trim().ToUpper()
            //    select c;

            //return q.FirstOrDefault();
            return null;
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

        public static string DisableEmptyCart() =>  DisableIfNoCustomerForUser();

        public static string DisableIfNoCustomerForUser() =>  GetCustomerForUser() == null? "User is not a recognised Customer": null;
    }
}