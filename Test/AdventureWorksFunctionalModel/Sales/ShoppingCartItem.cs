// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel.DataAnnotations;
using NakedFunctions;

namespace AW.Types {
    public record ShoppingCartItem {

        [Hidden]
        public virtual int ShoppingCartItemID { get; set; }

        [Hidden]
        public virtual string ShoppingCartID { get; set; }

        [MemberOrder(20)]
        public virtual int Quantity { get; set; }

        [Hidden]
        public virtual DateTime DateCreated { get; set; }

        #region Product
        [Hidden]
        public virtual int ProductID { get; set; }

        [ MemberOrder(10)]
        public virtual Product Product { get; set; }
        #endregion

        #region ModifiedDate

        [ MemberOrder(99),ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion

        public override string ToString() => $"{Quantity}  x {Product}";
    }
}