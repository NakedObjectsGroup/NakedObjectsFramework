// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.ComponentModel;
using System.Linq;
using NakedObjects;
using NakedObjects.Services;
using NakedFunctions;

namespace AdventureWorksModel {
    [DisplayName("Vendors")]
    public class VendorRepository : AWAbstractFactoryAndRepository {
        #region FindVendorByName

        
        [TableView(true, "AccountNumber", "ActiveFlag", "PreferredVendorStatus")]
        public static IQueryable<Vendor> FindVendorByName(string name, IQueryable<Vendor> allVendors) {
            return allVendors.Where(v => v.Name == name).OrderBy(v => v.Name);
        }

        #endregion;

        #region FindVendorByAccountNumber

        
        [QueryOnly]
        public static QueryResultSingle FindVendorByAccountNumber(string accountNumber, IFunctionalContainer container) {
            IQueryable<Vendor> query = from obj in container.Instances<Vendor>()
                where obj.AccountNumber == accountNumber
                select obj;

            return SingleObjectWarnIfNoMatch(query);
        }

        #endregion

        #region RandomVendor

        
        [QueryOnly]
        public static QueryResultSingle RandomVendor(IFunctionalContainer container) {
            return Random<Vendor>(container);
        }

        public static IQueryable<Vendor> AllVendorsWithWebAddresses(IFunctionalContainer container) {
            return from obj in container.Instances<Vendor>()
                where obj.PurchasingWebServiceURL != null
                orderby obj.Name
                select obj;
        }

        #endregion
    }
}