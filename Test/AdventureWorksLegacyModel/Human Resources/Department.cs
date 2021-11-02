// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Legacy.NakedObjects.Application;
using Legacy.NakedObjects.Application.ValueHolder;
using NakedObjects;

namespace AdventureWorksModel {

    [Bounded]
    [Immutable]
    [LegacyType]
    public class Department  {
        #region Life Cycle Methods
        public virtual void Persisting()  => ModifiedDate.setValue(DateTime.Now);

        public virtual void Updating()  => Persisting();

        #endregion

        #region Properties

        [NakedObjectsIgnore]
        public virtual short DepartmentID { get; set; }

        internal string mappedName;
        internal TextString cachedName;

        [MemberOrder(1)]
        public virtual TextString Name => cachedName ??= new TextString(mappedName, s => mappedName = s);


        internal string mappedGroupName;
        private TextString cachedGroupName;


        [MemberOrder(2)]
        public virtual TextString GroupName => cachedGroupName ??= new TextString(mappedGroupName, s => mappedGroupName = s );


        internal string mappedModifiedDate;
        private TextString cachedModifiedDate;

        [MemberOrder(99)]
        [Disabled]
        [ConcurrencyCheck]
        public virtual TimeStamp ModifiedDate => cachedModifiedDate ??= new TimeStamp(mappedModifiedDate, s => mappedModifiedDate = s);

        #endregion

        public Title title() => Name.title();

    }
}