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

    [Bounded, Immutable]
    [LegacyType]
    public class ProductSubcategory {

        #region Life Cycle Methods
        public virtual void Persisting() {
            rowguid = Guid.NewGuid();
            ModifiedDate.DateTime = DateTime.Now;
        }

        public virtual void Updating()  =>  ModifiedDate.DateTime = DateTime.Now;
        #endregion

        [NakedObjectsIgnore]
        public virtual int ProductSubcategoryID { get; set; }

        #region Name
        internal string mappedName;
        private TextString myName;

        [MemberOrder(1)]
        public virtual TextString Name => myName ??= new TextString(mappedName, v => mappedName = v);
        #endregion

        [NakedObjectsIgnore]
        public virtual int ProductCategoryID { get; set; }

        public virtual ProductCategory ProductCategory { get; set; }

        #region rowguid

        [NakedObjectsIgnore]
        public virtual Guid rowguid { get; set; }

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