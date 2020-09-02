// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Linq;
using NakedFunctions;
using NakedObjects;
using static NakedFunctions.Result;

namespace AdventureWorksModel {
    //TODO: Need to think how we want to do ViewModels. Can't require methods to be implemented.
    //Probably just need a constructor that takes all keys, as well as any required Injected params
    [ViewModel]
    public class StoreSalesInfo {
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

        [MemberOrder(1), Disabled]
        public string AccountNumber { get; set; }

        [MemberOrder(2)]
        public string StoreName { get; set; }

        [MemberOrder(3)]
        public SalesTerritory SalesTerritory { get; set; }

        [MemberOrder(4)]
        public SalesPerson SalesPerson { get; set; }

        public bool EditMode { get; set; }
    }

    public static class StoreSalesInfoFunctions {
        public static string Title(this StoreSalesInfo vm,
                                   [Injected] IQueryable<Customer> customers) {
            return $"{(vm.EditMode ? "Editing - " : "")} Sales Info for: {vm.StoreName}";
        }

        public static string[] DeriveKeys(StoreSalesInfo vm) {
            return new string[] {vm.AccountNumber, vm.EditMode.ToString()};
        }

        public static StoreSalesInfo PopulateUsingKeys(StoreSalesInfo vm,
                                                       string[] keys,
                                                       [Injected] IQueryable<Customer> customers) {
            var cus = CustomerRepository.FindCustomerByAccountNumber(keys[0], customers).Item1;
            return vm.With(x => x.AccountNumber, keys[0])
                .With(x => x.SalesTerritory, cus.SalesTerritory)
                .With(x => x.SalesPerson, cus.Store?.SalesPerson)
                .With(x => x.EditMode, bool.Parse(keys[1]));
        }

        public static StoreSalesInfo Edit(this StoreSalesInfo ssi) {
            return ssi.With(x => x.EditMode, true);
        }

        public static bool HideEdit(this StoreSalesInfo ssi) {
            return IsEditView(ssi);
        }

        public static bool IsEditView(StoreSalesInfo ssi)
        {
            return ssi.EditMode;
        }

        public static (Customer, Customer) Save(this StoreSalesInfo vm,
                                                [Injected] IQueryable<Customer> customers) {
            var (cus, _) = CustomerRepository.FindCustomerByAccountNumber(vm.AccountNumber, customers);
            var st = vm.SalesTerritory;
            var sp = vm.SalesPerson;
            var sn = vm.StoreName;
            var cus2 = cus
                .With(c => c.SalesTerritory, st)
                .With(c => c.Store.SalesPerson, sp)
                .With(c => c.Store.Name, sn);
            return DisplayAndPersist(cus2);
        }

        public static bool HideSave(this StoreSalesInfo ssi) {
            return !IsEditView(ssi);
        }
    }
}