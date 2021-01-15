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
        #region Life Cycle Methods
        public static BusinessEntityContact Persisted(Person p)
        {
            return new BusinessEntityContact()
            {
                //TODO:
                //p.ForEntity.BusinessEntityID,
                //p,
                //p.BusinessEntityID,
                //p,
                //p.ContactType.ContactTypeID,
                //p.ContactType,
                //guid,
                //now
            };
        }

        //TODO
        public static Person Persisting(Person p,  IContext context)
        {
            throw new NotImplementedException();
            ////return Updating(p, now) with {rowguid =  guid).CreateSaltAndHash(p.InitialPassword}
            ////     with {BusinessEntityRowguid =  guid}
            ////     with {BusinessEntityModifiedDate =  now};
        }

        //TODO
        public static Person Updating(Person x,  IContext context) => 
         x with {BusinessEntityModifiedDate =  context.Now(), ModifiedDate = context.Now()};


        #endregion

        #region ChangePassword (Action)

        
        [MemberOrder(1)]
        public static (Person, IContext) ChangePassword(this Person p, [Password] string oldPassword, [Password] string newPassword, [Named("New Password (Confirm)"), Password] string confirm, IContext context)
        {
            var pw = new Password()
            {
                BusinessEntityID = p.BusinessEntityID,
                PasswordHash = Hashed(newPassword, p.Password.PasswordSalt),
                PasswordSalt = CreateRandomSalt()
            };
            var p2 = p with { Password = pw };
            return (p2, context.WithPendingSave(p2, pw));
        }

        public static string ValidateChangePassword(this Person p, string oldPassword, string newPassword, string confirm)
        {
            var reason = "";
            if (Hashed(oldPassword, p.Password.PasswordSalt) != p.Password.PasswordHash)
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


        private static string Hashed(this string password, string salt)
        {
            string saltedPassword = password + salt;
            byte[] data = Encoding.UTF8.GetBytes(saltedPassword);
            byte[] hash = SHA256.Create().ComputeHash(data);
            char[] chars = Encoding.UTF8.GetChars(hash);
            return new string(chars);
        }

        private static string CreateRandomSalt()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var output = new StringBuilder();
            for (int i = 1; i <= 7; i++)
            {
                output.Append(chars.Substring(random.Next(chars.Length - 1), 1));
            }
            output.Append("=");
            return output.ToString();
        }

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

        //TODO: This must be changed to request all fields & return object to be persisted.
        public static (CreditCard, IContext context) CreateNewCreditCard(this Person p)
        {
            throw new NotImplementedException();
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

        // regex doesn't work!

    //    public static (Person, PersonPhone) CreateNewPhoneNumber(this Person p, PhoneNumberType type,
    //[RegEx(@"[0-9][0-9\s-]+")] string phoneNumber)
    //        => (p, new PersonPhone()
    //        {
    //            BusinessEntityID = p.BusinessEntityID,
    //            PhoneNumberType = type,
    //            PhoneNumber = phoneNumber
    //        });
    }
}