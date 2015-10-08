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
    public class VendorContact : IContactRole {
        #region Injected Services
        public IDomainObjectContainer Container { set; protected get; }
        #endregion

        [NakedObjectsIgnore]
        public virtual int VendorID { get; set; }

        [NakedObjectsIgnore]
        public virtual int ContactID { get; set; }

        [Disabled]
        public virtual Contact Contact { get; set; }

        [NakedObjectsIgnore]
        public virtual Vendor Vendor { get; set; }

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion

        #region IContactRole Members

        [NakedObjectsIgnore]
        public virtual int ContactTypeID { get; set; }

        public virtual ContactType ContactType { get; set; }

        #endregion

        #region Title

        public void Persisted() {
            Vendor.Contacts.Add(this);
        }

        public override string ToString() {
            var t = Container.NewTitleBuilder();
            t.Append(Contact).Append(":", ContactType);
            return t.ToString();
        }

        #endregion
    }
}