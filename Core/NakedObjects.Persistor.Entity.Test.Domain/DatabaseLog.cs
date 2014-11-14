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
    public partial class DatabaseLog {
        #region Primitive Properties

        #region DatabaseLogID (Int32)

        [MemberOrder(100)]
        public virtual int DatabaseLogID { get; set; }

        #endregion

        #region PostTime (DateTime)

        [MemberOrder(110), Mask("d")]
        public virtual DateTime PostTime { get; set; }

        #endregion

        #region DatabaseUser (String)

        [MemberOrder(120), StringLength(128)]
        public virtual string DatabaseUser { get; set; }

        #endregion

        #region Event (String)

        [MemberOrder(130), StringLength(128)]
        public virtual string Event { get; set; }

        #endregion

        #region Schema (String)

        [MemberOrder(140), Optionally, StringLength(128)]
        public virtual string Schema { get; set; }

        #endregion

        #region Object (String)

        [MemberOrder(150), Optionally, StringLength(128)]
        public virtual string Object { get; set; }

        #endregion

        #region TSQL (String)

        [MemberOrder(160)]
        public virtual string TSQL { get; set; }

        #endregion

        #region XmlEvent (String)

        [MemberOrder(170)]
        public virtual string XmlEvent { get; set; }

        #endregion

        #endregion
    }
}