// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects;

namespace AdventureWorksModel {
    [IconName("cellphone.png")]
    public class StoreContact : IContactRole {

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
        #region Contact

        [Disabled, MemberOrder(2)]
        public virtual Contact Contact { get; set; }

        #endregion

        #region Store

        [NakedObjectsIgnore]
        public virtual Store Store { get; set; }

        #endregion

        #region IContactRole Members

        #region ContactType

        [NakedObjectsIgnore]
        public virtual int ContactTypeID { get; set; }

        [MemberOrder(1)]
        public virtual ContactType ContactType { get; set; }

        #endregion

        #endregion

        #region Title & Icon

        public override string ToString() {
            var t = Container.NewTitleBuilder();
            t.Append(Contact).Append(",", ContactType);
            return t.ToString();
        }

        #endregion

        #region ID

        [NakedObjectsIgnore]
        public virtual int CustomerID { get; set; }

        [NakedObjectsIgnore]
        public virtual int ContactID { get; set; }

        #endregion

        #region ModifiedDate & rowguid

        public virtual Guid rowguid { get; set; }

        [MemberOrder(99)]
        [Disabled]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion
    }
}