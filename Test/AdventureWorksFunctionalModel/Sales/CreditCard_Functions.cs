// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using AW.Types;
using NakedFunctions;

namespace AW.Functions {
        public static class CreditCard_Functions {
        
        internal static string ObfuscatedNumber(CreditCard cc) =>
            cc.CardNumber != null && cc.CardNumber.Length > 4 ?
               cc.CardNumber.Substring(cc.CardNumber.Length - 4).PadLeft(cc.CardNumber.Length, '*')
               : null;

        public static string Validate(this CreditCard cc, byte expMonth, short expYear, IContext context) {
            if (expMonth == 0 || expYear == 0) {
                return null;
            }
            DateTime today = context.Today();
            DateTime expiryDate = new DateTime(expYear, expMonth, 1); //.EndOfMonth();
            if (expiryDate <= today) {
                return "Expiry date must be in the future";
            }
            return null;
        }


    }
}