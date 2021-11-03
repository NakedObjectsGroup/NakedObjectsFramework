// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel.DataAnnotations;
using Legacy.Types;
using NakedObjects;


namespace AdventureWorksModel {

    [Bounded]
    [Immutable]
    [LegacyType]
    public class Department  {
        #region Life Cycle Methods
        public virtual void Persisting()  => ModifiedDate.DateTime = DateTime.Now;

        public virtual void Updating()  => Persisting();

        #endregion

        [NakedObjectsIgnore]
        public virtual short DepartmentID { get; set; }

        #region Name
        internal string mappedName;
        internal TextString myName;

        [MemberOrder(1)]
        public virtual TextString Name => myName ??= new TextString(mappedName, s => mappedName = s);
        #endregion

        #region GroupName
        internal string mappedGroupName;
        private TextString cachedGroupName;


        [MemberOrder(2)]
        public virtual TextString GroupName => cachedGroupName ??= new TextString(mappedGroupName, s => mappedGroupName = s );
        #endregion

        #region ModifiedDate
        internal DateTime mappedModifiedDate;
        private TimeStamp myModifiedDate;

        [MemberOrder(99)]
        [Disabled]
        [ConcurrencyCheck]
        public virtual TimeStamp ModifiedDate => myModifiedDate ??= new TimeStamp(mappedModifiedDate, s => mappedModifiedDate = s);
        #endregion

        public Title title() => Name.Title();

    }
}