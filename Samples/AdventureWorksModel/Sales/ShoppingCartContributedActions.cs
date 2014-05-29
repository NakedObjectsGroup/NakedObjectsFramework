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
    public class ShoppingCartContributedActions : AbstractFactoryAndRepository
    {
        #region Injected Services
        public ShoppingCartRepository ShoppingCartRepository { set; protected get; }

        #endregion


        public IQueryable<ShoppingCartItem> RemoveItems(IQueryable<ShoppingCartItem> items)
        {
            ShoppingCartRepository.RemoveItems(items);
            return ShoppingCartRepository.Cart();
        }


        public void AddToCart(Product product)
        {
            ShoppingCartRepository.AddToShoppingCart(product);
        }

        public string DisableAddToCart()
        {
            return ShoppingCartRepository.DisableIfNoCustomerForUser();
        }


    }
}
