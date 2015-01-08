// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.ComponentModel;
using System.Linq;
using NakedObjects;
using NakedObjects.Services;

namespace AdventureWorksModel {
    [DisplayName("Vendors")]
    public class VendorRepository : AbstractFactoryAndRepository {
        #region Injected Services

        // This region should contain properties to hold references to any services required by the
        // object.  Use the 'injs' shortcut to add a new service.

        #endregion

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