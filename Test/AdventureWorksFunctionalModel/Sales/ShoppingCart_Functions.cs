






using System.Linq;
using AW.Types;
using NakedFunctions;

namespace AW.Functions {
    /// <summary>
    /// </summary>
    [Named("Cart")]
    public class ShoppingCart_Functions {
        public IQueryable<ShoppingCartItem> RemoveItems(
            IQueryable<ShoppingCartItem> items,
            IContext context) {
            ShoppingCart_MenuFunctions.RemoveItems(items, context);
            return ShoppingCart_MenuFunctions.Cart(context);
        }

        public IContext AddToCart(Product product, IContext context) => ShoppingCart_MenuFunctions.AddToShoppingCart(product, context);

        public string? DisableAddToCart(IContext context) => ShoppingCart_MenuFunctions.DisableIfNoCustomerForUser(context);
    }
}