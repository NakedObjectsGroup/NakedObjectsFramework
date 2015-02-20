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

namespace AdventureWorksModel {
    [DisplayName("Vendors")]
    public class VendorRepository : AbstractFactoryAndRepository {
        #region FindVendorByName

        [FinderAction]
        [TableView(true, "AccountNumber", "ActiveFlag", "PreferredVendorStatus")]
        public IQueryable<Vendor> FindVendorByName(string name) {
            return Container.Instances<Vendor>().Where(v => v.Name == name).OrderBy(v => v.Name);
        }

        #endregion;

        #region FindVendorByAccountNumber

        [FinderAction]
        [QueryOnly]
        public Vendor FindVendorByAccountNumber(string accountNumber) {
            IQueryable<Vendor> query = from obj in Instances<Vendor>()
                where obj.AccountNumber == accountNumber
                select obj;

            return SingleObjectWarnIfNoMatch(query);
        }

        #endregion

        #region Injected Services

        // This region should contain properties to hold references to any services required by the
        // object.  Use the 'injs' shortcut to add a new service.

        #endregion

        #region RandomVendor

        [FinderAction]
        [QueryOnly]
        public Vendor RandomVendor() {
            return Random<Vendor>();
        }

        public IQueryable<Vendor> AllVendorsWithWebAddresses() {
            return from obj in Instances<Vendor>()
                where obj.PurchasingWebServiceURL != null
                orderby obj.Name
                select obj;
        }

        #endregion
    }
}