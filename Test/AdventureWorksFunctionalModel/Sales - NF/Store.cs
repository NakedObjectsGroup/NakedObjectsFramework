// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using NakedFunctions;
using static AdventureWorksModel.Helpers;

namespace AdventureWorksModel
{
        public record Store : BusinessEntity, IBusinessEntityWithContacts, IHasModifiedDate
    {

         #region Properties

        [Named("Store Name"), MemberOrder(20)]
        public virtual string Name { get; init; }

        #region Demographics

        [Hidden]
        public virtual string Demographics { get; init; }

        [Named("Demographics"), MemberOrder(30), MultiLine(10)]
        public virtual string FormattedDemographics
        {
            get { return Utilities.FormatXML(Demographics); }
        }

        #endregion

        #region SalesPerson
        [Hidden]
        public virtual int? SalesPersonID { get; init; }

        
        [MemberOrder(40)]
        public virtual SalesPerson SalesPerson { get; init; }

        [PageSize(20)]
        public IQueryable<SalesPerson> AutoCompleteSalesPerson(
            [Range(2,0)] string name, IContainer container) =>
            Sales_MenuFunctions.FindSalesPersonByName(null, name, container);


        #endregion

        #region ModifiedDate and rowguid

        #region ModifiedDate

        [MemberOrder(99)]
        
        public virtual DateTime ModifiedDate { get; init; }

        #endregion

        #region rowguid

        [Hidden]
        public virtual Guid rowguid { get; init; }

        #endregion

        #endregion

        #endregion

        public override string ToString() => Name;

    }

    public static class StoreFunctions
    {

        #region Life Cycle Methods
        public static Store Updating(
            Store sp,
            [Injected] DateTime now)
        {
            return sp with { ModifiedDate = now };
        }
        #endregion

        public static (Store, IContainer) UpdateName(this Store s, string newName, IContainer container) =>
           DisplayAndSave(s with { Name = newName }, container);
    }
}