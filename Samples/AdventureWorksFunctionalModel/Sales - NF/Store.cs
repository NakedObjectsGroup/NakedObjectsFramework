// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;


using System.Linq;
using AdventureWorksFunctionalModel;
using NakedFunctions;
using NakedFunctions;

namespace AdventureWorksModel
{
        public record Store : BusinessEntity, IBusinessEntityWithContacts, IHasModifiedDate
    {

        private Store() { }

        public Store(
            string name,
            string demographics,
            int? salesPersonID,
            SalesPerson salesPerson,
            DateTime modifiedDate,
            Guid rowGuid,
            int businessEntityID,
            ICollection<BusinessEntityAddress> addresses,
            ICollection<BusinessEntityContact> contacts,
            Guid businessEntityRowguid,
            DateTime businessEntityModifiedDate
            ) : base(businessEntityID, addresses, contacts, businessEntityRowguid, businessEntityModifiedDate)
        {
            Name = name;
            Demographics = demographics;
            SalesPersonID = salesPersonID;
            SalesPerson = salesPerson;
            ModifiedDate = modifiedDate;
            rowguid = rowGuid;
        }

        #region Properties

        [Named("Store Name"), MemberOrder(20)]
        public virtual string Name { get; private set; }

        #region Demographics

        [Hidden]
        public virtual string Demographics { get; private set; }

        [Named("Demographics"), MemberOrder(30), MultiLine(NumberOfLines = 10), TypicalLength(500)]
        public virtual string FormattedDemographics
        {
            get { return Utilities.FormatXML(Demographics); }
        }

        #endregion

        #region SalesPerson
        [Hidden]
        public virtual int? SalesPersonID { get; private set; }

        
        [MemberOrder(40), FindMenu]
        public virtual SalesPerson SalesPerson { get; private set; }

        [PageSize(20)]
        public IQueryable<SalesPerson> AutoCompleteSalesPerson(
            [Range(2,0)] string name,
            IQueryable<Person> persons,
            IQueryable<SalesPerson> sps)
        {
            return SalesRepository.FindSalesPersonByName(null, name, persons, sps);
        }

        #endregion

        #region ModifiedDate and rowguid

        #region ModifiedDate

        [MemberOrder(99)]
        
        public virtual DateTime ModifiedDate { get; private set; }

        #endregion

        #region rowguid

        [Hidden]
        public virtual Guid rowguid { get; private set; }

        #endregion

        #endregion

        #endregion

    }

    public static class StoreFunctions
    {
        public static string Title(this Store s)
        {
            return s.CreateTitle(s.Name);
        }

        #region Life Cycle Methods
        public static Store Updating(
            Store sp,
            [Injected] DateTime now)
        {
            return sp with {ModifiedDate =  now};
        }
        #endregion

        public static (Store,Store) UpdateName(this Store s, string newName)
        {
            var s2 = s with {Name =  newName};
            return (s2, s2);
        }
    }
}