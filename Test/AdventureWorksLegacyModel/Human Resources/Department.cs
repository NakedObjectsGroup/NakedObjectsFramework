// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Legacy.NakedObjects.Application.ValueHolder;
using NakedObjects;

namespace AdventureWorksModel {

    [Bounded]
    [Immutable]
    [LegacyType]
    public class Department  {
        #region Life Cycle Methods
        public virtual void Persisting() {
            ModifiedDate = DateTime.Now;
        }

        public virtual void Updating() {
            ModifiedDate = DateTime.Now;
        }
        #endregion

        #region Properties

        [NakedObjectsIgnore]
        public virtual short DepartmentID { get; set; }

        internal string name;
        internal TextString _name;

        [Title]
        [MemberOrder(1)]
        [NotMapped]
        public virtual TextString Name => _name ??= new TextString(name) { BackingField = s => name = s };


        internal string groupName;
        private TextString _groupName;


        [MemberOrder(2)]
        [NotMapped]
        public virtual TextString GroupName => _groupName ??= new TextString(groupName) { BackingField = s => groupName = s };

        [MemberOrder(99)]
        [Disabled]
        [ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion
    }
}