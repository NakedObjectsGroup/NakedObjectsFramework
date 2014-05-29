// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using NakedObjects.Services;
using System.ComponentModel;
using System.Linq;
using System;
using NakedObjects;
namespace AdventureWorksModel
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    [DisplayName("Cart")]
    public class ShoppingCartRepository : AbstractFactoryAndRepository
    {
        #region Injected Services
        public OrderContributedActions OrderContributedActions { set; protected get; }
        #endregion

        [DisplayName("Show Cart")]
        public IQueryable<ShoppingCartItem> Cart()
        {
            string id = GetShoppingCartIDForUser();
            return from sci in Instances<ShoppingCartItem>()
                   where sci.ShoppingCartID == id
                   select sci;
        }

        public string DisableCart()
        {
            return DisableIfNoCustomerForUser();
        }


        private string GetShoppingCartIDForUser()
        {
            return GetCustomerForUser().Id.ToString();
        }

        [Hidden]
        public IQueryable<ShoppingCartItem> AddToShoppingCart(Product product)
        {
            string id = GetShoppingCartIDForUser();
            var item = NewTransientInstance<ShoppingCartItem>();
            item.ShoppingCartID = id;
            item.Product = product;
            item.Quantity = 1;
            item.DateCreated = DateTime.Now;
            Persist<ShoppingCartItem>(ref item);
            InformUser("1 x " + product.Name + " added to Cart");
            return Cart();
        }

        public SalesOrderHeader CheckOut()
        {
            var cust = GetCustomerForUser();
            var order = OrderContributedActions.CreateNewOrder(cust, true);
            order.AddItemsFromCart = true;
            return order;
        }

        public string DisableCheckOut()
        {
            return DisableIfNoCustomerForUser();
        }

        private Customer GetCustomerForUser()
        {
            Contact c = GetContactFromUserNameAsEmail();
            if (c == null) return null;

            var individuals = Instances<Individual>();
            var qi = from i in individuals
                     where i.Contact.ContactID == c.ContactID
                     select i;
            if (qi.Count() == 1)
            {
                return qi.First();
            }

            var stores = Instances<Store>();
            var storeContacts = Instances<StoreContact>();

            var qs = from s in storeContacts
                     where s.Contact.ContactID == c.ContactID
                     select s;
            if (qs.Count() == 1)
            {
                return qs.First().Store;
            }
            WarnUser("No Customer found with a Contact email address of: " + UserName());
            return null;
        }

        private Contact GetContactFromUserNameAsEmail()
        {
            string username = UserName();

            var q = from c in Container.Instances<Contact>()
                    where c.EmailAddress.Trim().ToUpper() == username.Trim().ToUpper()
                    select c;

            return q.FirstOrDefault();
        }

        private string UserName()
        {
            return Container.Principal.Identity.Name;
        }

        [Hidden]
        public void AddAllItemsInCartToOrder(SalesOrderHeader order)
        {
            foreach (ShoppingCartItem item in Cart())
            {
                var detail = order.AddNewDetail(item.Product, (short)item.Quantity);
                Container.Persist<SalesOrderDetail>(ref detail);
            }
            EmptyCart();
        }

        [Hidden]
        public void RemoveItems(IQueryable<ShoppingCartItem> items)
        {
            foreach (ShoppingCartItem item in items)
            {
                Container.DisposeInstance(item);
            }
        }

        public void EmptyCart()
        {
            RemoveItems(Cart());
        }

        public string DisableEmptyCart()
        {
            return DisableIfNoCustomerForUser();
        }

        [Hidden]
        public string DisableIfNoCustomerForUser()
        {
            var rb = new ReasonBuilder();
            rb.AppendOnCondition(GetCustomerForUser() == null, "User is not a recognised Customer");
            return rb.Reason;
        }

    }
}
