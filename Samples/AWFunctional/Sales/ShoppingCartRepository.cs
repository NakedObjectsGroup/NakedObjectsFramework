// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel;
using System.Linq;
using NakedObjects;
using NakedObjects.Services;
using NakedFunctions;
using System.Collections.Generic;

namespace AdventureWorksModel {
    /// <summary>
    /// 
    /// </summary>
    /// 
    [DisplayName("Cart")]
    public class ShoppingCartRepository : AWAbstractFactoryAndRepository {

        public QueryResultList ShowCart(IFunctionalContainer container)
        {
            return new QueryResultList(Cart(container));
        }

        [NakedObjectsIgnore]
        public static IQueryable<ShoppingCartItem> Cart(IFunctionalContainer container) {
            string id = GetShoppingCartIDForUser();
            return from sci in container.Instances<ShoppingCartItem>()
                where sci.ShoppingCartID == id
                select sci;
        }

        public static string DisableCart() {
            return DisableIfNoCustomerForUser();
        }

        private static string GetShoppingCartIDForUser() {
            return GetCustomerForUser().CustomerID.ToString();
        }

        [NakedObjectsIgnore]
        public static QueryResultList AddToShoppingCart(
            Product product,
            IFunctionalContainer container) {
            string id = GetShoppingCartIDForUser();
            var item = new ShoppingCartItem();
            item.ShoppingCartID = id;
            item.Product = product;
            item.Quantity = 1;
            item.DateCreated = DateTime.Now;
            Persist(ref item);
            string inform = "1 x " + product.Name + " added to Cart";
            return new QueryResultList(Cart(container), inform);
        }

        public static SalesOrderHeader CheckOut(IFunctionalContainer container) {
            var cust = GetCustomerForUser();
            var order = OrderContributedActions.CreateNewOrder(cust, true, container);
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

        private static Person GetContactFromUserNameAsEmail() {
            string username = UserName();

            //var q = from c in allPerson>()
            //    where c.EmailAddress.Trim().ToUpper() == username.Trim().ToUpper()
            //    select c;

            //return q.FirstOrDefault();
            return null;
        }

        private static string UserName() {
            return Container.Principal.Identity.Name;
        }

        [NakedObjectsIgnore]
        public static void AddAllItemsInCartToOrder(
            SalesOrderHeader order, 
            IFunctionalContainer container) {
            foreach (ShoppingCartItem item in Cart(container)) {
                var detail = order.AddNewDetail(item.Product, (short) item.Quantity, container);
                Container.Persist(ref detail);
            }
            EmptyCart(container);
        }

        [NakedObjectsIgnore]
        public static void RemoveItems(IEnumerable<ShoppingCartItem> items) {
            foreach (ShoppingCartItem item in items) {
                Container.DisposeInstance(item);
            }
        }

        public static void EmptyCart(IFunctionalContainer container) {
            RemoveItems(Cart(container));
        }

        public static string DisableEmptyCart() {
            return DisableIfNoCustomerForUser();
        }

        [NakedObjectsIgnore]
        public static string DisableIfNoCustomerForUser() {
            var rb = new ReasonBuilder();
            rb.AppendOnCondition(GetCustomerForUser() == null, "User is not a recognised Customer");
            return rb.Reason;
        }
    }
}