// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel.DataAnnotations;
using NakedFunctions;
using NakedObjects;

namespace AdventureWorksModel {
    [IconName("information")]
    public class ProductDescription: IHasRowGuid, IHasModifiedDate {
        public ProductDescription(
            int productDescriptionID,
            string description,
            Guid rowguid,
            DateTime modifiedDate)
        {
            ProductDescriptionID = productDescriptionID;
            Description = description;
            this.rowguid = rowguid;
            ModifiedDate = modifiedDate;
        }
        public ProductDescription() { }

       [NakedObjectsIgnore]
        public virtual int ProductDescriptionID { get; set; }

        [Title]
        [MultiLine(NumberOfLines = 10)]
        [TypicalLength(100)]
        [MemberOrder(2)]
        public virtual string Description { get; set; }

        #region Row Guid and Modified Date

        #region rowguid

        [NakedObjectsIgnore]
        public virtual Guid rowguid { get; set; }

        #endregion

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        [ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion

        #endregion
    }

    public static class ProductDescriptionFunctions
    {
        public static string Title(this ProductDescription pd)
        {
            return pd.CreateTitle(pd.Description);
        }
        public static ProductDescription Updating(ProductDescription a, [Injected] DateTime now)
        {
            return a.With(x => x.ModifiedDate, now);
        }
    }
}