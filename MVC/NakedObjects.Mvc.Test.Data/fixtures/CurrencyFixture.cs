// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using Expenses.Currencies;
using NakedObjects;

namespace Expenses.Fixtures {
    public class CurrencyFixture  {

        public IDomainObjectContainer Container { protected get; set; }


        public static Currency EUR;
        public static Currency GBP;
        public static Currency USD;

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