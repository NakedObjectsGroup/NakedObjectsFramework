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
    public class SpecialOffer {

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
        public virtual int SpecialOfferID { get; set; }

        [MemberOrder(10)]
        public virtual string Description { get; set; }

        [MemberOrder(20)]
        [Mask("P")]
        public virtual decimal DiscountPct { get; set; }

        [MemberOrder(30)]
        public virtual string Type { get; set; }

        [MemberOrder(40)]
        public virtual string Category { get; set; }

        [MemberOrder(51)]
        [Mask("d")]
        public virtual DateTime StartDate { get; set; }

        [MemberOrder(52)]
        [Mask("d")]
        public virtual DateTime EndDate { get; set; }

        [MemberOrder(61)]
        public virtual int MinQty { get; set; }

        [Optionally]
        [MemberOrder(62)]
        public virtual int? MaxQty { get; set; }

        public virtual string[] ChoicesCategory() {
            return new[] {"Reseller", "Customer"};
        }

        public virtual DateTime DefaultStartDate() {
            return DateTime.Now;
        }

        public virtual DateTime DefaultEndDate() {
            return DateTime.Now.AddDays(90);
        }

        #region Title

        public override string ToString() {
            var t = Container.NewTitleBuilder();
            t.Append(Description);
            return t.ToString();
        }

        public virtual string IconName() {
            if (Type == "No Discount") {
                return "default.png";
            }
            return "scissors.png";
        }

        #endregion

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
}