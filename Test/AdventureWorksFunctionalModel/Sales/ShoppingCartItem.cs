






using System;
using NakedFunctions;

namespace AW.Types {
    public class ShoppingCartItem {
        [Hidden]
        public int ShoppingCartItemID { get; init; }

        [Hidden]
        public string ShoppingCartID { get; init; } = "";

        [MemberOrder(20)]
        public int Quantity { get; init; }

        [Hidden]
        public DateTime DateCreated { get; init; }

        #region ModifiedDate

        [MemberOrder(99)]
        [Versioned]
        public DateTime ModifiedDate { get; init; }

        #endregion

        
        public override string ToString() => $"{Quantity}  x {Product}";



        #region Product

        [Hidden]
        public int ProductID { get; init; }

        [MemberOrder(10)]

        public virtual Product Product { get; init; }


        #endregion
    }
}