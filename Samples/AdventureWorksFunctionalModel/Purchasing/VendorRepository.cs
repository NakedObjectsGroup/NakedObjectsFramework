// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.ComponentModel;
using System.Linq;
using NakedFunctions;
using NakedObjects;
using static AdventureWorksModel.CommonFactoryAndRepositoryFunctions;

namespace AdventureWorksModel {
    [DisplayName("Vendors")]
    public static class VendorRepository  {

        [FinderAction]
        [TableView(true, "AccountNumber", "ActiveFlag", "PreferredVendorStatus")]
        public static IQueryable<Vendor> FindVendorByName(
            string name,
            [Injected] IQueryable<Vendor> vendors) {
            return vendors.Where(v => v.Name == name).OrderBy(v => v.Name);
        }

        [FinderAction]
        public static (Vendor, string) FindVendorByAccountNumber(
            string accountNumber,
            [Injected] IQueryable<Vendor> vendors) {
            return SingleObjectWarnIfNoMatch(vendors.Where(x => x.AccountNumber == accountNumber));
        }

        [FinderAction]
        public static Vendor RandomVendor(
            [Injected] IQueryable<Vendor> vendors,
            [Injected] int random) {
            return Random(vendors, random);
        }

        public static IQueryable<Vendor> AllVendorsWithWebAddresses(
            [Injected] IQueryable<Vendor> vendors) {
            return vendors.Where(x => x.PurchasingWebServiceURL != null).OrderBy(x => x.Name);
        }
    }
}