// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.


using System.Linq;
using NakedFunctions;
using AW.Types;
using static AW.Helpers;

namespace AW.Functions {
    [Named("Vendors")]
    public static class Vendor_MenuFunctions  {

        [TableView(true, "AccountNumber", "ActiveFlag", "PreferredVendorStatus")]
        public static IQueryable<Vendor> FindVendorByName(string name, IContext context) =>
            context.Instances<Vendor>().Where(v => v.Name == name).OrderBy(v => v.Name);

        public static Vendor FindVendorByAccountNumber(string accountNumber,IContext context) =>
            context.Instances<Vendor>().Where(x => x.AccountNumber == accountNumber).FirstOrDefault();


        public static Vendor RandomVendor(IContext context) => Random<Vendor>(context);

        public static IQueryable<Vendor> AllVendorsWithWebAddresses(IContext context) =>
            context.Instances<Vendor>().Where(x => x.PurchasingWebServiceURL != null).OrderBy(x => x.Name);
    }
}