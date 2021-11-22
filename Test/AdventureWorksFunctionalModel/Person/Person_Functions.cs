// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using AW.Types;
using NakedFunctions;

namespace AW.Functions {
    public static class Person_Functions {
        internal static IContext UpdatePerson(
            Person original, Person updated, IContext context) =>
            context.WithUpdated(original, new Person(original) { ModifiedDate = context.Now() });

        public static IContext EditNameFields(this Person p,
                                       [Optionally] string title,
                                       string firstName,
                                       [Optionally] string middleName,
                                       string lastName,
                                       [Optionally] string suffix,
                                       [Named("Reverse name order")] bool nameStyle,
                                       IContext context) =>
           UpdatePerson(p, new Person(p)
           {
               Title = title,
               FirstName = firstName,
               MiddleName = middleName,
               LastName = lastName,
               Suffix = suffix,
               NameStyle = nameStyle
           }, context);

        [Edit]
        public static IContext EditName(this Person p,
                                        [Optionally] string title,
                                        string firstName,
                                        [Optionally] string middleName,
                                        string lastName,
                                        [Optionally] string suffix,
                                        [Named("Reverse name order")] bool nameStyle,
                                        IContext context) =>
            UpdatePerson(p, new Person(p) {
                Title = title,
                FirstName = firstName,
                MiddleName = middleName,
                LastName = lastName,
                Suffix = suffix,
                NameStyle = nameStyle
            }, context);

        //Yes, this could be done with a Hidden attribute, but is done as a function here for testing.
        public static bool HideContacts(this Person p) => true;

        public static IContext CreateNewPhoneNumber(this Person p,
                                                    PhoneNumberType type,
                                                    [RegEx(@"^([0-9][0-9\s-]+)$")] string phoneNumber,
                                                    IContext context)
            => context.WithNew(new PersonPhone {
                BusinessEntityID = p.BusinessEntityID,
                PhoneNumberType = type,
                PhoneNumber = phoneNumber
            });

        #region CreditCards

        #region CreateNewCreditCard

        public static IContext CreateNewCreditCard(this Person p,
                                                   string cardType,
                                                   [RegEx("^[0-9]{16}$")] [DescribedAs("No spaces")]
                                                   string cardNumber,
                                                   [RegEx("^[0-9]{2}/[0-9]{2}")] [DescribedAs("mm/yy")]
                                                   string expires,
                                                   IContext context
        ) {
            var expMonth = Convert.ToByte(expires.Substring(0, 2));
            var expYear = Convert.ToByte(expires.Substring(3, 2));
            var cc = new CreditCard { CardType = cardType, CardNumber = cardNumber, ExpMonth = expMonth, ExpYear = expYear };
            var link = new PersonCreditCard { CreditCard = cc, Person = p };
            return context.WithNew(cc).WithNew(link);
        }

        public static IList<string> Choices1CreateNewCreditCard(this Person p) =>
            new[] { "Vista", "Distinguish", "SuperiorCard", "ColonialVoice" };

        #endregion

        public static IList<CreditCard> ListCreditCards(this Person p, IQueryable<PersonCreditCard> pccs) {
            var id = p.BusinessEntityID;
            return pccs.Where(pcc => pcc.PersonID == id).Select(pcc => pcc.CreditCard).ToList();
        }

        public static IQueryable<CreditCard> RecentCreditCards(this Person p, IQueryable<PersonCreditCard> pccs) {
            var id = p.BusinessEntityID;
            return pccs.Where(pcc => pcc.PersonID == id).Select(pcc => pcc.CreditCard).OrderByDescending(cc => cc.ModifiedDate);
        }

        #endregion
    }
}