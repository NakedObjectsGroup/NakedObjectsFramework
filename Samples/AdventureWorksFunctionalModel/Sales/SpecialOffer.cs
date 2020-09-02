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
    public class SpecialOffer: IHasRowGuid, IHasModifiedDate {

        public SpecialOffer()
        {

        }

        public SpecialOffer(
            int specialOfferID,
            string description,
            decimal discountPct,
            string type,
            string category,
            DateTime startDate,
            DateTime endDate,
            int minQty,
            int? maxQty,
            DateTime modifiedDate,
            Guid rowGuid
            )
        {
            SpecialOfferID = specialOfferID;
            Description = description;
            DiscountPct = discountPct;
            Type = type;
            Category = category;
            StartDate = startDate;
            EndDate = endDate;
            MinQty = minQty;
            MaxQty = maxQty;
            ModifiedDate = modifiedDate;
            rowguid = rowGuid;
        }

        [NakedObjectsIgnore]
        public virtual int SpecialOfferID { get; set; }

        [MemberOrder(10)]
        public virtual string Description { get; set; }

        [MemberOrder(20)]
        [Mask("P")]
        public virtual decimal DiscountPct { get; set; }

        [MemberOrder(30)]
        public virtual string Type { get; set; }

        [MemberOrder(40)]
        public virtual string Category { get; set; }

        [MemberOrder(51)]
        [Mask("d")]
        public virtual DateTime StartDate { get; set; }

        [MemberOrder(52)]
        [Mask("d")]
        public virtual DateTime EndDate { get; set; }

        [MemberOrder(61)]
        public virtual int MinQty { get; set; }

        [Optionally]
        [MemberOrder(62)]
        public virtual int? MaxQty { get; set; }


        #region ModifiedDate and rowguid

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        [ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion

        #region rowguid

        [NakedObjectsIgnore]
        public virtual Guid rowguid { get; set; }

        #endregion

        #endregion
    }

    public static class SpecialOfferFunctions
    {

        public static string Title(SpecialOffer sp)
        {
            return sp.Description;
        }

        public static string IconName(SpecialOffer sp)
        {
            return sp.Type == "No Discount"? "default.png":"scissors.png";
        }


        #region Life Cycle Methods
        public static SpecialOffer Updating(
            SpecialOffer sp,
            [Injected] DateTime now)
        {
            return sp.With(x => x.ModifiedDate, now);
        }
        #endregion

        public static string[] ChoicesCategory(SpecialOffer sp)
        {
            return new[] { "Reseller", "Customer" };
        }

        public static DateTime DefaultStartDate(
            SpecialOffer sp,
            [Injected] DateTime now)
        {
            return now;
        }

        public static DateTime DefaultEndDate(
            SpecialOffer sp,
            [Injected] DateTime now)
        {
            return now.AddDays(90);
        }
    }
}