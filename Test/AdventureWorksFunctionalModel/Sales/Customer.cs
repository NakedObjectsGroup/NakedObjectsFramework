// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedFunctions;

namespace AW.Types {
        public record Customer 
    {

        #region Life Cycle Methods
        public virtual void Persisting() {
            CustomerRowguid = Guid.NewGuid();
            CustomerModifiedDate = DateTime.Now;
        }

        public virtual void Updating() {
            CustomerModifiedDate = DateTime.Now;
        }
        #endregion

        [Hidden]
        public virtual int CustomerID { get; set; }

        [DescribedAs("xxx"), MemberOrder(10)]
        public virtual string AccountNumber { get; set; }

        #region Sales Territory
        [Hidden]
        public virtual int? SalesTerritoryID { get; set; }

        [ MemberOrder(20)]
        public virtual SalesTerritory SalesTerritory { get; set; }
        #endregion

        #region Store & Personal customers


        [Hidden]
        public virtual int? StoreID { get; set; }

        [ MemberOrder(20)]
        public virtual Store Store { get; set; }



        [Hidden]
        public virtual int? PersonID { get; set; }

        [ MemberOrder(20)]
        public virtual Person Person { get; set; }


        #endregion



        [Hidden]
        //[ConcurrencyCheck]
        public virtual DateTime CustomerModifiedDate { get; set; }

        [Hidden]
        public virtual Guid CustomerRowguid { get; set; }

        public override string ToString() => "No Title Yet";
           // IsStore(this) ? Store : Person;

    }
}