// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using NakedFramework;
using NakedObjects;

namespace AdventureWorksModel {
    [DisplayName("Reason")]
    [Immutable(WhenTo.OncePersisted)]
    public class SalesOrderHeaderSalesReason {
        #region Injected Services
        public IDomainObjectContainer Container { set; protected get; }
        #endregion

        #region Life Cycle Methods
        public virtual void Persisting() {
            ModifiedDate = DateTime.Now;
        }

        public virtual void Updating() {
            ModifiedDate = DateTime.Now;
        }
        #endregion

        [NakedObjectsIgnore]
        public virtual int SalesOrderID { get; set; }

        public virtual int SalesReasonID { get; set; }
        public virtual SalesOrderHeader SalesOrderHeader { get; set; }
        public virtual SalesReason SalesReason { get; set; }

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        [ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion

        public override string ToString() {
            var t = Container.NewTitleBuilder();
            t.Append(SalesReason);
            return t.ToString();
        }
    }
}