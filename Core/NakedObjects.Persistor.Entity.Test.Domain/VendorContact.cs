// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects;

namespace AdventureWorksModel {
    // ReSharper disable once PartialTypeWithSinglePart

    public partial class VendorContact {
        #region Primitive Properties

        #region VendorID (Int32)

        [MemberOrder(100)]
        public virtual int VendorID { get; set; }

        #endregion

        #region ContactID (Int32)

        [MemberOrder(110)]
        public virtual int ContactID { get; set; }

        #endregion

        #region ModifiedDate (DateTime)

        [MemberOrder(120), Mask("d")]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion

        #endregion

        #region Navigation Properties

        #region Contact (Contact)

        [MemberOrder(130)]
        public virtual Contact Contact { get; set; }

        #endregion

        #region ContactType (ContactType)

        [MemberOrder(140)]
        public virtual ContactType ContactType { get; set; }

        #endregion

        #region Vendor (Vendor)

        [MemberOrder(150)]
        public virtual Vendor Vendor { get; set; }

        #endregion

        #endregion
    }
}