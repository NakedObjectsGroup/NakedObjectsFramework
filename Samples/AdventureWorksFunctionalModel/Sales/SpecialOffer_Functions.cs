// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedFunctions;

namespace AdventureWorksModel {


    public static class SpecialOffer_Functions
    {

        #region Life Cycle Methods
        public static SpecialOffer Updating(SpecialOffer sp, [Injected] DateTime now) => sp with {ModifiedDate =  now};
        #endregion

        public static string[] ChoicesCategory(SpecialOffer sp) =>  new[] { "Reseller", "Customer" };

        public static DateTime DefaultStartDate(SpecialOffer sp, [Injected] DateTime now) => now;

        public static DateTime DefaultEndDate(SpecialOffer sp, [Injected] DateTime now) => now.AddDays(90);
    }
}