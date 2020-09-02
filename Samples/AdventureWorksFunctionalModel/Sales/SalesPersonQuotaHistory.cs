// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel.DataAnnotations;
using NakedObjects;
using NakedFunctions;

namespace AdventureWorksModel {
    [IconName("clipboard.png")]
    public class SalesPersonQuotaHistory  {
        #region Injected Services
        public IDomainObjectContainer Container { set; protected get; }
        #endregion

        #region Life Cycle Methods
        public virtual void Persisting() {
            rowguid = Guid.NewGuid();
            ModifiedDate = DateTime.Now;
        }

        public virtual void Updating() {
            ModifiedDate = DateTime.Now;
        }
        #endregion

        [NakedObjectsIgnore]
        public virtual int BusinessEntityID { get; set; }

        [MemberOrder(1)]
        [Mask("d")]
        public virtual DateTime QuotaDate { get; set; }

        [MemberOrder(2)]
        [Mask("C")]
        public virtual decimal SalesQuota { get; set; }

        [MemberOrder(3)]
        public virtual SalesPerson SalesPerson { get; set; }

        #region ModifiedDate and rowguid

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        [ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion

        #region rowguid

        [NakedObjectsIgnore]
        public virtual Guid rowguid { get; set; }

        #endregion

        #endregion
    }

    public static class SalesPersonQuotaHistoryFunctions
    {
        public static string Title(this SalesPersonQuotaHistory t)
        {
            return t.CreateTitle($"{t.QuotaDate.ToString("d")} {t.SalesQuota.ToString("C")}");
        }
    }
}