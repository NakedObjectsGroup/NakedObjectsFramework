// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedFunctions;

namespace AW.Types {
    //TODO: Need to think how we want to do ViewModels. Can't require methods to be implemented.
    //Probably just need a constructor that takes all keys, as well as any required Injected params
    [ViewModel]
    public record StoreSalesInfo {
        public StoreSalesInfo(
            string accountNumber,
            bool editMode,
            string storeName,
            SalesTerritory salesTerritory,
            SalesPerson salesPerson) {
            AccountNumber = accountNumber;
            StoreName = storeName;
            SalesTerritory = salesTerritory;
            SalesPerson = SalesPerson;
            EditMode = editMode;
        }

        public StoreSalesInfo() { }

        [MemberOrder(1), ]
        public string AccountNumber { get; init; }

        [MemberOrder(2)]
        public string StoreName { get; init; }

        [MemberOrder(3)]
        public SalesTerritory SalesTerritory { get; init; }

        [MemberOrder(4)]
        public SalesPerson SalesPerson { get; init; }

        public bool EditMode { get; init; }
    }

   
}