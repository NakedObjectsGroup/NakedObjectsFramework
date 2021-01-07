// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedFunctions;

namespace AW.Types
{
    public record Customer
    {
        [Hidden]
        public virtual int CustomerID { get; init; }

        [DescribedAs("xxx"), MemberOrder(10)]
        public virtual string AccountNumber { get; init; }

        #region Sales Territory
        [Hidden]
        public virtual int? SalesTerritoryID { get; init; }

        [MemberOrder(20)]
        public virtual SalesTerritory SalesTerritory { get; init; }
        #endregion

        #region Store & Personal customers


        [Hidden]
        public virtual int? StoreID { get; init; }

        [MemberOrder(20)]
        public virtual Store Store { get; init; }



        [Hidden]
        public virtual int? PersonID { get; init; }

        [MemberOrder(20)]
        public virtual Person Person { get; init; }


        #endregion



        [Hidden]
        //
        public virtual DateTime CustomerModifiedDate { get; init; }

        [Hidden]
        public virtual Guid CustomerRowguid { get; init; }

        public override string ToString() => "No Title Yet";
        // IsStore(this) ? Store : Person;

    }
}