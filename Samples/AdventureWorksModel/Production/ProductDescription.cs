// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using NakedObjects;

namespace AdventureWorksModel {
    [IconName("information")]
    public class ProductDescription : AWDomainObject {
        private ICollection<ProductModelProductDescriptionCulture> _ProductModelProductDescriptionCulture = new List<ProductModelProductDescriptionCulture>();

        [Hidden]
        public virtual int ProductDescriptionID { get; set; }

        [Title]
        [MultiLine(NumberOfLines = 10)]
        [TypicalLength(100)]
        [MemberOrder(2)]
        public virtual string Description { get; set; }

        #region Row Guid and Modified Date

        #region rowguid

        [Hidden]
        public override Guid rowguid { get; set; }

        #endregion

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        public override DateTime ModifiedDate { get; set; }

        #endregion

        #endregion
    }
}