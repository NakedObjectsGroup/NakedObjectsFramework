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
    public partial class ErrorLog {
        #region Primitive Properties

        #region ErrorLogID (Int32)

        [MemberOrder(100)]
        public virtual int ErrorLogID { get; set; }

        #endregion

        #region ErrorTime (DateTime)

        [MemberOrder(110), Mask("d")]
        public virtual DateTime ErrorTime { get; set; }

        #endregion

        #region UserName (String)

        [MemberOrder(120), StringLength(128)]
        public virtual string UserName { get; set; }

        #endregion

        #region ErrorNumber (Int32)

        [MemberOrder(130)]
        public virtual int ErrorNumber { get; set; }

        #endregion

        #region ErrorSeverity (Int32)

        [MemberOrder(140), Optionally]
        public virtual Nullable<int> ErrorSeverity { get; set; }

        #endregion

        #region ErrorState (Int32)

        [MemberOrder(150), Optionally]
        public virtual Nullable<int> ErrorState { get; set; }

        #endregion

        #region ErrorProcedure (String)

        [MemberOrder(160), Optionally, StringLength(126)]
        public virtual string ErrorProcedure { get; set; }

        #endregion

        #region ErrorLine (Int32)

        [MemberOrder(170), Optionally]
        public virtual Nullable<int> ErrorLine { get; set; }

        #endregion

        #region ErrorMessage (String)

        [MemberOrder(180), StringLength(4000)]
        public virtual string ErrorMessage { get; set; }

        #endregion

        #endregion
    }
}