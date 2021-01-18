// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;


using System.Linq;
using System.Security.Cryptography;
using System.Text;
using NakedFunctions;
using AW.Types;
using System.Collections.Generic;

namespace AW.Functions {

    public static class Person_Functions
    {


        #region ChangePassword (Action)

        
        [MemberOrder(1)]
        public static (Person, IContext) ChangePassword(this Person p, [Password] string oldPassword, [Password] string newPassword, [Named("New Password (Confirm)"), Password] string confirm, IContext context)
        {
              Password pw = Password_Functions.CreateNewPassword(newPassword, p.BusinessEntityID, context);
            var p2 = p with { Password = pw, ModifiedDate = context.Now()};
            return (p2, context.WithPendingSave(p2, pw));
        }

        public static string ValidateChangePassword(this Person p, string oldPassword, string newPassword, string confirm)
        {
            var reason = "";
            if (!p.Password.OfferedPasswordIsCorrect(oldPassword))
            {
                reason += "Old Password is incorrect";
            }
            if (newPassword != confirm)
            {
                reason += "New Password and Confirmation don't match";
            }
            if (newPassword.Length < 6)
            {
                reason += "New Password must be at least 6 characters";
            }
            if (newPassword == oldPassword)
            {
                reason += "New Password should be different from Old Password";
            }
            return reason;
        }
        #endregion


       

        #region Actions for test purposes only

        public static (Person, Person) UpdateMiddleName(this Person p, string newName)
        {
            var p1 = p with {MiddleName =  newName};
            return (p1, p1);
        }

        public static (Person, Person) UpdateSuffix(this Person p, string newSuffix)
        {
            var p1 = p with {Suffix =  newSuffix};
            return (p1, p1);
        }

        //To test a ViewModelEdit
        public static EmailTemplate CreateEmail(this Person p)
        {
            return new EmailTemplate("", "", "", "",EmailStatus.New);
        }

        public static object CreateLetter(this Person p,Address toAddress)
        {
            //No real implementation; function is for framework testing purposes only
            return null;
        }

        public static Address Default0CreateLetter(this Person p)
        {
            return p.Addresses.First().Address;
        }
        #endregion

        #region CreditCards

        //TODO: param validation. choices for type
        public static (Person, IContext context) CreateNewCreditCard(
            this Person p, 
            string cardType,
            [DescribedAs("No spaces")] string cardNumber,
            [DescribedAs("mm/yy")] string expires,
            IContext context
            )
        {
            byte expMonth = Convert.ToByte(expires.Substring(0, 2));
            byte expYear = Convert.ToByte(expires.Substring(3, 2));
            var cc = new CreditCard() { CardType = cardType, CardNumber = cardNumber, ExpMonth = expMonth, ExpYear = expYear };
            var link = new PersonCreditCard() { CreditCard = cc, Person = p };
            return (p, context.WithPendingSave(cc, link));                
        }

        public static IList<CreditCard> ListCreditCards(this Person p, IQueryable<PersonCreditCard> pccs)
        {
            int id = p.BusinessEntityID;
            return pccs.Where(pcc => pcc.PersonID == id).Select(pcc => pcc.CreditCard).ToList();
        }

        public static IQueryable<CreditCard> RecentCreditCards(this Person p, IQueryable<PersonCreditCard> pccs)
        {
            int id = p.BusinessEntityID;
            return pccs.Where(pcc => pcc.PersonID == id).Select(pcc => pcc.CreditCard).OrderByDescending(cc => cc.ModifiedDate);
        }

        #endregion

        //Yes, this could be done with a Hidden attribute, but is done as a function here for testing.
        public static bool HideContacts(this Person p)
        {
            return true;
        }


        public static (Person, IContext) CreateNewPhoneNumber(this Person p, PhoneNumberType type,
    [RegEx(@"^([0-9][0-9\s-]+)$")] string phoneNumber, IContext context)
            => (p, context.WithPendingSave(new PersonPhone()
            {
                BusinessEntityID = p.BusinessEntityID,
                PhoneNumberType = type,
                PhoneNumber = phoneNumber
            }));
    }
}