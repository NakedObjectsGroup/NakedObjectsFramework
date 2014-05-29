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

        [TableView(true, "AccountNumber", "ActiveFlag", "PreferredVendorStatus")]
        public IQueryable<Vendor> FindVendorByName(string name)
        {
            return FindByTitle<Vendor>(name).OrderBy(x => x.Name);
        }

        #endregion;

        #region FindVendorByAccountNumber

        [QueryOnly]
        public Vendor FindVendorByAccountNumber(string accountNumber) {
            IQueryable<Vendor> query = from obj in Instances<Vendor>()
                                       where obj.AccountNumber == accountNumber
                                       select obj;

            return SingleObjectWarnIfNoMatch(query);
        }

        #endregion

        #region RandomVendor

        [QueryOnly]
        public Vendor RandomVendor() {
            return Random<Vendor>();
        }

        public IQueryable<Vendor> AllVendorsWithWebAddresses()
        {
            return from obj in Instances<Vendor>()
                                       where obj.PurchasingWebServiceURL != null
                                       orderby obj.Name
                                       select obj;
        }

        #endregion

        #region Query Vendors

        public IQueryable<Vendor> QueryVendors([Optionally, TypicalLength(40)] string whereClause,
                                          [Optionally, TypicalLength(40)] string orderByClause,
                                          bool descending) {
            return DynamicQuery<Vendor>(whereClause, orderByClause, descending);
        }


        public virtual string ValidateQueryVendors(string whereClause, string orderByClause, bool descending) {
            return ValidateDynamicQuery<Vendor>(whereClause, orderByClause, descending);
        }

        #endregion
    }
}