// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel.DataAnnotations;
using NakedObjects;

namespace AdventureWorksModel {
    [IconName("currency.png")]
    public class CurrencyRate {

        #region Life Cycle Methods
        public virtual void Persisting() {
            ModifiedDate = DateTime.Now;
        }

        public virtual void Updating() {
            ModifiedDate = DateTime.Now;
        }
        #endregion

        [NakedObjectsIgnore]
        public virtual int CurrencyRateID { get; set; }

        public virtual DateTime CurrencyRateDate { get; set; }

        [Title]
        public virtual decimal AverageRate { get; set; }

        public virtual decimal EndOfDayRate { get; set; }

        [NakedObjectsIgnore]
        public virtual string FromCurrencyCode { get; set; }
        public virtual Currency Currency { get; set; }

        [NakedObjectsIgnore]
        public virtual string ToCurrencyCode { get; set; }

        public virtual Currency Currency1 { get; set; }

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        [ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion
    }
}