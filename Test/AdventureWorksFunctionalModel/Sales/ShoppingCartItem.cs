// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;

using NakedFunctions;

namespace AW.Types {
    public record ShoppingCartItem {

        [Hidden]
        public virtual int ShoppingCartItemID { get; init; }

        [Hidden]
        public virtual string ShoppingCartID { get; init; }

        [MemberOrder(20)]
        public virtual int Quantity { get; init; }

        [Hidden]
        public virtual DateTime DateCreated { get; init; }

        #region Product
        [Hidden]
        public virtual int ProductID { get; init; }

        [ MemberOrder(10)]
        public virtual Product Product { get; init; }
        #endregion

        #region ModifiedDate

        [ MemberOrder(99)]
        public virtual DateTime ModifiedDate { get; init; }

        #endregion

        public override string ToString() => $"{Quantity}  x {Product}";
    }
}