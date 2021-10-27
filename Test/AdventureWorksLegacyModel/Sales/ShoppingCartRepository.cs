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

namespace AdventureWorksModel {
    /// <summary>
    /// 
    /// </summary>
    /// 
    [DisplayName("Cart")]
    public class ShoppingCartRepository : AbstractFactoryAndRepository {
        #region Injected Services

        public OrderContributedActions OrderContributedActions { set; protected get; }

        #endregion

        [DisplayName("Show Cart")]
        public IQueryable<ShoppingCartItem> Cart() {
            string id = GetShoppingCartIDForUser();
            return from sci in Instances<ShoppingCartItem>()
                where sci.ShoppingCartID == id
                select sci;
        }

        public string DisableCart() {
            return DisableIfNoCustomerForUser();
        }

        private string GetShoppingCartIDForUser() {
            return GetCustomerForUser().CustomerID.ToString();
        }

        [NakedObjectsIgnore]
        public IQueryable<ShoppingCartItem> AddToShoppingCart(Product product) {
            string id = GetShoppingCartIDForUser();
            var item = NewTransientInstance<ShoppingCartItem>();
            item.ShoppingCartID = id;
            item.Product = product;
            item.Quantity = 1;
            item.DateCreated = DateTime.Now;
            Persist(ref item);
            InformUser("1 x " + product.Name + " added to Cart");
            return Cart();
        }

        public SalesOrderHeader CheckOut() {
            var cust = GetCustomerForUser();
            var order = OrderContributedActions.CreateNewOrder(cust, true);
            order.AddItemsFromCart = true;
            return order;
        }

        public string DisableCheckOut() {
            return DisableIfNoCustomerForUser();
        }

        private Customer GetCustomerForUser() {
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

        private Person GetContactFromUserNameAsEmail() {
            string username = UserName();

            //var q = from c in Container.Instances<Person>()
            //    where c.EmailAddress.Trim().ToUpper() == username.Trim().ToUpper()
            //    select c;

            //return q.FirstOrDefault();
            return null;
        }

        private string UserName() {
            return Container.Principal.Identity.Name;
        }

        [NakedObjectsIgnore]
        public void AddAllItemsInCartToOrder(SalesOrderHeader order) {
            foreach (ShoppingCartItem item in Cart()) {
                var detail = order.AddNewDetail(item.Product, (short) item.Quantity);
                Container.Persist(ref detail);
            }
            EmptyCart();
        }

        [NakedObjectsIgnore]
        public void RemoveItems(IQueryable<ShoppingCartItem> items) {
            foreach (ShoppingCartItem item in items) {
                Container.DisposeInstance(item);
            }
        }

        public void EmptyCart() {
            RemoveItems(Cart());
        }

        public string DisableEmptyCart() {
            return DisableIfNoCustomerForUser();
        }

        [NakedObjectsIgnore]
        public string DisableIfNoCustomerForUser() {
            var rb = new ReasonBuilder();
            rb.AppendOnCondition(GetCustomerForUser() == null, "User is not a recognised Customer");
            return rb.Reason;
        }
    }
}