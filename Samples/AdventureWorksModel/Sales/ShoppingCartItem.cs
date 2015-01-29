// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using NakedObjects;

namespace AdventureWorksModel {
    public class ShoppingCartItem : AWDomainObject {

        
        public string Title()
        {
            var t = Container.NewTitleBuilder();
            t.Append(Quantity).Append(" x", Product);
            return t.ToString();
        }
      

        [Hidden]
        public virtual int ShoppingCartItemID { get; set; }

         [Hidden]
        public virtual string ShoppingCartID { get; set; }

        [MemberOrder(20)]
        public virtual int Quantity { get; set; }

        [Hidden]
        public virtual DateTime DateCreated { get; set; }

        [Disabled, MemberOrder(10)]
        public virtual Product Product { get; set; }

        #region ModifiedDate

        [Disabled, MemberOrder(99)]
        public override DateTime ModifiedDate { get; set; }

        #endregion
    }
}