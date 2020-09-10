// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedFunctions;
using static NakedFunctions.Helpers;
using static System.Convert;

namespace AdventureWorksModel
{


    public static class SpecialOffer_EditFunctions
    {
        //WRITE A SNIPPET FOR THIS!

        #region Life Cycle Methods
        public static string[] DeriveKeys(this SpecialOffer_Edit edit)
        => new string[]
        {
                    edit.Category,
                    edit.Description,
                    edit.DiscountPct.ToString(),
                    edit.EndDate.ToString("d"),
                    edit.MaxQty.ToString(),
                    edit.MinQty.ToString(),
                    edit.SpecialOfferID.ToString(),
                    edit.StartDate.ToString("d"),
                    edit.Type
        };
        public static SpecialOffer_Edit PopulateUsingKeys(string[] k)
        => new SpecialOffer_Edit
        {
            Category = k[0],
            Description = k[1],
            DiscountPct = ToInt32(k[2]),
            EndDate = ToDateTime(k[3]),
            MaxQty = ToInt32(k[4]),
            MinQty = ToInt32(k[5]),
            SpecialOfferID = ToInt32(k[6]),
            StartDate = ToDateTime(k[7]),
            Type = k[8]
        };

        public static SpecialOffer_Edit CreateFrom(SpecialOffer x)
        => new SpecialOffer_Edit
        {
            Category = x.Category,
            Description = x.Description,
            DiscountPct = x.DiscountPct,
            EndDate = x.EndDate,
            MaxQty = x.MaxQty,
            MinQty = x.MinQty,
            SpecialOfferID = x.SpecialOfferID,
            StartDate = x.StartDate,
            Type = x.Type
        };

        public static (SpecialOffer, SpecialOffer) Save(this SpecialOffer_Edit x)
        => DisplayAndPersist(new SpecialOffer()
        {
            Category = x.Category,
            Description = x.Description,
            DiscountPct = x.DiscountPct,
            EndDate = x.EndDate,
            MaxQty = x.MaxQty,
            MinQty = x.MinQty,
            SpecialOfferID = x.SpecialOfferID,
            StartDate = x.StartDate,
            Type = x.Type
        });
        #endregion

        #region Property-associated functions
        public static string[] ChoicesCategory(this SpecialOffer sp) => new[] { "Reseller", "Customer" };

        public static DateTime DefaultStartDate(this SpecialOffer sp, [Injected] DateTime now) => now;

        public static DateTime DefaultEndDate(this SpecialOffer sp, [Injected] DateTime now) => now.AddDays(90);
        #endregion
    }
}