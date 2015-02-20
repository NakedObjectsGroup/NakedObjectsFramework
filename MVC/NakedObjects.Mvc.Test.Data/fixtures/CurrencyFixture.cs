// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Expenses.Currencies;
using NakedObjects;

namespace Expenses.Fixtures {
    public class CurrencyFixture {
        public static Currency EUR;
        public static Currency GBP;
        public static Currency USD;
        public IDomainObjectContainer Container { protected get; set; }

        public void Install() {
            EUR = CreateCurrency("EUR", "Euro Member Countries", "Euro");
            GBP = CreateCurrency("GBP", "United Kingdom", "Pounds");
            USD = CreateCurrency("USD", "United States of America", "Dollars");
        }

        private Currency CreateCurrency(string code, string country, string name) {
            var curr = Container.NewTransientInstance<Currency>();
            curr.CurrencyCode = code;
            curr.CurrencyCountry = country;
            curr.CurrencyName = name;
            Container.Persist(ref curr);
            return curr;
        }
    }
}